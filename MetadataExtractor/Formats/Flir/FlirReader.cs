// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using MetadataExtractor.Formats.Jpeg;
using static MetadataExtractor.Formats.Flir.FlirCameraInfoDirectory;

namespace MetadataExtractor.Formats.Flir
{
    public class FlirReader : IJpegSegmentMetadataReader
    {
        /// <summary>Flir metadata stored in JPEG files' APP1 segment are preceded by this six character preamble "FLIR\0\0".</summary>
        public const string JpegSegmentPreamble = "FLIR\0";

        public bool ExtractRawThermalImage { get; set; }

        private ReadOnlySpan<byte> PreambleBytes => "FLIR\0"u8;

        public IReadOnlyCollection<JpegSegmentType> SegmentTypes { get; } = [JpegSegmentType.App1];

        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            var preamble = PreambleBytes;
            var preambleLength = preamble.Length + 3;

            int length = 0;

            foreach (var segment in segments)
            {
                // Skip segments not starting with the required preamble
                if (segment.Span.StartsWith(preamble))
                {
                    length += segment.Bytes.Length - preambleLength;
                }
            }

            if (length == 0)
                return [];

            byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                using var merged = new MemoryStream(buffer);

                foreach (var segment in segments)
                {
                    // Skip segments not starting with the required preamble
                    if (segment.Span.StartsWith(preamble))
                    {
                        merged.Write(segment.Bytes, preambleLength, segment.Bytes.Length - preambleLength);
                    }
                }

