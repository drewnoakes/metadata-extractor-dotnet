// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SigmaMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SigmaMakernoteDescriptor : TagDescriptor<SigmaMakernoteDirectory>
    {
        public SigmaMakernoteDescriptor(SigmaMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                SigmaMakernoteDirectory.TagExposureMode => GetExposureModeDescription(),
                SigmaMakernoteDirectory.TagMeteringMode => GetMeteringModeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        private string? GetMeteringModeDescription()
        {
            var value = Directory.GetString(SigmaMakernoteDirectory.TagMeteringMode);
            if (string.IsNullOrEmpty(value))
                return null;

            return (value![0]) switch
            {
                '8' => "Multi Segment",
                'A' => "Average",
                'C' => "Center Weighted Average",
                _ => value,
            };
        }

        private string? GetExposureModeDescription()
        {
            var value = Directory.GetString(SigmaMakernoteDirectory.TagExposureMode);
            if (string.IsNullOrEmpty(value))
                return null;

            return (value![0]) switch
            {
                'A' => "Aperture Priority AE",
                'M' => "Manual",
                'P' => "Program AE",
                'S' => "Shutter Speed Priority AE",
                _ => value,
            };
        }
    }
}
