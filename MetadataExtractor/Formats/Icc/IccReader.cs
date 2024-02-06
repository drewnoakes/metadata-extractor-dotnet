// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using MetadataExtractor.Formats.Jpeg;

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
        public static ReadOnlySpan<byte> JpegSegmentPreamble => "ICC_PROFILE"u8; // TODO what are the extra three bytes here? are they always the same?

        // NOTE the header is 14 bytes, while "ICC_PROFILE" is 11
        private const int JpegSegmentPreambleLength = 14;

        IReadOnlyCollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = [JpegSegmentType.App2];

        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // ICC data can be spread across multiple JPEG segments.

            // Skip any segments that do not contain the required preamble
            var iccSegments = segments.Where(segment => segment.Span.StartsWith(JpegSegmentPreamble)).ToList();

            if (iccSegments.Count == 0)
                return [];

            byte[] buffer;
            if (iccSegments.Count == 1)
            {
                buffer = ArrayPool<byte>.Shared.Rent(iccSegments[0].Bytes.Length - JpegSegmentPreambleLength);
                Array.Copy(iccSegments[0].Bytes, JpegSegmentPreambleLength, buffer, 0, iccSegments[0].Bytes.Length - JpegSegmentPreambleLength);
            }
            else
            {
                // Concatenate all buffers
                var totalLength = iccSegments.Sum(s => s.Bytes.Length - JpegSegmentPreambleLength);
                buffer = ArrayPool<byte>.Shared.Rent(totalLength);
                for (int i = 0, pos = 0; i < iccSegments.Count; i++)
                {
                    var segment = iccSegments[i];
                    Array.Copy(segment.Bytes, JpegSegmentPreambleLength, buffer, pos, segment.Bytes.Length - JpegSegmentPreambleLength);
                    pos += segment.Bytes.Length - JpegSegmentPreambleLength;
                }
            }

            Directory directory = Extract(buffer);

            ArrayPool<byte>.Shared.Return(buffer);

            return [directory];
        }

        public IccDirectory Extract(ReadOnlySpan<byte> bytes)
        {
            var reader = new BufferReader(bytes, isBigEndian: true);

            // TODO review whether the 'tagPtr' values below really do require IndexedReader or whether SequentialReader may be used instead
            var directory = new IccDirectory();

            try
            {
                var profileByteCount = reader.GetInt32(IccDirectory.TagProfileByteCount);
                directory.Set(IccDirectory.TagProfileByteCount, profileByteCount);

                // For these tags, the int value of the tag is in fact its offset within the buffer.
                Set4ByteString(directory, IccDirectory.TagCmmType, ref reader);
                SetInt32(directory, IccDirectory.TagProfileVersion, ref reader);
                Set4ByteString(directory, IccDirectory.TagProfileClass, ref reader);
                Set4ByteString(directory, IccDirectory.TagColorSpace, ref reader);
                Set4ByteString(directory, IccDirectory.TagProfileConnectionSpace, ref reader);
                SetDate(directory, IccDirectory.TagProfileDateTime, ref reader);
                Set4ByteString(directory, IccDirectory.TagSignature, ref reader);
                Set4ByteString(directory, IccDirectory.TagPlatform, ref reader);
                SetInt32(directory, IccDirectory.TagCmmFlags, ref reader);
                Set4ByteString(directory, IccDirectory.TagDeviceMake, ref reader);

                var model = reader.GetInt32(IccDirectory.TagDeviceModel);
                if (model != 0)
                {
                    directory.Set(IccDirectory.TagDeviceModel, model <= 0x20202020
                        ? model
                        : GetStringFromUInt32(unchecked((uint)model)));
                }

                SetInt32(directory, IccDirectory.TagRenderingIntent, ref reader);
                SetInt64(directory, IccDirectory.TagDeviceAttr, ref reader);

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
                    byte[] b = new byte[tagLen];

                    reader.GetBytes(tagPtr, b);
                    directory.Set(tagType, b);
                }
            }
            catch (Exception ex)
            {
                directory.AddError("Exception reading ICC profile: " + ex.Message);
            }

            return directory;
        }

        private static void Set4ByteString(Directory directory, int tagType, ref BufferReader reader)
        {
            var i = reader.GetUInt32(tagType);
            if (i != 0)
                directory.Set(tagType, GetStringFromUInt32(i));
        }

        private static void SetInt32(Directory directory, int tagType, ref BufferReader reader)
        {
            var i = reader.GetInt32(tagType);
            if (i != 0)
                directory.Set(tagType, i);
        }

        private static void SetInt64(Directory directory, int tagType, ref BufferReader reader)
        {
            var l = reader.GetInt64(tagType);
            if (l != 0)
                directory.Set(tagType, l);
        }

        private static void SetDate(IccDirectory directory, int tagType, ref BufferReader reader)
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
