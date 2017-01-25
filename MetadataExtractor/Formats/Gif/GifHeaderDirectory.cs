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

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GifHeaderDirectory : Directory
    {
        public const int TagGifFormatVersion = 1;
        public const int TagImageWidth = 2;
        public const int TagImageHeight = 3;
        public const int TagColorTableSize = 4;
        public const int TagIsColorTableSorted = 5;
        public const int TagBitsPerPixel = 6;
        public const int TagHasGlobalColorTable = 7;
        public const int TagBackgroundColorIndex = 8;
        public const int TagPixelAspectRatio = 9;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagGifFormatVersion, "GIF Format Version" },
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagColorTableSize, "Color Table Size" },
            { TagIsColorTableSorted, "Is Color Table Sorted" },
            { TagBitsPerPixel, "Bits per Pixel" },
            { TagHasGlobalColorTable, "Has Global Color Table" },
            { TagBackgroundColorIndex, "Background Color Index" },
            { TagPixelAspectRatio, "Pixel Aspect Ratio" }
        };

        public GifHeaderDirectory()
        {
            SetDescriptor(new GifHeaderDescriptor(this));
        }

        public override string Name => "GIF Header";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
