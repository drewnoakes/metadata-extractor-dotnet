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
    public sealed class IccReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "ICC_PROFILE";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.App2;
        }

        public IReadOnlyList<Directory> ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
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
                return new[] { Extract(new ByteArrayReader(buffer)) };

            return new Directory[0];
        }

        public IccDirectory Extract(IndexedReader reader)
        {
            // TODO review whether the 'tagPtr' values below really do require IndexedReader or whether SequentialReader may be used instead
            var directory = new IccDirectory();
            try
            {
                var profileByteCount = reader.GetInt32(IccDirectory.TagProfileByteCount);
                directory.Set(IccDirectory.TagProfileByteCount, profileByteCount);
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
                    if (temp <= 0x20202020)
                    {
                        directory.Set(IccDirectory.TagDeviceModel, temp);
                    }
                    else
                    {
                        directory.Set(IccDirectory.TagDeviceModel, GetStringFromUInt32(unchecked((uint)temp)));
                    }
                }
                SetInt32(directory, IccDirectory.TagRenderingIntent, reader);
                SetInt64(directory, IccDirectory.TagDeviceAttr, reader);
                var xyz = new[] { reader.GetS15Fixed16(IccDirectory.TagXyzValues), reader.GetS15Fixed16(IccDirectory.TagXyzValues + 4), reader.GetS15Fixed16(IccDirectory.TagXyzValues + 8) };
                directory.Set(IccDirectory.TagXyzValues, xyz);
                // Process 'ICC tags'
                var tagCount = reader.GetInt32(IccDirectory.TagTagCount);
                directory.Set(IccDirectory.TagTagCount, tagCount);
                for (var i = 0; i < tagCount; i++)
                {
                    var pos = IccDirectory.TagTagCount + 4 + i * 12;
                    var tagType = reader.GetInt32(pos);
                    var tagPtr = reader.GetInt32(pos + 4);
                    var tagLen = reader.GetInt32(pos + 8);
                    var b = reader.GetBytes(tagPtr, tagLen);
                    directory.Set(tagType, b);
                }
            }
            catch (IOException ex)
            {
                directory.AddError("Exception reading ICC profile: " + ex.Message);
            }
            return directory;
        }

        /// <exception cref="System.IO.IOException"/>
        private static void Set4ByteString([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var i = reader.GetUInt32(tagType);
            if (i != 0)
            {
                directory.Set(tagType, GetStringFromUInt32(i));
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void SetInt32([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var i = reader.GetInt32(tagType);
            if (i != 0)
            {
                directory.Set(tagType, i);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void SetInt64([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var l = reader.GetInt64(tagType);
            if (l != 0)
            {
                directory.Set(tagType, l);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void SetDate([NotNull] IccDirectory directory, int tagType, [NotNull] IndexedReader reader)
        {
            directory.Set(
                tagType,
                new DateTime(year:   reader.GetUInt16(tagType),
                             month:  reader.GetUInt16(tagType + 2),
                             day:    reader.GetUInt16(tagType + 4),
                             hour:   reader.GetUInt16(tagType + 6),
                             minute: reader.GetUInt16(tagType + 8),
                             second: reader.GetUInt16(tagType + 10),
                             kind: DateTimeKind.Utc));
        }

        [NotNull]
        public static string GetStringFromUInt32(uint d)
        {
            // MSB
            var b = new[]
            {
                unchecked((byte)(d >> 24)),
                unchecked((byte)(d >> 16)),
                unchecked((byte)(d >> 8)),
                unchecked((byte)d)
            };

            return Encoding.UTF8.GetString(b);
        }
    }
}
