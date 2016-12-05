#region License
//
// Copyright 2002-2016 Drew Noakes
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
        // header
        public const int TagGifFormatVersion = 1;
        public const int TagImageWidth = 2;
        public const int TagImageHeight = 3;
        public const int TagFrameCount = 4;     // number of animated images
        public const int TagText = 5;           // deprecated; bytes are skipped in the reader
        public const int TagComment = 6;
        public const int TagDuration = 7;

        // Logical Screen Descriptor
        // These 'sub'-tag values have been created for clarity
        public static class Screen
        {
            internal const int Offset = 0x0100;

            public const int TagHasGlobalColorTable = Offset + 0x01;
            public const int TagColorResolutionDepth = Offset + 0x02;
            public const int TagBitsPerPixel = Offset + 0x03;
            public const int TagIsColorTableSorted = Offset + 0x04;
            public const int TagColorTableSize = Offset + 0x05;
            public const int TagColorTableLength = Offset + 0x06;

            public const int TagBackgroundColorIndex = Offset + 0x07;
            public const int TagPixelAspectRatio = Offset + 0x08;
        }

        // Netscape 2.0 animation extension
        // These 'sub'-tag values have been created for clarity
        public static class Animate
        {
            internal const int Offset = 0x0200;
            public const int TagAnimationIterations = Offset + 0x01;
        }

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagGifFormatVersion, "GIF Format Version" },
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagFrameCount, "Frame Count" },
            { TagComment, "Comment" },
            { TagDuration, "Duration" },
            { Screen.TagHasGlobalColorTable, "Has Global Color Table" },
            { Screen.TagColorResolutionDepth, "Color Resolution Depth" },
            { Screen.TagBitsPerPixel, "Bits per Pixel" },
            { Screen.TagIsColorTableSorted, "Is Color Table Sorted" },
            { Screen.TagColorTableSize, "Color Table Size" },
            { Screen.TagColorTableLength, "Color Table Byte Length" },
            { Screen.TagBackgroundColorIndex, "Background Color Index" },
            { Screen.TagPixelAspectRatio, "Pixel Aspect Ratio" },
            { Animate.TagAnimationIterations, "Animation Iterations" }
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
