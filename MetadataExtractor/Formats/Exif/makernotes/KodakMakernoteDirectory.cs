// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Kodak cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagKodakModel, "Kodak Model" },
            { TagQuality, "Quality" },
            { TagBurstMode, "Burst Mode" },
            { TagImageWidth, "Image Width" },
            { TagImageHeight, "Image Height" },
            { TagYearCreated, "Year Created" },
            { TagMonthDayCreated, "Month/Day Created" },
            { TagTimeCreated, "Time Created" },
            { TagBurstMode2, "Burst Mode 2" },
            { TagShutterMode, "Shutter Speed" },
            { TagMeteringMode, "Metering Mode" },
            { TagSequenceNumber, "Sequence Number" },
            { TagFNumber, "F Number" },
            { TagExposureTime, "Exposure Time" },
            { TagExposureCompensation, "Exposure Compensation" },
            { TagFocusMode, "Focus Mode" },
            { TagWhiteBalance, "White Balance" },
            { TagFlashMode, "Flash Mode" },
            { TagFlashFired, "Flash Fired" },
            { TagIsoSetting, "ISO Setting" },
            { TagIso, "ISO" },
            { TagTotalZoom, "Total Zoom" },
            { TagDateTimeStamp, "Date/Time Stamp" },
            { TagColorMode, "Color Mode" },
            { TagDigitalZoom, "Digital Zoom" },
            { TagSharpness, "Sharpness" }
        };

        public KodakMakernoteDirectory()
        {
            SetDescriptor(new KodakMakernoteDescriptor(this));
        }

        public override string Name => "Kodak Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
