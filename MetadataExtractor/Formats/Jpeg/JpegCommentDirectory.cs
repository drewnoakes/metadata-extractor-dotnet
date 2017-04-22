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

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Describes tags used by a JPEG file comment.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class JpegCommentDirectory : Directory
    {
        /// <summary>This value does not apply to a particular standard.</summary>
        /// <remarks>
        /// This value does not apply to a particular standard. Rather, this value has been fabricated to maintain
        /// consistency with other directory types.
        /// </remarks>
        public const int TagComment = 0;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagComment, "JPEG Comment" }
        };

        public JpegCommentDirectory(StringValue comment)
        {
            SetDescriptor(new JpegCommentDescriptor(this));
            Set(TagComment, comment);
        }

        public override string Name => "JpegComment";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
