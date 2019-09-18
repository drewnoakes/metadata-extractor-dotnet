// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class GifControlDescriptor : TagDescriptor<GifControlDirectory>
    {
        public GifControlDescriptor(GifControlDirectory directory)
            : base(directory)
        {
        }

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

            switch (value)
            {
                case 0:
                    return "Not Specified";
                case 1:
                    return "Don't Dispose";
                case 2:
                    return "Restore to Background Color";
                case 3:
                    return "Restore to Previous";
                case 4:
                case 5:
                case 6:
                case 7:
                    return "To Be Defined";
                default:
                    return $"Invalid value ({value})";
            }
        }
    }
}
