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
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GifImageDirectory : Directory
    {
        public const int TagLeft = 1;
        public const int TagTop = 2;
        public const int TagWidth = 3;
        public const int TagHeight = 4;
        public const int TagHasLocalColourTable = 5;
        public const int TagIsInterlaced = 6;
        public const int TagIsColorTableSorted = 7;
        public const int TagLocalColourTableBitsPerPixel = 8;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagLeft, "Left" },
            { TagTop, "Top" },
            { TagWidth, "Width" },
            { TagHeight, "Height" },
            { TagHasLocalColourTable, "Has Local Colour Table" },
            { TagIsInterlaced, "Is Interlaced" },
            { TagIsColorTableSorted, "Is Local Colour Table Sorted" },
            { TagLocalColourTableBitsPerPixel, "Local Colour Table Bits Per Pixel" }
        };

        public GifImageDirectory()
        {
            SetDescriptor(new GifImageDescriptor(this));
        }

        public override string Name => "GIF Image";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
