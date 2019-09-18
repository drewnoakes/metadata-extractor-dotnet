// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Pcx
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PcxDirectory : Directory
    {
        public const int TagVersion = 1;
        public const int TagBitsPerPixel = 2;
        public const int TagXMin = 3;
        public const int TagYMin = 4;
        public const int TagXMax = 5;
        public const int TagYMax = 6;
        public const int TagHorizontalDpi = 7;
        public const int TagVerticalDpi = 8;
        public const int TagPalette = 9;
        public const int TagColorPlanes = 10;
        public const int TagBytesPerLine = 11;
        public const int TagPaletteType = 12;
        public const int TagHScrSize = 13;
        public const int TagVScrSize = 14;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagVersion, "Version" },
            { TagBitsPerPixel, "Bits Per Pixel" },
            { TagXMin, "X Min" },
            { TagYMin, "Y Min" },
            { TagXMax, "X Max" },
            { TagYMax, "Y Max" },
            { TagHorizontalDpi, "Horizontal DPI" },
            { TagVerticalDpi, "Vertical DPI" },
            { TagPalette, "Palette" },
            { TagColorPlanes, "Color Planes" },
            { TagBytesPerLine, "Bytes Per Line" },
            { TagPaletteType, "Palette Type" },
            { TagHScrSize, "H Scr Size" },
            { TagVScrSize, "V Scr Size" }
        };

        public PcxDirectory()
        {
            SetDescriptor(new PcxDescriptor(this));
        }

        public override string Name => "PCX";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
