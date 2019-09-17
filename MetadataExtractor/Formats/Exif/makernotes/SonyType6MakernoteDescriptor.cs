// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SonyType6MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class SonyType6MakernoteDescriptor : TagDescriptor<SonyType6MakernoteDirectory>
    {
        public SonyType6MakernoteDescriptor(SonyType6MakernoteDirectory directory)
            : base(directory)
        {
        }

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
