// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Describes Exif interoperability tags.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ExifInteropDirectory : ExifDirectoryBase
    {
        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

        static ExifInteropDirectory()
        {
            AddExifTagNames(_tagNameMap);
        }

        public ExifInteropDirectory()
        {
            SetDescriptor(new ExifInteropDescriptor(this));
        }

        public override string Name => "Interoperability";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
