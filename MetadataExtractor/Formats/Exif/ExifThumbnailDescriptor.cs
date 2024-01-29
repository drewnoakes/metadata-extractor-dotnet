// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ExifThumbnailDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifThumbnailDescriptor(ExifThumbnailDirectory directory)
        : ExifDescriptorBase<ExifThumbnailDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                ExifThumbnailDirectory.TagThumbnailOffset => GetThumbnailOffsetDescription(),
                ExifThumbnailDirectory.TagThumbnailLength => GetThumbnailLengthDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetThumbnailLengthDescription()
        {
            var value = Directory.GetString(ExifThumbnailDirectory.TagThumbnailLength);
            return value is null ? null : value + " bytes";
        }

        public string? GetThumbnailOffsetDescription()
        {
            var offset = Directory.AdjustedThumbnailOffset;

            return offset is null ? null : offset + " bytes";
        }
    }
}
