// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class BmpHeaderDescriptor : TagDescriptor<BmpHeaderDirectory>
    {
        public BmpHeaderDescriptor(BmpHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case BmpHeaderDirectory.TagBitmapType:
                    return GetBitmapTypeDescription();
                case BmpHeaderDirectory.TagCompression:
                    return GetCompressionDescription();
                case BmpHeaderDirectory.TagRendering:
                    return GetRenderingDescription();
                case BmpHeaderDirectory.TagColorEncoding:
                    return GetColorEncodingDescription();
                case BmpHeaderDirectory.TagRedMask:
                case BmpHeaderDirectory.TagGreenMask:
                case BmpHeaderDirectory.TagBlueMask:
                case BmpHeaderDirectory.TagAlphaMask:
                    return string.Format("0x{0}", Directory.GetInt64(tagType).ToString("X" + 8.ToString()));
                case BmpHeaderDirectory.TagColorSpaceType:
                    return GetColorSpaceTypeDescription();
                case BmpHeaderDirectory.TagGammaRed:
                case BmpHeaderDirectory.TagGammaGreen:
                case BmpHeaderDirectory.TagGammaBlue:
                    return $"{((double)Directory.GetInt64(tagType) / 0x10000):0.###}";
                //return FormatFixed1616(Directory.GetInt64(tagType));
                case BmpHeaderDirectory.TagIntent:
                    return GetRenderingIntentDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        public string? GetBitmapTypeDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagBitmapType, out int value))
                return null;

            return value switch
            {
                (int)BmpHeaderDirectory.BitmapType.Bitmap => "Standard",
                (int)BmpHeaderDirectory.BitmapType.OS2BitmapArray => "Bitmap Array",
                (int)BmpHeaderDirectory.BitmapType.OS2ColorIcon => "Color Icon",
                (int)BmpHeaderDirectory.BitmapType.OS2ColorPointer => "Color Pointer",
                (int)BmpHeaderDirectory.BitmapType.OS2Icon => "Monochrome Icon",
                (int)BmpHeaderDirectory.BitmapType.OS2Pointer => "Monochrome Pointer",
                _ => "Unimplemented bitmap type " + value.ToString(),
            };
        }

        public string? GetCompressionDescription()
        {
            // 0 = None
            // 1 = RLE 8-bit/pixel
            // 2 = RLE 4-bit/pixel
            // 3 = Bit fields (not OS22XBITMAPHEADER (size 64))
            // 3 = Huffman 1D (if OS22XBITMAPHEADER (size 64))
            // 4 = JPEG (not OS22XBITMAPHEADER (size 64))
            // 5 = PNG
            // 6 = RGBA bit fields
            // 11 = CMYK
            // 12 = CMYK RLE-8
            // 13 = CMYK RLE-4

            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagCompression, out int value) ||
                !Directory.TryGetInt32(BmpHeaderDirectory.TagHeaderSize, out int headerSize))
                return null;

            return value switch
            {
                (int)BmpHeaderDirectory.Compression.Rgb => "None",
                (int)BmpHeaderDirectory.Compression.Rle8 => "RLE 8-bit/pixel",
                (int)BmpHeaderDirectory.Compression.Rle4 => "RLE 4-bit/pixel",
                (int)BmpHeaderDirectory.Compression.BitFields => headerSize == 64 ? "Huffman 1D" : "Bit Fields",
                (int)BmpHeaderDirectory.Compression.Jpeg => headerSize == 64 ? "RLE 24-bit/pixel" : "JPEG",
                (int)BmpHeaderDirectory.Compression.Png => "PNG",
                (int)BmpHeaderDirectory.Compression.AlphaBitFields => "RGBA Bit Fields",
                (int)BmpHeaderDirectory.Compression.Cmyk => "CMYK Uncompressed",
                (int)BmpHeaderDirectory.Compression.CmykRle8 => "CMYK RLE-8",
                (int)BmpHeaderDirectory.Compression.CmykRle4 => "CMYK RLE-4",
                _ => "Unimplemented compression type " + value.ToString(),
            };
        }

        public string? GetRenderingDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagRendering, out int value))
                return null;

            return value switch
            {
                (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.None => "No Halftoning Algorithm",
                (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.ErrorDiffusion => "Error Diffusion Halftoning",
                (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.Panda => "Processing Algorithm for Noncoded Document Acquisition",
                (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.SuperCircle => "Super-circle Halftoning",
                _ => "Unimplemented rendering halftoning algorithm type " + value.ToString(),
            };
        }

        public string? GetColorEncodingDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagColorEncoding, out int value))
                return null;

            return value switch
            {
                (int)BmpHeaderDirectory.ColorEncoding.Rgb => "RGB",
                _ => "Unimplemented color encoding type " + value.ToString(),
            };
        }

        public string? GetColorSpaceTypeDescription()
        {
            if (!Directory.TryGetInt64(BmpHeaderDirectory.TagColorSpaceType, out long value))
                return null;

            return value switch
            {
                (long)BmpHeaderDirectory.ColorSpaceType.LcsCalibratedRgb => "Calibrated RGB",
                (long)BmpHeaderDirectory.ColorSpaceType.LcsSRgb => "sRGB Color Space",
                (long)BmpHeaderDirectory.ColorSpaceType.LcsWindowsColorSpace => "System Default Color Space, sRGB",
                (long)BmpHeaderDirectory.ColorSpaceType.ProfileLinked => "Linked Profile",
                (long)BmpHeaderDirectory.ColorSpaceType.ProfileEmbedded => "Embedded Profile",
                _ => "Unimplemented color space type " + value.ToString(),
            };
        }

        public string? GetRenderingIntentDescription()
        {
            if (!Directory.TryGetInt64(BmpHeaderDirectory.TagIntent, out long value))
                return null;

            return value switch
            {
                (long)BmpHeaderDirectory.RenderingIntent.LcsGmBusiness => "Graphic, Saturation",
                (long)BmpHeaderDirectory.RenderingIntent.LcsGmGraphics => "Proof, Relative Colorimetric",
                (long)BmpHeaderDirectory.RenderingIntent.LcsGmImages => "Picture, Perceptual",
                (long)BmpHeaderDirectory.RenderingIntent.LcsGmAbsColorimetric => "Match, Absolute Colorimetric",
                _ => "Unimplemented rendering intent type " + value.ToString(),
            };
        }
    }
}
