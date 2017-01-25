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
    /// These tags are found only in ORF images of some models (eg. C8080WZ)
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusRawInfoMakernoteDirectory : Directory
    {
        public const int TagRawInfoVersion = 0x0000;
        public const int TagWbRbLevelsUsed = 0x0100;
        public const int TagWbRbLevelsAuto = 0x0110;
        public const int TagWbRbLevelsShade = 0x0120;
        public const int TagWbRbLevelsCloudy = 0x0121;
        public const int TagWbRbLevelsFineWeather = 0x0122;
        public const int TagWbRbLevelsTungsten = 0x0123;
        public const int TagWbRbLevelsEveningSunlight = 0x0124;
        public const int TagWbRbLevelsDaylightFluor = 0x0130;
        public const int TagWbRbLevelsDayWhiteFluor = 0x0131;
        public const int TagWbRbLevelsCoolWhiteFluor = 0x0132;
        public const int TagWbRbLevelsWhiteFluorescent = 0x0133;

        public const int TagColorMatrix2 = 0x0200;
        public const int TagCoringFilter = 0x0310;
        public const int TagCoringValues = 0x0311;
        public const int TagBlackLevel2 = 0x0600;
        public const int TagYCbCrCoefficients = 0x0601;
        public const int TagValidPixelDepth = 0x0611;
        public const int TagCropLeft = 0x0612;
        public const int TagCropTop = 0x0613;
        public const int TagCropWidth = 0x0614;
        public const int TagCropHeight = 0x0615;

        public const int TagLightSource = 0x1000;

        //the following 5 tags all have 3 values: val, min, max
        public const int TagWhiteBalanceComp = 0x1001;
        public const int TagSaturationSetting = 0x1010;
        public const int TagHueSetting = 0x1011;
        public const int TagContrastSetting = 0x1012;
        public const int TagSharpnessSetting = 0x1013;

        // settings written by Camedia Master 4.x
        public const int TagCmExposureCompensation = 0x2000;
        public const int TagCmWhiteBalance = 0x2001;
        public const int TagCmWhiteBalanceComp = 0x2002;
        public const int TagCmWhiteBalanceGrayPoint = 0x2010;
        public const int TagCmSaturation = 0x2020;
        public const int TagCmHue = 0x2021;
        public const int TagCmContrast = 0x2022;
        public const int TagCmSharpness = 0x2023;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagRawInfoVersion, "Raw Info Version" },
            { TagWbRbLevelsUsed, "WB RB Levels Used" },
            { TagWbRbLevelsAuto, "WB RB Levels Auto" },
            { TagWbRbLevelsShade, "WB RB Levels Shade" },
            { TagWbRbLevelsCloudy, "WB RB Levels Cloudy" },
            { TagWbRbLevelsFineWeather, "WB RB Levels Fine Weather" },
            { TagWbRbLevelsTungsten, "WB RB Levels Tungsten" },
            { TagWbRbLevelsEveningSunlight, "WB RB Levels Evening Sunlight" },
            { TagWbRbLevelsDaylightFluor, "WB RB Levels Daylight Fluor" },
            { TagWbRbLevelsDayWhiteFluor, "WB RB Levels Day White Fluor" },
            { TagWbRbLevelsCoolWhiteFluor, "WB RB Levels Cool White Fluor" },
            { TagWbRbLevelsWhiteFluorescent, "WB RB Levels White Fluorescent" },
            { TagColorMatrix2, "Color Matrix 2" },
            { TagCoringFilter, "Coring Filter" },
            { TagCoringValues, "Coring Values" },
            { TagBlackLevel2, "Black Level 2" },
            { TagYCbCrCoefficients, "YCbCrCoefficients" },
            { TagValidPixelDepth, "Valid Pixel Depth" },
            { TagCropLeft, "Crop Left" },
            { TagCropTop, "Crop Top" },
            { TagCropWidth, "Crop Width" },
            { TagCropHeight, "Crop Height" },
            { TagLightSource, "Light Source" },

            { TagWhiteBalanceComp, "White Balance Comp" },
            { TagSaturationSetting, "Saturation Setting" },
            { TagHueSetting, "Hue Setting" },
            { TagContrastSetting, "Contrast Setting" },
            { TagSharpnessSetting, "Sharpness Setting" },

            { TagCmExposureCompensation, "CM Exposure Compensation" },
            { TagCmWhiteBalance, "CM White Balance" },
            { TagCmWhiteBalanceComp, "CM White Balance Comp" },
            { TagCmWhiteBalanceGrayPoint, "CM White Balance Gray Point" },
            { TagCmSaturation, "CM Saturation" },
            { TagCmHue, "CM Hue" },
            { TagCmContrast, "CM Contrast" },
            { TagCmSharpness, "CM Sharpness" }
        };

        public OlympusRawInfoMakernoteDirectory()
        {
            SetDescriptor(new OlympusRawInfoMakernoteDescriptor(this));
        }

        public override string Name => "Olympus Raw Info";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
