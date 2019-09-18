// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace MetadataExtractor.Formats.Netpbm
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NetpbmHeaderDirectory : Directory
    {
        public const int TagFormatType = 1;
        public const int TagWidth = 2;
        public const int TagHeight = 3;
        public const int TagMaximumValue = 4;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagFormatType, "Format Type" },
            { TagWidth, "Width" },
            { TagHeight, "Height" },
            { TagMaximumValue, "Maximum Value" }
        };

        public NetpbmHeaderDirectory()
        {
            SetDescriptor(new NetpbmHeaderDescriptor(this));
        }

        public override string Name => "Netpbm";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
