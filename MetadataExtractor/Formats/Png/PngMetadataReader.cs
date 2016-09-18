#region License
//
// Copyright 2002-2016 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
#if !PORTABLE
using System.IO.Compression;
#else
using Ionic.Zlib;
#endif
using JetBrains.Annotations;
using MetadataExtractor.Formats.Icc;
#if !PORTABLE
using MetadataExtractor.Formats.FileSystem;
#endif
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using MetadataExtractor.Util;

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
            PngChunkType.iTXt,
            PngChunkType.tIME,
            PngChunkType.pHYs,
            PngChunkType.sBIT
        };

#if !PORTABLE
        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static
#if NET35
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadMetadata([NotNull] string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }
#endif

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static
#if NET35 || PORTABLE
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadMetadata([NotNull] Stream stream)
        {
            var directories = new List<Directory>();

            var chunks = new PngChunkReader().Extract(new SequentialStreamReader(stream), _desiredChunkTypes);

            foreach (var chunk in chunks)
            {
                try
                {
                    directories.AddRange(ProcessChunk(chunk));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return directories;
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
        private static IEnumerable<Directory> ProcessChunk([NotNull] PngChunk chunk)
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
                    var compressedProfile = reader.GetBytes(bytesLeft);
                    using (var inflaterStream = new InflaterInputStream(new MemoryStream(compressedProfile)))
                    {
                        var iccDirectory = new IccReader().Extract(new IndexedCapturingReader(inflaterStream));
                        iccDirectory.Parent = directory;
                        yield return iccDirectory;
                    }
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
                var directory = new PngDirectory(PngChunkType.iTXt);
                directory.Set(PngDirectory.TagTextualData, textPairs);
                yield return directory;
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
                byte[] textBytes = null;
                if (compressionFlag == 0)
                {
                    textBytes = reader.GetNullTerminatedBytes(bytesLeft);
                }
                else if (compressionFlag == 1)
                {
                    if (compressionMethod == 0)
                    {
                        using (var inflaterStream = new DeflateStream(new MemoryStream(bytes, bytes.Length - bytesLeft, bytesLeft), CompressionMode.Decompress))
                        using (var decompStream = new MemoryStream())
                        {
#if !NET35
                            inflaterStream.CopyTo(decompStream);
#else
                            byte[] buffer = new byte[256];
                            int count;
                            int totalBytes = 0;
                            while ((count = inflaterStream.Read(buffer, 0, 256)) > 0)
                            {
                                decompStream.Write(buffer, 0, count);
                                totalBytes += count;
                            }
#endif
                            textBytes = decompStream.ToArray();
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
                    if (keyword == "XML:com.adobe.xmp")
                    {
                        // NOTE in testing images, the XMP has parsed successfully, but we are not extracting tags from it as necessary
                        yield return new XmpReader().Extract(textBytes);
                    }
                    else
                    {
                        var textPairs = new List<KeyValuePair> { new KeyValuePair(keyword, new StringValue(textBytes, _latin1Encoding)) };
                        var directory = new PngDirectory(PngChunkType.iTXt);
                        directory.Set(PngDirectory.TagTextualData, textPairs);
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
                try
                {
                    var time = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                    directory.Set(PngDirectory.TagLastModificationTime, time);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    directory.AddError("Error constructing DateTime: " + e.Message);
                }
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
        }
    }
}
