// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga
{
    /// <author>Dmitry Shechtman</author>
    public sealed class TgaExtensionDescriptor(TgaExtensionDirectory directory) : TagDescriptor<TgaExtensionDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                TgaExtensionDirectory.TagAttributesType => GetAttributesTypeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetAttributesTypeDescription()
        {
            return GetIndexedDescription(
                TgaExtensionDirectory.TagAttributesType,
                "No alpha data included",
                "Undefined alpha data; ignore",
                "Undefined alpha data; retain",
                "Useful alpha data",
                "Pre-multiplied alpha data");
        }
    }
}
