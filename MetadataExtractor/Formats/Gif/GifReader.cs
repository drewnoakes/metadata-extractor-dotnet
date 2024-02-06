// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Xmp;

namespace MetadataExtractor.Formats.Gif
{
    /// <summary>Reader of GIF encoded data.</summary>
    /// <remarks>
    /// Resources:
    /// <list type="bullet">
    ///   <item>https://wiki.whatwg.org/wiki/GIF</item>
    ///   <item>https://www.w3.org/Graphics/GIF/spec-gif89a.txt</item>
    ///   <item>http://web.archive.org/web/20100929230301/http://www.etsimo.uniovi.es/gifanim/gif87a.txt</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class GifReader
    {
        private const string Gif87AVersionIdentifier = "87a";
        private const string Gif89AVersionIdentifier = "89a";

        public IReadOnlyList<Directory> Extract(SequentialReader reader)
        {
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            var directories = new List<Directory>();
            try
            {
                directories.AddRange(ReadGifDirectoriesInternal(reader));
            }
            catch (IOException)
            {
                directories.Add(new ErrorDirectory("IOException processing GIF data"));
            }
            return directories;
        }

        /// <summary>This method exists because generator methods cannot yield in try/catch blocks.</summary>
        private static IEnumerable<Directory> ReadGifDirectoriesInternal(SequentialReader reader)
        {
            var header = ReadGifHeader(reader);

            yield return header;

            if (header.HasError)
                yield break;

            if (header.TryGetBoolean(GifHeaderDirectory.TagHasGlobalColorTable, out bool hasGlobalColorTable))
            {
                // Skip over any global colour table
                if (hasGlobalColorTable && header.TryGetInt32(GifHeaderDirectory.TagColorTableSize, out int globalColorTableSize))
                {
                    // Colour table has R/G/B byte triplets
                    reader.Skip(3 * globalColorTableSize);
                }
            }
            else
            {
                yield return new ErrorDirectory("GIF did not have hasGlobalColorTable bit.");
            }

            // After the header comes a sequence of blocks
            while (true)
            {
                byte marker;
                try
                {
                    marker = reader.GetByte();
                }
                catch (IOException)
                {
                    yield break;
                }

                switch (marker)
                {
                    case (byte)'!': // 0x21
                    {
                        var extBlock = ReadGifExtensionBlock(reader);
                        if (extBlock is not null) yield return extBlock;
                        break;
                    }
                    case (byte)',': // 0x2c
                    {
                        var imageBlock = ReadImageBlock(reader);
                        if (imageBlock is not null) yield return imageBlock;

                        // skip image data blocks
                        SkipBlocks(reader);

                        break;
                    }
                    case (byte)';': // 0x3b
                    {
                        // terminator
                        yield break;
                    }
                    default:
                    {
                        // Anything other than these types is unexpected.
                        // GIF87a spec says to keep reading until a separator is found.
                        // GIF89a spec says file is corrupt.
                        yield return new ErrorDirectory("Unknown GIF block marker found.");
                        yield break;
                    }
                }
            }
        }

        /// <summary>Reads the fixed-position GIF header.</summary>
        private static GifHeaderDirectory ReadGifHeader(SequentialReader reader)
        {
            // FILE HEADER
            //
            // 3 - signature: "GIF"
            // 3 - version: either "87a" or "89a"
            //
            // LOGICAL SCREEN DESCRIPTOR
            //
            // 2 - pixel width
            // 2 - pixel height
            // 1 - screen and color map information flags (0 is LSB)
            //       0-2  Size of the global color table
            //       3    Color table sort flag (89a only)
            //       4-6  Color resolution
            //       7    Global color table flag
            // 1 - background color index
            // 1 - pixel aspect ratio

            var headerDirectory = new GifHeaderDirectory();

            Span<byte> signature = stackalloc byte[3];
            reader.GetBytes(signature);

            if (!signature.SequenceEqual("GIF"u8))
            {
                headerDirectory.AddError("Invalid GIF file signature");
                return headerDirectory;
            }

            var version = reader.GetString(3, Encoding.UTF8);
            if (version != Gif87AVersionIdentifier && version != Gif89AVersionIdentifier)
            {
                headerDirectory.AddError($"Unexpected GIF version \"{version}\"");
                return headerDirectory;
            }

            headerDirectory.Set(GifHeaderDirectory.TagGifFormatVersion, version);

            // LOGICAL SCREEN DESCRIPTOR

            headerDirectory.Set(GifHeaderDirectory.TagImageWidth, reader.GetUInt16());
            headerDirectory.Set(GifHeaderDirectory.TagImageHeight, reader.GetUInt16());

            var flags = reader.GetByte();

            // First three bits = (BPP - 1)
            var colorTableSize = 1 << ((flags & 7) + 1);
            var bitsPerPixel = ((flags & 0x70) >> 4) + 1;
            var hasGlobalColorTable = (flags >> 7) != 0;

            headerDirectory.Set(GifHeaderDirectory.TagColorTableSize, colorTableSize);

            if (version == Gif89AVersionIdentifier)
            {
                var isColorTableSorted = (flags & 8) != 0;
                headerDirectory.Set(GifHeaderDirectory.TagIsColorTableSorted, isColorTableSorted);
            }

            headerDirectory.Set(GifHeaderDirectory.TagBitsPerPixel, bitsPerPixel);
            headerDirectory.Set(GifHeaderDirectory.TagHasGlobalColorTable, hasGlobalColorTable);

            headerDirectory.Set(GifHeaderDirectory.TagBackgroundColorIndex, reader.GetByte());

            int aspectRatioByte = reader.GetByte();
            if (aspectRatioByte != 0)
            {
                var pixelAspectRatio = (float)((aspectRatioByte + 15d) / 64d);
                headerDirectory.Set(GifHeaderDirectory.TagPixelAspectRatio, pixelAspectRatio);
            }

            return headerDirectory;
        }

        private static Directory? ReadGifExtensionBlock(SequentialReader reader)
        {
            var extensionLabel = reader.GetByte();
            var blockSizeBytes = reader.GetByte();
            var blockStartPos = reader.Position;

            Directory? directory = extensionLabel switch
            {
                0x01 => ReadPlainTextBlock(reader, blockSizeBytes),
                0xf9 => ReadControlBlock(reader),
                0xfe => ReadCommentBlock(reader, blockSizeBytes),
                0xff => ReadApplicationExtensionBlock(reader, blockSizeBytes),
                _ => new ErrorDirectory($"Unsupported GIF extension block with type 0x{extensionLabel:X2}.")
            };

            var skipCount = blockStartPos + blockSizeBytes - reader.Position;
            if (skipCount > 0)
                reader.Skip(skipCount);

            return directory;
        }

        private static Directory? ReadPlainTextBlock(SequentialReader reader, byte blockSizeBytes)
        {
            // It seems this extension is deprecated. If somebody finds an image with this in it, could implement here.
            // Just skip the entire block for now.

            if (blockSizeBytes != 12)
                return new ErrorDirectory($"Invalid GIF plain text block size. Expected 12, got {blockSizeBytes}.");

            // skip 'blockSizeBytes' bytes
            reader.Skip(12);

            // keep reading and skipping until a 0 byte is reached
            SkipBlocks(reader);

            return null;
        }

        private static GifCommentDirectory ReadCommentBlock(SequentialReader reader, byte blockSizeBytes)
        {
            var buffer = GatherBytes(reader, blockSizeBytes);
            return new GifCommentDirectory(new StringValue(buffer, Encoding.ASCII));
        }

        private static Directory? ReadApplicationExtensionBlock(SequentialReader reader, byte blockSizeBytes)
        {
            if (blockSizeBytes != 11)
                return new ErrorDirectory($"Invalid GIF application extension block size. Expected 11, got {blockSizeBytes}.");

            Span<byte> extensionType = stackalloc byte[11];

            reader.GetBytes(extensionType);

            if (extensionType.SequenceEqual("XMP DataXMP"u8))
            {
                // XMP data extension
                var xmpBytes = GatherXmpBytes(reader);
                return xmpBytes is not null
                    ? new XmpReader().Extract(xmpBytes)
                    : null;
            }
            else if (extensionType.SequenceEqual("ICCRGBG1012"u8))
            {
                // ICC profile extension
                var iccBytes = GatherBytes(reader, reader.GetByte());
                return iccBytes.Length != 0
                    ? new IccReader().Extract(iccBytes)
                    : null;
            }
            else if (extensionType.SequenceEqual("NETSCAPE2.0"u8))
            {
                reader.Skip(2);
                // Netscape's animated GIF extension
                // Iteration count (0 means infinite)
                var iterationCount = reader.GetUInt16();
                // Skip terminator
                reader.Skip(1);
                var animationDirectory = new GifAnimationDirectory();
                animationDirectory.Set(GifAnimationDirectory.TagIterationCount, iterationCount);
                return animationDirectory;
            }
            else
            {
                SkipBlocks(reader);
                return null;
            }
        }

        private static GifControlDirectory ReadControlBlock(SequentialReader reader)
        {
            var directory = new GifControlDirectory();

            byte packedFields = reader.GetByte();
            directory.Set(GifControlDirectory.TagDisposalMethod, (packedFields >> 2) & 7);
            directory.Set(GifControlDirectory.TagUserInputFlag, (packedFields & 2) == 2);
            directory.Set(GifControlDirectory.TagTransparentColorFlag, (packedFields & 1) == 1);
            directory.Set(GifControlDirectory.TagDelay, reader.GetUInt16());
            directory.Set(GifControlDirectory.TagTransparentColorIndex, reader.GetByte());

            // skip 0x0 block terminator
            reader.Skip(1);

            return directory;
        }

        private static GifImageDirectory ReadImageBlock(SequentialReader reader)
        {
            var imageDirectory = new GifImageDirectory();

            imageDirectory.Set(GifImageDirectory.TagLeft, reader.GetUInt16());
            imageDirectory.Set(GifImageDirectory.TagTop, reader.GetUInt16());
            imageDirectory.Set(GifImageDirectory.TagWidth, reader.GetUInt16());
            imageDirectory.Set(GifImageDirectory.TagHeight, reader.GetUInt16());

            var flags = reader.GetByte();
            var hasColorTable = (flags >> 7) != 0;
            var isInterlaced = (flags & 0x40) != 0;

            imageDirectory.Set(GifImageDirectory.TagHasLocalColourTable, hasColorTable);
            imageDirectory.Set(GifImageDirectory.TagIsInterlaced, isInterlaced);

            if (hasColorTable)
            {
                var isColorTableSorted = (flags & 0x20) != 0;
                imageDirectory.Set(GifImageDirectory.TagIsColorTableSorted, isColorTableSorted);

                var bitsPerPixel = (flags & 0x7) + 1;
                imageDirectory.Set(GifImageDirectory.TagLocalColourTableBitsPerPixel, bitsPerPixel);

                // skip color table
                reader.Skip(3 * (2 << (flags & 0x7)));
            }

            // skip "LZW Minimum Code Size" byte
            reader.GetByte();

            return imageDirectory;
        }

        #region Utility methods

        private static byte[]? GatherXmpBytes(SequentialReader reader)
        {
            // GatherXmpBytes differs from GatherBytes in that this method includes the "length"
            // bytes in its output.

            var stream = new MemoryStream();
            var buffer = ArrayPool<byte>.Shared.Rent(byte.MaxValue + 1);

            while (true)
            {
                var len = reader.GetByte();
                if (len == 0)
                    break;
                buffer[0] = len;
                reader.GetBytes(buffer, offset: 1, count: len);
                stream.Write(buffer, 0, len + 1);
            }

            ArrayPool<byte>.Shared.Return(buffer);

            // Exclude the "magic trailer", see XMP Specification Part 3, 1.1.2 GIF
            int xmpLength = checked((int)stream.Length) - 257;

            if (xmpLength <= 0)
            {
                return null;
            }

            stream.SetLength(xmpLength);

            return stream.ToArray();
        }

        private static byte[] GatherBytes(SequentialReader reader, byte firstLength)
        {
            var stream = new MemoryStream();
            var buffer = ArrayPool<byte>.Shared.Rent(byte.MaxValue);

            var length = firstLength;

            while (length > 0)
            {
                reader.GetBytes(buffer.AsSpan().Slice(0, length));

                stream.Write(buffer, 0, length);

                length = reader.GetByte();
            }

            ArrayPool<byte>.Shared.Return(buffer);

            return stream.ToArray();
        }

        private static void SkipBlocks(SequentialReader reader)
        {
            while (true)
            {
                var length = reader.GetByte();

                if (length == 0)
                    return;

                reader.Skip(length);
            }
        }

        #endregion
    }
}
