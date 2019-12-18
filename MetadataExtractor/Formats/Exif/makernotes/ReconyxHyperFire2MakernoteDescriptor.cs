using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ReconyxHyperFire2MakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Reconyx uses a fixed makernote block. Tag values are the byte index of the tag within the makernote.</remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ReconyxHyperFire2MakernoteDescriptor : TagDescriptor<ReconyxHyperFire2MakernoteDirectory>
    {
        public ReconyxHyperFire2MakernoteDescriptor(ReconyxHyperFire2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ReconyxHyperFire2MakernoteDirectory.TagFileNumber:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagDirectoryNumber:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagFirmwareVersion:
                    var version = Directory.GetObject(tagType) as Version;
                    if (version != null)
                        return $"{version.Major}.{version.Minor}{(char)version.Build}";
                    else
                        return string.Empty;
                case ReconyxHyperFire2MakernoteDirectory.TagFirmwareDate:
                    return Directory.GetDateTime(tagType).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                case ReconyxHyperFire2MakernoteDirectory.TagTriggerMode:
                    return Directory.GetString(tagType) switch
                    {
                        "M" => "Motion Detection",
                        "P" => "Point and Shoot",
                        "T" => "Time Lapse",
                        _ => "Unknown trigger mode",
                    };
                case ReconyxHyperFire2MakernoteDirectory.TagSequence:
                    var sequence = Directory.GetInt32Array(tagType);
                    return sequence != null ? $"{sequence[0]}/{sequence[1]}" : base.GetDescription(tagType);
                case ReconyxHyperFire2MakernoteDirectory.TagEventNumber:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagDateTimeOriginal:
                    return Directory.GetDateTime(tagType).ToString("yyyy:MM:dd HH:mm:ss");
                case ReconyxHyperFire2MakernoteDirectory.TagDayOfWeek:
                    return GetIndexedDescription(tagType, CultureInfo.CurrentCulture.DateTimeFormat.DayNames);
                case ReconyxHyperFire2MakernoteDirectory.TagMoonPhase:
                    return GetIndexedDescription(tagType, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");
                case ReconyxHyperFire2MakernoteDirectory.TagAmbientTemperatureFahrenheit:
                    return $"{Directory.GetInt16(tagType)}°F";
                case ReconyxHyperFire2MakernoteDirectory.TagAmbientTemperature:
                    return $"{Directory.GetInt16(tagType)}°C";
                case ReconyxHyperFire2MakernoteDirectory.TagContrast:
                case ReconyxHyperFire2MakernoteDirectory.TagBrightness:
                case ReconyxHyperFire2MakernoteDirectory.TagSharpness:
                case ReconyxHyperFire2MakernoteDirectory.TagSaturation:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagFlash:
                    return GetIndexedDescription(tagType, "Off", "On");
                case ReconyxHyperFire2MakernoteDirectory.TagAmbientInfrared:
                case ReconyxHyperFire2MakernoteDirectory.TagAmbientLight:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagMotionSensitivity:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagBatteryVoltage:
                case ReconyxHyperFire2MakernoteDirectory.TagBatteryVoltageAvg:
                    return Directory.GetDouble(tagType).ToString("0.000");
                case ReconyxHyperFire2MakernoteDirectory.TagBatteryType:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFire2MakernoteDirectory.TagUserLabel:
                case ReconyxHyperFire2MakernoteDirectory.TagSerialNumber:
                    return Directory.GetString(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
