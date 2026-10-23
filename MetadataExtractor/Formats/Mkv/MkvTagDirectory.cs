// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

/// <summary>
/// The targets shared by one Tag's SimpleTag descendants. Target UIDs are stored as
/// <see cref="ulong"/> arrays, including single values. An absent UID list targets the whole
/// segment; a zero UID explicitly targets all elements of the corresponding kind.
/// </summary>
public sealed class MkvTagDirectory : Directory
{
    public const int TagTargetTypeValue = ElementIds.TargetTypeValue;
    public const int TagTargetType = ElementIds.TargetType;
    public const int TagTrackUids = ElementIds.TagTrackUid;
    public const int TagEditionUids = ElementIds.TagEditionUid;
    public const int TagChapterUids = ElementIds.TagChapterUid;
    public const int TagAttachmentUids = ElementIds.TagAttachmentUid;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagTargetTypeValue, "Target Type Value" },
        { TagTargetType, "Target Type" },
        { TagTrackUids, "Target Track UIDs" },
        { TagEditionUids, "Target Edition UIDs" },
        { TagChapterUids, "Target Chapter UIDs" },
        { TagAttachmentUids, "Target Attachment UIDs" }
    };

    public MkvTagDirectory() : base(_tagNameMap) => SetDescriptor(new MkvTagDescriptor(this));

    public override string Name => "Matroska Tag Targets";
}
