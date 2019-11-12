// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Tga
{
    /// <author>Dmitry Shechtman</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class TgaHeaderDirectory : Directory
    {
        public const int TagIdLength = 1;
        public const int TagDataType = 2;
        public const int TagColormapType = 3;
        public const int TagColormapOrigin = 4;
        public const int TagColormapLength = 5;
        public const int TagColormapDepth = 6;
        public const int TagXOrigin = 7;
        public const int TagYOrigin = 8;
        public const int TagImageWidth = 9;
        public const int TagImageHeight = 10;
        public const int TagImageDepth = 11;
        public const int TagAttributeBitsPerPixel = 12;
        public const int TagHorizontalOrder = 13;
        public const int TagVerticalOrder = 14;
        public const int TagId = 15;
        public const int TagColormap = 16;

        private static readonly string[] _tagNames =
        {
            "Image ID Length",
            "Data Type",
            "Color Map Type",
            "Color Map Origin",
            "Color Map Length",
            "Color Map Entry Size",
            "X-Origin",
            "Y-Origin",
            "Image Width",
            "Image Height",
            "Image Depth",
            "Attribute Bits per Pixel",
            "Horizontal Order",
            "Vertical Order",
            "Image ID",
            "Color Map"
        };

        public TgaHeaderDirectory()
        {
            SetDescriptor(new TgaHeaderDescriptor(this));
        }

        public override string Name => "TGA Header";

        protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
        {
            tagName = tagType > 0 && tagType <= _tagNames.Length
                ? _tagNames[tagType - 1]
                : null;
            return tagName != null;
        }
    }
}
