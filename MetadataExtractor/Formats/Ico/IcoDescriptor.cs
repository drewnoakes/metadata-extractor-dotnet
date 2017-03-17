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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Ico
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class IcoDescriptor : TagDescriptor<IcoDirectory>
    {
        public IcoDescriptor([NotNull] IcoDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case IcoDirectory.TagImageType:
                {
                    return GetImageTypeDescription();
                }

                case IcoDirectory.TagImageWidth:
                {
                    return GetImageWidthDescription();
                }

                case IcoDirectory.TagImageHeight:
                {
                    return GetImageHeightDescription();
                }

                case IcoDirectory.TagColourPaletteSize:
                {
                    return GetColourPaletteSizeDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetImageTypeDescription()
        {
            return GetIndexedDescription(IcoDirectory.TagImageType, 1, "Icon", "Cursor");
        }

        [CanBeNull]
        public string GetImageWidthDescription()
        {
            if (!Directory.TryGetInt32(IcoDirectory.TagImageWidth, out int width))
                return null;
            return (width == 0 ? 256 : width) + " pixels";
        }

        [CanBeNull]
        public string GetImageHeightDescription()
        {
            if (!Directory.TryGetInt32(IcoDirectory.TagImageHeight, out int height))
                return null;
            return (height == 0 ? 256 : height) + " pixels";
        }

        [CanBeNull]
        public string GetColourPaletteSizeDescription()
        {
            if (!Directory.TryGetInt32(IcoDirectory.TagColourPaletteSize, out int size))
                return null;
            return size == 0 ? "No palette" : size + " colour" + (size == 1 ? string.Empty : "s");
        }
    }
}
