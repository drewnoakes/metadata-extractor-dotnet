/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Iptc
{
    /// <summary>
    /// Decodes IPTC binary data, populating a <see cref="Com.Drew.Metadata.Metadata"/> object with tag values in an <see cref="IptcDirectory"/>.
    /// <para>
    /// http://www.iptc.org/std/IIM/4.1/specification/IIMV4.1.pdf
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcReader : IJpegSegmentMetadataReader
    {
        // TODO consider breaking the IPTC section up into multiple directories and providing segregation of each IPTC directory
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

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.Appd;
        }

        public void ReadJpegSegments(IEnumerable<byte[]> segments, Metadata metadata, JpegSegmentType segmentType)
        {
            foreach (byte[] segmentBytes in segments)
            {
                // Ensure data starts with the IPTC marker byte
                if (segmentBytes.Length != 0 && segmentBytes[0] == unchecked(0x1c))
                {
                    Extract(new SequentialByteArrayReader(segmentBytes), metadata, segmentBytes.Length);
                }
            }
        }

        /// <summary>
        /// Performs the IPTC data extraction, adding found values to the specified instance of <see cref="Com.Drew.Metadata.Metadata"/>.
        /// </summary>
        public void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata, long length)
        {
            IptcDirectory directory = new IptcDirectory();
            metadata.AddDirectory(directory);
            int offset = 0;
            // for each tag
            while (offset < length)
            {
                // identifies start of a tag
                short startByte;
                try
                {
                    startByte = reader.GetUInt8();
                    offset++;
                }
                catch (IOException)
                {
                    directory.AddError("Unable to read starting byte of IPTC tag");
                    return;
                }
                if (startByte != unchecked(0x1c))
                {
                    // NOTE have seen images where there was one extra byte at the end, giving
                    // offset==length at this point, which is not worth logging as an error.
                    if (offset != length)
                    {
                        directory.AddError(string.Format("Invalid IPTC tag marker at offset {0}. Expected '0x1c' but got '0x{1:X}'.", offset - 1, startByte));
                    }
                    return;
                }
                // we need at least five bytes left to read a tag
                if (offset + 5 >= length)
                {
                    directory.AddError("Too few bytes remain for a valid IPTC tag");
                    return;
                }
                int directoryType;
                int tagType;
                int tagByteCount;
                try
                {
                    directoryType = reader.GetUInt8();
                    tagType = reader.GetUInt8();
                    // TODO support Extended DataSet Tag (see 1.5(c), p14, IPTC-IIMV4.2.pdf)
                    tagByteCount = reader.GetUInt16();
                    offset += 4;
                }
                catch (IOException)
                {
                    directory.AddError("IPTC data segment ended mid-way through tag descriptor");
                    return;
                }
                if (offset + tagByteCount > length)
                {
                    directory.AddError("Data for tag extends beyond end of IPTC segment");
                    return;
                }
                try
                {
                    ProcessTag(reader, directory, directoryType, tagType, tagByteCount);
                }
                catch (IOException)
                {
                    directory.AddError("Error processing IPTC tag");
                    return;
                }
                offset += tagByteCount;
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void ProcessTag([NotNull] SequentialReader reader, [NotNull] Directory directory, int directoryType, int tagType, int tagByteCount)
        {
            int tagIdentifier = tagType | (directoryType << 8);
            // Some images have been seen that specify a zero byte tag, which cannot be of much use.
            // We elect here to completely ignore the tag. The IPTC specification doesn't mention
            // anything about the interpretation of this situation.
            // https://raw.githubusercontent.com/wiki/drewnoakes/metadata-extractor/docs/IPTC-IIMV4.2.pdf
            if (tagByteCount == 0)
            {
                directory.SetString(tagIdentifier, string.Empty);
                return;
            }
            string @string = null;
            switch (tagIdentifier)
            {
                case IptcDirectory.TagCodedCharacterSet:
                {
                    byte[] bytes = reader.GetBytes(tagByteCount);
                    string charset = Iso2022Converter.ConvertIso2022CharsetToJavaCharset(bytes);
                    if (charset == null)
                    {
                        // Unable to determine the charset, so fall through and treat tag as a regular string
                        @string = Encoding.UTF8.GetString(bytes);
                        break;
                    }
                    directory.SetString(tagIdentifier, charset);
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
                        int shortValue = reader.GetUInt16();
                        reader.Skip(tagByteCount - 2);
                        directory.SetInt(tagIdentifier, shortValue);
                        return;
                    }
                    break;
                }

                case IptcDirectory.TagUrgency:
                {
                    // byte
                    directory.SetInt(tagIdentifier, reader.GetUInt8());
                    reader.Skip(tagByteCount - 1);
                    return;
                }

                case IptcDirectory.TagReleaseDate:
                case IptcDirectory.TagDateCreated:
                {
                    // Date object
                    if (tagByteCount >= 8)
                    {
                        @string = reader.GetString(tagByteCount);
                        try
                        {
                            int year = Convert.ToInt32(Runtime.Substring(@string, 0, 4));
                            int month = Convert.ToInt32(Runtime.Substring(@string, 4, 6)) - 1;
                            int day = Convert.ToInt32(Runtime.Substring(@string, 6, 8));
                            DateTime date = new GregorianCalendar(year, month, day).GetTime();
                            directory.SetDate(tagIdentifier, date);
                            return;
                        }
                        catch (FormatException)
                        {
                        }
                    }
                    else
                    {
                        // fall through and we'll process the 'string' value below
                        reader.Skip(tagByteCount);
                    }
                    goto case IptcDirectory.TagReleaseTime;
                }

                case IptcDirectory.TagReleaseTime:
                case IptcDirectory.TagTimeCreated:
                default:
                {
                    break;
                }
            }
            // time...
            // fall through
            // If we haven't returned yet, treat it as a string
            // NOTE that there's a chance we've already loaded the value as a string above, but failed to parse the value
            if (@string == null)
            {
                string encodingName = directory.GetString(IptcDirectory.TagCodedCharacterSet);
                if (encodingName != null)
                {
                    var encoding = Encoding.GetEncoding(encodingName);
                    @string = reader.GetString(tagByteCount, encoding);
                }
                else
                {
                    byte[] bytes1 = reader.GetBytes(tagByteCount);
                    var encoding = Iso2022Converter.GuessEncoding(bytes1);
                    @string = encoding != null ? encoding.GetString(bytes1) : Encoding.UTF8.GetString(bytes1);
                }
            }
            if (directory.ContainsTag(tagIdentifier))
            {
                // this fancy string[] business avoids using an ArrayList for performance reasons
                string[] oldStrings = directory.GetStringArray(tagIdentifier);
                string[] newStrings;
                if (oldStrings == null)
                {
                    newStrings = new string[1];
                }
                else
                {
                    newStrings = new string[oldStrings.Length + 1];
                    Array.Copy(oldStrings, 0, newStrings, 0, oldStrings.Length);
                }
                newStrings[newStrings.Length - 1] = @string;
                directory.SetStringArray(tagIdentifier, newStrings);
            }
            else
            {
                directory.SetString(tagIdentifier, @string);
            }
        }
    }
}
