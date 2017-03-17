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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some information about this makernote taken from here:
    /// <list type="bullet">
    ///   <item><a href="http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html">http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html</a></item>
    ///   <item><a href="http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html">http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html</a></item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Philipp Sandhaus</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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
                    return GetQualityModeDescription();
                case PanasonicMakernoteDirectory.TagFirmwareVersion:
                    return GetVersionDescription();
                case PanasonicMakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case PanasonicMakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case PanasonicMakernoteDirectory.TagAfAreaMode:
                    return GetAfAreaModeDescription();
                case PanasonicMakernoteDirectory.TagImageStabilization:
                    return GetImageStabilizationDescription();
                case PanasonicMakernoteDirectory.TagMacroMode:
                    return GetMacroModeDescription();
                case PanasonicMakernoteDirectory.TagRecordMode:
                    return GetRecordModeDescription();
                case PanasonicMakernoteDirectory.TagAudio:
                    return GetAudioDescription();
                case PanasonicMakernoteDirectory.TagUnknownDataDump:
                    return GetUnknownDataDumpDescription();
                case PanasonicMakernoteDirectory.TagColorEffect:
                    return GetColorEffectDescription();
                case PanasonicMakernoteDirectory.TagUptime:
                    return GetUptimeDescription();
                case PanasonicMakernoteDirectory.TagBurstMode:
                    return GetBurstModeDescription();
                case PanasonicMakernoteDirectory.TagContrastMode:
                    return GetContrastModeDescription();
                case PanasonicMakernoteDirectory.TagNoiseReduction:
                    return GetNoiseReductionDescription();
                case PanasonicMakernoteDirectory.TagSelfTimer:
                    return GetSelfTimerDescription();
                case PanasonicMakernoteDirectory.TagRotation:
                    return GetRotationDescription();
                case PanasonicMakernoteDirectory.TagAfAssistLamp:
                    return GetAfAssistLampDescription();
                case PanasonicMakernoteDirectory.TagColorMode:
                    return GetColorModeDescription();
                case PanasonicMakernoteDirectory.TagOpticalZoomMode:
                    return GetOpticalZoomModeDescription();
                case PanasonicMakernoteDirectory.TagConversionLens:
                    return GetConversionLensDescription();
                case PanasonicMakernoteDirectory.TagContrast:
                    return GetContrastDescription();
                case PanasonicMakernoteDirectory.TagWorldTimeLocation:
                    return GetWorldTimeLocationDescription();
                case PanasonicMakernoteDirectory.TagAdvancedSceneMode:
                    return GetAdvancedSceneModeDescription();
                case PanasonicMakernoteDirectory.TagFaceDetectionInfo:
                    return GetDetectedFacesDescription();
                case PanasonicMakernoteDirectory.TagTransform:
                    return GetTransformDescription();
                case PanasonicMakernoteDirectory.TagTransform1:
                    return GetTransform1Description();
                case PanasonicMakernoteDirectory.TagIntelligentExposure:
                    return GetIntelligentExposureDescription();
                case PanasonicMakernoteDirectory.TagFlashWarning:
                    return GetFlashWarningDescription();
                case PanasonicMakernoteDirectory.TagCountry:
                    return GetCountryDescription();
                case PanasonicMakernoteDirectory.TagState:
                    return GetStateDescription();
                case PanasonicMakernoteDirectory.TagCity:
                    return GetCityDescription();
                case PanasonicMakernoteDirectory.TagLandmark:
                    return GetLandmarkDescription();
                case PanasonicMakernoteDirectory.TagIntelligentResolution:
                    return GetIntelligentResolutionDescription();
                case PanasonicMakernoteDirectory.TagFaceRecognitionInfo:
                    return GetRecognizedFacesDescription();
                case PanasonicMakernoteDirectory.TagSceneMode:
                    return GetSceneModeDescription();
                case PanasonicMakernoteDirectory.TagFlashFired:
                    return GetFlashFiredDescription();
                case PanasonicMakernoteDirectory.TagTextStamp:
                    return GetTextStampDescription();
                case PanasonicMakernoteDirectory.TagTextStamp1:
                    return GetTextStamp1Description();
                case PanasonicMakernoteDirectory.TagTextStamp2:
                    return GetTextStamp2Description();
                case PanasonicMakernoteDirectory.TagTextStamp3:
                    return GetTextStamp3Description();
                case PanasonicMakernoteDirectory.TagMakernoteVersion:
                    return GetMakernoteVersionDescription();
                case PanasonicMakernoteDirectory.TagExifVersion:
                    return GetExifVersionDescription();
                case PanasonicMakernoteDirectory.TagInternalSerialNumber:
                    return GetInternalSerialNumberDescription();
                case PanasonicMakernoteDirectory.TagTitle:
                    return GetTitleDescription();
                case PanasonicMakernoteDirectory.TagBracketSettings:
                    return GetBracketSettingsDescription();
                case PanasonicMakernoteDirectory.TagFlashCurtain:
                    return GetFlashCurtainDescription();
                case PanasonicMakernoteDirectory.TagLongExposureNoiseReduction:
                    return GetLongExposureNoiseReductionDescription();
                case PanasonicMakernoteDirectory.TagBabyName:
                    return GetBabyNameDescription();
                case PanasonicMakernoteDirectory.TagLocation:
                    return GetLocationDescription();
                case PanasonicMakernoteDirectory.TagLensFirmwareVersion:
                    return GetLensFirmwareVersionDescription();
                case PanasonicMakernoteDirectory.TagIntelligentDRange:
                    return GetIntelligentDRangeDescription();
                case PanasonicMakernoteDirectory.TagClearRetouch:
                    return GetClearRetouchDescription();
                case PanasonicMakernoteDirectory.TagPhotoStyle:
                    return GetPhotoStyleDescription();
                case PanasonicMakernoteDirectory.TagShadingCompensation:
                    return GetShadingCompensationDescription();

                case PanasonicMakernoteDirectory.TagAccelerometerZ:
                    return GetAccelerometerZDescription();
                case PanasonicMakernoteDirectory.TagAccelerometerX:
                    return GetAccelerometerXDescription();
                case PanasonicMakernoteDirectory.TagAccelerometerY:
                    return GetAccelerometerYDescription();
                case PanasonicMakernoteDirectory.TagCameraOrientation:
                    return GetCameraOrientationDescription();
                case PanasonicMakernoteDirectory.TagRollAngle:
                    return GetRollAngleDescription();
                case PanasonicMakernoteDirectory.TagPitchAngle:
                    return GetPitchAngleDescription();
                case PanasonicMakernoteDirectory.TagSweepPanoramaDirection:
                    return GetSweepPanoramaDirectionDescription();
                case PanasonicMakernoteDirectory.TagTimerRecording:
                    return GetTimerRecordingDescription();
                case PanasonicMakernoteDirectory.TagHDR:
                    return GetHDRDescription();
                case PanasonicMakernoteDirectory.TagShutterType:
                    return GetShutterTypeDescription();
                case PanasonicMakernoteDirectory.TagTouchAe:
                    return GetTouchAeDescription();

                case PanasonicMakernoteDirectory.TagBabyAge:
                    return GetBabyAgeDescription();
                case PanasonicMakernoteDirectory.TagBabyAge1:
                    return GetBabyAge1Description();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetTextStampDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp,
                1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetTextStamp1Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp1,
                1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetTextStamp2Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp2,
                1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetTextStamp3Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp3,
                1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetMacroModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagMacroMode,
                1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetFlashFiredDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashFired,
                1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetImageStabilizationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagImageStabilization,
                2,
                "On, Mode 1", "Off", "On, Mode 2");
        }

        [CanBeNull]
        public string GetAudioDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAudio,
                1,
                "Off", "On");
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
            var values = Directory.GetByteArray(tag);
            if (values == null)
                return null;

            IndexedReader reader = new ByteArrayReader(values);

            try
            {
                int val1 = reader.GetUInt16(0);
                int val2 = reader.GetUInt16(2);
                if (val1 == -1 && val2 == 1)
                    return "Slim Low";
                if (val1 == -3 && val2 == 2)
                    return "Slim High";
                if (val1 == 0 && val2 == 0)
                    return "Off";
                if (val1 == 1 && val2 == 1)
                    return "Stretch Low";
                if (val1 == 3 && val2 == 2)
                    return "Stretch High";

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
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentExposure,
                "Off", "Low", "Standard", "High");
        }

        [CanBeNull]
        public string GetFlashWarningDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashWarning,
                "No", "Yes (Flash required but disabled)");
        }

        [CanBeNull]
        public string GetCountryDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagCountry);
        }

        [CanBeNull]
        public string GetStateDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagState);
        }

        [CanBeNull]
        public string GetCityDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagCity);
        }

        [CanBeNull]
        public string GetLandmarkDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagLandmark);
        }

        [CanBeNull]
        public string GetTitleDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagTitle);
        }

        [CanBeNull]
        public string GetBracketSettingsDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagBracketSettings,
                "No Bracket", "3 Images, Sequence 0/-/+", "3 Images, Sequence -/0/+", "5 Images, Sequence 0/-/+",
                "5 Images, Sequence -/0/+", "7 Images, Sequence 0/-/+", "7 Images, Sequence -/0/+");
        }

        public string GetFlashCurtainDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashCurtain,
                "n/a", "1st", "2nd");
        }

        public string GetLongExposureNoiseReductionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagLongExposureNoiseReduction, 1,
                "Off", "On");
        }

        [CanBeNull]
        public string GetLensFirmwareVersionDescription()
        {
            // lens version has 4 parts separated by periods
            var bytes = Directory.GetByteArray(PanasonicMakernoteDirectory.TagLensFirmwareVersion);
            if (bytes == null)
                return null;

            return string.Join(".", bytes.Select(b => b.ToString()).ToArray());
        }

        public string GetIntelligentDRangeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentDRange,
                "Off", "Low", "Standard", "High");
        }

        public string GetClearRetouchDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagClearRetouch,
                    "Off", "On");

        }

        public string GetPhotoStyleDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagPhotoStyle,
                "Auto", "Standard or Custom", "Vivid", "Natural", "Monochrome", "Scenery", "Portrait");
        }

        public string GetShadingCompensationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagShadingCompensation,
                "Off", "On");
        }

        public string GetAccelerometerZDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagAccelerometerZ, out int value))
                return null;

            // positive is acceleration upwards
            return ((short)value).ToString();
        }

        public string GetAccelerometerXDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagAccelerometerX, out int value))
                return null;

            // positive is acceleration to the left
            return ((short)value).ToString();
        }

        public string GetAccelerometerYDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagAccelerometerY, out int value))
                return null;

            // positive is acceleration backwards
            return ((short)value).ToString();
        }

        public string GetCameraOrientationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagCameraOrientation,
                    "Normal", "Rotate CW", "Rotate 180", "Rotate CCW", "Tilt Upwards", "Tile Downwards");
        }

        public string GetRollAngleDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagRollAngle, out int value))
                return null;

            // converted to degrees of clockwise camera rotation
            return ((short)value/10.0).ToString();
        }

        public string GetPitchAngleDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagPitchAngle, out int value))
                return null;

            // converted to degrees of upward camera tilt
            return (-(short)value/10.0).ToString();
        }

        public string GetSweepPanoramaDirectionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSweepPanoramaDirection,
                    "Off", "Left to Right", "Right to Left", "Top to Bottom", "Bottom to Top");
        }

        public string GetTimerRecordingDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTimerRecording,
                    "Off", "Time Lapse", "Stop-motion Animation");
        }

        public string GetHDRDescription()
        {
            if (!Directory.TryGetUInt16(PanasonicMakernoteDirectory.TagHDR, out ushort value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 100:
                    return "1 EV";
                case 200:
                    return "2 EV";
                case 300:
                    return "3 EV";
                case 32868:
                    return "1 EV (Auto)";
                case 32968:
                    return "2 EV (Auto)";
                case 33068:
                    return "3 EV (Auto)";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        public string GetShutterTypeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagShutterType,
                    "Mechanical", "Electronic", "Hybrid");
        }

        public string GetTouchAeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTouchAe,
                    "Off", "On");

        }

        [CanBeNull]
        public string GetBabyNameDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagBabyName);
        }

        [CanBeNull]
        public string GetLocationDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagLocation);
        }

        [CanBeNull]
        public string GetIntelligentResolutionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentResolution,
                "Off", null, "Auto", "On");
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagContrast,
                "Normal");
        }

        [CanBeNull]
        public string GetWorldTimeLocationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagWorldTimeLocation,
                1,
                "Home", "Destination");
        }

        [CanBeNull]
        public string GetAdvancedSceneModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAdvancedSceneMode,
                1,
                "Normal", "Outdoor/Illuminations/Flower/HDR Art", "Indoor/Architecture/Objects/HDR B&W", "Creative", "Auto", null, "Expressive", "Retro", "Pure", "Elegant", null, "Monochrome", "Dynamic Art", "Silhouette");
        }

        [CanBeNull]
        public string GetUnknownDataDumpDescription()
        {
            return GetByteLengthDescription(PanasonicMakernoteDirectory.TagUnknownDataDump);
        }

        [CanBeNull]
        public string GetColorEffectDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagColorEffect,
                1,
                "Off", "Warm", "Cool", "Black & White", "Sepia");
        }

        [CanBeNull]
        public string GetUptimeDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagUptime, out int value))
                return null;
            return value / 100f + " s";
        }

        [CanBeNull]
        public string GetBurstModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagBurstMode,
                "Off", null, "On", "Indefinite", "Unlimited");
        }

        [CanBeNull]
        public string GetContrastModeDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagContrastMode, out int value))
                return null;

            switch (value)
            {
                case 0x0:
                    return "Normal";
                case 0x1:
                    return "Low";
                case 0x2:
                    return "High";
                case 0x6:
                    return "Medium Low";
                case 0x7:
                    return "Medium High";
                case 0x100:
                    return "Low";
                case 0x110:
                    return "Normal";
                case 0x120:
                    return "High";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetNoiseReductionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagNoiseReduction,
                "Standard (0)", "Low (-1)", "High (+1)", "Lowest (-2)", "Highest (+2)");
        }

        [CanBeNull]
        public string GetSelfTimerDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSelfTimer,
                1,
                "Off", "10 s", "2 s");
        }

        [CanBeNull]
        public string GetRotationDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagRotation, out int value))
                return null;

            switch (value)
            {
                case 1:
                    return "Horizontal";
                case 3:
                    return "Rotate 180";
                case 6:
                    return "Rotate 90 CW";
                case 8:
                    return "Rotate 270 CW";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetAfAssistLampDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAfAssistLamp,
                1,
                "Fired", "Enabled but not used", "Disabled but required", "Disabled and not required");
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagColorMode,
                "Normal", "Natural", "Vivid");
        }

        [CanBeNull]
        public string GetOpticalZoomModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagOpticalZoomMode,
                1,
                "Standard", "Extended");
        }

        [CanBeNull]
        public string GetConversionLensDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagConversionLens,
                1,
                "Off", "Wide", "Telephoto", "Macro");
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
        private static string BuildFacesDescription([CanBeNull] IEnumerable<Face> faces)
        {
            if (faces == null)
                return null;

            var description = string.Join(Environment.NewLine,
                faces.Select((f, i) => $"Face {i + 1}: {f}")
#if NET35
                .ToArray()
#endif
                );

            return description.Length == 0 ? null : description;
        }

        private static readonly string[] _sceneModes =
        {
            "Normal", "Portrait", "Scenery", "Sports", "Night Portrait", "Program", "Aperture Priority", "Shutter Priority", "Macro", "Spot", "Manual", "Movie Preview", "Panning", "Simple", "Color Effects",
            "Self Portrait", "Economy", "Fireworks", "Party", "Snow", "Night Scenery", "Food", "Baby", "Soft Skin", "Candlelight", "Starry Night", "High Sensitivity", "Panorama Assist", "Underwater", "Beach",
            "Aerial Photo", "Sunset", "Pet", "Intelligent ISO", "Clipboard", "High Speed Continuous Shooting", "Intelligent Auto", null, "Multi-aspect", null, "Transform", "Flash Burst", "Pin Hole", "Film Grain", "My Color", "Photo Frame", null, null, null, null, "HDR"
        };

        [CanBeNull]
        public string GetRecordModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagRecordMode, 1, _sceneModes);
        }

        [CanBeNull]
        public string GetSceneModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSceneMode, 1, _sceneModes);
        }

        [CanBeNull]
        public string GetFocusModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFocusMode, 1,
                "Auto", "Manual", null, "Auto, Focus Button", "Auto, Continuous");
        }

        [CanBeNull]
        public string GetAfAreaModeDescription()
        {
            var value = Directory.GetInt32Array(PanasonicMakernoteDirectory.TagAfAreaMode);
            if (value == null || value.Length < 2)
                return null;

            switch (value[0])
            {
                case 0:
                {
                    switch (value[1])
                    {
                        case 1:
                            return "Spot Mode On";
                        case 16:
                            return "Spot Mode Off";
                        default:
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                    }
                }
                case 1:
                {
                    switch (value[1])
                    {
                        case 0:
                            return "Spot Focusing";
                        case 1:
                            return "5-area";
                        default:
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                    }
                }
                case 16:
                {
                    switch (value[1])
                    {
                        case 0:
                            return "1-area";
                        case 16:
                            return "1-area (high speed)";
                        default:
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                    }
                }
                case 32:
                {
                    switch (value[1])
                    {
                        case 0:
                            return "Auto or Face Detect";
                        case 1:
                            return "3-area (left)";
                        case 2:
                            return "3-area (center)";
                        case 3:
                            return "3-area (right)";
                        default:
                            return "Unknown (" + value[0] + " " + value[1] + ")";
                    }
                }
                case 64:
                    return "Face Detect";
                default:
                    return "Unknown (" + value[0] + " " + value[1] + ")";
            }
        }

        [CanBeNull]
        public string GetQualityModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagQualityMode,
                2,
                "High", "Normal", null, null, "Very High", "Raw", null, "Motion Picture");
        }

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
            return GetStringFrom7BitBytes(PanasonicMakernoteDirectory.TagInternalSerialNumber);
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagWhiteBalance,
                1,
                "Auto", "Daylight", "Cloudy", "Incandescent", "Manual", null, null, "Flash", null, "Black & White", "Manual", "Shade");
        }

        [CanBeNull]
        public string GetBabyAgeDescription()
        {
            return Directory.GetAge(PanasonicMakernoteDirectory.TagBabyAge)?.ToFriendlyString();
        }

        [CanBeNull]
        public string GetBabyAge1Description()
        {
            return Directory.GetAge(PanasonicMakernoteDirectory.TagBabyAge1)?.ToFriendlyString();
        }
    }
}
