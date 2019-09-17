// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Ricoh cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class RicohMakernoteDirectory : Directory
    {
        public const int TagMakernoteDataType = 0x0001;
        public const int TagVersion = 0x0002;
        public const int TagPrintImageMatchingInfo = 0x0E00;
        public const int TagRicohCameraInfoMakernoteSubIfdPointer = 0x2001;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakernoteDataType, "Makernote Data Type" },
            { TagVersion, "Version" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagRicohCameraInfoMakernoteSubIfdPointer, "Ricoh Camera Info Makernote Sub-IFD" }
        };

        public RicohMakernoteDirectory()
        {
            SetDescriptor(new RicohMakernoteDescriptor(this));
        }

        public override string Name => "Ricoh Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
