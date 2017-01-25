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
    /// The Olympus raw development 2 makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusRawDevelopment2MakernoteDirectory : Directory
    {
        public const int TagRawDevVersion = 0x0000;
        public const int TagRawDevExposureBiasValue = 0x0100;
        public const int TagRawDevWhiteBalance = 0x0101;
        public const int TagRawDevWhiteBalanceValue = 0x0102;
        public const int TagRawDevWbFineAdjustment = 0x0103;
        public const int TagRawDevGrayPoint = 0x0104;
        public const int TagRawDevContrastValue = 0x0105;
        public const int TagRawDevSharpnessValue = 0x0106;
        public const int TagRawDevSaturationEmphasis = 0x0107;
        public const int TagRawDevMemoryColorEmphasis = 0x0108;
        public const int TagRawDevColorSpace = 0x0109;
        public const int TagRawDevNoiseReduction = 0x010a;
        public const int TagRawDevEngine = 0x010b;
        public const int TagRawDevPictureMode = 0x010c;
        public const int TagRawDevPmSaturation = 0x010d;
        public const int TagRawDevPmContrast = 0x010e;
        public const int TagRawDevPmSharpness = 0x010f;
        public const int TagRawDevPmBwFilter = 0x0110;
        public const int TagRawDevPmPictureTone = 0x0111;
        public const int TagRawDevGradation = 0x0112;
        public const int TagRawDevSaturation3 = 0x0113;
        public const int TagRawDevAutoGradation = 0x0119;
        public const int TagRawDevPmNoiseFilter = 0x0120;
        public const int TagRawDevArtFilter = 0x0121;


        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagRawDevVersion, "Raw Dev Version" },
            { TagRawDevExposureBiasValue, "Raw Dev Exposure Bias Value" },
            { TagRawDevWhiteBalance, "Raw Dev White Balance" },
            { TagRawDevWhiteBalanceValue, "Raw Dev White Balance Value" },
            { TagRawDevWbFineAdjustment, "Raw Dev WB Fine Adjustment" },
            { TagRawDevGrayPoint, "Raw Dev Gray Point" },
            { TagRawDevContrastValue, "Raw Dev Contrast Value" },
            { TagRawDevSharpnessValue, "Raw Dev Sharpness Value" },
            { TagRawDevSaturationEmphasis, "Raw Dev Saturation Emphasis" },
            { TagRawDevMemoryColorEmphasis, "Raw Dev Memory Color Emphasis" },
            { TagRawDevColorSpace, "Raw Dev Color Space" },
            { TagRawDevNoiseReduction, "Raw Dev Noise Reduction" },
            { TagRawDevEngine, "Raw Dev Engine" },
            { TagRawDevPictureMode, "Raw Dev Picture Mode" },
            { TagRawDevPmSaturation, "Raw Dev PM Saturation" },
            { TagRawDevPmContrast, "Raw Dev PM Contrast" },
            { TagRawDevPmSharpness, "Raw Dev PM Sharpness" },
            { TagRawDevPmBwFilter, "Raw Dev PM BW Filter" },
            { TagRawDevPmPictureTone, "Raw Dev PM Picture Tone" },
            { TagRawDevGradation, "Raw Dev Gradation" },
            { TagRawDevSaturation3, "Raw Dev Saturation 3" },
            { TagRawDevAutoGradation, "Raw Dev Auto Gradation" },
            { TagRawDevPmNoiseFilter, "Raw Dev PM Noise Filter" },
            { TagRawDevArtFilter, "Raw Dev Art Filter" }
        };

        public OlympusRawDevelopment2MakernoteDirectory()
        {
            SetDescriptor(new OlympusRawDevelopment2MakernoteDescriptor(this));
        }

        public override string Name => "Olympus Raw Development 2";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
