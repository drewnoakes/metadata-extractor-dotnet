// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Kyocera and Contax cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class KyoceraMakernoteDirectory : Directory
    {
        public const int TagProprietaryThumbnail = 0x0001;
        public const int TagPrintImageMatchingInfo = 0x0E00;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagProprietaryThumbnail, "Proprietary Thumbnail Format Data" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" }
        };

        public KyoceraMakernoteDirectory()
        {
            SetDescriptor(new KyoceraMakernoteDescriptor(this));
        }

        public override string Name => "Kyocera/Contax Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
