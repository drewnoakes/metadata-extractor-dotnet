#region License
//
// Copyright 2002-2017 Drew Noakes
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.QuickTime
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

        public override string Name { get; } = "QuickTime Movie Header";

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
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

        public QuickTimeMovieHeaderDirectory()
        {
            SetDescriptor(new TagDescriptor<QuickTimeMovieHeaderDirectory>(this));
        }

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}