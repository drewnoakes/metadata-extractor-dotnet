﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Wav;

/// <author>Dmitry Shechtman</author>
public sealed class WavFactDirectory : Directory
{
    public const int TagSampleLength = 1;

    private readonly string[] _tagNames =
    [
        "Number of Samples"
    ];

    public WavFactDirectory() : base(null)
    {
        SetDescriptor(new TagDescriptor<WavFactDirectory>(this));
    }

    public override string Name => "WAVE Fact";

    protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
    {
        tagName = tagType > 0 && tagType <= _tagNames.Length
            ? _tagNames[tagType - 1]
            : null;
        return tagName is not null;
    }
}
