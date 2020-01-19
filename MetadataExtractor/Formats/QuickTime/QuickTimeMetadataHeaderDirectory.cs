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
        public const int TagCreationDate = 7;
        public const int TagDescription = 8;
        public const int TagDirector = 9;
        public const int TagTitle = 10;
        public const int TagGenre = 11;
        public const int TagInformation = 12;
        public const int TagKeywords = 13;
        public const int TagGpsLocation = 14;
        public const int TagProducer = 15;
        public const int TagPublisher = 16;
        public const int TagSoftware = 17;
        public const int TagYear = 18;
        public const int TagCollection = 19;
        public const int TagRating = 20;

        public override string Name => "QuickTime Metadata Header";

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagAlbum,             "Album"},
            { TagArtist,            "Artist"},
            { TagArtwork,           "Artwork"},
            { TagAuthor,            "Author"},
            { TagComment,           "Comment"},
            { TagCopyright,         "Copyright"},
            { TagCreationDate,      "Creation Date"},
            { TagDescription,       "Description"},
            { TagDirector,          "Director"},
            { TagTitle,             "Title"},
            { TagGenre,             "Genre"},
            { TagInformation,       "Information"},
            { TagKeywords,          "Keywords"},
            { TagGpsLocation,       "GPS Location"},
            { TagProducer,          "Producer"},
            { TagPublisher,         "Publisher"},
            { TagSoftware,          "Software"},
            { TagYear,              "Year"},
            { TagCollection,        "Collection"},
            { TagRating,            "Rating"}
        };

        private static readonly Dictionary<string, int> _nameTagMap = new Dictionary<string, int>
        {
            { "com.apple.quicktime.album",              TagAlbum},
            { "com.apple.quicktime.artist",             TagArtist},
            { "com.apple.quicktime.artwork",            TagArtwork},
            { "com.apple.quicktime.author",             TagAuthor},
            { "com.apple.quicktime.comment",            TagComment},
            { "com.apple.quicktime.copyright",          TagCopyright},
            { "com.apple.quicktime.creationdate",       TagCreationDate},
            { "com.apple.quicktime.description",        TagDescription},
            { "com.apple.quicktime.director",           TagDirector},
            { "com.apple.quicktime.title",              TagTitle},
            { "com.apple.quicktime.genre",              TagGenre},
            { "com.apple.quicktime.information",        TagInformation},
            { "com.apple.quicktime.keywords",           TagKeywords},
            { "com.apple.quicktime.location.ISO6709",   TagGpsLocation},
            { "com.apple.quicktime.producer",           TagProducer},
            { "com.apple.quicktime.publisher",          TagPublisher},
            { "com.apple.quicktime.software",           TagSoftware},
            { "com.apple.quicktime.year",               TagYear},
            { "com.apple.quicktime.collection.user",    TagCollection},
            { "com.apple.quicktime.rating.user",        TagRating}
        };

        public QuickTimeMetadataHeaderDirectory()
        {
            SetDescriptor(new TagDescriptor<QuickTimeMetadataHeaderDirectory>(this));
        }

        protected override bool TryGetTagName(int tagType, [NotNullWhen(returnValue: true)] out string? tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public bool TryGetTag(string name, out int tagType)
        {
            return _nameTagMap.TryGetValue(name, out tagType);
        }
    }
}
