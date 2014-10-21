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
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>Describes Exif tags that contain Global Positioning System (GPS) data.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class GpsDirectory : Com.Drew.Metadata.Directory
	{
		/// <summary>GPS tag version GPSVersionID 0 0 BYTE 4</summary>
		public const int TagVersionId = unchecked((int)(0x0000));

		/// <summary>North or South Latitude GPSLatitudeRef 1 1 ASCII 2</summary>
		public const int TagLatitudeRef = unchecked((int)(0x0001));

		/// <summary>Latitude GPSLatitude 2 2 RATIONAL 3</summary>
		public const int TagLatitude = unchecked((int)(0x0002));

		/// <summary>East or West Longitude GPSLongitudeRef 3 3 ASCII 2</summary>
		public const int TagLongitudeRef = unchecked((int)(0x0003));

		/// <summary>Longitude GPSLongitude 4 4 RATIONAL 3</summary>
		public const int TagLongitude = unchecked((int)(0x0004));

		/// <summary>Altitude reference GPSAltitudeRef 5 5 BYTE 1</summary>
		public const int TagAltitudeRef = unchecked((int)(0x0005));

		/// <summary>Altitude GPSAltitude 6 6 RATIONAL 1</summary>
		public const int TagAltitude = unchecked((int)(0x0006));

		/// <summary>GPS time (atomic clock) GPSTimeStamp 7 7 RATIONAL 3</summary>
		public const int TagTimeStamp = unchecked((int)(0x0007));

		/// <summary>GPS satellites used for measurement GPSSatellites 8 8 ASCII Any</summary>
		public const int TagSatellites = unchecked((int)(0x0008));

		/// <summary>GPS receiver status GPSStatus 9 9 ASCII 2</summary>
		public const int TagStatus = unchecked((int)(0x0009));

		/// <summary>GPS measurement mode GPSMeasureMode 10 A ASCII 2</summary>
		public const int TagMeasureMode = unchecked((int)(0x000A));

		/// <summary>Measurement precision GPSDOP 11 B RATIONAL 1</summary>
		public const int TagDop = unchecked((int)(0x000B));

		/// <summary>Speed unit GPSSpeedRef 12 C ASCII 2</summary>
		public const int TagSpeedRef = unchecked((int)(0x000C));

		/// <summary>Speed of GPS receiver GPSSpeed 13 D RATIONAL 1</summary>
		public const int TagSpeed = unchecked((int)(0x000D));

		/// <summary>Reference for direction of movement GPSTrackRef 14 E ASCII 2</summary>
		public const int TagTrackRef = unchecked((int)(0x000E));

		/// <summary>Direction of movement GPSTrack 15 F RATIONAL 1</summary>
		public const int TagTrack = unchecked((int)(0x000F));

		/// <summary>Reference for direction of image GPSImgDirectionRef 16 10 ASCII 2</summary>
		public const int TagImgDirectionRef = unchecked((int)(0x0010));

		/// <summary>Direction of image GPSImgDirection 17 11 RATIONAL 1</summary>
		public const int TagImgDirection = unchecked((int)(0x0011));

		/// <summary>Geodetic survey data used GPSMapDatum 18 12 ASCII Any</summary>
		public const int TagMapDatum = unchecked((int)(0x0012));

		/// <summary>Reference for latitude of destination GPSDestLatitudeRef 19 13 ASCII 2</summary>
		public const int TagDestLatitudeRef = unchecked((int)(0x0013));

		/// <summary>Latitude of destination GPSDestLatitude 20 14 RATIONAL 3</summary>
		public const int TagDestLatitude = unchecked((int)(0x0014));

		/// <summary>Reference for longitude of destination GPSDestLongitudeRef 21 15 ASCII 2</summary>
		public const int TagDestLongitudeRef = unchecked((int)(0x0015));

		/// <summary>Longitude of destination GPSDestLongitude 22 16 RATIONAL 3</summary>
		public const int TagDestLongitude = unchecked((int)(0x0016));

		/// <summary>Reference for bearing of destination GPSDestBearingRef 23 17 ASCII 2</summary>
		public const int TagDestBearingRef = unchecked((int)(0x0017));

		/// <summary>Bearing of destination GPSDestBearing 24 18 RATIONAL 1</summary>
		public const int TagDestBearing = unchecked((int)(0x0018));

		/// <summary>Reference for distance to destination GPSDestDistanceRef 25 19 ASCII 2</summary>
		public const int TagDestDistanceRef = unchecked((int)(0x0019));

		/// <summary>Distance to destination GPSDestDistance 26 1A RATIONAL 1</summary>
		public const int TagDestDistance = unchecked((int)(0x001A));

		/// <summary>Values of "GPS", "CELLID", "WLAN" or "MANUAL" by the EXIF spec.</summary>
		public const int TagProcessingMethod = unchecked((int)(0x001B));

		public const int TagAreaInformation = unchecked((int)(0x001C));

		public const int TagDateStamp = unchecked((int)(0x001D));

		public const int TagDifferential = unchecked((int)(0x001E));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static GpsDirectory()
		{
			_tagNameMap.Put(TagVersionId, "GPS Version ID");
			_tagNameMap.Put(TagLatitudeRef, "GPS Latitude Ref");
			_tagNameMap.Put(TagLatitude, "GPS Latitude");
			_tagNameMap.Put(TagLongitudeRef, "GPS Longitude Ref");
			_tagNameMap.Put(TagLongitude, "GPS Longitude");
			_tagNameMap.Put(TagAltitudeRef, "GPS Altitude Ref");
			_tagNameMap.Put(TagAltitude, "GPS Altitude");
			_tagNameMap.Put(TagTimeStamp, "GPS Time-Stamp");
			_tagNameMap.Put(TagSatellites, "GPS Satellites");
			_tagNameMap.Put(TagStatus, "GPS Status");
			_tagNameMap.Put(TagMeasureMode, "GPS Measure Mode");
			_tagNameMap.Put(TagDop, "GPS DOP");
			_tagNameMap.Put(TagSpeedRef, "GPS Speed Ref");
			_tagNameMap.Put(TagSpeed, "GPS Speed");
			_tagNameMap.Put(TagTrackRef, "GPS Track Ref");
			_tagNameMap.Put(TagTrack, "GPS Track");
			_tagNameMap.Put(TagImgDirectionRef, "GPS Img Direction Ref");
			_tagNameMap.Put(TagImgDirection, "GPS Img Direction");
			_tagNameMap.Put(TagMapDatum, "GPS Map Datum");
			_tagNameMap.Put(TagDestLatitudeRef, "GPS Dest Latitude Ref");
			_tagNameMap.Put(TagDestLatitude, "GPS Dest Latitude");
			_tagNameMap.Put(TagDestLongitudeRef, "GPS Dest Longitude Ref");
			_tagNameMap.Put(TagDestLongitude, "GPS Dest Longitude");
			_tagNameMap.Put(TagDestBearingRef, "GPS Dest Bearing Ref");
			_tagNameMap.Put(TagDestBearing, "GPS Dest Bearing");
			_tagNameMap.Put(TagDestDistanceRef, "GPS Dest Distance Ref");
			_tagNameMap.Put(TagDestDistance, "GPS Dest Distance");
			_tagNameMap.Put(TagProcessingMethod, "GPS Processing Method");
			_tagNameMap.Put(TagAreaInformation, "GPS Area Information");
			_tagNameMap.Put(TagDateStamp, "GPS Date Stamp");
			_tagNameMap.Put(TagDifferential, "GPS Differential");
		}

		public GpsDirectory()
		{
			this.SetDescriptor(new GpsDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "GPS";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}

		/// <summary>
		/// Parses various tags in an attempt to obtain a single object representing the latitude and longitude
		/// at which this image was captured.
		/// </summary>
		/// <returns>The geographical location of this image, if possible, otherwise null</returns>
		[CanBeNull]
		public virtual GeoLocation GetGeoLocation()
		{
			Rational[] latitudes = GetRationalArray(Com.Drew.Metadata.Exif.GpsDirectory.TagLatitude);
			Rational[] longitudes = GetRationalArray(Com.Drew.Metadata.Exif.GpsDirectory.TagLongitude);
			string latitudeRef = GetString(Com.Drew.Metadata.Exif.GpsDirectory.TagLatitudeRef);
			string longitudeRef = GetString(Com.Drew.Metadata.Exif.GpsDirectory.TagLongitudeRef);
			// Make sure we have the required values
			if (latitudes == null || latitudes.Length != 3)
			{
				return null;
			}
			if (longitudes == null || longitudes.Length != 3)
			{
				return null;
			}
			if (latitudeRef == null || longitudeRef == null)
			{
				return null;
			}
			double? lat = GeoLocation.DegreesMinutesSecondsToDecimal(latitudes[0], latitudes[1], latitudes[2], Sharpen.Runtime.EqualsIgnoreCase(latitudeRef, "S"));
			double? lon = GeoLocation.DegreesMinutesSecondsToDecimal(longitudes[0], longitudes[1], longitudes[2], Sharpen.Runtime.EqualsIgnoreCase(longitudeRef, "W"));
			// This can return null, in cases where the conversion was not possible
			if (lat == null || lon == null)
			{
				return null;
			}
			return new GeoLocation(lat.Value, lon.Value);
		}
	}
}
