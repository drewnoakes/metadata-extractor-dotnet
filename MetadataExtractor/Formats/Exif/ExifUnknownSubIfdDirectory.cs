#region License
//
// Copyright 2002-2015 Drew Noakes
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
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Describes Exif subifd tags. This is returned when the current 
    /// Dictionary doesn't have a type defined for the given subifd</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifUnknownSubIfdDirectory : Directory
    {
        [NotNull] private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

        public ExifUnknownSubIfdDirectory()
        {
            SetDescriptor(new ExifUnknownSubIfdDescriptor(this));
        }

        public override string Name => "Unknown SubIfd";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
