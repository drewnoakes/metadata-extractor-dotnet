// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Holds the data found in Photoshop "ducky" segments, created during Save-for-Web.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DuckyDirectory : Directory
    {
        public const int TagQuality = 1;
        public const int TagComment = 2;
        public const int TagCopyright = 3;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagQuality, "Quality" },
            { TagComment, "Comment" },
            { TagCopyright, "Copyright" }
        };

        public DuckyDirectory()
        {
            SetDescriptor(new TagDescriptor<DuckyDirectory>(this));
        }

        public override string Name => "Ducky";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
