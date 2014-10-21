/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System.Collections.Generic;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>Describes tags specific to Sony cameras that use the Sony Type 1 makernote tags.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SonyType1MakernoteDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagCameraInfo = unchecked((int)(0x0010));

		public const int TagFocusInfo = unchecked((int)(0x0020));

		public const int TagImageQuality = unchecked((int)(0x0102));

		public const int TagFlashExposureComp = unchecked((int)(0x0104));

		public const int TagTeleconverter = unchecked((int)(0x0105));

		public const int TagWhiteBalanceFineTune = unchecked((int)(0x0112));

		public const int TagCameraSettings = unchecked((int)(0x0114));

		public const int TagWhiteBalance = unchecked((int)(0x0115));

		public const int TagExtraInfo = unchecked((int)(0x0116));

		public const int TagPrintImageMatchingInfo = unchecked((int)(0x0E00));

		public const int TagMultiBurstMode = unchecked((int)(0x1000));

		public const int TagMultiBurstImageWidth = unchecked((int)(0x1001));

		public const int TagMultiBurstImageHeight = unchecked((int)(0x1002));

		public const int TagPanorama = unchecked((int)(0x1003));

		public const int TagPreviewImage = unchecked((int)(0x2001));

		public const int TagRating = unchecked((int)(0x2002));

		public const int TagContrast = unchecked((int)(0x2004));

		public const int TagSaturation = unchecked((int)(0x2005));

		public const int TagSharpness = unchecked((int)(0x2006));

		public const int TagBrightness = unchecked((int)(0x2007));

		public const int TagLongExposureNoiseReduction = unchecked((int)(0x2008));

		public const int TagHighIsoNoiseReduction = unchecked((int)(0x2009));

		public const int TagHdr = unchecked((int)(0x200a));

		public const int TagMultiFrameNoiseReduction = unchecked((int)(0x200b));

		public const int TagPictureEffect = unchecked((int)(0x200e));

		public const int TagSoftSkinEffect = unchecked((int)(0x200f));

		public const int TagVignettingCorrection = unchecked((int)(0x2011));

		public const int TagLateralChromaticAberration = unchecked((int)(0x2012));

		public const int TagDistortionCorrection = unchecked((int)(0x2013));

		public const int TagWbShiftAmberMagenta = unchecked((int)(0x2014));

		public const int TagAutoPortraitFramed = unchecked((int)(0x2016));

		public const int TagFocusMode = unchecked((int)(0x201b));

		public const int TagAfPointSelected = unchecked((int)(0x201e));

		public const int TagShotInfo = unchecked((int)(0x3000));

		public const int TagFileFormat = unchecked((int)(0xb000));

		public const int TagSonyModelId = unchecked((int)(0xb001));

		public const int TagColorModeSetting = unchecked((int)(0xb020));

		public const int TagColorTemperature = unchecked((int)(0xb021));

		public const int TagColorCompensationFilter = unchecked((int)(0xb022));

		public const int TagSceneMode = unchecked((int)(0xb023));

		public const int TagZoneMatching = unchecked((int)(0xb024));

		public const int TagDynamicRangeOptimiser = unchecked((int)(0xb025));

		public const int TagImageStabilisation = unchecked((int)(0xb026));

		public const int TagLensId = unchecked((int)(0xb027));

		public const int TagMinoltaMakernote = unchecked((int)(0xb028));

		public const int TagColorMode = unchecked((int)(0xb029));

		public const int TagLensSpec = unchecked((int)(0xb02a));

		public const int TagFullImageSize = unchecked((int)(0xb02b));

		public const int TagPreviewImageSize = unchecked((int)(0xb02c));

		public const int TagMacro = unchecked((int)(0xb040));

		public const int TagExposureMode = unchecked((int)(0xb041));

		public const int TagFocusMode2 = unchecked((int)(0xb042));

		public const int TagAfMode = unchecked((int)(0xb043));

		public const int TagAfIlluminator = unchecked((int)(0xb044));

		public const int TagJpegQuality = unchecked((int)(0xb047));

		public const int TagFlashLevel = unchecked((int)(0xb048));

		public const int TagReleaseMode = unchecked((int)(0xb049));

		public const int TagSequenceNumber = unchecked((int)(0xb04a));

		public const int TagAntiBlur = unchecked((int)(0xb04b));

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
		public const int TagLongExposureNoiseReductionOrFocusMode = unchecked((int)(0xb04e));

		public const int TagDynamicRangeOptimizer = unchecked((int)(0xb04f));

		public const int TagHighIsoNoiseReduction2 = unchecked((int)(0xb050));

		public const int TagIntelligentAuto = unchecked((int)(0xb052));

		public const int TagWhiteBalance2 = unchecked((int)(0xb054));

		public const int TagNoPrint = unchecked((int)(0xFFFF));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static SonyType1MakernoteDirectory()
		{
			_tagNameMap.Put(TagCameraInfo, "Camera Info");
			_tagNameMap.Put(TagFocusInfo, "Focus Info");
			_tagNameMap.Put(TagImageQuality, "Image Quality");
			_tagNameMap.Put(TagFlashExposureComp, "Flash Exposure Compensation");
			_tagNameMap.Put(TagTeleconverter, "Teleconverter Model");
			_tagNameMap.Put(TagWhiteBalanceFineTune, "White Balance Fine Tune Value");
			_tagNameMap.Put(TagCameraSettings, "Camera Settings");
			_tagNameMap.Put(TagWhiteBalance, "White Balance");
			_tagNameMap.Put(TagExtraInfo, "Extra Info");
			_tagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching Info");
			_tagNameMap.Put(TagMultiBurstMode, "Multi Burst Mode");
			_tagNameMap.Put(TagMultiBurstImageWidth, "Multi Burst Image Width");
			_tagNameMap.Put(TagMultiBurstImageHeight, "Multi Burst Image Height");
			_tagNameMap.Put(TagPanorama, "Panorama");
			_tagNameMap.Put(TagPreviewImage, "Preview Image");
			_tagNameMap.Put(TagRating, "Rating");
			_tagNameMap.Put(TagContrast, "Contrast");
			_tagNameMap.Put(TagSaturation, "Saturation");
			_tagNameMap.Put(TagSharpness, "Sharpness");
			_tagNameMap.Put(TagBrightness, "Brightness");
			_tagNameMap.Put(TagLongExposureNoiseReduction, "Long Exposure Noise Reduction");
			_tagNameMap.Put(TagHighIsoNoiseReduction, "High ISO Noise Reduction");
			_tagNameMap.Put(TagHdr, "HDR");
			_tagNameMap.Put(TagMultiFrameNoiseReduction, "Multi Frame Noise Reduction");
			_tagNameMap.Put(TagPictureEffect, "Picture Effect");
			_tagNameMap.Put(TagSoftSkinEffect, "Soft Skin Effect");
			_tagNameMap.Put(TagVignettingCorrection, "Vignetting Correction");
			_tagNameMap.Put(TagLateralChromaticAberration, "Lateral Chromatic Aberration");
			_tagNameMap.Put(TagDistortionCorrection, "Distortion Correction");
			_tagNameMap.Put(TagWbShiftAmberMagenta, "WB Shift Amber/Magenta");
			_tagNameMap.Put(TagAutoPortraitFramed, "Auto Portrait Framing");
			_tagNameMap.Put(TagFocusMode, "Focus Mode");
			_tagNameMap.Put(TagAfPointSelected, "AF Point Selected");
			_tagNameMap.Put(TagShotInfo, "Shot Info");
			_tagNameMap.Put(TagFileFormat, "File Format");
			_tagNameMap.Put(TagSonyModelId, "Sony Model ID");
			_tagNameMap.Put(TagColorModeSetting, "Color Mode Setting");
			_tagNameMap.Put(TagColorTemperature, "Color Temperature");
			_tagNameMap.Put(TagColorCompensationFilter, "Color Compensation Filter");
			_tagNameMap.Put(TagSceneMode, "Scene Mode");
			_tagNameMap.Put(TagZoneMatching, "Zone Matching");
			_tagNameMap.Put(TagDynamicRangeOptimiser, "Dynamic Range Optimizer");
			_tagNameMap.Put(TagImageStabilisation, "Image Stabilisation");
			_tagNameMap.Put(TagLensId, "Lens ID");
			_tagNameMap.Put(TagMinoltaMakernote, "Minolta Makernote");
			_tagNameMap.Put(TagColorMode, "Color Mode");
			_tagNameMap.Put(TagLensSpec, "Lens Spec");
			_tagNameMap.Put(TagFullImageSize, "Full Image Size");
			_tagNameMap.Put(TagPreviewImageSize, "Preview Image Size");
			_tagNameMap.Put(TagMacro, "Macro");
			_tagNameMap.Put(TagExposureMode, "Exposure Mode");
			_tagNameMap.Put(TagFocusMode2, "Focus Mode");
			_tagNameMap.Put(TagAfMode, "AF Mode");
			_tagNameMap.Put(TagAfIlluminator, "AF Illuminator");
			_tagNameMap.Put(TagJpegQuality, "Quality");
			_tagNameMap.Put(TagFlashLevel, "Flash Level");
			_tagNameMap.Put(TagReleaseMode, "Release Mode");
			_tagNameMap.Put(TagSequenceNumber, "Sequence Number");
			_tagNameMap.Put(TagAntiBlur, "Anti Blur");
			_tagNameMap.Put(TagLongExposureNoiseReductionOrFocusMode, "Long Exposure Noise Reduction");
			_tagNameMap.Put(TagDynamicRangeOptimizer, "Dynamic Range Optimizer");
			_tagNameMap.Put(TagHighIsoNoiseReduction2, "High ISO Noise Reduction");
			_tagNameMap.Put(TagIntelligentAuto, "Intelligent Auto");
			_tagNameMap.Put(TagWhiteBalance2, "White Balance 2");
			_tagNameMap.Put(TagNoPrint, "No Print");
		}

		public SonyType1MakernoteDirectory()
		{
			this.SetDescriptor(new SonyType1MakernoteDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Sony Makernote";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
