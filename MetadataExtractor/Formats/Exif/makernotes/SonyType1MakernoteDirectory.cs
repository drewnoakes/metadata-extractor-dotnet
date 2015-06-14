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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sony cameras that use the Sony Type 1 makernote tags.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
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

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static SonyType1MakernoteDirectory()
        {
            TagNameMap[TagCameraInfo] = "Camera Info";
            TagNameMap[TagFocusInfo] = "Focus Info";
            TagNameMap[TagImageQuality] = "Image Quality";
            TagNameMap[TagFlashExposureComp] = "Flash Exposure Compensation";
            TagNameMap[TagTeleconverter] = "Teleconverter Model";
            TagNameMap[TagWhiteBalanceFineTune] = "White Balance Fine Tune Value";
            TagNameMap[TagCameraSettings] = "Camera Settings";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagExtraInfo] = "Extra Info";
            TagNameMap[TagPrintImageMatchingInfo] = "Print Image Matching Info";
            TagNameMap[TagMultiBurstMode] = "Multi Burst Mode";
            TagNameMap[TagMultiBurstImageWidth] = "Multi Burst Image Width";
            TagNameMap[TagMultiBurstImageHeight] = "Multi Burst Image Height";
            TagNameMap[TagPanorama] = "Panorama";
            TagNameMap[TagPreviewImage] = "Preview Image";
            TagNameMap[TagRating] = "Rating";
            TagNameMap[TagContrast] = "Contrast";
            TagNameMap[TagSaturation] = "Saturation";
            TagNameMap[TagSharpness] = "Sharpness";
            TagNameMap[TagBrightness] = "Brightness";
            TagNameMap[TagLongExposureNoiseReduction] = "Long Exposure Noise Reduction";
            TagNameMap[TagHighIsoNoiseReduction] = "High ISO Noise Reduction";
            TagNameMap[TagHdr] = "HDR";
            TagNameMap[TagMultiFrameNoiseReduction] = "Multi Frame Noise Reduction";
            TagNameMap[TagPictureEffect] = "Picture Effect";
            TagNameMap[TagSoftSkinEffect] = "Soft Skin Effect";
            TagNameMap[TagVignettingCorrection] = "Vignetting Correction";
            TagNameMap[TagLateralChromaticAberration] = "Lateral Chromatic Aberration";
            TagNameMap[TagDistortionCorrection] = "Distortion Correction";
            TagNameMap[TagWbShiftAmberMagenta] = "WB Shift Amber/Magenta";
            TagNameMap[TagAutoPortraitFramed] = "Auto Portrait Framing";
            TagNameMap[TagFocusMode] = "Focus Mode";
            TagNameMap[TagAfPointSelected] = "AF Point Selected";
            TagNameMap[TagShotInfo] = "Shot Info";
            TagNameMap[TagFileFormat] = "File Format";
            TagNameMap[TagSonyModelId] = "Sony Model ID";
            TagNameMap[TagColorModeSetting] = "Color Mode Setting";
            TagNameMap[TagColorTemperature] = "Color Temperature";
            TagNameMap[TagColorCompensationFilter] = "Color Compensation Filter";
            TagNameMap[TagSceneMode] = "Scene Mode";
            TagNameMap[TagZoneMatching] = "Zone Matching";
            TagNameMap[TagDynamicRangeOptimiser] = "Dynamic Range Optimizer";
            TagNameMap[TagImageStabilisation] = "Image Stabilisation";
            TagNameMap[TagLensId] = "Lens ID";
            TagNameMap[TagMinoltaMakernote] = "Minolta Makernote";
            TagNameMap[TagColorMode] = "Color Mode";
            TagNameMap[TagLensSpec] = "Lens Spec";
            TagNameMap[TagFullImageSize] = "Full Image Size";
            TagNameMap[TagPreviewImageSize] = "Preview Image Size";
            TagNameMap[TagMacro] = "Macro";
            TagNameMap[TagExposureMode] = "Exposure Mode";
            TagNameMap[TagFocusMode2] = "Focus Mode";
            TagNameMap[TagAfMode] = "AF Mode";
            TagNameMap[TagAfIlluminator] = "AF Illuminator";
            TagNameMap[TagJpegQuality] = "Quality";
            TagNameMap[TagFlashLevel] = "Flash Level";
            TagNameMap[TagReleaseMode] = "Release Mode";
            TagNameMap[TagSequenceNumber] = "Sequence Number";
            TagNameMap[TagAntiBlur] = "Anti Blur";
            TagNameMap[TagLongExposureNoiseReductionOrFocusMode] = "Long Exposure Noise Reduction";
            TagNameMap[TagDynamicRangeOptimizer] = "Dynamic Range Optimizer";
            TagNameMap[TagHighIsoNoiseReduction2] = "High ISO Noise Reduction";
            TagNameMap[TagIntelligentAuto] = "Intelligent Auto";
            TagNameMap[TagWhiteBalance2] = "White Balance 2";
            TagNameMap[TagNoPrint] = "No Print";
        }

        public SonyType1MakernoteDirectory()
        {
            SetDescriptor(new SonyType1MakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Sony Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
