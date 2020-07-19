// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime
{
    public sealed class QuickTimeMetadataHeaderDescriptor : TagDescriptor<QuickTimeMetadataHeaderDirectory>
    {
        public QuickTimeMetadataHeaderDescriptor(QuickTimeMetadataHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                QuickTimeMetadataHeaderDirectory.TagArtwork => GetArtworkDescription(),
                QuickTimeMetadataHeaderDirectory.TagLocationRole => GetLocationRoleDescription(),
                _ => base.GetDescription(tagType)
            };
        }

        public string? GetArtworkDescription()
        {
            return GetByteLengthDescription(QuickTimeMetadataHeaderDirectory.TagArtwork);
        }

        public string? GetLocationRoleDescription()
        {
            return GetIndexedDescription(QuickTimeMetadataHeaderDirectory.TagLocationRole,
                "Shooting location",
                "Real location",
                "Fictional location");
        }
    }
}
