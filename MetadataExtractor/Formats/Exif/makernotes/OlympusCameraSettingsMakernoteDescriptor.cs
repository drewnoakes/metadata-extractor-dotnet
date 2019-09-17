// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusCameraSettingsMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Many Description functions and the Filter type list converted from Exiftool version 10.10 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusCameraSettingsMakernoteDescriptor : TagDescriptor<OlympusCameraSettingsMakernoteDirectory>
    {
        public OlympusCameraSettingsMakernoteDescriptor(OlympusCameraSettingsMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                OlympusCameraSettingsMakernoteDirectory.TagCameraSettingsVersion => GetCameraSettingsVersionDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPreviewImageValid => GetPreviewImageValidDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagExposureMode => GetExposureModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagAeLock => GetAeLockDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagMeteringMode => GetMeteringModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagExposureShift => GetExposureShiftDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagNdFilter => GetNdFilterDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagMacroMode => GetMacroModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagFocusMode => GetFocusModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagFocusProcess => GetFocusProcessDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagAfSearch => GetAfSearchDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagAfAreas => GetAfAreasDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagAfPointSelected => GetAfPointSelectedDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagAfFineTune => GetAfFineTuneDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagFlashMode => GetFlashModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagFlashRemoteControl => GetFlashRemoteControlDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagFlashControlMode => GetFlashControlModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagFlashIntensity => GetFlashIntensityDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagManualFlashStrength => GetManualFlashStrengthDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagWhiteBalance2 => GetWhiteBalance2Description(),
                OlympusCameraSettingsMakernoteDirectory.TagWhiteBalanceTemperature => GetWhiteBalanceTemperatureDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagCustomSaturation => GetCustomSaturationDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagModifiedSaturation => GetModifiedSaturationDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagContrastSetting => GetContrastSettingDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagSharpnessSetting => GetSharpnessSettingDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagColorSpace => GetColorSpaceDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagSceneMode => GetSceneModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagNoiseReduction => GetNoiseReductionDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagDistortionCorrection => GetDistortionCorrectionDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagShadingCompensation => GetShadingCompensationDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagGradation => GetGradationDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureMode => GetPictureModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureModeSaturation => GetPictureModeSaturationDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureModeContrast => GetPictureModeContrastDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureModeSharpness => GetPictureModeSharpnessDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureModeBWFilter => GetPictureModeBWFilterDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureModeTone => GetPictureModeToneDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagNoiseFilter => GetNoiseFilterDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagArtFilter => GetArtFilterDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagMagicFilter => GetMagicFilterDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPictureModeEffect => GetPictureModeEffectDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagToneLevel => GetToneLevelDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagArtFilterEffect => GetArtFilterEffectDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagColorCreatorEffect => GetColorCreatorEffectDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagDriveMode => GetDriveModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPanoramaMode => GetPanoramaModeDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagImageQuality2 => GetImageQuality2Description(),
                OlympusCameraSettingsMakernoteDirectory.TagImageStabilization => GetImageStabilizationDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagStackedImage => GetStackedImageDescription(),

                OlympusCameraSettingsMakernoteDirectory.TagManometerPressure => GetManometerPressureDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagManometerReading => GetManometerReadingDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagExtendedWBDetect => GetExtendedWBDetectDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagRollAngle => GetRollAngleDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagPitchAngle => GetPitchAngleDescription(),
                OlympusCameraSettingsMakernoteDirectory.TagDateTimeUtc => GetDateTimeUtcDescription(),

                _ => base.GetDescription(tagType),
            };
        }

        public string? GetCameraSettingsVersionDescription()
        {
            return GetVersionBytesDescription(OlympusCameraSettingsMakernoteDirectory.TagCameraSettingsVersion, 4);
        }

        public string? GetPreviewImageValidDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagPreviewImageValid,
                "No", "Yes");
        }

        public string? GetExposureModeDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagExposureMode, 1,
                "Manual", "Program", "Aperture-priority AE", "Shutter speed priority", "Program-shift");
        }

        public string? GetAeLockDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagAeLock,
                "Off", "On");
        }

        public string? GetMeteringModeDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagMeteringMode, out int value))
                return null;

            return value switch
            {
                2 => "Center-weighted average",
                3 => "Spot",
                5 => "ESP",
                261 => "Pattern+AF",
                515 => "Spot+Highlight control",
                1027 => "Spot+Shadow control",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetExposureShiftDescription()
        {
            return GetRationalOrDoubleString(OlympusCameraSettingsMakernoteDirectory.TagExposureShift);
        }

        public string? GetNdFilterDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagNdFilter,
                "Off", "On");
        }

        public string? GetMacroModeDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagMacroMode,
                "Off", "On", "Super Macro");
        }

        public string? GetFocusModeDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagFocusMode) as ushort[];
            if (values == null)
            {
                // check if it's only one value long also
                if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagFocusMode, out int value))
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
                    sb.Append("Single AF");
                    break;
                case 1:
                    sb.Append("Sequential shooting AF");
                    break;
                case 2:
                    sb.Append("Continuous AF");
                    break;
                case 3:
                    sb.Append("Multi AF");
                    break;
                case 4:
                    sb.Append("Face detect");
                    break;
                case 10:
                    sb.Append("MF");
                    break;
                default:
                    sb.Append("Unknown (" + values[0] + ")");
                    break;
            }

            if (values.Length > 1)
            {
                sb.Append("; ");
                var value1 = values[1];

                if (value1 == 0)
                {
                    sb.Append("(none)");
                }
                else
                {
                    if (( value1       & 1) > 0) sb.Append("S-AF, ");
                    if (((value1 >> 2) & 1) > 0) sb.Append("C-AF, ");
                    if (((value1 >> 4) & 1) > 0) sb.Append("MF, ");
                    if (((value1 >> 5) & 1) > 0) sb.Append("Face detect, ");
                    if (((value1 >> 6) & 1) > 0) sb.Append("Imager AF, ");
                    if (((value1 >> 7) & 1) > 0) sb.Append("Live View Magnification Frame, ");
                    if (((value1 >> 8) & 1) > 0) sb.Append("AF sensor, ");

                    sb.Remove(sb.Length - 2, 2);
                }
            }

            return sb.ToString();
        }

        public string? GetFocusProcessDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagFocusProcess) as ushort[];
            if (values == null)
            {
                // check if it's only one value long also
                if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagFocusProcess, out int value))
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
                    sb.Append("AF not used");
                    break;
                case 1:
                    sb.Append("AF used");
                    break;
                default:
                    sb.Append("Unknown (" + values[0] + ")");
                    break;
            }

            if (values.Length > 1)
                sb.Append("; " + values[1]);

            return sb.ToString();
        }

        public string? GetAfSearchDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagAfSearch,
                "Not Ready", "Ready");
        }

        /// <summary>
        /// coordinates range from 0 to 255
        /// </summary>
        /// <returns></returns>
        public string? GetAfAreasDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagAfAreas) is uint[] points))
                return null;

            var sb = new StringBuilder();
            foreach (var point in points)
            {
                if (point == 0)
                    continue;
                if (sb.Length != 0)
                    sb.Append(", ");

                if (point == 0x36794285)
                    sb.Append("Left ");
                else if (point == 0x79798585)
                    sb.Append("Center ");
                else if (point == 0xBD79C985)
                    sb.Append("Right ");

                var bytesArray = BitConverter.GetBytes(point);
                sb.Append($"({bytesArray[3]}/255,{bytesArray[2]}/255)-({bytesArray[1]}/255,{bytesArray[0]}/255)");
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        /// <summary>
        /// coordinates expressed as a percent
        /// </summary>
        /// <returns></returns>
        public string? GetAfPointSelectedDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagAfPointSelected) is Rational[] vals))
                return "n/a";

            if (vals.Length < 4)
                return null;

            var index = 0;
            if (vals.Length == 5 && vals[0].ToInt64() == 0)
                index = 1;

            var p1 = (int)(vals[index    ].ToDouble() * 100);
            var p2 = (int)(vals[index + 1].ToDouble() * 100);
            var p3 = (int)(vals[index + 2].ToDouble() * 100);
            var p4 = (int)(vals[index + 3].ToDouble() * 100);

            if (p1 + p2 + p3 + p4 == 0)
                return "n/a";

            return $"({p1}%,{p2}%) ({p3}%,{p4}%)";

        }

        public string? GetAfFineTuneDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagAfFineTune,
                "Off", "On");
        }

        public string? GetFlashModeDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagFlashMode, out int value))
                return null;

            if (value == 0)
                return "Off";

            var sb = new StringBuilder();
            var v = (ushort)value;

            if (( v       & 1) != 0) sb.Append("On, ");
            if (((v >> 1) & 1) != 0) sb.Append("Fill-in, ");
            if (((v >> 2) & 1) != 0) sb.Append("Red-eye, ");
            if (((v >> 3) & 1) != 0) sb.Append("Slow-sync, ");
            if (((v >> 4) & 1) != 0) sb.Append("Forced On, ");
            if (((v >> 5) & 1) != 0) sb.Append("2nd Curtain, ");

            return sb.ToString(0, sb.Length - 2);
        }

        public string? GetFlashRemoteControlDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagFlashRemoteControl, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                0x01 => "Channel 1, Low",
                0x02 => "Channel 2, Low",
                0x03 => "Channel 3, Low",
                0x04 => "Channel 4, Low",
                0x09 => "Channel 1, Mid",
                0x0a => "Channel 2, Mid",
                0x0b => "Channel 3, Mid",
                0x0c => "Channel 4, Mid",
                0x11 => "Channel 1, High",
                0x12 => "Channel 2, High",
                0x13 => "Channel 3, High",
                0x14 => "Channel 4, High",

                _ => "Unknown (" + value + ")",
            };
        }

        /// <summary>
        /// 3 or 4 values
        /// </summary>
        /// <returns></returns>
        public string? GetFlashControlModeDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagFlashControlMode) is ushort[] values))
                return null;

            if (values.Length == 0)
                return null;

            var sb = new StringBuilder();

            switch (values[0])
            {
                case 0:
                    sb.Append("Off");
                    break;
                case 3:
                    sb.Append("TTL");
                    break;
                case 4:
                    sb.Append("Auto");
                    break;
                case 5:
                    sb.Append("Manual");
                    break;
                default:
                    sb.Append("Unknown (" + values[0] + ")");
                    break;
            }

            for (var i = 1; i < values.Length; i++)
                sb.Append("; ").Append(values[i]);

            return sb.ToString();
        }

        /// <summary>
        /// 3 or 4 values
        /// </summary>
        /// <returns></returns>
        public string? GetFlashIntensityDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagFlashIntensity) is Rational[] vals))
                return null;

            if (vals.Length == 3)
            {
                if (vals[0].Denominator == 0 && vals[1].Denominator == 0 && vals[2].Denominator == 0)
                    return "n/a";
            }
            else if (vals.Length == 4)
            {
                if (vals[0].Denominator == 0 && vals[1].Denominator == 0 && vals[2].Denominator == 0 && vals[3].Denominator == 0)
                    return "n/a (x4)";
            }

            var sb = new StringBuilder();
            foreach (var t in vals)
                sb.Append(t).Append(", ");
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public string? GetManualFlashStrengthDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagManualFlashStrength) is Rational[] vals))
                return "n/a";

            if (vals.Length == 3)
            {
                if (vals[0].Denominator == 0 && vals[1].Denominator == 0 && vals[2].Denominator == 0)
                    return "n/a";
            }
            else if (vals.Length == 4)
            {
                if (vals[0].Denominator == 0 && vals[1].Denominator == 0 && vals[2].Denominator == 0 && vals[3].Denominator == 0)
                    return "n/a (x4)";
            }

            var sb = new StringBuilder();
            foreach (var t in vals)
                sb.Append(t).Append(", ");
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public string? GetWhiteBalance2Description()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagWhiteBalance2, out int value))
                return null;

            return value switch
            {
                0 => "Auto",
                1 => "Auto (Keep Warm Color Off)",
                16 => "7500K (Fine Weather with Shade)",
                17 => "6000K (Cloudy)",
                18 => "5300K (Fine Weather)",
                20 => "3000K (Tungsten light)",
                21 => "3600K (Tungsten light-like)",
                22 => "Auto Setup",
                23 => "5500K (Flash)",
                33 => "6600K (Daylight fluorescent)",
                34 => "4500K (Neutral white fluorescent)",
                35 => "4000K (Cool white fluorescent)",
                36 => "White Fluorescent",
                48 => "3600K (Tungsten light-like)",
                67 => "Underwater",
                256 => "One Touch WB 1",
                257 => "One Touch WB 2",
                258 => "One Touch WB 3",
                259 => "One Touch WB 4",
                512 => "Custom WB 1",
                513 => "Custom WB 2",
                514 => "Custom WB 3",
                515 => "Custom WB 4",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetWhiteBalanceTemperatureDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagWhiteBalanceTemperature, out int value))
                return null;
            if (value == 0)
                return "Auto";
            return value.ToString();
        }

        public string? GetCustomSaturationDescription()
        {
            // TODO: if model is /^E-1\b/  then
            // $a-=$b; $c-=$b;
            // return "CS$a (min CS0, max CS$c)"
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagCustomSaturation);
        }

        public string? GetModifiedSaturationDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagModifiedSaturation,
                "Off", "CM1 (Red Enhance)", "CM2 (Green Enhance)", "CM3 (Blue Enhance)", "CM4 (Skin Tones)");
        }

        public string? GetContrastSettingDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagContrastSetting);
        }

        public string? GetSharpnessSettingDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagSharpnessSetting);
        }

        public string? GetColorSpaceDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagColorSpace,
                "sRGB", "Adobe RGB", "Pro Photo RGB");
        }

        public string? GetSceneModeDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagSceneMode, out int value))
                return null;

            return value switch
            {
                0 => "Standard",
                6 => "Auto",
                7 => "Sport",
                8 => "Portrait",
                9 => "Landscape+Portrait",
                10 => "Landscape",
                11 => "Night Scene",
                12 => "Self Portrait",
                13 => "Panorama",
                14 => "2 in 1",
                15 => "Movie",
                16 => "Landscape+Portrait",
                17 => "Night+Portrait",
                18 => "Indoor",
                19 => "Fireworks",
                20 => "Sunset",
                21 => "Beauty Skin",
                22 => "Macro",
                23 => "Super Macro",
                24 => "Food",
                25 => "Documents",
                26 => "Museum",
                27 => "Shoot & Select",
                28 => "Beach & Snow",
                29 => "Self Portrait+Timer",
                30 => "Candle",
                31 => "Available Light",
                32 => "Behind Glass",
                33 => "My Mode",
                34 => "Pet",
                35 => "Underwater Wide1",
                36 => "Underwater Macro",
                37 => "Shoot & Select1",
                38 => "Shoot & Select2",
                39 => "High Key",
                40 => "Digital Image Stabilization",
                41 => "Auction",
                42 => "Beach",
                43 => "Snow",
                44 => "Underwater Wide2",
                45 => "Low Key",
                46 => "Children",
                47 => "Vivid",
                48 => "Nature Macro",
                49 => "Underwater Snapshot",
                50 => "Shooting Guide",
                54 => "Face Portrait",
                57 => "Bulb",
                59 => "Smile Shot",
                60 => "Quick Shutter",
                63 => "Slow Shutter",
                64 => "Bird Watching",
                65 => "Multiple Exposure",
                66 => "e-Portrait",
                67 => "Soft Background Shot",
                142 => "Hand-held Starlight",
                154 => "HDR",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagNoiseReduction, out int value))
                return null;

            var sb = new StringBuilder();
            if (value == 0)
            {
                sb.Append("(none)");
            }
            else
            {
                var valshort = (ushort)value;

                if ((valshort & 1) > 0) sb.Append("Noise Reduction, ");
                if (((valshort >> 1) & 1) > 0) sb.Append("Noise Filter, ");
                if (((valshort >> 2) & 1) > 0) sb.Append("Noise Filter (ISO Boost), ");
                if (((valshort >> 3) & 1) > 0) sb.Append("Auto, ");

                sb.Remove(sb.Length - 2, 2);
            }

            return sb.ToString();
        }

        public string? GetDistortionCorrectionDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagDistortionCorrection,
                "Off", "On");
        }

        public string? GetShadingCompensationDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagShadingCompensation,
                "Off", "On");
        }

        /// <summary>
        /// 3 or 4 values
        /// </summary>
        /// <returns></returns>
        public string? GetGradationDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagGradation) is short[] values) || values.Length < 3)
                return null;

            var join = $"{values[0]} {values[1]} {values[2]}";
            var ret = join switch
            {
                "0 0 0" => "n/a",
                "-1 -1 1" => "Low Key",
                "0 -1 1" => "Normal",
                "1 -1 1" => "High Key",
                _ => "Unknown (" + join + ")",
            };
            if (values.Length > 3)
            {
                if (values[3] == 0)
                    ret += "; User-Selected";
                else if (values[3] == 1)
                    ret += "; Auto-Override";
            }

            return ret;
        }

        /// <summary>
        /// 1 or 2 values
        /// </summary>
        /// <returns></returns>
        public string? GetPictureModeDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPictureMode) as ushort[];
            if (values == null)
            {
                // check if it's only one value long also
                if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagPictureMode, out int value))
                    return null;

                values = new ushort[1];
                values[0] = (ushort)value;
            }

            if (values.Length == 0)
                return null;

            var sb = new StringBuilder();
            switch (values[0])
            {
                case 1:
                    sb.Append("Vivid");
                    break;
                case 2:
                    sb.Append("Natural");
                    break;
                case 3:
                    sb.Append("Muted");
                    break;
                case 4:
                    sb.Append("Portrait");
                    break;
                case 5:
                    sb.Append("i-Enhance");
                    break;
                case 256:
                    sb.Append("Monotone");
                    break;
                case 512:
                    sb.Append("Sepia");
                    break;
                default:
                    sb.Append("Unknown (").Append(values[0]).Append(')');
                    break;
            }

            if (values.Length > 1)
                sb.Append("; ").Append(values[1]);

            return sb.ToString();
        }

        public string? GetPictureModeSaturationDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeSaturation);
        }

        public string? GetPictureModeContrastDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeContrast);
        }

        public string? GetPictureModeSharpnessDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeSharpness);
        }

        public string? GetPictureModeBWFilterDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeBWFilter,
                "n/a", "Neutral", "Yellow", "Orange", "Red", "Green");
        }

        public string? GetPictureModeToneDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeTone,
                "n/a", "Neutral", "Sepia", "Blue", "Purple", "Green");
        }

        public string? GetNoiseFilterDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagNoiseFilter) is short[] values))
                return null;

            return $"{values[0]} {values[1]} {values[2]}" switch
            {
                "0 0 0" => "n/a",
                "-2 -2 1" => "Off",
                "-1 -2 1" => "Low",
                "0 -2 1" => "Standard",
                "1 -2 1" => "High",
                _ => $"Unknown ({values[0]} {values[1]} {values[2]})",
            };
        }

        public string? GetArtFilterDescription() => GetFilterDescription(OlympusCameraSettingsMakernoteDirectory.TagArtFilter);

        public string? GetMagicFilterDescription() => GetFilterDescription(OlympusCameraSettingsMakernoteDirectory.TagMagicFilter);

        public string? GetPictureModeEffectDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPictureModeEffect) is short[] values))
                return null;

            return $"{values[0]} {values[1]} {values[2]}" switch
            {
                "0 0 0" => "n/a",
                "-1 -1 1" => "Low",
                "0 -1 1" => "Standard",
                "1 -1 1" => "High",
                _ => "Unknown (" + $"{values[0]} {values[1]} {values[2]}" + ")",
            };
        }

        public string? GetToneLevelDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagToneLevel) is short[] values))
                return null;

            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0 || i == 4 || i == 8 || i == 12 || i == 16 || i == 20 || i == 24)
                    sb.Append(_toneLevelType[values[i]] + "; ");
                else
                    sb.Append(values[i] + "; ");
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public string? GetArtFilterEffectDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagArtFilterEffect) is ushort[] values))
                return null;

            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0)
                    sb.Append((_filters.ContainsKey(values[i]) ? _filters[values[i]] : "[unknown]") + "; ");
                else if (i == 3)
                    sb.Append("Partial Color " + values[i] + "; ");
                else if (i == 4)
                {
                    switch (values[i])
                    {
                        case 0x0000:
                            sb.Append("No Effect");
                            break;
                        case 0x8010:
                            sb.Append("Star Light");
                            break;
                        case 0x8020:
                            sb.Append("Pin Hole");
                            break;
                        case 0x8030:
                            sb.Append("Frame");
                            break;
                        case 0x8040:
                            sb.Append("Soft Focus");
                            break;
                        case 0x8050:
                            sb.Append("White Edge");
                            break;
                        case 0x8060:
                            sb.Append("B&W");
                            break;
                        default:
                            sb.Append("Unknown (").Append(values[i]).Append(')');
                            break;
                    }
                    sb.Append("; ");
                }
                else if (i == 6)
                {
                    switch (values[i])
                    {
                        case 0:
                            sb.Append("No Color Filter");
                            break;
                        case 1:
                            sb.Append("Yellow Color Filter");
                            break;
                        case 2:
                            sb.Append("Orange Color Filter");
                            break;
                        case 3:
                            sb.Append("Red Color Filter");
                            break;
                        case 4:
                            sb.Append("Green Color Filter");
                            break;
                        default:
                            sb.Append("Unknown (").Append(values[i]).Append(')');
                            break;
                    }
                    sb.Append("; ");
                }
                else
                {
                    sb.Append(values[i] + "; ");
                }
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public string? GetColorCreatorEffectDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagColorCreatorEffect) is short[] values))
                return null;

            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0)
                    sb.Append("Color " + values[i] + "; ");
                else if (i == 3)
                    sb.Append("Strength " + values[i] + "; ");
                else
                    sb.Append(values[i] + "; ");
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        /// <summary>
        /// 2 or 3 numbers: 1. Mode, 2. Shot number, 3. Mode bits
        /// </summary>
        /// <returns></returns>
        public string? GetDriveModeDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagDriveMode) is ushort[] values))
                return null;

            if (values.Length == 0 || values.Length > 0 && values[0] == 0)
                return "Single Shot";

            var a = new StringBuilder();

            if (values[0] == 5 && values.Length >= 3)
            {
                var c = values[2];
                if (( c       & 1) > 0) a.Append("AE");
                if (((c >> 1) & 1) > 0) a.Append("WB");
                if (((c >> 2) & 1) > 0) a.Append("FL");
                if (((c >> 3) & 1) > 0) a.Append("MF");
                if (((c >> 6) & 1) > 0) a.Append("Focus");

                a.Append(" Bracketing");
            }
            else
            {
                switch (values[0])
                {
                    case 1:
                        a.Append("Continuous Shooting");
                        break;
                    case 2:
                        a.Append("Exposure Bracketing");
                        break;
                    case 3:
                        a.Append("White Balance Bracketing");
                        break;
                    case 4:
                        a.Append("Exposure+WB Bracketing");
                        break;
                    default:
                        a.Append("Unknown (").Append(values[0]).Append(')');
                        break;
                }
            }

            a.Append(", Shot ").Append(values[1]);

            return a.ToString();
        }

        /// <summary>
        /// 2 numbers: 1. Mode, 2. Shot number
        /// </summary>
        /// <returns></returns>
        public string? GetPanoramaModeDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPanoramaMode) is ushort[] values))
                return null;

            if (values.Length == 0 || values[0] == 0)
                return "Off";
            var a = (values[0]) switch
            {
                1 => "Left to Right",
                2 => "Right to Left",
                3 => "Bottom to Top",
                4 => "Top to Bottom",
                _ => "Unknown (" + values[0] + ")",
            };
            return $"{a}, Shot {values[1]}";
        }

        public string? GetImageQuality2Description()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagImageQuality2, 1,
                "SQ", "HQ", "SHQ", "RAW", "SQ (5)");
        }

        public string? GetImageStabilizationDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagImageStabilization,
                "Off", "On, Mode 1", "On, Mode 2", "On, Mode 3", "On, Mode 4");
        }

        public string? GetStackedImageDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagStackedImage) is short[] values))
                return null;

            int v1 = values[0];
            int v2 = values[1];

            if (v1 == 0 && v2 == 0)
                return "No";
            if (v1 == 9 && v2 == 8)
                return "Focus-stacked (8 images)";
            return $"Unknown ({v1} {v2})";
        }

        /// <remarks>
        /// TODO: need better image examples to test this function
        /// </remarks>
        /// <returns></returns>
        public string? GetManometerPressureDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagManometerPressure, out int value))
                return null;

            return $"{value/10.0} kPa";
        }

        /// <remarks>
        /// TODO: need better image examples to test this function
        /// </remarks>
        /// <returns></returns>
        public string? GetManometerReadingDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagManometerReading) is int[] values) || values.Length < 2)
                return null;

            return $"{values[0]/10.0} m, {values[1]/10.0} ft";
        }

        public string? GetExtendedWBDetectDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagExtendedWBDetect,
                "Off", "On");
        }

        /// <summary>
        /// converted to degrees of clockwise camera rotation
        /// </summary>
        /// <remarks>
        /// TODO: need better image examples to test this function
        /// </remarks>
        /// <returns></returns>
        public string? GetRollAngleDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagRollAngle) is short[] values))
                return null;

            var ret = values[0] != 0
                ? (-values[0] / 10).ToString()
                : "n/a";

            return $"{ret} {values[1]}";
        }

        /// <summary>
        /// converted to degrees of upward camera tilt
        /// </summary>
        /// <remarks>
        /// TODO: need better image examples to test this function
        /// </remarks>
        /// <returns></returns>
        public string? GetPitchAngleDescription()
        {
            if (!(Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPitchAngle) is short[] values))
                return null;

            // (second value is 0 if level gauge is off)
            var ret = values[0] != 0
                ? (values[0] / 10).ToString()
                : "n/a";

            return $"{ret} {values[1]}";
        }

        public string? GetDateTimeUtcDescription()
        {
            return Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagDateTimeUtc)?.ToString();
        }

        private string? GetValueMinMaxDescription(int tagId)
        {
            if (!(Directory.GetObject(tagId) is short[] values) || values.Length < 3)
                return null;

            return $"{values[0]} (min {values[1]}, max {values[2]})";
        }

        private string? GetFilterDescription(int tagId)
        {
            if (!(Directory.GetObject(tagId) is short[] values) || values.Length == 0)
                return null;

            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0)
                    sb.Append(_filters.ContainsKey(values[i]) ? _filters[values[i]] : "[unknown]");
                else
                    sb.Append(values[i]);
                sb.Append("; ");
            }

            return sb.ToString(0, sb.Length - 2);
        }

        private static readonly Dictionary<int, string> _toneLevelType = new Dictionary<int, string>
        {
            { 0, "0" },
            { -31999, "Highlights " },
            { -31998, "Shadows " },
            { -31997, "Midtones " }
        };

        // ArtFilter, ArtFilterEffect and MagicFilter values
        private static readonly Dictionary<int, string> _filters = new Dictionary<int, string>
        {
            { 0, "Off" },
            { 1, "Soft Focus" },
            { 2, "Pop Art" },
            { 3, "Pale & Light Color" },
            { 4, "Light Tone" },
            { 5, "Pin Hole" },
            { 6, "Grainy Film" },
            { 9, "Diorama" },
            { 10, "Cross Process" },
            { 12, "Fish Eye" },
            { 13, "Drawing" },
            { 14, "Gentle Sepia" },
            { 15, "Pale & Light Color II" },
            { 16, "Pop Art II" },
            { 17, "Pin Hole II" },
            { 18, "Pin Hole III" },
            { 19, "Grainy Film II" },
            { 20, "Dramatic Tone" },
            { 21, "Punk" },
            { 22, "Soft Focus 2" },
            { 23, "Sparkle" },
            { 24, "Watercolor" },
            { 25, "Key Line" },
            { 26, "Key Line II" },
            { 27, "Miniature" },
            { 28, "Reflection" },
            { 29, "Fragmented" },
            { 31, "Cross Process II" },
            { 32, "Dramatic Tone II" },
            { 33, "Watercolor I" },
            { 34, "Watercolor II" },
            { 35, "Diorama II" },
            { 36, "Vintage" },
            { 37, "Vintage II" },
            { 38, "Vintage III" },
            { 39, "Partial Color" },
            { 40, "Partial Color II" },
            { 41, "Partial Color III" }
        };
    }
}
