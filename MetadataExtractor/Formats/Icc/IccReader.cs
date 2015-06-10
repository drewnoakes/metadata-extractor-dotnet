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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Sharpen;

namespace MetadataExtractor.Formats.Icc
{
    /// <summary>Reads an ICC profile.</summary>
    /// <remarks>
    /// Reads an ICC profile.
    /// <list type="bullet">
    /// <item>http://en.wikipedia.org/wiki/ICC_profile</item>
    /// <item>http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/ICC_Profile.html</item>
    /// </list>
    /// </remarks>
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IccReader : IJpegSegmentMetadataReader, IMetadataReader
    {
        public const string JpegSegmentPreamble = "ICC_PROFILE";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.App2;
        }

        public void ReadJpegSegments(IEnumerable<byte[]> segments, Metadata metadata, JpegSegmentType segmentType)
        {
            var preambleLength = JpegSegmentPreamble.Length;
            // ICC data can be spread across multiple JPEG segments.
            // We concat them together in this buffer for later processing.
            byte[] buffer = null;
            foreach (var segmentBytes in segments)
            {
                // Skip any segments that do not contain the required preamble
                if (segmentBytes.Length < preambleLength || !JpegSegmentPreamble.Equals (Encoding.UTF8.GetString(segmentBytes, 0, preambleLength), StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                // NOTE we ignore three bytes here -- are they useful for anything?
                // Grow the buffer
                if (buffer == null)
                {
                    buffer = new byte[segmentBytes.Length - 14];
                    // skip the first 14 bytes
                    Array.Copy(segmentBytes, 14, buffer, 0, segmentBytes.Length - 14);
                }
                else
                {
                    var newBuffer = new byte[buffer.Length + segmentBytes.Length - 14];
                    Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                    Array.Copy(segmentBytes, 14, newBuffer, buffer.Length, segmentBytes.Length - 14);
                    buffer = newBuffer;
                }
            }
            if (buffer != null)
            {
                Extract(new ByteArrayReader(buffer), metadata);
            }
        }

        public void Extract(IndexedReader reader, Metadata metadata)
        {
            // TODO review whether the 'tagPtr' values below really do require IndexedReader or whether SequentialReader may be used instead
            var directory = new IccDirectory();
            try
            {
                var profileByteCount = reader.GetInt32(IccDirectory.TagProfileByteCount);
                directory.SetInt(IccDirectory.TagProfileByteCount, profileByteCount);
                // For these tags, the int value of the tag is in fact it's offset within the buffer.
                Set4ByteString(directory, IccDirectory.TagCmmType, reader);
                SetInt32(directory, IccDirectory.TagProfileVersion, reader);
                Set4ByteString(directory, IccDirectory.TagProfileClass, reader);
                Set4ByteString(directory, IccDirectory.TagColorSpace, reader);
                Set4ByteString(directory, IccDirectory.TagProfileConnectionSpace, reader);
                SetDate(directory, IccDirectory.TagProfileDatetime, reader);
                Set4ByteString(directory, IccDirectory.TagSignature, reader);
                Set4ByteString(directory, IccDirectory.TagPlatform, reader);
                SetInt32(directory, IccDirectory.TagCmmFlags, reader);
                Set4ByteString(directory, IccDirectory.TagDeviceMake, reader);
                var temp = reader.GetInt32(IccDirectory.TagDeviceModel);
                if (temp != 0)
                {
                    if (temp <= unchecked(0x20202020))
                    {
                        directory.SetInt(IccDirectory.TagDeviceModel, temp);
                    }
                    else
                    {
                        directory.SetString(IccDirectory.TagDeviceModel, GetStringFromInt32(temp));
                    }
                }
                SetInt32(directory, IccDirectory.TagRenderingIntent, reader);
                SetInt64(directory, IccDirectory.TagDeviceAttr, reader);
                var xyz = new[] { reader.GetS15Fixed16(IccDirectory.TagXyzValues), reader.GetS15Fixed16(IccDirectory.TagXyzValues + 4), reader.GetS15Fixed16(IccDirectory.TagXyzValues + 8) };
                directory.SetObject(IccDirectory.TagXyzValues, xyz);
                // Process 'ICC tags'
                var tagCount = reader.GetInt32(IccDirectory.TagTagCount);
                directory.SetInt(IccDirectory.TagTagCount, tagCount);
                for (var i = 0; i < tagCount; i++)
                {
                    var pos = IccDirectory.TagTagCount + 4 + i * 12;
                    var tagType = reader.GetInt32(pos);
                    var tagPtr = reader.GetInt32(pos + 4);
                    var tagLen = reader.GetInt32(pos + 8);
                    var b = reader.GetBytes(tagPtr, tagLen);
                    directory.SetByteArray(tagType, b);
                }
            }
            catch (IOException ex)
            {
                directory.AddError("Exception reading ICC profile: " + ex.Message);
            }
            metadata.AddDirectory(directory);
        }

        /// <exception cref="System.IO.IOException"/>
        private static void Set4ByteString([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var i = reader.GetInt32(tagType);
            if (i != 0)
            {
                directory.SetString(tagType, GetStringFromInt32(i));
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void SetInt32([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var i = reader.GetInt32(tagType);
            if (i != 0)
            {
                directory.SetInt(tagType, i);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void SetInt64([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var l = reader.GetInt64(tagType);
            if (l != 0)
            {
                directory.SetLong(tagType, l);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void SetDate([NotNull] IccDirectory directory, int tagType, [NotNull] IndexedReader reader)
        {
            int y = reader.GetUInt16(tagType);
            int m = reader.GetUInt16(tagType + 2);
            int d = reader.GetUInt16(tagType + 4);
            int h = reader.GetUInt16(tagType + 6);
            int M = reader.GetUInt16(tagType + 8);
            int s = reader.GetUInt16(tagType + 10);
            //        final Date value = new Date(Date.UTC(y - 1900, m - 1, d, h, M, s));
            var calendar = Calendar.GetInstance(Extensions.GetTimeZone("UTC"));
            calendar.Set(y, m, d, h, M, s);
            var value = calendar.GetTime();
            directory.SetDate(tagType, value);
        }

        [NotNull]
        public static string GetStringFromInt32(int d)
        {
            // MSB
            var b = new[] { unchecked((byte)((d & unchecked((int)(0xFF000000))) >> 24)), unchecked((byte)((d & unchecked(0x00FF0000)) >> 16)), unchecked((byte)((d & unchecked(0x0000FF00)) >> 8)), unchecked((byte)((d & unchecked(
                0x000000FF)))) };
            return Encoding.UTF8.GetString(b);
        }
    }
}
