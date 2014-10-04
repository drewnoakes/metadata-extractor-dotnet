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
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="GpsDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class GpsDescriptor : TagDescriptor<GpsDirectory>
	{
		public GpsDescriptor(GpsDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case TagVersionId:
				{
					return GetGpsVersionIdDescription();
				}

				case TagAltitude:
				{
					return GetGpsAltitudeDescription();
				}

				case TagAltitudeRef:
				{
					return GetGpsAltitudeRefDescription();
				}

				case TagStatus:
				{
					return GetGpsStatusDescription();
				}

				case TagMeasureMode:
				{
					return GetGpsMeasureModeDescription();
				}

				case TagSpeedRef:
				{
					return GetGpsSpeedRefDescription();
				}

				case TagTrackRef:
				case TagImgDirectionRef:
				case TagDestBearingRef:
				{
					return GetGpsDirectionReferenceDescription(tagType);
				}

				case TagTrack:
				case TagImgDirection:
				case TagDestBearing:
				{
					return GetGpsDirectionDescription(tagType);
				}

				case TagDestDistanceRef:
				{
					return GetGpsDestinationReferenceDescription();
				}

				case TagTimeStamp:
				{
					return GetGpsTimeStampDescription();
				}

				case TagLongitude:
				{
					// three rational numbers -- displayed in HH"MM"SS.ss
					return GetGpsLongitudeDescription();
				}

				case TagLatitude:
				{
					// three rational numbers -- displayed in HH"MM"SS.ss
					return GetGpsLatitudeDescription();
				}

				case TagDifferential:
				{
					return GetGpsDifferentialDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		private string GetGpsVersionIdDescription()
		{
			return GetVersionBytesDescription(TagVersionId, 1);
		}

		[CanBeNull]
		public virtual string GetGpsLatitudeDescription()
		{
			GeoLocation location = _directory.GetGeoLocation();
			return location == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString(location.GetLatitude());
		}

		[CanBeNull]
		public virtual string GetGpsLongitudeDescription()
		{
			GeoLocation location = _directory.GetGeoLocation();
			return location == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString(location.GetLongitude());
		}

		[CanBeNull]
		public virtual string GetGpsTimeStampDescription()
		{
			// time in hour, min, sec
			int[] timeComponents = _directory.GetIntArray(TagTimeStamp);
			return timeComponents == null ? null : Sharpen.Extensions.StringFormat("%d:%d:%d UTC", timeComponents[0], timeComponents[1], timeComponents[2]);
		}

		[CanBeNull]
		public virtual string GetGpsDestinationReferenceDescription()
		{
			string value = _directory.GetString(TagDestDistanceRef);
			if (value == null)
			{
				return null;
			}
			string distanceRef = Sharpen.Extensions.Trim(value);
			if (Sharpen.Runtime.EqualsIgnoreCase("K", distanceRef))
			{
				return "kilometers";
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("M", distanceRef))
				{
					return "miles";
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase("N", distanceRef))
					{
						return "knots";
					}
					else
					{
						return "Unknown (" + distanceRef + ")";
					}
				}
			}
		}

		[CanBeNull]
		public virtual string GetGpsDirectionDescription(int tagType)
		{
			Rational angle = _directory.GetRational(tagType);
			// provide a decimal version of rational numbers in the description, to avoid strings like "35334/199 degrees"
			string value = angle != null ? new DecimalFormat("0.##").Format(angle.DoubleValue()) : _directory.GetString(tagType);
			return value == null || Sharpen.Extensions.Trim(value).Length == 0 ? null : Sharpen.Extensions.Trim(value) + " degrees";
		}

		[CanBeNull]
		public virtual string GetGpsDirectionReferenceDescription(int tagType)
		{
			string value = _directory.GetString(tagType);
			if (value == null)
			{
				return null;
			}
			string gpsDistRef = Sharpen.Extensions.Trim(value);
			if (Sharpen.Runtime.EqualsIgnoreCase("T", gpsDistRef))
			{
				return "True direction";
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("M", gpsDistRef))
				{
					return "Magnetic direction";
				}
				else
				{
					return "Unknown (" + gpsDistRef + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetGpsSpeedRefDescription()
		{
			string value = _directory.GetString(TagSpeedRef);
			if (value == null)
			{
				return null;
			}
			string gpsSpeedRef = Sharpen.Extensions.Trim(value);
			if (Sharpen.Runtime.EqualsIgnoreCase("K", gpsSpeedRef))
			{
				return "kph";
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("M", gpsSpeedRef))
				{
					return "mph";
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase("N", gpsSpeedRef))
					{
						return "knots";
					}
					else
					{
						return "Unknown (" + gpsSpeedRef + ")";
					}
				}
			}
		}

		[CanBeNull]
		public virtual string GetGpsMeasureModeDescription()
		{
			string value = _directory.GetString(TagMeasureMode);
			if (value == null)
			{
				return null;
			}
			string gpsSpeedMeasureMode = Sharpen.Extensions.Trim(value);
			if (Sharpen.Runtime.EqualsIgnoreCase("2", gpsSpeedMeasureMode))
			{
				return "2-dimensional measurement";
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("3", gpsSpeedMeasureMode))
				{
					return "3-dimensional measurement";
				}
				else
				{
					return "Unknown (" + gpsSpeedMeasureMode + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetGpsStatusDescription()
		{
			string value = _directory.GetString(TagStatus);
			if (value == null)
			{
				return null;
			}
			string gpsStatus = Sharpen.Extensions.Trim(value);
			if (Sharpen.Runtime.EqualsIgnoreCase("A", gpsStatus))
			{
				return "Active (Measurement in progress)";
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("V", gpsStatus))
				{
					return "Void (Measurement Interoperability)";
				}
				else
				{
					return "Unknown (" + gpsStatus + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetGpsAltitudeRefDescription()
		{
			return GetIndexedDescription(TagAltitudeRef, "Sea level", "Below sea level");
		}

		[CanBeNull]
		public virtual string GetGpsAltitudeDescription()
		{
			Rational value = _directory.GetRational(TagAltitude);
			return value == null ? null : value.IntValue() + " metres";
		}

		[CanBeNull]
		public virtual string GetGpsDifferentialDescription()
		{
			return GetIndexedDescription(TagDifferential, "No Correction", "Differential Corrected");
		}

		[CanBeNull]
		public virtual string GetDegreesMinutesSecondsDescription()
		{
			GeoLocation location = _directory.GetGeoLocation();
			return location == null ? null : location.ToDMSString();
		}
	}
}
