// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Avi
{
    /// <author>Payton Garland</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AviDescriptor : TagDescriptor<AviDirectory>
    {
        public AviDescriptor(AviDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case AviDirectory.TAG_WIDTH:
                case AviDirectory.TAG_HEIGHT:
                    return Directory.GetString(tagType) + " pixels";
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
