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

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>One of several Exif directories.</summary>
    /// <remarks>Holds information about image IFD's in a chain after the first. The first page is stored in IFD0.</remarks>
    /// <remarks>Currently, this only applies to multi-page TIFF images</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ExifImageDirectory : ExifDirectoryBase
    {
        public ExifImageDirectory()
        {
            SetDescriptor(new ExifImageDescriptor(this));
        }

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

        static ExifImageDirectory()
        {
            AddExifTagNames(_tagNameMap);
        }

        public override string Name => "Exif Image";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
