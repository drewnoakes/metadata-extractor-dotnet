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
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using MetadataExtractor.Util;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Icc
{
    /// <summary>Reads ICC profile data.</summary>
    /// <remarks>
    /// ICC is the International Color Consortium.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/ICC_profile</item>
    ///   <item>http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/ICC_Profile.html</item>
    ///   <item>https://developer.apple.com/library/mac/samplecode/ImageApp/Listings/ICC_h.html</item>
    /// </list>
    /// </remarks>
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IccReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "ICC_PROFILE"; // TODO what are the extra three bytes here? are they always the same?
        private static readonly byte[] _jpegSegmentPreambleBytes = Encoding.UTF8.GetBytes(JpegSegmentPreamble);

        // NOTE the header is 14 bytes, while "ICC_PROFILE" is 11
        private const int JpegSegmentPreambleLength = 14;

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.App2 };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // ICC data can be spread across multiple JPEG segments.

            // Skip any segments that do not contain the required preamble
            var iccSegments = segments.Where(segment => segment.Bytes.Length > JpegSegmentPreambleLength && IsSubarrayEqualTo(segment.Bytes, 0, _jpegSegmentPreambleBytes)).ToList();

            if (iccSegments.Count == 0)
                return new Directory[0];

            byte[] buffer;
            if (iccSegments.Count == 1)
            {
                buffer = new byte[iccSegments[0].Bytes.Length - JpegSegmentPreambleLength];
                Array.Copy(iccSegments[0].Bytes, JpegSegmentPreambleLength, buffer, 0, iccSegments[0].Bytes.Length - JpegSegmentPreambleLength);
            }
            else
            {
                // Concatenate all buffers
                var totalLength = iccSegments.Sum(s => s.Bytes.Length - JpegSegmentPreambleLength);
                buffer = new byte[totalLength];
                for (int i = 0, pos = 0; i < iccSegments.Count; i++)
                {
                    var segment = iccSegments[i];
                    Array.Copy(segment.Bytes, JpegSegmentPreambleLength, buffer, pos, segment.Bytes.Length - JpegSegmentPreambleLength);
                    pos += segment.Bytes.Length - JpegSegmentPreambleLength;
                }
            }

            return new Directory[] { Extract(new ByteArrayReader(buffer)) };
        }

        [NotNull]
        public IccDirectory Extract([NotNull] IndexedReader reader)
        {
            // TODO review whether the 'tagPtr' values below really do require IndexedReader or whether SequentialReader may be used instead
            var directory = new IccDirectory();

            try
            {
                var profileByteCount = reader.GetInt32(IccDirectory.TagProfileByteCount);
                directory.Set(IccDirectory.TagProfileByteCount, profileByteCount);

                // For these tags, the int value of the tag is in fact its offset within the buffer.
                Set4ByteString(directory, IccDirectory.TagCmmType, reader);
                SetInt32(directory, IccDirectory.TagProfileVersion, reader);
                Set4ByteString(directory, IccDirectory.TagProfileClass, reader);
                Set4ByteString(directory, IccDirectory.TagColorSpace, reader);
                Set4ByteString(directory, IccDirectory.TagProfileConnectionSpace, reader);
                SetDate(directory, IccDirectory.TagProfileDateTime, reader);
                Set4ByteString(directory, IccDirectory.TagSignature, reader);
                Set4ByteString(directory, IccDirectory.TagPlatform, reader);
                SetInt32(directory, IccDirectory.TagCmmFlags, reader);
                Set4ByteString(directory, IccDirectory.TagDeviceMake, reader);

                var model = reader.GetInt32(IccDirectory.TagDeviceModel);
                if (model != 0)
                {
                    directory.Set(IccDirectory.TagDeviceModel, model <= 0x20202020
                        ? (object)model
                        : GetStringFromUInt32(unchecked((uint)model)));
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
            catch (Exception ex)
            {
                directory.AddError("Exception reading ICC profile: " + ex.Message);
            }

            return directory;
        }

        private static void Set4ByteString([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var i = reader.GetUInt32(tagType);
            if (i != 0)
                directory.Set(tagType, GetStringFromUInt32(i));
        }

        private static void SetInt32([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var i = reader.GetInt32(tagType);
            if (i != 0)
                directory.Set(tagType, i);
        }

        private static void SetInt64([NotNull] Directory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var l = reader.GetInt64(tagType);
            if (l != 0)
                directory.Set(tagType, l);
        }

        private static void SetDate([NotNull] IccDirectory directory, int tagType, [NotNull] IndexedReader reader)
        {
            var year = reader.GetUInt16(tagType);
            var month = reader.GetUInt16(tagType + 2);
            var day = reader.GetUInt16(tagType + 4);
            var hours = reader.GetUInt16(tagType + 6);
            var minutes = reader.GetUInt16(tagType + 8);
            var seconds = reader.GetUInt16(tagType + 10);

            if (DateUtil.IsValidDate(year, month, day) &&
                DateUtil.IsValidTime(hours, minutes, seconds))
                directory.Set(tagType, new DateTime(year, month, day, hours, minutes, seconds, kind: DateTimeKind.Utc));
            else
                directory.AddError($"ICC data describes an invalid date/time: year={year} month={month} day={day} hour={hours} minute={minutes} second={seconds}");
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

            return Encoding.UTF8.GetString(b, 0, b.Length);
        }

        private static bool IsSubarrayEqualTo<T>([NotNull] T[] source, int sourceIndex, [NotNull] T[] pattern)
        {
            if (sourceIndex + pattern.Length >= source.Length)
                return false;

            for (int i = sourceIndex, j = 0; j < pattern.Length; i++, j++)
            {
                if (!source[i].Equals(pattern[j]))
                    return false;
            }

            return true;
        }
    }
}
