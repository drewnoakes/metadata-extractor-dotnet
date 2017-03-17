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
    /// Provides human-readable string representations of tag values stored in a <see cref="FujifilmMakernoteDirectory"/>.
    /// </summary>
    /// <summary>
    /// Fujifilm added their Makernote tag from the Year 2000's models (e.g.Finepix1400,
    /// Finepix4700). It uses IFD format and start from ASCII character 'FUJIFILM', and next 4
    /// bytes (value 0x000c) points the offset to first IFD entry.
    /// <code>
    /// :0000: 46 55 4A 49 46 49 4C 4D-0C 00 00 00 0F 00 00 00 :0000: FUJIFILM........
    /// :0010: 07 00 04 00 00 00 30 31-33 30 00 10 02 00 08 00 :0010: ......0130......
    /// </code>
    /// There are two big differences to the other manufacturers.
    /// <list type="bullet">
    /// <item>Fujifilm's Exif data uses Motorola align, but Makernote ignores it and uses Intel align.</item>
    /// <item>
    /// The other manufacturer's Makernote counts the "offset to data" from the first byte of TIFF header
    /// (same as the other IFD), but Fujifilm counts it from the first byte of Makernote itself.
    /// </item>
    /// </list>
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class FujifilmMakernoteDescriptor : TagDescriptor<FujifilmMakernoteDirectory>
    {
        public FujifilmMakernoteDescriptor([NotNull] FujifilmMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case FujifilmMakernoteDirectory.TagMakernoteVersion:
                    return GetMakernoteVersionDescription();
                case FujifilmMakernoteDirectory.TagSharpness:
                    return GetSharpnessDescription();
                case FujifilmMakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case FujifilmMakernoteDirectory.TagColorSaturation:
                    return GetColorSaturationDescription();
                case FujifilmMakernoteDirectory.TagTone:
                    return GetToneDescription();
                case FujifilmMakernoteDirectory.TagContrast:
                    return GetContrastDescription();
                case FujifilmMakernoteDirectory.TagNoiseReduction:
                    return GetNoiseReductionDescription();
                case FujifilmMakernoteDirectory.TagHighIsoNoiseReduction:
                    return GetHighIsoNoiseReductionDescription();
                case FujifilmMakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                case FujifilmMakernoteDirectory.TagFlashEv:
                    return GetFlashExposureValueDescription();
                case FujifilmMakernoteDirectory.TagMacro:
                    return GetMacroDescription();
                case FujifilmMakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case FujifilmMakernoteDirectory.TagSlowSync:
                    return GetSlowSyncDescription();
                case FujifilmMakernoteDirectory.TagPictureMode:
                    return GetPictureModeDescription();
                case FujifilmMakernoteDirectory.TagExrAuto:
                    return GetExrAutoDescription();
                case FujifilmMakernoteDirectory.TagExrMode:
                    return GetExrModeDescription();
                case FujifilmMakernoteDirectory.TagAutoBracketing:
                    return GetAutoBracketingDescription();
                case FujifilmMakernoteDirectory.TagFinePixColor:
                    return GetFinePixColorDescription();
                case FujifilmMakernoteDirectory.TagBlurWarning:
                    return GetBlurWarningDescription();
                case FujifilmMakernoteDirectory.TagFocusWarning:
                    return GetFocusWarningDescription();
                case FujifilmMakernoteDirectory.TagAutoExposureWarning:
                    return GetAutoExposureWarningDescription();
                case FujifilmMakernoteDirectory.TagDynamicRange:
                    return GetDynamicRangeDescription();
                case FujifilmMakernoteDirectory.TagFilmMode:
                    return GetFilmModeDescription();
                case FujifilmMakernoteDirectory.TagDynamicRangeSetting:
                    return GetDynamicRangeSettingDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        private string GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(FujifilmMakernoteDirectory.TagMakernoteVersion, 2);
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagSharpness, out int value))
                return null;

            switch (value)
            {
                case 1:
                    return "Softest";
                case 2:
                    return "Soft";
                case 3:
                    return "Normal";
                case 4:
                    return "Hard";
                case 5:
                    return "Hardest";
                case 0x82:
                    return "Medium Soft";
                case 0x84:
                    return "Medium Hard";
                case 0x8000:
                    return "Film Simulation";
                case 0xFFFF:
                    return "N/A";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagWhiteBalance, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Auto";
                case 0x100:
                    return "Daylight";
                case 0x200:
                    return "Cloudy";
                case 0x300:
                    return "Daylight Fluorescent";
                case 0x301:
                    return "Day White Fluorescent";
                case 0x302:
                    return "White Fluorescent";
                case 0x303:
                    return "Warm White Fluorescent";
                case 0x304:
                    return "Living Room Warm White Fluorescent";
                case 0x400:
                    return "Incandescence";
                case 0x500:
                    return "Flash";
                case 0xf00:
                    return "Custom White Balance";
                case 0xf01:
                    return "Custom White Balance 2";
                case 0xf02:
                    return "Custom White Balance 3";
                case 0xf03:
                    return "Custom White Balance 4";
                case 0xf04:
                    return "Custom White Balance 5";
                case 0xff0:
                    return "Kelvin";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetColorSaturationDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagColorSaturation, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Normal";
                case 0x080:
                    return "Medium High";
                case 0x100:
                    return "High";
                case 0x180:
                    return "Medium Low";
                case 0x200:
                    return "Low";
                case 0x300:
                    return "None (B&W)";
                case 0x301:
                    return "B&W Green Filter";
                case 0x302:
                    return "B&W Yellow Filter";
                case 0x303:
                    return "B&W Blue Filter";
                case 0x304:
                    return "B&W Sepia";
                case 0x8000:
                    return "Film Simulation";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetToneDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagTone, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Normal";
                case 0x080:
                    return "Medium High";
                case 0x100:
                    return "High";
                case 0x180:
                    return "Medium Low";
                case 0x200:
                    return "Low";
                case 0x300:
                    return "None (B&W)";
                case 0x8000:
                    return "Film Simulation";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagContrast, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Normal";
                case 0x100:
                    return "High";
                case 0x300:
                    return "Low";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagNoiseReduction, out int value))
                return null;
            switch (value)
            {
                case 0x040:
                    return "Low";
                case 0x080:
                    return "Normal";
                case 0x100:
                    return "N/A";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetHighIsoNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagHighIsoNoiseReduction, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Normal";
                case 0x100:
                    return "Strong";
                case 0x200:
                    return "Weak";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetFlashModeDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagFlashMode,
                "Auto", "On", "Off", "Red-eye Reduction", "External");
        }

        [CanBeNull]
        public string GetFlashExposureValueDescription()
        {
            if (!Directory.TryGetRational(FujifilmMakernoteDirectory.TagFlashEv, out Rational value))
                return null;
            return value.ToSimpleString(allowDecimal: false) + " EV (Apex)";
        }

        [CanBeNull]
        public string GetMacroDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagMacro,
                "Off", "On");
        }

        [CanBeNull]
        public string GetFocusModeDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagFocusMode,
                "Auto Focus", "Manual Focus");
        }

        [CanBeNull]
        public string GetSlowSyncDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagSlowSync,
                "Off", "On");
        }

        [CanBeNull]
        public string GetPictureModeDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagPictureMode, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Auto";
                case 0x001:
                    return "Portrait scene";
                case 0x002:
                    return "Landscape scene";
                case 0x003:
                    return "Macro";
                case 0x004:
                    return "Sports scene";
                case 0x005:
                    return "Night scene";
                case 0x006:
                    return "Program AE";
                case 0x007:
                    return "Natural Light";
                case 0x008:
                    return "Anti-blur";
                case 0x009:
                    return "Beach & Snow";
                case 0x00a:
                    return "Sunset";
                case 0x00b:
                    return "Museum";
                case 0x00c:
                    return "Party";
                case 0x00d:
                    return "Flower";
                case 0x00e:
                    return "Text";
                case 0x00f:
                    return "Natural Light & Flash";
                case 0x010:
                    return "Beach";
                case 0x011:
                    return "Snow";
                case 0x012:
                    return "Fireworks";
                case 0x013:
                    return "Underwater";
                case 0x014:
                    return "Portrait with Skin Correction";
                // skip 0x015
                case 0x016:
                    return "Panorama";
                case 0x017:
                    return "Night (Tripod)";
                case 0x018:
                    return "Pro Low-light";
                case 0x019:
                    return "Pro Focus";
                // skip 0x01a
                case 0x01b:
                    return "Dog Face Detection";
                case 0x01c:
                    return "Cat Face Detection";
                case 0x100:
                    return "Aperture priority AE";
                case 0x200:
                    return "Shutter priority AE";
                case 0x300:
                    return "Manual exposure";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetExrAutoDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagExrAuto,
                "Auto", "Manual");
        }

        [CanBeNull]
        public string GetExrModeDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagExrMode, out int value))
                return null;

            switch (value)
            {
                case 0x100:
                    return "HR (High Resolution)";
                case 0x200:
                    return "SN (Signal to Noise Priority)";
                case 0x300:
                    return "DR (Dynamic Range Priority)";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetAutoBracketingDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagAutoBracketing,
                "Off", "On", "No Flash & Flash");
        }

        [CanBeNull]
        public string GetFinePixColorDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagFinePixColor, out int value))
                return null;
            switch (value)
            {
                case 0x00:
                    return "Standard";
                case 0x10:
                    return "Chrome";
                case 0x30:
                    return "B&W";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetBlurWarningDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagBlurWarning,
                "No Blur Warning", "Blur warning");
        }

        [CanBeNull]
        public string GetFocusWarningDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagFocusWarning,
                "Good Focus", "Out Of Focus");
        }

        [CanBeNull]
        public string GetAutoExposureWarningDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagAutoExposureWarning,
                "AE Good", "Over Exposed");
        }

        [CanBeNull]
        public string GetDynamicRangeDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagDynamicRange,
                1,
                "Standard", null, "Wide");
        }

        [CanBeNull]
        public string GetFilmModeDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagFilmMode, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "F0/Standard (Provia) ";
                case 0x100:
                    return "F1/Studio Portrait";
                case 0x110:
                    return "F1a/Studio Portrait Enhanced Saturation";
                case 0x120:
                    return "F1b/Studio Portrait Smooth Skin Tone (Astia)";
                case 0x130:
                    return "F1c/Studio Portrait Increased Sharpness";
                case 0x200:
                    return "F2/Fujichrome (Velvia)";
                case 0x300:
                    return "F3/Studio Portrait Ex";
                case 0x400:
                    return "F4/Velvia";
                case 0x500:
                    return "Pro Neg. Std";
                case 0x501:
                    return "Pro Neg. Hi";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetDynamicRangeSettingDescription()
        {
            if (!Directory.TryGetInt32(FujifilmMakernoteDirectory.TagDynamicRangeSetting, out int value))
                return null;

            switch (value)
            {
                case 0x000:
                    return "Auto (100-400%)";
                case 0x001:
                    return "Manual";
                case 0x100:
                    return "Standard (100%)";
                case 0x200:
                    return "Wide 1 (230%)";
                case 0x201:
                    return "Wide 2 (400%)";
                case 0x8000:
                    return "Film Simulation";
                default:
                    return "Unknown (" + value + ")";
            }
        }
    }
}
