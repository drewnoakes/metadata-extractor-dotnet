// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class QuickTimeTrackHeaderDirectory : Directory
    {
        public const int TagVersion = 1;
        public const int TagFlags = 2;
        public const int TagCreated = 3;
        public const int TagModified = 4;
        public const int TagTrackId = 5;
        public const int TagDuration = 6;
        public const int TagLayer = 7;
        public const int TagAlternateGroup = 8;
        public const int TagVolume = 9;
        public const int TagWidth = 10;
        public const int TagHeight = 11;
        public const int TagMatrix = 12;
        public const int TagRotation = 13;

        public override string Name => "QuickTime Track Header";

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagVersion,        "Version" },
            { TagFlags,          "Flags" },
            { TagCreated,        "Created" },
            { TagModified,       "Modified" },
            { TagTrackId,        "TrackId" },
            { TagDuration,       "Duration" },
            { TagLayer,          "Layer" },
            { TagAlternateGroup, "Alternate Group" },
            { TagVolume,         "Volume" },
            { TagWidth,          "Width" },
            { TagHeight,         "Height" },
            { TagMatrix,         "Matrix" },
            { TagRotation,       "Rotation" },
        };

        public QuickTimeTrackHeaderDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new TagDescriptor<QuickTimeTrackHeaderDirectory>(this));
        }
    }
}
