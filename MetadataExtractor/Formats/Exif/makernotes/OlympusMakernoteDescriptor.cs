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

using System;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class OlympusMakernoteDescriptor : TagDescriptor<OlympusMakernoteDirectory>
    {
        public OlympusMakernoteDescriptor([NotNull] OlympusMakernoteDirectory directory)
            : base(directory)
        {
        }

        // TODO extend support for some offset-encoded byte[] tags: http://www.ozhiker.com/electronics/pjmt/jpeg_info/olympus_mn.html
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusMakernoteDirectory.TagMakernoteVersion:
                    return GetMakernoteVersionDescription();
                case OlympusMakernoteDirectory.TagColourMode:
                    return GetColorModeDescription();
                case OlympusMakernoteDirectory.TagImageQuality1:
                    return GetImageQuality1Description();
                case OlympusMakernoteDirectory.TagImageQuality2:
                    return GetImageQuality2Description();
                case OlympusMakernoteDirectory.TagSpecialMode:
                    return GetSpecialModeDescription();
                case OlympusMakernoteDirectory.TagJpegQuality:
                    return GetJpegQualityDescription();
                case OlympusMakernoteDirectory.TagMacroMode:
                    return GetMacroModeDescription();
                case OlympusMakernoteDirectory.TagBwMode:
                    return GetBwModeDescription();
                case OlympusMakernoteDirectory.TagDigiZoomRatio:
                    return GetDigiZoomRatioDescription();
                case OlympusMakernoteDirectory.TagCameraId:
                    return GetCameraIdDescription();
                case OlympusMakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                case OlympusMakernoteDirectory.TagFocusRange:
                    return GetFocusRangeDescription();
                case OlympusMakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case OlympusMakernoteDirectory.TagSharpness:
                    return GetSharpnessDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagExposureMode:
                    return GetExposureModeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFlashMode:
                    return GetFlashModeCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagImageSize:
                    return GetImageSizeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagImageQuality:
                    return GetImageQualityDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagShootingMode:
                    return GetShootingModeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagMeteringMode:
                    return GetMeteringModeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagApexFilmSpeedValue:
                    return GetApexFilmSpeedDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagApexShutterSpeedTimeValue:
                    return GetApexShutterSpeedTimeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagApexApertureValue:
                    return GetApexApertureDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagMacroMode:
                    return GetMacroModeCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagDigitalZoom:
                    return GetDigitalZoomDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagExposureCompensation:
                    return GetExposureCompensationDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagBracketStep:
                    return GetBracketStepDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagIntervalLength:
                    return GetIntervalLengthDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagIntervalNumber:
                    return GetIntervalNumberDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFocalLength:
                    return GetFocalLengthDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFocusDistance:
                    return GetFocusDistanceDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFlashFired:
                    return GetFlastFiredDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagDate:
                    return GetDateDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagTime:
                    return GetTimeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagMaxApertureAtFocalLength:
                    return GetMaxApertureAtFocalLengthDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFileNumberMemory:
                    return GetFileNumberMemoryDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagLastFileNumber:
                    return GetLastFileNumberDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceRed:
                    return GetWhiteBalanceRedDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceGreen:
                    return GetWhiteBalanceGreenDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceBlue:
                    return GetWhiteBalanceBlueDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSaturation:
                    return GetSaturationDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagContrast:
                    return GetContrastDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSharpness:
                    return GetSharpnessCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSubjectProgram:
                    return GetSubjectProgramDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFlashCompensation:
                    return GetFlastCompensationDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagIsoSetting:
                    return GetIsoSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagCameraModel:
                    return GetCameraModelDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagIntervalMode:
                    return GetIntervalModeDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFolderName:
                    return GetFolderNameDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagColorMode:
                    return GetColorModeCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagColorFilter:
                    return GetColorFilterDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagBlackAndWhiteFilter:
                    return GetBlackAndWhiteFilterDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagInternalFlash:
                    return GetInternalFlashDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagApexBrightnessValue:
                    return GetApexBrightnessDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointXCoordinate:
                    return GetSpotFocusPointXCoordinateDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointYCoordinate:
                    return GetSpotFocusPointYCoordinateDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagWideFocusZone:
                    return GetWideFocusZoneDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFocusMode:
                    return GetFocusModeCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFocusArea:
                    return GetFocusAreaDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagDecSwitchPosition:
                    return GetDecSwitchPositionDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetExposureModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagExposureMode,
                "P", "A", "S", "M");
        }

        [CanBeNull]
        public string GetFlashModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFlashMode,
                "Normal", "Red-eye reduction", "Rear flash sync", "Wireless");
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalance,
                "Auto", "Daylight", "Cloudy", "Tungsten", null, "Custom", null, "Fluorescent", "Fluorescent 2", null, null, "Custom 2", "Custom 3");
        }

        [CanBeNull]
        public string GetImageSizeDescription()
        {
            // This is a pretty weird way to store this information!
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagImageSize,
                "2560 x 1920", "1600 x 1200", "1280 x 960", "640 x 480");
        }

        [CanBeNull]
        public string GetImageQualityDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagImageQuality,
                "Raw", "Super Fine", "Fine", "Standard", "Economy", "Extra Fine");
        }

        [CanBeNull]
        public string GetShootingModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagShootingMode,
                "Single", "Continuous", "Self Timer", null, "Bracketing", "Interval", "UHS Continuous", "HS Continuous");
        }

        [CanBeNull]
        public string GetMeteringModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagMeteringMode,
                "Multi-Segment", "Centre Weighted", "Spot");
        }

        [CanBeNull]
        public string GetApexFilmSpeedDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Speed value = value/8-1 ,
            // ISO = (2^(value/8-1))*3.125
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagApexFilmSpeedValue);
            if (value == null)
                return null;
            var iso = Math.Pow(((double)value / 8d) - 1, 2) * 3.125;
            return iso.ToString();
        }

        [CanBeNull]
        public string GetApexShutterSpeedTimeDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Time value = value/8-6 ,
            // ShutterSpeed = 2^( (48-value)/8 ),
            // Due to rounding error value=8 should be displayed as 30 sec.
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagApexShutterSpeedTimeValue);
            if (value == null)
                return null;
            var shutterSpeed = Math.Pow((49 - (long)value) / 8d, 2);
            return shutterSpeed + " sec";
        }

        [CanBeNull]
        public string GetApexApertureDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Aperture Value = value/8-1 ,
            // Aperture F-stop = 2^( value/16-0.5 )
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagApexApertureValue);
            if (value == null)
                return null;
            var fStop = Math.Pow((value.Value / 16d) - 0.5, 2);
            return "f/" + fStop.ToString("0.0");
        }

        [CanBeNull]
        public string GetMacroModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagMacroMode,
                "Off", "On");
        }

        [CanBeNull]
        public string GetDigitalZoomDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagDigitalZoom,
                "Off", "Electronic magnification", "Digital zoom 2x");
        }

        [CanBeNull]
        public string GetExposureCompensationDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagExposureCompensation);
            return value == null ? null : (((double)value / 3d) - 2) + " EV";
        }

        [CanBeNull]
        public string GetBracketStepDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagBracketStep,
                "1/3 EV", "2/3 EV", "1 EV");
        }

        [CanBeNull]
        public string GetIntervalLengthDescription()
        {
            if (!Directory.IsIntervalMode())
                return "N/A";
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagIntervalLength);
            return value == null ? null : value + " min";
        }

        [CanBeNull]
        public string GetIntervalNumberDescription()
        {
            if (!Directory.IsIntervalMode())
                return "N/A";
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagIntervalNumber);
            return value == null ? null : Convert.ToString((long)value);
        }

        [CanBeNull]
        public string GetFocalLengthDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagFocalLength);
            return value == null ? null : ((object)((double)value / 256d)).ToString() + " mm";
        }

        [CanBeNull]
        public string GetFocusDistanceDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagFocusDistance);
            return value == null ? null : value == 0 ? "Infinity" : value + " mm";
        }

        [CanBeNull]
        public string GetFlastFiredDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFlashFired,
                "No", "Yes");
        }

        [CanBeNull]
        public string GetDateDescription()
        {
            // day = value%256,
            // month = floor( (value - floor( value/65536 )*65536 )/256 )
            // year = floor( value/65536)
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagDate);
            if (value == null)
                return null;
            var day = value & 0xFF;
            var month = (value >> 16) & 0xFF;
            var year = (value >> 8) & 0xFF;
            return new DateTime((int)year + 1970, (int)month + 1, (int)day).ToString("ddd MMM dd HH:mm:ss zzz yyyy");
        }

        [CanBeNull]
        public string GetTimeDescription()
        {
            // hours = floor( value/65536 ),
            // minutes = floor( ( value - floor( value/65536 )*65536 )/256 ),
            // seconds = value%256
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagTime);
            if (value == null)
                return null;
            var hours = (value >> 8) & 0xFF;
            var minutes = (value >> 16) & 0xFF;
            var seconds = value & 0xFF;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }

        [CanBeNull]
        public string GetMaxApertureAtFocalLengthDescription()
        {
            // Aperture F-Stop = 2^(value/16-0.5)
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagTime);
            if (value == null)
                return null;
            var fStop = Math.Pow(((double)value / 16d) - 0.5, 2);
            return "f/" + fStop.ToString("0.0");
        }

        [CanBeNull]
        public string GetFileNumberMemoryDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFileNumberMemory,
                "Off", "On");
        }

        [CanBeNull]
        public string GetLastFileNumberDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagLastFileNumber);
            return value == null ? null : value == 0 ? "File Number Memory Off" : Convert.ToString((long)value);
        }

        [CanBeNull]
        public string GetWhiteBalanceRedDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceRed);
            return value == null ? null : ((object)((double)value / 256d)).ToString();
        }

        [CanBeNull]
        public string GetWhiteBalanceGreenDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceGreen);
            return value == null ? null : ((object)((double)value / 256d)).ToString();
        }

        [CanBeNull]
        public string GetWhiteBalanceBlueDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceBlue);
            return value == null ? null : ((object)((double)value / 256d)).ToString();
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagSaturation);
            return value == null ? null : Convert.ToString((long)value - 3);
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagContrast);
            return value == null ? null : Convert.ToString((long)value - 3);
        }

        [CanBeNull]
        public string GetSharpnessCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagSharpness,
                "Hard", "Normal", "Soft");
        }

        [CanBeNull]
        public string GetSubjectProgramDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagSubjectProgram,
                "None", "Portrait", "Text", "Night Portrait", "Sunset", "Sports Action");
        }

        [CanBeNull]
        public string GetFlastCompensationDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagFlashCompensation);
            return value == null ? null : (((long)value - 6) / 3d) + " EV";
        }

        [CanBeNull]
        public string GetIsoSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagIsoSetting,
                "100", "200", "400", "800", "Auto", "64");
        }

        [CanBeNull]
        public string GetCameraModelDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagCameraModel,
                "DiMAGE 7", "DiMAGE 5", "DiMAGE S304", "DiMAGE S404", "DiMAGE 7i", "DiMAGE 7Hi", "DiMAGE A1", "DiMAGE S414");
        }

        [CanBeNull]
        public string GetIntervalModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagIntervalMode,
                "Still Image", "Time Lapse Movie");
        }

        [CanBeNull]
        public string GetFolderNameDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFolderName,
                "Standard Form", "Data Form");
        }

        [CanBeNull]
        public string GetColorModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagColorMode,
                "Natural Color", "Black & White", "Vivid Color", "Solarization", "AdobeRGB");
        }

        [CanBeNull]
        public string GetColorFilterDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagColorFilter);
            return value == null ? null : Convert.ToString((long)value - 3);
        }

        [CanBeNull]
        public string GetBlackAndWhiteFilterDescription()
        {
            return base.GetDescription(OlympusMakernoteDirectory.CameraSettings.TagBlackAndWhiteFilter);
        }

        [CanBeNull]
        public string GetInternalFlashDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagInternalFlash,
                "Did Not Fire", "Fired");
        }

        [CanBeNull]
        public string GetApexBrightnessDescription()
        {
            var value = Directory.GetInt64Nullable(OlympusMakernoteDirectory.CameraSettings.TagApexBrightnessValue);
            return value == null ? null : ((object)(((double)value / 8d) - 6)).ToString();
        }

        [CanBeNull]
        public string GetSpotFocusPointXCoordinateDescription()
        {
            return base.GetDescription(OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointXCoordinate);
        }

        [CanBeNull]
        public string GetSpotFocusPointYCoordinateDescription()
        {
            return base.GetDescription(OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointYCoordinate);
        }

        [CanBeNull]
        public string GetWideFocusZoneDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagWideFocusZone,
                "No Zone or AF Failed", "Center Zone (Horizontal Orientation)", "Center Zone (Vertical Orientation)", "Left Zone", "Right Zone");
        }

        [CanBeNull]
        public string GetFocusModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFocusMode,
                "Auto Focus", "Manual Focus");
        }

        [CanBeNull]
        public string GetFocusAreaDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFocusArea,
                "Wide Focus (Normal)", "Spot Focus");
        }

        [CanBeNull]
        public string GetDecSwitchPositionDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagDecSwitchPosition,
                "Exposure", "Contrast", "Saturation", "Filter");
        }

        [CanBeNull]
        public string GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(OlympusMakernoteDirectory.TagMakernoteVersion, 2);
        }

        [CanBeNull]
        public string GetImageQuality2Description()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagImageQuality2,
                "Raw", "Super Fine", "Fine", "Standard", "Extra Fine");
        }

        [CanBeNull]
        public string GetImageQuality1Description()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagImageQuality1,
                "Raw", "Super Fine", "Fine", "Standard", "Extra Fine");
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagColourMode,
                "Natural Colour", "Black & White", "Vivid Colour", "Solarization", "AdobeRGB");
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagSharpness,
                "Normal", "Hard", "Soft");
        }

        [CanBeNull]
        public string GetFocusModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagFocusMode,
                "Auto", "Manual");
        }

        [CanBeNull]
        public string GetFocusRangeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagFocusRange,
                "Normal", "Macro");
        }

        [CanBeNull]
        public string GetFlashModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagFlashMode,
                null, null, "On", "Off");
        }

        [CanBeNull]
        public string GetDigiZoomRatioDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagDigiZoomRatio,
                "Normal", null, "Digital 2x Zoom");
        }

        [CanBeNull]
        public string GetCameraIdDescription()
        {
            var bytes = Directory.GetByteArray(OlympusMakernoteDirectory.TagCameraId);
            if (bytes == null)
                return null;
            return Encoding.UTF8.GetString(bytes);
        }

        [CanBeNull]
        public string GetMacroModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagMacroMode,
                "Normal (no macro)", "Macro");
        }

        [CanBeNull]
        public string GetBwModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagBwMode,
                "Off", "On");
        }

        [CanBeNull]
        public string GetJpegQualityDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagJpegQuality,
                1,
                "Standard Quality", "High Quality", "Super High Quality");
        }

        [CanBeNull]
        public string GetSpecialModeDescription()
        {
            var values = (long[])Directory.GetObject(OlympusMakernoteDirectory.TagSpecialMode);
            if (values == null)
                return null;
            if (values.Length < 1)
                return string.Empty;

            var description = new StringBuilder();

            switch (values[0])
            {
                case 0:
                    description.Append("Normal picture taking mode");
                    break;
                case 1:
                    description.Append("Unknown picture taking mode");
                    break;
                case 2:
                    description.Append("Fast picture taking mode");
                    break;
                case 3:
                    description.Append("Panorama picture taking mode");
                    break;
                default:
                    description.Append("Unknown picture taking mode");
                    break;
            }

            if (values.Length >= 2)
            {
                switch (values[1])
                {
                    case 0:
                        break;
                    case 1:
                        description.Append(" / 1st in a sequence");
                        break;
                    case 2:
                        description.Append(" / 2nd in a sequence");
                        break;
                    case 3:
                        description.Append(" / 3rd in a sequence");
                        break;
                    default:
                        description.AppendFormat(" / {0}th in a sequence", values[1]);
                        break;
                }
            }

            if (values.Length >= 3)
            {
                switch (values[2])
                {
                    case 1:
                        description.Append(" / Left to right panorama direction");
                        break;
                    case 2:
                        description.Append(" / Right to left panorama direction");
                        break;
                    case 3:
                        description.Append(" / Bottom to top panorama direction");
                        break;
                    case 4:
                        description.Append(" / Top to bottom panorama direction");
                        break;
                }
            }

            return description.ToString();
        }
    }
}
