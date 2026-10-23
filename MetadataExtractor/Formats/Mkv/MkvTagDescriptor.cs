// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.MkvTagDirectory;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvTagDescriptor(MkvTagDirectory directory) : TagDescriptor<MkvTagDirectory>(directory)
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagTargetTypeValue => GetTargetTypeValueDescription(),
        TagTrackUids => GetTrackUidsDescription(),
        TagEditionUids => GetEditionUidsDescription(),
        TagChapterUids => GetChapterUidsDescription(),
        TagAttachmentUids => GetAttachmentUidsDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetTargetTypeValueDescription()
    {
        return Directory.TryGetUInt64(TagTargetTypeValue, out var value) ? value switch
        {
            10 => "Shot",
            20 => "Subtrack / Movement / Scene",
            30 => "Track / Song / Chapter",
            40 => "Part / Session",
            50 => "Album / Opera / Concert / Movie / Episode",
            60 => "Edition / Issue / Volume / Opus / Season / Sequel",
            70 => "Collection",
            _ => base.GetDescription(TagTargetTypeValue)
        } : null;
    }

    public string? GetTrackUidsDescription() => GetUidsDescription(TagTrackUids, "All Tracks");
    public string? GetEditionUidsDescription() => GetUidsDescription(TagEditionUids, "All Editions");
    public string? GetChapterUidsDescription() => GetUidsDescription(TagChapterUids, "All Chapters");
    public string? GetAttachmentUidsDescription() => GetUidsDescription(TagAttachmentUids, "All Attachments");

    private string? GetUidsDescription(int tagType, string all)
        => Directory.GetObject(tagType) is ulong[] uids
            ? string.Join(", ", uids.Select(uid => uid == 0 ? all : uid.ToString(System.Globalization.CultureInfo.InvariantCulture)))
            : base.GetDescription(tagType);
}
