// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
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

        private static readonly Dictionary<int, string> _tagNameMap = new()
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

        public BmpHeaderDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new BmpHeaderDescriptor(this));
        }

        public override string Name => "BMP Header";

        /// <summary>
        /// Possible "magic bytes" indicating bitmap type
        /// </summary>
        public enum BitmapType : int
        {
            /// <summary>"BM" - Windows or OS/2 bitmap</summary>
            Bitmap = 0x4D42,

            /// <summary>"BA" - OS/2 Bitmap array (multiple bitmaps)</summary>
            OS2BitmapArray = 0x4142,

            /// <summary>"IC" - OS/2 Icon</summary>
            OS2Icon = 0x4349,

            /// <summary>"CI" - OS/2 Color icon</summary>
            OS2ColorIcon = 0x4943,

            /// <summary>"CP" - OS/2 Color pointer</summary>
            OS2ColorPointer = 0x5043,

            /// <summary>"PT" - OS/2 Pointer</summary>
            OS2Pointer = 0x5450
        }

        public enum Compression : int
        {
            /// <summary>0 = None</summary>
            Rgb = 0,

            /// <summary>1 = RLE 8-bit/pixel</summary>
            Rle8 = 1,

            /// <summary>2 = RLE 4-bit/pixel</summary>
            Rle4 = 2,

            /// <summary>3 = Bit fields (not OS22XBITMAPHEADER (size 64))</summary>
            BitFields = 3,

            /// <summary>3 = Huffman 1D (if OS22XBITMAPHEADER (size 64))</summary>
            Huffman1D = 3,

            /// <summary>4 = JPEG (not OS22XBITMAPHEADER (size 64))</summary>
            Jpeg = 4,

            /// <summary>4 = RLE 24-bit/pixel (if OS22XBITMAPHEADER (size 64))</summary>
            Rle24 = 4,

            /// <summary>5 = PNG</summary>
            Png = 5,

            /// <summary>6 = RGBA bit fields</summary>
            AlphaBitFields = 6,

            /// <summary>11 = CMYK</summary>
            Cmyk = 11,

            /// <summary>12 = CMYK RLE-8</summary>
            CmykRle8 = 12,

            /// <summary>13 = CMYK RLE-4</summary>
            CmykRle4 = 13
        }

        public enum RenderingHalftoningAlgorithm : int
        {
            /// <summary>No halftoning algorithm</summary>
            None = 0,

            /// <summary>Error Diffusion Halftoning</summary>
            ErrorDiffusion = 1,

            /// <summary>Processing Algorithm for Noncoded Document Acquisition</summary>
            Panda = 2,

            /// <summary>Super-circle Halftoning</summary>
            SuperCircle = 3
        }

        public enum ColorEncoding : int
        {
            Rgb = 0
        }

        public enum ColorSpaceType : long
        {
            /// <summary>0 = Calibrated RGB</summary>
            LcsCalibratedRgb = 0L,

            /// <summary>"sRGB" = sRGB Color Space</summary>
            LcsSRgb = 0x73524742L,

            /// <summary>"Win " = System Default Color Space, sRGB</summary>
            LcsWindowsColorSpace = 0x57696E20L,

            /// <summary>"LINK" = Linked Profile</summary>
            ProfileLinked = 0x4C494E4BL,

            /// <summary>"MBED" = Embedded Profile</summary>
            ProfileEmbedded = 0x4D424544L
        }

        public enum RenderingIntent : long
        {
            /// <summary>Graphic, Saturation</summary>
            LcsGmBusiness = 1,

            /// <summary>Proof, Relative Colorimetric</summary>
            LcsGmGraphics = 2,

            /// <summary>Picture, Perceptual</summary>
            LcsGmImages = 4,

            /// <summary>Match, Absolute Colorimetric</summary>
            LcsGmAbsColorimetric = 8
        }
    }
}
