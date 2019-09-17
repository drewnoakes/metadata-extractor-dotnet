// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Netpbm
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NetpbmHeaderDescriptor : TagDescriptor<NetpbmHeaderDirectory>
    {
        public NetpbmHeaderDescriptor(NetpbmHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                NetpbmHeaderDirectory.TagFormatType => GetFormatTypeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        private string? GetFormatTypeDescription()
        {
            return GetIndexedDescription(NetpbmHeaderDirectory.TagFormatType, 1,
                "Portable BitMap (ASCII, B&W)",
                "Portable GrayMap (ASCII, B&W)",
                "Portable PixMap (ASCII, B&W)",
                "Portable BitMap (RAW, B&W)",
                "Portable GrayMap (RAW, B&W)",
                "Portable PixMap (RAW, B&W)",
                "Portable Arbitrary Map");
        }
    }
}
