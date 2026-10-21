// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvSegmentDirectory : Directory
{
    public const int TagTimestampScale = ElementIds.TimestampScale;
    public const int TagMuxingApp = ElementIds.MuxingApp;
    public const int TagWritingApp = ElementIds.WritingApp;
    public const int TagDuration = ElementIds.Duration;
    public const int TagSegmentUuid = ElementIds.SegmentUuid;
    public const int TagDurationSeconds = 1;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagTimestampScale, "Timestamp Scale" },
        { TagMuxingApp, "Muxing Application" },
        { TagWritingApp, "Writing Application" },
        { TagDuration, "Duration (Segment Ticks)" },
        { TagSegmentUuid, "Segment UUID" },
        { TagDurationSeconds, "Duration" }
    };

    public MkvSegmentDirectory() : base(_tagNameMap) => SetDescriptor(new MkvSegmentDescriptor(this));

    public override string Name => "Matroska Segment";
}
