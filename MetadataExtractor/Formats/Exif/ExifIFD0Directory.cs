// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Describes Exif tags from the IFD0 directory.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ExifIfd0Directory : ExifDirectoryBase
    {
        /// <summary>This tag is a pointer to the Exif SubIFD.</summary>
        public const int TagExifSubIfdOffset = 0x8769;

        /// <summary>This tag is a pointer to the Exif GPS IFD.</summary>
        public const int TagGpsInfoOffset = 0x8825;

        public ExifIfd0Directory()
        {
            SetDescriptor(new ExifIfd0Descriptor(this));
        }

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

        static ExifIfd0Directory()
        {
            AddExifTagNames(_tagNameMap);
        }

        public override string Name => "Exif IFD0";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
