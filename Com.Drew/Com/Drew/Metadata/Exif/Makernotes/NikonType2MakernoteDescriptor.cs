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
	/// <see cref="NikonType2MakernoteDirectory"/>
	/// .
	/// Type-2 applies to the E990 and D-series cameras such as the D1, D70 and D100.
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class NikonType2MakernoteDescriptor : TagDescriptor<NikonType2MakernoteDirectory>
	{
		public NikonType2MakernoteDescriptor(NikonType2MakernoteDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case TagProgramShift:
				{
					return GetProgramShiftDescription();
				}

				case TagExposureDifference:
				{
					return GetExposureDifferenceDescription();
				}

				case TagLens:
				{
					return GetLensDescription();
				}

				case TagCameraHueAdjustment:
				{
					return GetHueAdjustmentDescription();
				}

				case TagCameraColorMode:
				{
					return GetColorModeDescription();
				}

				case TagAutoFlashCompensation:
				{
					return GetAutoFlashCompensationDescription();
				}

				case TagFlashExposureCompensation:
				{
					return GetFlashExposureCompensationDescription();
				}

				case TagFlashBracketCompensation:
				{
					return GetFlashBracketCompensationDescription();
				}

				case TagExposureTuning:
				{
					return GetExposureTuningDescription();
				}

				case TagLensStops:
				{
					return GetLensStopsDescription();
				}

				case TagColorSpace:
				{
					return GetColorSpaceDescription();
				}

				case TagActiveDLighting:
				{
					return GetActiveDLightingDescription();
				}

				case TagVignetteControl:
				{
					return GetVignetteControlDescription();
				}

				case TagIso1:
				{
					return GetIsoSettingDescription();
				}

				case TagDigitalZoom:
				{
					return GetDigitalZoomDescription();
				}

				case TagFlashUsed:
				{
					return GetFlashUsedDescription();
				}

				case TagAfFocusPosition:
				{
					return GetAutoFocusPositionDescription();
				}

				case TagFirmwareVersion:
				{
					return GetFirmwareVersionDescription();
				}

				case TagLensType:
				{
					return GetLensTypeDescription();
				}

				case TagShootingMode:
				{
					return GetShootingModeDescription();
				}

				case TagNefCompression:
				{
					return GetNEFCompressionDescription();
				}

				case TagHighIsoNoiseReduction:
				{
					return GetHighISONoiseReductionDescription();
				}

				case TagPowerUpTime:
				{
					return GetPowerUpTimeDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetPowerUpTimeDescription()
		{
			return GetEpochTimeDescription(TagPowerUpTime);
		}

		[CanBeNull]
		public virtual string GetHighISONoiseReductionDescription()
		{
			return GetIndexedDescription(TagHighIsoNoiseReduction, "Off", "Minimal", "Low", null, "Normal", null, "High");
		}

		[CanBeNull]
		public virtual string GetFlashUsedDescription()
		{
			return GetIndexedDescription(TagFlashUsed, "Flash Not Used", "Manual Flash", null, "Flash Not Ready", null, null, null, "External Flash", "Fired, Commander Mode", "Fired, TTL Mode");
		}

		[CanBeNull]
		public virtual string GetNEFCompressionDescription()
		{
			return GetIndexedDescription(TagNefCompression, 1, "Lossy (Type 1)", null, "Uncompressed", null, null, null, "Lossless", "Lossy (Type 2)");
		}

		[CanBeNull]
		public virtual string GetShootingModeDescription()
		{
			return GetBitFlagDescription(TagShootingMode, new string[] { "Single Frame", "Continuous" }, "Delay", null, "PC Control", "Exposure Bracketing", "Auto ISO", "White-Balance Bracketing", "IR Control");
		}

		// LSB [low label, high label]
		[CanBeNull]
		public virtual string GetLensTypeDescription()
		{
			return GetBitFlagDescription(TagLensType, new string[] { "AF", "MF" }, "D", "G", "VR");
		}

		// LSB [low label, high label]
		[CanBeNull]
		public virtual string GetColorSpaceDescription()
		{
			return GetIndexedDescription(TagColorSpace, 1, "sRGB", "Adobe RGB");
		}

		[CanBeNull]
		public virtual string GetActiveDLightingDescription()
		{
			int value = _directory.GetInteger(TagActiveDLighting);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "Off";
				}

				case 1:
				{
					return "Light";
				}

				case 3:
				{
					return "Normal";
				}

				case 5:
				{
					return "High";
				}

				case 7:
				{
					return "Extra High";
				}

				case 65535:
				{
					return "Auto";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetVignetteControlDescription()
		{
			int value = _directory.GetInteger(TagVignetteControl);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "Off";
				}

				case 1:
				{
					return "Low";
				}

				case 3:
				{
					return "Normal";
				}

				case 5:
				{
					return "High";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetAutoFocusPositionDescription()
		{
			int[] values = _directory.GetIntArray(TagAfFocusPosition);
			if (values == null)
			{
				return null;
			}
			if (values.Length != 4 || values[0] != 0 || values[2] != 0 || values[3] != 0)
			{
				return "Unknown (" + _directory.GetString(TagAfFocusPosition) + ")";
			}
			switch (values[1])
			{
				case 0:
				{
					return "Centre";
				}

				case 1:
				{
					return "Top";
				}

				case 2:
				{
					return "Bottom";
				}

				case 3:
				{
					return "Left";
				}

				case 4:
				{
					return "Right";
				}

				default:
				{
					return "Unknown (" + values[1] + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetDigitalZoomDescription()
		{
			Rational value = _directory.GetRational(TagDigitalZoom);
			if (value == null)
			{
				return null;
			}
			return value.IntValue() == 1 ? "No digital zoom" : value.ToSimpleString(true) + "x digital zoom";
		}

		[CanBeNull]
		public virtual string GetProgramShiftDescription()
		{
			return GetEVDescription(TagProgramShift);
		}

		[CanBeNull]
		public virtual string GetExposureDifferenceDescription()
		{
			return GetEVDescription(TagExposureDifference);
		}

		[CanBeNull]
		public virtual string GetAutoFlashCompensationDescription()
		{
			return GetEVDescription(TagAutoFlashCompensation);
		}

		[CanBeNull]
		public virtual string GetFlashExposureCompensationDescription()
		{
			return GetEVDescription(TagFlashExposureCompensation);
		}

		[CanBeNull]
		public virtual string GetFlashBracketCompensationDescription()
		{
			return GetEVDescription(TagFlashBracketCompensation);
		}

		[CanBeNull]
		public virtual string GetExposureTuningDescription()
		{
			return GetEVDescription(TagExposureTuning);
		}

		[CanBeNull]
		public virtual string GetLensStopsDescription()
		{
			return GetEVDescription(TagLensStops);
		}

		[CanBeNull]
		private string GetEVDescription(int tagType)
		{
			int[] values = _directory.GetIntArray(tagType);
			if (values == null)
			{
				return null;
			}
			if (values.Length < 3 || values[2] == 0)
			{
				return null;
			}
			DecimalFormat decimalFormat = new DecimalFormat("0.##");
			double ev = values[0] * values[1] / (double)values[2];
			return decimalFormat.Format(ev) + " EV";
		}

		[CanBeNull]
		public virtual string GetIsoSettingDescription()
		{
			int[] values = _directory.GetIntArray(TagIso1);
			if (values == null)
			{
				return null;
			}
			if (values[0] != 0 || values[1] == 0)
			{
				return "Unknown (" + _directory.GetString(TagIso1) + ")";
			}
			return "ISO " + values[1];
		}

		[CanBeNull]
		public virtual string GetLensDescription()
		{
			Rational[] values = _directory.GetRationalArray(TagLens);
			return values == null ? null : values.Length < 4 ? _directory.GetString(TagLens) : Sharpen.Extensions.StringFormat("%d-%dmm f/%.1f-%.1f", values[0].IntValue(), values[1].IntValue(), values[2].FloatValue(), values[3].FloatValue());
		}

		[CanBeNull]
		public virtual string GetHueAdjustmentDescription()
		{
			return GetFormattedString(TagCameraHueAdjustment, "%s degrees");
		}

		[CanBeNull]
		public virtual string GetColorModeDescription()
		{
			string value = _directory.GetString(TagCameraColorMode);
			return value == null ? null : value.StartsWith("MODE1") ? "Mode I (sRGB)" : value;
		}

		[CanBeNull]
		public virtual string GetFirmwareVersionDescription()
		{
			return GetVersionBytesDescription(TagFirmwareVersion, 2);
		}
	}
}
