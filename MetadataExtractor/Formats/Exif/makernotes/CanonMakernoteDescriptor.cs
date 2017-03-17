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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="CanonMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class CanonMakernoteDescriptor : TagDescriptor<CanonMakernoteDirectory>
    {
        public CanonMakernoteDescriptor([NotNull] CanonMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case CanonMakernoteDirectory.TagCanonSerialNumber:
                    return GetSerialNumberDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFlashActivity:
                    return GetFlashActivityDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFocusType:
                    return GetFocusTypeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagLensType:
                    return GetLensTypeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagDigitalZoom:
                    return GetDigitalZoomDescription();
                case CanonMakernoteDirectory.CameraSettings.TagRecordMode:
                    return GetRecordModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagQuality:
                    return GetQualityDescription();
                case CanonMakernoteDirectory.CameraSettings.TagMacroMode:
                    return GetMacroModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagSelfTimerDelay:
                    return GetSelfTimerDelayDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFlashMode:
                    return GetFlashModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagContinuousDriveMode:
                    return GetContinuousDriveModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFocusMode1:
                    return GetFocusMode1Description();
                case CanonMakernoteDirectory.CameraSettings.TagImageSize:
                    return GetImageSizeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagEasyShootingMode:
                    return GetEasyShootingModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagContrast:
                    return GetContrastDescription();
                case CanonMakernoteDirectory.CameraSettings.TagSaturation:
                    return GetSaturationDescription();
                case CanonMakernoteDirectory.CameraSettings.TagSharpness:
                    return GetSharpnessDescription();
                case CanonMakernoteDirectory.CameraSettings.TagIso:
                    return GetIsoDescription();
                case CanonMakernoteDirectory.CameraSettings.TagMeteringMode:
                    return GetMeteringModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagAfPointSelected:
                    return GetAfPointSelectedDescription();
                case CanonMakernoteDirectory.CameraSettings.TagExposureMode:
                    return GetExposureModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagLongFocalLength:
                    return GetLongFocalLengthDescription();
                case CanonMakernoteDirectory.CameraSettings.TagShortFocalLength:
                    return GetShortFocalLengthDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFocalUnitsPerMm:
                    return GetFocalUnitsPerMillimetreDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFlashDetails:
                    return GetFlashDetailsDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFocusMode2:
                    return GetFocusMode2Description();
                case CanonMakernoteDirectory.FocalLength.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case CanonMakernoteDirectory.FocalLength.TagAfPointUsed:
                    return GetAfPointUsedDescription();
                case CanonMakernoteDirectory.FocalLength.TagFlashBias:
                    return GetFlashBiasDescription();
                case CanonMakernoteDirectory.AfInfo.TagAfPointsInFocus:
                    return GetTagAfPointsInFocus();
                case CanonMakernoteDirectory.CameraSettings.TagMaxAperture:
                    return GetMaxApertureDescription();
                case CanonMakernoteDirectory.CameraSettings.TagMinAperture:
                    return GetMinApertureDescription();
                case CanonMakernoteDirectory.CameraSettings.TagFocusContinuous:
                    return GetFocusContinuousDescription();
                case CanonMakernoteDirectory.CameraSettings.TagAESetting:
                    return GetAESettingDescription();
                case CanonMakernoteDirectory.CameraSettings.TagDisplayAperture:
                    return GetDisplayApertureDescription();
                case CanonMakernoteDirectory.CameraSettings.TagSpotMeteringMode:
                    return GetSpotMeteringModeDescription();
                case CanonMakernoteDirectory.CameraSettings.TagPhotoEffect:
                    return GetPhotoEffectDescription();
                case CanonMakernoteDirectory.CameraSettings.TagManualFlashOutput:
                    return GetManualFlashOutputDescription();
                case CanonMakernoteDirectory.CameraSettings.TagColorTone:
                    return GetColorToneDescription();
                case CanonMakernoteDirectory.CameraSettings.TagSRawQuality:
                    return GetSRawQualityDescription();
                // It turns out that these values are dependent upon the camera model and therefore the below code was
                // incorrect for some Canon models.  This needs to be revisited.
//              case TAG_CANON_CUSTOM_FUNCTION_LONG_EXPOSURE_NOISE_REDUCTION:
//                  return getLongExposureNoiseReductionDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_SHUTTER_AUTO_EXPOSURE_LOCK_BUTTONS:
//                  return getShutterAutoExposureLockButtonDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_MIRROR_LOCKUP:
//                  return getMirrorLockupDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_TV_AV_AND_EXPOSURE_LEVEL:
//                  return getTvAndAvExposureLevelDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_AF_ASSIST_LIGHT:
//                  return getAutoFocusAssistLightDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_SHUTTER_SPEED_IN_AV_MODE:
//                  return getShutterSpeedInAvModeDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_BRACKETING:
//                  return getAutoExposureBracketingSequenceAndAutoCancellationDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_SHUTTER_CURTAIN_SYNC:
//                  return getShutterCurtainSyncDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_AF_STOP:
//                  return getLensAutoFocusStopButtonDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_FILL_FLASH_REDUCTION:
//                  return getFillFlashReductionDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_MENU_BUTTON_RETURN:
//                  return getMenuButtonReturnPositionDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_SET_BUTTON_FUNCTION:
//                  return getSetButtonFunctionWhenShootingDescription();
//              case TAG_CANON_CUSTOM_FUNCTION_SENSOR_CLEANING:
//                  return getSensorCleaningDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetSerialNumberDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/canon_mn.html
            return Directory.TryGetInt32(CanonMakernoteDirectory.TagCanonSerialNumber, out int value)
                ? $"{(value >> 8) & 0xFF:X4}{value & 0xFF:D5}"
                : null;
        }

/*
        @Nullable
        public String getLongExposureNoiseReductionDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_LONG_EXPOSURE_NOISE_REDUCTION);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Off";
                case 1:     return "On";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getShutterAutoExposureLockButtonDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_SHUTTER_AUTO_EXPOSURE_LOCK_BUTTONS);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "AF/AE lock";
                case 1:     return "AE lock/AF";
                case 2:     return "AF/AF lock";
                case 3:     return "AE+release/AE+AF";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getMirrorLockupDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_MIRROR_LOCKUP);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Disabled";
                case 1:     return "Enabled";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getTvAndAvExposureLevelDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_TV_AV_AND_EXPOSURE_LEVEL);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "1/2 stop";
                case 1:     return "1/3 stop";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getAutoFocusAssistLightDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_AF_ASSIST_LIGHT);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "On (Auto)";
                case 1:     return "Off";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getShutterSpeedInAvModeDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_SHUTTER_SPEED_IN_AV_MODE);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Automatic";
                case 1:     return "1/200 (fixed)";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getAutoExposureBracketingSequenceAndAutoCancellationDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_BRACKETING);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "0,-,+ / Enabled";
                case 1:     return "0,-,+ / Disabled";
                case 2:     return "-,0,+ / Enabled";
                case 3:     return "-,0,+ / Disabled";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getShutterCurtainSyncDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_SHUTTER_CURTAIN_SYNC);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "1st Curtain Sync";
                case 1:     return "2nd Curtain Sync";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getLensAutoFocusStopButtonDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_AF_STOP);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "AF stop";
                case 1:     return "Operate AF";
                case 2:     return "Lock AE and start timer";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getFillFlashReductionDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_FILL_FLASH_REDUCTION);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Enabled";
                case 1:     return "Disabled";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getMenuButtonReturnPositionDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_MENU_BUTTON_RETURN);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Top";
                case 1:     return "Previous (volatile)";
                case 2:     return "Previous";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getSetButtonFunctionWhenShootingDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_SET_BUTTON_FUNCTION);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Not Assigned";
                case 1:     return "Change Quality";
                case 2:     return "Change ISO Speed";
                case 3:     return "Select Parameters";
                default:    return "Unknown (" + value + ")";
            }
        }

        @Nullable
        public String getSensorCleaningDescription()
        {
            Integer value = _directory.getInteger(TAG_CANON_CUSTOM_FUNCTION_SENSOR_CLEANING);
            if (value==null)
                return null;
            switch (value) {
                case 0:     return "Disabled";
                case 1:     return "Enabled";
                default:    return "Unknown (" + value + ")";
            }
        }
*/
        [CanBeNull]
        public string GetFlashBiasDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.FocalLength.TagFlashBias, out int value))
                return null;

            var isNegative = false;

            if (value > 0xF000)
            {
                isNegative = true;
                value = 0xFFFF - value;
                value++;
            }

            // this tag is interesting in that the values returned are:
            //  0, 0.375, 0.5, 0.626, 1
            // not
            //  0, 0.33,  0.5, 0.66,  1
            return (isNegative ? "-" : string.Empty) + (value / 32f).ToString("0.0###########") + " EV";
        }

        [CanBeNull]
        public string GetAfPointUsedDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.FocalLength.TagAfPointUsed, out int value))
                return null;

            switch (value & 0x7)
            {
                case 0: return "Right";
                case 1: return "Centre";
                case 2: return "Left";
            }

            return "Unknown (" + value + ")";
        }

        [CanBeNull]
        public string GetTagAfPointsInFocus()
        {
            if (!Directory.TryGetInt16(CanonMakernoteDirectory.AfInfo.TagAfPointsInFocus, out short value))
                return null;

            var sb = new StringBuilder();

            for (var i = 0; i < 16; i++)
            {
                if ((value & 1 << i) != 0)
                {
                    if (sb.Length != 0)
                        sb.Append(',');
                    sb.Append(i);
                }
            }

            return sb.Length == 0 ? "None" : sb.ToString();
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.FocalLength.TagWhiteBalance,
                "Auto", "Sunny", "Cloudy", "Tungsten", "Florescent", "Flash", "Custom");
        }

        [CanBeNull]
        public string GetFocusMode2Description()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagFocusMode2,
                "Single", "Continuous");
        }

        [CanBeNull]
        public string GetFlashDetailsDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagFlashDetails, out int value))
                return null;

            if (((value >> 14) & 1) > 0) return "External E-TTL";
            if (((value >> 13) & 1) > 0) return "Internal flash";
            if (((value >> 11) & 1) > 0) return "FP sync used";
            if (((value >>  4) & 1) > 0) return "FP sync enabled";

            return "Unknown (" + value + ")";
        }

        [CanBeNull]
        public string GetFocalUnitsPerMillimetreDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagFocalUnitsPerMm, out int value))
                return null;

            return value != 0 ? value.ToString() : string.Empty;
        }

        [CanBeNull]
        public string GetShortFocalLengthDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagShortFocalLength, out int value))
                return null;

            var units = GetFocalUnitsPerMillimetreDescription();
            return value + " " + units;
        }

        [CanBeNull]
        public string GetLongFocalLengthDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagLongFocalLength, out int value))
                return null;

            return value + " " + GetFocalUnitsPerMillimetreDescription();
        }

        [CanBeNull]
        public string GetExposureModeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagExposureMode,
                "Easy shooting", "Program", "Tv-priority", "Av-priority", "Manual", "A-DEP");
        }

        [CanBeNull]
        public string GetAfPointSelectedDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagAfPointSelected,
                0x3000,
                "None (MF)", "Auto selected", "Right", "Centre", "Left");
        }

        [CanBeNull]
        public string GetMeteringModeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagMeteringMode,
                3,
                "Evaluative", "Partial", "Centre weighted");
        }

        [CanBeNull]
        public string GetIsoDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagIso, out int value))
                return null;

            // Canon PowerShot S3 is special
            const int canonMask = 0x4000;
            if ((value & canonMask) != 0)
                return (value & ~canonMask).ToString();

            switch (value)
            {
                case 0:  return "Not specified (see ISOSpeedRatings tag)";
                case 15: return "Auto";
                case 16: return "50";
                case 17: return "100";
                case 18: return "200";
                case 19: return "400";
                default: return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagSharpness, out int value))
                return null;

            switch (value)
            {
                case 0xFFFF: return "Low";
                case 0x0000: return "Normal";
                case 0x0001: return "High";
                default:     return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagSaturation, out int value))
                return null;

            switch (value)
            {
                case 0xFFFF: return "Low";
                case 0x0000: return "Normal";
                case 0x0001: return "High";
                default:     return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagContrast, out int value))
                return null;

            switch (value)
            {
                case 0xFFFF: return "Low";
                case 0x0000: return "Normal";
                case 0x0001: return "High";
                default:     return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetEasyShootingModeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagEasyShootingMode, "Full auto", "Manual", "Landscape", "Fast shutter", "Slow shutter", "Night", "B&W", "Sepia", "Portrait", "Sports", "Macro / Closeup", "Pan focus");
        }

        [CanBeNull]
        public string GetImageSizeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagImageSize, "Large", "Medium", "Small");
        }

        [CanBeNull]
        public string GetFocusMode1Description()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagFocusMode1, "One-shot", "AI Servo", "AI Focus", "Manual Focus", "Single", "Continuous", "Manual Focus");
        }

        // TODO should check field 32 here (FOCUS_MODE_2)
        [CanBeNull]
        public string GetContinuousDriveModeDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagContinuousDriveMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    if (Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagSelfTimerDelay, out int delay))
                        return delay == 0 ? "Single shot" : "Single shot with self-timer";
                    goto case 1;
                case 1:
                    return "Continuous";
            }
            return "Unknown (" + value + ")";
        }

        [CanBeNull]
        public string GetFlashModeDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagFlashMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "No flash fired";
                case 1:
                    return "Auto";
                case 2:
                    return "On";
                case 3:
                    return "Red-eye reduction";
                case 4:
                    return "Slow-synchro";
                case 5:
                    return "Auto and red-eye reduction";
                case 6:
                    return "On and red-eye reduction";
                case 16:
                    // note: this value not set on Canon D30
                    return "External flash";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSelfTimerDelayDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagSelfTimerDelay, out int value))
                return null;

            // TODO find an image that tests this calculation
            return value == 0
                ? "Self timer not used"
                : $"{value*0.1d} sec";
        }

        [CanBeNull]
        public string GetMacroModeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagMacroMode, 1, "Macro", "Normal");
        }

        [CanBeNull]
        public string GetQualityDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagQuality, 2, "Normal", "Fine", null, "Superfine");
        }

        [CanBeNull]
        public string GetDigitalZoomDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagDigitalZoom, "No digital zoom", "2x", "4x");
        }

        [CanBeNull]
        public string GetRecordModeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagRecordMode, 1, "JPEG", "CRW+THM", "AVI+THM", "TIF", "TIF+JPEG", "CR2", "CR2+JPEG", null, "MOV", "MP4");
        }

        [CanBeNull]
        public string GetFocusTypeDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagFocusType, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Manual";
                case 1:
                    return "Auto";
                case 3:
                    return "Close-up (Macro)";
                case 8:
                    return "Locked (Pan Mode)";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetLensTypeDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagLensType, out int value))
                return null;

            return _lensTypeById.TryGetValue(value, out string lensType)
                ? lensType
                : $"Unknown ({value})";
        }

        [CanBeNull]
        public string GetMaxApertureDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagMaxAperture, out int value))
                return null;
            if (value > 512)
                return $"Unknown ({value})";
            return GetFStopDescription(Math.Exp(DecodeCanonEv(value) * Math.Log(2.0) / 2.0));
        }

        [CanBeNull]
        public string GetMinApertureDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagMinAperture, out int value))
                return null;
            if (value > 512)
                return $"Unknown ({value})";
            return GetFStopDescription(Math.Exp(DecodeCanonEv(value) * Math.Log(2.0) / 2.0));
        }

        [CanBeNull]
        public string GetFlashActivityDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagFlashActivity, "Flash did not fire", "Flash fired");
        }

        [CanBeNull]
        public string GetFocusContinuousDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagFocusContinuous, 0,
                "Single", "Continuous", null, null, null, null, null, null, "Manual");
        }

        [CanBeNull]
        public string GetAESettingDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagAESetting, 0,
                "Normal AE", "Exposure Compensation", "AE Lock", "AE Lock + Exposure Comp.", "No AE");
        }

        [CanBeNull]
        public string GetDisplayApertureDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagDisplayAperture, out int value))
                return null;

            if (value == ushort.MaxValue)
                return value.ToString();
            return GetFStopDescription(value / 10.0);
        }

        [CanBeNull]
        public string GetSpotMeteringModeDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagSpotMeteringMode, 0, "Center", "AF Point");
        }

        [CanBeNull]
        public string GetPhotoEffectDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagPhotoEffect, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "Vivid";
                case 2:
                    return "Neutral";
                case 3:
                    return "Smooth";
                case 4:
                    return "Sepia";
                case 5:
                    return "B&W";
                case 6:
                    return "Custom";
                case 100:
                    return "My Color Data";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetManualFlashOutputDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagManualFlashOutput, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "n/a";
                case 0x500:
                    return "Full";
                case 0x502:
                    return "Medium";
                case 0x504:
                    return "Low";
                case 0x7fff:
                    return "n/a";   // (EOS models)
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetColorToneDescription()
        {
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagColorTone, out int value))
                return null;

            return value == 0x7fff ? "n/a" : value.ToString();
        }

        [CanBeNull]
        public string GetSRawQualityDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagSRawQuality, 0, "n/a", "sRAW1 (mRAW)", "sRAW2 (sRAW)");
        }

        /// <summary>
        /// Canon hex-based EV (modulo 0x20) to real number
        /// </summary>
        /// <remarks>
        /// <code>
        /// 0x00 -> 0
        /// 0x0c -> 0.33333
        /// 0x10 -> 0.5
        /// 0x14 -> 0.66666
        /// 0x20 -> 1   ... etc
        /// </code>
        /// <para />
        /// Converted from Exiftool version 10.10 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\Canon.pm
        /// </remarks>
        /// <param name="val">value to convert</param>
        [Pure]
        private static double DecodeCanonEv(int val)
        {
            var sign = 1;
            if (val < 0)
            {
                val = -val;
                sign = -1;
            }

            var frac = val & 0x1f;
            val -= frac;

            if (frac == 0x0c)
                frac = 0x20 / 3;
            else if (frac == 0x14)
                frac = 0x40 / 3;

            return sign * (val + frac) / (double)0x20;
        }

        /// <summary>
        /// Map from <see cref="CanonMakernoteDirectory.CameraSettings.TagLensType"/> to string descriptions.
        /// </summary>
        /// <remarks>
        /// Data sourced from http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Canon.html#LensType
        /// <para />
        /// Note that only Canon lenses are listed. Lenses from other manufacturers may identify themselves to the camera
        /// as being from this set, but in fact may be quite different. This limits the usefulness of this data, unfortunately.
        /// </remarks>
        private readonly Dictionary<int, string> _lensTypeById = new Dictionary<int, string>
        {
            { 1, "Canon EF 50mm f/1.8" },
            { 2, "Canon EF 28mm f/2.8" },
            { 3, "Canon EF 135mm f/2.8 Soft" },
            { 4, "Canon EF 35-105mm f/3.5-4.5 or Sigma Lens" },
            { 5, "Canon EF 35-70mm f/3.5-4.5" },
            { 6, "Canon EF 28-70mm f/3.5-4.5 or Sigma or Tokina Lens" },
            { 7, "Canon EF 100-300mm f/5.6L" },
            { 8, "Canon EF 100-300mm f/5.6 or Sigma or Tokina Lens" },
            { 9, "Canon EF 70-210mm f/4" },
            { 10, "Canon EF 50mm f/2.5 Macro or Sigma Lens" },
            { 11, "Canon EF 35mm f/2" },
            { 13, "Canon EF 15mm f/2.8 Fisheye" },
            { 14, "Canon EF 50-200mm f/3.5-4.5L" },
            { 15, "Canon EF 50-200mm f/3.5-4.5" },
            { 16, "Canon EF 35-135mm f/3.5-4.5" },
            { 17, "Canon EF 35-70mm f/3.5-4.5A" },
            { 18, "Canon EF 28-70mm f/3.5-4.5" },
            { 20, "Canon EF 100-200mm f/4.5A" },
            { 21, "Canon EF 80-200mm f/2.8L" },
            { 22, "Canon EF 20-35mm f/2.8L or Tokina Lens" },
            { 23, "Canon EF 35-105mm f/3.5-4.5" },
            { 24, "Canon EF 35-80mm f/4-5.6 Power Zoom" },
            { 25, "Canon EF 35-80mm f/4-5.6 Power Zoom" },
            { 26, "Canon EF 100mm f/2.8 Macro or Other Lens" },
            { 27, "Canon EF 35-80mm f/4-5.6" },
            { 28, "Canon EF 80-200mm f/4.5-5.6 or Tamron Lens" },
            { 29, "Canon EF 50mm f/1.8 II" },
            { 30, "Canon EF 35-105mm f/4.5-5.6" },
            { 31, "Canon EF 75-300mm f/4-5.6 or Tamron Lens" },
            { 32, "Canon EF 24mm f/2.8 or Sigma Lens" },
            { 33, "Voigtlander or Carl Zeiss Lens" },
            { 35, "Canon EF 35-80mm f/4-5.6" },
            { 36, "Canon EF 38-76mm f/4.5-5.6" },
            { 37, "Canon EF 35-80mm f/4-5.6 or Tamron Lens" },
            { 38, "Canon EF 80-200mm f/4.5-5.6" },
            { 39, "Canon EF 75-300mm f/4-5.6" },
            { 40, "Canon EF 28-80mm f/3.5-5.6" },
            { 41, "Canon EF 28-90mm f/4-5.6" },
            { 42, "Canon EF 28-200mm f/3.5-5.6 or Tamron Lens" },
            { 43, "Canon EF 28-105mm f/4-5.6" },
            { 44, "Canon EF 90-300mm f/4.5-5.6" },
            { 45, "Canon EF-S 18-55mm f/3.5-5.6 [II]" },
            { 46, "Canon EF 28-90mm f/4-5.6" },
            { 47, "Zeiss Milvus 35mm f/2 or 50mm f/2" },
            { 48, "Canon EF-S 18-55mm f/3.5-5.6 IS" },
            { 49, "Canon EF-S 55-250mm f/4-5.6 IS" },
            { 50, "Canon EF-S 18-200mm f/3.5-5.6 IS" },
            { 51, "Canon EF-S 18-135mm f/3.5-5.6 IS" },
            { 52, "Canon EF-S 18-55mm f/3.5-5.6 IS II" },
            { 53, "Canon EF-S 18-55mm f/3.5-5.6 III" },
            { 54, "Canon EF-S 55-250mm f/4-5.6 IS II" },
            { 94, "Canon TS-E 17mm f/4L" },
            { 95, "Canon TS-E 24.0mm f/3.5 L II" },
            { 124, "Canon MP-E 65mm f/2.8 1-5x Macro Photo" },
            { 125, "Canon TS-E 24mm f/3.5L" },
            { 126, "Canon TS-E 45mm f/2.8" },
            { 127, "Canon TS-E 90mm f/2.8" },
            { 129, "Canon EF 300mm f/2.8L" },
            { 130, "Canon EF 50mm f/1.0L" },
            { 131, "Canon EF 28-80mm f/2.8-4L or Sigma Lens" },
            { 132, "Canon EF 1200mm f/5.6L" },
            { 134, "Canon EF 600mm f/4L IS" },
            { 135, "Canon EF 200mm f/1.8L" },
            { 136, "Canon EF 300mm f/2.8L" },
            { 137, "Canon EF 85mm f/1.2L or Sigma or Tamron Lens" },
            { 138, "Canon EF 28-80mm f/2.8-4L" },
            { 139, "Canon EF 400mm f/2.8L" },
            { 140, "Canon EF 500mm f/4.5L" },
            { 141, "Canon EF 500mm f/4.5L" },
            { 142, "Canon EF 300mm f/2.8L IS" },
            { 143, "Canon EF 500mm f/4L IS or Sigma Lens" },
            { 144, "Canon EF 35-135mm f/4-5.6 USM" },
            { 145, "Canon EF 100-300mm f/4.5-5.6 USM" },
            { 146, "Canon EF 70-210mm f/3.5-4.5 USM" },
            { 147, "Canon EF 35-135mm f/4-5.6 USM" },
            { 148, "Canon EF 28-80mm f/3.5-5.6 USM" },
            { 149, "Canon EF 100mm f/2 USM" },
            { 150, "Canon EF 14mm f/2.8L or Sigma Lens" },
            { 151, "Canon EF 200mm f/2.8L" },
            { 152, "Canon EF 300mm f/4L IS or Sigma Lens" },
            { 153, "Canon EF 35-350mm f/3.5-5.6L or Sigma or Tamron Lens" },
            { 154, "Canon EF 20mm f/2.8 USM or Zeiss Lens" },
            { 155, "Canon EF 85mm f/1.8 USM" },
            { 156, "Canon EF 28-105mm f/3.5-4.5 USM or Tamron Lens" },
            { 160, "Canon EF 20-35mm f/3.5-4.5 USM or Tamron or Tokina Lens" },
            { 161, "Canon EF 28-70mm f/2.8L or Sigma or Tamron Lens" },
            { 162, "Canon EF 200mm f/2.8L" },
            { 163, "Canon EF 300mm f/4L" },
            { 164, "Canon EF 400mm f/5.6L" },
            { 165, "Canon EF 70-200mm f/2.8 L" },
            { 166, "Canon EF 70-200mm f/2.8 L + 1.4x" },
            { 167, "Canon EF 70-200mm f/2.8 L + 2x" },
            { 168, "Canon EF 28mm f/1.8 USM or Sigma Lens" },
            { 169, "Canon EF 17-35mm f/2.8L or Sigma Lens" },
            { 170, "Canon EF 200mm f/2.8L II" },
            { 171, "Canon EF 300mm f/4L" },
            { 172, "Canon EF 400mm f/5.6L or Sigma Lens" },
            { 173, "Canon EF 180mm Macro f/3.5L or Sigma Lens" },
            { 174, "Canon EF 135mm f/2L or Other Lens" },
            { 175, "Canon EF 400mm f/2.8L" },
            { 176, "Canon EF 24-85mm f/3.5-4.5 USM" },
            { 177, "Canon EF 300mm f/4L IS" },
            { 178, "Canon EF 28-135mm f/3.5-5.6 IS" },
            { 179, "Canon EF 24mm f/1.4L" },
            { 180, "Canon EF 35mm f/1.4L or Other Lens" },
            { 181, "Canon EF 100-400mm f/4.5-5.6L IS + 1.4x or Sigma Lens" },
            { 182, "Canon EF 100-400mm f/4.5-5.6L IS + 2x or Sigma Lens" },
            { 183, "Canon EF 100-400mm f/4.5-5.6L IS or Sigma Lens" },
            { 184, "Canon EF 400mm f/2.8L + 2x" },
            { 185, "Canon EF 600mm f/4L IS" },
            { 186, "Canon EF 70-200mm f/4L" },
            { 187, "Canon EF 70-200mm f/4L + 1.4x" },
            { 188, "Canon EF 70-200mm f/4L + 2x" },
            { 189, "Canon EF 70-200mm f/4L + 2.8x" },
            { 190, "Canon EF 100mm f/2.8 Macro USM" },
            { 191, "Canon EF 400mm f/4 DO IS" },
            { 193, "Canon EF 35-80mm f/4-5.6 USM" },
            { 194, "Canon EF 80-200mm f/4.5-5.6 USM" },
            { 195, "Canon EF 35-105mm f/4.5-5.6 USM" },
            { 196, "Canon EF 75-300mm f/4-5.6 USM" },
            { 197, "Canon EF 75-300mm f/4-5.6 IS USM" },
            { 198, "Canon EF 50mm f/1.4 USM or Zeiss Lens" },
            { 199, "Canon EF 28-80mm f/3.5-5.6 USM" },
            { 200, "Canon EF 75-300mm f/4-5.6 USM" },
            { 201, "Canon EF 28-80mm f/3.5-5.6 USM" },
            { 202, "Canon EF 28-80mm f/3.5-5.6 USM IV" },
            { 208, "Canon EF 22-55mm f/4-5.6 USM" },
            { 209, "Canon EF 55-200mm f/4.5-5.6" },
            { 210, "Canon EF 28-90mm f/4-5.6 USM" },
            { 211, "Canon EF 28-200mm f/3.5-5.6 USM" },
            { 212, "Canon EF 28-105mm f/4-5.6 USM" },
            { 213, "Canon EF 90-300mm f/4.5-5.6 USM or Tamron Lens" },
            { 214, "Canon EF-S 18-55mm f/3.5-5.6 USM" },
            { 215, "Canon EF 55-200mm f/4.5-5.6 II USM" },
            { 217, "Tamron AF 18-270mm f/3.5-6.3 Di II VC PZD" },
            { 224, "Canon EF 70-200mm f/2.8L IS" },
            { 225, "Canon EF 70-200mm f/2.8L IS + 1.4x" },
            { 226, "Canon EF 70-200mm f/2.8L IS + 2x" },
            { 227, "Canon EF 70-200mm f/2.8L IS + 2.8x" },
            { 228, "Canon EF 28-105mm f/3.5-4.5 USM" },
            { 229, "Canon EF 16-35mm f/2.8L" },
            { 230, "Canon EF 24-70mm f/2.8L" },
            { 231, "Canon EF 17-40mm f/4L" },
            { 232, "Canon EF 70-300mm f/4.5-5.6 DO IS USM" },
            { 233, "Canon EF 28-300mm f/3.5-5.6L IS" },
            { 234, "Canon EF-S 17-85mm f/4-5.6 IS USM or Tokina Lens" },
            { 235, "Canon EF-S 10-22mm f/3.5-4.5 USM" },
            { 236, "Canon EF-S 60mm f/2.8 Macro USM" },
            { 237, "Canon EF 24-105mm f/4L IS" },
            { 238, "Canon EF 70-300mm f/4-5.6 IS USM" },
            { 239, "Canon EF 85mm f/1.2L II" },
            { 240, "Canon EF-S 17-55mm f/2.8 IS USM" },
            { 241, "Canon EF 50mm f/1.2L" },
            { 242, "Canon EF 70-200mm f/4L IS" },
            { 243, "Canon EF 70-200mm f/4L IS + 1.4x" },
            { 244, "Canon EF 70-200mm f/4L IS + 2x" },
            { 245, "Canon EF 70-200mm f/4L IS + 2.8x" },
            { 246, "Canon EF 16-35mm f/2.8L II" },
            { 247, "Canon EF 14mm f/2.8L II USM" },
            { 248, "Canon EF 200mm f/2L IS or Sigma Lens" },
            { 249, "Canon EF 800mm f/5.6L IS" },
            { 250, "Canon EF 24mm f/1.4L II or Sigma Lens" },
            { 251, "Canon EF 70-200mm f/2.8L IS II USM" },
            { 252, "Canon EF 70-200mm f/2.8L IS II USM + 1.4x" },
            { 253, "Canon EF 70-200mm f/2.8L IS II USM + 2x" },
            { 254, "Canon EF 100mm f/2.8L Macro IS USM" },
            { 255, "Sigma 24-105mm f/4 DG OS HSM | A or Other Sigma Lens" },
            { 488, "Canon EF-S 15-85mm f/3.5-5.6 IS USM" },
            { 489, "Canon EF 70-300mm f/4-5.6L IS USM" },
            { 490, "Canon EF 8-15mm f/4L Fisheye USM" },
            { 491, "Canon EF 300mm f/2.8L IS II USM" },
            { 492, "Canon EF 400mm f/2.8L IS II USM" },
            { 493, "Canon EF 500mm f/4L IS II USM or EF 24-105mm f4L IS USM" },
            { 494, "Canon EF 600mm f/4.0L IS II USM" },
            { 495, "Canon EF 24-70mm f/2.8L II USM" },
            { 496, "Canon EF 200-400mm f/4L IS USM" },
            { 499, "Canon EF 200-400mm f/4L IS USM + 1.4x" },
            { 502, "Canon EF 28mm f/2.8 IS USM" },
            { 503, "Canon EF 24mm f/2.8 IS USM" },
            { 504, "Canon EF 24-70mm f/4L IS USM" },
            { 505, "Canon EF 35mm f/2 IS USM" },
            { 506, "Canon EF 400mm f/4 DO IS II USM" },
            { 507, "Canon EF 16-35mm f/4L IS USM" },
            { 508, "Canon EF 11-24mm f/4L USM" },
            { 747, "Canon EF 100-400mm f/4.5-5.6L IS II USM" },
            { 750, "Canon EF 35mm f/1.4L II USM" },
            { 4142, "Canon EF-S 18-135mm f/3.5-5.6 IS STM" },
            { 4143, "Canon EF-M 18-55mm f/3.5-5.6 IS STM or Tamron Lens" },
            { 4144, "Canon EF 40mm f/2.8 STM" },
            { 4145, "Canon EF-M 22mm f/2 STM" },
            { 4146, "Canon EF-S 18-55mm f/3.5-5.6 IS STM" },
            { 4147, "Canon EF-M 11-22mm f/4-5.6 IS STM" },
            { 4148, "Canon EF-S 55-250mm f/4-5.6 IS STM" },
            { 4149, "Canon EF-M 55-200mm f/4.5-6.3 IS STM" },
            { 4150, "Canon EF-S 10-18mm f/4.5-5.6 IS STM" },
            { 4152, "Canon EF 24-105mm f/3.5-5.6 IS STM" },
            { 4153, "Canon EF-M 15-45mm f/3.5-6.3 IS STM" },
            { 4154, "Canon EF-S 24mm f/2.8 STM" },
            { 4156, "Canon EF 50mm f/1.8 STM" },
            { 36912, "Canon EF-S 18-135mm f/3.5-5.6 IS USM" },
            { 65535, "N/A" }
        };
    }
}
