// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class GifControlDescriptor(GifControlDirectory directory)
        : TagDescriptor<GifControlDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                GifControlDirectory.TagDisposalMethod => GetDisposalMethodDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetDisposalMethodDescription()
        {
            if (!Directory.TryGetInt32(GifControlDirectory.TagDisposalMethod, out int value))
                return null;

            return value switch
            {
                0 => "Not Specified",
                1 => "Don't Dispose",
                2 => "Restore to Background Color",
                3 => "Restore to Previous",
                4 or 5 or 6 or 7 => "To Be Defined",
                _ => $"Invalid value ({value})"
            };
        }
    }
}
