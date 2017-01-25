#region License
//
// Copyright 2002-2017 Drew Noakes
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// The Olympus raw development makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusRawDevelopmentMakernoteDirectory : Directory
    {
        public const int TagRawDevVersion = 0x0000;
        public const int TagRawDevExposureBiasValue = 0x0100;
        public const int TagRawDevWhiteBalanceValue = 0x0101;
        public const int TagRawDevWbFineAdjustment = 0x0102;
        public const int TagRawDevGrayPoint = 0x0103;
        public const int TagRawDevSaturationEmphasis = 0x0104;
        public const int TagRawDevMemoryColorEmphasis = 0x0105;
        public const int TagRawDevContrastValue = 0x0106;
        public const int TagRawDevSharpnessValue = 0x0107;
        public const int TagRawDevColorSpace = 0x0108;
        public const int TagRawDevEngine = 0x0109;
        public const int TagRawDevNoiseReduction = 0x010a;
        public const int TagRawDevEditStatus = 0x010b;
        public const int TagRawDevSettings = 0x010c;


        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagRawDevVersion, "Raw Dev Version" },
            { TagRawDevExposureBiasValue, "Raw Dev Exposure Bias Value" },
            { TagRawDevWhiteBalanceValue, "Raw Dev White Balance Value" },
            { TagRawDevWbFineAdjustment, "Raw Dev WB Fine Adjustment" },
            { TagRawDevGrayPoint, "Raw Dev Gray Point" },
            { TagRawDevSaturationEmphasis, "Raw Dev Saturation Emphasis" },
            { TagRawDevMemoryColorEmphasis, "Raw Dev Memory Color Emphasis" },
            { TagRawDevContrastValue, "Raw Dev Contrast Value" },
            { TagRawDevSharpnessValue, "Raw Dev Sharpness Value" },
            { TagRawDevColorSpace, "Raw Dev Color Space" },
            { TagRawDevEngine, "Raw Dev Engine" },
            { TagRawDevNoiseReduction, "Raw Dev Noise Reduction" },
            { TagRawDevEditStatus, "Raw Dev Edit Status" },
            { TagRawDevSettings, "Raw Dev Settings" }
    };

        public OlympusRawDevelopmentMakernoteDirectory()
        {
            SetDescriptor(new OlympusRawDevelopmentMakernoteDescriptor(this));
        }

        public override string Name => "Olympus Raw Development";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
