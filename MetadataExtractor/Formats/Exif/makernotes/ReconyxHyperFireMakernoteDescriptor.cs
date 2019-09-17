// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ReconyxHyperFireMakernoteDirectory"/>.
    /// </summary>
    /// <author>Todd West http://cascadescarnivoreproject.blogspot.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ReconyxHyperFireMakernoteDescriptor : TagDescriptor<ReconyxHyperFireMakernoteDirectory>
    {
        public ReconyxHyperFireMakernoteDescriptor(ReconyxHyperFireMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ReconyxHyperFireMakernoteDirectory.TagMakernoteVersion:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagFirmwareVersion:
                    // invokes Version.ToString()
                    return Directory.GetString(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagTriggerMode:
                    return Directory.GetString(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagSequence:
                    var sequence = Directory.GetInt32Array(tagType);
                    return sequence != null ? $"{sequence[0]}/{sequence[1]}" : base.GetDescription(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagEventNumber:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagMotionSensitivity:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagBatteryVoltage:
                    return Directory.GetDouble(tagType).ToString("0.000");
                case ReconyxHyperFireMakernoteDirectory.TagDateTimeOriginal:
                    return Directory.GetDateTime(tagType).ToString("yyyy:MM:dd HH:mm:ss");
                case ReconyxHyperFireMakernoteDirectory.TagMoonPhase:
                    return GetIndexedDescription(tagType, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");
                case ReconyxHyperFireMakernoteDirectory.TagAmbientTemperatureFahrenheit:
                case ReconyxHyperFireMakernoteDirectory.TagAmbientTemperature:
                    return Directory.GetInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagSerialNumber:
                    return Directory.GetString(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagContrast:
                case ReconyxHyperFireMakernoteDirectory.TagBrightness:
                case ReconyxHyperFireMakernoteDirectory.TagSharpness:
                case ReconyxHyperFireMakernoteDirectory.TagSaturation:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagInfraredIlluminator:
                    return GetIndexedDescription(tagType, "Off", "On");
                case ReconyxHyperFireMakernoteDirectory.TagUserLabel:
                    return Directory.GetString(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
