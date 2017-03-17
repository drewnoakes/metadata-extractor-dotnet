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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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
                case OlympusMakernoteDirectory.TagDigitalZoom:
                    return GetDigitalZoomDescription();
                case OlympusMakernoteDirectory.TagFocalPlaneDiagonal:
                    return GetFocalPlaneDiagonalDescription();
                case OlympusMakernoteDirectory.TagCameraType:
                    return GetCameraTypeDescription();
                case OlympusMakernoteDirectory.TagCameraId:
                    return GetCameraIdDescription();
                case OlympusMakernoteDirectory.TagOneTouchWb:
                    return GetOneTouchWbDescription();
                case OlympusMakernoteDirectory.TagShutterSpeedValue:
                    return GetShutterSpeedDescription();
                case OlympusMakernoteDirectory.TagIsoValue:
                    return GetIsoValueDescription();
                case OlympusMakernoteDirectory.TagApertureValue:
                    return GetApertureValueDescription();
                case OlympusMakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                case OlympusMakernoteDirectory.TagFocusRange:
                    return GetFocusRangeDescription();
                case OlympusMakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case OlympusMakernoteDirectory.TagSharpness:
                    return GetSharpnessDescription();
                case OlympusMakernoteDirectory.TagColourMatrix:
                    return GetColorMatrixDescription();
                case OlympusMakernoteDirectory.TagWbMode:
                    return GetWbModeDescription();
                case OlympusMakernoteDirectory.TagRedBalance:
                    return GetRedBalanceDescription();
                case OlympusMakernoteDirectory.TagBlueBalance:
                    return GetBlueBalanceDescription();
                case OlympusMakernoteDirectory.TagContrast:
                    return GetContrastDescription();
                case OlympusMakernoteDirectory.TagPreviewImageValid:
                    return GetPreviewImageValidDescription();
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
                    return GetDigitalZoomCameraSettingDescription();
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
                    return GetFlashFiredDescription();
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
                    return GetContrastCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSharpness:
                    return GetSharpnessCameraSettingDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagSubjectProgram:
                    return GetSubjectProgramDescription();
                case OlympusMakernoteDirectory.CameraSettings.TagFlashCompensation:
                    return GetFlashCompensationDescription();
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
            if (!Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagApexFilmSpeedValue, out long value))
                return null;
            var iso = Math.Pow(value/8d - 1, 2)*3.125;
            return iso.ToString("0.##");
        }

        [CanBeNull]
        public string GetApexShutterSpeedTimeDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Time value = value/8-6 ,
            // ShutterSpeed = 2^( (48-value)/8 ),
            // Due to rounding error value=8 should be displayed as 30 sec.
            if (!Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagApexShutterSpeedTimeValue, out long value))
                return null;
            var shutterSpeed = Math.Pow((49 - value) / 8d, 2);
            return $"{shutterSpeed:0.###} sec";
        }

        [CanBeNull]
        public string GetApexApertureDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Aperture Value = value/8-1 ,
            // Aperture F-stop = 2^( value/16-0.5 )
            if (!Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagApexApertureValue, out long value))
                return null;
            var fStop = Math.Pow(value/16d - 0.5, 2);
            return GetFStopDescription(fStop);
        }

        [CanBeNull]
        public string GetMacroModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagMacroMode,
                "Off", "On");
        }

        [CanBeNull]
        public string GetDigitalZoomCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagDigitalZoom,
                "Off", "Electronic magnification", "Digital zoom 2x");
        }

        [CanBeNull]
        public string GetExposureCompensationDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagExposureCompensation, out long value)
                ? $"{value / 3d - 2:0.##} EV"
                : null;
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

            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagIntervalLength, out long value)
                ? value + " min"
                : null;
        }

        [CanBeNull]
        public string GetIntervalNumberDescription()
        {
            if (!Directory.IsIntervalMode())
                return "N/A";

            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagIntervalNumber, out long value)
                ? value.ToString()
                : null;
        }

        [CanBeNull]
        public string GetFocalLengthDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagFocalLength, out long value)
                ? GetFocalLengthDescription(value / 256d)
                : null;
        }

        [CanBeNull]
        public string GetFocusDistanceDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagFocusDistance, out long value)
                ? value == 0 ? "Infinity" : value + " mm"
                : null;
        }

        [CanBeNull]
        public string GetFlashFiredDescription()
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
            if (!Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagDate, out long value))
                return null;

            var day = (int)(value & 0xFF);
