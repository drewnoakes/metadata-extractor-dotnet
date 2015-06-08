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
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a
    /// <see cref="KodakMakernoteDirectory"/>
    /// .
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class KodakMakernoteDescriptor : TagDescriptor<KodakMakernoteDirectory>
    {
        public KodakMakernoteDescriptor([NotNull] KodakMakernoteDirectory directory)
            : base(directory)
        {
        }

        [CanBeNull]
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case KodakMakernoteDirectory.TagQuality:
                {
                    return GetQualityDescription();
                }

                case KodakMakernoteDirectory.TagBurstMode:
                {
                    return GetBurstModeDescription();
                }

                case KodakMakernoteDirectory.TagShutterMode:
                {
                    return GetShutterModeDescription();
                }

                case KodakMakernoteDirectory.TagFocusMode:
                {
                    return GetFocusModeDescription();
                }

                case KodakMakernoteDirectory.TagWhiteBalance:
                {
                    return GetWhiteBalanceDescription();
                }

                case KodakMakernoteDirectory.TagFlashMode:
                {
                    return GetFlashModeDescription();
                }

                case KodakMakernoteDirectory.TagFlashFired:
                {
                    return GetFlashFiredDescription();
                }

                case KodakMakernoteDirectory.TagColorMode:
                {
                    return GetColorModeDescription();
                }

                case KodakMakernoteDirectory.TagSharpness:
                {
                    return GetSharpnessDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public virtual string GetSharpnessDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagSharpness, "Normal");
        }

        [CanBeNull]
        public virtual string GetColorModeDescription()
        {
            int? value = _directory.GetInteger(KodakMakernoteDirectory.TagColorMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x001)):
                case unchecked((int)(0x2000)):
                {
                    return "B&W";
                }

                case unchecked((int)(0x002)):
                case unchecked((int)(0x4000)):
                {
                    return "Sepia";
                }

                case unchecked((int)(0x003)):
                {
                    return "B&W Yellow Filter";
                }

                case unchecked((int)(0x004)):
                {
                    return "B&W Red Filter";
                }

                case unchecked((int)(0x020)):
                {
                    return "Saturated Color";
                }

                case unchecked((int)(0x040)):
                case unchecked((int)(0x200)):
                {
                    return "Neutral Color";
                }

                case unchecked((int)(0x100)):
                {
                    return "Saturated Color";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetFlashFiredDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFlashFired, "No", "Yes");
        }

        [CanBeNull]
        public virtual string GetFlashModeDescription()
        {
            int? value = _directory.GetInteger(KodakMakernoteDirectory.TagFlashMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case unchecked((int)(0x00)):
                {
                    return "Auto";
                }

                case unchecked((int)(0x10)):
                case unchecked((int)(0x01)):
                {
                    return "Fill Flash";
                }

                case unchecked((int)(0x20)):
                case unchecked((int)(0x02)):
                {
                    return "Off";
                }

                case unchecked((int)(0x40)):
                case unchecked((int)(0x03)):
                {
                    return "Red Eye";
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
            return GetIndexedDescription(KodakMakernoteDirectory.TagWhiteBalance, "Auto", "Flash", "Tungsten", "Daylight");
        }

        [CanBeNull]
        public virtual string GetFocusModeDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFocusMode, "Normal", null, "Macro");
        }

        [CanBeNull]
        public virtual string GetShutterModeDescription()
        {
            int? value = _directory.GetInteger(KodakMakernoteDirectory.TagShutterMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Auto";
                }

                case 8:
                {
                    return "Aperture Priority";
                }

                case 32:
                {
                    return "Manual";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetBurstModeDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagBurstMode, "Off", "On");
        }

        [CanBeNull]
        public virtual string GetQualityDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagQuality, 1, "Fine", "Normal");
        }
    }
}
