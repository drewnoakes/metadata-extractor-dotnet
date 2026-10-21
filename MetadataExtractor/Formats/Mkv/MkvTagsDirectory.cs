// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

/// <summary>
/// One Tags element, parented to its segment. Later Tags elements supersede earlier ones in
/// live streams; each update is retained separately in file order.
/// </summary>
public sealed class MkvTagsDirectory : Directory
{
    public const int TagUpdateNumber = 1;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagUpdateNumber, "Tag Update Number" }
    };

    public MkvTagsDirectory() : base(_tagNameMap) => SetDescriptor(new TagDescriptor<MkvTagsDirectory>(this));

    public override string Name => "Matroska Tags";
}
