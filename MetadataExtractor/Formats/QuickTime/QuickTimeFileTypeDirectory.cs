// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.QuickTime
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class QuickTimeFileTypeDirectory : Directory
    {
        public const int TagMajorBrand = 1;
        public const int TagMinorVersion = 2;
        public const int TagCompatibleBrands = 3;

        public override string Name { get; } = "QuickTime File Type";

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMajorBrand,       "Major Brand" },
            { TagMinorVersion,     "Minor Version" },
            { TagCompatibleBrands, "Compatible Brands" }
        };

        public QuickTimeFileTypeDirectory()
        {
            SetDescriptor(new TagDescriptor<QuickTimeFileTypeDirectory>(this));
        }

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
