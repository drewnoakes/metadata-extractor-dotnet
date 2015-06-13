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

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Directory of tags and values for the SOF0 Jfif segment.</summary>
    /// <remarks>Directory of tags and values for the SOF0 Jfif segment.  This segment holds basic metadata about the image.</remarks>
    /// <author>Yuri Binev, Drew Noakes</author>
    public sealed class JfifDirectory : Directory
    {
        public const int TagVersion = 5;

        /// <summary>Units for pixel density fields.</summary>
        /// <remarks>Units for pixel density fields.  One of None, Pixels per Inch, Pixels per Centimetre.</remarks>
        public const int TagUnits = 7;

        public const int TagResX = 8;

        public const int TagResY = 10;

        [NotNull] private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static JfifDirectory()
        {
            TagNameMap[TagVersion] = "Version";
            TagNameMap[TagUnits] = "Resolution Units";
            TagNameMap[TagResY] = "Y Resolution";
            TagNameMap[TagResX] = "X Resolution";
        }

        public JfifDirectory()
        {
            SetDescriptor(new JfifDescriptor(this));
        }

        public override string Name
        {
            get { return "JFIF"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        /// <exception cref="MetadataException"/>
        public int GetVersion()
        {
            return this.GetInt(TagVersion);
        }

        /// <exception cref="MetadataException"/>
        public int GetResUnits()
        {
            return this.GetInt(TagUnits);
        }

        /// <exception cref="MetadataException"/>
        public int GetImageWidth()
        {
            return this.GetInt(TagResY);
        }

        /// <exception cref="MetadataException"/>
        public int GetImageHeight()
        {
            return this.GetInt(TagResX);
        }
    }
}
