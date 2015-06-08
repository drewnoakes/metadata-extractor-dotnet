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

using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a
    /// <see cref="FujifilmMakernoteDirectory"/>
    /// .
    /// <p>
    /// Fujifilm added their Makernote tag from the Year 2000's models (e.g.Finepix1400,
    /// Finepix4700). It uses IFD format and start from ASCII character 'FUJIFILM', and next 4
    /// bytes (value 0x000c) points the offset to first IFD entry.
    /// <pre><code>
    /// :0000: 46 55 4A 49 46 49 4C 4D-0C 00 00 00 0F 00 00 00 :0000: FUJIFILM........
    /// :0010: 07 00 04 00 00 00 30 31-33 30 00 10 02 00 08 00 :0010: ......0130......
    /// </code></pre>
    /// There are two big differences to the other manufacturers.
    /// <ul>
    /// <li>Fujifilm's Exif data uses Motorola align, but Makernote ignores it and uses Intel align.</li>
    /// <li>
    /// The other manufacturer's Makernote counts the "offset to data" from the first byte of TIFF header
    /// (same as the other IFD), but Fujifilm counts it from the first byte of Makernote itself.
    /// </li>
    /// </ul>
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class FujifilmMakernoteDescriptor : TagDescriptor<FujifilmMakernoteDirectory>
    {
        public FujifilmMakernoteDescriptor([NotNull] FujifilmMakernoteDirectory directory)
            : base(directory)
        {
        }

        [CanBeNull]
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case FujifilmMakernoteDirectory.TagMakernoteVersion:
                {
                    return GetMakernoteVersionDescription();
                }

                case FujifilmMakernoteDirectory.TagSharpness:
                {
                    return GetSharpnessDescription();
                }

                case FujifilmMakernoteDirectory.TagWhiteBalance:
                {
                    return GetWhiteBalanceDescription();
                }

                case FujifilmMakernoteDirectory.TagColorSaturation:
                {
                    return GetColorSaturationDescription();
                }

                case FujifilmMakernoteDirectory.TagTone:
                {
                    return GetToneDescription();
                }

                case FujifilmMakernoteDirectory.TagContrast:
                {
                    return GetContrastDescription();
                }

                case FujifilmMakernoteDirectory.TagNoiseReduction:
                {
                    return GetNoiseReductionDescription();
                }

                case FujifilmMakernoteDirectory.TagHighIsoNoiseReduction:
                {
                    return GetHighIsoNoiseReductionDescription();
                }

                case FujifilmMakernoteDirectory.TagFlashMode:
                {
                    return GetFlashModeDescription();
                }

                case FujifilmMakernoteDirectory.TagFlashEv:
                {
                    return GetFlashExposureValueDescription();
                }

                case FujifilmMakernoteDirectory.TagMacro:
                {
                    return GetMacroDescription();
                }

                case FujifilmMakernoteDirectory.TagFocusMode:
                {
                    return GetFocusModeDescription();
                }

                case FujifilmMakernoteDirectory.TagSlowSync:
                {
                    return GetSlowSyncDescription();
                }

                case FujifilmMakernoteDirectory.TagPictureMode:
                {
                    return GetPictureModeDescription();
                }

                case FujifilmMakernoteDirectory.TagExrAuto:
                {
                    return GetExrAutoDescription();
                }

                case FujifilmMakernoteDirectory.TagExrMode:
                {
                    return GetExrModeDescription();
                }

                case FujifilmMakernoteDirectory.TagAutoBracketing:
                {
                    return GetAutoBracketingDescription();
                }

                case FujifilmMakernoteDirectory.TagFinePixColor:
                {
                    return GetFinePixColorDescription();
                }

                case FujifilmMakernoteDirectory.TagBlurWarning:
                {
                    return GetBlurWarningDescription();
                }

                case FujifilmMakernoteDirectory.TagFocusWarning:
                {
                    return GetFocusWarningDescription();
                }

                case FujifilmMakernoteDirectory.TagAutoExposureWarning:
                {
                    return GetAutoExposureWarningDescription();
                }

                case FujifilmMakernoteDirectory.TagDynamicRange:
                {
                    return GetDynamicRangeDescription();
                }

                case FujifilmMakernoteDirectory.TagFilmMode:
                {
                    return GetFilmModeDescription();
                }

                case FujifilmMakernoteDirectory.TagDynamicRangeSetting:
                {
                    return GetDynamicRangeSettingDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        private string GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(FujifilmMakernoteDirectory.TagMakernoteVersion, 2);
        }

        [CanBeNull]
        public virtual string GetSharpnessDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagSharpness);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 1:
                {
                    return "Softest";
                }

                case 2:
                {
                    return "Soft";
                }

                case 3:
                {
                    return "Normal";
                }

                case 4:
                {
                    return "Hard";
                }

                case 5:
                {
                    return "Hardest";
                }

                case unchecked((int)(0x82)):
                {
                    return "Medium Soft";
                }

                case unchecked((int)(0x84)):
                {
                    return "Medium Hard";
                }

                case unchecked((int)(0x8000)):
                {
                    return "Film Simulation";
                }

                case unchecked((int)(0xFFFF)):
                {
                    return "N/A";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagWhiteBalance);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Auto";
                }

                case unchecked((int)(0x100)):
                {
                    return "Daylight";
                }

                case unchecked((int)(0x200)):
                {
                    return "Cloudy";
                }

                case unchecked((int)(0x300)):
                {
                    return "Daylight Fluorescent";
                }

                case unchecked((int)(0x301)):
                {
                    return "Day White Fluorescent";
                }

                case unchecked((int)(0x302)):
                {
                    return "White Fluorescent";
                }

                case unchecked((int)(0x303)):
                {
                    return "Warm White Fluorescent";
                }

                case unchecked((int)(0x304)):
                {
                    return "Living Room Warm White Fluorescent";
                }

                case unchecked((int)(0x400)):
                {
                    return "Incandescence";
                }

                case unchecked((int)(0x500)):
                {
                    return "Flash";
                }

                case unchecked((int)(0xf00)):
                {
                    return "Custom White Balance";
                }

                case unchecked((int)(0xf01)):
                {
                    return "Custom White Balance 2";
                }

                case unchecked((int)(0xf02)):
                {
                    return "Custom White Balance 3";
                }

                case unchecked((int)(0xf03)):
                {
                    return "Custom White Balance 4";
                }

                case unchecked((int)(0xf04)):
                {
                    return "Custom White Balance 5";
                }

                case unchecked((int)(0xff0)):
                {
                    return "Kelvin";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetColorSaturationDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagColorSaturation);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Normal";
                }

                case unchecked((int)(0x080)):
                {
                    return "Medium High";
                }

                case unchecked((int)(0x100)):
                {
                    return "High";
                }

                case unchecked((int)(0x180)):
                {
                    return "Medium Low";
                }

                case unchecked((int)(0x200)):
                {
                    return "Low";
                }

                case unchecked((int)(0x300)):
                {
                    return "None (B&W)";
                }

                case unchecked((int)(0x301)):
                {
                    return "B&W Green Filter";
                }

                case unchecked((int)(0x302)):
                {
                    return "B&W Yellow Filter";
                }

                case unchecked((int)(0x303)):
                {
                    return "B&W Blue Filter";
                }

                case unchecked((int)(0x304)):
                {
                    return "B&W Sepia";
                }

                case unchecked((int)(0x8000)):
                {
                    return "Film Simulation";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetToneDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagTone);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Normal";
                }

                case unchecked((int)(0x080)):
                {
                    return "Medium High";
                }

                case unchecked((int)(0x100)):
                {
                    return "High";
                }

                case unchecked((int)(0x180)):
                {
                    return "Medium Low";
                }

                case unchecked((int)(0x200)):
                {
                    return "Low";
                }

                case unchecked((int)(0x300)):
                {
                    return "None (B&W)";
                }

                case unchecked((int)(0x8000)):
                {
                    return "Film Simulation";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetContrastDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagContrast);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Normal";
                }

                case unchecked((int)(0x100)):
                {
                    return "High";
                }

                case unchecked((int)(0x300)):
                {
                    return "Low";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetNoiseReductionDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagNoiseReduction);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x040)):
                {
                    return "Low";
                }

                case unchecked((int)(0x080)):
                {
                    return "Normal";
                }

                case unchecked((int)(0x100)):
                {
                    return "N/A";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetHighIsoNoiseReductionDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagHighIsoNoiseReduction);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Normal";
                }

                case unchecked((int)(0x100)):
                {
                    return "Strong";
                }

                case unchecked((int)(0x200)):
                {
                    return "Weak";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetFlashModeDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagFlashMode, "Auto", "On", "Off", "Red-eye Reduction", "External");
        }

        [CanBeNull]
        public virtual string GetFlashExposureValueDescription()
        {
            Rational value = _directory.GetRational(FujifilmMakernoteDirectory.TagFlashEv);
            return value == null ? null : value.ToSimpleString(false) + " EV (Apex)";
        }

        [CanBeNull]
        public virtual string GetMacroDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagMacro, "Off", "On");
        }

        [CanBeNull]
        public virtual string GetFocusModeDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagFocusMode, "Auto Focus", "Manual Focus");
        }

        [CanBeNull]
        public virtual string GetSlowSyncDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagSlowSync, "Off", "On");
        }

        [CanBeNull]
        public virtual string GetPictureModeDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagPictureMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Auto";
                }

                case unchecked((int)(0x001)):
                {
                    return "Portrait scene";
                }

                case unchecked((int)(0x002)):
                {
                    return "Landscape scene";
                }

                case unchecked((int)(0x003)):
                {
                    return "Macro";
                }

                case unchecked((int)(0x004)):
                {
                    return "Sports scene";
                }

                case unchecked((int)(0x005)):
                {
                    return "Night scene";
                }

                case unchecked((int)(0x006)):
                {
                    return "Program AE";
                }

                case unchecked((int)(0x007)):
                {
                    return "Natural Light";
                }

                case unchecked((int)(0x008)):
                {
                    return "Anti-blur";
                }

                case unchecked((int)(0x009)):
                {
                    return "Beach & Snow";
                }

                case unchecked((int)(0x00a)):
                {
                    return "Sunset";
                }

                case unchecked((int)(0x00b)):
                {
                    return "Museum";
                }

                case unchecked((int)(0x00c)):
                {
                    return "Party";
                }

                case unchecked((int)(0x00d)):
                {
                    return "Flower";
                }

                case unchecked((int)(0x00e)):
                {
                    return "Text";
                }

                case unchecked((int)(0x00f)):
                {
                    return "Natural Light & Flash";
                }

                case unchecked((int)(0x010)):
                {
                    return "Beach";
                }

                case unchecked((int)(0x011)):
                {
                    return "Snow";
                }

                case unchecked((int)(0x012)):
                {
                    return "Fireworks";
                }

                case unchecked((int)(0x013)):
                {
                    return "Underwater";
                }

                case unchecked((int)(0x014)):
                {
                    return "Portrait with Skin Correction";
                }

                case unchecked((int)(0x016)):
                {
                    // skip 0x015
                    return "Panorama";
                }

                case unchecked((int)(0x017)):
                {
                    return "Night (Tripod)";
                }

                case unchecked((int)(0x018)):
                {
                    return "Pro Low-light";
                }

                case unchecked((int)(0x019)):
                {
                    return "Pro Focus";
                }

                case unchecked((int)(0x01b)):
                {
                    // skip 0x01a
                    return "Dog Face Detection";
                }

                case unchecked((int)(0x01c)):
                {
                    return "Cat Face Detection";
                }

                case unchecked((int)(0x100)):
                {
                    return "Aperture priority AE";
                }

                case unchecked((int)(0x200)):
                {
                    return "Shutter priority AE";
                }

                case unchecked((int)(0x300)):
                {
                    return "Manual exposure";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetExrAutoDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagExrAuto, "Auto", "Manual");
        }

        [CanBeNull]
        public virtual string GetExrModeDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagExrMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x100)):
                {
                    return "HR (High Resolution)";
                }

                case unchecked((int)(0x200)):
                {
                    return "SN (Signal to Noise Priority)";
                }

                case unchecked((int)(0x300)):
                {
                    return "DR (Dynamic Range Priority)";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetAutoBracketingDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagAutoBracketing, "Off", "On", "No Flash & Flash");
        }

        [CanBeNull]
        public virtual string GetFinePixColorDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagFinePixColor);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x00)):
                {
                    return "Standard";
                }

                case unchecked((int)(0x10)):
                {
                    return "Chrome";
                }

                case unchecked((int)(0x30)):
                {
                    return "B&W";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetBlurWarningDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagBlurWarning, "No Blur Warning", "Blur warning");
        }

        [CanBeNull]
        public virtual string GetFocusWarningDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagFocusWarning, "Good Focus", "Out Of Focus");
        }

        [CanBeNull]
        public virtual string GetAutoExposureWarningDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagAutoExposureWarning, "AE Good", "Over Exposed");
        }

        [CanBeNull]
        public virtual string GetDynamicRangeDescription()
        {
            return GetIndexedDescription(FujifilmMakernoteDirectory.TagDynamicRange, 1, "Standard", null, "Wide");
        }

        [CanBeNull]
        public virtual string GetFilmModeDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagFilmMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "F0/Standard (Provia) ";
                }

                case unchecked((int)(0x100)):
                {
                    return "F1/Studio Portrait";
                }

                case unchecked((int)(0x110)):
                {
                    return "F1a/Studio Portrait Enhanced Saturation";
                }

                case unchecked((int)(0x120)):
                {
                    return "F1b/Studio Portrait Smooth Skin Tone (Astia)";
                }

                case unchecked((int)(0x130)):
                {
                    return "F1c/Studio Portrait Increased Sharpness";
                }

                case unchecked((int)(0x200)):
                {
                    return "F2/Fujichrome (Velvia)";
                }

                case unchecked((int)(0x300)):
                {
                    return "F3/Studio Portrait Ex";
                }

                case unchecked((int)(0x400)):
                {
                    return "F4/Velvia";
                }

                case unchecked((int)(0x500)):
                {
                    return "Pro Neg. Std";
                }

                case unchecked((int)(0x501)):
                {
                    return "Pro Neg. Hi";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetDynamicRangeSettingDescription()
        {
            int? value = _directory.GetInteger(FujifilmMakernoteDirectory.TagDynamicRangeSetting);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x000)):
                {
                    return "Auto (100-400%)";
                }

                case unchecked((int)(0x001)):
                {
                    return "Manual";
                }

                case unchecked((int)(0x100)):
                {
                    return "Standard (100%)";
                }

                case unchecked((int)(0x200)):
                {
                    return "Wide 1 (230%)";
                }

                case unchecked((int)(0x201)):
                {
                    return "Wide 2 (400%)";
                }

                case unchecked((int)(0x8000)):
                {
                    return "Film Simulation";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }
    }
}