//            var month = (int)Math.Floor((value - Math.Floor(value/65536.0)*65536.0)/256.0);
//            var year = (int)Math.Floor(value/65536.0);
            var month = (int)(value >> 16) & 0xFF;
            var year = ((int)(value >> 8) & 0xFF) + 1970;

            if (!DateUtil.IsValidDate(year, month, day))
                return "Invalid date";

            return $"{year:0000}-{month + 1:00}-{day:00}";
        }

        [CanBeNull]
        public string GetTimeDescription()
        {
            // hours = floor( value/65536 ),
            // minutes = floor( ( value - floor( value/65536 )*65536 )/256 ),
            // seconds = value%256
            if (!Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagTime, out long value))
                return null;

            var hours = (int)((value >> 8) & 0xFF);
            var minutes = (int)((value >> 16) & 0xFF);
            var seconds = (int)(value & 0xFF);

            if (!DateUtil.IsValidTime(hours, minutes, seconds))
                return "Invalid date";

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        [CanBeNull]
        public string GetMaxApertureAtFocalLengthDescription()
        {
            // Aperture F-Stop = 2^(value/16-0.5)
            if (!Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagTime, out long value))
                return null;
            var fStop = Math.Pow(value/16d - 0.5, 2);
            return GetFStopDescription(fStop);
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
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagLastFileNumber, out long value)
                ? value == 0
                    ? "File Number Memory Off"
                    : value.ToString()
                : null;
        }

        [CanBeNull]
        public string GetWhiteBalanceRedDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceRed, out long value)
                ? (value / 256d).ToString("0.##")
                : null;
        }

        [CanBeNull]
        public string GetWhiteBalanceGreenDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceGreen, out long value)
                ? (value / 256d).ToString("0.##")
                : null;
        }

        [CanBeNull]
        public string GetWhiteBalanceBlueDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceBlue, out long value)
                ? (value / 256d).ToString("0.##")
                : null;
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagSaturation, out long value)
                ? (value - 3).ToString()
                : null;
        }

        [CanBeNull]
        public string GetContrastCameraSettingDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagContrast, out long value)
                ? (value - 3).ToString()
                : null;
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
        public string GetFlashCompensationDescription()
        {
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagFlashCompensation, out long value)
                ? $"{(value - 6) / 3d:0.##} EV"
                : null;
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
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagColorFilter, out long value)
                ? (value - 3).ToString()
                : null;
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
            return Directory.TryGetInt64(OlympusMakernoteDirectory.CameraSettings.TagApexBrightnessValue, out long value)
                ? (value/8d - 6).ToString()
                : null;
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
        public string GetColorMatrixDescription()
        {
            var values = Directory.GetObject(OlympusMakernoteDirectory.TagColourMatrix) as short[];
            if (values == null)
                return null;

            return string.Join(" ", values.Select(b => b.ToString()).ToArray());
        }

        [CanBeNull]
        public string GetWbModeDescription()
        {
            var values = Directory.GetObject(OlympusMakernoteDirectory.TagWbMode) as short[];
            if (values == null)
                return null;

            switch ($"{values[0]} {values[1]}".Trim())
            {
                case "1":
                case "1 0":
                    return "Auto";
                case "1 2":
                    return "Auto (2)";
                case "1 4":
                    return "Auto (4)";
                case "2 2":
                    return "3000 Kelvin";
                case "2 3":
                    return "3700 Kelvin";
                case "2 4":
                    return "4000 Kelvin";
                case "2 5":
                    return "4500 Kelvin";
                case "2 6":
                    return "5500 Kelvin";
                case "2 7":
                    return "6500 Kelvin";
                case "2 8":
                    return "7500 Kelvin";
                case "3 0":
                    return "One-touch";
                default:
                    return $"Unknown ({values[0]} {values[1]})";
            }
        }

        [CanBeNull]
        public string GetRedBalanceDescription()
        {
            var values = Directory.GetObject(OlympusMakernoteDirectory.TagRedBalance) as ushort[];
            if (values == null || values.Length < 2)
                return null;

            return (values[0] / 256.0d).ToString();
        }

        [CanBeNull]
        public string GetBlueBalanceDescription()
        {
            var values = Directory.GetObject(OlympusMakernoteDirectory.TagBlueBalance) as ushort[];
            if (values == null || values.Length < 2)
                return null;

            return (values[0] / 256.0d).ToString();
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagContrast,
                "High", "Normal", "Low");
        }

        [CanBeNull]
        public string GetPreviewImageValidDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagPreviewImageValid,
                "No", "Yes");
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
        public string GetDigitalZoomDescription()
        {
            if (!Directory.TryGetRational(OlympusMakernoteDirectory.TagDigitalZoom, out Rational value))
                return null;
            return value.ToSimpleString(allowDecimal: false);
        }

        [CanBeNull]
        public string GetFocalPlaneDiagonalDescription()
        {
            if (!Directory.TryGetRational(OlympusMakernoteDirectory.TagFocalPlaneDiagonal, out Rational value))
                return null;
            return value.ToDouble().ToString("0.###") + " mm";
        }

        [CanBeNull]
        public string GetCameraTypeDescription()
        {
            var cameratype = Directory.GetString(OlympusMakernoteDirectory.TagCameraType);
            if (cameratype == null)
                return null;

            if(OlympusMakernoteDirectory.OlympusCameraTypes.ContainsKey(cameratype))
                return OlympusMakernoteDirectory.OlympusCameraTypes[cameratype];

            return cameratype;
        }

        [CanBeNull]
        public string GetCameraIdDescription()
        {
            var bytes = Directory.GetByteArray(OlympusMakernoteDirectory.TagCameraId);
            if (bytes == null)
                return null;

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        [CanBeNull]
        public string GetOneTouchWbDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagOneTouchWb,
                "Off", "On", "On (Preset)");
        }

        [CanBeNull]
        public string GetShutterSpeedDescription()
        {
            return GetShutterSpeedDescription(OlympusMakernoteDirectory.TagShutterSpeedValue);
        }

        [CanBeNull]
        public string GetIsoValueDescription()
        {
            if (!Directory.TryGetRational(OlympusMakernoteDirectory.TagIsoValue, out Rational value))
                return null;

            return Math.Round(Math.Pow(2, value.ToDouble() - 5) * 100, 0).ToString();
        }

        [CanBeNull]
        public string GetApertureValueDescription()
        {
            if (!Directory.TryGetDouble(OlympusMakernoteDirectory.TagApertureValue, out double aperture))
                return null;
            return GetFStopDescription(PhotographicConversions.ApertureToFStop(aperture));
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
            var cameratype = Directory.GetString(OlympusMakernoteDirectory.TagCameraType);

            if(cameratype != null)
            {
                if (!Directory.TryGetInt32(OlympusMakernoteDirectory.TagJpegQuality, out int value))
                    return null;

                if ((cameratype.StartsWith("SX", StringComparison.OrdinalIgnoreCase) && !cameratype.StartsWith("SX151", StringComparison.OrdinalIgnoreCase))
                    || cameratype.StartsWith("D4322", StringComparison.OrdinalIgnoreCase))
                {
                    switch (value)
                    {
                        case 0:
                            return "Standard Quality (Low)";
                        case 1:
                            return "High Quality (Normal)";
                        case 2:
                            return "Super High Quality (Fine)";
                        case 6:
                            return "RAW";
                        default:
                            return "Unknown (" + value + ")";
                    }
                }
                else
                {
                    switch (value)
                    {
                        case 0:
                            return "Standard Quality (Low)";
                        case 1:
                            return "High Quality (Normal)";
                        case 2:
                            return "Super High Quality (Fine)";
                        case 4:
                            return "RAW";
                        case 5:
                            return "Medium-Fine";
                        case 6:
                            return "Small-Fine";
                        case 33:
                            return "Uncompressed";
                        default:
                            return "Unknown (" + value + ")";
                    }
                }
            }
            else
                return GetIndexedDescription(OlympusMakernoteDirectory.TagJpegQuality,
                    1,
                    "Standard Quality", "High Quality", "Super High Quality");
        }

        [CanBeNull]
        public string GetSpecialModeDescription()
        {
            var values = Directory.GetObject(OlympusMakernoteDirectory.TagSpecialMode) as uint[];
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
                    // TODO return at this point, as future values are not likely to be any good
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
