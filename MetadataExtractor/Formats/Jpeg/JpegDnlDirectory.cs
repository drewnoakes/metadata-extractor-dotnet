// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Describes tags parsed from JPEG DNL data, holding the image height with information missing from the JPEG SOFx segment</summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class JpegDnlDirectory : Directory
    {
        /// <summary>The image's height, gleaned from DNL data instead of an SOFx segment</summary>
        public const int TagImageHeight = 1;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagImageHeight, "Image Height" }
        };

        public JpegDnlDirectory()
        {
            SetDescriptor(new JpegDnlDescriptor(this));
        }

        public override string Name => "JPEG DNL";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
