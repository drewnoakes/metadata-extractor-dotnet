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

        public override string Name { get; } = "QuickTime Track Header";

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
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

        public QuickTimeTrackHeaderDirectory()
        {
            SetDescriptor(new TagDescriptor<QuickTimeTrackHeaderDirectory>(this));
        }

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}