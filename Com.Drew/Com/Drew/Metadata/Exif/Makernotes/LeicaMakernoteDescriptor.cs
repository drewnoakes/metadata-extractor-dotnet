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
	/// <see cref="LeicaMakernoteDirectory"/>
	/// .
	/// <p/>
	/// Tag reference from: http://gvsoft.homedns.org/exif/makernote-leica-type1.html
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class LeicaMakernoteDescriptor : TagDescriptor<LeicaMakernoteDirectory>
	{
		public LeicaMakernoteDescriptor(LeicaMakernoteDirectory directory)
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

				case TagUserProfile:
				{
					return GetUserProfileDescription();
				}

				case TagWhiteBalance:
				{
					//            case TAG_SERIAL:
					//                return getSerialNumberDescription();
					return GetWhiteBalanceDescription();
				}

				case TagExternalSensorBrightnessValue:
				{
					return GetExternalSensorBrightnessValueDescription();
				}

				case TagMeasuredLv:
				{
					return GetMeasuredLvDescription();
				}

				case TagApproximateFNumber:
				{
					return GetApproximateFNumberDescription();
				}

				case TagCameraTemperature:
				{
					return GetCameraTemperatureDescription();
				}

				case TagWbRedLevel:
				case TagWbBlueLevel:
				case TagWbGreenLevel:
				{
					return GetSimpleRational(tagType);
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		private string GetCameraTemperatureDescription()
		{
			return GetFormattedInt(TagCameraTemperature, "%d C");
		}

		[CanBeNull]
		private string GetApproximateFNumberDescription()
		{
			return GetSimpleRational(TagApproximateFNumber);
		}

		[CanBeNull]
		private string GetMeasuredLvDescription()
		{
			return GetSimpleRational(TagMeasuredLv);
		}

		[CanBeNull]
		private string GetExternalSensorBrightnessValueDescription()
		{
			return GetSimpleRational(TagExternalSensorBrightnessValue);
		}

		[CanBeNull]
		private string GetWhiteBalanceDescription()
		{
			return GetIndexedDescription(TagWhiteBalance, "Auto or Manual", "Daylight", "Fluorescent", "Tungsten", "Flash", "Cloudy", "Shadow");
		}

		[CanBeNull]
		private string GetUserProfileDescription()
		{
			return GetIndexedDescription(TagQuality, 1, "User Profile 1", "User Profile 2", "User Profile 3", "User Profile 0 (Dynamic)");
		}

		[CanBeNull]
		private string GetQualityDescription()
		{
			return GetIndexedDescription(TagQuality, 1, "Fine", "Basic");
		}
	}
}
