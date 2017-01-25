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
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PngDirectory : Directory
    {
        public const int TagImageWidth = 1;
        public const int TagImageHeight = 2;
        public const int TagBitsPerSample = 3;
        public const int TagColorType = 4;
        public const int TagCompressionType = 5;
        public const int TagFilterMethod = 6;
        public const int TagInterlaceMethod = 7;
        public const int TagPaletteSize = 8;
        public const int TagPaletteHasTransparency = 9;
        public const int TagSrgbRenderingIntent = 10;
        public const int TagGamma = 11;
        public const int TagIccProfileName = 12;
        public const int TagTextualData = 13;
        public const int TagLastModificationTime = 14;
        public const int TagBackgroundColor = 15;
        public const int TagPixelsPerUnitX = 16;
        public const int TagPixelsPerUnitY = 17;
        public const int TagUnitSpecifier = 18;
        public const int TagSignificantBits = 19;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagBitsPerSample, "Bits Per Sample" },
            { TagColorType, "Color Type" },
            { TagCompressionType, "Compression Type" },
            { TagFilterMethod, "Filter Method" },
            { TagInterlaceMethod, "Interlace Method" },
            { TagPaletteSize, "Palette Size" },
            { TagPaletteHasTransparency, "Palette Has Transparency" },
            { TagSrgbRenderingIntent, "sRGB Rendering Intent" },
            { TagGamma, "Image Gamma" },
            { TagIccProfileName, "ICC Profile Name" },
            { TagTextualData, "Textual Data" },
            { TagLastModificationTime, "Last Modification Time" },
            { TagBackgroundColor, "Background Color" },
            { TagPixelsPerUnitX, "Pixels Per Unit X" },
            { TagPixelsPerUnitY, "Pixels Per Unit Y" },
            { TagUnitSpecifier, "Unit Specifier" },
            { TagSignificantBits, "Significant Bits" }
        };

        private readonly PngChunkType _pngChunkType;

        public PngDirectory([NotNull] PngChunkType pngChunkType)
        {
            _pngChunkType = pngChunkType;
            SetDescriptor(new PngDescriptor(this));
        }

        [NotNull]
        public PngChunkType GetPngChunkType()
        {
            return _pngChunkType;
        }

        public override string Name => "PNG-" + _pngChunkType.Identifier;

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
