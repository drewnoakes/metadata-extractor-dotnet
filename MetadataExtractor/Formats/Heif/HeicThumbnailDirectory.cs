// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicThumbnailDirectory : Directory
    {
        public override string Name => "HEIC Thumbnail Data";

        public HeicThumbnailDirectory()
        {
            SetDescriptor(new HeicThumbnailTagDescriptor(this));
        }

        public const int TagFileOffset = 1;
        public const int TagLength = 2;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            { TagFileOffset, "Offset From Beginning of File" },
            { TagLength, "Data Length" }
        };

        protected override bool TryGetTagName(int tagType, [NotNullWhen(returnValue: true)] out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
