﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Wav;

/// <author>Dmitry Shechtman</author>
public sealed class WavFormatDirectory : Directory
{
    public const int TagFormat = 1;
    public const int TagChannels = 2;
    public const int TagSamplesPerSec = 3;
    public const int TagBytesPerSec = 4;
    public const int TagBlockAlign = 5;
    public const int TagBitsPerSample = 6;
    public const int TagValidBitsPerSample = 7;
    public const int TagChannelMask = 8;
    public const int TagSubformat = 9;

    private readonly string[] _tagNames =
    [
        "Format",
        "Channels",
        "Sampling Rate",
        "Data Rate",
        "Data Block Size",
        "Bits Per Sample",
        "Valid Bits Per Sample",
        "Speaker Position Mask",
        "Subformat"
    ];

    public WavFormatDirectory() : base(null)
    {
        SetDescriptor(new WavFormatDescriptor(this));
    }

    public override string Name => "WAVE Format";

    protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
    {
        tagName = tagType > 0 && tagType <= _tagNames.Length
            ? _tagNames[tagType - 1]
            : null;
        return tagName is not null;
    }
}
