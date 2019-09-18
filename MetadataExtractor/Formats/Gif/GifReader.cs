// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

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

        public DirectoryList Extract(SequentialReader reader)
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
                        if (extBlock != null) yield return extBlock;
                        break;
                    }
                    case (byte)',': // 0x2c
                    {
                        var imageBlock = ReadImageBlock(reader);
                        if (imageBlock != null) yield return imageBlock;

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

            var signature = reader.GetString(3, Encoding.UTF8);
            if (signature != "GIF")
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

            Directory? directory;
            switch (extensionLabel)
            {
                case 0x01:
                {
                    directory = ReadPlainTextBlock(reader, blockSizeBytes);
                    break;
                }
                case 0xf9:
                {
                    directory = ReadControlBlock(reader, blockSizeBytes);
                    break;
                }
                case 0xfe:
                {
                    directory = ReadCommentBlock(reader, blockSizeBytes);
                    break;
                }
                case 0xff:
                {
                    directory = ReadApplicationExtensionBlock(reader, blockSizeBytes);
                    break;
                }
                default:
                {
                    directory = new ErrorDirectory($"Unsupported GIF extension block with type 0x{extensionLabel:X2}.");
                    break;
                }
            }

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

            var extensionType = reader.GetString(blockSizeBytes, Encoding.UTF8);

            switch (extensionType)
            {
                case "XMP DataXMP":
                {
                    // XMP data extension
                    var xmpBytes = GatherBytes(reader);
                    int xmpLength = xmpBytes.Length - 257; // Exclude the "magic trailer", see XMP Specification Part 3, 1.1.2 GIF
                    // Only extract valid blocks
                    return xmpLength > 0
                        ? new XmpReader().Extract(xmpBytes, 0, xmpBytes.Length - 257)
                        : null;
                }
                case "ICCRGBG1012":
                {
                    // ICC profile extension
                    var iccBytes = GatherBytes(reader, reader.GetByte());
                    return iccBytes.Length != 0
                        ? new IccReader().Extract(new ByteArrayReader(iccBytes))
                        : null;
                }
                case "NETSCAPE2.0":
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
                default:
                {
                    SkipBlocks(reader);
                    return null;
                }
            }
        }

        private static GifControlDirectory ReadControlBlock(SequentialReader reader, byte blockSizeBytes)
        {
            if (blockSizeBytes < 4)
                blockSizeBytes = 4;

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

        private static byte[] GatherBytes(SequentialReader reader)
        {
            var bytes = new MemoryStream();
            var buffer = new byte[257];

            while (true)
            {
                var b = reader.GetByte();
                if (b == 0)
                    return bytes.ToArray();
                buffer[0] = b;
                reader.GetBytes(buffer, 1, b);
                bytes.Write(buffer, 0, b + 1);
            }
        }

        private static byte[] GatherBytes(SequentialReader reader, int firstLength)
        {
            var buffer = new MemoryStream();

            var length = firstLength;

            while (length > 0)
            {
                buffer.Write(reader.GetBytes(length), 0, length);

                length = reader.GetByte();
            }

            return buffer.ToArray();
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
