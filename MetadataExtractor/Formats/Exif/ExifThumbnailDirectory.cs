// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>One of several Exif directories.</summary>
    /// <remarks>Otherwise known as IFD1, this directory holds information about an embedded thumbnail image.</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ExifThumbnailDirectory : ExifDirectoryBase
    {
        /// <summary>The offset to thumbnail image bytes.</summary>
        public const int TagThumbnailOffset = 0x0201;

        /// <summary>The size of the thumbnail image data in bytes.</summary>
        public const int TagThumbnailLength = 0x0202;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagThumbnailOffset, "Thumbnail Offset" },
            { TagThumbnailLength, "Thumbnail Length" }
        };

        static ExifThumbnailDirectory()
        {
            AddExifTagNames(_tagNameMap);
        }

        public ExifThumbnailDirectory()
        {
            SetDescriptor(new ExifThumbnailDescriptor(this));
        }

        public override string Name => "Exif Thumbnail";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
