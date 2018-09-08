#region License
//
// Copyright 2002-2017 Drew Noakes
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
                    return formatHex(Directory.GetInt64(tagType), 8);
                case BmpHeaderDirectory.TagColorSpaceType:
                    return GetColorSpaceTypeDescription();
                case BmpHeaderDirectory.TagGammaRed:
                case BmpHeaderDirectory.TagGammaGreen:
                case BmpHeaderDirectory.TagGammaBlue:
                    return formatFixed1616(Directory.GetInt64(tagType));
                case BmpHeaderDirectory.TagIntent:
                    return GetRenderingIntentDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        private static string formatHex(long value, int digits)
        {
            return value.ToString("X" + digits.ToString());
        }

        public static string formatFixed1616(long value)
        {
            double d = (double)value / 0x10000;
            return $"{d:0.###}";
        }

        public string GetBitmapTypeDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagBitmapType, out int value))
                return null;

            switch (value)
            {
                case (int)BitmapType.Bitmap:
                    return "Standard";
                case (int)BitmapType.OS2BitmapArray:
                    return "Bitmap Array";
                case (int)BitmapType.OS2ColorIcon:
                    return "Color Icon";
                case (int)BitmapType.OS2ColorPointer:
                    return "Color Pointer";
                case (int)BitmapType.OS2Icon:
                    return "Monochrome Icon";
                case (int)BitmapType.OS2Pointer:
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
            // 3 = Bit field (or Huffman 1D if BITMAPCOREHEADER2 (size 64))
            // 4 = JPEG (or RLE-24 if BITMAPCOREHEADER2 (size 64))
            // 5 = PNG
            // 6 = Bit field

            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagCompression, out int value) ||
                !Directory.TryGetInt32(BmpHeaderDirectory.TagHeaderSize, out int headerSize))
                return null;

            switch (value)
            {
                case 0:
                    return "None";
                case 1:
                    return "RLE 8-bit/pixel";
                case 2:
                    return "RLE 4-bit/pixel";
                case 3:
                    return headerSize == 64 ? "Bit field" : "Huffman 1D";
                case 4:
                    return headerSize == 64 ? "JPEG" : "RLE-24";
                case 5:
                    return "PNG";
                case 6:
                    return "Bit field";
            }

            return base.GetDescription(BmpHeaderDirectory.TagCompression);
        }

        public string GetRenderingDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagRendering, out int value))
                return null;

            switch (value)
            {
                case (int)RenderingHalftoningAlgorithm.None:
                    return "No Halftoning Algorithm";
                case (int)RenderingHalftoningAlgorithm.ErrorDiffusion:
                    return "Error Diffusion Halftoning";
                case (int)RenderingHalftoningAlgorithm.Panda:
                    return "Processing Algorithm for Noncoded Document Acquisition";
                case (int)RenderingHalftoningAlgorithm.SuperCircle:
                    return "Super-circle Halftoning";
                default:
                    return "Unimplemented rendering halftoning algorithm type " + value.ToString();
            }
        }

        public string GetColorEncodingDescription()
        {
            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagColorEncoding, out int value))
                return null;

            switch (value)
            {
                case (int)ColorEncoding.Rgb:
                    return "RGB";
                default:
                    return "Unimplemented color encoding type " + value.ToString();
            }
        }

        public string GetColorSpaceTypeDescription()
        {
            if (!Directory.TryGetInt64(BmpHeaderDirectory.TagColorSpaceType, out long value))
                return null;

            switch (value)
            {
                case (long)ColorSpaceType.LcsCalibratedRgb:
                    return "Calibrated RGB";
                case (long)ColorSpaceType.LcsSRgb:
                    return "sRGB Color Space";
                case (long)ColorSpaceType.LcsWindowsColorSpace:
                    return "System Default Color Space, sRGB";
                case (long)ColorSpaceType.ProfileLinked:
                    return "Linked Profile";
                case (long)ColorSpaceType.ProfileEmbedded:
                    return "Embedded Profile";
                default:
                    return "Unimplemented color space type " + value.ToString();
            }
        }

        public string GetRenderingIntentDescription()
        {
            if (!Directory.TryGetInt64(BmpHeaderDirectory.TagRendering, out long value))
                return null;

            switch (value)
            {
                case (long)RenderingIntent.LcsGmBusiness:
                    return "Graphic, Saturation";
                case (long)RenderingIntent.LcsGmGraphics:
                    return "Proof, Relative Colorimetric";
                case (long)RenderingIntent.LcsGmImages:
                    return "Picture, Perceptual";
                case (long)RenderingIntent.LcsGmAbsColorimetric:
                    return "Match, Absolute Colorimetric";
                default:
                    return "Unimplemented rendering intent type " + value.ToString();
            }
        }
    }
}
