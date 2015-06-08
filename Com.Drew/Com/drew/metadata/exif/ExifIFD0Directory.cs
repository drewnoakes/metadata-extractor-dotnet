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

namespace Com.Drew.Metadata.Exif
{
    /// <summary>Describes Exif tags from the IFD0 directory.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifIfd0Directory : ExifDirectoryBase
    {
        /// <summary>This tag is a pointer to the Exif SubIFD.</summary>
        public const int TagExifSubIfdOffset = unchecked((int)(0x8769));

        /// <summary>This tag is a pointer to the Exif GPS IFD.</summary>
        public const int TagGpsInfoOffset = unchecked((int)(0x8825));

        public ExifIfd0Directory()
        {
            SetDescriptor(new ExifIfd0Descriptor(this));
        }

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static ExifIfd0Directory()
        {
            AddExifTagNames(TagNameMap);
        }

        [NotNull]
        public override string GetName()
        {
            return "Exif IFD0";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
