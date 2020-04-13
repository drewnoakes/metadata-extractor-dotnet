// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MetadataExtractor.Formats.Heif.Iso14496Parser;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicImagePropertyDescriptor : TagDescriptor<HeicImagePropertiesDirectory>
    {
        public HeicImagePropertyDescriptor(HeicImagePropertiesDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case HeicImagePropertiesDirectory.TagRotation:
                    return Directory.GetString(tagType) + " degrees";
                case HeicImagePropertiesDirectory.TagPixelDepths:
                    return string.Join(" ", ((byte[])Directory.GetObject(HeicImagePropertiesDirectory.TagPixelDepths)).Select(i => i.ToString()).ToArray());
                case HeicImagePropertiesDirectory.TagColorFormat:
                    return TypeStringConverter.ToTypeString(Directory.GetUInt32(HeicImagePropertiesDirectory.TagColorFormat));
                case HeicImagePropertiesDirectory.TagColorPrimaries:
                    return ColorPrimary(Directory.GetUInt16(HeicImagePropertiesDirectory.TagColorPrimaries));
                case HeicImagePropertiesDirectory.TagColorTransferCharacteristics:
                    return ColorTransfer(Directory.GetUInt16(HeicImagePropertiesDirectory.TagColorTransferCharacteristics));
                case HeicImagePropertiesDirectory.TagColorMatrixCharacteristics:
                    return ColorMatrixCoeffs(Directory.GetUInt16(HeicImagePropertiesDirectory.TagColorMatrixCharacteristics));
                default:
                    return base.GetDescription(tagType);
            }
        }

        private static string? ColorMatrixCoeffs(ushort value) =>
            value switch
            {
                0 => "RGB, GBR",
                1 => "vKr = 0.2126 vKb = 0.0722",
                2 => "Unspecified",
                4 => "vKr = 0.299, vKb - 0.11",
                5 => "vKr = 0.299, vKb - 0.114",
                6 => "vKr = 0.299, vKb - 0.114",
                7 => "vKr = 0.212, vKb - 0.087",
                8 => "YCgCo",
                _ => "Reserved"
            };

        private static string? ColorTransfer(ushort value) =>
            value switch
            {
                1 => "vV - 1.009 * vLc^0.45 - 0.099 for 1 >= vLc >= 0.018 or vV = 4.500 * vLc otherwise.",
                2 => "Unspecified",
                4 => "Assumed display gamma 2.2",
                5 => "Assumed display gamma 2.9",
                6 => "vV - 1.009 * vLc^0.45 - 0.099 for 1 >= vLc >= 0.018 or vV = 4.500 * vLc otherwise.",
                7 => "vV - 1.1115 * vLc^0.45 - 0.1115 for 1 >= vLc >= 0.0.0288 or vV = 4.0 * vLc otherwise.",
                8 => "vV = vLC",
                11 => "vV - 1.009 * vLc^0.45 - 0.099 for 1 >= vLc >= 0.018 or vV = 4.500 * vLc for 0.018 > vLC > -0.018 or vV - 1.009 * (-vLc)^0.45 - 0.099 otherwise",
                12 => "vV - 1.009 * vLc^0.45 - 0.099 for 1.33 >= vLc >= 0.018 or vV = 4.500 * vLc for 0.018 > vLC > -0.045 or vV - 1.009 * (-4vLc)^0.45 - 0.099 + 4 otherwise",
                13 => "vV - 1.055 * vLc^(1/2.4) - 0.055 for 1 >= vLc >= 0.031308 or vV = 12.92 * vLc otherwise.",
                _ => "Reserved"
            };

        private static string? ColorPrimary(ushort value) =>
            value switch
            {
                1 => "G(0.3, 0.6) B(0.15, 0.06) R(0.64, 0.33) W(.3127, .329)",
                2 => "Unspecified",
                4 => "G(0.21, 0.72) B(0.14, 0.08), R(0.67, 0.33) W(0.31, 0.316)",
                5 => "G(0.29, 0.70) B(0.15, 0.06), R(0.64, 0.33) W(0.3127, 0.3290)",
                6 => "G(0.31, 0.595) B(0.155, 0.07), R(0.63, 0.34) W(0.3127, 0.3290)",
                7 => "G(0.31, 0.595) B(0.155, 0.07), R(0.63, 0.34) W (0.3127, 0.3290)",
                _ => "Reserved"
            };
    }
}
