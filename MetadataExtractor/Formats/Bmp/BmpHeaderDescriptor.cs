#region License
//
// Copyright 2002-2019 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class BmpHeaderDescriptor : TagDescriptor<BmpHeaderDirectory>
    {
        public BmpHeaderDescriptor([NotNull] BmpHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
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

        [CanBeNull]
        public string GetBitmapTypeDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagBitmapType, out int value))
                return null;

            switch (value)
            {
                case (int)BmpHeaderDirectory.BitmapType.Bitmap:
                    return "Standard";
                case (int)BmpHeaderDirectory.BitmapType.OS2BitmapArray:
                    return "Bitmap Array";
                case (int)BmpHeaderDirectory.BitmapType.OS2ColorIcon:
                    return "Color Icon";
                case (int)BmpHeaderDirectory.BitmapType.OS2ColorPointer:
                    return "Color Pointer";
                case (int)BmpHeaderDirectory.BitmapType.OS2Icon:
                    return "Monochrome Icon";
                case (int)BmpHeaderDirectory.BitmapType.OS2Pointer:
                    return "Monochrome Pointer";
                default:
                    return "Unimplemented bitmap type " + value.ToString();
            }
        }

        [CanBeNull]
        public string GetCompressionDescription()
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

            switch (value)
            {
                case (int)BmpHeaderDirectory.Compression.Rgb:
                    return "None";
                case (int)BmpHeaderDirectory.Compression.Rle8:
                    return "RLE 8-bit/pixel";
                case (int)BmpHeaderDirectory.Compression.Rle4:
                    return "RLE 4-bit/pixel";
                case (int)BmpHeaderDirectory.Compression.BitFields:
                    return headerSize == 64 ? "Huffman 1D" : "Bit Fields";
                case (int)BmpHeaderDirectory.Compression.Jpeg:
                    return headerSize == 64 ? "RLE 24-bit/pixel" : "JPEG";
                case (int)BmpHeaderDirectory.Compression.Png:
                    return "PNG";
                case (int)BmpHeaderDirectory.Compression.AlphaBitFields:
                    return "RGBA Bit Fields";
                case (int)BmpHeaderDirectory.Compression.Cmyk:
                    return "CMYK Uncompressed";
                case (int)BmpHeaderDirectory.Compression.CmykRle8:
                    return "CMYK RLE-8";
                case (int)BmpHeaderDirectory.Compression.CmykRle4:
                    return "CMYK RLE-4";
                default:
                    return "Unimplemented compression type " + value.ToString();
            }
        }

        [CanBeNull]
        public string GetRenderingDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagRendering, out int value))
                return null;

            switch (value)
            {
                case (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.None:
                    return "No Halftoning Algorithm";
                case (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.ErrorDiffusion:
                    return "Error Diffusion Halftoning";
                case (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.Panda:
                    return "Processing Algorithm for Noncoded Document Acquisition";
                case (int)BmpHeaderDirectory.RenderingHalftoningAlgorithm.SuperCircle:
                    return "Super-circle Halftoning";
                default:
                    return "Unimplemented rendering halftoning algorithm type " + value.ToString();
            }
        }

        [CanBeNull]
        public string GetColorEncodingDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagColorEncoding, out int value))
                return null;

            switch (value)
            {
                case (int)BmpHeaderDirectory.ColorEncoding.Rgb:
                    return "RGB";
                default:
                    return "Unimplemented color encoding type " + value.ToString();
            }
        }

        [CanBeNull]
        public string GetColorSpaceTypeDescription()
        {
            if (!Directory.TryGetInt64(BmpHeaderDirectory.TagColorSpaceType, out long value))
                return null;

            switch (value)
            {
                case (long)BmpHeaderDirectory.ColorSpaceType.LcsCalibratedRgb:
                    return "Calibrated RGB";
                case (long)BmpHeaderDirectory.ColorSpaceType.LcsSRgb:
                    return "sRGB Color Space";
                case (long)BmpHeaderDirectory.ColorSpaceType.LcsWindowsColorSpace:
                    return "System Default Color Space, sRGB";
                case (long)BmpHeaderDirectory.ColorSpaceType.ProfileLinked:
                    return "Linked Profile";
                case (long)BmpHeaderDirectory.ColorSpaceType.ProfileEmbedded:
                    return "Embedded Profile";
                default:
                    return "Unimplemented color space type " + value.ToString();
            }
        }

        [CanBeNull]
        public string GetRenderingIntentDescription()
        {
            if (!Directory.TryGetInt64(BmpHeaderDirectory.TagIntent, out long value))
                return null;

            switch (value)
            {
                case (long)BmpHeaderDirectory.RenderingIntent.LcsGmBusiness:
                    return "Graphic, Saturation";
                case (long)BmpHeaderDirectory.RenderingIntent.LcsGmGraphics:
                    return "Proof, Relative Colorimetric";
                case (long)BmpHeaderDirectory.RenderingIntent.LcsGmImages:
                    return "Picture, Perceptual";
                case (long)BmpHeaderDirectory.RenderingIntent.LcsGmAbsColorimetric:
                    return "Match, Absolute Colorimetric";
                default:
                    return "Unimplemented rendering intent type " + value.ToString();
            }
        }
    }
}
