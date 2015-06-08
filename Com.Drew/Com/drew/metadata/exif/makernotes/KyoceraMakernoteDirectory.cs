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

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>Describes tags specific to Kyocera and Contax cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class KyoceraMakernoteDirectory : Com.Drew.Metadata.Directory
    {
        public const int TagProprietaryThumbnail = unchecked((int)(0x0001));

        public const int TagPrintImageMatchingInfo = unchecked((int)(0x0E00));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static KyoceraMakernoteDirectory()
        {
            _tagNameMap.Put(TagProprietaryThumbnail, "Proprietary Thumbnail Format Data");
            _tagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info");
        }

        public KyoceraMakernoteDirectory()
        {
            this.SetDescriptor(new KyoceraMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Kyocera/Contax Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
