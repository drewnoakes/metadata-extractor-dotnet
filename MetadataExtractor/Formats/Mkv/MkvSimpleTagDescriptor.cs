// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.MkvSimpleTagDirectory;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvSimpleTagDescriptor(MkvSimpleTagDirectory directory) : TagDescriptor<MkvSimpleTagDirectory>(directory)
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagDefault => GetDefaultDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetDefaultDescription() => GetIndexedDescription(TagDefault, "No", "Yes");
}
