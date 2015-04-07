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

namespace Com.Drew.Metadata.Photoshop
{
	/// <summary>Holds the metadata found in the APPD segment of a JPEG file saved by Photoshop.</summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	/// <author>Yuri Binev</author>
	public class PhotoshopDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagChannelsRowsColumnsDepthMode = unchecked((int)(0x03E8));

		public const int TagMacPrintInfo = unchecked((int)(0x03E9));

		public const int TagXml = unchecked((int)(0x03EA));

		public const int TagIndexedColorTable = unchecked((int)(0x03EB));

		public const int TagResolutionInfo = unchecked((int)(0x03ED));

		public const int TagAlphaChannels = unchecked((int)(0x03EE));

		public const int TagDisplayInfoObsolete = unchecked((int)(0x03EF));

		public const int TagCaption = unchecked((int)(0x03F0));

		public const int TagBorderInformation = unchecked((int)(0x03F1));

		public const int TagBackgroundColor = unchecked((int)(0x03F2));

		public const int TagPrintFlags = unchecked((int)(0x03F3));

		public const int TagGrayscaleAndMultichannelHalftoningInformation = unchecked((int)(0x03F4));

		public const int TagColorHalftoningInformation = unchecked((int)(0x03F5));

		public const int TagDuotoneHalftoningInformation = unchecked((int)(0x03F6));

		public const int TagGrayscaleAndMultichannelTransferFunction = unchecked((int)(0x03F7));

		public const int TagColorTransferFunctions = unchecked((int)(0x03F8));

		public const int TagDuotoneTransferFunctions = unchecked((int)(0x03F9));

		public const int TagDuotoneImageInformation = unchecked((int)(0x03FA));

		public const int TagEffectiveBlackAndWhiteValues = unchecked((int)(0x03FB));

		public const int TagEpsOptions = unchecked((int)(0x03FD));

		public const int TagQuickMaskInformation = unchecked((int)(0x03FE));

		public const int TagLayerStateInformation = unchecked((int)(0x0400));

		public const int TagLayersGroupInformation = unchecked((int)(0x0402));

		public const int TagIptc = unchecked((int)(0x0404));

		public const int TagImageModeForRawFormatFiles = unchecked((int)(0x0405));

		public const int TagJpegQuality = unchecked((int)(0x0406));

		public const int TagGridAndGuidesInformation = unchecked((int)(0x0408));

		public const int TagThumbnailOld = unchecked((int)(0x0409));

		public const int TagCopyright = unchecked((int)(0x040A));

		public const int TagUrl = unchecked((int)(0x040B));

		public const int TagThumbnail = unchecked((int)(0x040C));

		public const int TagGlobalAngle = unchecked((int)(0x040D));

		public const int TagIccProfileBytes = unchecked((int)(0x040F));

		public const int TagWatermark = unchecked((int)(0x0410));

		public const int TagIccUntaggedProfile = unchecked((int)(0x0411));

		public const int TagEffectsVisible = unchecked((int)(0x0412));

		public const int TagSpotHalftone = unchecked((int)(0x0413));

		public const int TagSeedNumber = unchecked((int)(0x0414));

		public const int TagUnicodeAlphaNames = unchecked((int)(0x0415));

		public const int TagIndexedColorTableCount = unchecked((int)(0x0416));

		public const int TagTransparencyIndex = unchecked((int)(0x0417));

		public const int TagGlobalAltitude = unchecked((int)(0x0419));

		public const int TagSlices = unchecked((int)(0x041A));

		public const int TagWorkflowUrl = unchecked((int)(0x041B));

		public const int TagJumpToXpep = unchecked((int)(0x041C));

		public const int TagAlphaIdentifiers = unchecked((int)(0x041D));

		public const int TagUrlList = unchecked((int)(0x041E));

		public const int TagVersion = unchecked((int)(0x0421));

		public const int TagExifData1 = unchecked((int)(0x0422));

		public const int TagExifData3 = unchecked((int)(0x0423));

		public const int TagXmpData = unchecked((int)(0x0424));

		public const int TagCaptionDigest = unchecked((int)(0x0425));

		public const int TagPrintScale = unchecked((int)(0x0426));

		public const int TagPixelAspectRatio = unchecked((int)(0x0428));

		public const int TagLayerComps = unchecked((int)(0x0429));

		public const int TagAlternateDuotoneColors = unchecked((int)(0x042A));

		public const int TagAlternateSpotColors = unchecked((int)(0x042B));

		public const int TagLayerSelectionIds = unchecked((int)(0x042D));

		public const int TagHdrToningInfo = unchecked((int)(0x042E));

		public const int TagPrintInfo = unchecked((int)(0x042F));

		public const int TagLayerGroupsEnabledId = unchecked((int)(0x0430));

		public const int TagColorSamplers = unchecked((int)(0x0431));

