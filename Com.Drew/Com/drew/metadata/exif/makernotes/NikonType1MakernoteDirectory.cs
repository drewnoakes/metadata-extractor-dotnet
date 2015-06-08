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
    /// <pre><code>
    /// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
    /// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
    /// </code></pre>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class NikonType1MakernoteDirectory : Directory
    {
        public const int TagUnknown1 = unchecked((int)(0x0002));

        public const int TagQuality = unchecked((int)(0x0003));

        public const int TagColorMode = unchecked((int)(0x0004));

        public const int TagImageAdjustment = unchecked((int)(0x0005));

        public const int TagCcdSensitivity = unchecked((int)(0x0006));

        public const int TagWhiteBalance = unchecked((int)(0x0007));

        public const int TagFocus = unchecked((int)(0x0008));

        public const int TagUnknown2 = unchecked((int)(0x0009));

        public const int TagDigitalZoom = unchecked((int)(0x000A));

        public const int TagConverter = unchecked((int)(0x000B));

        public const int TagUnknown3 = unchecked((int)(0x0F00));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static NikonType1MakernoteDirectory()
        {
            _tagNameMap.Put(TagCcdSensitivity, "CCD Sensitivity");
            _tagNameMap.Put(TagColorMode, "Color Mode");
            _tagNameMap.Put(TagDigitalZoom, "Digital Zoom");
            _tagNameMap.Put(TagConverter, "Fisheye Converter");
            _tagNameMap.Put(TagFocus, "Focus");
            _tagNameMap.Put(TagImageAdjustment, "Image Adjustment");
            _tagNameMap.Put(TagQuality, "Quality");
            _tagNameMap.Put(TagUnknown1, "Makernote Unknown 1");
            _tagNameMap.Put(TagUnknown2, "Makernote Unknown 2");
            _tagNameMap.Put(TagUnknown3, "Makernote Unknown 3");
            _tagNameMap.Put(TagWhiteBalance, "White Balance");
        }

        public NikonType1MakernoteDirectory()
        {
            this.SetDescriptor(new NikonType1MakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Nikon Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
