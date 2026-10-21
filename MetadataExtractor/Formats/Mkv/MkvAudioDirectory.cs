// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvAudioDirectory : MkvTrackDirectory
{
    public const int TagChannels = ElementIds.Channels;
    public const int TagSamplingFrequency = ElementIds.SamplingFrequency;
    public const int TagBitDepth = ElementIds.BitDepth;

    private static readonly Dictionary<int, string> _tagNameMap = new(CreateTagNameMap())
    {
        { TagChannels, "Channels" },
        { TagSamplingFrequency, "Sampling Frequency" },
        { TagBitDepth, "Bit Depth" }
    };

    public MkvAudioDirectory() : base(_tagNameMap) => SetDescriptor(new MkvAudioDescriptor(this));

    public override string Name => "Matroska Audio";
}
