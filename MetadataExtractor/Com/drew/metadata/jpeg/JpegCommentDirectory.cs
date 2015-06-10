/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Jpeg
{
    /// <summary>Describes tags used by a JPEG file comment.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class JpegCommentDirectory : Directory
    {
        /// <summary>This value does not apply to a particular standard.</summary>
        /// <remarks>
        /// This value does not apply to a particular standard. Rather, this value has been fabricated to maintain
        /// consistency with other directory types.
        /// </remarks>
        public const int TagComment = 0;

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static JpegCommentDirectory()
        {
            TagNameMap[TagComment] = "JPEG Comment";
        }

        public JpegCommentDirectory()
        {
            SetDescriptor(new JpegCommentDescriptor(this));
        }

        public override string GetName()
        {
            return "JpegComment";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
