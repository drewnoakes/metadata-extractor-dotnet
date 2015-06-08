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

namespace Com.Drew.Metadata.Jfif
{
    /// <summary>Directory of tags and values for the SOF0 Jfif segment.</summary>
    /// <remarks>Directory of tags and values for the SOF0 Jfif segment.  This segment holds basic metadata about the image.</remarks>
    /// <author>Yuri Binev, Drew Noakes</author>
    public class JfifDirectory : Directory
    {
        public const int TagVersion = 5;

        /// <summary>Units for pixel density fields.</summary>
        /// <remarks>Units for pixel density fields.  One of None, Pixels per Inch, Pixels per Centimetre.</remarks>
        public const int TagUnits = 7;

        public const int TagResx = 8;

        public const int TagResy = 10;

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static JfifDirectory()
        {
            _tagNameMap.Put(TagVersion, "Version");
            _tagNameMap.Put(TagUnits, "Resolution Units");
            _tagNameMap.Put(TagResy, "Y Resolution");
            _tagNameMap.Put(TagResx, "X Resolution");
        }

        public JfifDirectory()
        {
            this.SetDescriptor(new JfifDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "JFIF";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }

        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public virtual int GetVersion()
        {
            return GetInt(TagVersion);
        }

        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public virtual int GetResUnits()
        {
            return GetInt(TagUnits);
        }

        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public virtual int GetImageWidth()
        {
            return GetInt(TagResy);
        }

        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public virtual int GetImageHeight()
        {
            return GetInt(TagResx);
        }
    }
}
