#region License
//
// Copyright 2002-2017 Drew Noakes
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
using System.Text;
using JetBrains.Annotations;
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
        public OlympusCameraSettingsMakernoteDescriptor([NotNull] OlympusCameraSettingsMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusCameraSettingsMakernoteDirectory.TagCameraSettingsVersion:
                    return GetCameraSettingsVersionDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPreviewImageValid:
                    return GetPreviewImageValidDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagExposureMode:
                    return GetExposureModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagAeLock:
                    return GetAeLockDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagMeteringMode:
                    return GetMeteringModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagExposureShift:
                    return GetExposureShiftDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagNdFilter:
                    return GetNdFilterDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagMacroMode:
                    return GetMacroModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagFocusProcess:
                    return GetFocusProcessDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagAfSearch:
                    return GetAfSearchDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagAfAreas:
                    return GetAfAreasDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagAfPointSelected:
                    return GetAfPointSelectedDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagAfFineTune:
                    return GetAfFineTuneDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagFlashRemoteControl:
                    return GetFlashRemoteControlDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagFlashControlMode:
                    return GetFlashControlModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagFlashIntensity:
                    return GetFlashIntensityDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagManualFlashStrength:
                    return GetManualFlashStrengthDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagWhiteBalance2:
                    return GetWhiteBalance2Description();
                case OlympusCameraSettingsMakernoteDirectory.TagWhiteBalanceTemperature:
                    return GetWhiteBalanceTemperatureDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagCustomSaturation:
                    return GetCustomSaturationDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagModifiedSaturation:
                    return GetModifiedSaturationDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagContrastSetting:
                    return GetContrastSettingDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagSharpnessSetting:
                    return GetSharpnessSettingDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagColorSpace:
                    return GetColorSpaceDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagSceneMode:
                    return GetSceneModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagNoiseReduction:
                    return GetNoiseReductionDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagDistortionCorrection:
                    return GetDistortionCorrectionDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagShadingCompensation:
                    return GetShadingCompensationDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagGradation:
                    return GetGradationDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureMode:
                    return GetPictureModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureModeSaturation:
                    return GetPictureModeSaturationDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureModeContrast:
                    return GetPictureModeContrastDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureModeSharpness:
                    return GetPictureModeSharpnessDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureModeBWFilter:
                    return GetPictureModeBWFilterDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureModeTone:
                    return GetPictureModeToneDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagNoiseFilter:
                    return GetNoiseFilterDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagArtFilter:
                    return GetArtFilterDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagMagicFilter:
                    return GetMagicFilterDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPictureModeEffect:
                    return GetPictureModeEffectDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagToneLevel:
                    return GetToneLevelDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagArtFilterEffect:
                    return GetArtFilterEffectDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagColorCreatorEffect:
                    return GetColorCreatorEffectDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagDriveMode:
                    return GetDriveModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPanoramaMode:
                    return GetPanoramaModeDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagImageQuality2:
                    return GetImageQuality2Description();
                case OlympusCameraSettingsMakernoteDirectory.TagImageStabilization:
                    return GetImageStabilizationDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagStackedImage:
                    return GetStackedImageDescription();

                case OlympusCameraSettingsMakernoteDirectory.TagManometerPressure:
                    return GetManometerPressureDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagManometerReading:
                    return GetManometerReadingDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagExtendedWBDetect:
                    return GetExtendedWBDetectDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagRollAngle:
                    return GetRollAngleDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagPitchAngle:
                    return GetPitchAngleDescription();
                case OlympusCameraSettingsMakernoteDirectory.TagDateTimeUtc:
                    return GetDateTimeUtcDescription();

                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetCameraSettingsVersionDescription()
        {
            return GetVersionBytesDescription(OlympusCameraSettingsMakernoteDirectory.TagCameraSettingsVersion, 4);
        }

        [CanBeNull]
        public string GetPreviewImageValidDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagPreviewImageValid,
                "No", "Yes");
        }

        [CanBeNull]
        public string GetExposureModeDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagExposureMode, 1,
                "Manual", "Program", "Aperture-priority AE", "Shutter speed priority", "Program-shift");
        }

        [CanBeNull]
        public string GetAeLockDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagAeLock,
                "Off", "On");
        }

        [CanBeNull]
        public string GetMeteringModeDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagMeteringMode, out int value))
                return null;

            switch (value)
            {
                case 2:
                    return "Center-weighted average";
                case 3:
                    return "Spot";
                case 5:
                    return "ESP";
                case 261:
                    return "Pattern+AF";
                case 515:
                    return "Spot+Highlight control";
                case 1027:
                    return "Spot+Shadow control";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetExposureShiftDescription()
        {
            return GetRationalOrDoubleString(OlympusCameraSettingsMakernoteDirectory.TagExposureShift);
        }

        [CanBeNull]
        public string GetNdFilterDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagNdFilter,
                "Off", "On");
        }

        [CanBeNull]
        public string GetMacroModeDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagMacroMode,
                "Off", "On", "Super Macro");
        }

        [CanBeNull]
        public string GetFocusModeDescription()
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

        [CanBeNull]
        public string GetFocusProcessDescription()
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

        [CanBeNull]
        public string GetAfSearchDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagAfSearch,
                "Not Ready", "Ready");
        }

        /// <summary>
        /// coordinates range from 0 to 255
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public string GetAfAreasDescription()
        {
            var points = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagAfAreas) as uint[];
            if (points == null)
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
        [CanBeNull]
        public string GetAfPointSelectedDescription()
        {
            var vals = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagAfPointSelected) as Rational[];
            if (vals == null)
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

            if(p1 + p2 + p3 + p4 == 0)
                return "n/a";

            return $"({p1}%,{p2}%) ({p3}%,{p4}%)";

        }

        [CanBeNull]
        public string GetAfFineTuneDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagAfFineTune,
                "Off", "On");
        }

        [CanBeNull]
        public string GetFlashModeDescription()
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

        [CanBeNull]
        public string GetFlashRemoteControlDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagFlashRemoteControl, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 0x01:
                    return "Channel 1, Low";
                case 0x02:
                    return "Channel 2, Low";
                case 0x03:
                    return "Channel 3, Low";
                case 0x04:
                    return "Channel 4, Low";
                case 0x09:
                    return "Channel 1, Mid";
                case 0x0a:
                    return "Channel 2, Mid";
                case 0x0b:
                    return "Channel 3, Mid";
                case 0x0c:
                    return "Channel 4, Mid";
                case 0x11:
                    return "Channel 1, High";
                case 0x12:
                    return "Channel 2, High";
                case 0x13:
                    return "Channel 3, High";
                case 0x14:
                    return "Channel 4, High";

                default:
                    return "Unknown (" + value + ")";
            }
        }

        /// <summary>
        /// 3 or 4 values
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public string GetFlashControlModeDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagFlashControlMode) as ushort[];
            if (values == null)
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
        [CanBeNull]
        public string GetFlashIntensityDescription()
        {
            var vals = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagFlashIntensity) as Rational[];
            if (vals == null)
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

        [CanBeNull]
        public string GetManualFlashStrengthDescription()
        {
            var vals = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagManualFlashStrength) as Rational[];
            if (vals == null)
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

        [CanBeNull]
        public string GetWhiteBalance2Description()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagWhiteBalance2, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Auto";
                case 1:
                    return "Auto (Keep Warm Color Off)";
                case 16:
                    return "7500K (Fine Weather with Shade)";
                case 17:
                    return "6000K (Cloudy)";
                case 18:
                    return "5300K (Fine Weather)";
                case 20:
                    return "3000K (Tungsten light)";
                case 21:
                    return "3600K (Tungsten light-like)";
                case 22:
                    return "Auto Setup";
                case 23:
                    return "5500K (Flash)";
                case 33:
                    return "6600K (Daylight fluorescent)";
                case 34:
                    return "4500K (Neutral white fluorescent)";
                case 35:
                    return "4000K (Cool white fluorescent)";
                case 36:
                    return "White Fluorescent";
                case 48:
                    return "3600K (Tungsten light-like)";
                case 67:
                    return "Underwater";
                case 256:
                    return "One Touch WB 1";
                case 257:
                    return "One Touch WB 2";
                case 258:
                    return "One Touch WB 3";
                case 259:
                    return "One Touch WB 4";
                case 512:
                    return "Custom WB 1";
                case 513:
                    return "Custom WB 2";
                case 514:
                    return "Custom WB 3";
                case 515:
                    return "Custom WB 4";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetWhiteBalanceTemperatureDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagWhiteBalanceTemperature, out int value))
                return null;
            if (value == 0)
                return "Auto";
            return value.ToString();
        }

        [CanBeNull]
        public string GetCustomSaturationDescription()
        {
            // TODO: if model is /^E-1\b/  then
            // $a-=$b; $c-=$b;
            // return "CS$a (min CS0, max CS$c)"
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagCustomSaturation);
        }

        [CanBeNull]
        public string GetModifiedSaturationDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagModifiedSaturation,
                "Off", "CM1 (Red Enhance)", "CM2 (Green Enhance)", "CM3 (Blue Enhance)", "CM4 (Skin Tones)");
        }

        [CanBeNull]
        public string GetContrastSettingDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagContrastSetting);
        }

        [CanBeNull]
        public string GetSharpnessSettingDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagSharpnessSetting);
        }

        [CanBeNull]
        public string GetColorSpaceDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagColorSpace,
                "sRGB", "Adobe RGB", "Pro Photo RGB");
        }

        [CanBeNull]
        public string GetSceneModeDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagSceneMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Standard";
                case 6:
                    return "Auto";
                case 7:
                    return "Sport";
                case 8:
                    return "Portrait";
                case 9:
                    return "Landscape+Portrait";
                case 10:
                    return "Landscape";
                case 11:
                    return "Night Scene";
                case 12:
                    return "Self Portrait";
                case 13:
                    return "Panorama";
                case 14:
                    return "2 in 1";
                case 15:
                    return "Movie";
                case 16:
                    return "Landscape+Portrait";
                case 17:
                    return "Night+Portrait";
                case 18:
                    return "Indoor";
                case 19:
                    return "Fireworks";
                case 20:
                    return "Sunset";
                case 21:
                    return "Beauty Skin";
                case 22:
                    return "Macro";
                case 23:
                    return "Super Macro";
                case 24:
                    return "Food";
                case 25:
                    return "Documents";
                case 26:
                    return "Museum";
                case 27:
                    return "Shoot & Select";
                case 28:
                    return "Beach & Snow";
                case 29:
                    return "Self Portrait+Timer";
                case 30:
                    return "Candle";
                case 31:
                    return "Available Light";
                case 32:
                    return "Behind Glass";
                case 33:
                    return "My Mode";
                case 34:
                    return "Pet";
                case 35:
                    return "Underwater Wide1";
                case 36:
                    return "Underwater Macro";
                case 37:
                    return "Shoot & Select1";
                case 38:
                    return "Shoot & Select2";
                case 39:
                    return "High Key";
                case 40:
                    return "Digital Image Stabilization";
                case 41:
                    return "Auction";
                case 42:
                    return "Beach";
                case 43:
                    return "Snow";
                case 44:
                    return "Underwater Wide2";
                case 45:
                    return "Low Key";
                case 46:
                    return "Children";
                case 47:
                    return "Vivid";
                case 48:
                    return "Nature Macro";
                case 49:
                    return "Underwater Snapshot";
                case 50:
                    return "Shooting Guide";
                case 54:
                    return "Face Portrait";
                case 57:
                    return "Bulb";
                case 59:
                    return "Smile Shot";
                case 60:
                    return "Quick Shutter";
                case 63:
                    return "Slow Shutter";
                case 64:
                    return "Bird Watching";
                case 65:
                    return "Multiple Exposure";
                case 66:
                    return "e-Portrait";
                case 67:
                    return "Soft Background Shot";
                case 142:
                    return "Hand-held Starlight";
                case 154:
                    return "HDR";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetNoiseReductionDescription()
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

        [CanBeNull]
        public string GetDistortionCorrectionDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagDistortionCorrection,
                "Off", "On");
        }

        [CanBeNull]
        public string GetShadingCompensationDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagShadingCompensation,
                "Off", "On");
        }

        /// <summary>
        /// 3 or 4 values
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public string GetGradationDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagGradation) as short[];
            if (values == null || values.Length < 3)
                return null;

            var join = $"{values[0]} {values[1]} {values[2]}";

            string ret;
            switch (join)
            {
                case "0 0 0":
                    ret = "n/a";
                    break;
                case "-1 -1 1":
                    ret = "Low Key";
                    break;
                case "0 -1 1":
                    ret = "Normal";
                    break;
                case "1 -1 1":
                    ret = "High Key";
                    break;
                default:
                    ret = "Unknown (" + join + ")";
                    break;
            }

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
        [CanBeNull]
        public string GetPictureModeDescription()
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

        [CanBeNull]
        public string GetPictureModeSaturationDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeSaturation);
        }

        [CanBeNull]
        public string GetPictureModeContrastDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeContrast);
        }

        [CanBeNull]
        public string GetPictureModeSharpnessDescription()
        {
            return GetValueMinMaxDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeSharpness);
        }

        [CanBeNull]
        public string GetPictureModeBWFilterDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeBWFilter,
                "n/a", "Neutral", "Yellow", "Orange", "Red", "Green");
        }

        [CanBeNull]
        public string GetPictureModeToneDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagPictureModeTone,
                "n/a", "Neutral", "Sepia", "Blue", "Purple", "Green");
        }

        [CanBeNull]
        public string GetNoiseFilterDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagNoiseFilter) as short[];
            if (values == null)
                return null;

            switch ($"{values[0]} {values[1]} {values[2]}")
            {
                case "0 0 0":
                    return "n/a";
                case "-2 -2 1":
                    return "Off";
                case "-1 -2 1":
                    return "Low";
                case "0 -2 1":
                    return "Standard";
                case "1 -2 1":
                    return "High";
                default:
                    return $"Unknown ({values[0]} {values[1]} {values[2]})";
            }
        }

        [CanBeNull]
        public string GetArtFilterDescription() => GetFilterDescription(OlympusCameraSettingsMakernoteDirectory.TagArtFilter);

        [CanBeNull]
        public string GetMagicFilterDescription() => GetFilterDescription(OlympusCameraSettingsMakernoteDirectory.TagMagicFilter);

        [CanBeNull]
        public string GetPictureModeEffectDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPictureModeEffect) as short[];
            if (values == null)
                return null;

            switch ($"{values[0]} {values[1]} {values[2]}")
            {
                case "0 0 0":
                    return "n/a";
                case "-1 -1 1":
                    return "Low";
                case "0 -1 1":
                    return "Standard";
                case "1 -1 1":
                    return "High";
                default:
                    return "Unknown (" + $"{values[0]} {values[1]} {values[2]}" + ")";
            }
        }

        [CanBeNull]
        public string GetToneLevelDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagToneLevel) as short[];
            if (values == null)
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

        [CanBeNull]
        public string GetArtFilterEffectDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagArtFilterEffect) as ushort[];
            if (values == null)
                return null;

            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0)
                    sb.Append((_filters.ContainsKey(values[i]) ? _filters[values[i]] : "[unknown]") + "; ");
                else if(i == 3)
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

        [CanBeNull]
        public string GetColorCreatorEffectDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagColorCreatorEffect) as short[];
            if (values == null)
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
        [CanBeNull]
        public string GetDriveModeDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagDriveMode) as ushort[];
            if (values == null)
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
        [CanBeNull]
        public string GetPanoramaModeDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPanoramaMode) as ushort[];
            if (values == null)
                return null;

            if (values.Length == 0 || values[0] == 0)
                return "Off";

            string a;
            switch (values[0])
            {
                case 1:
                    a = "Left to Right";
                    break;
                case 2:
                    a = "Right to Left";
                    break;
                case 3:
                    a = "Bottom to Top";
                    break;
                case 4:
                    a = "Top to Bottom";
                    break;
                default:
                    a = "Unknown (" + values[0] + ")";
                    break;
            }

            return $"{a}, Shot {values[1]}";
        }

        [CanBeNull]
        public string GetImageQuality2Description()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagImageQuality2, 1,
                "SQ", "HQ", "SHQ", "RAW", "SQ (5)");
        }

        [CanBeNull]
        public string GetImageStabilizationDescription()
        {
            return GetIndexedDescription(OlympusCameraSettingsMakernoteDirectory.TagImageStabilization,
                "Off", "On, Mode 1", "On, Mode 2", "On, Mode 3", "On, Mode 4");
        }

        [CanBeNull]
        public string GetStackedImageDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagStackedImage) as short[];
            if (values == null)
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
        [CanBeNull]
        public string GetManometerPressureDescription()
        {
            if (!Directory.TryGetInt32(OlympusCameraSettingsMakernoteDirectory.TagManometerPressure, out int value))
                return null;

            return $"{value/10.0} kPa";
        }

        /// <remarks>
        /// TODO: need better image examples to test this function
        /// </remarks>
        /// <returns></returns>
        [CanBeNull]
        public string GetManometerReadingDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagManometerReading) as int[];
            if (values == null || values.Length < 2)
                return null;

            return $"{values[0]/10.0} m, {values[1]/10.0} ft";
        }

        [CanBeNull]
        public string GetExtendedWBDetectDescription()
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
        [CanBeNull]
        public string GetRollAngleDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagRollAngle) as short[];
            if (values == null)
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
        [CanBeNull]
        public string GetPitchAngleDescription()
        {
            var values = Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagPitchAngle) as short[];
            if (values == null)
                return null;

            // (second value is 0 if level gauge is off)
            var ret = values[0] != 0
                ? (values[0] / 10).ToString()
                : "n/a";

            return $"{ret} {values[1]}";
        }

        [CanBeNull]
        public string GetDateTimeUtcDescription()
        {
            return Directory.GetObject(OlympusCameraSettingsMakernoteDirectory.TagDateTimeUtc)?.ToString();
        }

        [CanBeNull]
        private string GetValueMinMaxDescription(int tagId)
        {
            var values = Directory.GetObject(tagId) as short[];
            if (values == null || values.Length < 3)
                return null;

            return $"{values[0]} (min {values[1]}, max {values[2]})";
        }

        [CanBeNull]
        private string GetFilterDescription(int tagId)
        {
            var values = Directory.GetObject(tagId) as short[];
            if (values == null || values.Length == 0)
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
