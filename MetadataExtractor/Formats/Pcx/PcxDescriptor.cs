// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Pcx
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PcxDescriptor : TagDescriptor<PcxDirectory>
    {
        public PcxDescriptor(PcxDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PcxDirectory.TagVersion => GetVersionDescription(),
                PcxDirectory.TagColorPlanes => GetColorPlanesDescription(),
                PcxDirectory.TagPaletteType => GetPaletteTypeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetVersionDescription()
        {
            // Prior to v2.5 of PC Paintbrush, the PCX image file format was considered proprietary information
            // by ZSoft Corporation
            return GetIndexedDescription(PcxDirectory.TagVersion,
                "2.5 with fixed EGA palette information",
                null,
                "2.8 with modifiable EGA palette information",
                "2.8 without palette information (default palette)",
                "PC Paintbrush for Windows",
                "3.0 or better");
        }

        public string? GetColorPlanesDescription()
        {
            return GetIndexedDescription(PcxDirectory.TagColorPlanes, 3, "24-bit color", "16 colors");
        }

        public string? GetPaletteTypeDescription()
        {
            return GetIndexedDescription(PcxDirectory.TagPaletteType, 1, "Color or B&W", "Grayscale");
        }
    }
}
