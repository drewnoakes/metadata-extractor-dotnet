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
    /// Provides human-readable string representations of tag values stored in a <see cref="KodakMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class KodakMakernoteDescriptor : TagDescriptor<KodakMakernoteDirectory>
    {
        public KodakMakernoteDescriptor([NotNull] KodakMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case KodakMakernoteDirectory.TagQuality:
                    return GetQualityDescription();
                case KodakMakernoteDirectory.TagBurstMode:
                    return GetBurstModeDescription();
                case KodakMakernoteDirectory.TagShutterMode:
                    return GetShutterModeDescription();
                case KodakMakernoteDirectory.TagFocusMode:
                    return GetFocusModeDescription();
                case KodakMakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case KodakMakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                case KodakMakernoteDirectory.TagFlashFired:
                    return GetFlashFiredDescription();
                case KodakMakernoteDirectory.TagColorMode:
                    return GetColorModeDescription();
                case KodakMakernoteDirectory.TagSharpness:
                    return GetSharpnessDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagSharpness, "Normal");
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagColorMode, out int value))
                return null;

            switch (value)
            {
                case 0x0001:
                case 0x2000:
                    return "B&W";
                case 0x0002:
                case 0x4000:
                    return "Sepia";
                case 0x003:
                    return "B&W Yellow Filter";
                case 0x004:
                    return "B&W Red Filter";
                case 0x020:
                    return "Saturated Color";
                case 0x040:
                case 0x200:
                    return "Neutral Color";
                case 0x100:
                    return "Saturated Color";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetFlashFiredDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFlashFired, "No", "Yes");
        }

        [CanBeNull]
        public string GetFlashModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagFlashMode, out int value))
                return null;

            switch (value)
            {
                case 0x00:
                    return "Auto";
                case 0x10:
                case 0x01:
                    return "Fill Flash";
                case 0x20:
                case 0x02:
                    return "Off";
                case 0x40:
                case 0x03:
                    return "Red Eye";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagWhiteBalance,
                "Auto", "Flash", "Tungsten", "Daylight");
        }

        [CanBeNull]
        public string GetFocusModeDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFocusMode,
                "Normal", null, "Macro");
        }

        [CanBeNull]
        public string GetShutterModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagShutterMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Auto";
                case 8:
                    return "Aperture Priority";
                case 32:
                    return "Manual";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetBurstModeDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagBurstMode,
                "Off", "On");
        }

        [CanBeNull]
        public string GetQualityDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagQuality,
                1,
                "Fine", "Normal");
        }
    }
}
