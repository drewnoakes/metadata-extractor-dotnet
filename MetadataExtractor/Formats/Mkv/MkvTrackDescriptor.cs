// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.MkvTrackDirectory;

namespace MetadataExtractor.Formats.Mkv;

public class MkvTrackDescriptor<T>(T directory) : TagDescriptor<T>(directory) where T : MkvTrackDirectory
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagFlagLacing => GetFlagLacingDescription(),
        TagDefaultDuration => GetDefaultDurationDescription(),
        TagCodecDelay => GetCodecDelayDescription(),
        TagSeekPreRoll => GetSeekPreRollDescription(),
        TagTrackType => GetTrackTypeDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetFlagLacingDescription() => GetIndexedDescription(TagFlagLacing, "No", "Yes");
    public string? GetDefaultDurationDescription() => GetFormattedString(TagDefaultDuration, "{0} ns");
    public string? GetCodecDelayDescription() => GetFormattedString(TagCodecDelay, "{0} ns");
    public string? GetSeekPreRollDescription() => GetFormattedString(TagSeekPreRoll, "{0} ns");

    public string? GetTrackTypeDescription()
    {
        return Directory.TryGetUInt64(TagTrackType, out var type) ? type switch
        {
            1 => "Video",
            2 => "Audio",
            3 => "Complex",
            16 => "Logo",
            17 => "Subtitle",
            18 => "Buttons",
            32 => "Control",
            33 => "Metadata",
            _ => $"Unknown ({type})"
        } : null;
    }
}
