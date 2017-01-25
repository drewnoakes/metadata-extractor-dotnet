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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class NikonType1MakernoteDirectory : Directory
    {
        public const int TagUnknown1 = 0x0002;
        public const int TagQuality = 0x0003;
        public const int TagColorMode = 0x0004;
        public const int TagImageAdjustment = 0x0005;
        public const int TagCcdSensitivity = 0x0006;
        public const int TagWhiteBalance = 0x0007;
        public const int TagFocus = 0x0008;
        public const int TagUnknown2 = 0x0009;
        public const int TagDigitalZoom = 0x000A;
        public const int TagConverter = 0x000B;
        public const int TagUnknown3 = 0x0F00;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagCcdSensitivity, "CCD Sensitivity" },
            { TagColorMode, "Color Mode" },
            { TagDigitalZoom, "Digital Zoom" },
            { TagConverter, "Fisheye Converter" },
            { TagFocus, "Focus" },
            { TagImageAdjustment, "Image Adjustment" },
            { TagQuality, "Quality" },
            { TagUnknown1, "Makernote Unknown 1" },
            { TagUnknown2, "Makernote Unknown 2" },
            { TagUnknown3, "Makernote Unknown 3" },
            { TagWhiteBalance, "White Balance" }
        };

        public NikonType1MakernoteDirectory()
        {
            SetDescriptor(new NikonType1MakernoteDescriptor(this));
        }

        public override string Name => "Nikon Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
