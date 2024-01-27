// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Provides human-readable string versions of the tags stored in a JpegDirectory.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a JpegDirectory.
    /// Thanks to Darrell Silver (www.darrellsilver.com) for the initial version of this class.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegDescriptor(JpegDirectory directory) : TagDescriptor<JpegDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                JpegDirectory.TagCompressionType => GetImageCompressionTypeDescription(),
                JpegDirectory.TagComponentData1 => GetComponentDataDescription(0),
                JpegDirectory.TagComponentData2 => GetComponentDataDescription(1),
                JpegDirectory.TagComponentData3 => GetComponentDataDescription(2),
                JpegDirectory.TagComponentData4 => GetComponentDataDescription(3),
                JpegDirectory.TagDataPrecision => GetDataPrecisionDescription(),
                JpegDirectory.TagImageHeight => GetImageHeightDescription(),
                JpegDirectory.TagImageWidth => GetImageWidthDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetImageCompressionTypeDescription()
        {
            return GetIndexedDescription(JpegDirectory.TagCompressionType,
                "Baseline",
                "Extended sequential, Huffman",
                "Progressive, Huffman",
                "Lossless, Huffman",
                null, // no 4
                "Differential sequential, Huffman",
                "Differential progressive, Huffman",
                "Differential lossless, Huffman",
                "Reserved for JPEG extensions",
                "Extended sequential, arithmetic",
                "Progressive, arithmetic",
                "Lossless, arithmetic",
                null, // no 12
                "Differential sequential, arithmetic",
                "Differential progressive, arithmetic",
                "Differential lossless, arithmetic");
        }

        public string? GetImageWidthDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagImageWidth);

            return value is null ? null : value + " pixels";
        }

        public string? GetImageHeightDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagImageHeight);

            return value is null ? null : value + " pixels";
        }

        public string? GetDataPrecisionDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagDataPrecision);

            return value is null ? null : value + " bits";
        }

        public string? GetComponentDataDescription(int componentNumber)
        {
            var value = Directory.GetComponent(componentNumber);

            if (value is null)
                return null;

            return $"{value.Name} component: {value}";
        }
    }
}
