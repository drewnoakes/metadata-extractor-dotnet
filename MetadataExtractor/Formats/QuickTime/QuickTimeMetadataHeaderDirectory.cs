// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        public const int TagMake = 21;
        public const int TagModel = 22;
        public const int TagLocationName = 23;
        public const int TagLocationBody = 24;
        public const int TagLocationNote = 25;
        public const int TagLocationRole = 26;
        public const int TagLocationDate = 27;
        public const int TagDirectionFacing = 28;
        public const int TagDirectionMotion = 29;
        public const int TagDisplayName = 30;
        public const int TagContentIdentifier = 31;
        public const int TagOriginatingSignature = 32;
        public const int TagPixelDensity = 33;
        public const int TagAndroidVersion = 34;

        public override string Name => "QuickTime Metadata Header";

        private static readonly Dictionary<int, string> _tagNameMap = new()
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
            { TagRating,            "Rating"},
            { TagLocationName,      "Location Name" },
            { TagLocationBody,      "Location Body" },
            { TagLocationNote,      "Location Note" },
            { TagLocationRole,      "Location Role" },
            { TagLocationDate,      "Location Date" },
            { TagDirectionFacing,   "Direction Facing" },
            { TagDirectionMotion,   "Direction Motion" },
            { TagDisplayName,       "Display Name" },
            { TagContentIdentifier, "Content Identifier" },
            { TagMake,              "Make"},
            { TagModel,             "Model"},
            { TagOriginatingSignature, "Originating Signature" },
            { TagPixelDensity,         "Pixel Density" },
            { TagAndroidVersion,       "Android Version" },
        };

        private static readonly Dictionary<string, int> _nameTagMap = new()
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
            { "com.apple.quicktime.rating.user",        TagRating},
            { "com.apple.quicktime.location.name",      TagLocationName },
            { "com.apple.quicktime.location.body",      TagLocationBody },
            { "com.apple.quicktime.location.note",      TagLocationNote },
            { "com.apple.quicktime.location.role",      TagLocationRole },
            { "com.apple.quicktime.location.date",      TagLocationDate },
            { "com.apple.quicktime.direction.facing",   TagDirectionFacing },
            { "com.apple.quicktime.direction.motion",   TagDirectionMotion },
            { "com.apple.quicktime.displayname",        TagDisplayName },
            { "com.apple.quicktime.content.identifier", TagContentIdentifier },
            { "com.apple.quicktime.make",               TagMake},
            { "com.apple.quicktime.model",              TagModel},
            { "com.apple.photos.originating.signature", TagOriginatingSignature },
            { "com.apple.quicktime.pixeldensity",       TagPixelDensity },
            { "com.android.version",                    TagAndroidVersion },
        };

        public QuickTimeMetadataHeaderDirectory()
        {
            SetDescriptor(new QuickTimeMetadataHeaderDescriptor(this));
        }

        protected override bool TryGetTagName(int tagType, [NotNullWhen(returnValue: true)] out string? tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public static bool TryGetTag(string name, out int tagType)
        {
            return _nameTagMap.TryGetValue(name, out tagType);
        }
    }
}
