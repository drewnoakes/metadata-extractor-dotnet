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
                case NikonType2MakernoteDirectory.TagProgramShift:
				{
					return GetProgramShiftDescription();
				}

                case NikonType2MakernoteDirectory.TagExposureDifference:
				{
					return GetExposureDifferenceDescription();
				}

                case NikonType2MakernoteDirectory.TagLens:
				{
					return GetLensDescription();
				}

                case NikonType2MakernoteDirectory.TagCameraHueAdjustment:
				{
					return GetHueAdjustmentDescription();
				}

                case NikonType2MakernoteDirectory.TagCameraColorMode:
				{
					return GetColorModeDescription();
				}

                case NikonType2MakernoteDirectory.TagAutoFlashCompensation:
				{
					return GetAutoFlashCompensationDescription();
				}

                case NikonType2MakernoteDirectory.TagFlashExposureCompensation:
				{
					return GetFlashExposureCompensationDescription();
				}

                case NikonType2MakernoteDirectory.TagFlashBracketCompensation:
				{
					return GetFlashBracketCompensationDescription();
				}

                case NikonType2MakernoteDirectory.TagExposureTuning:
				{
					return GetExposureTuningDescription();
				}

                case NikonType2MakernoteDirectory.TagLensStops:
				{
					return GetLensStopsDescription();
				}

                case NikonType2MakernoteDirectory.TagColorSpace:
				{
					return GetColorSpaceDescription();
				}

                case NikonType2MakernoteDirectory.TagActiveDLighting:
				{
					return GetActiveDLightingDescription();
				}

                case NikonType2MakernoteDirectory.TagVignetteControl:
				{
					return GetVignetteControlDescription();
				}

                case NikonType2MakernoteDirectory.TagIso1:
				{
					return GetIsoSettingDescription();
				}

                case NikonType2MakernoteDirectory.TagDigitalZoom:
				{
					return GetDigitalZoomDescription();
				}

                case NikonType2MakernoteDirectory.TagFlashUsed:
				{
					return GetFlashUsedDescription();
				}

                case NikonType2MakernoteDirectory.TagAfFocusPosition:
				{
					return GetAutoFocusPositionDescription();
				}

                case NikonType2MakernoteDirectory.TagFirmwareVersion:
				{
					return GetFirmwareVersionDescription();
				}

                case NikonType2MakernoteDirectory.TagLensType:
				{
					return GetLensTypeDescription();
				}

                case NikonType2MakernoteDirectory.TagShootingMode:
				{
					return GetShootingModeDescription();
				}

                case NikonType2MakernoteDirectory.TagNefCompression:
				{
					return GetNEFCompressionDescription();
				}

                case NikonType2MakernoteDirectory.TagHighIsoNoiseReduction:
				{
					return GetHighISONoiseReductionDescription();
				}

                case NikonType2MakernoteDirectory.TagPowerUpTime:
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
            return GetEpochTimeDescription(NikonType2MakernoteDirectory.TagPowerUpTime);
		}

		[CanBeNull]
		public virtual string GetHighISONoiseReductionDescription()
		{
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagHighIsoNoiseReduction, "Off", "Minimal", "Low", null, "Normal", null, "High");
		}

		[CanBeNull]
		public virtual string GetFlashUsedDescription()
		{
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagFlashUsed, "Flash Not Used", "Manual Flash", null, "Flash Not Ready", null, null, null, "External Flash", "Fired, Commander Mode", "Fired, TTL Mode");
		}

		[CanBeNull]
		public virtual string GetNEFCompressionDescription()
		{
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagNefCompression, 1, "Lossy (Type 1)", null, "Uncompressed", null, null, null, "Lossless", "Lossy (Type 2)");
		}

		[CanBeNull]
		public virtual string GetShootingModeDescription()
		{
            return GetBitFlagDescription(NikonType2MakernoteDirectory.TagShootingMode, new string[] { "Single Frame", "Continuous" }, "Delay", null, "PC Control", "Exposure Bracketing", "Auto ISO", "White-Balance Bracketing", "IR Control");
		}

		// LSB [low label, high label]
		[CanBeNull]
		public virtual string GetLensTypeDescription()
		{
            return GetBitFlagDescription(NikonType2MakernoteDirectory.TagLensType, new string[] { "AF", "MF" }, "D", "G", "VR");
		}

		// LSB [low label, high label]
		[CanBeNull]
		public virtual string GetColorSpaceDescription()
		{
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagColorSpace, 1, "sRGB", "Adobe RGB");
		}

		[CanBeNull]
		public virtual string GetActiveDLightingDescription()
		{
			int? value = _directory.GetInteger(NikonType2MakernoteDirectory.TagActiveDLighting);
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
            int? value = _directory.GetInteger(NikonType2MakernoteDirectory.TagVignetteControl);
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
            int[] values = _directory.GetIntArray(NikonType2MakernoteDirectory.TagAfFocusPosition);
			if (values == null)
			{
				return null;
			}
			if (values.Length != 4 || values[0] != 0 || values[2] != 0 || values[3] != 0)
			{
                return "Unknown (" + _directory.GetString(NikonType2MakernoteDirectory.TagAfFocusPosition) + ")";
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
            Rational value = _directory.GetRational(NikonType2MakernoteDirectory.TagDigitalZoom);
			if (value == null)
			{
				return null;
			}
			return value.IntValue() == 1 ? "No digital zoom" : value.ToSimpleString(true) + "x digital zoom";
		}

		[CanBeNull]
		public virtual string GetProgramShiftDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagProgramShift);
		}

		[CanBeNull]
		public virtual string GetExposureDifferenceDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagExposureDifference);
		}

		[CanBeNull]
		public virtual string GetAutoFlashCompensationDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagAutoFlashCompensation);
		}

		[CanBeNull]
		public virtual string GetFlashExposureCompensationDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagFlashExposureCompensation);
		}

		[CanBeNull]
		public virtual string GetFlashBracketCompensationDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagFlashBracketCompensation);
		}

		[CanBeNull]
		public virtual string GetExposureTuningDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagExposureTuning);
		}

		[CanBeNull]
		public virtual string GetLensStopsDescription()
		{
            return GetEVDescription(NikonType2MakernoteDirectory.TagLensStops);
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
            int[] values = _directory.GetIntArray(NikonType2MakernoteDirectory.TagIso1);
			if (values == null)
			{
				return null;
			}
			if (values[0] != 0 || values[1] == 0)
			{
                return "Unknown (" + _directory.GetString(NikonType2MakernoteDirectory.TagIso1) + ")";
			}
			return "ISO " + values[1];
		}

		[CanBeNull]
		public virtual string GetLensDescription()
		{
            Rational[] values = _directory.GetRationalArray(NikonType2MakernoteDirectory.TagLens);
            return values == null ? null : values.Length < 4 ? _directory.GetString(NikonType2MakernoteDirectory.TagLens) : Sharpen.Extensions.StringFormat("%d-%dmm f/%.1f-%.1f", values[0].IntValue(), values[1].IntValue(), values[2].FloatValue(), values[3].FloatValue());
		}

		[CanBeNull]
		public virtual string GetHueAdjustmentDescription()
		{
            return GetFormattedString(NikonType2MakernoteDirectory.TagCameraHueAdjustment, "%s degrees");
		}

		[CanBeNull]
		public virtual string GetColorModeDescription()
		{
            string value = _directory.GetString(NikonType2MakernoteDirectory.TagCameraColorMode);
			return value == null ? null : value.StartsWith("MODE1") ? "Mode I (sRGB)" : value;
		}

		[CanBeNull]
		public virtual string GetFirmwareVersionDescription()
		{
            return GetVersionBytesDescription(NikonType2MakernoteDirectory.TagFirmwareVersion, 2);
		}
	}
}
