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
using Sharpen;

namespace Com.Drew.Metadata.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
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

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PngChromaticitiesDirectory()
        {
            TagNameMap[TagWhitePointX] = "White Point X";
            TagNameMap[TagWhitePointY] = "White Point Y";
            TagNameMap[TagRedX] = "Red X";
            TagNameMap[TagRedY] = "Red Y";
            TagNameMap[TagGreenX] = "Green X";
            TagNameMap[TagGreenY] = "Green Y";
            TagNameMap[TagBlueX] = "Blue X";
            TagNameMap[TagBlueY] = "Blue Y";
        }

        public PngChromaticitiesDirectory()
        {
            SetDescriptor(new TagDescriptor<PngChromaticitiesDirectory>(this));
        }

        public override string GetName()
        {
            return "PNG Chromaticities";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
