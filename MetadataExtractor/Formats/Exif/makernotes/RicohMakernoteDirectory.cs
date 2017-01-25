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

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Ricoh cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class RicohMakernoteDirectory : Directory
    {
        public const int TagMakernoteDataType = 0x0001;
        public const int TagVersion = 0x0002;
        public const int TagPrintImageMatchingInfo = 0x0E00;
        public const int TagRicohCameraInfoMakernoteSubIfdPointer = 0x2001;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakernoteDataType, "Makernote Data Type" },
            { TagVersion, "Version" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagRicohCameraInfoMakernoteSubIfdPointer, "Ricoh Camera Info Makernote Sub-IFD" }
        };

        public RicohMakernoteDirectory()
        {
            SetDescriptor(new RicohMakernoteDescriptor(this));
        }

        public override string Name => "Ricoh Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
