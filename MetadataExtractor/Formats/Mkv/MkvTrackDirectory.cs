// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

/// <summary>Metadata common to Matroska tracks. Unsigned integers, including 64-bit UIDs, are stored as <see cref="ulong"/>.</summary>
public class MkvTrackDirectory : Directory
{
    public const int TagTrackNumber = ElementIds.TrackNumber;
    public const int TagTrackUid = ElementIds.TrackUid;
    public const int TagTrackType = ElementIds.TrackType;
    public const int TagFlagLacing = ElementIds.FlagLacing;
    public const int TagCodecId = ElementIds.CodecId;
    public const int TagLanguage = ElementIds.Language;
    public const int TagLanguageBcp47 = ElementIds.LanguageBcp47;
    public const int TagDefaultDuration = ElementIds.DefaultDuration;
    public const int TagCodecDelay = ElementIds.CodecDelay;
    public const int TagSeekPreRoll = ElementIds.SeekPreRoll;

    private static readonly Dictionary<int, string> _tagNameMap = CreateTagNameMap();

    public MkvTrackDirectory() : this(_tagNameMap) { }

    protected MkvTrackDirectory(Dictionary<int, string> tagNameMap) : base(tagNameMap)
        => SetDescriptor(new MkvTrackDescriptor<MkvTrackDirectory>(this));

    protected static Dictionary<int, string> CreateTagNameMap() => new()
    {
        { TagTrackNumber, "Track Number" },
        { TagTrackUid, "Track UID" },
        { TagTrackType, "Track Type" },
        { TagFlagLacing, "Flag Lacing" },
        { TagCodecId, "Codec ID" },
        { TagLanguage, "Language" },
        { TagLanguageBcp47, "Language BCP47" },
        { TagDefaultDuration, "Default Duration" },
        { TagCodecDelay, "Codec Delay" },
        { TagSeekPreRoll, "Seek Pre-roll" }
    };

    public override string Name => "Matroska Track";
}
