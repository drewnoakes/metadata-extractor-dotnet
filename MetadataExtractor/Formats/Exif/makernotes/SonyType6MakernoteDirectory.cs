// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sony cameras that use the Sony Type 6 makernote tags.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SonyType6MakernoteDirectory : Directory
    {
        public const int TagMakernoteThumbOffset = 0x0513;
        public const int TagMakernoteThumbLength = 0x0514;
//      public const int TagUnknown1 = 0x0515;
        public const int TagMakernoteThumbVersion = 0x2000;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakernoteThumbOffset, "Makernote Thumb Offset" },
            { TagMakernoteThumbLength, "Makernote Thumb Length" },
            { TagMakernoteThumbVersion, "Makernote Thumb Version" }
        };

        public SonyType6MakernoteDirectory()
        {
            SetDescriptor(new SonyType6MakernoteDescriptor(this));
        }

        public override string Name => "Sony Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
