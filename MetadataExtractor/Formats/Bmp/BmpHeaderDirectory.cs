// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class BmpHeaderDirectory : Directory
    {
        public const int TagBitmapType = -2;
        public const int TagHeaderSize = -1;
        public const int TagImageHeight = 1;
        public const int TagImageWidth = 2;
        public const int TagColourPlanes = 3;
        public const int TagBitsPerPixel = 4;
        public const int TagCompression = 5;
        public const int TagXPixelsPerMeter = 6;
        public const int TagYPixelsPerMeter = 7;
        public const int TagPaletteColourCount = 8;
        public const int TagImportantColourCount = 9;
        public const int TagRendering = 10;
        public const int TagColorEncoding = 11;
        public const int TagRedMask = 12;
        public const int TagGreenMask = 13;
        public const int TagBlueMask = 14;
        public const int TagAlphaMask = 15;
        public const int TagColorSpaceType = 16;
        public const int TagGammaRed = 17;
        public const int TagGammaGreen = 18;
        public const int TagGammaBlue = 19;
        public const int TagIntent = 20;
        public const int TagLinkedProfile = 21;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagBitmapType, "Bitmap type" },
            { TagHeaderSize, "Header Size" },
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagColourPlanes, "Planes" },
            { TagBitsPerPixel, "Bits Per Pixel" },
            { TagCompression, "Compression" },
            { TagXPixelsPerMeter, "X Pixels per Meter" },
            { TagYPixelsPerMeter, "Y Pixels per Meter" },
            { TagPaletteColourCount, "Palette Colour Count" },
            { TagImportantColourCount, "Important Colour Count" },
            { TagRendering, "Rendering" },
            { TagColorEncoding, "Color Encoding" },
            { TagRedMask, "Red Mask" },
            { TagGreenMask, "Green Mask" },
            { TagBlueMask, "Blue Mask" },
            { TagAlphaMask, "Alpha Mask" },
            { TagColorSpaceType, "Color Space Type" },
            { TagGammaRed, "Red Gamma Curve" },
            { TagGammaGreen, "Green Gamma Curve" },
            { TagGammaBlue, "Blue Gamma Curve" },
            { TagIntent, "Rendering Intent" },
            { TagLinkedProfile, "Linked Profile File Name" }
        };

        public BmpHeaderDirectory()
        {
            SetDescriptor(new BmpHeaderDescriptor(this));
        }

        public override string Name => "BMP Header";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <summary>
        /// Possible "magic bytes" indicating bitmap type
        /// </summary>
        public enum BitmapType : int
        {
            /** "BM" - Windows or OS/2 bitmap */
            Bitmap = 0x4D42,

            /** "BA" - OS/2 Bitmap array (multiple bitmaps) */
            OS2BitmapArray = 0x4142,

            /** "IC" - OS/2 Icon */
            OS2Icon = 0x4349,

            /** "CI" - OS/2 Color icon */
            OS2ColorIcon = 0x4943,

            /** "CP" - OS/2 Color pointer */
            OS2ColorPointer = 0x5043,

            /** "PT" - OS/2 Pointer */
            OS2Pointer = 0x5450
        }

        public enum Compression : int
        {
            /** 0 = None */
            Rgb = 0,

            /** 1 = RLE 8-bit/pixel */
            Rle8 = 1,

            /** 2 = RLE 4-bit/pixel */
            Rle4 = 2,

            /** 3 = Bit fields (not OS22XBITMAPHEADER (size 64)) */
            BitFields = 3,

            /** 3 = Huffman 1D (if OS22XBITMAPHEADER (size 64)) */
            Huffman1D = 3,

            /** 4 = JPEG (not OS22XBITMAPHEADER (size 64)) */
            Jpeg = 4,

            /** 4 = RLE 24-bit/pixel (if OS22XBITMAPHEADER (size 64)) */
            Rle24 = 4,

            /** 5 = PNG */
            Png = 5,

            /** 6 = RGBA bit fields */
            AlphaBitFields = 6,

            /** 11 = CMYK */
            Cmyk = 11,

            /** 12 = CMYK RLE-8 */
            CmykRle8 = 12,

            /** 13 = CMYK RLE-4 */
            CmykRle4 = 13
        }

        public enum RenderingHalftoningAlgorithm : int
        {
            /** No halftoning algorithm */
            None = 0,

            /** Error Diffusion Halftoning */
            ErrorDiffusion = 1,

            /** Processing Algorithm for Noncoded Document Acquisition */
            Panda = 2,

            /** Super-circle Halftoning */
            SuperCircle = 3
        }

        public enum ColorEncoding : int
        {
            Rgb = 0
        }

        public enum ColorSpaceType : long
        {
            /** 0 = Calibrated RGB */
            LcsCalibratedRgb = 0L,

            /** "sRGB" = sRGB Color Space */
            LcsSRgb = 0x73524742L,

            /** "Win " = System Default Color Space, sRGB */
            LcsWindowsColorSpace = 0x57696E20L,

            /** "LINK" = Linked Profile */
            ProfileLinked = 0x4C494E4BL,

            /** "MBED" = Embedded Profile */
            ProfileEmbedded = 0x4D424544L
        }

        public enum RenderingIntent : long
        {
            /** Graphic, Saturation */
            LcsGmBusiness = 1,

            /** Proof, Relative Colorimetric */
            LcsGmGraphics = 2,

            /** Picture, Perceptual */
            LcsGmImages = 4,

            /** Match, Absolute Colorimetric */
            LcsGmAbsColorimetric = 8
        }
    }
}
