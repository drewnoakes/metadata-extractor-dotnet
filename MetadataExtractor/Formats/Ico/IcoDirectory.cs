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

namespace MetadataExtractor.Formats.Ico
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IcoDirectory : Directory
    {
        public const int TagImageType = 1;
        public const int TagImageWidth = 2;
        public const int TagImageHeight = 3;
        public const int TagColourPaletteSize = 4;
        public const int TagColourPlanes = 5;
        public const int TagCursorHotspotX = 6;
        public const int TagBitsPerPixel = 7;
        public const int TagCursorHotspotY = 8;
        public const int TagImageSizeBytes = 9;
        public const int TagImageOffsetBytes = 10;

        [NotNull] private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static IcoDirectory()
        {
            TagNameMap[TagImageType] = "Image Type";
            TagNameMap[TagImageWidth] = "Image Width";
            TagNameMap[TagImageHeight] = "Image Height";
            TagNameMap[TagColourPaletteSize] = "Colour Palette Size";
            TagNameMap[TagColourPlanes] = "Colour Planes";
            TagNameMap[TagCursorHotspotX] = "Hotspot X";
            TagNameMap[TagBitsPerPixel] = "Bits Per Pixel";
            TagNameMap[TagCursorHotspotY] = "Hotspot Y";
            TagNameMap[TagImageSizeBytes] = "Image Size Bytes";
            TagNameMap[TagImageOffsetBytes] = "Image Offset Bytes";
        }

        public IcoDirectory()
        {
            SetDescriptor(new IcoDescriptor(this));
        }

        public override string GetName()
        {
            return "ICO";
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
