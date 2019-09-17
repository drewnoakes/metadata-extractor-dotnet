// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="JpegDnlDirectory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JpegDnlDescriptor : TagDescriptor<JpegDnlDirectory>
    {
        public JpegDnlDescriptor(JpegDnlDirectory directory)
            : base(directory)
        {}

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                JpegDnlDirectory.TagImageHeight => GetImageHeightDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetImageHeightDescription()
        {
            var value = Directory.GetString(JpegDnlDirectory.TagImageHeight);

            return value == null ? null : value + " pixels";
        }
    }
}
