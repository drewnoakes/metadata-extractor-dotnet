// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="KyoceraMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
    /// <para />
    /// Most manufacturer's Makernote counts the "offset to data" from the first byte
    /// of TIFF header (same as the other IFD), but Kyocera (along with Fujifilm) counts
    /// it from the first byte of Makernote itself.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class KyoceraMakernoteDescriptor : TagDescriptor<KyoceraMakernoteDirectory>
    {
        public KyoceraMakernoteDescriptor(KyoceraMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                KyoceraMakernoteDirectory.TagProprietaryThumbnail => GetProprietaryThumbnailDataDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetProprietaryThumbnailDataDescription()
        {
            return GetByteLengthDescription(KyoceraMakernoteDirectory.TagProprietaryThumbnail);
        }
    }
}
