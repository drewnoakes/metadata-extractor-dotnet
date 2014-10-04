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
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="CasioType2MakernoteDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class CasioType2MakernoteDescriptor : TagDescriptor<CasioType2MakernoteDirectory>
	{
		public CasioType2MakernoteDescriptor(CasioType2MakernoteDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case TagThumbnailDimensions:
				{
					return GetThumbnailDimensionsDescription();
				}

				case TagThumbnailSize:
				{
					return GetThumbnailSizeDescription();
				}

				case TagThumbnailOffset:
				{
					return GetThumbnailOffsetDescription();
				}

				case TagQualityMode:
				{
					return GetQualityModeDescription();
				}

				case TagImageSize:
				{
					return GetImageSizeDescription();
				}

				case TagFocusMode1:
				{
					return GetFocusMode1Description();
				}

				case TagIsoSensitivity:
				{
					return GetIsoSensitivityDescription();
				}

				case TagWhiteBalance1:
				{
					return GetWhiteBalance1Description();
				}

				case TagFocalLength:
				{
					return GetFocalLengthDescription();
				}

				case TagSaturation:
				{
					return GetSaturationDescription();
				}

				case TagContrast:
				{
					return GetContrastDescription();
				}

				case TagSharpness:
				{
					return GetSharpnessDescription();
				}

				case TagPrintImageMatchingInfo:
				{
					return GetPrintImageMatchingInfoDescription();
				}

				case TagPreviewThumbnail:
				{
					return GetCasioPreviewThumbnailDescription();
				}

				case TagWhiteBalanceBias:
				{
					return GetWhiteBalanceBiasDescription();
				}

				case TagWhiteBalance2:
				{
					return GetWhiteBalance2Description();
				}

				case TagObjectDistance:
				{
					return GetObjectDistanceDescription();
				}

				case TagFlashDistance:
				{
					return GetFlashDistanceDescription();
				}

				case TagRecordMode:
				{
					return GetRecordModeDescription();
				}

				case TagSelfTimer:
				{
					return GetSelfTimerDescription();
				}

				case TagQuality:
				{
					return GetQualityDescription();
				}

				case TagFocusMode2:
				{
					return GetFocusMode2Description();
				}

				case TagTimeZone:
				{
					return GetTimeZoneDescription();
				}

				case TagCcdIsoSensitivity:
				{
					return GetCcdIsoSensitivityDescription();
				}

				case TagColourMode:
				{
					return GetColourModeDescription();
				}

				case TagEnhancement:
				{
					return GetEnhancementDescription();
				}

				case TagFilter:
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
			return GetIndexedDescription(TagFilter, "Off");
		}

		[CanBeNull]
		public virtual string GetEnhancementDescription()
		{
			return GetIndexedDescription(TagEnhancement, "Off");
		}

		[CanBeNull]
		public virtual string GetColourModeDescription()
		{
			return GetIndexedDescription(TagColourMode, "Off");
		}

		[CanBeNull]
		public virtual string GetCcdIsoSensitivityDescription()
		{
			return GetIndexedDescription(TagCcdIsoSensitivity, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetTimeZoneDescription()
		{
			return _directory.GetString(TagTimeZone);
		}

		[CanBeNull]
		public virtual string GetFocusMode2Description()
		{
			int value = _directory.GetInteger(TagFocusMode2);
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
			return GetIndexedDescription(TagQuality, 3, "Fine");
		}

		[CanBeNull]
		public virtual string GetSelfTimerDescription()
		{
			return GetIndexedDescription(TagSelfTimer, 1, "Off");
		}

		[CanBeNull]
		public virtual string GetRecordModeDescription()
		{
			return GetIndexedDescription(TagRecordMode, 2, "Normal");
		}

		[CanBeNull]
		public virtual string GetFlashDistanceDescription()
		{
			return GetIndexedDescription(TagFlashDistance, "Off");
		}

		[CanBeNull]
		public virtual string GetObjectDistanceDescription()
		{
			int value = _directory.GetInteger(TagObjectDistance);
			if (value == null)
			{
				return null;
			}
			return Sharpen.Extensions.ToString(value) + " mm";
		}

		[CanBeNull]
		public virtual string GetWhiteBalance2Description()
		{
			int value = _directory.GetInteger(TagWhiteBalance2);
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
			return _directory.GetString(TagWhiteBalanceBias);
		}

		[CanBeNull]
		public virtual string GetCasioPreviewThumbnailDescription()
		{
			sbyte[] bytes = _directory.GetByteArray(TagPreviewThumbnail);
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
			return _directory.GetString(TagPrintImageMatchingInfo);
		}

		[CanBeNull]
		public virtual string GetSharpnessDescription()
		{
			return GetIndexedDescription(TagSharpness, "-1", "Normal", "+1");
		}

		[CanBeNull]
		public virtual string GetContrastDescription()
		{
			return GetIndexedDescription(TagContrast, "-1", "Normal", "+1");
		}

		[CanBeNull]
		public virtual string GetSaturationDescription()
		{
			return GetIndexedDescription(TagSaturation, "-1", "Normal", "+1");
		}

		[CanBeNull]
		public virtual string GetFocalLengthDescription()
		{
			double value = _directory.GetDoubleObject(TagFocalLength);
			if (value == null)
			{
				return null;
			}
			return (value / 10d).ToString() + " mm";
		}

		[CanBeNull]
		public virtual string GetWhiteBalance1Description()
		{
			return GetIndexedDescription(TagWhiteBalance1, "Auto", "Daylight", "Shade", "Tungsten", "Florescent", "Manual");
		}

		[CanBeNull]
		public virtual string GetIsoSensitivityDescription()
		{
			int value = _directory.GetInteger(TagIsoSensitivity);
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
			return GetIndexedDescription(TagFocusMode1, "Normal", "Macro");
		}

		[CanBeNull]
		public virtual string GetImageSizeDescription()
		{
			int value = _directory.GetInteger(TagImageSize);
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
			return GetIndexedDescription(TagQualityMode, 1, "Fine", "Super Fine");
		}

		[CanBeNull]
		public virtual string GetThumbnailOffsetDescription()
		{
			return _directory.GetString(TagThumbnailOffset);
		}

		[CanBeNull]
		public virtual string GetThumbnailSizeDescription()
		{
			int value = _directory.GetInteger(TagThumbnailSize);
			if (value == null)
			{
				return null;
			}
			return Sharpen.Extensions.ToString(value) + " bytes";
		}

		[CanBeNull]
		public virtual string GetThumbnailDimensionsDescription()
		{
			int[] dimensions = _directory.GetIntArray(TagThumbnailDimensions);
			if (dimensions == null || dimensions.Length != 2)
			{
				return _directory.GetString(TagThumbnailDimensions);
			}
			return dimensions[0] + " x " + dimensions[1] + " pixels";
		}
	}
}
