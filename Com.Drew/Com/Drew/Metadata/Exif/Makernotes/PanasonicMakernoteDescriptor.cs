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
using System.IO;
using System.Text;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="PanasonicMakernoteDirectory"/>
	/// .
	/// <p/>
	/// Some information about this makernote taken from here:
	/// <ul>
	/// <li><a href="http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html">http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html</a></li>
	/// <li><a href="http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html">http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html</a></li>
	/// </ul>
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	/// <author>Philipp Sandhaus</author>
	public class PanasonicMakernoteDescriptor : TagDescriptor<PanasonicMakernoteDirectory>
	{
		public PanasonicMakernoteDescriptor(PanasonicMakernoteDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case TagQualityMode:
				{
					return GetQualityModeDescription();
				}

				case TagFirmwareVersion:
				{
					return GetVersionDescription();
				}

				case TagWhiteBalance:
				{
					return GetWhiteBalanceDescription();
				}

				case TagFocusMode:
				{
					return GetFocusModeDescription();
				}

				case TagAfAreaMode:
				{
					return GetAfAreaModeDescription();
				}

				case TagImageStabilization:
				{
					return GetImageStabilizationDescription();
				}

				case TagMacroMode:
				{
					return GetMacroModeDescription();
				}

				case TagRecordMode:
				{
					return GetRecordModeDescription();
				}

				case TagAudio:
				{
					return GetAudioDescription();
				}

				case TagUnknownDataDump:
				{
					return GetUnknownDataDumpDescription();
				}

				case TagColorEffect:
				{
					return GetColorEffectDescription();
				}

				case TagUptime:
				{
					return GetUptimeDescription();
				}

				case TagBurstMode:
				{
					return GetBurstModeDescription();
				}

				case TagContrastMode:
				{
					return GetContrastModeDescription();
				}

				case TagNoiseReduction:
				{
					return GetNoiseReductionDescription();
				}

				case TagSelfTimer:
				{
					return GetSelfTimerDescription();
				}

				case TagRotation:
				{
					return GetRotationDescription();
				}

				case TagAfAssistLamp:
				{
					return GetAfAssistLampDescription();
				}

				case TagColorMode:
				{
					return GetColorModeDescription();
				}

				case TagOpticalZoomMode:
				{
					return GetOpticalZoomModeDescription();
				}

				case TagConversionLens:
				{
					return GetConversionLensDescription();
				}

				case TagContrast:
				{
					return GetContrastDescription();
				}

				case TagWorldTimeLocation:
				{
					return GetWorldTimeLocationDescription();
				}

				case TagAdvancedSceneMode:
				{
					return GetAdvancedSceneModeDescription();
				}

				case TagFaceDetectionInfo:
				{
					return GetDetectedFacesDescription();
				}

				case TagTransform:
				{
					return GetTransformDescription();
				}

				case TagTransform1:
				{
					return GetTransform1Description();
				}

				case TagIntelligentExposure:
				{
					return GetIntelligentExposureDescription();
				}

				case TagFlashWarning:
				{
					return GetFlashWarningDescription();
				}

				case TagCountry:
				{
					return GetCountryDescription();
				}

				case TagState:
				{
					return GetStateDescription();
				}

				case TagCity:
				{
					return GetCityDescription();
				}

				case TagLandmark:
				{
					return GetLandmarkDescription();
				}

				case TagIntelligentResolution:
				{
					return GetIntelligentResolutionDescription();
				}

				case TagFaceRecognitionInfo:
				{
					return GetRecognizedFacesDescription();
				}

				case TagPrintImageMatchingInfo:
				{
					return GetPrintImageMatchingInfoDescription();
				}

				case TagSceneMode:
				{
					return GetSceneModeDescription();
				}

				case TagFlashFired:
				{
					return GetFlashFiredDescription();
				}

				case TagTextStamp:
				{
					return GetTextStampDescription();
				}

				case TagTextStamp1:
				{
					return GetTextStamp1Description();
				}

				case TagTextStamp2:
				{
					return GetTextStamp2Description();
				}

				case TagTextStamp3:
				{
					return GetTextStamp3Description();
				}

				case TagMakernoteVersion:
				{
					return GetMakernoteVersionDescription();
				}

				case TagExifVersion:
				{
					return GetExifVersionDescription();
				}

				case TagInternalSerialNumber:
				{
					return GetInternalSerialNumberDescription();
				}

				case TagTitle:
				{
					return GetTitleDescription();
				}

				case TagBabyName:
				{
					return GetBabyNameDescription();
				}

				case TagLocation:
				{
					return GetLocationDescription();
				}

				case TagBabyAge:
				{
					return GetBabyAgeDescription();
				}

				case TagBabyAge1:
				{
					return GetBabyAge1Description();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetPrintImageMatchingInfoDescription()
		{
			return GetByteLengthDescription(TagPrintImageMatchingInfo);
		}

		[CanBeNull]
		public virtual string GetTextStampDescription()
		{
			return GetIndexedDescription(TagTextStamp, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetTextStamp1Description()
		{
			return GetIndexedDescription(TagTextStamp1, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetTextStamp2Description()
		{
			return GetIndexedDescription(TagTextStamp2, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetTextStamp3Description()
		{
			return GetIndexedDescription(TagTextStamp3, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetMacroModeDescription()
		{
			return GetIndexedDescription(TagMacroMode, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetFlashFiredDescription()
		{
			return GetIndexedDescription(TagFlashFired, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetImageStabilizationDescription()
		{
			return GetIndexedDescription(TagImageStabilization, 2, "On, Mode 1", "Off", "On, Mode 2");
		}

		[CanBeNull]
		public virtual string GetAudioDescription()
		{
			return GetIndexedDescription(TagAudio, 1, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetTransformDescription()
		{
			return GetTransformDescription(TagTransform);
		}

		[CanBeNull]
		public virtual string GetTransform1Description()
		{
			return GetTransformDescription(TagTransform1);
		}

		[CanBeNull]
		private string GetTransformDescription(int tag)
		{
			sbyte[] values = _directory.GetByteArray(tag);
			if (values == null)
			{
				return null;
			}
			RandomAccessReader reader = new ByteArrayReader(values);
			try
			{
				int val1 = reader.GetUInt16(0);
				int val2 = reader.GetUInt16(2);
				if (val1 == -1 && val2 == 1)
				{
					return "Slim Low";
				}
				if (val1 == -3 && val2 == 2)
				{
					return "Slim High";
				}
				if (val1 == 0 && val2 == 0)
				{
					return "Off";
				}
				if (val1 == 1 && val2 == 1)
				{
					return "Stretch Low";
				}
				if (val1 == 3 && val2 == 2)
				{
					return "Stretch High";
				}
				return "Unknown (" + val1 + " " + val2 + ")";
			}
			catch (IOException)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetIntelligentExposureDescription()
		{
			return GetIndexedDescription(TagIntelligentExposure, "Off", "Low", "Standard", "High");
		}

		[CanBeNull]
		public virtual string GetFlashWarningDescription()
		{
			return GetIndexedDescription(TagFlashWarning, "No", "Yes (Flash required but disabled)");
		}

		[CanBeNull]
		public virtual string GetCountryDescription()
		{
			return GetAsciiStringFromBytes(TagCountry);
		}

		[CanBeNull]
		public virtual string GetStateDescription()
		{
			return GetAsciiStringFromBytes(TagState);
		}

		[CanBeNull]
		public virtual string GetCityDescription()
		{
			return GetAsciiStringFromBytes(TagCity);
		}

		[CanBeNull]
		public virtual string GetLandmarkDescription()
		{
			return GetAsciiStringFromBytes(TagLandmark);
		}

		[CanBeNull]
		public virtual string GetTitleDescription()
		{
			return GetAsciiStringFromBytes(TagTitle);
		}

		[CanBeNull]
		public virtual string GetBabyNameDescription()
		{
			return GetAsciiStringFromBytes(TagBabyName);
		}

		[CanBeNull]
		public virtual string GetLocationDescription()
		{
			return GetAsciiStringFromBytes(TagLocation);
		}

		[CanBeNull]
		public virtual string GetIntelligentResolutionDescription()
		{
			return GetIndexedDescription(TagIntelligentResolution, "Off", null, "Auto", "On");
		}

		[CanBeNull]
		public virtual string GetContrastDescription()
		{
			return GetIndexedDescription(TagContrast, "Normal");
		}

		[CanBeNull]
		public virtual string GetWorldTimeLocationDescription()
		{
			return GetIndexedDescription(TagWorldTimeLocation, 1, "Home", "Destination");
		}

		[CanBeNull]
		public virtual string GetAdvancedSceneModeDescription()
		{
			return GetIndexedDescription(TagAdvancedSceneMode, 1, "Normal", "Outdoor/Illuminations/Flower/HDR Art", "Indoor/Architecture/Objects/HDR B&W", "Creative", "Auto", null, "Expressive", "Retro", "Pure", "Elegant", null, "Monochrome", "Dynamic Art"
				, "Silhouette");
		}

		[CanBeNull]
		public virtual string GetUnknownDataDumpDescription()
		{
			return GetByteLengthDescription(TagUnknownDataDump);
		}

		[CanBeNull]
		public virtual string GetColorEffectDescription()
		{
			return GetIndexedDescription(TagColorEffect, 1, "Off", "Warm", "Cool", "Black & White", "Sepia");
		}

		[CanBeNull]
		public virtual string GetUptimeDescription()
		{
			int value = _directory.GetInteger(TagUptime);
			if (value == null)
			{
				return null;
			}
			return value / 100f + " s";
		}

		[CanBeNull]
		public virtual string GetBurstModeDescription()
		{
			return GetIndexedDescription(TagBurstMode, "Off", null, "On", "Indefinite", "Unlimited");
		}

		[CanBeNull]
		public virtual string GetContrastModeDescription()
		{
			int value = _directory.GetInteger(TagContrastMode);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case unchecked((int)(0x0)):
				{
					return "Normal";
				}

				case unchecked((int)(0x1)):
				{
					return "Low";
				}

				case unchecked((int)(0x2)):
				{
					return "High";
				}

				case unchecked((int)(0x6)):
				{
					return "Medium Low";
				}

				case unchecked((int)(0x7)):
				{
					return "Medium High";
				}

				case unchecked((int)(0x100)):
				{
					return "Low";
				}

				case unchecked((int)(0x110)):
				{
					return "Normal";
				}

				case unchecked((int)(0x120)):
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
		public virtual string GetNoiseReductionDescription()
		{
			return GetIndexedDescription(TagNoiseReduction, "Standard (0)", "Low (-1)", "High (+1)", "Lowest (-2)", "Highest (+2)");
		}

		[CanBeNull]
		public virtual string GetSelfTimerDescription()
		{
			return GetIndexedDescription(TagSelfTimer, 1, "Off", "10 s", "2 s");
		}

		[CanBeNull]
		public virtual string GetRotationDescription()
		{
			int value = _directory.GetInteger(TagRotation);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 1:
				{
					return "Horizontal";
				}

				case 3:
				{
					return "Rotate 180";
				}

				case 6:
				{
					return "Rotate 90 CW";
				}

				case 8:
				{
					return "Rotate 270 CW";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetAfAssistLampDescription()
		{
			return GetIndexedDescription(TagAfAssistLamp, 1, "Fired", "Enabled but not used", "Disabled but required", "Disabled and not required");
		}

		[CanBeNull]
		public virtual string GetColorModeDescription()
		{
			return GetIndexedDescription(TagColorMode, "Normal", "Natural", "Vivid");
		}

		[CanBeNull]
		public virtual string GetOpticalZoomModeDescription()
		{
			return GetIndexedDescription(TagOpticalZoomMode, 1, "Standard", "Extended");
		}

		[CanBeNull]
		public virtual string GetConversionLensDescription()
		{
			return GetIndexedDescription(TagConversionLens, 1, "Off", "Wide", "Telephoto", "Macro");
		}

		[CanBeNull]
		public virtual string GetDetectedFacesDescription()
		{
			return BuildFacesDescription(_directory.GetDetectedFaces());
		}

		[CanBeNull]
		public virtual string GetRecognizedFacesDescription()
		{
			return BuildFacesDescription(_directory.GetRecognizedFaces());
		}

		[CanBeNull]
		private string BuildFacesDescription(Face[] faces)
		{
			if (faces == null)
			{
				return null;
			}
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < faces.Length; i++)
			{
				result.Append("Face ").Append(i + 1).Append(": ").Append(faces[i].ToString()).Append("\n");
			}
			return result.Length > 0 ? result.Substring(0, result.Length - 1) : null;
		}

		private static readonly string[] _sceneModes = new string[] { "Normal", "Portrait", "Scenery", "Sports", "Night Portrait", "Program", "Aperture Priority", "Shutter Priority", "Macro", "Spot", "Manual", "Movie Preview", "Panning", "Simple", "Color Effects"
			, "Self Portrait", "Economy", "Fireworks", "Party", "Snow", "Night Scenery", "Food", "Baby", "Soft Skin", "Candlelight", "Starry Night", "High Sensitivity", "Panorama Assist", "Underwater", "Beach", "Aerial Photo", "Sunset", "Pet", "Intelligent ISO"
			, "Clipboard", "High Speed Continuous Shooting", "Intelligent Auto", null, "Multi-aspect", null, "Transform", "Flash Burst", "Pin Hole", "Film Grain", "My Color", "Photo Frame", null, null, null, null, "HDR" };

		// 1
		// 10
		// 20
		// 30
		// 40
		// 50
		[CanBeNull]
		public virtual string GetRecordModeDescription()
		{
			return GetIndexedDescription(TagRecordMode, 1, _sceneModes);
		}

		[CanBeNull]
		public virtual string GetSceneModeDescription()
		{
			return GetIndexedDescription(TagSceneMode, 1, _sceneModes);
		}

		[CanBeNull]
		public virtual string GetFocusModeDescription()
		{
			return GetIndexedDescription(TagFocusMode, 1, "Auto", "Manual", null, "Auto, Focus Button", "Auto, Continuous");
		}

		[CanBeNull]
		public virtual string GetAfAreaModeDescription()
		{
			int[] value = _directory.GetIntArray(TagAfAreaMode);
			if (value == null || value.Length < 2)
			{
				return null;
			}
			switch (value[0])
			{
				case 0:
				{
					switch (value[1])
					{
						case 1:
						{
							return "Spot Mode On";
						}

						case 16:
						{
							return "Spot Mode Off";
						}

						default:
						{
							return "Unknown (" + value[0] + " " + value[1] + ")";
						}
					}
					goto case 1;
				}

				case 1:
				{
					switch (value[1])
					{
						case 0:
						{
							return "Spot Focusing";
						}

						case 1:
						{
							return "5-area";
						}

						default:
						{
							return "Unknown (" + value[0] + " " + value[1] + ")";
						}
					}
					goto case 16;
				}

				case 16:
				{
					switch (value[1])
					{
						case 0:
						{
							return "1-area";
						}

						case 16:
						{
							return "1-area (high speed)";
						}

						default:
						{
							return "Unknown (" + value[0] + " " + value[1] + ")";
						}
					}
					goto case 32;
				}

				case 32:
				{
					switch (value[1])
					{
						case 0:
						{
							return "Auto or Face Detect";
						}

						case 1:
						{
							return "3-area (left)";
						}

						case 2:
						{
							return "3-area (center)";
						}

						case 3:
						{
							return "3-area (right)";
						}

						default:
						{
							return "Unknown (" + value[0] + " " + value[1] + ")";
						}
					}
					goto case 64;
				}

				case 64:
				{
					return "Face Detect";
				}

				default:
				{
					return "Unknown (" + value[0] + " " + value[1] + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetQualityModeDescription()
		{
			return GetIndexedDescription(TagQualityMode, 2, "High", "Normal", null, null, "Very High", "Raw", null, "Motion Picture");
		}

		// 2
		// 9
		[CanBeNull]
		public virtual string GetVersionDescription()
		{
			return GetVersionBytesDescription(TagFirmwareVersion, 2);
		}

		[CanBeNull]
		public virtual string GetMakernoteVersionDescription()
		{
			return GetVersionBytesDescription(TagMakernoteVersion, 2);
		}

		[CanBeNull]
		public virtual string GetExifVersionDescription()
		{
			return GetVersionBytesDescription(TagExifVersion, 2);
		}

		[CanBeNull]
		public virtual string GetInternalSerialNumberDescription()
		{
			return Get7BitStringFromBytes(TagInternalSerialNumber);
		}

		[CanBeNull]
		public virtual string GetWhiteBalanceDescription()
		{
			return GetIndexedDescription(TagWhiteBalance, 1, "Auto", "Daylight", "Cloudy", "Incandescent", "Manual", null, null, "Flash", null, "Black & White", "Manual", "Shade");
		}

		// 1
		// 10
		// 12
		[CanBeNull]
		public virtual string GetBabyAgeDescription()
		{
			Age age = _directory.GetAge(TagBabyAge);
			return age == null ? null : age.ToFriendlyString();
		}

		[CanBeNull]
		public virtual string GetBabyAge1Description()
		{
			Age age = _directory.GetAge(TagBabyAge1);
			return age == null ? null : age.ToFriendlyString();
		}
	}
}
