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
    /// <summary>Describes tags specific to Nikon (type 1) cameras.</summary>
    /// <remarks>
    /// Describes tags specific to Nikon (type 1) cameras.  Type-1 is for E-Series cameras prior to (not including) E990.
    /// There are 3 formats of Nikon's Makernote. Makernote of E700/E800/E900/E900S/E910/E950
    /// starts from ASCII string "Nikon". Data format is the same as IFD, but it starts from
    /// offset 0x08. This is the same as Olympus except start string. Example of actual data
    /// structure is shown below.
    /// <pre><c>
    /// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
    /// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
    /// </c></pre>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class NikonType1MakernoteDirectory : Directory
    {
        public const int TagUnknown1 = unchecked(0x0002);

        public const int TagQuality = unchecked(0x0003);

        public const int TagColorMode = unchecked(0x0004);

        public const int TagImageAdjustment = unchecked(0x0005);

        public const int TagCcdSensitivity = unchecked(0x0006);

        public const int TagWhiteBalance = unchecked(0x0007);

        public const int TagFocus = unchecked(0x0008);

        public const int TagUnknown2 = unchecked(0x0009);

        public const int TagDigitalZoom = unchecked(0x000A);

        public const int TagConverter = unchecked(0x000B);

        public const int TagUnknown3 = unchecked(0x0F00);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static NikonType1MakernoteDirectory()
        {
            TagNameMap[TagCcdSensitivity] = "CCD Sensitivity";
            TagNameMap[TagColorMode] = "Color Mode";
            TagNameMap[TagDigitalZoom] = "Digital Zoom";
            TagNameMap[TagConverter] = "Fisheye Converter";
            TagNameMap[TagFocus] = "Focus";
            TagNameMap[TagImageAdjustment] = "Image Adjustment";
            TagNameMap[TagQuality] = "Quality";
            TagNameMap[TagUnknown1] = "Makernote Unknown 1";
            TagNameMap[TagUnknown2] = "Makernote Unknown 2";
            TagNameMap[TagUnknown3] = "Makernote Unknown 3";
            TagNameMap[TagWhiteBalance] = "White Balance";
        }

        public NikonType1MakernoteDirectory()
        {
            SetDescriptor(new NikonType1MakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Nikon Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
