// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jfxx
{
    /// <summary>Directory of tags and values for the SOF0 Jfif segment.</summary>
    /// <author>Drew Noakes</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JfxxDirectory : Directory
    {
        public const int TagExtensionCode = 5;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagExtensionCode, "Extension Code" }
        };

        public JfxxDirectory()
        {
            SetDescriptor(new JfxxDescriptor(this));
        }

        public override string Name => "JFXX";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public int GetExtensionCode()
        {
            return this.GetInt32(TagExtensionCode);
        }
    }
}
