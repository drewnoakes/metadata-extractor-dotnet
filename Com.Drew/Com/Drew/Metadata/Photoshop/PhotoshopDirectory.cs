/*
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
using Com.Drew.Metadata.Photoshop;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
	/// <summary>Holds the metadata found in the APPD segment of a JPEG file saved by Photoshop.</summary>
	/// <author>Yuri Binev, Drew Noakes http://drewnoakes.com</author>
	public class PhotoshopDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagChannelsRowsColumnsDepthMode = unchecked((int)(0x03E8));

		public const int TagMacPrintInfo = unchecked((int)(0x03E9));

		public const int TagXml = unchecked((int)(0x03EA));

		public const int TagIndexedColorTable = unchecked((int)(0x03EB));

		public const int TagResolutionInfo = unchecked((int)(0x03ED));

		public const int TagAlphaChannels = unchecked((int)(0x03EE));

		public const int TagDisplayInfo = unchecked((int)(0x03EF));

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

		public const int TagIccUntaggedProfile = unchecked((int)(0x0411));

		public const int TagSeedNumber = unchecked((int)(0x0414));

		public const int TagGlobalAltitude = unchecked((int)(0x0419));

		public const int TagSlices = unchecked((int)(0x041A));

		public const int TagUrlList = unchecked((int)(0x041E));

		public const int TagVersion = unchecked((int)(0x0421));

		public const int TagCaptionDigest = unchecked((int)(0x0425));

		public const int TagPrintScale = unchecked((int)(0x0426));

		public const int TagPixelAspectRatio = unchecked((int)(0x0428));

		public const int TagPrintInfo = unchecked((int)(0x042F));

		public const int TagPrintFlagsInfo = unchecked((int)(0x2710));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static PhotoshopDirectory()
		{
			_tagNameMap.Put(TagChannelsRowsColumnsDepthMode, "Channels, Rows, Columns, Depth, Mode");
			_tagNameMap.Put(TagMacPrintInfo, "Mac Print Info");
			_tagNameMap.Put(TagXml, "XML Data");
			_tagNameMap.Put(TagIndexedColorTable, "Indexed Color Table");
			_tagNameMap.Put(TagResolutionInfo, "Resolution Info");
			_tagNameMap.Put(TagAlphaChannels, "Alpha Channels");
			_tagNameMap.Put(TagDisplayInfo, "Display Info");
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
			_tagNameMap.Put(TagIccUntaggedProfile, "ICC Untagged Profile");
			_tagNameMap.Put(TagSeedNumber, "Seed Number");
			_tagNameMap.Put(TagGlobalAltitude, "Global Altitude");
			_tagNameMap.Put(TagSlices, "Slices");
			_tagNameMap.Put(TagUrlList, "URL List");
			_tagNameMap.Put(TagVersion, "Version Info");
			_tagNameMap.Put(TagCaptionDigest, "Caption Digest");
			_tagNameMap.Put(TagPrintScale, "Print Scale");
			_tagNameMap.Put(TagPixelAspectRatio, "Pixel Aspect Ratio");
			_tagNameMap.Put(TagPrintInfo, "Print Info");
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
		protected internal override Dictionary<int, string> GetTagNameMap()
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