		public const int TagMeasurementScale = unchecked((int)(0x0432));

		public const int TagTimelineInformation = unchecked((int)(0x0433));

		public const int TagSheetDisclosure = unchecked((int)(0x0434));

		public const int TagDisplayInfo = unchecked((int)(0x0435));

		public const int TagOnionSkins = unchecked((int)(0x0436));

		public const int TagCountInformation = unchecked((int)(0x0438));

		public const int TagPrintInfo2 = unchecked((int)(0x043A));

		public const int TagPrintStyle = unchecked((int)(0x043B));

		public const int TagMacNsprintinfo = unchecked((int)(0x043C));

		public const int TagWinDevmode = unchecked((int)(0x043D));

		public const int TagAutoSaveFilePath = unchecked((int)(0x043E));

		public const int TagAutoSaveFormat = unchecked((int)(0x043F));

		public const int TagPathSelectionState = unchecked((int)(0x0440));

		public const int TagClippingPathName = unchecked((int)(0x0BB7));

		public const int TagOriginPathInfo = unchecked((int)(0x0BB8));

		public const int TagImageReadyVariablesXml = unchecked((int)(0x1B58));

		public const int TagImageReadyDataSets = unchecked((int)(0x1B59));

		public const int TagLightroomWorkflow = unchecked((int)(0x1F40));

		public const int TagPrintFlagsInfo = unchecked((int)(0x2710));

		[NotNull]
		protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

