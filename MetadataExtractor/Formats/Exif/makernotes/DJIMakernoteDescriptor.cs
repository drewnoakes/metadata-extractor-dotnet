// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="DJIMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Using information from https://metacpan.org/pod/distribution/Image-ExifTool/lib/Image/ExifTool/TagNames.pod#DJI-Tags</remarks>
    /// <author>Charlie Matherne, adapted from Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DJIMakernoteDescriptor : TagDescriptor<DJIMakernoteDirectory>
    {
        public DJIMakernoteDescriptor(DJIMakernoteDirectory directory)
            : base(directory)
        {
        }
    }
}
