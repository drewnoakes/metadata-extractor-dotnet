/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.Text;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a
    /// <see cref="OlympusMakernoteDirectory"/>
    /// .
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class OlympusMakernoteDescriptor : TagDescriptor<OlympusMakernoteDirectory>
    {
        public OlympusMakernoteDescriptor([NotNull] OlympusMakernoteDirectory directory)
            : base(directory)
        {
        }

        // TODO extend support for some offset-encoded byte[] tags: http://www.ozhiker.com/electronics/pjmt/jpeg_info/olympus_mn.html
        [CanBeNull]
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusMakernoteDirectory.TagMakernoteVersion:
                {
                    return GetMakernoteVersionDescription();
                }

                case OlympusMakernoteDirectory.TagColourMode:
                {
                    return GetColorModeDescription();
                }

                case OlympusMakernoteDirectory.TagImageQuality1:
                {
                    return GetImageQuality1Description();
                }

                case OlympusMakernoteDirectory.TagImageQuality2:
                {
                    return GetImageQuality2Description();
                }

                case OlympusMakernoteDirectory.TagSpecialMode:
                {
                    return GetSpecialModeDescription();
                }

                case OlympusMakernoteDirectory.TagJpegQuality:
                {
                    return GetJpegQualityDescription();
                }

                case OlympusMakernoteDirectory.TagMacroMode:
                {
                    return GetMacroModeDescription();
                }

                case OlympusMakernoteDirectory.TagBwMode:
                {
                    return GetBWModeDescription();
                }

                case OlympusMakernoteDirectory.TagDigiZoomRatio:
                {
                    return GetDigiZoomRatioDescription();
                }

                case OlympusMakernoteDirectory.TagCameraId:
                {
                    return GetCameraIdDescription();
                }

                case OlympusMakernoteDirectory.TagFlashMode:
                {
                    return GetFlashModeDescription();
                }

                case OlympusMakernoteDirectory.TagFocusRange:
                {
                    return GetFocusRangeDescription();
                }

                case OlympusMakernoteDirectory.TagFocusMode:
                {
                    return GetFocusModeDescription();
                }

                case OlympusMakernoteDirectory.TagSharpness:
                {
                    return GetSharpnessDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagExposureMode:
                {
                    return GetExposureModeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFlashMode:
                {
                    return GetFlashModeCameraSettingDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalance:
                {
                    return GetWhiteBalanceDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagImageSize:
                {
                    return GetImageSizeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagImageQuality:
                {
                    return GetImageQualityDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagShootingMode:
                {
                    return GetShootingModeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagMeteringMode:
                {
                    return GetMeteringModeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagApexFilmSpeedValue:
                {
                    return GetApexFilmSpeedDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagApexShutterSpeedTimeValue:
                {
                    return GetApexShutterSpeedTimeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagApexApertureValue:
                {
                    return GetApexApertureDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagMacroMode:
                {
                    return GetMacroModeCameraSettingDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagDigitalZoom:
                {
                    return GetDigitalZoomDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagExposureCompensation:
                {
                    return GetExposureCompensationDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagBracketStep:
                {
                    return GetBracketStepDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagIntervalLength:
                {
                    return GetIntervalLengthDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagIntervalNumber:
                {
                    return GetIntervalNumberDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFocalLength:
                {
                    return GetFocalLengthDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFocusDistance:
                {
                    return GetFocusDistanceDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFlashFired:
                {
                    return GetFlastFiredDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagDate:
                {
                    return GetDateDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagTime:
                {
                    return GetTimeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagMaxApertureAtFocalLength:
                {
                    return GetMaxApertureAtFocalLengthDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFileNumberMemory:
                {
                    return GetFileNumberMemoryDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagLastFileNumber:
                {
                    return GetLastFileNumberDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceRed:
                {
                    return GetWhiteBalanceRedDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceGreen:
                {
                    return GetWhiteBalanceGreenDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceBlue:
                {
                    return GetWhiteBalanceBlueDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagSaturation:
                {
                    return GetSaturationDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagContrast:
                {
                    return GetContrastDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagSharpness:
                {
                    return GetSharpnessCameraSettingDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagSubjectProgram:
                {
                    return GetSubjectProgramDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFlashCompensation:
                {
                    return GetFlastCompensationDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagIsoSetting:
                {
                    return GetIsoSettingDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagCameraModel:
                {
                    return GetCameraModelDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagIntervalMode:
                {
                    return GetIntervalModeDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFolderName:
                {
                    return GetFolderNameDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagColorMode:
                {
                    return GetColorModeCameraSettingDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagColorFilter:
                {
                    return GetColorFilterDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagBlackAndWhiteFilter:
                {
                    return GetBlackAndWhiteFilterDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagInternalFlash:
                {
                    return GetInternalFlashDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagApexBrightnessValue:
                {
                    return GetApexBrightnessDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointXCoordinate:
                {
                    return GetSpotFocusPointXCoordinateDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointYCoordinate:
                {
                    return GetSpotFocusPointYCoordinateDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagWideFocusZone:
                {
                    return GetWideFocusZoneDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFocusMode:
                {
                    return GetFocusModeCameraSettingDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagFocusArea:
                {
                    return GetFocusAreaDescription();
                }

                case OlympusMakernoteDirectory.CameraSettings.TagDecSwitchPosition:
                {
                    return GetDecSwitchPositionDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public virtual string GetExposureModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagExposureMode, "P", "A", "S", "M");
        }

        [CanBeNull]
        public virtual string GetFlashModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFlashMode, "Normal", "Red-eye reduction", "Rear flash sync", "Wireless");
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalance, "Auto", "Daylight", "Cloudy", "Tungsten", null, "Custom", null, "Fluorescent", "Fluorescent 2", null, null, "Custom 2", "Custom 3");
        }

        // 0
        // 5
        // 10
        [CanBeNull]
        public virtual string GetImageSizeDescription()
        {
            // This is a pretty weird way to store this information!
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagImageSize, "2560 x 1920", "1600 x 1200", "1280 x 960", "640 x 480");
        }

        [CanBeNull]
        public virtual string GetImageQualityDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagImageQuality, "Raw", "Super Fine", "Fine", "Standard", "Economy", "Extra Fine");
        }

        [CanBeNull]
        public virtual string GetShootingModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagShootingMode, "Single", "Continuous", "Self Timer", null, "Bracketing", "Interval", "UHS Continuous", "HS Continuous");
        }

        [CanBeNull]
        public virtual string GetMeteringModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagMeteringMode, "Multi-Segment", "Centre Weighted", "Spot");
        }

        [CanBeNull]
        public virtual string GetApexFilmSpeedDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Speed value = value/8-1 ,
            // ISO = (2^(value/8-1))*3.125
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagApexFilmSpeedValue);
            if (value == null)
            {
                return null;
            }
            double iso = Math.Pow(((double)value / 8d) - 1, 2) * 3.125;
            return Extensions.ConvertToString(iso);
        }

        [CanBeNull]
        public virtual string GetApexShutterSpeedTimeDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Time value = value/8-6 ,
            // ShutterSpeed = 2^( (48-value)/8 ),
            // Due to rounding error value=8 should be displayed as 30 sec.
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagApexShutterSpeedTimeValue);
            if (value == null)
            {
                return null;
            }
            double shutterSpeed = Math.Pow((49 - (long)value) / 8d, 2);
            return Extensions.ConvertToString(shutterSpeed) + " sec";
        }

        [CanBeNull]
        public virtual string GetApexApertureDescription()
        {
            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html#Minolta_Camera_Settings
            // Apex Aperture Value = value/8-1 ,
            // Aperture F-stop = 2^( value/16-0.5 )
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagApexApertureValue);
            if (value == null)
            {
                return null;
            }
            double fStop = Math.Pow(((double)value / 16d) - 0.5, 2);
            return "f/" + fStop.ToString("0.0");
        }

        [CanBeNull]
        public virtual string GetMacroModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagMacroMode, "Off", "On");
        }

        [CanBeNull]
        public virtual string GetDigitalZoomDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagDigitalZoom, "Off", "Electronic magnification", "Digital zoom 2x");
        }

        [CanBeNull]
        public virtual string GetExposureCompensationDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagExposureCompensation);
            return value == null ? null : (((double)value / 3d) - 2) + " EV";
        }

        [CanBeNull]
        public virtual string GetBracketStepDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagBracketStep, "1/3 EV", "2/3 EV", "1 EV");
        }

        [CanBeNull]
        public virtual string GetIntervalLengthDescription()
        {
            if (!_directory.IsIntervalMode())
            {
                return "N/A";
            }
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagIntervalLength);
            return value == null ? null : value + " min";
        }

        [CanBeNull]
        public virtual string GetIntervalNumberDescription()
        {
            if (!_directory.IsIntervalMode())
            {
                return "N/A";
            }
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagIntervalNumber);
            return value == null ? null : Convert.ToString((long)value);
        }

        [CanBeNull]
        public virtual string GetFocalLengthDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagFocalLength);
            return value == null ? null : Extensions.ConvertToString((double)value / 256d) + " mm";
        }

        [CanBeNull]
        public virtual string GetFocusDistanceDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagFocusDistance);
            return value == null ? null : value == 0 ? "Infinity" : value + " mm";
        }

        [CanBeNull]
        public virtual string GetFlastFiredDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFlashFired, "No", "Yes");
        }

        [CanBeNull]
        public virtual string GetDateDescription()
        {
            // day = value%256,
            // month = floor( (value - floor( value/65536 )*65536 )/256 )
            // year = floor( value/65536)
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagDate);
            if (value == null)
            {
                return null;
            }
            long day = (long)value & unchecked((int)(0xFF));
            long month = ((long)value >> 16) & unchecked((int)(0xFF));
            long year = ((long)value >> 8) & unchecked((int)(0xFF));
            return Extensions.ConvertToString(new GregorianCalendar((int)year + 1970, (int)month, (int)day).GetTime());
        }

        [CanBeNull]
        public virtual string GetTimeDescription()
        {
            // hours = floor( value/65536 ),
            // minutes = floor( ( value - floor( value/65536 )*65536 )/256 ),
            // seconds = value%256
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagTime);
            if (value == null)
            {
                return null;
            }
            long hours = ((long)value >> 8) & unchecked((int)(0xFF));
            long minutes = ((long)value >> 16) & unchecked((int)(0xFF));
            long seconds = (long)value & unchecked((int)(0xFF));
            return Extensions.StringFormat("%02d:%02d:%02d", hours, minutes, seconds);
        }

        [CanBeNull]
        public virtual string GetMaxApertureAtFocalLengthDescription()
        {
            // Aperture F-Stop = 2^(value/16-0.5)
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagTime);
            if (value == null)
            {
                return null;
            }
            double fStop = Math.Pow(((double)value / 16d) - 0.5, 2);
            return "f/" + fStop.ToString("0.0");
        }

        [CanBeNull]
        public virtual string GetFileNumberMemoryDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFileNumberMemory, "Off", "On");
        }

        [CanBeNull]
        public virtual string GetLastFileNumberDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagLastFileNumber);
            return value == null ? null : value == 0 ? "File Number Memory Off" : Convert.ToString((long)value);
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceRedDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceRed);
            return value == null ? null : Extensions.ConvertToString((double)value / 256d);
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceGreenDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceGreen);
            return value == null ? null : Extensions.ConvertToString((double)value / 256d);
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceBlueDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagWhiteBalanceBlue);
            return value == null ? null : Extensions.ConvertToString((double)value / 256d);
        }

        [CanBeNull]
        public virtual string GetSaturationDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagSaturation);
            return value == null ? null : Convert.ToString((long)value - 3);
        }

        [CanBeNull]
        public virtual string GetContrastDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagContrast);
            return value == null ? null : Convert.ToString((long)value - 3);
        }

        [CanBeNull]
        public virtual string GetSharpnessCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagSharpness, "Hard", "Normal", "Soft");
        }

        [CanBeNull]
        public virtual string GetSubjectProgramDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagSubjectProgram, "None", "Portrait", "Text", "Night Portrait", "Sunset", "Sports Action");
        }

        [CanBeNull]
        public virtual string GetFlastCompensationDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagFlashCompensation);
            return value == null ? null : (((long)value - 6) / 3d) + " EV";
        }

        [CanBeNull]
        public virtual string GetIsoSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagIsoSetting, "100", "200", "400", "800", "Auto", "64");
        }

        [CanBeNull]
        public virtual string GetCameraModelDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagCameraModel, "DiMAGE 7", "DiMAGE 5", "DiMAGE S304", "DiMAGE S404", "DiMAGE 7i", "DiMAGE 7Hi", "DiMAGE A1", "DiMAGE S414");
        }

        [CanBeNull]
        public virtual string GetIntervalModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagIntervalMode, "Still Image", "Time Lapse Movie");
        }

        [CanBeNull]
        public virtual string GetFolderNameDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFolderName, "Standard Form", "Data Form");
        }

        [CanBeNull]
        public virtual string GetColorModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagColorMode, "Natural Color", "Black & White", "Vivid Color", "Solarization", "AdobeRGB");
        }

        [CanBeNull]
        public virtual string GetColorFilterDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagColorFilter);
            return value == null ? null : Convert.ToString((long)value - 3);
        }

        [CanBeNull]
        public virtual string GetBlackAndWhiteFilterDescription()
        {
            return base.GetDescription(OlympusMakernoteDirectory.CameraSettings.TagBlackAndWhiteFilter);
        }

        [CanBeNull]
        public virtual string GetInternalFlashDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagInternalFlash, "Did Not Fire", "Fired");
        }

        [CanBeNull]
        public virtual string GetApexBrightnessDescription()
        {
            long? value = _directory.GetLongObject(OlympusMakernoteDirectory.CameraSettings.TagApexBrightnessValue);
            return value == null ? null : Extensions.ConvertToString(((double)value / 8d) - 6);
        }

        [CanBeNull]
        public virtual string GetSpotFocusPointXCoordinateDescription()
        {
            return base.GetDescription(OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointXCoordinate);
        }

        [CanBeNull]
        public virtual string GetSpotFocusPointYCoordinateDescription()
        {
            return base.GetDescription(OlympusMakernoteDirectory.CameraSettings.TagSpotFocusPointYCoordinate);
        }

        [CanBeNull]
        public virtual string GetWideFocusZoneDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagWideFocusZone, "No Zone or AF Failed", "Center Zone (Horizontal Orientation)", "Center Zone (Vertical Orientation)", "Left Zone", "Right Zone");
        }

        [CanBeNull]
        public virtual string GetFocusModeCameraSettingDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFocusMode, "Auto Focus", "Manual Focus");
        }

        [CanBeNull]
        public virtual string GetFocusAreaDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagFocusArea, "Wide Focus (Normal)", "Spot Focus");
        }

        [CanBeNull]
        public virtual string GetDecSwitchPositionDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.CameraSettings.TagDecSwitchPosition, "Exposure", "Contrast", "Saturation", "Filter");
        }

        [CanBeNull]
        public virtual string GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(OlympusMakernoteDirectory.TagMakernoteVersion, 2);
        }

        [CanBeNull]
        public virtual string GetImageQuality2Description()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagImageQuality2, "Raw", "Super Fine", "Fine", "Standard", "Extra Fine");
        }

        [CanBeNull]
        public virtual string GetImageQuality1Description()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagImageQuality1, "Raw", "Super Fine", "Fine", "Standard", "Extra Fine");
        }

        [CanBeNull]
        public virtual string GetColorModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagColourMode, "Natural Colour", "Black & White", "Vivid Colour", "Solarization", "AdobeRGB");
        }

        [CanBeNull]
        public virtual string GetSharpnessDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagSharpness, "Normal", "Hard", "Soft");
        }

        [CanBeNull]
        public virtual string GetFocusModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagFocusMode, "Auto", "Manual");
        }

        [CanBeNull]
        public virtual string GetFocusRangeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagFocusRange, "Normal", "Macro");
        }

        [CanBeNull]
        public virtual string GetFlashModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagFlashMode, null, null, "On", "Off");
        }

        [CanBeNull]
        public virtual string GetDigiZoomRatioDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagDigiZoomRatio, "Normal", null, "Digital 2x Zoom");
        }

        [CanBeNull]
        public virtual string GetCameraIdDescription()
        {
            sbyte[] bytes = _directory.GetByteArray(OlympusMakernoteDirectory.TagCameraId);
            if (bytes == null)
            {
                return null;
            }
            return Runtime.GetStringForBytes(bytes);
        }

        [CanBeNull]
        public virtual string GetMacroModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagMacroMode, "Normal (no macro)", "Macro");
        }

        [CanBeNull]
        public virtual string GetBWModeDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagBwMode, "Off", "On");
        }

        [CanBeNull]
        public virtual string GetJpegQualityDescription()
        {
            return GetIndexedDescription(OlympusMakernoteDirectory.TagJpegQuality, 1, "Standard Quality", "High Quality", "Super High Quality");
        }

        [CanBeNull]
        public virtual string GetSpecialModeDescription()
        {
            long[] values = (long[])_directory.GetObject(OlympusMakernoteDirectory.TagSpecialMode);
            if (values == null)
            {
                return null;
            }
            if (values.Length < 1)
            {
                return string.Empty;
            }
            StringBuilder desc = new StringBuilder();
            switch ((int)values[0])
            {
                case 0:
                {
                    desc.Append("Normal picture taking mode");
                    break;
                }

                case 1:
                {
                    desc.Append("Unknown picture taking mode");
                    break;
                }

                case 2:
                {
                    desc.Append("Fast picture taking mode");
                    break;
                }

                case 3:
                {
                    desc.Append("Panorama picture taking mode");
                    break;
                }

                default:
                {
                    desc.Append("Unknown picture taking mode");
                    break;
                }
            }
            if (values.Length >= 2)
            {
                switch ((int)values[1])
                {
                    case 0:
                    {
                        break;
                    }

                    case 1:
                    {
                        desc.Append(" / 1st in a sequence");
                        break;
                    }

                    case 2:
                    {
                        desc.Append(" / 2nd in a sequence");
                        break;
                    }

                    case 3:
                    {
                        desc.Append(" / 3rd in a sequence");
                        break;
                    }

                    default:
                    {
                        desc.Append(" / ");
                        desc.Append(values[1]);
                        desc.Append("th in a sequence");
                        break;
                    }
                }
            }
            if (values.Length >= 3)
            {
                switch ((int)values[2])
                {
                    case 1:
                    {
                        desc.Append(" / Left to right panorama direction");
                        break;
                    }

                    case 2:
                    {
                        desc.Append(" / Right to left panorama direction");
                        break;
                    }

                    case 3:
                    {
                        desc.Append(" / Bottom to top panorama direction");
                        break;
                    }

                    case 4:
                    {
                        desc.Append(" / Top to bottom panorama direction");
                        break;
                    }
                }
            }
            return Extensions.ConvertToString(desc);
        }
    }
}
