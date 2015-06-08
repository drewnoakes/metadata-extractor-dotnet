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
using Com.Drew.Imaging.Png;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PngDirectory : Directory
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

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PngDirectory()
        {
            TagNameMap.Put(TagImageHeight, "Image Height");
            TagNameMap.Put(TagImageWidth, "Image Width");
            TagNameMap.Put(TagBitsPerSample, "Bits Per Sample");
            TagNameMap.Put(TagColorType, "Color Type");
            TagNameMap.Put(TagCompressionType, "Compression Type");
            TagNameMap.Put(TagFilterMethod, "Filter Method");
            TagNameMap.Put(TagInterlaceMethod, "Interlace Method");
            TagNameMap.Put(TagPaletteSize, "Palette Size");
            TagNameMap.Put(TagPaletteHasTransparency, "Palette Has Transparency");
            TagNameMap.Put(TagSrgbRenderingIntent, "sRGB Rendering Intent");
            TagNameMap.Put(TagGamma, "Image Gamma");
            TagNameMap.Put(TagIccProfileName, "ICC Profile Name");
            TagNameMap.Put(TagTextualData, "Textual Data");
            TagNameMap.Put(TagLastModificationTime, "Last Modification Time");
            TagNameMap.Put(TagBackgroundColor, "Background Color");
            TagNameMap.Put(TagPixelsPerUnitX, "Pixels Per Unit X");
            TagNameMap.Put(TagPixelsPerUnitY, "Pixels Per Unit Y");
            TagNameMap.Put(TagUnitSpecifier, "Unit Specifier");
            TagNameMap.Put(TagSignificantBits, "Significant Bits");
        }

        private readonly PngChunkType _pngChunkType;

        public PngDirectory([NotNull] PngChunkType pngChunkType)
        {
            _pngChunkType = pngChunkType;
            SetDescriptor(new PngDescriptor(this));
        }

        [NotNull]
        public virtual PngChunkType GetPngChunkType()
        {
            return _pngChunkType;
        }

        [NotNull]
        public override string GetName()
        {
            return "PNG-" + _pngChunkType.GetIdentifier();
        }

        [NotNull]
        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
