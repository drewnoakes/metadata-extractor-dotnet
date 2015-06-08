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

namespace Com.Drew.Metadata.Pcx
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PcxDirectory : Directory
    {
        public const int TagVersion = 1;

        public const int TagBitsPerPixel = 2;

        public const int TagXmin = 3;

        public const int TagYmin = 4;

        public const int TagXmax = 5;

        public const int TagYmax = 6;

        public const int TagHorizontalDpi = 7;

        public const int TagVerticalDpi = 8;

        public const int TagPalette = 9;

        public const int TagColorPlanes = 10;

        public const int TagBytesPerLine = 11;

        public const int TagPaletteType = 12;

        public const int TagHscrSize = 13;

        public const int TagVscrSize = 14;

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PcxDirectory()
        {
            TagNameMap.Put(TagVersion, "Version");
            TagNameMap.Put(TagBitsPerPixel, "Bits Per Pixel");
            TagNameMap.Put(TagXmin, "X Min");
            TagNameMap.Put(TagYmin, "Y Min");
            TagNameMap.Put(TagXmax, "X Max");
            TagNameMap.Put(TagYmax, "Y Max");
            TagNameMap.Put(TagHorizontalDpi, "Horitzontal DPI");
            TagNameMap.Put(TagVerticalDpi, "Vertical DPI");
            TagNameMap.Put(TagPalette, "Palette");
            TagNameMap.Put(TagColorPlanes, "Color Planes");
            TagNameMap.Put(TagBytesPerLine, "Bytes Per Line");
            TagNameMap.Put(TagPaletteType, "Palette Type");
            TagNameMap.Put(TagHscrSize, "H Hcr Size");
            TagNameMap.Put(TagVscrSize, "V Scr Size");
        }

        public PcxDirectory()
        {
            this.SetDescriptor(new PcxDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "PCX";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
