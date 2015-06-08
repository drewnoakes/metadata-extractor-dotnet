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
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PngChromaticitiesDirectory : Com.Drew.Metadata.Directory
    {
        public const int TagWhitePointX = 1;

        public const int TagWhitePointY = 2;

        public const int TagRedX = 3;

        public const int TagRedY = 4;

        public const int TagGreenX = 5;

        public const int TagGreenY = 6;

        public const int TagBlueX = 7;

        public const int TagBlueY = 8;

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static PngChromaticitiesDirectory()
        {
            _tagNameMap.Put(TagWhitePointX, "White Point X");
            _tagNameMap.Put(TagWhitePointY, "White Point Y");
            _tagNameMap.Put(TagRedX, "Red X");
            _tagNameMap.Put(TagRedY, "Red Y");
            _tagNameMap.Put(TagGreenX, "Green X");
            _tagNameMap.Put(TagGreenY, "Green Y");
            _tagNameMap.Put(TagBlueX, "Blue X");
            _tagNameMap.Put(TagBlueY, "Blue Y");
        }

        public PngChromaticitiesDirectory()
        {
            this.SetDescriptor(new TagDescriptor<Com.Drew.Metadata.Png.PngChromaticitiesDirectory>(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "PNG Chromaticities";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
