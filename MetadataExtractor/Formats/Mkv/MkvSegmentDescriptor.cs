// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.MkvSegmentDirectory;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvSegmentDescriptor(MkvSegmentDirectory directory) : TagDescriptor<MkvSegmentDirectory>(directory)
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagDuration => GetDurationDescription(),
        TagDurationSeconds => GetDurationSecondsDescription(),
        TagTimestampScale => GetTimestampScaleDescription(),
        TagSegmentUuid => GetSegmentUuidDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetDurationDescription() => GetFormattedString(TagDuration, "{0} segment ticks");
    public string? GetDurationSecondsDescription() => GetFormattedString(TagDurationSeconds, "{0} seconds");
    public string? GetTimestampScaleDescription() => GetFormattedString(TagTimestampScale, "{0} ns");

    public string? GetSegmentUuidDescription()
    {
        if (Directory.GetByteArray(TagSegmentUuid) is not { Length: 16 } bytes)
            return base.GetDescription(TagSegmentUuid);

        // Guid's byte-array constructor expects little-endian fields, unlike EBML's network order.
        return new Guid(
            (int)((uint)bytes[0] << 24 | (uint)bytes[1] << 16 | (uint)bytes[2] << 8 | bytes[3]),
            (short)(bytes[4] << 8 | bytes[5]), (short)(bytes[6] << 8 | bytes[7]),
            bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15]).ToString();
    }
}
