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
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="CasioType2MakernoteDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class CasioType2MakernoteDescriptor : TagDescriptor<CasioType2MakernoteDirectory>
	{
		public CasioType2MakernoteDescriptor([NotNull] CasioType2MakernoteDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case CasioType2MakernoteDirectory.TagThumbnailDimensions:
				{
					return GetThumbnailDimensionsDescription();
				}

				case CasioType2MakernoteDirectory.TagThumbnailSize:
				{
					return GetThumbnailSizeDescription();
				}

				case CasioType2MakernoteDirectory.TagThumbnailOffset:
				{
					return GetThumbnailOffsetDescription();
				}

				case CasioType2MakernoteDirectory.TagQualityMode:
				{
					return GetQualityModeDescription();
				}

				case CasioType2MakernoteDirectory.TagImageSize:
				{
					return GetImageSizeDescription();
				}

				case CasioType2MakernoteDirectory.TagFocusMode1:
				{
					return GetFocusMode1Description();
				}

				case CasioType2MakernoteDirectory.TagIsoSensitivity:
				{
					return GetIsoSensitivityDescription();
				}

				case CasioType2MakernoteDirectory.TagWhiteBalance1:
				{
					return GetWhiteBalance1Description();
				}

				case CasioType2MakernoteDirectory.TagFocalLength:
				{
					return GetFocalLengthDescription();
				}

				case CasioType2MakernoteDirectory.TagSaturation:
				{
					return GetSaturationDescription();
				}

				case CasioType2MakernoteDirectory.TagContrast:
				{
					return GetContrastDescription();
				}

				case CasioType2MakernoteDirectory.TagSharpness:
				{
					return GetSharpnessDescription();
				}

				case CasioType2MakernoteDirectory.TagPrintImageMatchingInfo:
				{
					return GetPrintImageMatchingInfoDescription();
				}

				case CasioType2MakernoteDirectory.TagPreviewThumbnail:
				{
					return GetCasioPreviewThumbnailDescription();
				}

				case CasioType2MakernoteDirectory.TagWhiteBalanceBias:
				{
					return GetWhiteBalanceBiasDescription();
				}

				case CasioType2MakernoteDirectory.TagWhiteBalance2:
				{
					return GetWhiteBalance2Description();
				}

				case CasioType2MakernoteDirectory.TagObjectDistance:
				{
					return GetObjectDistanceDescription();
				}

				case CasioType2MakernoteDirectory.TagFlashDistance:
				{
					return GetFlashDistanceDescription();
				}

				case CasioType2MakernoteDirectory.TagRecordMode:
				{
					return GetRecordModeDescription();
				}

				case CasioType2MakernoteDirectory.TagSelfTimer:
				{
					return GetSelfTimerDescription();
				}

				case CasioType2MakernoteDirectory.TagQuality:
				{
					return GetQualityDescription();
				}

				case CasioType2MakernoteDirectory.TagFocusMode2:
				{
					return GetFocusMode2Description();
				}

				case CasioType2MakernoteDirectory.TagTimeZone:
				{
					return GetTimeZoneDescription();
				}

				case CasioType2MakernoteDirectory.TagCcdIsoSensitivity:
				{
					return GetCcdIsoSensitivityDescription();
				}

				case CasioType2MakernoteDirectory.TagColourMode:
				{
					return GetColourModeDescription();
				}

				case CasioType2MakernoteDirectory.TagEnhancement:
				{
					return GetEnhancementDescription();
				}

				case CasioType2MakernoteDirectory.TagFilter:
				{
					return GetFilterDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetFilterDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagFilter, "Off");
		}

		[CanBeNull]
		public virtual string GetEnhancementDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagEnhancement, "Off");
		}

		[CanBeNull]
		public virtual string GetColourModeDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagColourMode, "Off");
		}

		[CanBeNull]
		public virtual string GetCcdIsoSensitivityDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagCcdIsoSensitivity, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetTimeZoneDescription()
		{
			return _directory.GetString(CasioType2MakernoteDirectory.TagTimeZone);
		}

		[CanBeNull]
		public virtual string GetFocusMode2Description()
		{
			int? value = _directory.GetInteger(CasioType2MakernoteDirectory.TagFocusMode2);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 1:
				{
					return "Fixation";
				}

				case 6:
				{
					return "Multi-Area Focus";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetQualityDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagQuality, 3, "Fine");
		}

		[CanBeNull]
		public virtual string GetSelfTimerDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagSelfTimer, 1, "Off");
		}

		[CanBeNull]
		public virtual string GetRecordModeDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagRecordMode, 2, "Normal");
		}

		[CanBeNull]
		public virtual string GetFlashDistanceDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagFlashDistance, "Off");
		}

		[CanBeNull]
		public virtual string GetObjectDistanceDescription()
		{
			int? value = _directory.GetInteger(CasioType2MakernoteDirectory.TagObjectDistance);
			if (value == null)
			{
				return null;
			}
			return Sharpen.Extensions.ConvertToString((int)value) + " mm";
		}

		[CanBeNull]
		public virtual string GetWhiteBalance2Description()
		{
			int? value = _directory.GetInteger(CasioType2MakernoteDirectory.TagWhiteBalance2);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "Manual";
				}

				case 1:
				{
					return "Auto";
				}

				case 4:
				{
					// unsure about this
					return "Flash";
				}

				case 12:
				{
					// unsure about this
					return "Flash";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetWhiteBalanceBiasDescription()
		{
			return _directory.GetString(CasioType2MakernoteDirectory.TagWhiteBalanceBias);
		}

		[CanBeNull]
		public virtual string GetCasioPreviewThumbnailDescription()
		{
			sbyte[] bytes = _directory.GetByteArray(CasioType2MakernoteDirectory.TagPreviewThumbnail);
			if (bytes == null)
			{
				return null;
			}
			return "<" + bytes.Length + " bytes of image data>";
		}

		[CanBeNull]
		public virtual string GetPrintImageMatchingInfoDescription()
		{
			// TODO research PIM specification http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html
			return _directory.GetString(CasioType2MakernoteDirectory.TagPrintImageMatchingInfo);
		}

		[CanBeNull]
		public virtual string GetSharpnessDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagSharpness, "-1", "Normal", "+1");
		}

		[CanBeNull]
		public virtual string GetContrastDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagContrast, "-1", "Normal", "+1");
		}

		[CanBeNull]
		public virtual string GetSaturationDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagSaturation, "-1", "Normal", "+1");
		}

		[CanBeNull]
		public virtual string GetFocalLengthDescription()
		{
			double? value = _directory.GetDoubleObject(CasioType2MakernoteDirectory.TagFocalLength);
			if (value == null)
			{
				return null;
			}
			return Sharpen.Extensions.ConvertToString((double)value / 10d) + " mm";
		}

		[CanBeNull]
		public virtual string GetWhiteBalance1Description()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagWhiteBalance1, "Auto", "Daylight", "Shade", "Tungsten", "Florescent", "Manual");
		}

		[CanBeNull]
		public virtual string GetIsoSensitivityDescription()
		{
			int? value = _directory.GetInteger(CasioType2MakernoteDirectory.TagIsoSensitivity);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 3:
				{
					return "50";
				}

				case 4:
				{
					return "64";
				}

				case 6:
				{
					return "100";
				}

				case 9:
				{
					return "200";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetFocusMode1Description()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagFocusMode1, "Normal", "Macro");
		}

		[CanBeNull]
		public virtual string GetImageSizeDescription()
		{
			int? value = _directory.GetInteger(CasioType2MakernoteDirectory.TagImageSize);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "640 x 480 pixels";
				}

				case 4:
				{
					return "1600 x 1200 pixels";
				}

				case 5:
				{
					return "2048 x 1536 pixels";
				}

				case 20:
				{
					return "2288 x 1712 pixels";
				}

				case 21:
				{
					return "2592 x 1944 pixels";
				}

				case 22:
				{
					return "2304 x 1728 pixels";
				}

				case 36:
				{
					return "3008 x 2008 pixels";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetQualityModeDescription()
		{
			return GetIndexedDescription(CasioType2MakernoteDirectory.TagQualityMode, 1, "Fine", "Super Fine");
		}

		[CanBeNull]
		public virtual string GetThumbnailOffsetDescription()
		{
			return _directory.GetString(CasioType2MakernoteDirectory.TagThumbnailOffset);
		}

		[CanBeNull]
		public virtual string GetThumbnailSizeDescription()
		{
			int? value = _directory.GetInteger(CasioType2MakernoteDirectory.TagThumbnailSize);
			if (value == null)
			{
				return null;
			}
			return Sharpen.Extensions.ConvertToString((int)value) + " bytes";
		}

		[CanBeNull]
		public virtual string GetThumbnailDimensionsDescription()
		{
			int[] dimensions = _directory.GetIntArray(CasioType2MakernoteDirectory.TagThumbnailDimensions);
			if (dimensions == null || dimensions.Length != 2)
			{
				return _directory.GetString(CasioType2MakernoteDirectory.TagThumbnailDimensions);
			}
			return dimensions[0] + " x " + dimensions[1] + " pixels";
		}
	}
}
