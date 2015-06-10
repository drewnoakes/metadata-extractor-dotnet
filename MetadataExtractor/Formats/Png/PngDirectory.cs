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

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
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

        private static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static PngDirectory()
        {
            _tagNameMap[TagImageHeight] = "Image Height";
            _tagNameMap[TagImageWidth] = "Image Width";
            _tagNameMap[TagBitsPerSample] = "Bits Per Sample";
            _tagNameMap[TagColorType] = "Color Type";
            _tagNameMap[TagCompressionType] = "Compression Type";
            _tagNameMap[TagFilterMethod] = "Filter Method";
            _tagNameMap[TagInterlaceMethod] = "Interlace Method";
            _tagNameMap[TagPaletteSize] = "Palette Size";
            _tagNameMap[TagPaletteHasTransparency] = "Palette Has Transparency";
            _tagNameMap[TagSrgbRenderingIntent] = "sRGB Rendering Intent";
            _tagNameMap[TagGamma] = "Image Gamma";
            _tagNameMap[TagIccProfileName] = "ICC Profile Name";
            _tagNameMap[TagTextualData] = "Textual Data";
            _tagNameMap[TagLastModificationTime] = "Last Modification Time";
            _tagNameMap[TagBackgroundColor] = "Background Color";
            _tagNameMap[TagPixelsPerUnitX] = "Pixels Per Unit X";
            _tagNameMap[TagPixelsPerUnitY] = "Pixels Per Unit Y";
            _tagNameMap[TagUnitSpecifier] = "Unit Specifier";
            _tagNameMap[TagSignificantBits] = "Significant Bits";
        }

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

        public override string Name
        {
            get { return "PNG-" + _pngChunkType.GetIdentifier(); }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
