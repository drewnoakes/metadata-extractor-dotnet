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

using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PentaxMakernoteDirectory"/>.
    /// <para>
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pentax_mn.html
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PentaxMakernoteDescriptor : TagDescriptor<PentaxMakernoteDirectory>
    {
        public PentaxMakernoteDescriptor([NotNull] PentaxMakernoteDirectory directory)
            : base(directory)
        {
        }

        [CanBeNull]
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PentaxMakernoteDirectory.TagCaptureMode:
                {
                    return GetCaptureModeDescription();
                }

                case PentaxMakernoteDirectory.TagQualityLevel:
                {
                    return GetQualityLevelDescription();
                }

                case PentaxMakernoteDirectory.TagFocusMode:
                {
                    return GetFocusModeDescription();
                }

                case PentaxMakernoteDirectory.TagFlashMode:
                {
                    return GetFlashModeDescription();
                }

                case PentaxMakernoteDirectory.TagWhiteBalance:
                {
                    return GetWhiteBalanceDescription();
                }

                case PentaxMakernoteDirectory.TagDigitalZoom:
                {
                    return GetDigitalZoomDescription();
                }

                case PentaxMakernoteDirectory.TagSharpness:
                {
                    return GetSharpnessDescription();
                }

                case PentaxMakernoteDirectory.TagContrast:
                {
                    return GetContrastDescription();
                }

                case PentaxMakernoteDirectory.TagSaturation:
                {
                    return GetSaturationDescription();
                }

                case PentaxMakernoteDirectory.TagIsoSpeed:
                {
                    return GetIsoSpeedDescription();
                }

                case PentaxMakernoteDirectory.TagColour:
                {
                    return GetColourDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public virtual string GetColourDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagColour, 1, "Normal", "Black & White", "Sepia");
        }

        [CanBeNull]
        public virtual string GetIsoSpeedDescription()
        {
            int? value = Directory.GetInteger(PentaxMakernoteDirectory.TagIsoSpeed);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 10:
                {
                    // TODO there must be other values which aren't catered for here
                    return "ISO 100";
                }

                case 16:
                {
                    return "ISO 200";
                }

                case 100:
                {
                    return "ISO 100";
                }

                case 200:
                {
                    return "ISO 200";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetSaturationDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagSaturation, "Normal", "Low", "High");
        }

        [CanBeNull]
        public virtual string GetContrastDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagContrast, "Normal", "Low", "High");
        }

        [CanBeNull]
        public virtual string GetSharpnessDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagSharpness, "Normal", "Soft", "Hard");
        }

        [CanBeNull]
        public virtual string GetDigitalZoomDescription()
        {
            float? value = Directory.GetFloatObject(PentaxMakernoteDirectory.TagDigitalZoom);
            if (value == null)
            {
                return null;
            }
            if (value == 0)
            {
                return "Off";
            }
            return Extensions.ConvertToString((float)value);
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagWhiteBalance, "Auto", "Daylight", "Shade", "Tungsten", "Fluorescent", "Manual");
        }

        [CanBeNull]
        public virtual string GetFlashModeDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagFlashMode, 1, "Auto", "Flash On", null, "Flash Off", null, "Red-eye Reduction");
        }

        [CanBeNull]
        public virtual string GetFocusModeDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagFocusMode, 2, "Custom", "Auto");
        }

        [CanBeNull]
        public virtual string GetQualityLevelDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagQualityLevel, "Good", "Better", "Best");
        }

        [CanBeNull]
        public virtual string GetCaptureModeDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagCaptureMode, "Auto", "Night-scene", "Manual", null, "Multiple");
        }
    }
}
