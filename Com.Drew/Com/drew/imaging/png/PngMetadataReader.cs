using System;
using System.Collections.Generic;
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
    public class PngMetadataReader
    {
        private static ICollection<PngChunkType> _desiredChunkTypes;

        static PngMetadataReader()
        {
            ICollection<PngChunkType> desiredChunkTypes = new HashSet<PngChunkType>();
            desiredChunkTypes.Add(PngChunkType.Ihdr);
            desiredChunkTypes.Add(PngChunkType.Plte);
            desiredChunkTypes.Add(PngChunkType.tRNS);
            desiredChunkTypes.Add(PngChunkType.cHRM);
            desiredChunkTypes.Add(PngChunkType.sRGB);
            desiredChunkTypes.Add(PngChunkType.gAMA);
            desiredChunkTypes.Add(PngChunkType.iCCP);
            desiredChunkTypes.Add(PngChunkType.bKGD);
            desiredChunkTypes.Add(PngChunkType.tEXt);
            desiredChunkTypes.Add(PngChunkType.iTXt);
            desiredChunkTypes.Add(PngChunkType.tIME);
            desiredChunkTypes.Add(PngChunkType.pHYs);
            desiredChunkTypes.Add(PngChunkType.sBIT);
            _desiredChunkTypes = Collections.UnmodifiableSet(desiredChunkTypes);
        }

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] FilePath file)
        {
            InputStream inputStream = new FileInputStream(file);
            Metadata.Metadata metadata;
            try
            {
                metadata = ReadMetadata(inputStream);
            }
            finally
            {
                inputStream.Close();
            }
            new FileMetadataReader().Read(file, metadata);
            return metadata;
        }

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] InputStream inputStream)
        {
            Iterable<PngChunk> chunks = new PngChunkReader().Extract(new StreamReader(inputStream), _desiredChunkTypes);
            Metadata.Metadata metadata = new Metadata.Metadata();
            foreach (PngChunk chunk in chunks)
            {
                try
                {
                    ProcessChunk(metadata, chunk);
                }
                catch (Exception e)
                {
                    Runtime.PrintStackTrace(e, Console.Error);
                }
            }
            return metadata;
        }

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        private static void ProcessChunk([NotNull] Metadata.Metadata metadata, [NotNull] PngChunk chunk)
        {
            PngChunkType chunkType = chunk.GetChunkType();
            sbyte[] bytes = chunk.GetBytes();
            if (chunkType.Equals(PngChunkType.Ihdr))
            {
                PngHeader header = new PngHeader(bytes);
                PngDirectory directory = new PngDirectory(PngChunkType.Ihdr);
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
                    PngDirectory directory = new PngDirectory(PngChunkType.Plte);
                    directory.SetInt(PngDirectory.TagPaletteSize, bytes.Length / 3);
                    metadata.AddDirectory(directory);
                }
                else
                {
                    if (chunkType.Equals(PngChunkType.tRNS))
                    {
                        PngDirectory directory = new PngDirectory(PngChunkType.tRNS);
                        directory.SetInt(PngDirectory.TagPaletteHasTransparency, 1);
                        metadata.AddDirectory(directory);
                    }
                    else
                    {
                        if (chunkType.Equals(PngChunkType.sRGB))
                        {
                            int srgbRenderingIntent = new SequentialByteArrayReader(bytes).GetInt8();
                            PngDirectory directory = new PngDirectory(PngChunkType.sRGB);
                            directory.SetInt(PngDirectory.TagSrgbRenderingIntent, srgbRenderingIntent);
                            metadata.AddDirectory(directory);
                        }
                        else
                        {
                            if (chunkType.Equals(PngChunkType.cHRM))
                            {
                                PngChromaticities chromaticities = new PngChromaticities(bytes);
                                PngChromaticitiesDirectory directory = new PngChromaticitiesDirectory();
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
                                if (chunkType.Equals(PngChunkType.gAMA))
                                {
                                    int gammaInt = new SequentialByteArrayReader(bytes).GetInt32();
                                    PngDirectory directory = new PngDirectory(PngChunkType.gAMA);
                                    directory.SetDouble(PngDirectory.TagGamma, gammaInt / 100000.0);
                                    metadata.AddDirectory(directory);
                                }
                                else
                                {
                                    if (chunkType.Equals(PngChunkType.iCCP))
                                    {
                                        SequentialReader reader = new SequentialByteArrayReader(bytes);
                                        string profileName = reader.GetNullTerminatedString(79);
                                        PngDirectory directory = new PngDirectory(PngChunkType.iCCP);
                                        directory.SetString(PngDirectory.TagIccProfileName, profileName);
                                        sbyte compressionMethod = reader.GetInt8();
                                        if (compressionMethod == 0)
                                        {
                                            // Only compression method allowed by the spec is zero: deflate
                                            // This assumes 1-byte-per-char, which it is by spec.
                                            int bytesLeft = bytes.Length - profileName.Length - 2;
                                            sbyte[] compressedProfile = reader.GetBytes(bytesLeft);
                                            InflaterInputStream inflateStream = new InflaterInputStream(new ByteArrayInputStream(compressedProfile));
                                            new IccReader().Extract(new RandomAccessStreamReader(inflateStream), metadata);
                                            inflateStream.Close();
                                        }
                                        metadata.AddDirectory(directory);
                                    }
                                    else
                                    {
                                        if (chunkType.Equals(PngChunkType.bKGD))
                                        {
                                            PngDirectory directory = new PngDirectory(PngChunkType.bKGD);
                                            directory.SetByteArray(PngDirectory.TagBackgroundColor, bytes);
                                            metadata.AddDirectory(directory);
                                        }
                                        else
                                        {
                                            if (chunkType.Equals(PngChunkType.tEXt))
                                            {
                                                SequentialReader reader = new SequentialByteArrayReader(bytes);
                                                string keyword = reader.GetNullTerminatedString(79);
                                                int bytesLeft = bytes.Length - keyword.Length - 1;
                                                string value = reader.GetNullTerminatedString(bytesLeft);
                                                IList<KeyValuePair> textPairs = new AList<KeyValuePair>();
                                                textPairs.Add(new KeyValuePair(keyword, value));
                                                PngDirectory directory = new PngDirectory(PngChunkType.iTXt);
                                                directory.SetObject(PngDirectory.TagTextualData, textPairs);
                                                metadata.AddDirectory(directory);
                                            }
                                            else
                                            {
                                                if (chunkType.Equals(PngChunkType.iTXt))
                                                {
                                                    SequentialReader reader = new SequentialByteArrayReader(bytes);
                                                    string keyword = reader.GetNullTerminatedString(79);
                                                    sbyte compressionFlag = reader.GetInt8();
                                                    sbyte compressionMethod = reader.GetInt8();
                                                    string languageTag = reader.GetNullTerminatedString(bytes.Length);
                                                    string translatedKeyword = reader.GetNullTerminatedString(bytes.Length);
                                                    int bytesLeft = bytes.Length - keyword.Length - 1 - 1 - 1 - languageTag.Length - 1 - translatedKeyword.Length - 1;
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
                                                                text = StringUtil.FromStream(new InflaterInputStream(new ByteArrayInputStream(bytes, bytes.Length - bytesLeft, bytesLeft)));
                                                            }
                                                            else
                                                            {
                                                                PngDirectory directory = new PngDirectory(PngChunkType.iTXt);
                                                                directory.AddError("Invalid compression method value");
                                                                metadata.AddDirectory(directory);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            PngDirectory directory = new PngDirectory(PngChunkType.iTXt);
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
                                                            IList<KeyValuePair> textPairs = new AList<KeyValuePair>();
                                                            textPairs.Add(new KeyValuePair(keyword, text));
                                                            PngDirectory directory = new PngDirectory(PngChunkType.iTXt);
                                                            directory.SetObject(PngDirectory.TagTextualData, textPairs);
                                                            metadata.AddDirectory(directory);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (chunkType.Equals(PngChunkType.tIME))
                                                    {
                                                        SequentialByteArrayReader reader = new SequentialByteArrayReader(bytes);
                                                        int year = reader.GetUInt16();
                                                        int month = reader.GetUInt8() - 1;
                                                        int day = reader.GetUInt8();
                                                        int hour = reader.GetUInt8();
                                                        int minute = reader.GetUInt8();
                                                        int second = reader.GetUInt8();
                                                        Calendar calendar = Calendar.GetInstance(Extensions.GetTimeZone("UTC"));
                                                        //noinspection MagicConstant
                                                        calendar.Set(year, month, day, hour, minute, second);
                                                        PngDirectory directory = new PngDirectory(PngChunkType.tIME);
                                                        directory.SetDate(PngDirectory.TagLastModificationTime, calendar.GetTime());
                                                        metadata.AddDirectory(directory);
                                                    }
                                                    else
                                                    {
                                                        if (chunkType.Equals(PngChunkType.pHYs))
                                                        {
                                                            SequentialByteArrayReader reader = new SequentialByteArrayReader(bytes);
                                                            int pixelsPerUnitX = reader.GetInt32();
                                                            int pixelsPerUnitY = reader.GetInt32();
                                                            sbyte unitSpecifier = reader.GetInt8();
                                                            PngDirectory directory = new PngDirectory(PngChunkType.pHYs);
                                                            directory.SetInt(PngDirectory.TagPixelsPerUnitX, pixelsPerUnitX);
                                                            directory.SetInt(PngDirectory.TagPixelsPerUnitY, pixelsPerUnitY);
                                                            directory.SetInt(PngDirectory.TagUnitSpecifier, unitSpecifier);
                                                            metadata.AddDirectory(directory);
                                                        }
                                                        else
                                                        {
                                                            if (chunkType.Equals(PngChunkType.sBIT))
                                                            {
                                                                PngDirectory directory = new PngDirectory(PngChunkType.sBIT);
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
