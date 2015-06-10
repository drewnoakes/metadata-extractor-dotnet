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

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Kodak cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class KodakMakernoteDirectory : Directory
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
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static KodakMakernoteDirectory()
        {
            TagNameMap[TagKodakModel] = "Kodak Model";
            TagNameMap[TagQuality] = "Quality";
            TagNameMap[TagBurstMode] = "Burst Mode";
            TagNameMap[TagImageWidth] = "Image Width";
            TagNameMap[TagImageHeight] = "Image Height";
            TagNameMap[TagYearCreated] = "Year Created";
            TagNameMap[TagMonthDayCreated] = "Month/Day Created";
            TagNameMap[TagTimeCreated] = "Time Created";
            TagNameMap[TagBurstMode2] = "Burst Mode 2";
            TagNameMap[TagShutterMode] = "Shutter Speed";
            TagNameMap[TagMeteringMode] = "Metering Mode";
            TagNameMap[TagSequenceNumber] = "Sequence Number";
            TagNameMap[TagFNumber] = "F Number";
            TagNameMap[TagExposureTime] = "Exposure Time";
            TagNameMap[TagExposureCompensation] = "Exposure Compensation";
            TagNameMap[TagFocusMode] = "Focus Mode";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagFlashMode] = "Flash Mode";
            TagNameMap[TagFlashFired] = "Flash Fired";
            TagNameMap[TagIsoSetting] = "ISO Setting";
            TagNameMap[TagIso] = "ISO";
            TagNameMap[TagTotalZoom] = "Total Zoom";
            TagNameMap[TagDateTimeStamp] = "Date/Time Stamp";
            TagNameMap[TagColorMode] = "Color Mode";
            TagNameMap[TagDigitalZoom] = "Digital Zoom";
            TagNameMap[TagSharpness] = "Sharpness";
        }

        public KodakMakernoteDirectory()
        {
            SetDescriptor(new KodakMakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Kodak Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