                return Extract(new ByteArrayReader(buffer));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public IEnumerable<Directory> Extract(IndexedReader reader)
        {
            Span<byte> header = stackalloc byte[4];
            reader.GetBytes(0, header);

            if (!header.SequenceEqual("FFF\0"u8))
            {
                var flirHeaderDirectory = new FlirHeaderDirectory();
                flirHeaderDirectory.AddError("Unexpected FFF header bytes.");
                yield return flirHeaderDirectory;
                yield break;
            }

            var headerDirectory = new FlirHeaderDirectory();
            headerDirectory.Set(FlirHeaderDirectory.TagCreatorSoftware, reader.GetNullTerminatedStringValue(4, 16));
            yield return headerDirectory;

            var baseIndexOffset = reader.GetUInt32(24);
            var indexCount = reader.GetUInt32(28);

            var indexOffset = checked((int)baseIndexOffset);
            for (int i = 0; i < indexCount; i++)
            {
                var mainType = (FlirMainTagType)reader.GetUInt16(indexOffset);
                var subType = reader.GetUInt16(indexOffset + 2); // 1=BE, 2=LE, 3=PNG; 1 for other record types
                var version = reader.GetUInt32(indexOffset + 4);
                var id = reader.GetUInt32(indexOffset + 8);
                var dataOffset = reader.GetInt32(indexOffset + 12);
                var dataLength = reader.GetInt32(indexOffset + 16);

                if (mainType == FlirMainTagType.Pixels)
                {
                    var reader2 = reader.WithShiftedBaseOffset(dataOffset);

                    // should be 0x0002 if byte order is correct
                    var marker = reader2.GetUInt16(0);
                    if (marker > 0x0100)
                        reader2 = reader2.WithByteOrder(!reader2.IsMotorolaByteOrder);

                    var directory = new FlirRawDataDirectory();

                    directory.Set(FlirRawDataDirectory.TagRawThermalImageWidth, reader2.GetUInt16(FlirRawDataDirectory.TagRawThermalImageWidth));
                    directory.Set(FlirRawDataDirectory.TagRawThermalImageHeight, reader2.GetUInt16(FlirRawDataDirectory.TagRawThermalImageHeight));
                    directory.Set(FlirRawDataDirectory.TagRawThermalImageType, reader2.GetUInt16(FlirRawDataDirectory.TagRawThermalImageType));

                    if (ExtractRawThermalImage)
                    {
                        directory.Set(FlirRawDataDirectory.TagRawThermalImage, reader2.GetBytes(32, dataLength - 32));
                    }

                    yield return directory;
                }
                else if (mainType == FlirMainTagType.BasicData)
                {
                    var reader2 = reader.WithShiftedBaseOffset(dataOffset);

                    // should be 0x0002 if byte order is correct
                    var marker = reader2.GetUInt16(0);
                    if (marker > 0x0100)
                        reader2 = reader2.WithByteOrder(!reader2.IsMotorolaByteOrder);

                    var directory = new FlirCameraInfoDirectory();

                    directory.Set(TagEmissivity, reader2.GetFloat32(TagEmissivity));
                    directory.Set(TagObjectDistance, reader2.GetFloat32(TagObjectDistance));
                    directory.Set(TagReflectedApparentTemperature, reader2.GetFloat32(TagReflectedApparentTemperature));
                    directory.Set(TagAtmosphericTemperature, reader2.GetFloat32(TagAtmosphericTemperature));
                    directory.Set(TagIRWindowTemperature, reader2.GetFloat32(TagIRWindowTemperature));
                    directory.Set(TagIRWindowTransmission, reader2.GetFloat32(TagIRWindowTransmission));
                    directory.Set(TagRelativeHumidity, reader2.GetFloat32(TagRelativeHumidity));
                    directory.Set(TagPlanckR1, reader2.GetFloat32(TagPlanckR1));
                    directory.Set(TagPlanckB, reader2.GetFloat32(TagPlanckB));
                    directory.Set(TagPlanckF, reader2.GetFloat32(TagPlanckF));
                    directory.Set(TagAtmosphericTransAlpha1, reader2.GetFloat32(TagAtmosphericTransAlpha1));
                    directory.Set(TagAtmosphericTransAlpha2, reader2.GetFloat32(TagAtmosphericTransAlpha2));
                    directory.Set(TagAtmosphericTransBeta1, reader2.GetFloat32(TagAtmosphericTransBeta1));
                    directory.Set(TagAtmosphericTransBeta2, reader2.GetFloat32(TagAtmosphericTransBeta2));
                    directory.Set(TagAtmosphericTransX, reader2.GetFloat32(TagAtmosphericTransX));
                    directory.Set(TagCameraTemperatureRangeMax, reader2.GetFloat32(TagCameraTemperatureRangeMax));
                    directory.Set(TagCameraTemperatureRangeMin, reader2.GetFloat32(TagCameraTemperatureRangeMin));
                    directory.Set(TagCameraTemperatureMaxClip, reader2.GetFloat32(TagCameraTemperatureMaxClip));
                    directory.Set(TagCameraTemperatureMinClip, reader2.GetFloat32(TagCameraTemperatureMinClip));
                    directory.Set(TagCameraTemperatureMaxWarn, reader2.GetFloat32(TagCameraTemperatureMaxWarn));
                    directory.Set(TagCameraTemperatureMinWarn, reader2.GetFloat32(TagCameraTemperatureMinWarn));
                    directory.Set(TagCameraTemperatureMaxSaturated, reader2.GetFloat32(TagCameraTemperatureMaxSaturated));
                    directory.Set(TagCameraTemperatureMinSaturated, reader2.GetFloat32(TagCameraTemperatureMinSaturated));
                    directory.Set(TagCameraModel, reader2.GetNullTerminatedStringValue(TagCameraModel, 32));
                    directory.Set(TagCameraPartNumber, reader2.GetNullTerminatedStringValue(TagCameraPartNumber, 16));
                    directory.Set(TagCameraSerialNumber, reader2.GetNullTerminatedStringValue(TagCameraSerialNumber, 16));
                    directory.Set(TagCameraSoftware, reader2.GetNullTerminatedStringValue(TagCameraSoftware, 16));
                    directory.Set(TagLensModel, reader2.GetNullTerminatedStringValue(TagLensModel, 32));
                    directory.Set(TagLensPartNumber, reader2.GetNullTerminatedStringValue(TagLensPartNumber, 16));
                    directory.Set(TagLensSerialNumber, reader2.GetNullTerminatedStringValue(TagLensSerialNumber, 16));
                    directory.Set(TagFieldOfView, reader2.GetFloat32(TagFieldOfView));
                    directory.Set(TagFilterModel, reader2.GetNullTerminatedStringValue(TagFilterModel, 16));
                    directory.Set(TagFilterPartNumber, reader2.GetNullTerminatedStringValue(TagFilterPartNumber, 32));
                    directory.Set(TagFilterSerialNumber, reader2.GetNullTerminatedStringValue(TagFilterSerialNumber, 32));
                    directory.Set(TagPlanckO, reader2.GetInt32(TagPlanckO));
                    directory.Set(TagPlanckR2, reader2.GetFloat32(TagPlanckR2));
                    directory.Set(TagRawValueRangeMin, reader2.GetUInt16(TagRawValueRangeMin));
                    directory.Set(TagRawValueRangeMax, reader2.GetUInt16(TagRawValueRangeMax));
                    directory.Set(TagRawValueMedian, reader2.GetUInt16(TagRawValueMedian));
                    directory.Set(TagRawValueRange, reader2.GetUInt16(TagRawValueRange));

                    Span<byte> dateTimeBytes = stackalloc byte[10];
                    reader2.GetBytes(TagDateTimeOriginal, dateTimeBytes);
                    var dateTimeReader = new BufferReader(dateTimeBytes, isBigEndian: false);
                    var tm = dateTimeReader.GetUInt32();
                    var ss = dateTimeReader.GetUInt32() & 0xffff;
                    var tz = dateTimeReader.GetInt16();
                    directory.Set(TagDateTimeOriginal, new DateTimeOffset(
                        dateTime: DateUtil.FromUnixTime(tm - tz * 60).AddSeconds(ss / 1000d),
                        offset: TimeSpan.FromMinutes(tz)));

                    directory.Set(TagFocusStepCount, reader2.GetUInt16(TagFocusStepCount));
                    directory.Set(TagFocusDistance, reader2.GetFloat32(TagFocusDistance));
                    directory.Set(TagFrameRate, reader2.GetUInt16(TagFrameRate));

                    yield return directory;
                }

                indexOffset += 32;
            }
        }
    }
}
