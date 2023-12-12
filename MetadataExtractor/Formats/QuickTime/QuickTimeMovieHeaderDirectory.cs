// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeMovieHeaderDirectory : Directory
{
    public const int TagVersion = 1;
    public const int TagFlags = 2;
    public const int TagCreated = 3;
    public const int TagModified = 4;
    public const int TagTimeScale = 5;
    public const int TagDuration = 6;
    public const int TagPreferredRate = 7;
    public const int TagPreferredVolume = 8;
    public const int TagMatrix = 9;
    public const int TagPreviewTime = 10;
    public const int TagPreviewDuration = 11;
    public const int TagPosterTime = 12;
    public const int TagSelectionTime = 13;
    public const int TagSelectionDuration = 14;
    public const int TagCurrentTime = 15;
    public const int TagNextTrackId = 16;

    public override string Name => "QuickTime Movie Header";

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagVersion,           "Version" },
        { TagFlags,             "Flags" },
        { TagCreated,           "Created" },
        { TagModified,          "Modified" },
        { TagTimeScale,         "TrackId" },
        { TagDuration,          "Duration" },
        { TagPreferredRate,     "Preferred Rate" },
        { TagPreferredVolume,   "Preferred Volume" },
        { TagMatrix,            "Matrix" },
        { TagPreviewTime,       "Preview Time" },
        { TagPreviewDuration,   "Preview Duration" },
        { TagPosterTime,        "Poster Time" },
        { TagSelectionTime,     "Selection Time" },
        { TagSelectionDuration, "Selection Duration" },
        { TagCurrentTime,       "Current Time" },
        { TagNextTrackId,       "Next Track Id" }
    };

    public QuickTimeMovieHeaderDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new TagDescriptor<QuickTimeMovieHeaderDirectory>(this));
    }
}
