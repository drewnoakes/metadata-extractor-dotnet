// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Eps
{
    /// <summary>Provides human-readable string versions of the tags stored in a <see cref="EpsDirectory"/>.</summary>
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class EpsDescriptor(EpsDirectory directory)
        : TagDescriptor<EpsDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case EpsDirectory.TagImageWidth:
                case EpsDirectory.TagImageHeight:
                    return GetPixelDescription(tagType);
                case EpsDirectory.TagTiffPreviewSize:
                case EpsDirectory.TagTiffPreviewOffset:
                    return GetByteSizeDescription(tagType);
                case EpsDirectory.TagColorType:
                    return GetColorTypeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        public string GetPixelDescription(int tagType)
        {
            return Directory.GetString(tagType) + " pixels";
        }

        public string GetByteSizeDescription(int tagType)
        {
            return Directory.GetString(tagType) + " bytes";
        }

        public string? GetColorTypeDescription()
        {
            return GetIndexedDescription(EpsDirectory.TagColorType, 1,
                "Grayscale", "Lab", "RGB", "CMYK");
        }
    }
}
