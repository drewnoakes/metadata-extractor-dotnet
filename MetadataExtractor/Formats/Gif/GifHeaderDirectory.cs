#region License
//
// Copyright 2002-2015 Drew Noakes
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
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class GifHeaderDirectory : Directory
    {
        public const int TagGifFormatVersion = 1;

        public const int TagImageWidth = 2;

        public const int TagImageHeight = 3;

        public const int TagColorTableSize = 4;

        public const int TagIsColorTableSorted = 5;

        public const int TagBitsPerPixel = 6;

        public const int TagHasGlobalColorTable = 7;

        public const int TagTransparentColorIndex = 8;

        public const int TagPixelAspectRatio = 9;

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static GifHeaderDirectory()
        {
            TagNameMap[TagGifFormatVersion] = "GIF Format Version";
            TagNameMap[TagImageHeight] = "Image Height";
            TagNameMap[TagImageWidth] = "Image Width";
            TagNameMap[TagColorTableSize] = "Color Table Size";
            TagNameMap[TagIsColorTableSorted] = "Is Color Table Sorted";
            TagNameMap[TagBitsPerPixel] = "Bits per Pixel";
            TagNameMap[TagHasGlobalColorTable] = "Has Global Color Table";
            TagNameMap[TagTransparentColorIndex] = "Transparent Color Index";
            TagNameMap[TagPixelAspectRatio] = "Pixel Aspect Ratio";
        }

        public GifHeaderDirectory()
        {
            SetDescriptor(new GifHeaderDescriptor(this));
        }

        public override string Name
        {
            get { return "GIF Header"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
