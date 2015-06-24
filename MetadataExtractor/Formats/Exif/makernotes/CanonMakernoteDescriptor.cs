#region License
//
// Copyright 2002-2015 Drew Noakes
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

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="CanonMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
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
            int value;
            return Directory.TryGetInt32(CanonMakernoteDirectory.TagCanonSerialNumber, out value)
                ? string.Format("{0:X4}{1:D4}", (value >> 8) & 0xFF, value & 0xFF)
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
            int value;
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.FocalLength.TagFlashBias, out value))
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
            return ((isNegative) ? "-" : string.Empty) + (value / 32f).ToString("0.0###########") + " EV";
        }

        [CanBeNull]
        public string GetAfPointUsedDescription()
        {
            int value;
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.FocalLength.TagAfPointUsed, out value))
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
            int value;
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagFlashDetails, out value))
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
            int value;
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagFocalUnitsPerMm, out value))
                return null;

            return value != 0 ? value.ToString() : string.Empty;
        }

        [CanBeNull]
        public string GetShortFocalLengthDescription()
        {
            int value;
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagShortFocalLength, out value))
                return null;

            var units = GetFocalUnitsPerMillimetreDescription();
            return value + " " + units;
        }

        [CanBeNull]
        public string GetLongFocalLengthDescription()
        {
            int value;
            if (!Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagLongFocalLength, out value))
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
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagIso);
            if (value == null)
            {
                return null;
            }
            // Canon PowerShot S3 is special
            const int canonMask = 0x4000;
            if (((int)value & canonMask) > 0)
            {
                return string.Empty + ((int)value & ~canonMask);
            }
            switch (value)
            {
                case 0:
                {
                    return "Not specified (see ISOSpeedRatings tag)";
                }

                case 15:
                {
                    return "Auto";
                }

                case 16:
                {
                    return "50";
                }

                case 17:
                {
                    return "100";
                }

                case 18:
                {
                    return "200";
                }

                case 19:
                {
                    return "400";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagSharpness);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0xFFFF:
                {
                    return "Low";
                }

                case 0x000:
                {
                    return "Normal";
                }

                case 0x001:
                {
                    return "High";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagSaturation);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0xFFFF:
                {
                    return "Low";
                }

                case 0x000:
                {
                    return "Normal";
                }

                case 0x001:
                {
                    return "High";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagContrast);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0xFFFF:
                {
                    return "Low";
                }

                case 0x000:
                {
                    return "Normal";
                }

                case 0x001:
                {
                    return "High";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
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
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagContinuousDriveMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    var delay = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagSelfTimerDelay);
                    if (delay != null)
                    {
                        return delay == 0 ? "Single shot" : "Single shot with self-timer";
                    }
                    goto case 1;
                }

                case 1:
                {
                    return "Continuous";
                }
            }
            return "Unknown (" + value + ")";
        }

        [CanBeNull]
        public string GetFlashModeDescription()
        {
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagFlashMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "No flash fired";
                }

                case 1:
                {
                    return "Auto";
                }

                case 2:
                {
                    return "On";
                }

                case 3:
                {
                    return "Red-eye reduction";
                }

                case 4:
                {
                    return "Slow-synchro";
                }

                case 5:
                {
                    return "Auto and red-eye reduction";
                }

                case 6:
                {
                    return "On and red-eye reduction";
                }

                case 16:
                {
                    // note: this value not set on Canon D30
                    return "External flash";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetSelfTimerDelayDescription()
        {
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagSelfTimerDelay);
            if (value == null)
            {
                return null;
            }
            if (value == 0)
            {
                return "Self timer not used";
            }
            // TODO find an image that tests this calculation
            return ((object)((double)value * 0.1d)).ToString() + " sec";
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
        public string GetFocusTypeDescription()
        {
            var value = Directory.GetInt32Nullable(CanonMakernoteDirectory.CameraSettings.TagFocusType);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Manual";
                }

                case 1:
                {
                    return "Auto";
                }

                case 3:
                {
                    return "Close-up (Macro)";
                }

                case 8:
                {
                    return "Locked (Pan Mode)";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetLensTypeDescription()
        {
            int value;
            return !Directory.TryGetInt32(CanonMakernoteDirectory.CameraSettings.TagLensType, out value)
                ? null
                : "Lens type: " + value;
        }

        [CanBeNull]
        public string GetFlashActivityDescription()
        {
            return GetIndexedDescription(CanonMakernoteDirectory.CameraSettings.TagFlashActivity, "Flash did not fire", "Flash fired");
        }
    }
}
