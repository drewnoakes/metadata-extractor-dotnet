// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MetadataExtractor.IO;

using static MetadataExtractor.Formats.Exif.ExifDirectoryBase;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Base class for several Exif format descriptor classes.</summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract class ExifDescriptorBase<T> : TagDescriptor<T> where T : Directory
    {
        protected ExifDescriptorBase(T directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            // REMOVED
        }
        
        public string? GetYResolutionDescription()
        {
            var resolution = GetRationalOrDoubleString(TagYResolution);
            if (resolution == null)
                return null;
            var unit = GetResolutionDescription();
            return $"{resolution} dots per {unit?.ToLower() ?? "unit"}";
        }

        public string? GetXResolutionDescription()
        {
            var resolution = GetRationalOrDoubleString(TagXResolution);
            if (resolution == null)
                return null;
            var unit = GetResolutionDescription();
            return $"{resolution} dots per {unit?.ToLower() ?? "unit"}";
        }

        /// <summary>
        /// String description of CFA Pattern
        /// </summary>
        /// <remarks>
        /// Converted from Exiftool version 10.33 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\Exif.pm
        ///
        /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.
        /// It does not apply to all sensing methods.
        /// </remarks>
        public string? GetCfaPatternDescription()
        {
            return FormatCfaPattern(DecodeCfaPattern(TagCfaPattern));
        }

        /// <summary>
        /// String description of CFA Pattern
        /// </summary>
        /// <remarks>
        /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.
        /// It does not apply to all sensing methods.
        ///
        /// <see cref="TagCfaPattern2"/> holds only the pixel pattern. <see cref="TagCfaRepeatPatternDim"/> is expected to exist and pass
        /// some conditional tests.
        /// </remarks>
        public string? GetCfaPattern2Description()
        {
            var values = Directory.GetByteArray(TagCfaPattern2);
            if (values == null)
                return null;

            if (Directory.GetObject(TagCfaRepeatPatternDim) is not ushort[] repeatPattern)
                return $"Repeat Pattern not found for CFAPattern ({base.GetDescription(TagCfaPattern2)})";

            if (repeatPattern.Length == 2 && values.Length == (repeatPattern[0] * repeatPattern[1]))
            {
                var intpattern = new int[2 + values.Length];
                intpattern[0] = repeatPattern[0];
                intpattern[1] = repeatPattern[1];

                Array.Copy(values, 0, intpattern, 2, values.Length);

                return FormatCfaPattern(intpattern);
            }

            return $"Unknown Pattern ({base.GetDescription(TagCfaPattern2)})";
        }

        private static string? FormatCfaPattern(int[]? pattern)
        {
            if (pattern == null)
                return null;
            if (pattern.Length < 2)
                return "<truncated data>";
            if (pattern[0] == 0 && pattern[1] == 0)
                return "<zero pattern size>";

            var end = 2 + pattern[0] * pattern[1];
            if (end > pattern.Length)
                return "<invalid pattern size>";

            string[] cfaColors = { "Red", "Green", "Blue", "Cyan", "Magenta", "Yellow", "White" };

            var ret = new StringBuilder();
            ret.Append("[");
            for (var pos = 2; pos < end; pos++)
            {
                if (pattern[pos] <= cfaColors.Length - 1)
                    ret.Append(cfaColors[pattern[pos]]);
                else
                    ret.Append("Unknown");  // indicated pattern position is outside the array bounds

                if ((pos - 2) % pattern[1] == 0)
                    ret.Append(",");
                else if (pos != end - 1)
                    ret.Append("][");
            }
            ret.Append("]");

            return ret.ToString();
        }

        /// <summary>
        /// Decode raw CFAPattern value
        /// </summary>
        /// <remarks>
        /// Converted from Exiftool version 10.33 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\Exif.pm
        ///
        /// The value consists of:
        /// - Two short, being the grid width and height of the repeated pattern.
        /// - Next, for every pixel in that pattern, an identification code.
        /// </remarks>
        private int[]? DecodeCfaPattern(int tagType)
        {
            int[] ret;

            var values = Directory.GetByteArray(tagType);
            if (values == null)
                return null;

            if (values.Length < 4)
            {
                ret = new int[values.Length];
                for (var i = 0; i < values.Length; i++)
                    ret[i] = values[i];
                return ret;
            }

            IndexedReader reader = new ByteArrayReader(values);

            // first two values should be read as 16-bits (2 bytes)
            var item0 = reader.GetInt16(0);
            var item1 = reader.GetInt16(2);

            ret = new int[values.Length - 2];

            var copyArray = false;
            var end = 2 + item0 * item1;
            if (end > values.Length) // sanity check in case of byte order problems; calculated 'end' should be <= length of the values
            {
                // try swapping byte order (I have seen this order different than in EXIF)
                reader = reader.WithByteOrder(!reader.IsMotorolaByteOrder);
                item0 = reader.GetInt16(0);
                item1 = reader.GetInt16(2);

                if (values.Length >= 2 + item0 * item1)
                    copyArray = true;
            }
            else
            {
                copyArray = true;
            }

            if (copyArray)
            {
                ret[0] = item0;
                ret[1] = item1;

                for (var i = 4; i < values.Length; i++)
                    ret[i - 2] = reader.GetByte(i);
            }
            return ret;
        }

        public string? GetFocalPlaneXResolutionDescription()
        {
            if (!Directory.TryGetRational(TagFocalPlaneXResolution, out Rational value))
                return null;
            var unit = GetFocalPlaneResolutionUnitDescription();
            return value.Reciprocal.ToSimpleString() + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        public string? GetFocalPlaneYResolutionDescription()
        {
            if (!Directory.TryGetRational(TagFocalPlaneYResolution, out Rational value))
                return null;
            var unit = GetFocalPlaneResolutionUnitDescription();
            return value.Reciprocal.ToSimpleString() + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        public string? GetFocalPlaneResolutionUnitDescription()
        {
            // Unit of FocalPlaneXResolution/FocalPlaneYResolution.
            // '1' means no-unit, '2' inch, '3' centimeter.
            return GetIndexedDescription(TagFocalPlaneResolutionUnit, 1,
                "(No unit)",
                "Inches",
                "cm");
        }
        
        // public string? GetExifImageWidthDescription()
        // {
        //     if (!Directory.TryGetInt32(TagExifImageWidth, out int value))
        //         return null;
        //     return value + " pixels";
        // }
        //
        // public string? GetExifImageHeightDescription()
        // {
        //     if (!Directory.TryGetInt32(TagExifImageHeight, out int value))
        //         return null;
        //     return value + " pixels";
        // }
        //
        // public string? GetColorSpaceDescription()
        // {
        //     if (!Directory.TryGetInt32(TagColorSpace, out int value))
        //         return null;
        //     if (value == 1)
        //         return "sRGB";
        //     if (value == 65535)
        //         return "Undefined";
        //     return "Unknown (" + value + ")";
        // }
        //
        // public string? GetFocalLengthDescription()
        // {
        //     if (!Directory.TryGetRational(TagFocalLength, out Rational value))
        //         return null;
        //     return GetFocalLengthDescription(value.ToDouble());
        // }
        //
        // public string? GetFlashDescription()
        // {
        //     /*
        //      * This is a bit mask.
        //      * 0 = flash fired
        //      * 1 = return detected
        //      * 2 = return able to be detected
        //      * 3 = unknown
        //      * 4 = auto used
        //      * 5 = unknown
        //      * 6 = red eye reduction used
        //      */
        //     if (!Directory.TryGetInt32(TagFlash, out int value))
        //         return null;
        //
        //     var sb = new StringBuilder();
        //     sb.Append((value & 0x1) != 0 ? "Flash fired" : "Flash did not fire");
        //     // check if we're able to detect a return, before we mention it
        //     if ((value & 0x4) != 0)
        //         sb.Append((value & 0x2) != 0 ? ", return detected" : ", return not detected");
        //     // If 0x10 is set and the lowest byte is not zero - then flash is Auto
        //     if ((value & 0x10) != 0 && (value & 0x0F) != 0)
        //         sb.Append(", auto");
        //     if ((value & 0x40) != 0)
        //         sb.Append(", red-eye reduction");
        //     return sb.ToString();
        // }
        //
        // public string? GetWhiteBalanceDescription()
        // {
        //     if (!Directory.TryGetInt32(TagWhiteBalance, out int value))
        //         return null;
        //
        //     return GetWhiteBalanceDescription(value);
        // }
        //
        // internal static string GetWhiteBalanceDescription(int value)
        // {
        //     // See http://web.archive.org/web/20131018091152/http://exif.org/Exif2-2.PDF page 35
        //
        //     return value switch
        //     {
        //         0 => "Unknown",
        //         1 => "Daylight",
        //         2 => "Florescent",
        //         3 => "Tungsten (Incandescent)",
        //         4 => "Flash",
        //         9 => "Fine Weather",
        //         10 => "Cloudy",
        //         11 => "Shade",
        //         12 => "Daylight Fluorescent",   // (D 5700 - 7100K)
        //         13 => "Day White Fluorescent",  // (N 4600 - 5500K)
        //         14 => "Cool White Fluorescent", // (W 3800 - 4500K)
        //         15 => "White Fluorescent",      // (WW 3250 - 3800K)
        //         16 => "Warm White Fluorescent", // (L 2600 - 3250K)
        //         17 => "Standard light A",
        //         18 => "Standard light B",
        //         19 => "Standard light C",
        //         20 => "D55",
        //         21 => "D65",
        //         22 => "D75",
        //         23 => "D50",
        //         24 => "ISO Studio Tungsten",
        //         255 => "Other",
        //         _ => "Unknown (" + value + ")",
        //     };
        // }
        //
        // public string? GetMeteringModeDescription()
        // {
        //     // '0' means unknown, '1' average, '2' center weighted average, '3' spot
        //     // '4' multi-spot, '5' multi-segment, '6' partial, '255' other
        //     if (!Directory.TryGetInt32(TagMeteringMode, out int value))
        //         return null;
        //
        //     return value switch
        //     {
        //         0 => "Unknown",
        //         1 => "Average",
        //         2 => "Center weighted average",
        //         3 => "Spot",
        //         4 => "Multi-spot",
        //         5 => "Multi-segment",
        //         6 => "Partial",
        //         255 => "(Other)",
        //         _ => "Unknown (" + value + ")",
        //     };
        // }
        //
        // public string? GetCompressionDescription()
        // {
        //     if (!Directory.TryGetInt32(TagCompression, out int value))
        //         return null;
        //
        //     return value switch
        //     {
        //         1 => "Uncompressed",
        //         2 => "CCITT 1D",
        //         3 => "T4/Group 3 Fax",
        //         4 => "T6/Group 4 Fax",
        //         5 => "LZW",
        //         6 => "JPEG (old-style)",
        //         7 => "JPEG",
        //         8 => "Adobe Deflate",
        //         9 => "JBIG B&W",
        //         10 => "JBIG Color",
        //         99 => "JPEG",
        //         262 => "Kodak 262",
        //         32766 => "Next",
        //         32767 => "Sony ARW Compressed",
        //         32769 => "Packed RAW",
        //         32770 => "Samsung SRW Compressed",
        //         32771 => "CCIRLEW",
        //         32772 => "Samsung SRW Compressed 2",
        //         32773 => "PackBits",
        //         32809 => "Thunderscan",
        //         32867 => "Kodak KDC Compressed",
        //         32895 => "IT8CTPAD",
        //         32896 => "IT8LW",
        //         32897 => "IT8MP",
        //         32898 => "IT8BL",
        //         32908 => "PixarFilm",
        //         32909 => "PixarLog",
        //         32946 => "Deflate",
        //         32947 => "DCS",
        //         34661 => "JBIG",
        //         34676 => "SGILog",
        //         34677 => "SGILog24",
        //         34712 => "JPEG 2000",
        //         34713 => "Nikon NEF Compressed",
        //         34715 => "JBIG2 TIFF FX",
        //         34718 => "Microsoft Document Imaging (MDI) Binary Level Codec",
        //         34719 => "Microsoft Document Imaging (MDI) Progressive Transform Codec",
        //         34720 => "Microsoft Document Imaging (MDI) Vector",
        //         34892 => "Lossy JPEG",
        //         65000 => "Kodak DCR Compressed",
        //         65535 => "Pentax PEF Compressed",
        //         _ => "Unknown (" + value + ")",
        //     };
        // }
        //
        // public string? GetSubjectDistanceDescription()
        // {
        //     if (!Directory.TryGetRational(TagSubjectDistance, out Rational value))
        //         return null;
        //     if (value.Numerator == 0xFFFFFFFFL)
        //         return "Infinity";
        //     if (value.Numerator == 0)
        //         return "Unknown";
        //     return $"{value.ToDouble():0.0##} metres";
        // }
        //
        // public string? GetCompressedAverageBitsPerPixelDescription()
        // {
        //     if (!Directory.TryGetRational(TagCompressedAverageBitsPerPixel, out Rational value))
        //         return null;
        //     var ratio = value.ToSimpleString();
        //     return value.IsInteger && value.ToInt32() == 1 ? ratio + " bit/pixel" : ratio + " bits/pixel";
        // }
        //
        // public string? GetExposureTimeDescription()
        // {
        //     var value = Directory.GetString(TagExposureTime);
        //     return value == null ? null : value + " sec";
        // }
        //
        // public string? GetShutterSpeedDescription()
        // {
        //     return GetShutterSpeedDescription(TagShutterSpeed);
        // }
        //
        // public string? GetFNumberDescription()
        // {
        //     if (!Directory.TryGetRational(TagFNumber, out Rational value))
        //         return null;
        //     return GetFStopDescription(value.ToDouble());
        // }
        //
        // public string? GetSensingMethodDescription()
        // {
        //     // '1' Not defined, '2' One-chip color area sensor, '3' Two-chip color area sensor
        //     // '4' Three-chip color area sensor, '5' Color sequential area sensor
        //     // '7' Trilinear sensor '8' Color sequential linear sensor,  'Other' reserved
        //     return GetIndexedDescription(TagSensingMethod, 1,
        //         "(Not defined)",
        //         "One-chip color area sensor",
        //         "Two-chip color area sensor",
        //         "Three-chip color area sensor",
        //         "Color sequential area sensor",
        //         null,
        //         "Trilinear sensor",
        //         "Color sequential linear sensor");
        // }
        //
        // public string? GetComponentConfigurationDescription()
        // {
        //     var components = Directory.GetInt32Array(TagComponentsConfiguration);
        //     if (components == null)
        //         return null;
        //     var componentStrings = new[] { string.Empty, "Y", "Cb", "Cr", "R", "G", "B" };
        //     var componentConfig = new StringBuilder();
        //     for (var i = 0; i < Math.Min(4, components.Length); i++)
        //     {
        //         var j = components[i];
        //         if (j > 0 && j < componentStrings.Length)
        //             componentConfig.Append(componentStrings[j]);
        //     }
        //     return componentConfig.ToString();
        // }
        //
        // public string? GetJpegProcDescription()
        // {
        //     if (!Directory.TryGetInt32(TagJpegProc, out int value))
        //         return null;
        //
        //     return value switch
        //     {
        //         1 => "Baseline",
        //         14 => "Lossless",
        //         _ => "Unknown (" + value + ")",
        //     };
        // }
        //
        // public string? GetExtraSamplesDescription()
        // {
        //     return GetIndexedDescription(
        //         TagExtraSamples,
        //         "Unspecified",
        //         "Associated alpha",
        //         "Unassociated alpha");
        // }
        //
        // public string? GetSampleFormatDescription()
        // {
        //     var values = Directory.GetInt32Array(TagSampleFormat);
        //
        //     if (values is null)
        //         return null;
        //
        //     var sb = new StringBuilder();
        //
        //     foreach (var value in values)
        //     {
        //         if (sb.Length != 0)
        //             sb.Append(", ");
        //
        //         sb.Append(value switch
        //         {
        //             1 => "Unsigned",
        //             2 => "Signed",
        //             3 => "Float",
        //             4 => "Undefined",
        //             5 => "Complex int",
        //             6 => "Complex float",
        //             _ => $"Unknown ({value})"
        //         });
        //     }
        //
        //     return sb.ToString();
        // }
    }
}
