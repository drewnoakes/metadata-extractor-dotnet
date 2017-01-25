#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
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

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PngChromaticitiesDirectory : Directory
    {
        public const int TagWhitePointX = 1;
        public const int TagWhitePointY = 2;
        public const int TagRedX = 3;
        public const int TagRedY = 4;
        public const int TagGreenX = 5;
        public const int TagGreenY = 6;
        public const int TagBlueX = 7;
        public const int TagBlueY = 8;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagWhitePointX, "White Point X" },
            { TagWhitePointY, "White Point Y" },
            { TagRedX, "Red X" },
            { TagRedY, "Red Y" },
            { TagGreenX, "Green X" },
            { TagGreenY, "Green Y" },
            { TagBlueX, "Blue X" },
            { TagBlueY, "Blue Y" }
        };

        public PngChromaticitiesDirectory()
        {
            SetDescriptor(new TagDescriptor<PngChromaticitiesDirectory>(this));
        }

        public override string Name => "PNG Chromaticities";

        protected override bool TryGetTagName(int tagType, out string tagName) => _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
