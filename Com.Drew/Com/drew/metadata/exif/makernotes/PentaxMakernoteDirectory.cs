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
    /// <summary>Describes tags specific to Pentax and Asahi cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PentaxMakernoteDirectory : Directory
    {
        /// <summary>
        /// 0 = Auto
        /// 1 = Night-scene
        /// 2 = Manual
        /// 4 = Multiple
        /// </summary>
        public const int TagCaptureMode = unchecked(0x0001);

        /// <summary>
        /// 0 = Good
        /// 1 = Better
        /// 2 = Best
        /// </summary>
        public const int TagQualityLevel = unchecked(0x0002);

        /// <summary>
        /// 2 = Custom
        /// 3 = Auto
        /// </summary>
        public const int TagFocusMode = unchecked(0x0003);

        /// <summary>
        /// 1 = Auto
        /// 2 = Flash on
        /// 4 = Flash off
        /// 6 = Red-eye Reduction
        /// </summary>
        public const int TagFlashMode = unchecked(0x0004);

        /// <summary>
        /// 0 = Auto
        /// 1 = Daylight
        /// 2 = Shade
        /// 3 = Tungsten
        /// 4 = Fluorescent
        /// 5 = Manual
        /// </summary>
        public const int TagWhiteBalance = unchecked(0x0007);

        /// <summary>(0 = Off)</summary>
        public const int TagDigitalZoom = unchecked(0x000A);

        /// <summary>
        /// 0 = Normal
        /// 1 = Soft
        /// 2 = Hard
        /// </summary>
        public const int TagSharpness = unchecked(0x000B);

        /// <summary>
        /// 0 = Normal
        /// 1 = Low
        /// 2 = High
        /// </summary>
        public const int TagContrast = unchecked(0x000C);

        /// <summary>
        /// 0 = Normal
        /// 1 = Low
        /// 2 = High
        /// </summary>
        public const int TagSaturation = unchecked(0x000D);

        /// <summary>
        /// 10 = ISO 100
        /// 16 = ISO 200
        /// 100 = ISO 100
        /// 200 = ISO 200
        /// </summary>
        public const int TagIsoSpeed = unchecked(0x0014);

        /// <summary>
        /// 1 = Normal
        /// 2 = Black &amp; White
        /// 3 = Sepia
        /// </summary>
        public const int TagColour = unchecked(0x0017);

        /// <summary>See Print Image Matching for specification.</summary>
        /// <remarks>
        /// See Print Image Matching for specification.
        /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html
        /// </remarks>
        public const int TagPrintImageMatchingInfo = unchecked(0x0E00);

        /// <summary>(String).</summary>
        public const int TagTimeZone = unchecked(0x1000);

        /// <summary>(String).</summary>
        public const int TagDaylightSavings = unchecked(0x1001);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PentaxMakernoteDirectory()
        {
            TagNameMap.Put(TagCaptureMode, "Capture Mode");
            TagNameMap.Put(TagQualityLevel, "Quality Level");
            TagNameMap.Put(TagFocusMode, "Focus Mode");
            TagNameMap.Put(TagFlashMode, "Flash Mode");
            TagNameMap.Put(TagWhiteBalance, "White Balance");
            TagNameMap.Put(TagDigitalZoom, "Digital Zoom");
            TagNameMap.Put(TagSharpness, "Sharpness");
            TagNameMap.Put(TagContrast, "Contrast");
            TagNameMap.Put(TagSaturation, "Saturation");
            TagNameMap.Put(TagIsoSpeed, "ISO Speed");
            TagNameMap.Put(TagColour, "Colour");
            TagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info");
            TagNameMap.Put(TagTimeZone, "Time Zone");
            TagNameMap.Put(TagDaylightSavings, "Daylight Savings");
        }

        public PentaxMakernoteDirectory()
        {
            SetDescriptor(new PentaxMakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Pentax Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
