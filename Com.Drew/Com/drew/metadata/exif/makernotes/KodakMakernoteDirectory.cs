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
using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>Describes tags specific to Kodak cameras.</summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class KodakMakernoteDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagKodakModel = 0;

		public const int TagQuality = 9;

		public const int TagBurstMode = 10;

		public const int TagImageWidth = 12;

		public const int TagImageHeight = 14;

		public const int TagYearCreated = 16;

		public const int TagMonthDayCreated = 18;

		public const int TagTimeCreated = 20;

		public const int TagBurstMode2 = 24;

		public const int TagShutterMode = 27;

		public const int TagMeteringMode = 28;

		public const int TagSequenceNumber = 29;

		public const int TagFNumber = 30;

		public const int TagExposureTime = 32;

		public const int TagExposureCompensation = 36;

		public const int TagFocusMode = 56;

		public const int TagWhiteBalance = 64;

		public const int TagFlashMode = 92;

		public const int TagFlashFired = 93;

		public const int TagIsoSetting = 94;

		public const int TagIso = 96;

		public const int TagTotalZoom = 98;

		public const int TagDateTimeStamp = 100;

		public const int TagColorMode = 102;

		public const int TagDigitalZoom = 104;

		public const int TagSharpness = 107;

		[NotNull]
		protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

		static KodakMakernoteDirectory()
		{
			_tagNameMap.Put(TagKodakModel, "Kodak Model");
			_tagNameMap.Put(TagQuality, "Quality");
			_tagNameMap.Put(TagBurstMode, "Burst Mode");
			_tagNameMap.Put(TagImageWidth, "Image Width");
			_tagNameMap.Put(TagImageHeight, "Image Height");
			_tagNameMap.Put(TagYearCreated, "Year Created");
			_tagNameMap.Put(TagMonthDayCreated, "Month/Day Created");
			_tagNameMap.Put(TagTimeCreated, "Time Created");
			_tagNameMap.Put(TagBurstMode2, "Burst Mode 2");
			_tagNameMap.Put(TagShutterMode, "Shutter Speed");
			_tagNameMap.Put(TagMeteringMode, "Metering Mode");
			_tagNameMap.Put(TagSequenceNumber, "Sequence Number");
			_tagNameMap.Put(TagFNumber, "F Number");
			_tagNameMap.Put(TagExposureTime, "Exposure Time");
			_tagNameMap.Put(TagExposureCompensation, "Exposure Compensation");
			_tagNameMap.Put(TagFocusMode, "Focus Mode");
			_tagNameMap.Put(TagWhiteBalance, "White Balance");
			_tagNameMap.Put(TagFlashMode, "Flash Mode");
			_tagNameMap.Put(TagFlashFired, "Flash Fired");
			_tagNameMap.Put(TagIsoSetting, "ISO Setting");
			_tagNameMap.Put(TagIso, "ISO");
			_tagNameMap.Put(TagTotalZoom, "Total Zoom");
			_tagNameMap.Put(TagDateTimeStamp, "Date/Time Stamp");
			_tagNameMap.Put(TagColorMode, "Color Mode");
			_tagNameMap.Put(TagDigitalZoom, "Digital Zoom");
			_tagNameMap.Put(TagSharpness, "Sharpness");
		}

		public KodakMakernoteDirectory()
		{
			this.SetDescriptor(new KodakMakernoteDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Kodak Makernote";
		}

		[NotNull]
		protected internal override Dictionary<int?, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
