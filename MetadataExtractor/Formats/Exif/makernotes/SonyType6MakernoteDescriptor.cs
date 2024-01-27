// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SonyType6MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SonyType6MakernoteDescriptor(SonyType6MakernoteDirectory directory)
        : TagDescriptor<SonyType6MakernoteDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                SonyType6MakernoteDirectory.TagMakernoteThumbVersion => GetMakernoteThumbVersionDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetMakernoteThumbVersionDescription()
        {
            return GetVersionBytesDescription(SonyType6MakernoteDirectory.TagMakernoteThumbVersion, 2);
        }
    }
}
