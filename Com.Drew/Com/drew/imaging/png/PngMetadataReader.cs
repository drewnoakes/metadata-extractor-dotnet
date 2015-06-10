using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Com.Drew.Lang;
using Com.Drew.Metadata.File;
using Com.Drew.Metadata.Icc;
using Com.Drew.Metadata.Png;
using Com.Drew.Metadata.Xmp;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class PngMetadataReader
    {
        private static readonly ICollection<PngChunkType> DesiredChunkTypes;

        static PngMetadataReader()
        {
            ICollection<PngChunkType> desiredChunkTypes = new HashSet<PngChunkType>();
            desiredChunkTypes.Add(PngChunkType.Ihdr);
            desiredChunkTypes.Add(PngChunkType.Plte);
            desiredChunkTypes.Add(PngChunkType.TRns);
            desiredChunkTypes.Add(PngChunkType.CHrm);
            desiredChunkTypes.Add(PngChunkType.SRgb);
            desiredChunkTypes.Add(PngChunkType.GAma);
            desiredChunkTypes.Add(PngChunkType.ICcp);
            desiredChunkTypes.Add(PngChunkType.BKgd);
            desiredChunkTypes.Add(PngChunkType.TEXt);
            desiredChunkTypes.Add(PngChunkType.ITXt);
            desiredChunkTypes.Add(PngChunkType.TIme);
            desiredChunkTypes.Add(PngChunkType.PHYs);
            desiredChunkTypes.Add(PngChunkType.SBit);
            DesiredChunkTypes = Collections.UnmodifiableSet(desiredChunkTypes);
        }

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] string filePath)
        {
            Metadata.Metadata metadata;
            using (Stream stream = new FileStream(filePath, FileMode.Open))
                metadata = ReadMetadata(stream);
            new FileMetadataReader().Read(filePath, metadata);
            return metadata;
        }

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] Stream stream)
        {
            var chunks = new PngChunkReader().Extract(new SequentialStreamReader(stream), DesiredChunkTypes);
            var metadata = new Metadata.Metadata();
            foreach (var chunk in chunks)
            {
                try
                {
                    ProcessChunk(metadata, chunk);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine (e);
                }
            }
            return metadata;
        }

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        private static void ProcessChunk([NotNull] Metadata.Metadata metadata, [NotNull] PngChunk chunk)
        {
            var chunkType = chunk.ChunkType;
            var bytes = chunk.Bytes;
            if (chunkType.Equals(PngChunkType.Ihdr))
            {
                var header = new PngHeader(bytes);
                var directory = new PngDirectory(PngChunkType.Ihdr);
                directory.SetInt(PngDirectory.TagImageWidth, header.GetImageWidth());
                directory.SetInt(PngDirectory.TagImageHeight, header.GetImageHeight());
                directory.SetInt(PngDirectory.TagBitsPerSample, header.GetBitsPerSample());
                directory.SetInt(PngDirectory.TagColorType, header.GetColorType().GetNumericValue());
                directory.SetInt(PngDirectory.TagCompressionType, header.GetCompressionType());
                directory.SetInt(PngDirectory.TagFilterMethod, header.GetFilterMethod());
                directory.SetInt(PngDirectory.TagInterlaceMethod, header.GetInterlaceMethod());
                metadata.AddDirectory(directory);
            }
            else
            {
                if (chunkType.Equals(PngChunkType.Plte))
                {
                    var directory = new PngDirectory(PngChunkType.Plte);
                    directory.SetInt(PngDirectory.TagPaletteSize, bytes.Length / 3);
                    metadata.AddDirectory(directory);
                }
                else
                {
                    if (chunkType.Equals(PngChunkType.TRns))
                    {
                        var directory = new PngDirectory(PngChunkType.TRns);
                        directory.SetInt(PngDirectory.TagPaletteHasTransparency, 1);
                        metadata.AddDirectory(directory);
                    }
                    else
                    {
                        if (chunkType.Equals(PngChunkType.SRgb))
                        {
                            int srgbRenderingIntent = new SequentialByteArrayReader(bytes).GetInt8();
                            var directory = new PngDirectory(PngChunkType.SRgb);
                            directory.SetInt(PngDirectory.TagSrgbRenderingIntent, srgbRenderingIntent);
                            metadata.AddDirectory(directory);
                        }
                        else
                        {
                            if (chunkType.Equals(PngChunkType.CHrm))
                            {
                                var chromaticities = new PngChromaticities(bytes);
                                var directory = new PngChromaticitiesDirectory();
                                directory.SetInt(PngChromaticitiesDirectory.TagWhitePointX, chromaticities.GetWhitePointX());
                                directory.SetInt(PngChromaticitiesDirectory.TagWhitePointX, chromaticities.GetWhitePointX());
                                directory.SetInt(PngChromaticitiesDirectory.TagRedX, chromaticities.GetRedX());
                                directory.SetInt(PngChromaticitiesDirectory.TagRedY, chromaticities.GetRedY());
                                directory.SetInt(PngChromaticitiesDirectory.TagGreenX, chromaticities.GetGreenX());
                                directory.SetInt(PngChromaticitiesDirectory.TagGreenY, chromaticities.GetGreenY());
                                directory.SetInt(PngChromaticitiesDirectory.TagBlueX, chromaticities.GetBlueX());
                                directory.SetInt(PngChromaticitiesDirectory.TagBlueY, chromaticities.GetBlueY());
                                metadata.AddDirectory(directory);
                            }
                            else
                            {
                                if (chunkType.Equals(PngChunkType.GAma))
                                {
                                    var gammaInt = new SequentialByteArrayReader(bytes).GetInt32();
                                    var directory = new PngDirectory(PngChunkType.GAma);
                                    directory.SetDouble(PngDirectory.TagGamma, gammaInt / 100000.0);
                                    metadata.AddDirectory(directory);
                                }
                                else
                                {
                                    if (chunkType.Equals(PngChunkType.ICcp))
                                    {
                                        SequentialReader reader = new SequentialByteArrayReader(bytes);
                                        var profileName = reader.GetNullTerminatedString(79);
                                        var directory = new PngDirectory(PngChunkType.ICcp);
                                        directory.SetString(PngDirectory.TagIccProfileName, profileName);
                                        var compressionMethod = reader.GetInt8();
                                        if (compressionMethod == 0)
                                        {
                                            // Only compression method allowed by the spec is zero: deflate
                                            // This assumes 1-byte-per-char, which it is by spec.
                                            var bytesLeft = bytes.Length - profileName.Length - 2;
                                            var compressedProfile = reader.GetBytes(bytesLeft);
                                            using (var inflaterStream = new DeflateStream(new MemoryStream(compressedProfile), CompressionMode.Decompress))
                                                new IccReader().Extract(new IndexedCapturingReader(inflaterStream), metadata);
                                        }
                                        metadata.AddDirectory(directory);
                                    }
                                    else
                                    {
                                        if (chunkType.Equals(PngChunkType.BKgd))
                                        {
                                            var directory = new PngDirectory(PngChunkType.BKgd);
                                            directory.SetByteArray(PngDirectory.TagBackgroundColor, bytes);
                                            metadata.AddDirectory(directory);
                                        }
                                        else
                                        {
                                            if (chunkType.Equals(PngChunkType.TEXt))
                                            {
                                                SequentialReader reader = new SequentialByteArrayReader(bytes);
                                                var keyword = reader.GetNullTerminatedString(79);
                                                var bytesLeft = bytes.Length - keyword.Length - 1;
                                                var value = reader.GetNullTerminatedString(bytesLeft);
                                                IList<KeyValuePair> textPairs = new List<KeyValuePair>();
                                                textPairs.Add(new KeyValuePair(keyword, value));
                                                var directory = new PngDirectory(PngChunkType.ITXt);
                                                directory.SetObject(PngDirectory.TagTextualData, textPairs);
                                                metadata.AddDirectory(directory);
                                            }
                                            else
                                            {
                                                if (chunkType.Equals(PngChunkType.ITXt))
                                                {
                                                    SequentialReader reader = new SequentialByteArrayReader(bytes);
                                                    var keyword = reader.GetNullTerminatedString(79);
                                                    var compressionFlag = reader.GetInt8();
                                                    var compressionMethod = reader.GetInt8();
                                                    var languageTag = reader.GetNullTerminatedString(bytes.Length);
                                                    var translatedKeyword = reader.GetNullTerminatedString(bytes.Length);
                                                    var bytesLeft = bytes.Length - keyword.Length - 1 - 1 - 1 - languageTag.Length - 1 - translatedKeyword.Length - 1;
                                                    string text = null;
                                                    if (compressionFlag == 0)
                                                    {
                                                        text = reader.GetNullTerminatedString(bytesLeft);
                                                    }
                                                    else
                                                    {
                                                        if (compressionFlag == 1)
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
                                                                metadata.AddDirectory(directory);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var directory = new PngDirectory(PngChunkType.ITXt);
                                                            directory.AddError("Invalid compression flag value");
                                                            metadata.AddDirectory(directory);
                                                        }
                                                    }
                                                    if (text != null)
                                                    {
                                                        if (keyword.Equals("XML:com.adobe.xmp"))
                                                        {
                                                            // NOTE in testing images, the XMP has parsed successfully, but we are not extracting tags from it as necessary
                                                            new XmpReader().Extract(text, metadata);
                                                        }
                                                        else
                                                        {
                                                            IList<KeyValuePair> textPairs = new List<KeyValuePair>();
                                                            textPairs.Add(new KeyValuePair(keyword, text));
                                                            var directory = new PngDirectory(PngChunkType.ITXt);
                                                            directory.SetObject(PngDirectory.TagTextualData, textPairs);
                                                            metadata.AddDirectory(directory);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (chunkType.Equals(PngChunkType.TIme))
                                                    {
                                                        var reader = new SequentialByteArrayReader(bytes);
                                                        var year = reader.GetUInt16();
                                                        var month = reader.GetUInt8() - 1;
                                                        int day = reader.GetUInt8();
                                                        int hour = reader.GetUInt8();
                                                        int minute = reader.GetUInt8();
                                                        int second = reader.GetUInt8();
                                                        var calendar = Calendar.GetInstance(Extensions.GetTimeZone("UTC"));
                                                        //noinspection MagicConstant
                                                        calendar.Set(year, month, day, hour, minute, second);
                                                        var directory = new PngDirectory(PngChunkType.TIme);
                                                        directory.SetDate(PngDirectory.TagLastModificationTime, calendar.GetTime());
                                                        metadata.AddDirectory(directory);
                                                    }
                                                    else
                                                    {
                                                        if (chunkType.Equals(PngChunkType.PHYs))
                                                        {
                                                            var reader = new SequentialByteArrayReader(bytes);
                                                            var pixelsPerUnitX = reader.GetInt32();
                                                            var pixelsPerUnitY = reader.GetInt32();
                                                            var unitSpecifier = reader.GetInt8();
                                                            var directory = new PngDirectory(PngChunkType.PHYs);
                                                            directory.SetInt(PngDirectory.TagPixelsPerUnitX, pixelsPerUnitX);
                                                            directory.SetInt(PngDirectory.TagPixelsPerUnitY, pixelsPerUnitY);
                                                            directory.SetInt(PngDirectory.TagUnitSpecifier, unitSpecifier);
                                                            metadata.AddDirectory(directory);
                                                        }
                                                        else
                                                        {
                                                            if (chunkType.Equals(PngChunkType.SBit))
                                                            {
                                                                var directory = new PngDirectory(PngChunkType.SBit);
                                                                directory.SetByteArray(PngDirectory.TagSignificantBits, bytes);
                                                                metadata.AddDirectory(directory);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
