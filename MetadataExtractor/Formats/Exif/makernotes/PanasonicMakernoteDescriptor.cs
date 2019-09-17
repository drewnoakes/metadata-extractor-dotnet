// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
        public PanasonicMakernoteDescriptor(PanasonicMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PanasonicMakernoteDirectory.TagQualityMode => GetQualityModeDescription(),
                PanasonicMakernoteDirectory.TagFirmwareVersion => GetVersionDescription(),
                PanasonicMakernoteDirectory.TagWhiteBalance => GetWhiteBalanceDescription(),
                PanasonicMakernoteDirectory.TagFocusMode => GetFocusModeDescription(),
                PanasonicMakernoteDirectory.TagAfAreaMode => GetAfAreaModeDescription(),
                PanasonicMakernoteDirectory.TagImageStabilization => GetImageStabilizationDescription(),
                PanasonicMakernoteDirectory.TagMacroMode => GetMacroModeDescription(),
                PanasonicMakernoteDirectory.TagRecordMode => GetRecordModeDescription(),
                PanasonicMakernoteDirectory.TagAudio => GetAudioDescription(),
                PanasonicMakernoteDirectory.TagUnknownDataDump => GetUnknownDataDumpDescription(),
                PanasonicMakernoteDirectory.TagColorEffect => GetColorEffectDescription(),
                PanasonicMakernoteDirectory.TagUptime => GetUptimeDescription(),
                PanasonicMakernoteDirectory.TagBurstMode => GetBurstModeDescription(),
                PanasonicMakernoteDirectory.TagContrastMode => GetContrastModeDescription(),
                PanasonicMakernoteDirectory.TagNoiseReduction => GetNoiseReductionDescription(),
                PanasonicMakernoteDirectory.TagSelfTimer => GetSelfTimerDescription(),
                PanasonicMakernoteDirectory.TagRotation => GetRotationDescription(),
                PanasonicMakernoteDirectory.TagAfAssistLamp => GetAfAssistLampDescription(),
                PanasonicMakernoteDirectory.TagColorMode => GetColorModeDescription(),
                PanasonicMakernoteDirectory.TagOpticalZoomMode => GetOpticalZoomModeDescription(),
                PanasonicMakernoteDirectory.TagConversionLens => GetConversionLensDescription(),
                PanasonicMakernoteDirectory.TagContrast => GetContrastDescription(),
                PanasonicMakernoteDirectory.TagWorldTimeLocation => GetWorldTimeLocationDescription(),
                PanasonicMakernoteDirectory.TagAdvancedSceneMode => GetAdvancedSceneModeDescription(),
                PanasonicMakernoteDirectory.TagFaceDetectionInfo => GetDetectedFacesDescription(),
                PanasonicMakernoteDirectory.TagTransform => GetTransformDescription(),
                PanasonicMakernoteDirectory.TagTransform1 => GetTransform1Description(),
                PanasonicMakernoteDirectory.TagIntelligentExposure => GetIntelligentExposureDescription(),
                PanasonicMakernoteDirectory.TagFlashWarning => GetFlashWarningDescription(),
                PanasonicMakernoteDirectory.TagCountry => GetCountryDescription(),
                PanasonicMakernoteDirectory.TagState => GetStateDescription(),
                PanasonicMakernoteDirectory.TagCity => GetCityDescription(),
                PanasonicMakernoteDirectory.TagLandmark => GetLandmarkDescription(),
                PanasonicMakernoteDirectory.TagIntelligentResolution => GetIntelligentResolutionDescription(),
                PanasonicMakernoteDirectory.TagFaceRecognitionInfo => GetRecognizedFacesDescription(),
                PanasonicMakernoteDirectory.TagSceneMode => GetSceneModeDescription(),
                PanasonicMakernoteDirectory.TagFlashFired => GetFlashFiredDescription(),
                PanasonicMakernoteDirectory.TagTextStamp => GetTextStampDescription(),
                PanasonicMakernoteDirectory.TagTextStamp1 => GetTextStamp1Description(),
                PanasonicMakernoteDirectory.TagTextStamp2 => GetTextStamp2Description(),
                PanasonicMakernoteDirectory.TagTextStamp3 => GetTextStamp3Description(),
                PanasonicMakernoteDirectory.TagMakernoteVersion => GetMakernoteVersionDescription(),
                PanasonicMakernoteDirectory.TagExifVersion => GetExifVersionDescription(),
                PanasonicMakernoteDirectory.TagInternalSerialNumber => GetInternalSerialNumberDescription(),
                PanasonicMakernoteDirectory.TagTitle => GetTitleDescription(),
                PanasonicMakernoteDirectory.TagBracketSettings => GetBracketSettingsDescription(),
                PanasonicMakernoteDirectory.TagFlashCurtain => GetFlashCurtainDescription(),
                PanasonicMakernoteDirectory.TagLongExposureNoiseReduction => GetLongExposureNoiseReductionDescription(),
                PanasonicMakernoteDirectory.TagBabyName => GetBabyNameDescription(),
                PanasonicMakernoteDirectory.TagLocation => GetLocationDescription(),
                PanasonicMakernoteDirectory.TagLensFirmwareVersion => GetLensFirmwareVersionDescription(),
                PanasonicMakernoteDirectory.TagIntelligentDRange => GetIntelligentDRangeDescription(),
                PanasonicMakernoteDirectory.TagClearRetouch => GetClearRetouchDescription(),
                PanasonicMakernoteDirectory.TagPhotoStyle => GetPhotoStyleDescription(),
                PanasonicMakernoteDirectory.TagShadingCompensation => GetShadingCompensationDescription(),

                PanasonicMakernoteDirectory.TagAccelerometerZ => GetAccelerometerZDescription(),
                PanasonicMakernoteDirectory.TagAccelerometerX => GetAccelerometerXDescription(),
                PanasonicMakernoteDirectory.TagAccelerometerY => GetAccelerometerYDescription(),
                PanasonicMakernoteDirectory.TagCameraOrientation => GetCameraOrientationDescription(),
                PanasonicMakernoteDirectory.TagRollAngle => GetRollAngleDescription(),
                PanasonicMakernoteDirectory.TagPitchAngle => GetPitchAngleDescription(),
                PanasonicMakernoteDirectory.TagSweepPanoramaDirection => GetSweepPanoramaDirectionDescription(),
                PanasonicMakernoteDirectory.TagTimerRecording => GetTimerRecordingDescription(),
                PanasonicMakernoteDirectory.TagHDR => GetHDRDescription(),
                PanasonicMakernoteDirectory.TagShutterType => GetShutterTypeDescription(),
                PanasonicMakernoteDirectory.TagTouchAe => GetTouchAeDescription(),

                PanasonicMakernoteDirectory.TagBabyAge => GetBabyAgeDescription(),
                PanasonicMakernoteDirectory.TagBabyAge1 => GetBabyAge1Description(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetTextStampDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp,
                1,
                "Off", "On");
        }

        public string? GetTextStamp1Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp1,
                1,
                "Off", "On");
        }

        public string? GetTextStamp2Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp2,
                1,
                "Off", "On");
        }

        public string? GetTextStamp3Description()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTextStamp3,
                1,
                "Off", "On");
        }

        public string? GetMacroModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagMacroMode,
                1,
                "Off", "On");
        }

        public string? GetFlashFiredDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashFired,
                1,
                "Off", "On");
        }

        public string? GetImageStabilizationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagImageStabilization,
                2,
                "On, Mode 1", "Off", "On, Mode 2");
        }

        public string? GetAudioDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAudio,
                1,
                "Off", "On");
        }

        public string? GetTransformDescription()
        {
            return GetTransformDescription(PanasonicMakernoteDirectory.TagTransform);
        }

        public string? GetTransform1Description()
        {
            return GetTransformDescription(PanasonicMakernoteDirectory.TagTransform1);
        }

        private string? GetTransformDescription(int tag)
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

        public string? GetIntelligentExposureDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentExposure,
                "Off", "Low", "Standard", "High");
        }

        public string? GetFlashWarningDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashWarning,
                "No", "Yes (Flash required but disabled)");
        }

        public string? GetCountryDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagCountry);
        }

        public string? GetStateDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagState);
        }

        public string? GetCityDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagCity);
        }

        public string? GetLandmarkDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagLandmark);
        }

        public string? GetTitleDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagTitle);
        }

        public string? GetBracketSettingsDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagBracketSettings,
                "No Bracket", "3 Images, Sequence 0/-/+", "3 Images, Sequence -/0/+", "5 Images, Sequence 0/-/+",
                "5 Images, Sequence -/0/+", "7 Images, Sequence 0/-/+", "7 Images, Sequence -/0/+");
        }

        public string? GetFlashCurtainDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFlashCurtain,
                "n/a", "1st", "2nd");
        }

        public string? GetLongExposureNoiseReductionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagLongExposureNoiseReduction, 1,
                "Off", "On");
        }

        public string? GetLensFirmwareVersionDescription()
        {
            // lens version has 4 parts separated by periods
            var bytes = Directory.GetByteArray(PanasonicMakernoteDirectory.TagLensFirmwareVersion);
            if (bytes == null)
                return null;

            return string.Join(".", bytes.Select(b => b.ToString()).ToArray());
        }

        public string? GetIntelligentDRangeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentDRange,
                "Off", "Low", "Standard", "High");
        }

        public string? GetClearRetouchDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagClearRetouch,
                    "Off", "On");

        }

        public string? GetPhotoStyleDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagPhotoStyle,
                "Auto", "Standard or Custom", "Vivid", "Natural", "Monochrome", "Scenery", "Portrait");
        }

        public string? GetShadingCompensationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagShadingCompensation,
                "Off", "On");
        }

        public string? GetAccelerometerZDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagAccelerometerZ, out int value))
                return null;

            // positive is acceleration upwards
            return ((short)value).ToString();
        }

        public string? GetAccelerometerXDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagAccelerometerX, out int value))
                return null;

            // positive is acceleration to the left
            return ((short)value).ToString();
        }

        public string? GetAccelerometerYDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagAccelerometerY, out int value))
                return null;

            // positive is acceleration backwards
            return ((short)value).ToString();
        }

        public string? GetCameraOrientationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagCameraOrientation,
                    "Normal", "Rotate CW", "Rotate 180", "Rotate CCW", "Tilt Upwards", "Tile Downwards");
        }

        public string? GetRollAngleDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagRollAngle, out int value))
                return null;

            // converted to degrees of clockwise camera rotation
            return ((short)value/10.0).ToString();
        }

        public string? GetPitchAngleDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagPitchAngle, out int value))
                return null;

            // converted to degrees of upward camera tilt
            return (-(short)value/10.0).ToString();
        }

        public string? GetSweepPanoramaDirectionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSweepPanoramaDirection,
                    "Off", "Left to Right", "Right to Left", "Top to Bottom", "Bottom to Top");
        }

        public string? GetTimerRecordingDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTimerRecording,
                    "Off", "Time Lapse", "Stop-motion Animation");
        }

        public string? GetHDRDescription()
        {
            if (!Directory.TryGetUInt16(PanasonicMakernoteDirectory.TagHDR, out ushort value))
                return null;

            return value switch
            {
                0 => "Off",
                100 => "1 EV",
                200 => "2 EV",
                300 => "3 EV",
                32868 => "1 EV (Auto)",
                32968 => "2 EV (Auto)",
                33068 => "3 EV (Auto)",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetShutterTypeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagShutterType,
                    "Mechanical", "Electronic", "Hybrid");
        }

        public string? GetTouchAeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagTouchAe,
                    "Off", "On");

        }

        public string? GetBabyNameDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagBabyName);
        }

        public string? GetLocationDescription()
        {
            return GetStringFromUtf8Bytes(PanasonicMakernoteDirectory.TagLocation);
        }

        public string? GetIntelligentResolutionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagIntelligentResolution,
                "Off", null, "Auto", "On");
        }

        public string? GetContrastDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagContrast,
                "Normal");
        }

        public string? GetWorldTimeLocationDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagWorldTimeLocation,
                1,
                "Home", "Destination");
        }

        public string? GetAdvancedSceneModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAdvancedSceneMode,
                1,
                "Normal", "Outdoor/Illuminations/Flower/HDR Art", "Indoor/Architecture/Objects/HDR B&W", "Creative", "Auto", null, "Expressive", "Retro", "Pure", "Elegant", null, "Monochrome", "Dynamic Art", "Silhouette");
        }

        public string? GetUnknownDataDumpDescription()
        {
            return GetByteLengthDescription(PanasonicMakernoteDirectory.TagUnknownDataDump);
        }

        public string? GetColorEffectDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagColorEffect,
                1,
                "Off", "Warm", "Cool", "Black & White", "Sepia");
        }

        public string? GetUptimeDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagUptime, out int value))
                return null;
            return $"{(value / 100f):0.0##}" + " s";
        }

        public string? GetBurstModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagBurstMode,
                "Off", null, "On", "Indefinite", "Unlimited");
        }

        public string? GetContrastModeDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagContrastMode, out int value))
                return null;

            return value switch
            {
                0x0 => "Normal",
                0x1 => "Low",
                0x2 => "High",
                0x6 => "Medium Low",
                0x7 => "Medium High",
                0x100 => "Low",
                0x110 => "Normal",
                0x120 => "High",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetNoiseReductionDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagNoiseReduction,
                "Standard (0)", "Low (-1)", "High (+1)", "Lowest (-2)", "Highest (+2)");
        }

        public string? GetSelfTimerDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSelfTimer,
                1,
                "Off", "10 s", "2 s");
        }

        public string? GetRotationDescription()
        {
            if (!Directory.TryGetInt32(PanasonicMakernoteDirectory.TagRotation, out int value))
                return null;

            return value switch
            {
                1 => "Horizontal",
                3 => "Rotate 180",
                6 => "Rotate 90 CW",
                8 => "Rotate 270 CW",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetAfAssistLampDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagAfAssistLamp,
                1,
                "Fired", "Enabled but not used", "Disabled but required", "Disabled and not required");
        }

        public string? GetColorModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagColorMode,
                "Normal", "Natural", "Vivid");
        }

        public string? GetOpticalZoomModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagOpticalZoomMode,
                1,
                "Standard", "Extended");
        }

        public string? GetConversionLensDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagConversionLens,
                1,
                "Off", "Wide", "Telephoto", "Macro");
        }

        public string? GetDetectedFacesDescription()
        {
            return BuildFacesDescription(Directory.GetDetectedFaces());
        }

        public string? GetRecognizedFacesDescription()
        {
            return BuildFacesDescription(Directory.GetRecognizedFaces());
        }

        private static string? BuildFacesDescription(IEnumerable<Face>? faces)
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

        private static readonly string?[] _sceneModes =
        {
            "Normal", "Portrait", "Scenery", "Sports", "Night Portrait", "Program", "Aperture Priority", "Shutter Priority", "Macro", "Spot", "Manual", "Movie Preview", "Panning", "Simple", "Color Effects",
            "Self Portrait", "Economy", "Fireworks", "Party", "Snow", "Night Scenery", "Food", "Baby", "Soft Skin", "Candlelight", "Starry Night", "High Sensitivity", "Panorama Assist", "Underwater", "Beach",
            "Aerial Photo", "Sunset", "Pet", "Intelligent ISO", "Clipboard", "High Speed Continuous Shooting", "Intelligent Auto", null, "Multi-aspect", null, "Transform", "Flash Burst", "Pin Hole", "Film Grain", "My Color", "Photo Frame", null, null, null, null, "HDR"
        };

        public string? GetRecordModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagRecordMode, 1, _sceneModes);
        }

        public string? GetSceneModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagSceneMode, 1, _sceneModes);
        }

        public string? GetFocusModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagFocusMode, 1,
                "Auto", "Manual", null, "Auto, Focus Button", "Auto, Continuous");
        }

        public string? GetAfAreaModeDescription()
        {
            var value = Directory.GetInt32Array(PanasonicMakernoteDirectory.TagAfAreaMode);
            if (value == null || value.Length < 2)
                return null;

            switch (value[0])
            {
                case 0:
                {
                        return (value[1]) switch
                        {
                            1 => "Spot Mode On",
                            16 => "Spot Mode Off",
                            _ => "Unknown (" + value[0] + " " + value[1] + ")",
                        };
                    }
                case 1:
                {
                        return (value[1]) switch
                        {
                            0 => "Spot Focusing",
                            1 => "5-area",
                            _ => "Unknown (" + value[0] + " " + value[1] + ")",
                        };
                    }
                case 16:
                {
                        return (value[1]) switch
                        {
                            0 => "1-area",
                            16 => "1-area (high speed)",
                            _ => "Unknown (" + value[0] + " " + value[1] + ")",
                        };
                    }
                case 32:
                {
                        return (value[1]) switch
                        {
                            0 => "Auto or Face Detect",
                            1 => "3-area (left)",
                            2 => "3-area (center)",
                            3 => "3-area (right)",
                            _ => "Unknown (" + value[0] + " " + value[1] + ")",
                        };
                    }
                case 64:
                    return "Face Detect";
                default:
                    return "Unknown (" + value[0] + " " + value[1] + ")";
            }
        }

        public string? GetQualityModeDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagQualityMode,
                2,
                "High", "Normal", null, null, "Very High", "Raw", null, "Motion Picture");
        }

        public string? GetVersionDescription()
        {
            return GetVersionBytesDescription(PanasonicMakernoteDirectory.TagFirmwareVersion, 2);
        }

        public string? GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(PanasonicMakernoteDirectory.TagMakernoteVersion, 2);
        }

        public string? GetExifVersionDescription()
        {
            return GetVersionBytesDescription(PanasonicMakernoteDirectory.TagExifVersion, 2);
        }

        public string? GetInternalSerialNumberDescription()
        {
            return GetStringFrom7BitBytes(PanasonicMakernoteDirectory.TagInternalSerialNumber);
        }

        public string? GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(PanasonicMakernoteDirectory.TagWhiteBalance,
                1,
                "Auto", "Daylight", "Cloudy", "Incandescent", "Manual", null, null, "Flash", null, "Black & White", "Manual", "Shade");
        }

        public string? GetBabyAgeDescription()
        {
            return Directory.GetAge(PanasonicMakernoteDirectory.TagBabyAge)?.ToFriendlyString();
        }

        public string? GetBabyAge1Description()
        {
            return Directory.GetAge(PanasonicMakernoteDirectory.TagBabyAge1)?.ToFriendlyString();
        }
    }
}
