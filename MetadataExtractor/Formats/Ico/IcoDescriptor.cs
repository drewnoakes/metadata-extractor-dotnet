// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Ico
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class IcoDescriptor : TagDescriptor<IcoDirectory>
    {
        public IcoDescriptor(IcoDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
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

        public string? GetImageTypeDescription()
        {
            return GetIndexedDescription(IcoDirectory.TagImageType, 1, "Icon", "Cursor");
        }

        public string? GetImageWidthDescription()
        {
            if (!Directory.TryGetInt32(IcoDirectory.TagImageWidth, out int width))
                return null;
            return (width == 0 ? 256 : width) + " pixels";
        }

        public string? GetImageHeightDescription()
        {
            if (!Directory.TryGetInt32(IcoDirectory.TagImageHeight, out int height))
                return null;
            return (height == 0 ? 256 : height) + " pixels";
        }

        public string? GetColourPaletteSizeDescription()
        {
            if (!Directory.TryGetInt32(IcoDirectory.TagColourPaletteSize, out int size))
                return null;
            return size == 0 ? "No palette" : size + " colour" + (size == 1 ? string.Empty : "s");
        }
    }
}
