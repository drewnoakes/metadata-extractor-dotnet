// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jfxx
{
    /// <summary>Directory of tags and values for the SOF0 Jfif segment.</summary>
    /// <author>Drew Noakes</author>
    public sealed class JfxxDirectory : Directory
    {
        public const int TagExtensionCode = 5;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagExtensionCode, "Extension Code" }
        };

        public JfxxDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new JfxxDescriptor(this));
        }

        public override string Name => "JFXX";

        public int GetExtensionCode()
        {
            return this.GetInt32(TagExtensionCode);
        }
    }
}
