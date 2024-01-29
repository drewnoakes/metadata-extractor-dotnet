// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PrintIMDirectory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PrintIMDescriptor(PrintIMDirectory directory)
        : TagDescriptor<PrintIMDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PrintIMDirectory.TagPrintImVersion:
                    return base.GetDescription(tagType);
                default:
                    if (!Directory.TryGetUInt32(tagType, out uint value))
                        return null;
                    return "0x" + value.ToString("x8");
            }
        }
    }
}