		static PhotoshopDirectory()
		{
			// OBSOLETE                                                                     0x03FC
			// OBSOLETE                                                                     0x03FF
			// Working path (not saved)                                                     0x0401
			// OBSOLETE                                                                     0x0403
			// OBSOLETE                                                                     0x040E
			// CLIPPING PATHS                                                               0x07D0 -> 0x0BB6
			// PLUG IN RESOURCES                                                            0x0FA0 -> 0x1387
			_tagNameMap.Put(TagChannelsRowsColumnsDepthMode, "Channels, Rows, Columns, Depth, Mode");
			_tagNameMap.Put(TagMacPrintInfo, "Mac Print Info");
			_tagNameMap.Put(TagXml, "XML Data");
			_tagNameMap.Put(TagIndexedColorTable, "Indexed Color Table");
			_tagNameMap.Put(TagResolutionInfo, "Resolution Info");
			_tagNameMap.Put(TagAlphaChannels, "Alpha Channels");
			_tagNameMap.Put(TagDisplayInfoObsolete, "Display Info (Obsolete)");
			_tagNameMap.Put(TagCaption, "Caption");
			_tagNameMap.Put(TagBorderInformation, "Border Information");
			_tagNameMap.Put(TagBackgroundColor, "Background Color");
			_tagNameMap.Put(TagPrintFlags, "Print Flags");
			_tagNameMap.Put(TagGrayscaleAndMultichannelHalftoningInformation, "Grayscale and Multichannel Halftoning Information");
			_tagNameMap.Put(TagColorHalftoningInformation, "Color Halftoning Information");
			_tagNameMap.Put(TagDuotoneHalftoningInformation, "Duotone Halftoning Information");
			_tagNameMap.Put(TagGrayscaleAndMultichannelTransferFunction, "Grayscale and Multichannel Transfer Function");
			_tagNameMap.Put(TagColorTransferFunctions, "Color Transfer Functions");
			_tagNameMap.Put(TagDuotoneTransferFunctions, "Duotone Transfer Functions");
			_tagNameMap.Put(TagDuotoneImageInformation, "Duotone Image Information");
			_tagNameMap.Put(TagEffectiveBlackAndWhiteValues, "Effective Black and White Values");
			_tagNameMap.Put(TagEpsOptions, "EPS Options");
			_tagNameMap.Put(TagQuickMaskInformation, "Quick Mask Information");
			_tagNameMap.Put(TagLayerStateInformation, "Layer State Information");
			_tagNameMap.Put(TagLayersGroupInformation, "Layers Group Information");
			_tagNameMap.Put(TagIptc, "IPTC-NAA Record");
			_tagNameMap.Put(TagImageModeForRawFormatFiles, "Image Mode for Raw Format Files");
			_tagNameMap.Put(TagJpegQuality, "JPEG Quality");
			_tagNameMap.Put(TagGridAndGuidesInformation, "Grid and Guides Information");
			_tagNameMap.Put(TagThumbnailOld, "Photoshop 4.0 Thumbnail");
			_tagNameMap.Put(TagCopyright, "Copyright Flag");
			_tagNameMap.Put(TagUrl, "URL");
			_tagNameMap.Put(TagThumbnail, "Thumbnail Data");
			_tagNameMap.Put(TagGlobalAngle, "Global Angle");
			_tagNameMap.Put(TagIccProfileBytes, "ICC Profile Bytes");
			_tagNameMap.Put(TagWatermark, "Watermark");
			_tagNameMap.Put(TagIccUntaggedProfile, "ICC Untagged Profile");
			_tagNameMap.Put(TagEffectsVisible, "Effects Visible");
			_tagNameMap.Put(TagSpotHalftone, "Spot Halftone");
			_tagNameMap.Put(TagSeedNumber, "Seed Number");
			_tagNameMap.Put(TagUnicodeAlphaNames, "Unicode Alpha Names");
			_tagNameMap.Put(TagIndexedColorTableCount, "Indexed Color Table Count");
			_tagNameMap.Put(TagTransparencyIndex, "Transparency Index");
			_tagNameMap.Put(TagGlobalAltitude, "Global Altitude");
			_tagNameMap.Put(TagSlices, "Slices");
			_tagNameMap.Put(TagWorkflowUrl, "Workflow URL");
			_tagNameMap.Put(TagJumpToXpep, "Jump To XPEP");
			_tagNameMap.Put(TagAlphaIdentifiers, "Alpha Identifiers");
			_tagNameMap.Put(TagUrlList, "URL List");
			_tagNameMap.Put(TagVersion, "Version Info");
			_tagNameMap.Put(TagExifData1, "EXIF Data 1");
			_tagNameMap.Put(TagExifData3, "EXIF Data 3");
			_tagNameMap.Put(TagXmpData, "XMP Data");
			_tagNameMap.Put(TagCaptionDigest, "Caption Digest");
			_tagNameMap.Put(TagPrintScale, "Print Scale");
			_tagNameMap.Put(TagPixelAspectRatio, "Pixel Aspect Ratio");
			_tagNameMap.Put(TagLayerComps, "Layer Comps");
			_tagNameMap.Put(TagAlternateDuotoneColors, "Alternate Duotone Colors");
			_tagNameMap.Put(TagAlternateSpotColors, "Alternate Spot Colors");
			_tagNameMap.Put(TagLayerSelectionIds, "Layer Selection IDs");
			_tagNameMap.Put(TagHdrToningInfo, "HDR Toning Info");
			_tagNameMap.Put(TagPrintInfo, "Print Info");
			_tagNameMap.Put(TagLayerGroupsEnabledId, "Layer Groups Enabled ID");
			_tagNameMap.Put(TagColorSamplers, "Color Samplers");
			_tagNameMap.Put(TagMeasurementScale, "Measurement Scale");
			_tagNameMap.Put(TagTimelineInformation, "Timeline Information");
			_tagNameMap.Put(TagSheetDisclosure, "Sheet Disclosure");
			_tagNameMap.Put(TagDisplayInfo, "Display Info");
			_tagNameMap.Put(TagOnionSkins, "Onion Skins");
			_tagNameMap.Put(TagCountInformation, "Count information");
			_tagNameMap.Put(TagPrintInfo2, "Print Info 2");
			_tagNameMap.Put(TagPrintStyle, "Print Style");
			_tagNameMap.Put(TagMacNsprintinfo, "Mac NSPrintInfo");
			_tagNameMap.Put(TagWinDevmode, "Win DEVMODE");
			_tagNameMap.Put(TagAutoSaveFilePath, "Auto Save File Path");
			_tagNameMap.Put(TagAutoSaveFormat, "Auto Save Format");
			_tagNameMap.Put(TagPathSelectionState, "Path Selection State");
			_tagNameMap.Put(TagClippingPathName, "Clipping Path Name");
			_tagNameMap.Put(TagOriginPathInfo, "Origin Path Info");
			_tagNameMap.Put(TagImageReadyVariablesXml, "Image Ready Variables XML");
			_tagNameMap.Put(TagImageReadyDataSets, "Image Ready Data Sets");
			_tagNameMap.Put(TagLightroomWorkflow, "Lightroom Workflow");
			_tagNameMap.Put(TagPrintFlagsInfo, "Print Flags Information");
		}

		public PhotoshopDirectory()
		{
			this.SetDescriptor(new PhotoshopDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Photoshop";
		}

		[NotNull]
		protected internal override Dictionary<int?, string> GetTagNameMap()
		{
			return _tagNameMap;
		}

		[CanBeNull]
		public virtual sbyte[] GetThumbnailBytes()
		{
			sbyte[] storedBytes = GetByteArray(Com.Drew.Metadata.Photoshop.PhotoshopDirectory.TagThumbnail);
			if (storedBytes == null)
			{
				storedBytes = GetByteArray(Com.Drew.Metadata.Photoshop.PhotoshopDirectory.TagThumbnailOld);
			}
			if (storedBytes == null)
			{
				return null;
			}
			int thumbSize = storedBytes.Length - 28;
			sbyte[] thumbBytes = new sbyte[thumbSize];
			System.Array.Copy(storedBytes, 28, thumbBytes, 0, thumbSize);
			return thumbBytes;
		}
	}
}
