// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Heif;

public class HeicThumbnailTagDescriptor : TagDescriptor<HeicThumbnailDirectory>
{
    public HeicThumbnailTagDescriptor(HeicThumbnailDirectory directory)
        : base(directory)
    {
    }
}
