// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.WebP
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WebPDirectory : Directory
    {
        public const int TagImageHeight = 1;
        public const int TagImageWidth = 2;
        public const int TagHasAlpha = 3;
        public const int TagIsAnimation = 4;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagHasAlpha, "Has Alpha" },
            { TagIsAnimation, "Is Animation" }
        };

        public WebPDirectory()
        {
            SetDescriptor(new WebPDescriptor(this));
        }

        public override string Name => "WebP";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
