// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.IO.Compression;

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using MetadataExtractor.Util;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class PngMetadataReader
    {
        private static readonly HashSet<PngChunkType> _desiredChunkTypes = new HashSet<PngChunkType>
        {
            PngChunkType.IHDR,
            PngChunkType.PLTE,
            PngChunkType.tRNS,
            PngChunkType.cHRM,
            PngChunkType.sRGB,
            PngChunkType.gAMA,
            PngChunkType.iCCP,
            PngChunkType.bKGD,
            PngChunkType.tEXt,
            PngChunkType.zTXt,
            PngChunkType.iTXt,
            PngChunkType.tIME,
            PngChunkType.pHYs,
            PngChunkType.sBIT
        };

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static DirectoryList ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static DirectoryList ReadMetadata(Stream stream)
        {
            List<Directory>? directories = null;

            var chunks = new PngChunkReader().Extract(new SequentialStreamReader(stream), _desiredChunkTypes);

            foreach (var chunk in chunks)
            {
                if(directories == null)
                    directories = new List<Directory>();

                try
                {
                    directories.AddRange(ProcessChunk(chunk));
                }
                catch (Exception ex)
                {
                    directories.Add(new ErrorDirectory("Exception reading PNG chunk: " + ex.Message));
                }
            }

            return directories ?? Directory.EmptyList;
        }

        /// <summary>
        /// The PNG spec states that ISO_8859_1 (Latin-1) encoding should be used for:
        /// <list type="bullet">
        ///   <item>"tEXt" and "zTXt" chunks, both for keys and values (https://www.w3.org/TR/PNG/#11tEXt)</item>
        ///   <item>"iCCP" chunks, for the profile name (https://www.w3.org/TR/PNG/#11iCCP)</item>
        ///   <item>"sPLT" chunks, for the palette name (https://www.w3.org/TR/PNG/#11sPLT)</item>
        /// </list>
        /// Note that "iTXt" chunks use UTF-8 encoding (https://www.w3.org/TR/PNG/#11iTXt).
        /// <para/>
        /// For more guidance: http://www.w3.org/TR/PNG-Decoders.html#D.Text-chunk-processing
        /// </summary>
        private static readonly Encoding _latin1Encoding = Encoding.GetEncoding("ISO-8859-1");

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        private static IEnumerable<Directory> ProcessChunk(PngChunk chunk)
        {
            var chunkType = chunk.ChunkType;
            var bytes = chunk.Bytes;

            if (chunkType == PngChunkType.IHDR)
            {
                var header = new PngHeader(bytes);
                var directory = new PngDirectory(PngChunkType.IHDR);
                directory.Set(PngDirectory.TagImageWidth, header.ImageWidth);
                directory.Set(PngDirectory.TagImageHeight, header.ImageHeight);
                directory.Set(PngDirectory.TagBitsPerSample, header.BitsPerSample);
                directory.Set(PngDirectory.TagColorType, header.ColorType.NumericValue);
                directory.Set(PngDirectory.TagCompressionType, header.CompressionType);
                directory.Set(PngDirectory.TagFilterMethod, header.FilterMethod);
                directory.Set(PngDirectory.TagInterlaceMethod, header.InterlaceMethod);
                yield return directory;
            }
            else if (chunkType == PngChunkType.PLTE)
            {
                var directory = new PngDirectory(PngChunkType.PLTE);
                directory.Set(PngDirectory.TagPaletteSize, bytes.Length / 3);
                yield return directory;
            }
            else if (chunkType == PngChunkType.tRNS)
            {
                var directory = new PngDirectory(PngChunkType.tRNS);
                directory.Set(PngDirectory.TagPaletteHasTransparency, 1);
                yield return directory;
            }
            else if (chunkType == PngChunkType.sRGB)
            {
                int srgbRenderingIntent = unchecked((sbyte)bytes[0]);
                var directory = new PngDirectory(PngChunkType.sRGB);
                directory.Set(PngDirectory.TagSrgbRenderingIntent, srgbRenderingIntent);
                yield return directory;
            }
            else if (chunkType == PngChunkType.cHRM)
            {
                var chromaticities = new PngChromaticities(bytes);
                var directory = new PngChromaticitiesDirectory();
                directory.Set(PngChromaticitiesDirectory.TagWhitePointX, chromaticities.WhitePointX);
                directory.Set(PngChromaticitiesDirectory.TagWhitePointY, chromaticities.WhitePointY);
                directory.Set(PngChromaticitiesDirectory.TagRedX, chromaticities.RedX);
                directory.Set(PngChromaticitiesDirectory.TagRedY, chromaticities.RedY);
                directory.Set(PngChromaticitiesDirectory.TagGreenX, chromaticities.GreenX);
                directory.Set(PngChromaticitiesDirectory.TagGreenY, chromaticities.GreenY);
                directory.Set(PngChromaticitiesDirectory.TagBlueX, chromaticities.BlueX);
                directory.Set(PngChromaticitiesDirectory.TagBlueY, chromaticities.BlueY);
                yield return directory;
            }
            else if (chunkType == PngChunkType.gAMA)
            {
                var gammaInt = ByteConvert.ToInt32BigEndian(bytes);
                var directory = new PngDirectory(PngChunkType.gAMA);
                directory.Set(PngDirectory.TagGamma, gammaInt / 100000.0);
                yield return directory;
            }
            else if (chunkType == PngChunkType.iCCP)
            {
                var reader = new SequentialByteArrayReader(bytes);
                var profileName = reader.GetNullTerminatedStringValue(maxLengthBytes: 79);
                var directory = new PngDirectory(PngChunkType.iCCP);
                directory.Set(PngDirectory.TagIccProfileName, profileName);
                var compressionMethod = reader.GetSByte();
                if (compressionMethod == 0)
                {
                    // Only compression method allowed by the spec is zero: deflate
                    // This assumes 1-byte-per-char, which it is by spec.
                    var bytesLeft = bytes.Length - profileName.Bytes.Length - 2;

                    // http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
                    // First two bytes are part of the zlib specification (RFC 1950), not the deflate specification (RFC 1951).
                    reader.Skip(2);
                    bytesLeft -= 2;

                    var compressedProfile = reader.GetBytes(bytesLeft);

                    IccDirectory? iccDirectory = null;
                    Exception? ex = null;
                    try
                    {
                        using var inflaterStream = new DeflateStream(new MemoryStream(compressedProfile), CompressionMode.Decompress);
                        iccDirectory = new IccReader().Extract(new IndexedCapturingReader(inflaterStream));
                        iccDirectory.Parent = directory;
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }

                    if (iccDirectory != null)
                        yield return iccDirectory;
                    else if (ex != null)
                        directory.AddError($"Exception decompressing {nameof(PngChunkType.iCCP)} chunk: {ex.Message}");
                }
                else
                {
                    directory.AddError("Invalid compression method value");
                }
                yield return directory;
            }
            else if (chunkType == PngChunkType.bKGD)
            {
                var directory = new PngDirectory(PngChunkType.bKGD);
                directory.Set(PngDirectory.TagBackgroundColor, bytes);
                yield return directory;
            }
            else if (chunkType == PngChunkType.tEXt)
            {
                var reader = new SequentialByteArrayReader(bytes);
                var keyword = reader.GetNullTerminatedStringValue(maxLengthBytes: 79).ToString(_latin1Encoding);
                var bytesLeft = bytes.Length - keyword.Length - 1;
                var value = reader.GetNullTerminatedStringValue(bytesLeft, _latin1Encoding);

                var textPairs = new List<KeyValuePair> { new KeyValuePair(keyword, value) };
                var directory = new PngDirectory(PngChunkType.tEXt);
                directory.Set(PngDirectory.TagTextualData, textPairs);
                yield return directory;
            }
            else if (chunkType == PngChunkType.zTXt)
            {
                var reader = new SequentialByteArrayReader(bytes);
                var keyword = reader.GetNullTerminatedStringValue(maxLengthBytes: 79).ToString(_latin1Encoding);
                var compressionMethod = reader.GetSByte();

                var bytesLeft = bytes.Length - keyword.Length - 1 - 1 - 1 - 1;
                byte[]? textBytes = null;
                if (compressionMethod == 0)
                {
                    if (!TryDeflate(bytes, bytesLeft, out textBytes, out string? errorMessage))
                    {
                        var directory = new PngDirectory(PngChunkType.zTXt);
                        directory.AddError($"Exception decompressing {nameof(PngChunkType.zTXt)} chunk with keyword \"{keyword}\": {errorMessage}");
                        yield return directory;
                    }
                }
                else
                {
                    var directory = new PngDirectory(PngChunkType.zTXt);
                    directory.AddError("Invalid compression method value");
                    yield return directory;
                }

                if (textBytes != null)
                {
                    foreach (var directory in ProcessTextChunk(keyword, textBytes))
                    {
                        yield return directory; 
                    }
                }
            }
            else if (chunkType == PngChunkType.iTXt)
            {
                var reader = new SequentialByteArrayReader(bytes);
                var keyword = reader.GetNullTerminatedStringValue(maxLengthBytes: 79).ToString(_latin1Encoding);
                var compressionFlag = reader.GetSByte();
                var compressionMethod = reader.GetSByte();

                // TODO we currently ignore languageTagBytes and translatedKeywordBytes
                var languageTagBytes = reader.GetNullTerminatedBytes(bytes.Length);
                var translatedKeywordBytes = reader.GetNullTerminatedBytes(bytes.Length);

                var bytesLeft = bytes.Length - keyword.Length - 1 - 1 - 1 - languageTagBytes.Length - 1 - translatedKeywordBytes.Length - 1;
                byte[]? textBytes = null;
                if (compressionFlag == 0)
                {
                    textBytes = reader.GetNullTerminatedBytes(bytesLeft);
                }
                else if (compressionFlag == 1)
                {
                    if (compressionMethod == 0)
                    {
                        if (!TryDeflate(bytes, bytesLeft, out textBytes, out string? errorMessage))
                        {
                            var directory = new PngDirectory(PngChunkType.iTXt);
                            directory.AddError($"Exception decompressing {nameof(PngChunkType.iTXt)} chunk with keyword \"{keyword}\": {errorMessage}");
                            yield return directory;
                        }
                    }
                    else
                    {
                        var directory = new PngDirectory(PngChunkType.iTXt);
                        directory.AddError("Invalid compression method value");
                        yield return directory;
                    }
                }
                else
                {
                    var directory = new PngDirectory(PngChunkType.iTXt);
                    directory.AddError("Invalid compression flag value");
                    yield return directory;
                }

                if (textBytes != null)
                {
                    foreach (var directory in ProcessTextChunk(keyword, textBytes))
                    {
                        yield return directory;
                    }
                }
            }
            else if (chunkType == PngChunkType.tIME)
            {
                var reader = new SequentialByteArrayReader(bytes);
                var year = reader.GetUInt16();
                var month = reader.GetByte();
                int day = reader.GetByte();
                int hour = reader.GetByte();
                int minute = reader.GetByte();
                int second = reader.GetByte();
                var directory = new PngDirectory(PngChunkType.tIME);
                if (DateUtil.IsValidDate(year, month, day) && DateUtil.IsValidTime(hour, minute, second))
                {
                    var time = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                    directory.Set(PngDirectory.TagLastModificationTime, time);
                }
                else
                    directory.AddError($"PNG tIME data describes an invalid date/time: year={year} month={month} day={day} hour={hour} minute={minute} second={second}");
                yield return directory;
            }
            else if (chunkType == PngChunkType.pHYs)
            {
                var reader = new SequentialByteArrayReader(bytes);
                var pixelsPerUnitX = reader.GetInt32();
                var pixelsPerUnitY = reader.GetInt32();
                var unitSpecifier = reader.GetSByte();
                var directory = new PngDirectory(PngChunkType.pHYs);
                directory.Set(PngDirectory.TagPixelsPerUnitX, pixelsPerUnitX);
                directory.Set(PngDirectory.TagPixelsPerUnitY, pixelsPerUnitY);
                directory.Set(PngDirectory.TagUnitSpecifier, unitSpecifier);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.sBIT))
            {
                var directory = new PngDirectory(PngChunkType.sBIT);
                directory.Set(PngDirectory.TagSignificantBits, bytes);
                yield return directory;
            }

            yield break;

            IEnumerable<Directory> ProcessTextChunk(string keyword, byte[] textBytes)
            {
                if (keyword == "XML:com.adobe.xmp")
                {
                    yield return new XmpReader().Extract(textBytes);
                }
                else if (keyword == "Raw profile type xmp")
                {
                    if (TryProcessRawProfile(out int byteCount))
                    {
                        yield return new XmpReader().Extract(textBytes, 0, byteCount);
                    }
                    else
                    {
                        yield return ReadTextDirectory(keyword, textBytes, chunkType);
                    }
                }
                else if (keyword == "Raw profile type exif" || keyword == "Raw profile type APP1")
                {
                    if (TryProcessRawProfile(out _))
                    {
                        foreach (var exifDirectory in new ExifReader().Extract(new ByteArrayReader(textBytes)))
                            yield return exifDirectory;
                    }
                    else
                    {
                        yield return ReadTextDirectory(keyword, textBytes, chunkType);
                    }
                }
                else if (keyword == "Raw profile type icc" || keyword == "Raw profile type icm")
                {
                    if (TryProcessRawProfile(out _))
                    {
                        yield return new IccReader().Extract(new ByteArrayReader(textBytes));
                    }
                    else
                    {
                        yield return ReadTextDirectory(keyword, textBytes, chunkType);
                    }
                }
                else if (keyword == "Raw profile type iptc")
                {
                    if (TryProcessRawProfile(out int byteCount))
                    {
                        yield return new IptcReader().Extract(new SequentialByteArrayReader(textBytes), byteCount);
                    }
                    else
                    {
                        yield return ReadTextDirectory(keyword, textBytes, chunkType);
                    }
                }
                else
                {
                    yield return ReadTextDirectory(keyword, textBytes, chunkType);
                }

                PngDirectory ReadTextDirectory(string keyword, byte[] textBytes, PngChunkType pngChunkType)
                {
                    var textPairs = new[] { new KeyValuePair(keyword, new StringValue(textBytes, _latin1Encoding)) };
                    var directory = new PngDirectory(pngChunkType);
                    directory.Set(PngDirectory.TagTextualData, textPairs);
                    return directory;
                }

                bool TryProcessRawProfile(out int byteCount)
                {
                    // Raw profiles have form "\n<name>\n<length>\n<hex>\n"

                    if (textBytes.Length == 0 || textBytes[0] != '\n')
                    {
                        byteCount = default;
                        return false;
                    }

                    var i = 1;

                    // Skip name
                    while (i < textBytes.Length && textBytes[i] != '\n')
                        i++;

                    if (i == textBytes.Length)
                    {
                        byteCount = default;
                        return false;
                    }

                    // Read length
                    int length = 0;
                    while (true)
                    {
                        i++;
                        var c = (char)textBytes[i];

                        if (c == ' ')
                            continue;
                        if (c == '\n')
                            break;

                        if (c >= '0' && c <= '9')
                        {
                            length *= 10;
                            length += c - '0';
                        }
                        else
                        {
                            byteCount = default;
                            return false;
                        }
                    }

                    i++;

                    // We should be at the ASCII-encoded hex data. Walk through the remaining bytes, re-writing as raw bytes
                    // starting at offset zero in the array. We have to skip \n characters.

                    // Validate the data can be correctly parsed before modifying it in-place, because if parsing fails later
                    // consumers may want the unmodified data.

                    // Each row must have 72 characters (36 bytes once decoded) separated by \n
                    const int rowCharCount = 72;
                    int charsInRow = rowCharCount;

                    for (int j = i; j < length + i; j++)
                    {
                        byte c = textBytes[j];

                        if (charsInRow-- == 0)
                        {
                            if (c != '\n')
                            {
                                byteCount = default;
                                return false;
                            }

                            charsInRow = rowCharCount;
                            continue;
                        }

                        if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
                        {
                            byteCount = default;
                            return false;
                        }
                    }

                    byteCount = length;
                    var writeIndex = 0;
                    charsInRow = rowCharCount;
                    while (length > 0)
                    {
                        var c1 = textBytes[i++];

                        if (charsInRow-- == 0)
                        {
                            Debug.Assert(c1 == '\n');
                            charsInRow = rowCharCount;
                            continue;
                        }

                        var c2 = textBytes[i++];

                        charsInRow--;

                        var n1 = ParseHexNibble(c1);
                        var n2 = ParseHexNibble(c2);

                        length--;
                        textBytes[writeIndex++] = (byte) ((n1 << 4) | n2);
                    }

                    return writeIndex == byteCount;

                    static int ParseHexNibble(int h)
                    {
                        if (h >= '0' && h <= '9')
                        {
                            return h - '0';
                        }

                        if (h >= 'a' && h <= 'f')
                        {
                            return 10 + (h - 'a');
                        }

                        if (h >= 'A' && h <= 'F')
                        {
                            return 10 + (h - 'A');
                        }

                        Debug.Fail("Should not reach here");
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private static bool TryDeflate(
            byte[] bytes,
            int bytesLeft,
            [NotNullWhen(returnValue: true)] out byte[]? textBytes,
            [NotNullWhen(returnValue: true)] out string? errorMessage)
        {
            using var inflaterStream = new DeflateStream(new MemoryStream(bytes, bytes.Length - bytesLeft, bytesLeft), CompressionMode.Decompress);
            try
            {
                var ms = new MemoryStream();

#if !NET35
                inflaterStream.CopyTo(ms);
#else
                var buffer = new byte[1024];
                int count;
                while ((count = inflaterStream.Read(buffer, 0, 256)) > 0)
                    ms.Write(buffer, 0, count);
#endif

                textBytes = ms.ToArray();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                textBytes = default;
                return false;
            }

            errorMessage = default;
            return true;
        }
    }
}
