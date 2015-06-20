#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class PngMetadataReader
    {
        private static readonly HashSet<PngChunkType> _desiredChunkTypes;

        static PngMetadataReader()
        {
            _desiredChunkTypes = new HashSet<PngChunkType>
            {
                PngChunkType.Ihdr,
                PngChunkType.Plte,
                PngChunkType.TRns,
                PngChunkType.CHrm,
                PngChunkType.SRgb,
                PngChunkType.GAma,
                PngChunkType.ICcp,
                PngChunkType.BKgd,
                PngChunkType.TEXt,
                PngChunkType.ITXt,
                PngChunkType.TIme,
                PngChunkType.PHYs,
                PngChunkType.SBit
            };
        }

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static IReadOnlyList<Directory> ReadMetadata([NotNull] string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static IReadOnlyList<Directory> ReadMetadata([NotNull] Stream stream)
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
                    Console.Error.WriteLine(e);
                }
            }

            return directories;
        }

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        private static IEnumerable<Directory> ProcessChunk([NotNull] PngChunk chunk)
        {
            var chunkType = chunk.ChunkType;
            var bytes = chunk.Bytes;

            if (chunkType.Equals(PngChunkType.Ihdr))
            {
                var header = new PngHeader(bytes);
                var directory = new PngDirectory(PngChunkType.Ihdr);
                directory.Set(PngDirectory.TagImageWidth, header.ImageWidth);
                directory.Set(PngDirectory.TagImageHeight, header.ImageHeight);
                directory.Set(PngDirectory.TagBitsPerSample, header.BitsPerSample);
                directory.Set(PngDirectory.TagColorType, header.ColorType.NumericValue);
                directory.Set(PngDirectory.TagCompressionType, header.CompressionType);
                directory.Set(PngDirectory.TagFilterMethod, header.FilterMethod);
                directory.Set(PngDirectory.TagInterlaceMethod, header.InterlaceMethod);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.Plte))
            {
                var directory = new PngDirectory(PngChunkType.Plte);
                directory.Set(PngDirectory.TagPaletteSize, bytes.Length / 3);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.TRns))
            {
                var directory = new PngDirectory(PngChunkType.TRns);
                directory.Set(PngDirectory.TagPaletteHasTransparency, 1);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.SRgb))
            {
                int srgbRenderingIntent = new SequentialByteArrayReader(bytes).GetSByte();
                var directory = new PngDirectory(PngChunkType.SRgb);
                directory.Set(PngDirectory.TagSrgbRenderingIntent, srgbRenderingIntent);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.CHrm))
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
            else if (chunkType.Equals(PngChunkType.GAma))
            {
                var gammaInt = new SequentialByteArrayReader(bytes).GetInt32();
                var directory = new PngDirectory(PngChunkType.GAma);
                directory.Set(PngDirectory.TagGamma, gammaInt / 100000.0);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.ICcp))
            {
                SequentialReader reader = new SequentialByteArrayReader(bytes);
                var profileName = reader.GetNullTerminatedString(79);
                var directory = new PngDirectory(PngChunkType.ICcp);
                directory.Set(PngDirectory.TagIccProfileName, profileName);
                var compressionMethod = reader.GetSByte();
                if (compressionMethod == 0)
                {
                    // Only compression method allowed by the spec is zero: deflate
                    // This assumes 1-byte-per-char, which it is by spec.
                    var bytesLeft = bytes.Length - profileName.Length - 2;
                    var compressedProfile = reader.GetBytes(bytesLeft);
                    using (var inflaterStream = new DeflateStream(new MemoryStream(compressedProfile), CompressionMode.Decompress))
                        yield return new IccReader().Extract(new IndexedCapturingReader(inflaterStream));
                }
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.BKgd))
            {
                var directory = new PngDirectory(PngChunkType.BKgd);
                directory.Set(PngDirectory.TagBackgroundColor, bytes);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.TEXt))
            {
                SequentialReader reader = new SequentialByteArrayReader(bytes);
                var keyword = reader.GetNullTerminatedString(79);
                var bytesLeft = bytes.Length - keyword.Length - 1;
                var value = reader.GetNullTerminatedString(bytesLeft);
                IList<KeyValuePair> textPairs = new List<KeyValuePair>();
                textPairs.Add(new KeyValuePair(keyword, value));
                var directory = new PngDirectory(PngChunkType.ITXt);
                directory.Set(PngDirectory.TagTextualData, textPairs);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.ITXt))
            {
                SequentialReader reader = new SequentialByteArrayReader(bytes);
                var keyword = reader.GetNullTerminatedString(79);
                var compressionFlag = reader.GetSByte();
                var compressionMethod = reader.GetSByte();
                var languageTag = reader.GetNullTerminatedString(bytes.Length);
                var translatedKeyword = reader.GetNullTerminatedString(bytes.Length);
                var bytesLeft = bytes.Length - keyword.Length - 1 - 1 - 1 - languageTag.Length - 1 - translatedKeyword.Length - 1;
                string text = null;
                if (compressionFlag == 0)
                {
                    text = reader.GetNullTerminatedString(bytesLeft);
                }
                else if (compressionFlag == 1)
                {
                    if (compressionMethod == 0)
                    {
                        using (var inflaterStream = new DeflateStream(new MemoryStream(bytes, bytes.Length - bytesLeft, bytesLeft), CompressionMode.Decompress))
                            text = new StreamReader(inflaterStream).ReadToEnd();
                    }
                    else
                    {
                        var directory = new PngDirectory(PngChunkType.ITXt);
                        directory.AddError("Invalid compression method value");
                        yield return directory;
                    }
                }
                else
                {
                    var directory = new PngDirectory(PngChunkType.ITXt);
                    directory.AddError("Invalid compression flag value");
                    yield return directory;
                }

                if (text != null)
                {
                    if (keyword.Equals("XML:com.adobe.xmp"))
                    {
                        // NOTE in testing images, the XMP has parsed successfully, but we are not extracting tags from it as necessary
                        yield return new XmpReader().Extract(text);
                    }
                    else
                    {
                        IList<KeyValuePair> textPairs = new List<KeyValuePair>();
                        textPairs.Add(new KeyValuePair(keyword, text));
                        var directory = new PngDirectory(PngChunkType.ITXt);
                        directory.Set(PngDirectory.TagTextualData, textPairs);
                        yield return directory;
                    }
                }
            }
            else if (chunkType.Equals(PngChunkType.TIme))
            {
                var reader = new SequentialByteArrayReader(bytes);
                var year = reader.GetUInt16();
                var month = reader.GetByte();
                int day = reader.GetByte();
                int hour = reader.GetByte();
                int minute = reader.GetByte();
                int second = reader.GetByte();
                var directory = new PngDirectory(PngChunkType.TIme);
                try
                {
                    var time = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                    directory.Set(PngDirectory.TagLastModificationTime, time);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    directory.AddError("Error constructing DateTime: " + e.Message);
                }
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.PHYs))
            {
                var reader = new SequentialByteArrayReader(bytes);
                var pixelsPerUnitX = reader.GetInt32();
                var pixelsPerUnitY = reader.GetInt32();
                var unitSpecifier = reader.GetSByte();
                var directory = new PngDirectory(PngChunkType.PHYs);
                directory.Set(PngDirectory.TagPixelsPerUnitX, pixelsPerUnitX);
                directory.Set(PngDirectory.TagPixelsPerUnitY, pixelsPerUnitY);
                directory.Set(PngDirectory.TagUnitSpecifier, unitSpecifier);
                yield return directory;
            }
            else if (chunkType.Equals(PngChunkType.SBit))
            {
                var directory = new PngDirectory(PngChunkType.SBit);
                directory.Set(PngDirectory.TagSignificantBits, bytes);
                yield return directory;
            }
        }
    }
}
