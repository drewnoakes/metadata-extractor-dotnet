// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusImageProcessingMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions converted from Exiftool version 10.33 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusImageProcessingMakernoteDescriptor : TagDescriptor<OlympusImageProcessingMakernoteDirectory>
    {
        public OlympusImageProcessingMakernoteDescriptor(OlympusImageProcessingMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                OlympusImageProcessingMakernoteDirectory.TagImageProcessingVersion => GetImageProcessingVersionDescription(),
                OlympusImageProcessingMakernoteDirectory.TagColorMatrix => GetColorMatrixDescription(),
                OlympusImageProcessingMakernoteDirectory.TagNoiseReduction2 => GetNoiseReduction2Description(),
                OlympusImageProcessingMakernoteDirectory.TagDistortionCorrection2 => GetDistortionCorrection2Description(),
                OlympusImageProcessingMakernoteDirectory.TagShadingCompensation2 => GetShadingCompensation2Description(),
                OlympusImageProcessingMakernoteDirectory.TagMultipleExposureMode => GetMultipleExposureModeDescription(),
                OlympusImageProcessingMakernoteDirectory.TagAspectRatio => GetAspectRatioDescription(),
                OlympusImageProcessingMakernoteDirectory.TagKeystoneCompensation => GetKeystoneCompensationDescription(),
                OlympusImageProcessingMakernoteDirectory.TagKeystoneDirection => GetKeystoneDirectionDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetImageProcessingVersionDescription()
        {
            return GetVersionBytesDescription(OlympusImageProcessingMakernoteDirectory.TagImageProcessingVersion, 4);
        }

        public string? GetColorMatrixDescription()
        {
            if (!(Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagColorMatrix) is short[] values))
                return null;

            var str = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i != 0)
                    str.Append(' ');
                str.Append(values[i]);
            }

            return str.ToString();
        }

        public string? GetNoiseReduction2Description()
        {
            if (!Directory.TryGetInt32(OlympusImageProcessingMakernoteDirectory.TagNoiseReduction2, out int value))
                return null;

            if (value == 0)
                return "(none)";

            var sb = new StringBuilder();
            var v = (ushort)value;

            if (( v       & 1) != 0) sb.Append("Noise Reduction, ");
            if (((v >> 1) & 1) != 0) sb.Append("Noise Filter, ");
            if (((v >> 2) & 1) != 0) sb.Append("Noise Filter (ISO Boost), ");

            return sb.ToString(0, sb.Length - 2);
        }

        public string? GetDistortionCorrection2Description()
        {
            return GetIndexedDescription(OlympusImageProcessingMakernoteDirectory.TagDistortionCorrection2,
                "Off", "On");
        }

        public string? GetShadingCompensation2Description()
        {
            return GetIndexedDescription(OlympusImageProcessingMakernoteDirectory.TagShadingCompensation2,
                "Off", "On");
        }

        public string? GetMultipleExposureModeDescription()
        {
            var values = Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagMultipleExposureMode) as ushort[];
            if (values == null)
            {
                // check if it's only one value long also
                if (!Directory.TryGetInt32(OlympusImageProcessingMakernoteDirectory.TagMultipleExposureMode, out int value))
                    return null;

                values = new ushort[1];
                values[0] = (ushort)value;
            }

            if (values.Length == 0)
                return null;

            var sb = new StringBuilder();

            switch (values[0])
            {
                case 0:
                    sb.Append("Off");
                    break;
                case 2:
                    sb.Append("On (2 frames)");
                    break;
                case 3:
                    sb.Append("On (3 frames)");
                    break;
                default:
                    sb.Append("Unknown (" + values[0] + ")");
                    break;
            }

            if (values.Length > 1)
                sb.Append("; " + values[1]);

            return sb.ToString();
        }

        public string? GetAspectRatioDescription()
        {
            if (!(Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagAspectRatio) is byte[] values) || values.Length < 2)
                return null;

            var join = $"{values[0]} {values[1]}";
            var ret = join switch
            {
                "1 1" => "4:3",
                "1 4" => "1:1",
                "2 1" => "3:2 (RAW)",
                "2 2" => "3:2",
                "3 1" => "16:9 (RAW)",
                "3 3" => "16:9",
                "4 1" => "1:1 (RAW)",
                "4 4" => "6:6",
                "5 5" => "5:4",
                "6 6" => "7:6",
                "7 7" => "6:5",
                "8 8" => "7:5",
                "9 1" => "3:4 (RAW)",
                "9 9" => "3:4",
                _ => "Unknown (" + join + ")",
            };
            return ret;
        }

        public string? GetKeystoneCompensationDescription()
        {
            if (!(Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagKeystoneCompensation) is byte[] values) || values.Length < 2)
                return null;

            var join = $"{values[0]} {values[1]}";
            var ret = join switch
            {
                "0 0" => "Off",
                "0 1" => "On",
                _ => "Unknown (" + join + ")",
            };
            return ret;
        }

        public string? GetKeystoneDirectionDescription()
        {
            return GetIndexedDescription(OlympusImageProcessingMakernoteDirectory.TagKeystoneDirection,
                "Vertical", "Horizontal");
        }
    }
}
