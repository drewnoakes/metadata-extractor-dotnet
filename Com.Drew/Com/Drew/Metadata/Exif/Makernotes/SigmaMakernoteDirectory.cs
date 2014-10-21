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
	/// <summary>Describes tags specific to Sigma / Foveon cameras.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SigmaMakernoteDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagSerialNumber = unchecked((int)(0x2));

		public const int TagDriveMode = unchecked((int)(0x3));

		public const int TagResolutionMode = unchecked((int)(0x4));

		public const int TagAutoFocusMode = unchecked((int)(0x5));

		public const int TagFocusSetting = unchecked((int)(0x6));

		public const int TagWhiteBalance = unchecked((int)(0x7));

		public const int TagExposureMode = unchecked((int)(0x8));

		public const int TagMeteringMode = unchecked((int)(0x9));

		public const int TagLensRange = unchecked((int)(0xa));

		public const int TagColorSpace = unchecked((int)(0xb));

		public const int TagExposure = unchecked((int)(0xc));

		public const int TagContrast = unchecked((int)(0xd));

		public const int TagShadow = unchecked((int)(0xe));

		public const int TagHighlight = unchecked((int)(0xf));

		public const int TagSaturation = unchecked((int)(0x10));

		public const int TagSharpness = unchecked((int)(0x11));

		public const int TagFillLight = unchecked((int)(0x12));

		public const int TagColorAdjustment = unchecked((int)(0x14));

		public const int TagAdjustmentMode = unchecked((int)(0x15));

		public const int TagQuality = unchecked((int)(0x16));

		public const int TagFirmware = unchecked((int)(0x17));

		public const int TagSoftware = unchecked((int)(0x18));

		public const int TagAutoBracket = unchecked((int)(0x19));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static SigmaMakernoteDirectory()
		{
			_tagNameMap.Put(TagSerialNumber, "Serial Number");
			_tagNameMap.Put(TagDriveMode, "Drive Mode");
			_tagNameMap.Put(TagResolutionMode, "Resolution Mode");
			_tagNameMap.Put(TagAutoFocusMode, "Auto Focus Mode");
			_tagNameMap.Put(TagFocusSetting, "Focus Setting");
			_tagNameMap.Put(TagWhiteBalance, "White Balance");
			_tagNameMap.Put(TagExposureMode, "Exposure Mode");
			_tagNameMap.Put(TagMeteringMode, "Metering Mode");
			_tagNameMap.Put(TagLensRange, "Lens Range");
			_tagNameMap.Put(TagColorSpace, "Color Space");
			_tagNameMap.Put(TagExposure, "Exposure");
			_tagNameMap.Put(TagContrast, "Contrast");
			_tagNameMap.Put(TagShadow, "Shadow");
			_tagNameMap.Put(TagHighlight, "Highlight");
			_tagNameMap.Put(TagSaturation, "Saturation");
			_tagNameMap.Put(TagSharpness, "Sharpness");
			_tagNameMap.Put(TagFillLight, "Fill Light");
			_tagNameMap.Put(TagColorAdjustment, "Color Adjustment");
			_tagNameMap.Put(TagAdjustmentMode, "Adjustment Mode");
			_tagNameMap.Put(TagQuality, "Quality");
			_tagNameMap.Put(TagFirmware, "Firmware");
			_tagNameMap.Put(TagSoftware, "Software");
			_tagNameMap.Put(TagAutoBracket, "Auto Bracket");
		}

		public SigmaMakernoteDirectory()
		{
			this.SetDescriptor(new SigmaMakernoteDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Sigma Makernote";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
