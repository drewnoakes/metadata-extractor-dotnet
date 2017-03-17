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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

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
        public SonyType1MakernoteDescriptor([NotNull] SonyType1MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case SonyType1MakernoteDirectory.TagImageQuality:
                    return GetImageQualityDescription();
                case SonyType1MakernoteDirectory.TagFlashExposureComp:
                    return GetFlashExposureCompensationDescription();
                case SonyType1MakernoteDirectory.TagTeleconverter:
                    return GetTeleconverterDescription();
                case SonyType1MakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case SonyType1MakernoteDirectory.TagColorTemperature:
                    return GetColorTemperatureDescription();
                case SonyType1MakernoteDirectory.TagSceneMode:
                    return GetSceneModeDescription();
                case SonyType1MakernoteDirectory.TagZoneMatching:
                    return GetZoneMatchingDescription();
                case SonyType1MakernoteDirectory.TagDynamicRangeOptimiser:
                    return GetDynamicRangeOptimizerDescription();
                case SonyType1MakernoteDirectory.TagImageStabilisation:
                    return GetImageStabilizationDescription();
//              case SonyType1MakernoteDirectory.TagLensId:
//                  // Unfortunately it seems that there is no definite mapping between a lens ID and a lens model
//                  // http://gvsoft.homedns.org/exif/makernote-sony-type1.html#0xb027
//                  return getLensIDDescription();
                case SonyType1MakernoteDirectory.TagColorMode:
                    return GetColorModeDescription();
                case SonyType1MakernoteDirectory.TagMacro:
                    return GetMacroDescription();
                case SonyType1MakernoteDirectory.TagExposureMode:
                    return GetExposureModeDescription();
                case SonyType1MakernoteDirectory.TagJpegQuality:
                    return GetJpegQualityDescription();
                case SonyType1MakernoteDirectory.TagAntiBlur:
                    return GetAntiBlurDescription();
                case SonyType1MakernoteDirectory.TagLongExposureNoiseReductionOrFocusMode:
                    return GetLongExposureNoiseReductionDescription();
                case SonyType1MakernoteDirectory.TagHighIsoNoiseReduction:
                    return GetHighIsoNoiseReductionDescription();
                case SonyType1MakernoteDirectory.TagPictureEffect:
                    return GetPictureEffectDescription();
                case SonyType1MakernoteDirectory.TagSoftSkinEffect:
                    return GetSoftSkinEffectDescription();
                case SonyType1MakernoteDirectory.TagVignettingCorrection:
                    return GetVignettingCorrectionDescription();
                case SonyType1MakernoteDirectory.TagLateralChromaticAberration:
                    return GetLateralChromaticAberrationDescription();
                case SonyType1MakernoteDirectory.TagDistortionCorrection:
                    return GetDistortionCorrectionDescription();
                case SonyType1MakernoteDirectory.TagAutoPortraitFramed:
                    return GetAutoPortraitFramedDescription();
                case SonyType1MakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case SonyType1MakernoteDirectory.TagAfPointSelected:
                    return GetAfPointSelectedDescription();
                case SonyType1MakernoteDirectory.TagSonyModelId:
                    return GetSonyModelIdDescription();
                case SonyType1MakernoteDirectory.TagAfMode:
                    return GetAfModeDescription();
                case SonyType1MakernoteDirectory.TagAfIlluminator:
                    return GetAfIlluminatorDescription();
                case SonyType1MakernoteDirectory.TagFlashLevel:
                    return GetFlashLevelDescription();
                case SonyType1MakernoteDirectory.TagReleaseMode:
                    return GetReleaseModeDescription();
                case SonyType1MakernoteDirectory.TagSequenceNumber:
                    return GetSequenceNumberDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetImageQualityDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagImageQuality,
                "RAW", "Super Fine", "Fine", "Standard", "Economy", "Extra Fine", "RAW + JPEG", "Compressed RAW", "Compressed RAW + JPEG");
        }

        [CanBeNull]
        public string GetFlashExposureCompensationDescription()
        {
            return GetFormattedInt(SonyType1MakernoteDirectory.TagFlashExposureComp,
                "{0} EV");
        }

        [CanBeNull]
        public string GetTeleconverterDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagTeleconverter, out int value))
                return null;

            switch (value)
            {
                case 0x00:
                    return "None";
                case 0x48:
                    return "Minolta/Sony AF 2x APO (D)";
                case 0x50:
                    return "Minolta AF 2x APO II";
                case 0x60:
                    return "Minolta AF 2x APO";
                case 0x88:
                    return "Minolta/Sony AF 1.4x APO (D)";
                case 0x90:
                    return "Minolta AF 1.4x APO II";
                case 0xa0:
                    return "Minolta AF 1.4x APO";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagWhiteBalance, out int value))
                return null;

            switch (value)
            {
                case 0x00:
                    return "Auto";
                case 0x01:
                    return "Color Temperature/Color Filter";
                case 0x10:
                    return "Daylight";
                case 0x20:
                    return "Cloudy";
                case 0x30:
                    return "Shade";
                case 0x40:
                    return "Tungsten";
                case 0x50:
                    return "Flash";
                case 0x60:
                    return "Fluorescent";
                case 0x70:
                    return "Custom";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetColorTemperatureDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagColorTemperature, out int value))
                return null;
            if (value == 0)
                return "Auto";
            var kelvin = ((value & 0x00FF0000) >> 8) | ((value & unchecked((int)0xFF000000)) >> 24);
            return $"{kelvin} K";
        }

        [CanBeNull]
        public string GetZoneMatchingDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagZoneMatching,
                "ISO Setting Used", "High Key", "Low Key");
        }

        [CanBeNull]
        public string GetDynamicRangeOptimizerDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagDynamicRangeOptimiser, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "Standard";
                case 2:
                    return "Advanced Auto";
                case 3:
                    return "Auto";
                case 8:
                    return "Advanced LV1";
                case 9:
                    return "Advanced LV2";
                case 10:
                    return "Advanced LV3";
                case 11:
                    return "Advanced LV4";
                case 12:
                    return "Advanced LV5";
                case 16:
                    return "LV1";
                case 17:
                    return "LV2";
                case 18:
                    return "LV3";
                case 19:
                    return "LV4";
                case 20:
                    return "LV5";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetImageStabilizationDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagImageStabilisation, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "On";
                default:
                    return "N/A";
            }
        }

        [CanBeNull]
        public string GetColorModeDescription()
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

        [CanBeNull]
        public string GetMacroDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagMacro, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "On";
                case 2:
                    return "Magnifying Glass/Super Macro";
                case 0xFFFF:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetExposureModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagExposureMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Program";
                case 1:
                    return "Portrait";
                case 2:
                    return "Beach";
                case 3:
                    return "Sports";
                case 4:
                    return "Snow";
                case 5:
                    return "Landscape";
                case 6:
                    return "Auto";
                case 7:
                    return "Aperture Priority";
                case 8:
                    return "Shutter Priority";
                case 9:
                    return "Night Scene / Twilight";
                case 10:
                    return "Hi-Speed Shutter";
                case 11:
                    return "Twilight Portrait";
                case 12:
                    return "Soft Snap/Portrait";
                case 13:
                    return "Fireworks";
                case 14:
                    return "Smile Shutter";
                case 15:
                    return "Manual";
                case 18:
                    return "High Sensitivity";
                case 19:
                    return "Macro";
                case 20:
                    return "Advanced Sports Shooting";
                case 29:
                    return "Underwater";
                case 33:
                    return "Food";
                case 34:
                    return "Panorama";
                case 35:
                    return "Handheld Night Shot";
                case 36:
                    return "Anti Motion Blur";
                case 37:
                    return "Pet";
                case 38:
                    return "Backlight Correction HDR";
                case 39:
                    return "Superior Auto";
                case 40:
                    return "Background Defocus";
                case 41:
                    return "Soft Skin";
                case 42:
                    return "3D Image";
                case 0xFFFF:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetJpegQualityDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagJpegQuality, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Normal";
                case 1:
                    return "Fine";
                case 2:
                    return "Extra Fine";
                case 0xFFFF:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetAntiBlurDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagAntiBlur, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "On (Continuous)";
                case 2:
                    return "On (Shooting)";
                case 0xFFFF:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetLongExposureNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagLongExposureNoiseReductionOrFocusMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "On";
                case 0xFFFF:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetHighIsoNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagHighIsoNoiseReduction, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "On";
                case 2:
                    return "Normal";
                case 3:
                    return "High";
                case 0x100:
                    return "Auto";
                case 0xffff:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetPictureEffectDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagPictureEffect, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "Toy Camera";
                case 2:
                    return "Pop Color";
                case 3:
                    return "Posterization";
                case 4:
                    return "Posterization B/W";
                case 5:
                    return "Retro Photo";
                case 6:
                    return "Soft High Key";
                case 7:
                    return "Partial Color (red)";
                case 8:
                    return "Partial Color (green)";
                case 9:
                    return "Partial Color (blue)";
                case 10:
                    return "Partial Color (yellow)";
                case 13:
                    return "High Contrast Monochrome";
                case 16:
                    return "Toy Camera (normal)";
                case 17:
                    return "Toy Camera (cool)";
                case 18:
                    return "Toy Camera (warm)";
                case 19:
                    return "Toy Camera (green)";
                case 20:
                    return "Toy Camera (magenta)";
                case 32:
                    return "Soft Focus (low)";
                case 33:
                    return "Soft Focus";
                case 34:
                    return "Soft Focus (high)";
                case 48:
                    return "Miniature (auto)";
                case 49:
                    return "Miniature (top)";
                case 50:
                    return "Miniature (middle horizontal)";
                case 51:
                    return "Miniature (bottom)";
                case 52:
                    return "Miniature (left)";
                case 53:
                    return "Miniature (middle vertical)";
                case 54:
                    return "Miniature (right)";
                case 64:
                    return "HDR Painting (low)";
                case 65:
                    return "HDR Painting";
                case 66:
                    return "HDR Painting (high)";
                case 80:
                    return "Rich-tone Monochrome";
                case 97:
                    return "Water Color";
                case 98:
                    return "Water Color 2";
                case 112:
                    return "Illustration (low)";
                case 113:
                    return "Illustration";
                case 114:
                    return "Illustration (high)";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetSoftSkinEffectDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagSoftSkinEffect,
                "Off", "Low", "Mid", "High");
        }

        [CanBeNull]
        public string GetVignettingCorrectionDescription()
        {
            if (!Directory.TryGetUInt32(SonyType1MakernoteDirectory.TagVignettingCorrection, out uint value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 2:
                    return "Auto";
                case 0xffffffff:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetLateralChromaticAberrationDescription()
        {
            if (!Directory.TryGetUInt32(SonyType1MakernoteDirectory.TagLateralChromaticAberration, out uint value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 2:
                    return "Auto";
                case 0xffffffff:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetDistortionCorrectionDescription()
        {
            if (!Directory.TryGetUInt32(SonyType1MakernoteDirectory.TagDistortionCorrection, out uint value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 2:
                    return "Auto";
                case 0xffffffff:
                    return "N/A";
                default:
                    return $"Unknown ({value})";
            }
        }

        [CanBeNull]
        public string GetAutoPortraitFramedDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagAutoPortraitFramed,
                "No", "Yes");
        }

        [CanBeNull]
        public string GetFocusModeDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagFocusMode,
                "Manual", null, "AF-A", "AF-C", "AF-S", null, "DMF", "AF-D");
        }

        [CanBeNull]
        public string GetAfPointSelectedDescription()
        {
            return GetIndexedDescription(SonyType1MakernoteDirectory.TagAfPointSelected,
                "Auto", "Center", "Top", "Upper-right", "Right", "Lower-right", "Bottom", "Lower-left", "Left", "Upper-left",
                "Far Right", "Far Left", "Upper-middle", "Near Right", "Lower-middle", "Near Left", "Upper Far Right",
                "Lower Far Right", "Lower Far Left", "Upper Far Left");
        }

        [CanBeNull]
        public string GetSonyModelIdDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagSonyModelId, out int value))
                return null;

            switch (value)
            {
                case 2:
                    return "DSC-R1";
                case 256:
                    return "DSLR-A100";
                case 257:
                    return "DSLR-A900";
                case 258:
                    return "DSLR-A700";
                case 259:
                    return "DSLR-A200";
                case 260:
                    return "DSLR-A350";
                case 261:
                    return "DSLR-A300";
                case 262:
                    return "DSLR-A900 (APS-C mode)";
                case 263:
                    return "DSLR-A380/A390";
                case 264:
                    return "DSLR-A330";
                case 265:
                    return "DSLR-A230";
                case 266:
                    return "DSLR-A290";
                case 269:
                    return "DSLR-A850";
                case 270:
                    return "DSLR-A850 (APS-C mode)";
                case 273:
                    return "DSLR-A550";
                case 274:
                    return "DSLR-A500";
                case 275:
                    return "DSLR-A450";
                case 278:
                    return "NEX-5";
                case 279:
                    return "NEX-3";
                case 280:
                    return "SLT-A33";
                case 281:
                    return "SLT-A55V";
                case 282:
                    return "DSLR-A560";
                case 283:
                    return "DSLR-A580";
                case 284:
                    return "NEX-C3";
                case 285:
                    return "SLT-A35";
                case 286:
                    return "SLT-A65V";
                case 287:
                    return "SLT-A77V";
                case 288:
                    return "NEX-5N";
                case 289:
                    return "NEX-7";
                case 290:
                    return "NEX-VG20E";
                case 291:
                    return "SLT-A37";
                case 292:
                    return "SLT-A57";
                case 293:
                    return "NEX-F3";
                case 294:
                    return "SLT-A99V";
                case 295:
                    return "NEX-6";
                case 296:
                    return "NEX-5R";
                case 297:
                    return "DSC-RX100";
                case 298:
                    return "DSC-RX1";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSceneModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagSceneMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Standard";
                case 1:
                    return "Portrait";
                case 2:
                    return "Text";
                case 3:
                    return "Night Scene";
                case 4:
                    return "Sunset";
                case 5:
                    return "Sports";
                case 6:
                    return "Landscape";
                case 7:
                    return "Night Portrait";
                case 8:
                    return "Macro";
                case 9:
                    return "Super Macro";
                case 16:
                    return "Auto";
                case 17:
                    return "Night View/Portrait";
                case 18:
                    return "Sweep Panorama";
                case 19:
                    return "Handheld Night Shot";
                case 20:
                    return "Anti Motion Blur";
                case 21:
                    return "Cont. Priority AE";
                case 22:
                    return "Auto+";
                case 23:
                    return "3D Sweep Panorama";
                case 24:
                    return "Superior Auto";
                case 25:
                    return "High Sensitivity";
                case 26:
                    return "Fireworks";
                case 27:
                    return "Food";
                case 28:
                    return "Pet";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetAfModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagAfMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Default";
                case 1:
                    return "Multi";
                case 2:
                    return "Center";
                case 3:
                    return "Spot";
                case 4:
                    return "Flexible Spot";
                case 6:
                    return "Touch";
                case 14:
                    return "Manual Focus";
                case 15:
                    return "Face Detected";
                case 0xffff:
                    return "n/a";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetAfIlluminatorDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagAfIlluminator, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Off";
                case 1:
                    return "Auto";
                case 0xffff:
                    return "n/a";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetFlashLevelDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagFlashLevel, out int value))
                return null;

            switch (value)
            {
                case -32768:
                    return "Low";
                case -3:
                    return "-3/3";
                case -2:
                    return "-2/3";
                case -1:
                    return "-1/3";
                case 0:
                    return "Normal";
                case 1:
                    return "+1/3";
                case 2:
                    return "+2/3";
                case 3:
                    return "+3/3";
                case 128:
                    return "n/a";
                case 32767:
                    return "High";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetReleaseModeDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagReleaseMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Normal";
                case 2:
                    return "Continuous";
                case 5:
                    return "Exposure Bracketing";
                case 6:
                    return "White Balance Bracketing";
                case 65535:
                    return "n/a";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSequenceNumberDescription()
        {
            if (!Directory.TryGetInt32(SonyType1MakernoteDirectory.TagReleaseMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Single";
                case 65535:
                    return "n/a";
                default:
                    return value.ToString();
            }
        }
    }
}
