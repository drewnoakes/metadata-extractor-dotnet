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

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Directory of tags and values for the SOF0 Jfif segment.</summary>
    /// <author>Yuri Binev, Drew Noakes</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JfifDirectory : Directory
    {
        public const int TagVersion = 5;

        /// <summary>Units for pixel density fields.</summary>
        /// <remarks>One of None, Pixels per Inch, Pixels per Centimetre.</remarks>
        public const int TagUnits = 7;

        public const int TagResX = 8;
        public const int TagResY = 10;
        public const int TagThumbWidth = 12;
        public const int TagThumbHeight = 13;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagVersion, "Version" },
            { TagUnits, "Resolution Units" },
            { TagResY, "Y Resolution" },
            { TagResX, "X Resolution" },
            { TagThumbWidth, "Thumbnail Width Pixels" },
            { TagThumbHeight, "Thumbnail Height Pixels" }
        };

        public JfifDirectory()
        {
            SetDescriptor(new JfifDescriptor(this));
        }

        public override string Name => "JFIF";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public int GetVersion()
        {
            return this.GetInt32(TagVersion);
        }

        public int GetResUnits()
        {
            return this.GetInt32(TagUnits);
        }

        public int GetImageWidth()
        {
            return this.GetInt32(TagResY);
        }

        public int GetImageHeight()
        {
            return this.GetInt32(TagResX);
        }
    }
}
