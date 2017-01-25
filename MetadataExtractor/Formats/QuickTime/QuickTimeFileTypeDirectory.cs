#region License
//
// Copyright 2002-2017 Drew Noakes
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

namespace MetadataExtractor.Formats.QuickTime
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class QuickTimeFileTypeDirectory : Directory
    {
        public const int TagMajorBrand = 1;
        public const int TagMinorVersion = 2;
        public const int TagCompatibleBrands = 3;

        public override string Name { get; } = "QuickTime File Type";

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMajorBrand,       "Major Brand" },
            { TagMinorVersion,     "Minor Version" },
            { TagCompatibleBrands, "Compatible Brands" }
        };

        public QuickTimeFileTypeDirectory()
        {
            SetDescriptor(new TagDescriptor<QuickTimeFileTypeDirectory>(this));
        }

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}