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
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sony cameras that use the Sony Type 1 makernote tags.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SonyType1MakernoteDirectory : Directory
    {
        public const int TagCameraInfo = unchecked(0x0010);

        public const int TagFocusInfo = unchecked(0x0020);

        public const int TagImageQuality = unchecked(0x0102);

        public const int TagFlashExposureComp = unchecked(0x0104);

        public const int TagTeleconverter = unchecked(0x0105);

        public const int TagWhiteBalanceFineTune = unchecked(0x0112);

        public const int TagCameraSettings = unchecked(0x0114);

        public const int TagWhiteBalance = unchecked(0x0115);

        public const int TagExtraInfo = unchecked(0x0116);

        public const int TagPrintImageMatchingInfo = unchecked(0x0E00);

        public const int TagMultiBurstMode = unchecked(0x1000);

        public const int TagMultiBurstImageWidth = unchecked(0x1001);

        public const int TagMultiBurstImageHeight = unchecked(0x1002);

        public const int TagPanorama = unchecked(0x1003);

        public const int TagPreviewImage = unchecked(0x2001);

        public const int TagRating = unchecked(0x2002);

        public const int TagContrast = unchecked(0x2004);

        public const int TagSaturation = unchecked(0x2005);

        public const int TagSharpness = unchecked(0x2006);

        public const int TagBrightness = unchecked(0x2007);

        public const int TagLongExposureNoiseReduction = unchecked(0x2008);

        public const int TagHighIsoNoiseReduction = unchecked(0x2009);

        public const int TagHdr = unchecked(0x200a);

        public const int TagMultiFrameNoiseReduction = unchecked(0x200b);

        public const int TagPictureEffect = unchecked(0x200e);

        public const int TagSoftSkinEffect = unchecked(0x200f);

        public const int TagVignettingCorrection = unchecked(0x2011);

        public const int TagLateralChromaticAberration = unchecked(0x2012);

        public const int TagDistortionCorrection = unchecked(0x2013);

        public const int TagWbShiftAmberMagenta = unchecked(0x2014);

        public const int TagAutoPortraitFramed = unchecked(0x2016);

        public const int TagFocusMode = unchecked(0x201b);

        public const int TagAfPointSelected = unchecked(0x201e);

        public const int TagShotInfo = unchecked(0x3000);

        public const int TagFileFormat = unchecked(0xb000);

        public const int TagSonyModelId = unchecked(0xb001);

        public const int TagColorModeSetting = unchecked(0xb020);

        public const int TagColorTemperature = unchecked(0xb021);

        public const int TagColorCompensationFilter = unchecked(0xb022);

        public const int TagSceneMode = unchecked(0xb023);

        public const int TagZoneMatching = unchecked(0xb024);

        public const int TagDynamicRangeOptimiser = unchecked(0xb025);

        public const int TagImageStabilisation = unchecked(0xb026);

        public const int TagLensId = unchecked(0xb027);

        public const int TagMinoltaMakernote = unchecked(0xb028);

        public const int TagColorMode = unchecked(0xb029);

        public const int TagLensSpec = unchecked(0xb02a);

        public const int TagFullImageSize = unchecked(0xb02b);

        public const int TagPreviewImageSize = unchecked(0xb02c);

        public const int TagMacro = unchecked(0xb040);

        public const int TagExposureMode = unchecked(0xb041);

        public const int TagFocusMode2 = unchecked(0xb042);

        public const int TagAfMode = unchecked(0xb043);

        public const int TagAfIlluminator = unchecked(0xb044);

        public const int TagJpegQuality = unchecked(0xb047);

        public const int TagFlashLevel = unchecked(0xb048);

        public const int TagReleaseMode = unchecked(0xb049);

        public const int TagSequenceNumber = unchecked(0xb04a);

        public const int TagAntiBlur = unchecked(0xb04b);

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
        public const int TagLongExposureNoiseReductionOrFocusMode = unchecked(0xb04e);

        public const int TagDynamicRangeOptimizer = unchecked(0xb04f);

        public const int TagHighIsoNoiseReduction2 = unchecked(0xb050);

        public const int TagIntelligentAuto = unchecked(0xb052);

        public const int TagWhiteBalance2 = unchecked(0xb054);

        public const int TagNoPrint = unchecked(0xFFFF);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static SonyType1MakernoteDirectory()
        {
            TagNameMap.Put(TagCameraInfo, "Camera Info");
            TagNameMap.Put(TagFocusInfo, "Focus Info");
            TagNameMap.Put(TagImageQuality, "Image Quality");
            TagNameMap.Put(TagFlashExposureComp, "Flash Exposure Compensation");
            TagNameMap.Put(TagTeleconverter, "Teleconverter Model");
            TagNameMap.Put(TagWhiteBalanceFineTune, "White Balance Fine Tune Value");
            TagNameMap.Put(TagCameraSettings, "Camera Settings");
            TagNameMap.Put(TagWhiteBalance, "White Balance");
            TagNameMap.Put(TagExtraInfo, "Extra Info");
            TagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching Info");
            TagNameMap.Put(TagMultiBurstMode, "Multi Burst Mode");
            TagNameMap.Put(TagMultiBurstImageWidth, "Multi Burst Image Width");
            TagNameMap.Put(TagMultiBurstImageHeight, "Multi Burst Image Height");
            TagNameMap.Put(TagPanorama, "Panorama");
            TagNameMap.Put(TagPreviewImage, "Preview Image");
            TagNameMap.Put(TagRating, "Rating");
            TagNameMap.Put(TagContrast, "Contrast");
            TagNameMap.Put(TagSaturation, "Saturation");
            TagNameMap.Put(TagSharpness, "Sharpness");
            TagNameMap.Put(TagBrightness, "Brightness");
            TagNameMap.Put(TagLongExposureNoiseReduction, "Long Exposure Noise Reduction");
            TagNameMap.Put(TagHighIsoNoiseReduction, "High ISO Noise Reduction");
            TagNameMap.Put(TagHdr, "HDR");
            TagNameMap.Put(TagMultiFrameNoiseReduction, "Multi Frame Noise Reduction");
            TagNameMap.Put(TagPictureEffect, "Picture Effect");
            TagNameMap.Put(TagSoftSkinEffect, "Soft Skin Effect");
            TagNameMap.Put(TagVignettingCorrection, "Vignetting Correction");
            TagNameMap.Put(TagLateralChromaticAberration, "Lateral Chromatic Aberration");
            TagNameMap.Put(TagDistortionCorrection, "Distortion Correction");
            TagNameMap.Put(TagWbShiftAmberMagenta, "WB Shift Amber/Magenta");
            TagNameMap.Put(TagAutoPortraitFramed, "Auto Portrait Framing");
            TagNameMap.Put(TagFocusMode, "Focus Mode");
            TagNameMap.Put(TagAfPointSelected, "AF Point Selected");
            TagNameMap.Put(TagShotInfo, "Shot Info");
            TagNameMap.Put(TagFileFormat, "File Format");
            TagNameMap.Put(TagSonyModelId, "Sony Model ID");
            TagNameMap.Put(TagColorModeSetting, "Color Mode Setting");
            TagNameMap.Put(TagColorTemperature, "Color Temperature");
            TagNameMap.Put(TagColorCompensationFilter, "Color Compensation Filter");
            TagNameMap.Put(TagSceneMode, "Scene Mode");
            TagNameMap.Put(TagZoneMatching, "Zone Matching");
            TagNameMap.Put(TagDynamicRangeOptimiser, "Dynamic Range Optimizer");
            TagNameMap.Put(TagImageStabilisation, "Image Stabilisation");
            TagNameMap.Put(TagLensId, "Lens ID");
            TagNameMap.Put(TagMinoltaMakernote, "Minolta Makernote");
            TagNameMap.Put(TagColorMode, "Color Mode");
            TagNameMap.Put(TagLensSpec, "Lens Spec");
            TagNameMap.Put(TagFullImageSize, "Full Image Size");
            TagNameMap.Put(TagPreviewImageSize, "Preview Image Size");
            TagNameMap.Put(TagMacro, "Macro");
            TagNameMap.Put(TagExposureMode, "Exposure Mode");
            TagNameMap.Put(TagFocusMode2, "Focus Mode");
            TagNameMap.Put(TagAfMode, "AF Mode");
            TagNameMap.Put(TagAfIlluminator, "AF Illuminator");
            TagNameMap.Put(TagJpegQuality, "Quality");
            TagNameMap.Put(TagFlashLevel, "Flash Level");
            TagNameMap.Put(TagReleaseMode, "Release Mode");
            TagNameMap.Put(TagSequenceNumber, "Sequence Number");
            TagNameMap.Put(TagAntiBlur, "Anti Blur");
            TagNameMap.Put(TagLongExposureNoiseReductionOrFocusMode, "Long Exposure Noise Reduction");
            TagNameMap.Put(TagDynamicRangeOptimizer, "Dynamic Range Optimizer");
            TagNameMap.Put(TagHighIsoNoiseReduction2, "High ISO Noise Reduction");
            TagNameMap.Put(TagIntelligentAuto, "Intelligent Auto");
            TagNameMap.Put(TagWhiteBalance2, "White Balance 2");
            TagNameMap.Put(TagNoPrint, "No Print");
        }

        public SonyType1MakernoteDirectory()
        {
            SetDescriptor(new SonyType1MakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Sony Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
