#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Pentax and Asahi cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PentaxMakernoteDirectory : Directory
    {
        /// <summary>
        /// 0 = Auto
        /// 1 = Night-scene
        /// 2 = Manual
        /// 4 = Multiple
        /// </summary>
        public const int TagCaptureMode = 0x0001;

        /// <summary>
        /// 0 = Good
        /// 1 = Better
        /// 2 = Best
        /// </summary>
        public const int TagQualityLevel = 0x0002;

        /// <summary>
        /// 2 = Custom
        /// 3 = Auto
        /// </summary>
        public const int TagFocusMode = 0x0003;

        /// <summary>
        /// 1 = Auto
        /// 2 = Flash on
        /// 4 = Flash off
        /// 6 = Red-eye Reduction
        /// </summary>
        public const int TagFlashMode = 0x0004;

        /// <summary>
        /// 0 = Auto
        /// 1 = Daylight
        /// 2 = Shade
        /// 3 = Tungsten
        /// 4 = Fluorescent
        /// 5 = Manual
        /// </summary>
        public const int TagWhiteBalance = 0x0007;

        /// <summary>(0 = Off)</summary>
        public const int TagDigitalZoom = 0x000A;

        /// <summary>
        /// 0 = Normal
        /// 1 = Soft
        /// 2 = Hard
        /// </summary>
        public const int TagSharpness = 0x000B;

        /// <summary>
        /// 0 = Normal
        /// 1 = Low
        /// 2 = High
        /// </summary>
        public const int TagContrast = 0x000C;

        /// <summary>
        /// 0 = Normal
        /// 1 = Low
        /// 2 = High
        /// </summary>
        public const int TagSaturation = 0x000D;

        /// <summary>
        /// 10 = ISO 100
        /// 16 = ISO 200
        /// 100 = ISO 100
        /// 200 = ISO 200
        /// </summary>
        public const int TagIsoSpeed = 0x0014;

        /// <summary>
        /// 1 = Normal
        /// 2 = Black &amp; White
        /// 3 = Sepia
        /// </summary>
        public const int TagColour = 0x0017;

        /// <summary>See Print Image Matching for specification.</summary>
        /// <remarks>
        /// See Print Image Matching for specification.
        /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html
        /// </remarks>
        public const int TagPrintImageMatchingInfo = 0x0E00;

        /// <summary>(String).</summary>
        public const int TagTimeZone = 0x1000;

        /// <summary>(String).</summary>
        public const int TagDaylightSavings = 0x1001;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagCaptureMode, "Capture Mode" },
            { TagQualityLevel, "Quality Level" },
            { TagFocusMode, "Focus Mode" },
            { TagFlashMode, "Flash Mode" },
            { TagWhiteBalance, "White Balance" },
            { TagDigitalZoom, "Digital Zoom" },
            { TagSharpness, "Sharpness" },
            { TagContrast, "Contrast" },
            { TagSaturation, "Saturation" },
            { TagIsoSpeed, "ISO Speed" },
            { TagColour, "Colour" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagTimeZone, "Time Zone" },
            { TagDaylightSavings, "Daylight Savings" }
        };

        public PentaxMakernoteDirectory()
        {
            SetDescriptor(new PentaxMakernoteDescriptor(this));
        }

        public override string Name => "Pentax Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
