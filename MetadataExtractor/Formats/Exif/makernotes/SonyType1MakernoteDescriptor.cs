// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SonyType1MakernoteDirectory"/>.
    /// Thanks to David Carson for the initial version of this class.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class SonyType1MakernoteDescriptor : TagDescriptor<SonyType1MakernoteDirectory>
    {
        public SonyType1MakernoteDescriptor(SonyType1MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                SonyType1MakernoteDirectory.TagImageQuality => GetImageQualityDescription(),
                SonyType1MakernoteDirectory.TagFlashExposureComp => GetFlashExposureCompensationDescription(),
                SonyType1MakernoteDirectory.TagTeleconverter => GetTeleconverterDescription(),
                SonyType1MakernoteDirectory.TagWhiteBalance => GetWhiteBalanceDescription(),
                SonyType1MakernoteDirectory.TagColorTemperature => GetColorTemperatureDescription(),
                SonyType1MakernoteDirectory.TagSceneMode => GetSceneModeDescription(),
                SonyType1MakernoteDirectory.TagZoneMatching => GetZoneMatchingDescription(),
                SonyType1MakernoteDirectory.TagDynamicRangeOptimiser => GetDynamicRangeOptimizerDescription(),
                SonyType1MakernoteDirectory.TagImageStabilisation => GetImageStabilizationDescription(),
                //              case SonyType1MakernoteDirectory.TagLensId:
                //                  // Unfortunately it seems that there is no definite mapping between a lens ID and a lens model
                //                  // http://gvsoft.homedns.org/exif/makernote-sony-type1.html#0xb027
                //                  return getLensIDDescription();
                SonyType1MakernoteDirectory.TagColorMode => GetColorModeDescription(),
                SonyType1MakernoteDirectory.TagMacro => GetMacroDescription(),
                SonyType1MakernoteDirectory.TagExposureMode => GetExposureModeDescription(),
                SonyType1MakernoteDirectory.TagJpegQuality => GetJpegQualityDescription(),
                SonyType1MakernoteDirectory.TagAntiBlur => GetAntiBlurDescription(),
                SonyType1MakernoteDirectory.TagLongExposureNoiseReductionOrFocusMode => GetLongExposureNoiseReductionDescription(),
                SonyType1MakernoteDirectory.TagHighIsoNoiseReduction => GetHighIsoNoiseReductionDescription(),
                SonyType1MakernoteDirectory.TagPictureEffect => GetPictureEffectDescription(),
                SonyType1MakernoteDirectory.TagSoftSkinEffect => GetSoftSkinEffectDescription(),
                SonyType1MakernoteDirectory.TagVignettingCorrection => GetVignettingCorrectionDescription(),
                SonyType1MakernoteDirectory.TagLateralChromaticAberration => GetLateralChromaticAberrationDescription(),
                SonyType1MakernoteDirectory.TagDistortionCorrection => GetDistortionCorrectionDescription(),
                SonyType1MakernoteDirectory.TagAutoPortraitFramed => GetAutoPortraitFramedDescription(),
                SonyType1MakernoteDirectory.TagFocusMode => GetFocusModeDescription(),
                SonyType1MakernoteDirectory.TagAfPointSelected => GetAfPointSelectedDescription(),
                SonyType1MakernoteDirectory.TagSonyModelId => GetSonyModelIdDescription(),
                SonyType1MakernoteDirectory.TagAfMode => GetAfModeDescription(),
                SonyType1MakernoteDirectory.TagAfIlluminator => GetAfIlluminatorDescription(),
                SonyType1MakernoteDirectory.TagFlashLevel => GetFlashLevelDescription(),
                SonyType1MakernoteDirectory.TagReleaseMode => GetReleaseModeDescription(),
                SonyType1MakernoteDirectory.TagSequenceNumber => GetSequenceNumberDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetImageQualityDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagImageQuality,
                "RAW", "Super Fine", "Fine", "Standard", "Economy", "Extra Fine", "RAW + JPEG", "Compressed RAW", "Compressed RAW + JPEG");
        }

        public string? GetFlashExposureCompensationDescription()
        {
            return GetFormattedInt(SonyType1MakernoteDirectory.TagFlashExposureComp,
                "{0} EV");
        }

        public string? GetTeleconverterDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagTeleconverter, out int value))
                return null;

            return value switch
            {
                0x00 => "None",
                0x48 => "Minolta/Sony AF 2x APO (D)",
                0x50 => "Minolta AF 2x APO II",
                0x60 => "Minolta AF 2x APO",
                0x88 => "Minolta/Sony AF 1.4x APO (D)",
                0x90 => "Minolta AF 1.4x APO II",
                0xa0 => "Minolta AF 1.4x APO",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetWhiteBalanceDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagWhiteBalance, out int value))
                return null;

            return value switch
            {
                0x00 => "Auto",
                0x01 => "Color Temperature/Color Filter",
                0x10 => "Daylight",
                0x20 => "Cloudy",
                0x30 => "Shade",
                0x40 => "Tungsten",
                0x50 => "Flash",
                0x60 => "Fluorescent",
                0x70 => "Custom",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetColorTemperatureDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagColorTemperature, out int value))
                return null;
            if (value == 0)
                return "Auto";
            var kelvin = ((value & 0x00FF0000) >> 8) | ((value & unchecked((int)0xFF000000)) >> 24);
            return $"{kelvin} K";
        }

        public string? GetZoneMatchingDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagZoneMatching,
                "ISO Setting Used", "High Key", "Low Key");
        }

        public string? GetDynamicRangeOptimizerDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagDynamicRangeOptimiser, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "Standard",
                2 => "Advanced Auto",
                3 => "Auto",
                8 => "Advanced LV1",
                9 => "Advanced LV2",
                10 => "Advanced LV3",
                11 => "Advanced LV4",
                12 => "Advanced LV5",
                16 => "LV1",
                17 => "LV2",
                18 => "LV3",
                19 => "LV4",
                20 => "LV5",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetImageStabilizationDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagImageStabilisation, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "On",
                _ => "N/A",
            };
        }

        public string? GetColorModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagColorMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Standard";
                case 1:
                    return "Vivid";
                case 2:
                    return "Portrait";
                case 3:
                    return "Landscape";
                case 4:
                    return "Sunset";
                case 5:
                    return "Night Portrait";
                case 6:
                    return "Black & White";
                case 7:
                    return "Adobe RGB";
                case 12:
                case 100:
                    return "Neutral";
                case 13:
                case 101:
                    return "Clear";
                case 14:
                case 102:
                    return "Deep";
                case 15:
                case 103:
                    return "Light";
                case 16:
                    return "Autumn";
                case 17:
                    return "Sepia";
                case 104:
                    return "Night View";
                case 105:
                    return "Autumn Leaves";
                default:
                    return $"Unknown ({value})";
            }
        }

        public string? GetMacroDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagMacro, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "On",
                2 => "Magnifying Glass/Super Macro",
                0xFFFF => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetExposureModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagExposureMode, out int value))
                return null;

            return value switch
            {
                0 => "Program",
                1 => "Portrait",
                2 => "Beach",
                3 => "Sports",
                4 => "Snow",
                5 => "Landscape",
                6 => "Auto",
                7 => "Aperture Priority",
                8 => "Shutter Priority",
                9 => "Night Scene / Twilight",
                10 => "Hi-Speed Shutter",
                11 => "Twilight Portrait",
                12 => "Soft Snap/Portrait",
                13 => "Fireworks",
                14 => "Smile Shutter",
                15 => "Manual",
                18 => "High Sensitivity",
                19 => "Macro",
                20 => "Advanced Sports Shooting",
                29 => "Underwater",
                33 => "Food",
                34 => "Panorama",
                35 => "Handheld Night Shot",
                36 => "Anti Motion Blur",
                37 => "Pet",
                38 => "Backlight Correction HDR",
                39 => "Superior Auto",
                40 => "Background Defocus",
                41 => "Soft Skin",
                42 => "3D Image",
                0xFFFF => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetJpegQualityDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagJpegQuality, out int value))
                return null;

            return value switch
            {
                0 => "Normal",
                1 => "Fine",
                2 => "Extra Fine",
                0xFFFF => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetAntiBlurDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagAntiBlur, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "On (Continuous)",
                2 => "On (Shooting)",
                0xFFFF => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetLongExposureNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagLongExposureNoiseReductionOrFocusMode, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "On",
                0xFFFF => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetHighIsoNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagHighIsoNoiseReduction, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "On",
                2 => "Normal",
                3 => "High",
                0x100 => "Auto",
                0xffff => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetPictureEffectDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagPictureEffect, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "Toy Camera",
                2 => "Pop Color",
                3 => "Posterization",
                4 => "Posterization B/W",
                5 => "Retro Photo",
                6 => "Soft High Key",
                7 => "Partial Color (red)",
                8 => "Partial Color (green)",
                9 => "Partial Color (blue)",
                10 => "Partial Color (yellow)",
                13 => "High Contrast Monochrome",
                16 => "Toy Camera (normal)",
                17 => "Toy Camera (cool)",
                18 => "Toy Camera (warm)",
                19 => "Toy Camera (green)",
                20 => "Toy Camera (magenta)",
                32 => "Soft Focus (low)",
                33 => "Soft Focus",
                34 => "Soft Focus (high)",
                48 => "Miniature (auto)",
                49 => "Miniature (top)",
                50 => "Miniature (middle horizontal)",
                51 => "Miniature (bottom)",
                52 => "Miniature (left)",
                53 => "Miniature (middle vertical)",
                54 => "Miniature (right)",
                64 => "HDR Painting (low)",
                65 => "HDR Painting",
                66 => "HDR Painting (high)",
                80 => "Rich-tone Monochrome",
                97 => "Water Color",
                98 => "Water Color 2",
                112 => "Illustration (low)",
                113 => "Illustration",
                114 => "Illustration (high)",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetSoftSkinEffectDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagSoftSkinEffect,
                "Off", "Low", "Mid", "High");
        }

        public string? GetVignettingCorrectionDescription()
        {
            if (!Directory.TryGetUInt32(SonyType1MakernoteDirectory.TagVignettingCorrection, out uint value))
                return null;

            return value switch
            {
                0 => "Off",
                2 => "Auto",
                0xffffffff => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetLateralChromaticAberrationDescription()
        {
            if (!Directory.TryGetUInt32(SonyType1MakernoteDirectory.TagLateralChromaticAberration, out uint value))
                return null;

            return value switch
            {
                0 => "Off",
                2 => "Auto",
                0xffffffff => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetDistortionCorrectionDescription()
        {
            if (!Directory.TryGetUInt32(SonyType1MakernoteDirectory.TagDistortionCorrection, out uint value))
                return null;

            return value switch
            {
                0 => "Off",
                2 => "Auto",
                0xffffffff => "N/A",
                _ => $"Unknown ({value})",
            };
        }

        public string? GetAutoPortraitFramedDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagAutoPortraitFramed,
                "No", "Yes");
        }

        public string? GetFocusModeDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagFocusMode,
                "Manual", null, "AF-A", "AF-C", "AF-S", null, "DMF", "AF-D");
        }

        public string? GetAfPointSelectedDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagAfPointSelected,
                "Auto", "Center", "Top", "Upper-right", "Right", "Lower-right", "Bottom", "Lower-left", "Left", "Upper-left",
                "Far Right", "Far Left", "Upper-middle", "Near Right", "Lower-middle", "Near Left", "Upper Far Right",
                "Lower Far Right", "Lower Far Left", "Upper Far Left");
        }

        public string? GetSonyModelIdDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagSonyModelId, out int value))
                return null;

            return value switch
            {
                2 => "DSC-R1",
                256 => "DSLR-A100",
                257 => "DSLR-A900",
                258 => "DSLR-A700",
                259 => "DSLR-A200",
                260 => "DSLR-A350",
                261 => "DSLR-A300",
                262 => "DSLR-A900 (APS-C mode)",
                263 => "DSLR-A380/A390",
                264 => "DSLR-A330",
                265 => "DSLR-A230",
                266 => "DSLR-A290",
                269 => "DSLR-A850",
                270 => "DSLR-A850 (APS-C mode)",
                273 => "DSLR-A550",
                274 => "DSLR-A500",
                275 => "DSLR-A450",
                278 => "NEX-5",
                279 => "NEX-3",
                280 => "SLT-A33",
                281 => "SLT-A55V",
                282 => "DSLR-A560",
                283 => "DSLR-A580",
                284 => "NEX-C3",
                285 => "SLT-A35",
                286 => "SLT-A65V",
                287 => "SLT-A77V",
                288 => "NEX-5N",
                289 => "NEX-7",
                290 => "NEX-VG20E",
                291 => "SLT-A37",
                292 => "SLT-A57",
                293 => "NEX-F3",
                294 => "SLT-A99V",
                295 => "NEX-6",
                296 => "NEX-5R",
                297 => "DSC-RX100",
                298 => "DSC-RX1",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetSceneModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagSceneMode, out int value))
                return null;

            return value switch
            {
                0 => "Standard",
                1 => "Portrait",
                2 => "Text",
                3 => "Night Scene",
                4 => "Sunset",
                5 => "Sports",
                6 => "Landscape",
                7 => "Night Portrait",
                8 => "Macro",
                9 => "Super Macro",
                16 => "Auto",
                17 => "Night View/Portrait",
                18 => "Sweep Panorama",
                19 => "Handheld Night Shot",
                20 => "Anti Motion Blur",
                21 => "Cont. Priority AE",
                22 => "Auto+",
                23 => "3D Sweep Panorama",
                24 => "Superior Auto",
                25 => "High Sensitivity",
                26 => "Fireworks",
                27 => "Food",
                28 => "Pet",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetAfModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagAfMode, out int value))
                return null;

            return value switch
            {
                0 => "Default",
                1 => "Multi",
                2 => "Center",
                3 => "Spot",
                4 => "Flexible Spot",
                6 => "Touch",
                14 => "Manual Focus",
                15 => "Face Detected",
                0xffff => "n/a",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetAfIlluminatorDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagAfIlluminator, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "Auto",
                0xffff => "n/a",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetFlashLevelDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagFlashLevel, out int value))
                return null;

            return value switch
            {
                -32768 => "Low",
                -3 => "-3/3",
                -2 => "-2/3",
                -1 => "-1/3",
                0 => "Normal",
                1 => "+1/3",
                2 => "+2/3",
                3 => "+3/3",
                128 => "n/a",
                32767 => "High",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetReleaseModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagReleaseMode, out int value))
                return null;

            return value switch
            {
                0 => "Normal",
                2 => "Continuous",
                5 => "Exposure Bracketing",
                6 => "White Balance Bracketing",
                65535 => "n/a",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetSequenceNumberDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagReleaseMode, out int value))
                return null;

            return value switch
            {
                0 => "Single",
                65535 => "n/a",
                _ => value.ToString(),
            };
        }
    }
}
