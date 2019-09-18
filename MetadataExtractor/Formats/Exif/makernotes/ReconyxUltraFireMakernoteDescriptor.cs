// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ReconyxHyperFireMakernoteDirectory"/>.
    /// </summary>
    /// <author>Todd West http://cascadescarnivoreproject.blogspot.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ReconyxUltraFireMakernoteDescriptor : TagDescriptor<ReconyxUltraFireMakernoteDirectory>
    {
        public ReconyxUltraFireMakernoteDescriptor(ReconyxUltraFireMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ReconyxUltraFireMakernoteDirectory.TagLabel:
                    return Directory.GetString(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagMakernoteID:
                    return "0x" + Directory.GetUInt32(tagType).ToString("x8");
                case ReconyxUltraFireMakernoteDirectory.TagMakernoteSize:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagMakernotePublicID:
                    return "0x" + Directory.GetUInt32(tagType).ToString("x8");
                case ReconyxUltraFireMakernoteDirectory.TagMakernotePublicSize:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagAmbientTemperatureFahrenheit:
                case ReconyxUltraFireMakernoteDirectory.TagAmbientTemperature:
                    return Directory.GetInt16(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagCameraVersion:
                case ReconyxUltraFireMakernoteDirectory.TagUibVersion:
                case ReconyxUltraFireMakernoteDirectory.TagBtlVersion:
                case ReconyxUltraFireMakernoteDirectory.TagPexVersion:
                case ReconyxUltraFireMakernoteDirectory.TagEventType:
                    return Directory.GetString(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagSequence:
                    var sequence = Directory.GetInt32Array(tagType);
                    return sequence != null ? $"{sequence[0]}/{sequence[1]}" : base.GetDescription(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagEventNumber:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagDateTimeOriginal:
                    return Directory.GetDateTime(tagType).ToString("yyyy:MM:dd HH:mm:ss");
                case ReconyxUltraFireMakernoteDirectory.TagDayOfWeek:
                    return GetIndexedDescription(tagType, CultureInfo.CurrentCulture.DateTimeFormat.DayNames);
                case ReconyxUltraFireMakernoteDirectory.TagMoonPhase:
                    return GetIndexedDescription(tagType, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");
                case ReconyxUltraFireMakernoteDirectory.TagFlash:
                    return GetIndexedDescription(tagType, "Off", "On");
                case ReconyxUltraFireMakernoteDirectory.TagSerialNumber:
                    return Directory.GetString(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagBatteryVoltage:
                    return Directory.GetDouble(tagType).ToString("0.000");
                case ReconyxUltraFireMakernoteDirectory.TagUserLabel:
                    return Directory.GetString(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
