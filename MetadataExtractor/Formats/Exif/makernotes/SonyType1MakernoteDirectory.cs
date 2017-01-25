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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sony cameras that use the Sony Type 1 makernote tags.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SonyType1MakernoteDirectory : Directory
    {
        public const int TagCameraInfo = 0x0010;
        public const int TagFocusInfo = 0x0020;
        public const int TagImageQuality = 0x0102;
        public const int TagFlashExposureComp = 0x0104;
        public const int TagTeleconverter = 0x0105;
        public const int TagWhiteBalanceFineTune = 0x0112;
        public const int TagCameraSettings = 0x0114;
        public const int TagWhiteBalance = 0x0115;
        public const int TagExtraInfo = 0x0116;
        public const int TagPrintImageMatchingInfo = 0x0E00;
        public const int TagMultiBurstMode = 0x1000;
        public const int TagMultiBurstImageWidth = 0x1001;
        public const int TagMultiBurstImageHeight = 0x1002;
        public const int TagPanorama = 0x1003;
        public const int TagPreviewImage = 0x2001;
        public const int TagRating = 0x2002;
        public const int TagContrast = 0x2004;
        public const int TagSaturation = 0x2005;
        public const int TagSharpness = 0x2006;
        public const int TagBrightness = 0x2007;
        public const int TagLongExposureNoiseReduction = 0x2008;
        public const int TagHighIsoNoiseReduction = 0x2009;
        public const int TagHdr = 0x200a;
        public const int TagMultiFrameNoiseReduction = 0x200b;
        public const int TagPictureEffect = 0x200e;
        public const int TagSoftSkinEffect = 0x200f;
        public const int TagVignettingCorrection = 0x2011;
        public const int TagLateralChromaticAberration = 0x2012;
        public const int TagDistortionCorrection = 0x2013;
        public const int TagWbShiftAmberMagenta = 0x2014;
        public const int TagAutoPortraitFramed = 0x2016;
        public const int TagFocusMode = 0x201b;
        public const int TagAfPointSelected = 0x201e;
        public const int TagShotInfo = 0x3000;
        public const int TagFileFormat = 0xb000;
        public const int TagSonyModelId = 0xb001;
        public const int TagColorModeSetting = 0xb020;
        public const int TagColorTemperature = 0xb021;
        public const int TagColorCompensationFilter = 0xb022;
        public const int TagSceneMode = 0xb023;
        public const int TagZoneMatching = 0xb024;
        public const int TagDynamicRangeOptimiser = 0xb025;
        public const int TagImageStabilisation = 0xb026;
        public const int TagLensId = 0xb027;
        public const int TagMinoltaMakernote = 0xb028;
        public const int TagColorMode = 0xb029;
        public const int TagLensSpec = 0xb02a;
        public const int TagFullImageSize = 0xb02b;
        public const int TagPreviewImageSize = 0xb02c;
        public const int TagMacro = 0xb040;
        public const int TagExposureMode = 0xb041;
        public const int TagFocusMode2 = 0xb042;
        public const int TagAfMode = 0xb043;
        public const int TagAfIlluminator = 0xb044;
        public const int TagJpegQuality = 0xb047;
        public const int TagFlashLevel = 0xb048;
        public const int TagReleaseMode = 0xb049;
        public const int TagSequenceNumber = 0xb04a;
        public const int TagAntiBlur = 0xb04b;

        /// <summary>
        /// (FocusMode for RX100)
        /// 0 = Manual
        /// 2 = AF-S
        /// 3 = AF-C
        /// 5 = Semi-manual
        /// 6 = Direct Manual Focus
        /// (LongExposureNoiseReduction for other models)
        /// 0 = Off
        /// 1 = On
        /// 2 = On 2
        /// 65535 = n/a
        /// </summary>
        public const int TagLongExposureNoiseReductionOrFocusMode = 0xb04e;

        public const int TagDynamicRangeOptimizer = 0xb04f;
        public const int TagHighIsoNoiseReduction2 = 0xb050;
        public const int TagIntelligentAuto = 0xb052;
        public const int TagWhiteBalance2 = 0xb054;
        public const int TagNoPrint = 0xFFFF;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagCameraInfo, "Camera Info" },
            { TagFocusInfo, "Focus Info" },
            { TagImageQuality, "Image Quality" },
            { TagFlashExposureComp, "Flash Exposure Compensation" },
            { TagTeleconverter, "Teleconverter Model" },
            { TagWhiteBalanceFineTune, "White Balance Fine Tune Value" },
            { TagCameraSettings, "Camera Settings" },
            { TagWhiteBalance, "White Balance" },
            { TagExtraInfo, "Extra Info" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagMultiBurstMode, "Multi Burst Mode" },
            { TagMultiBurstImageWidth, "Multi Burst Image Width" },
            { TagMultiBurstImageHeight, "Multi Burst Image Height" },
            { TagPanorama, "Panorama" },
            { TagPreviewImage, "Preview Image" },
            { TagRating, "Rating" },
            { TagContrast, "Contrast" },
            { TagSaturation, "Saturation" },
            { TagSharpness, "Sharpness" },
            { TagBrightness, "Brightness" },
            { TagLongExposureNoiseReduction, "Long Exposure Noise Reduction" },
            { TagHighIsoNoiseReduction, "High ISO Noise Reduction" },
            { TagHdr, "HDR" },
            { TagMultiFrameNoiseReduction, "Multi Frame Noise Reduction" },
            { TagPictureEffect, "Picture Effect" },
            { TagSoftSkinEffect, "Soft Skin Effect" },
            { TagVignettingCorrection, "Vignetting Correction" },
            { TagLateralChromaticAberration, "Lateral Chromatic Aberration" },
            { TagDistortionCorrection, "Distortion Correction" },
            { TagWbShiftAmberMagenta, "WB Shift Amber/Magenta" },
            { TagAutoPortraitFramed, "Auto Portrait Framing" },
            { TagFocusMode, "Focus Mode" },
            { TagAfPointSelected, "AF Point Selected" },
            { TagShotInfo, "Shot Info" },
            { TagFileFormat, "File Format" },
            { TagSonyModelId, "Sony Model ID" },
            { TagColorModeSetting, "Color Mode Setting" },
            { TagColorTemperature, "Color Temperature" },
            { TagColorCompensationFilter, "Color Compensation Filter" },
            { TagSceneMode, "Scene Mode" },
            { TagZoneMatching, "Zone Matching" },
            { TagDynamicRangeOptimiser, "Dynamic Range Optimizer" },
            { TagImageStabilisation, "Image Stabilisation" },
            { TagLensId, "Lens ID" },
            { TagMinoltaMakernote, "Minolta Makernote" },
            { TagColorMode, "Color Mode" },
            { TagLensSpec, "Lens Spec" },
            { TagFullImageSize, "Full Image Size" },
            { TagPreviewImageSize, "Preview Image Size" },
            { TagMacro, "Macro" },
            { TagExposureMode, "Exposure Mode" },
            { TagFocusMode2, "Focus Mode" },
            { TagAfMode, "AF Mode" },
            { TagAfIlluminator, "AF Illuminator" },
            { TagJpegQuality, "Quality" },
            { TagFlashLevel, "Flash Level" },
            { TagReleaseMode, "Release Mode" },
            { TagSequenceNumber, "Sequence Number" },
            { TagAntiBlur, "Anti Blur" },
            { TagLongExposureNoiseReductionOrFocusMode, "Long Exposure Noise Reduction" },
            { TagDynamicRangeOptimizer, "Dynamic Range Optimizer" },
            { TagHighIsoNoiseReduction2, "High ISO Noise Reduction" },
            { TagIntelligentAuto, "Intelligent Auto" },
            { TagWhiteBalance2, "White Balance 2" },
            { TagNoPrint, "No Print" }
        };

        public SonyType1MakernoteDirectory()
        {
            SetDescriptor(new SonyType1MakernoteDescriptor(this));
        }

        public override string Name => "Sony Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
