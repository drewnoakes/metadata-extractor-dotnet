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

using System.IO;
using System.Text;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicMakernoteDirectory"/>.
    /// <para>
    /// Some information about this makernote taken from here:
    /// <list type="bullet">
    /// <item><a href="http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html">http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html</a></item>
    /// <item><a href="http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html">http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html</a></item>
    /// </list>
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Philipp Sandhaus</author>
    public sealed class PanasonicMakernoteDescriptor : TagDescriptor<PanasonicMakernoteDirectory>
    {
        public PanasonicMakernoteDescriptor([NotNull] PanasonicMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PanasonicMakernoteDirectory.TagQualityMode:
                {
                    return GetQualityModeDescription();
                }

                case PanasonicMakernoteDirectory.TagFirmwareVersion:
                {
                    return GetVersionDescription();
                }

                case PanasonicMakernoteDirectory.TagWhiteBalance:
                {
                    return GetWhiteBalanceDescription();
                }

                case PanasonicMakernoteDirectory.TagFocusMode:
                {
                    return GetFocusModeDescription();
                }

                case PanasonicMakernoteDirectory.TagAfAreaMode:
                {
                    return GetAfAreaModeDescription();
                }

                case PanasonicMakernoteDirectory.TagImageStabilization:
                {
                    return GetImageStabilizationDescription();
                }

                case PanasonicMakernoteDirectory.TagMacroMode:
                {
                    return GetMacroModeDescription();
                }

                case PanasonicMakernoteDirectory.TagRecordMode:
                {
                    return GetRecordModeDescription();
                }

                case PanasonicMakernoteDirectory.TagAudio:
                {
                    return GetAudioDescription();
                }

                case PanasonicMakernoteDirectory.TagUnknownDataDump:
                {
                    return GetUnknownDataDumpDescription();
                }

                case PanasonicMakernoteDirectory.TagColorEffect:
                {
                    return GetColorEffectDescription();
                }

                case PanasonicMakernoteDirectory.TagUptime:
                {
                    return GetUptimeDescription();
                }

                case PanasonicMakernoteDirectory.TagBurstMode:
                {
                    return GetBurstModeDescription();
                }

                case PanasonicMakernoteDirectory.TagContrastMode:
                {
                    return GetContrastModeDescription();
                }

                case PanasonicMakernoteDirectory.TagNoiseReduction:
                {
                    return GetNoiseReductionDescription();
                }

                case PanasonicMakernoteDirectory.TagSelfTimer:
                {
                    return GetSelfTimerDescription();
                }

                case PanasonicMakernoteDirectory.TagRotation:
                {
                    return GetRotationDescription();
                }

                case PanasonicMakernoteDirectory.TagAfAssistLamp:
                {
                    return GetAfAssistLampDescription();
                }

                case PanasonicMakernoteDirectory.TagColorMode:
                {
                    return GetColorModeDescription();
                }

                case PanasonicMakernoteDirectory.TagOpticalZoomMode:
                {
                    return GetOpticalZoomModeDescription();
                }

                case PanasonicMakernoteDirectory.TagConversionLens:
                {
                    return GetConversionLensDescription();
                }

                case PanasonicMakernoteDirectory.TagContrast:
                {
                    return GetContrastDescription();
                }

                case PanasonicMakernoteDirectory.TagWorldTimeLocation:
                {
                    return GetWorldTimeLocationDescription();
                }

                case PanasonicMakernoteDirectory.TagAdvancedSceneMode:
                {
                    return GetAdvancedSceneModeDescription();
                }

                case PanasonicMakernoteDirectory.TagFaceDetectionInfo:
                {
                    return GetDetectedFacesDescription();
                }

                case PanasonicMakernoteDirectory.TagTransform:
                {
                    return GetTransformDescription();
                }

                case PanasonicMakernoteDirectory.TagTransform1:
                {
                    return GetTransform1Description();
                }

                case PanasonicMakernoteDirectory.TagIntelligentExposure:
                {
                    return GetIntelligentExposureDescription();
                }

                case PanasonicMakernoteDirectory.TagFlashWarning:
                {
                    return GetFlashWarningDescription();
                }

                case PanasonicMakernoteDirectory.TagCountry:
                {
                    return GetCountryDescription();
                }

                case PanasonicMakernoteDirectory.TagState:
                {
                    return GetStateDescription();
                }

                case PanasonicMakernoteDirectory.TagCity:
                {
                    return GetCityDescription();
                }

                case PanasonicMakernoteDirectory.TagLandmark:
                {
                    return GetLandmarkDescription();
                }

                case PanasonicMakernoteDirectory.TagIntelligentResolution:
                {
                    return GetIntelligentResolutionDescription();
                }

                case PanasonicMakernoteDirectory.TagFaceRecognitionInfo:
                {
                    return GetRecognizedFacesDescription();
                }

                case PanasonicMakernoteDirectory.TagPrintImageMatchingInfo:
                {
                    return GetPrintImageMatchingInfoDescription();
                }

                case PanasonicMakernoteDirectory.TagSceneMode:
                {
                    return GetSceneModeDescription();
                }

                case PanasonicMakernoteDirectory.TagFlashFired:
                {
                    return GetFlashFiredDescription();
                }

                case PanasonicMakernoteDirectory.TagTextStamp:
                {
                    return GetTextStampDescription();
                }

                case PanasonicMakernoteDirectory.TagTextStamp1:
                {
                    return GetTextStamp1Description();
                }

                case PanasonicMakernoteDirectory.TagTextStamp2:
                {
                    return GetTextStamp2Description();
                }

                case PanasonicMakernoteDirectory.TagTextStamp3:
                {
                    return GetTextStamp3Description();
                }

                case PanasonicMakernoteDirectory.TagMakernoteVersion:
                {
                    return GetMakernoteVersionDescription();
                }

                case PanasonicMakernoteDirectory.TagExifVersion:
                {
                    return GetExifVersionDescription();
                }

                case PanasonicMakernoteDirectory.TagInternalSerialNumber:
                {
                    return GetInternalSerialNumberDescription();
                }

                case PanasonicMakernoteDirectory.TagTitle:
                {
                    return GetTitleDescription();
                }

                case PanasonicMakernoteDirectory.TagBabyName:
                {
                    return GetBabyNameDescription();
                }

                case PanasonicMakernoteDirectory.TagLocation:
                {
                    return GetLocationDescription();
                }

                case PanasonicMakernoteDirectory.TagBabyAge:
                {
                    return GetBabyAgeDescription();
                }

                case PanasonicMakernoteDirectory.TagBabyAge1:
                {
                    return GetBabyAge1Description();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetPrintImageMatchingInfoDescription()
        {
            return GetByteLengthDescription(PanasonicMakernoteDirectory.TagPrintImageMatchingInfo);
        }

        [CanBeNull]
        public string GetTextStampDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetTextStamp1Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp1, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetTextStamp2Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp2, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetTextStamp3Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp3, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetMacroModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagMacroMode, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetFlashFiredDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashFired, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetImageStabilizationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagImageStabilization, 2, "On, Mode 1", "Off", "On, Mode 2");
        }

        [CanBeNull]
        public string GetAudioDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAudio, 1, "Off", "On");
        }

        [CanBeNull]
        public string GetTransformDescription()
        {
            return GetTransformDescription(PanasonicMakernoteDirectory.TagTransform);
        }

        [CanBeNull]
        public string GetTransform1Description()
        {
            return GetTransformDescription(PanasonicMakernoteDirectory.TagTransform1);
        }

        [CanBeNull]
        private string GetTransformDescription(int tag)
        {
            byte[] values = Directory.GetByteArray(tag);
            if (values == null)
            {
                return null;
            }
            RandomAccessReader reader = new ByteArrayReader(values);
            try
            {
                int val1 = reader.GetUInt16(0);
                int val2 = reader.GetUInt16(2);
                if (val1 == -1 && val2 == 1)
                {
                    return "Slim Low";
                }
                if (val1 == -3 && val2 == 2)
                {
                    return "Slim High";
                }
                if (val1 == 0 && val2 == 0)
                {
                    return "Off";
                }
                if (val1 == 1 && val2 == 1)
                {
                    return "Stretch Low";
                }
                if (val1 == 3 && val2 == 2)
                {
                    return "Stretch High";
                }
                return "Unknown (" + val1 + " " + val2 + ")";
            }
            catch (IOException)
            {
                return null;
            }
        }

        [CanBeNull]
        public string GetIntelligentExposureDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentExposure, "Off", "Low", "Standard", "High");
        }

        [CanBeNull]
        public string GetFlashWarningDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashWarning, "No", "Yes (Flash required but disabled)");
        }

        [CanBeNull]
        public string GetCountryDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagCountry);
        }

        [CanBeNull]
        public string GetStateDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagState);
        }

        [CanBeNull]
        public string GetCityDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagCity);
        }

        [CanBeNull]
        public string GetLandmarkDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagLandmark);
        }

        [CanBeNull]
        public string GetTitleDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagTitle);
        }

        [CanBeNull]
        public string GetBabyNameDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagBabyName);
        }

        [CanBeNull]
        public string GetLocationDescription()
        {
            return GetAsciiStringFromBytes(PanasonicMakernoteDirectory.TagLocation);
        }

        [CanBeNull]
        public string GetIntelligentResolutionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentResolution, "Off", null, "Auto", "On");
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagContrast, "Normal");
        }

        [CanBeNull]
        public string GetWorldTimeLocationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagWorldTimeLocation, 1, "Home", "Destination");
        }

        [CanBeNull]
        public string GetAdvancedSceneModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAdvancedSceneMode, 1, "Normal", "Outdoor/Illuminations/Flower/HDR Art", "Indoor/Architecture/Objects/HDR B&W", "Creative", "Auto", null, "Expressive", "Retro", "Pure", "Elegant", null
                , "Monochrome", "Dynamic Art", "Silhouette");
        }

        [CanBeNull]
        public string GetUnknownDataDumpDescription()
        {
            return GetByteLengthDescription(PanasonicMakernoteDirectory.TagUnknownDataDump);
        }

        [CanBeNull]
        public string GetColorEffectDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagColorEffect, 1, "Off", "Warm", "Cool", "Black & White", "Sepia");
        }

        [CanBeNull]
        public string GetUptimeDescription()
        {
            int? value = Directory.GetInteger(PanasonicMakernoteDirectory.TagUptime);
            if (value == null)
            {
                return null;
            }
            return value / 100f + " s";
        }

        [CanBeNull]
        public string GetBurstModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagBurstMode, "Off", null, "On", "Indefinite", "Unlimited");
        }

        [CanBeNull]
        public string GetContrastModeDescription()
        {
            int? value = Directory.GetInteger(PanasonicMakernoteDirectory.TagContrastMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked(0x0):
                {
                    return "Normal";
                }

                case unchecked(0x1):
                {
                    return "Low";
                }

                case unchecked(0x2):
                {
                    return "High";
                }

                case unchecked(0x6):
                {
                    return "Medium Low";
                }

                case unchecked(0x7):
                {
                    return "Medium High";
                }

                case unchecked(0x100):
                {
                    return "Low";
                }

                case unchecked(0x110):
                {
                    return "Normal";
                }

                case unchecked(0x120):
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
        public string GetNoiseReductionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagNoiseReduction, "Standard (0)", "Low (-1)", "High (+1)", "Lowest (-2)", "Highest (+2)");
        }

        [CanBeNull]
        public string GetSelfTimerDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSelfTimer, 1, "Off", "10 s", "2 s");
        }

        [CanBeNull]
        public string GetRotationDescription()
        {
            int? value = Directory.GetInteger(PanasonicMakernoteDirectory.TagRotation);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 1:
                {
                    return "Horizontal";
                }

                case 3:
                {
                    return "Rotate 180";
                }

                case 6:
                {
                    return "Rotate 90 CW";
                }

                case 8:
                {
                    return "Rotate 270 CW";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetAfAssistLampDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAfAssistLamp, 1, "Fired", "Enabled but not used", "Disabled but required", "Disabled and not required");
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagColorMode, "Normal", "Natural", "Vivid");
        }

        [CanBeNull]
        public string GetOpticalZoomModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagOpticalZoomMode, 1, "Standard", "Extended");
        }

        [CanBeNull]
        public string GetConversionLensDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagConversionLens, 1, "Off", "Wide", "Telephoto", "Macro");
        }

        [CanBeNull]
        public string GetDetectedFacesDescription()
        {
            return BuildFacesDescription(Directory.GetDetectedFaces());
        }

        [CanBeNull]
        public string GetRecognizedFacesDescription()
        {
            return BuildFacesDescription(Directory.GetRecognizedFaces());
        }

        [CanBeNull]
        private static string BuildFacesDescription([CanBeNull] Face[] faces)
        {
            if (faces == null)
            {
                return null;
            }
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < faces.Length; i++)
            {
                result.Append("Face ").Append(i + 1).Append(": ").Append(Extensions.ConvertToString(faces[i])).Append("\n");
            }
            return result.Length > 0 ? result.Substring(0, result.Length - 1) : null;
        }

        private static readonly string[] SceneModes = new string[] { "Normal", "Portrait", "Scenery", "Sports", "Night Portrait", "Program", "Aperture Priority", "Shutter Priority", "Macro", "Spot", "Manual", "Movie Preview", "Panning", "Simple", "Color Effects"
            , "Self Portrait", "Economy", "Fireworks", "Party", "Snow", "Night Scenery", "Food", "Baby", "Soft Skin", "Candlelight", "Starry Night", "High Sensitivity", "Panorama Assist", "Underwater", "Beach", "Aerial Photo", "Sunset", "Pet", "Intelligent ISO"
            , "Clipboard", "High Speed Continuous Shooting", "Intelligent Auto", null, "Multi-aspect", null, "Transform", "Flash Burst", "Pin Hole", "Film Grain", "My Color", "Photo Frame", null, null, null, null, "HDR" };

        // 1
        // 10
        // 20
        // 30
        // 40
        // 50
        [CanBeNull]
        public string GetRecordModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagRecordMode, 1, SceneModes);
        }

        [CanBeNull]
        public string GetSceneModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSceneMode, 1, SceneModes);
        }

        [CanBeNull]
        public string GetFocusModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFocusMode, 1, "Auto", "Manual", null, "Auto, Focus Button", "Auto, Continuous");
        }

        [CanBeNull]
        public string GetAfAreaModeDescription()
        {
            int[] value = Directory.GetIntArray(PanasonicMakernoteDirectory.TagAfAreaMode);
            if (value == null || value.Length < 2)
            {
                return null;
            }
            switch (value[0])
            {
                case 0:
                {
                    switch (value[1])
                    {
                        case 1:
                        {
                            return "Spot Mode On";
                        }

                        case 16:
                        {
                            return "Spot Mode Off";
                        }

                        default:
                        {
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                        }
                    }
                    goto case 1;
                }

                case 1:
                {
                    switch (value[1])
                    {
                        case 0:
                        {
                            return "Spot Focusing";
                        }

                        case 1:
                        {
                            return "5-area";
                        }

                        default:
                        {
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                        }
                    }
                    goto case 16;
                }

                case 16:
                {
                    switch (value[1])
                    {
                        case 0:
                        {
                            return "1-area";
                        }

                        case 16:
                        {
                            return "1-area (high speed)";
                        }

                        default:
                        {
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                        }
                    }
                    goto case 32;
                }

                case 32:
                {
                    switch (value[1])
                    {
                        case 0:
                        {
                            return "Auto or Face Detect";
                        }

                        case 1:
                        {
                            return "3-area (left)";
                        }

                        case 2:
                        {
                            return "3-area (center)";
                        }

                        case 3:
                        {
                            return "3-area (right)";
                        }

                        default:
                        {
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                        }
                    }
                    goto case 64;
                }

                case 64:
                {
                    return "Face Detect";
                }

                default:
                {
                    return "Unknown (" + value[0] + " " + value[1] + ")";
                }
            }
        }

        [CanBeNull]
        public string GetQualityModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagQualityMode, 2, "High", "Normal", null, null, "Very High", "Raw", null, "Motion Picture");
        }

        // 2
        // 9
        [CanBeNull]
        public string GetVersionDescription()
        {
            return GetVersionBytesDescription(PanasonicMakernoteDirectory.TagFirmwareVersion, 2);
        }

        [CanBeNull]
        public string GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(PanasonicMakernoteDirectory.TagMakernoteVersion, 2);
        }

        [CanBeNull]
        public string GetExifVersionDescription()
        {
            return GetVersionBytesDescription(PanasonicMakernoteDirectory.TagExifVersion, 2);
        }

        [CanBeNull]
        public string GetInternalSerialNumberDescription()
        {
            return Get7BitStringFromBytes(PanasonicMakernoteDirectory.TagInternalSerialNumber);
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagWhiteBalance, 1, "Auto", "Daylight", "Cloudy", "Incandescent", "Manual", null, null, "Flash", null, "Black & White", "Manual", "Shade");
        }

        // 1
        // 10
        // 12
        [CanBeNull]
        public string GetBabyAgeDescription()
        {
            Age age = Directory.GetAge(PanasonicMakernoteDirectory.TagBabyAge);
            return age == null ? null : age.ToFriendlyString();
        }

        [CanBeNull]
        public string GetBabyAge1Description()
        {
            Age age = Directory.GetAge(PanasonicMakernoteDirectory.TagBabyAge1);
            return age == null ? null : age.ToFriendlyString();
        }
    }
}
