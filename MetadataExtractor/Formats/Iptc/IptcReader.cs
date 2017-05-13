#region License
//
// Copyright 2002-2017 Drew Noakes
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
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Iptc
{
    /// <summary>Reads IPTC data.</summary>
    /// <remarks>
    /// Extracted values are returned from <see cref="Extract"/> in an <see cref="IptcDirectory"/>.
    /// <para />
    /// See the IPTC specification: http://www.iptc.org/std/IIM/4.1/specification/IIMV4.1.pdf
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcReader : IJpegSegmentMetadataReader
    {
        // TODO consider storing each IPTC record in a separate directory

/*
        public static final int DIRECTORY_IPTC = 2;

        public static final int ENVELOPE_RECORD = 1;
        public static final int APPLICATION_RECORD_2 = 2;
        public static final int APPLICATION_RECORD_3 = 3;
        public static final int APPLICATION_RECORD_4 = 4;
        public static final int APPLICATION_RECORD_5 = 5;
        public static final int APPLICATION_RECORD_6 = 6;
        public static final int PRE_DATA_RECORD = 7;
        public static final int DATA_RECORD = 8;
        public static final int POST_DATA_RECORD = 9;
*/

        private const byte IptcMarkerByte = 0x1c;

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.AppD };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Ensure data starts with the IPTC marker byte
            return segments
                .Where(segment => segment.Bytes.Length != 0 && segment.Bytes[0] == IptcMarkerByte)
                .Select(segment => Extract(new SequentialByteArrayReader(segment.Bytes), segment.Bytes.Length))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        /// <summary>Reads IPTC values and returns them in an <see cref="IptcDirectory"/>.</summary>
        /// <remarks>
        /// Note that IPTC data does not describe its own length, hence <paramref name="length"/> is required.
        /// </remarks>
        [NotNull]
        public IptcDirectory Extract([NotNull] SequentialReader reader, long length)
        {
            var directory = new IptcDirectory();

            var offset = 0;

            // for each tag
            while (offset < length)
            {
                // identifies start of a tag
                byte startByte;
                try
                {
                    startByte = reader.GetByte();
                    offset++;
                }
                catch (IOException)
                {
                    directory.AddError("Unable to read starting byte of IPTC tag");
                    break;
                }

                if (startByte != IptcMarkerByte)
                {
                    // NOTE have seen images where there was one extra byte at the end, giving
                    // offset==length at this point, which is not worth logging as an error.
                    if (offset != length)
                        directory.AddError($"Invalid IPTC tag marker at offset {offset - 1}. Expected '0x{IptcMarkerByte:x2}' but got '0x{startByte:x}'.");
                    break;
                }

                // we need at least five bytes left to read a tag
                if (offset + 5 > length)
                {
                    directory.AddError("Too few bytes remain for a valid IPTC tag");
                    break;
                }

                int directoryType;
                int tagType;
                int tagByteCount;
                try
                {
                    directoryType = reader.GetByte();
                    tagType = reader.GetByte();
                    // TODO support Extended DataSet Tag (see 1.5(c), p14, IPTC-IIMV4.2.pdf)
                    tagByteCount = reader.GetUInt16();
                    offset += 4;
                }
                catch (IOException)
                {
                    directory.AddError("IPTC data segment ended mid-way through tag descriptor");
                    break;
                }

                if (offset + tagByteCount > length)
                {
                    directory.AddError("Data for tag extends beyond end of IPTC segment");
                    break;
                }

                try
                {
                    ProcessTag(reader, directory, directoryType, tagType, tagByteCount);
                }
                catch (IOException)
                {
                    directory.AddError("Error processing IPTC tag");
                    break;
                }

                offset += tagByteCount;
            }

            return directory;
        }

        private static void ProcessTag([NotNull] SequentialReader reader, [NotNull] Directory directory, int directoryType, int tagType, int tagByteCount)
        {
            var tagIdentifier = tagType | (directoryType << 8);

            // Some images have been seen that specify a zero byte tag, which cannot be of much use.
            // We elect here to completely ignore the tag. The IPTC specification doesn't mention
            // anything about the interpretation of this situation.
            // https://raw.githubusercontent.com/wiki/drewnoakes/metadata-extractor/docs/IPTC-IIMV4.2.pdf
            if (tagByteCount == 0)
            {
                directory.Set(tagIdentifier, string.Empty);
                return;
            }

            switch (tagIdentifier)
            {
                case IptcDirectory.TagCodedCharacterSet:
                {
                    var bytes = reader.GetBytes(tagByteCount);
                    var charset = Iso2022Converter.ConvertEscapeSequenceToEncodingName(bytes);
                    if (charset == null)
                    {
                        // Unable to determine the charset, so fall through and treat tag as a regular string
                        charset = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    }
                    directory.Set(tagIdentifier, charset);
                    return;
                }

                case IptcDirectory.TagEnvelopeRecordVersion:
                case IptcDirectory.TagApplicationRecordVersion:
                case IptcDirectory.TagFileVersion:
                case IptcDirectory.TagArmVersion:
                case IptcDirectory.TagProgramVersion:
                {
                    // short
                    if (tagByteCount >= 2)
                    {
                        var shortValue = reader.GetUInt16();
                        reader.Skip(tagByteCount - 2);
                        directory.Set(tagIdentifier, shortValue);
                        return;
                    }
                    break;
                }

                case IptcDirectory.TagUrgency:
                {
                    // byte
                    directory.Set(tagIdentifier, reader.GetByte());
                    reader.Skip(tagByteCount - 1);
                    return;
                }
            }

            // If we haven't returned yet, treat it as a string
            // NOTE that there's a chance we've already loaded the value as a string above, but failed to parse the value
            var encodingName = directory.GetString(IptcDirectory.TagCodedCharacterSet);
            Encoding encoding = null;
            if (encodingName != null)
            {
                try
                {
                    encoding = Encoding.GetEncoding(encodingName);
                }
                catch (ArgumentException)
                { }
            }

            StringValue str;
            if (encoding != null)
                str = reader.GetStringValue(tagByteCount, encoding);
            else
            {
                var bytes = reader.GetBytes(tagByteCount);
                encoding = Iso2022Converter.GuessEncoding(bytes);
                str = new StringValue(bytes, encoding);
            }

            if (directory.ContainsTag(tagIdentifier))
            {
                // this fancy string[] business avoids using an ArrayList for performance reasons
                var oldStrings = directory.GetStringValueArray(tagIdentifier);

                StringValue[] newStrings;
                if (oldStrings == null)
                {
                    // TODO hitting this block means any prior value(s) are discarded
                    newStrings = new StringValue[1];
                }
                else
                {
                    newStrings = new StringValue[oldStrings.Length + 1];
                    Array.Copy(oldStrings, 0, newStrings, 0, oldStrings.Length);
                }
                newStrings[newStrings.Length - 1] = str;
                directory.Set(tagIdentifier, newStrings);
            }
            else
            {
                directory.Set(tagIdentifier, str);
            }
        }
    }
}
