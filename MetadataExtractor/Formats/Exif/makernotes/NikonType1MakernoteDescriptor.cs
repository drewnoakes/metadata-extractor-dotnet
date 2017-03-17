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
    /// Provides human-readable string representations of tag values stored in a <see cref="NikonType1MakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Type-1 is for E-Series cameras prior to (not including) E990.  For example: E700, E800, E900,
    /// E900S, E910, E950.
    /// <para />
    /// Makernote starts from ASCII string "Nikon". Data format is the same as IFD, but it starts from
    /// offset 0x08. This is the same as Olympus except start string. Example of actual data
    /// structure is shown below.
    /// <pre><c>
    /// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
    /// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
    /// </c></pre>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class NikonType1MakernoteDescriptor : TagDescriptor<NikonType1MakernoteDirectory>
    {
        public NikonType1MakernoteDescriptor([NotNull] NikonType1MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case NikonType1MakernoteDirectory.TagQuality:
                    return GetQualityDescription();
                case NikonType1MakernoteDirectory.TagColorMode:
                    return GetColorModeDescription();
                case NikonType1MakernoteDirectory.TagImageAdjustment:
                    return GetImageAdjustmentDescription();
                case NikonType1MakernoteDirectory.TagCcdSensitivity:
                    return GetCcdSensitivityDescription();
                case NikonType1MakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case NikonType1MakernoteDirectory.TagFocus:
                    return GetFocusDescription();
                case NikonType1MakernoteDirectory.TagDigitalZoom:
                    return GetDigitalZoomDescription();
                case NikonType1MakernoteDirectory.TagConverter:
                    return GetConverterDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetConverterDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagConverter,
                "None", "Fisheye converter");
        }

        [CanBeNull]
        public string GetDigitalZoomDescription()
        {
            if (!Directory.TryGetRational(NikonType1MakernoteDirectory.TagDigitalZoom, out Rational value))
                return null;
            return value.Numerator == 0 
                ? "No digital zoom" 
                : value.ToSimpleString() + "x digital zoom";
        }

        [CanBeNull]
        public string GetFocusDescription()
        {
            if (!Directory.TryGetRational(NikonType1MakernoteDirectory.TagFocus, out Rational value))
                return null;
            return value.Numerator == 1 && value.Denominator == 0 
                ? "Infinite" 
                : value.ToSimpleString();
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagWhiteBalance,
                "Auto", "Preset", "Daylight", "Incandescence", "Florescence", "Cloudy", "SpeedLight");
        }

        [CanBeNull]
        public string GetCcdSensitivityDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagCcdSensitivity,
                "ISO80", null, "ISO160", null, "ISO320", "ISO100");
        }

        [CanBeNull]
        public string GetImageAdjustmentDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagImageAdjustment,
                "Normal", "Bright +", "Bright -", "Contrast +", "Contrast -");
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagColorMode,
                1,
                "Color", "Monochrome");
        }

        [CanBeNull]
        public string GetQualityDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagQuality,
                1,
                "VGA Basic", "VGA Normal", "VGA Fine", "SXGA Basic", "SXGA Normal", "SXGA Fine");
        }
    }
}
