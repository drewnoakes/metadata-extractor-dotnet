// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Ico
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagImageType, "Image Type" },
            { TagImageWidth, "Image Width" },
            { TagImageHeight, "Image Height" },
            { TagColourPaletteSize, "Colour Palette Size" },
            { TagColourPlanes, "Colour Planes" },
            { TagCursorHotspotX, "Hotspot X" },
            { TagBitsPerPixel, "Bits Per Pixel" },
            { TagCursorHotspotY, "Hotspot Y" },
            { TagImageSizeBytes, "Image Size Bytes" },
            { TagImageOffsetBytes, "Image Offset Bytes" }
        };

        public IcoDirectory()
        {
            SetDescriptor(new IcoDescriptor(this));
        }

        public override string Name => "ICO";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
