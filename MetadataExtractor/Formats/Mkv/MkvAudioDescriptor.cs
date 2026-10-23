// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.MkvAudioDirectory;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvAudioDescriptor(MkvAudioDirectory directory) : MkvTrackDescriptor<MkvAudioDirectory>(directory)
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagSamplingFrequency => GetSamplingFrequencyDescription(),
        TagBitDepth => GetBitDepthDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetSamplingFrequencyDescription() => GetFormattedString(TagSamplingFrequency, "{0} Hz");
    public string? GetBitDepthDescription() => GetFormattedString(TagBitDepth, "{0} bits");
}
