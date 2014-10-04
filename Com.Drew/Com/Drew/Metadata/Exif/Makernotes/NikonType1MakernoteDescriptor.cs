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
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="NikonType1MakernoteDirectory"/>
	/// .
	/// <p/>
	/// Type-1 is for E-Series cameras prior to (not including) E990.  For example: E700, E800, E900,
	/// E900S, E910, E950.
	/// <p/>
	/// Makernote starts from ASCII string "Nikon". Data format is the same as IFD, but it starts from
	/// offset 0x08. This is the same as Olympus except start string. Example of actual data
	/// structure is shown below.
	/// <pre><code>
	/// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
	/// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
	/// </code></pre>
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class NikonType1MakernoteDescriptor : TagDescriptor<NikonType1MakernoteDirectory>
	{
		public NikonType1MakernoteDescriptor(NikonType1MakernoteDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case TagQuality:
				{
					return GetQualityDescription();
				}

				case TagColorMode:
				{
					return GetColorModeDescription();
				}

				case TagImageAdjustment:
				{
					return GetImageAdjustmentDescription();
				}

				case TagCcdSensitivity:
				{
					return GetCcdSensitivityDescription();
				}

				case TagWhiteBalance:
				{
					return GetWhiteBalanceDescription();
				}

				case TagFocus:
				{
					return GetFocusDescription();
				}

				case TagDigitalZoom:
				{
					return GetDigitalZoomDescription();
				}

				case TagConverter:
				{
					return GetConverterDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetConverterDescription()
		{
			return GetIndexedDescription(TagConverter, "None", "Fisheye converter");
		}

		[CanBeNull]
		public virtual string GetDigitalZoomDescription()
		{
			Rational value = _directory.GetRational(TagDigitalZoom);
			return value == null ? null : value.GetNumerator() == 0 ? "No digital zoom" : value.ToSimpleString(true) + "x digital zoom";
		}

		[CanBeNull]
		public virtual string GetFocusDescription()
		{
			Rational value = _directory.GetRational(TagFocus);
			return value == null ? null : value.GetNumerator() == 1 && value.GetDenominator() == 0 ? "Infinite" : value.ToSimpleString(true);
		}

		[CanBeNull]
		public virtual string GetWhiteBalanceDescription()
		{
			return GetIndexedDescription(TagWhiteBalance, "Auto", "Preset", "Daylight", "Incandescence", "Florescence", "Cloudy", "SpeedLight");
		}

		[CanBeNull]
		public virtual string GetCcdSensitivityDescription()
		{
			return GetIndexedDescription(TagCcdSensitivity, "ISO80", null, "ISO160", null, "ISO320", "ISO100");
		}

		[CanBeNull]
		public virtual string GetImageAdjustmentDescription()
		{
			return GetIndexedDescription(TagImageAdjustment, "Normal", "Bright +", "Bright -", "Contrast +", "Contrast -");
		}

		[CanBeNull]
		public virtual string GetColorModeDescription()
		{
			return GetIndexedDescription(TagColorMode, 1, "Color", "Monochrome");
		}

		[CanBeNull]
		public virtual string GetQualityDescription()
		{
			return GetIndexedDescription(TagQuality, 1, "VGA Basic", "VGA Normal", "VGA Fine", "SXGA Basic", "SXGA Normal", "SXGA Fine");
		}
	}
}
