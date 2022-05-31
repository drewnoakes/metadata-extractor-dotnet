// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using static MetadataExtractor.Formats.Exif.Makernotes.AppleMakernoteDirectory;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="AppleMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Using information from http://owl.phy.queensu.ca/~phil/exiftool/TagNames/Apple.html</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AppleMakernoteDescriptor : TagDescriptor<AppleMakernoteDirectory>
    {
        public AppleMakernoteDescriptor(AppleMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                TagHdrImageType => GetHdrImageTypeDescription(),
                TagAccelerationVector => GetAccelerationVectorDescription(),
                _ => base.GetDescription(tagType)
            };
        }

        public string? GetHdrImageTypeDescription()
        {
            return GetIndexedDescription(TagHdrImageType, 3, "HDR Image", "Original Image");
        }

        public string? GetAccelerationVectorDescription()
        {
            var values = Directory.GetRationalArray(TagAccelerationVector);
            if (values is null || values.Length != 3)
                return null;
            return $"{values[0].Absolute.ToDouble():N2}g {(values[0].IsPositive ? "left" : "right")}, " +
                   $"{values[1].Absolute.ToDouble():N2}g {(values[1].IsPositive ? "down" : "up")}, " +
                   $"{values[2].Absolute.ToDouble():N2}g {(values[2].IsPositive ? "forward" : "backward")}";
        }
    }
}
