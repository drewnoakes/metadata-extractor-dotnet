// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MetadataExtractor.Formats.QuickTime
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class QuickTimeMetadataHeaderDirectory : Directory
    {
        public const int TagAlbum = 1;
        public const int TagArtist = 2;
        public const int TagArtwork = 3;
        public const int TagAuthor = 4;
        public const int TagComment = 5;
        public const int TagCopyright = 6;
        public const int TagCreationdate = 7;
        public const int TagDescription = 8;
        public const int TagDirector = 9;
        public const int TagTitle = 10;
        public const int TagGenre = 11;
        public const int TagInformation = 12;
        public const int TagKeywords = 13;
        public const int TagLocationIso6709 = 14;
        public const int TagProducer = 15;
        public const int TagPublisher = 16;
        public const int TagSoftware = 17;
        public const int TagYear = 18;
        public const int TagCollectionUser = 19;
        public const int TagRatingUser = 20;


        public override string Name { get; } = "QuickTime Metadata Header";

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagAlbum,             "com.apple.quicktime.album"},
            { TagArtist,            "com.apple.quicktime.artist"},
            { TagArtwork,           "com.apple.quicktime.artwork"},
            { TagAuthor,            "com.apple.quicktime.author"},
            { TagComment,           "com.apple.quicktime.comment"},
            { TagCopyright,         "com.apple.quicktime.copyright"},
            { TagCreationdate,      "com.apple.quicktime.creationdate"},
            { TagDescription,       "com.apple.quicktime.description"},
            { TagDirector,          "com.apple.quicktime.director"},
            { TagTitle,             "com.apple.quicktime.title"},
            { TagGenre,             "com.apple.quicktime.genre"},
            { TagInformation,       "com.apple.quicktime.information"},
            { TagKeywords,          "com.apple.quicktime.keywords"},
            { TagLocationIso6709,   "com.apple.quicktime.location.ISO6709"},
            { TagProducer,          "com.apple.quicktime.producer"},
            { TagPublisher,         "com.apple.quicktime.publisher"},
            { TagSoftware,          "com.apple.quicktime.software"},
            { TagYear,              "com.apple.quicktime.year"},
            { TagCollectionUser,    "com.apple.quicktime.collection.user"},
            { TagRatingUser,        "com.apple.quicktime.rating.user"}
        };

        private static readonly Dictionary<string, int> _nameTagMap = _tagNameMap.ToDictionary(e => e.Value, e => e.Key);

        public QuickTimeMetadataHeaderDirectory()
        {
            SetDescriptor(new TagDescriptor<QuickTimeMetadataHeaderDirectory>(this));
        }

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public bool TryGetTag(string name, out int tagType)
        {
            return _nameTagMap.TryGetValue(name, out tagType);
        }
    }
}
