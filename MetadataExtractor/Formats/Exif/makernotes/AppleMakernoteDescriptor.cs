// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="AppleMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Using information from http://owl.phy.queensu.ca/~phil/exiftool/TagNames/Apple.html</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AppleMakernoteDescriptor : TagDescriptor<AppleMakernoteDirectory>
    {
        public AppleMakernoteDescriptor(AppleMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case AppleMakernoteDirectory.TagHdrImageType:
                {
                    return GetHdrImageTypeDescription();
                }
                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        private string? GetHdrImageTypeDescription()
        {
            return GetIndexedDescription(AppleMakernoteDirectory.TagHdrImageType, 3, "HDR Image", "Original Image");
        }
    }
}
