// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MetadataExtractor.Util;
using MetadataExtractor.IO;

using static MetadataExtractor.Formats.Exif.ExifDirectoryBase;

// ReSharper disable MemberCanBePrivate.Global

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Base class for several Exif format descriptor classes.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract class ExifDescriptorBase<T> : TagDescriptor<T> where T : Directory
    {
        protected ExifDescriptorBase(T directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            // TODO order case blocks and corresponding methods in the same order as the TAG_* values are defined

            return tagType switch
            {
                TagInteropIndex                  => GetInteropIndexDescription(),
                TagInteropVersion                => GetInteropVersionDescription(),
                TagOrientation                   => GetOrientationDescription(),
                TagResolutionUnit                => GetResolutionDescription(),
                TagYCbCrPositioning              => GetYCbCrPositioningDescription(),
                TagXResolution                   => GetXResolutionDescription(),
                TagYResolution                   => GetYResolutionDescription(),
                TagImageWidth                    => GetImageWidthDescription(),
                TagImageHeight                   => GetImageHeightDescription(),
                TagBitsPerSample                 => GetBitsPerSampleDescription(),
                TagPhotometricInterpretation     => GetPhotometricInterpretationDescription(),
                TagRowsPerStrip                  => GetRowsPerStripDescription(),
                TagStripByteCounts               => GetStripByteCountsDescription(),
                TagSamplesPerPixel               => GetSamplesPerPixelDescription(),
                TagPlanarConfiguration           => GetPlanarConfigurationDescription(),
                TagYCbCrSubsampling              => GetYCbCrSubsamplingDescription(),
                TagReferenceBlackWhite           => GetReferenceBlackWhiteDescription(),
                TagWinAuthor                     => GetWindowsAuthorDescription(),
                TagWinComment                    => GetWindowsCommentDescription(),
                TagWinKeywords                   => GetWindowsKeywordsDescription(),
                TagWinSubject                    => GetWindowsSubjectDescription(),
                TagWinTitle                      => GetWindowsTitleDescription(),
                TagNewSubfileType                => GetNewSubfileTypeDescription(),
                TagSubfileType                   => GetSubfileTypeDescription(),
                TagThresholding                  => GetThresholdingDescription(),
                TagFillOrder                     => GetFillOrderDescription(),
                TagCfaPattern2                   => GetCfaPattern2Description(),
                TagExposureTime                  => GetExposureTimeDescription(),
                TagShutterSpeed                  => GetShutterSpeedDescription(),
                TagFNumber                       => GetFNumberDescription(),
                TagCompressedAverageBitsPerPixel => GetCompressedAverageBitsPerPixelDescription(),
                TagSubjectDistance               => GetSubjectDistanceDescription(),
                TagMeteringMode                  => GetMeteringModeDescription(),
                TagWhiteBalance                  => GetWhiteBalanceDescription(),
                TagFlash                         => GetFlashDescription(),
                TagFocalLength                   => GetFocalLengthDescription(),
                TagColorSpace                    => GetColorSpaceDescription(),
                TagExifImageWidth                => GetExifImageWidthDescription(),
                TagExifImageHeight               => GetExifImageHeightDescription(),
                TagFocalPlaneResolutionUnit      => GetFocalPlaneResolutionUnitDescription(),
                TagFocalPlaneXResolution         => GetFocalPlaneXResolutionDescription(),
                TagFocalPlaneYResolution         => GetFocalPlaneYResolutionDescription(),
                TagExposureProgram               => GetExposureProgramDescription(),
                TagAperture                      => GetApertureValueDescription(),
                TagBrightnessValue               => GetBrightnessValueDescription(),
                TagMaxAperture                   => GetMaxApertureValueDescription(),
                TagSensingMethod                 => GetSensingMethodDescription(),
                TagExposureBias                  => GetExposureBiasDescription(),
                TagFileSource                    => GetFileSourceDescription(),
                TagSceneType                     => GetSceneTypeDescription(),
                TagCfaPattern                    => GetCfaPatternDescription(),
                TagComponentsConfiguration       => GetComponentConfigurationDescription(),
                TagExifVersion                   => GetExifVersionDescription(),
                TagFlashpixVersion               => GetFlashPixVersionDescription(),
                TagIsoEquivalent                 => GetIsoEquivalentDescription(),
                TagUserComment                   => GetUserCommentDescription(),
                TagCustomRendered                => GetCustomRenderedDescription(),
                TagExposureMode                  => GetExposureModeDescription(),
                TagWhiteBalanceMode              => GetWhiteBalanceModeDescription(),
                TagDigitalZoomRatio              => GetDigitalZoomRatioDescription(),
                Tag35MMFilmEquivFocalLength      => Get35MMFilmEquivFocalLengthDescription(),
                TagSceneCaptureType              => GetSceneCaptureTypeDescription(),
                TagGainControl                   => GetGainControlDescription(),
                TagContrast                      => GetContrastDescription(),
                TagSaturation                    => GetSaturationDescription(),
                TagSharpness                     => GetSharpnessDescription(),
                TagSubjectDistanceRange          => GetSubjectDistanceRangeDescription(),
                TagSensitivityType               => GetSensitivityTypeDescription(),
                TagCompression                   => GetCompressionDescription(),
                TagJpegProc                      => GetJpegProcDescription(),
                TagLensSpecification             => GetLensSpecificationDescription(),
                _                                => base.GetDescription(tagType),
            };
        }

        public string? GetInteropVersionDescription()
        {
            return GetVersionBytesDescription(TagInteropVersion, 2);
        }

        public string? GetInteropIndexDescription()
        {
            var value = Directory.GetString(TagInteropIndex);
            if (value == null)
                return null;
            return string.Equals("R98", value.Trim(), StringComparison.OrdinalIgnoreCase)
                ? "Recommended Exif Interoperability Rules (ExifR98)"
                : "Unknown (" + value + ")";
        }

        public string? GetReferenceBlackWhiteDescription()
        {
            var ints = Directory.GetInt32Array(TagReferenceBlackWhite);
            if (ints == null || ints.Length < 6)
                return null;
            var blackR = ints[0];
            var whiteR = ints[1];
            var blackG = ints[2];
            var whiteG = ints[3];
            var blackB = ints[4];
            var whiteB = ints[5];
            return $"[{blackR},{blackG},{blackB}] [{whiteR},{whiteG},{whiteB}]";
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

        public string? GetYCbCrPositioningDescription()
        {
            return GetIndexedDescription(TagYCbCrPositioning, 1,
                "Center of pixel array",
                "Datum point");
        }

        public string? GetOrientationDescription()
        {
            return base.GetOrientationDescription(TagOrientation);
        }

        public string? GetResolutionDescription()
        {
            // '1' means no-unit, '2' means inch, '3' means centimeter. Default value is '2'(inch)
            return GetIndexedDescription(TagResolutionUnit, 1,
                "(No unit)",
                "Inch",
                "cm");
        }

        /// <summary>The Windows specific tags uses plain Unicode.</summary>
        private string? GetUnicodeDescription(int tag)
        {
            var bytes = Directory.GetByteArray(tag);
            if (bytes == null)
                return null;
            try
            {
                // Decode the Unicode string and trim the Unicode zero "\0" from the end.
                return Encoding.Unicode.GetString(bytes, 0, bytes.Length).TrimEnd('\0');
            }
            catch
            {
                return null;
            }
        }

        public string? GetWindowsAuthorDescription()
        {
            return GetUnicodeDescription(TagWinAuthor);
        }

        public string? GetWindowsCommentDescription()
        {
            return GetUnicodeDescription(TagWinComment);
        }

        public string? GetWindowsKeywordsDescription()
        {
            return GetUnicodeDescription(TagWinKeywords);
        }

        public string? GetWindowsTitleDescription()
        {
            return GetUnicodeDescription(TagWinTitle);
        }

        public string? GetWindowsSubjectDescription()
        {
            return GetUnicodeDescription(TagWinSubject);
        }

        public string? GetYCbCrSubsamplingDescription()
        {
            var positions = Directory.GetInt32Array(TagYCbCrSubsampling);
            if (positions == null || positions.Length < 2)
                return null;
            if (positions[0] == 2 && positions[1] == 1)
                return "YCbCr4:2:2";
            if (positions[0] == 2 && positions[1] == 2)
                return "YCbCr4:2:0";
            return "(Unknown)";
        }

        public string? GetPlanarConfigurationDescription()
        {
            // When image format is no compression YCbCr, this value shows byte aligns of YCbCr
            // data. If value is '1', Y/Cb/Cr value is chunky format, contiguous for each subsampling
            // pixel. If value is '2', Y/Cb/Cr value is separated and stored to Y plane/Cb plane/Cr
            // plane format.
            return GetIndexedDescription(TagPlanarConfiguration, 1, "Chunky (contiguous for each subsampling pixel)", "Separate (Y-plane/Cb-plane/Cr-plane format)");
        }

        public string? GetSamplesPerPixelDescription()
        {
            var value = Directory.GetString(TagSamplesPerPixel);
            return value == null ? null : value + " samples/pixel";
        }

        public string? GetRowsPerStripDescription()
        {
            var value = Directory.GetString(TagRowsPerStrip);
            return value == null ? null : value + " rows/strip";
        }

        public string? GetStripByteCountsDescription()
        {
            var value = Directory.GetString(TagStripByteCounts);
            return value == null ? null : value + " bytes";
        }

        public string? GetPhotometricInterpretationDescription()
        {
            // Shows the color space of the image data components
            if (!Directory.TryGetInt32(TagPhotometricInterpretation, out int value))
                return null;

            return value switch
            {
                0 => "WhiteIsZero",
                1 => "BlackIsZero",
                2 => "RGB",
                3 => "RGB Palette",
                4 => "Transparency Mask",
                5 => "CMYK",
                6 => "YCbCr",
                8 => "CIELab",
                9 => "ICCLab",
                10 => "ITULab",
                32803 => "Color Filter Array",
                32844 => "Pixar LogL",
                32845 => "Pixar LogLuv",
                32892 => "Linear Raw",
                _ => "Unknown colour space",
            };
        }

        public string? GetBitsPerSampleDescription()
        {
            var value = Directory.GetString(TagBitsPerSample);
            return value == null ? null : value + " bits/component/pixel";
        }

        public string? GetImageWidthDescription()
        {
            var value = Directory.GetString(TagImageWidth);
            return value == null ? null : value + " pixels";
        }

        public string? GetImageHeightDescription()
        {
            var value = Directory.GetString(TagImageHeight);
            return value == null ? null : value + " pixels";
        }

        public string? GetNewSubfileTypeDescription()
        {
            return GetIndexedDescription(TagNewSubfileType, 0,
                "Full-resolution image",
                "Reduced-resolution image",
                "Single page of multi-page image",
                "Single page of multi-page reduced-resolution image",
                "Transparency mask",
                "Transparency mask of reduced-resolution image",
                "Transparency mask of multi-page image",
                "Transparency mask of reduced-resolution multi-page image");
        }

        public string? GetSubfileTypeDescription()
        {
            return GetIndexedDescription(TagSubfileType, 1,
                "Full-resolution image",
                "Reduced-resolution image",
                "Single page of multi-page image");
        }

        public string? GetThresholdingDescription()
        {
            return GetIndexedDescription(TagThresholding, 1,
                "No dithering or halftoning",
                "Ordered dither or halftone",
                "Randomized dither");
        }

        public string? GetFillOrderDescription()
        {
            return GetIndexedDescription(TagFillOrder, 1,
                "Normal",
                "Reversed");
        }

        public string? GetSubjectDistanceRangeDescription()
        {
            return GetIndexedDescription(TagSubjectDistanceRange,
                "Unknown",
                "Macro",
                "Close view",
                "Distant view");
        }

        public string? GetSensitivityTypeDescription()
        {
            return GetIndexedDescription(TagSensitivityType,
                "Unknown",
                "Standard Output Sensitivity",
                "Recommended Exposure Index",
                "ISO Speed",
                "Standard Output Sensitivity and Recommended Exposure Index",
                "Standard Output Sensitivity and ISO Speed",
                "Recommended Exposure Index and ISO Speed",
                "Standard Output Sensitivity, Recommended Exposure Index and ISO Speed");
        }

        public string? GetLensSpecificationDescription()
        {
            return GetLensSpecificationDescription(TagLensSpecification);
        }

        public string? GetSharpnessDescription()
        {
            return GetIndexedDescription(TagSharpness,
                "None",
                "Low",
                "Hard");
        }

        public string? GetSaturationDescription()
        {
            return GetIndexedDescription(TagSaturation,
                "None",
                "Low saturation",
                "High saturation");
        }

        public string? GetContrastDescription()
        {
            return GetIndexedDescription(TagContrast,
                "None",
                "Soft",
                "Hard");
        }

        public string? GetGainControlDescription()
        {
            return GetIndexedDescription(TagGainControl,
                "None",
                "Low gain up",
                "Low gain down",
                "High gain up",
                "High gain down");
        }

        public string? GetSceneCaptureTypeDescription()
        {
            return GetIndexedDescription(TagSceneCaptureType,
                "Standard",
                "Landscape",
                "Portrait",
                "Night scene");
        }

        public string? Get35MMFilmEquivFocalLengthDescription()
        {
            if (!Directory.TryGetInt32(Tag35MMFilmEquivFocalLength, out int value))
                return null;
            return value == 0 ? "Unknown" : GetFocalLengthDescription(value);
        }

        public string? GetDigitalZoomRatioDescription()
        {
            if (!Directory.TryGetRational(TagDigitalZoomRatio, out Rational value))
                return null;
            return value.Numerator == 0
                ? "Digital zoom not used"
                : value.ToDouble().ToString("0.#");
        }

        public string? GetWhiteBalanceModeDescription()
        {
            return GetIndexedDescription(TagWhiteBalanceMode,
                "Auto white balance",
                "Manual white balance");
        }

        public string? GetExposureModeDescription()
        {
            return GetIndexedDescription(TagExposureMode,
                "Auto exposure",
                "Manual exposure",
                "Auto bracket");
        }

        public string? GetCustomRenderedDescription()
        {
            return GetIndexedDescription(TagCustomRendered,
                "Normal process",
                "Custom process");
        }

        public string? GetUserCommentDescription()
        {
            return GetEncodedTextDescription(TagUserComment);
        }

        public string? GetIsoEquivalentDescription()
        {
            // Have seen an exception here from files produced by ACDSEE that stored an int[] here with two values
            // There used to be a check here that multiplied ISO values < 50 by 200.
            // Issue 36 shows a smart-phone image from a Samsung Galaxy S2 with ISO-40.
            if (!Directory.TryGetInt32(TagIsoEquivalent, out int value))
                return null;
            return value.ToString();
        }

        public string? GetExifVersionDescription()
        {
            return GetVersionBytesDescription(TagExifVersion, 2);
        }

        public string? GetFlashPixVersionDescription()
        {
            return GetVersionBytesDescription(TagFlashpixVersion, 2);
        }

        public string? GetSceneTypeDescription()
        {
            return GetIndexedDescription(TagSceneType, 1,
                "Directly photographed image");
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
            return FormatCFAPattern(DecodeCFAPattern(TagCfaPattern));
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

            if (!(Directory.GetObject(TagCfaRepeatPatternDim) is ushort[] repeatPattern))
                return $"Repeat Pattern not found for CFAPattern ({base.GetDescription(TagCfaPattern2)})";

            if (repeatPattern.Length == 2 && values.Length == (repeatPattern[0] * repeatPattern[1]))
            {
                var intpattern = new int[2 + values.Length];
                intpattern[0] = repeatPattern[0];
                intpattern[1] = repeatPattern[1];

                Array.Copy(values, 0, intpattern, 2, values.Length);

                return FormatCFAPattern(intpattern);
            }

            return $"Unknown Pattern ({base.GetDescription(TagCfaPattern2)})";
        }

        private static string? FormatCFAPattern(int[]? pattern)
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
        private int[]? DecodeCFAPattern(int tagType)
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

        public string? GetFileSourceDescription()
        {
            return GetIndexedDescription(TagFileSource, 1,
                "Film Scanner",
                "Reflection Print Scanner",
                "Digital Still Camera (DSC)");
        }

        public string? GetExposureBiasDescription()
        {
            if (!Directory.TryGetRational(TagExposureBias, out Rational value))
                return null;
            return value.ToSimpleString() + " EV";
        }

        public string? GetMaxApertureValueDescription()
        {
            if (!Directory.TryGetDouble(TagMaxAperture, out double aperture))
                return null;
            return GetFStopDescription(PhotographicConversions.ApertureToFStop(aperture));
        }

        public string? GetApertureValueDescription()
        {
            if (!Directory.TryGetDouble(TagAperture, out double aperture))
                return null;
            return GetFStopDescription(PhotographicConversions.ApertureToFStop(aperture));
        }

        public string? GetBrightnessValueDescription()
        {
            if (!Directory.TryGetRational(TagBrightnessValue, out Rational value))
                return null;
            if (value.Numerator == 0xFFFFFFFFL)
                return "Unknown";

            return $"{value.ToDouble():0.0##}";
        }

        public string? GetExposureProgramDescription()
        {
            return GetIndexedDescription(TagExposureProgram, 1,
                "Manual control",
                "Program normal",
                "Aperture priority",
                "Shutter priority",
                "Program creative (slow program)",
                "Program action (high-speed program)",
                "Portrait mode",
                "Landscape mode");
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

        public string? GetExifImageWidthDescription()
        {
            if (!Directory.TryGetInt32(TagExifImageWidth, out int value))
                return null;
            return value + " pixels";
        }

        public string? GetExifImageHeightDescription()
        {
            if (!Directory.TryGetInt32(TagExifImageHeight, out int value))
                return null;
            return value + " pixels";
        }

        public string? GetColorSpaceDescription()
        {
            if (!Directory.TryGetInt32(TagColorSpace, out int value))
                return null;
            if (value == 1)
                return "sRGB";
            if (value == 65535)
                return "Undefined";
            return "Unknown (" + value + ")";
        }

        public string? GetFocalLengthDescription()
        {
            if (!Directory.TryGetRational(TagFocalLength, out Rational value))
                return null;
            return GetFocalLengthDescription(value.ToDouble());
        }

        public string? GetFlashDescription()
        {
            /*
             * This is a bit mask.
             * 0 = flash fired
             * 1 = return detected
             * 2 = return able to be detected
             * 3 = unknown
             * 4 = auto used
             * 5 = unknown
             * 6 = red eye reduction used
             */
            if (!Directory.TryGetInt32(TagFlash, out int value))
                return null;

            var sb = new StringBuilder();
            sb.Append((value & 0x1) != 0 ? "Flash fired" : "Flash did not fire");
            // check if we're able to detect a return, before we mention it
            if ((value & 0x4) != 0)
                sb.Append((value & 0x2) != 0 ? ", return detected" : ", return not detected");
            // If 0x10 is set and the lowest byte is not zero - then flash is Auto
            if ((value & 0x10) != 0 && (value & 0x0F) != 0)
                sb.Append(", auto");
            if ((value & 0x40) != 0)
                sb.Append(", red-eye reduction");
            return sb.ToString();
        }

        public string? GetWhiteBalanceDescription()
        {
            if (!Directory.TryGetInt32(TagWhiteBalance, out int value))
                return null;

            return GetWhiteBalanceDescription(value);
        }

        internal static string GetWhiteBalanceDescription(int value)
        {
            // See http://web.archive.org/web/20131018091152/http://exif.org/Exif2-2.PDF page 35

            return value switch
            {
                0 => "Unknown",
                1 => "Daylight",
                2 => "Florescent",
                3 => "Tungsten (Incandescent)",
                4 => "Flash",
                9 => "Fine Weather",
                10 => "Cloudy",
                11 => "Shade",
                12 => "Daylight Fluorescent",   // (D 5700 - 7100K)
                13 => "Day White Fluorescent",  // (N 4600 - 5500K)
                14 => "Cool White Fluorescent", // (W 3800 - 4500K)
                15 => "White Fluorescent",      // (WW 3250 - 3800K)
                16 => "Warm White Fluorescent", // (L 2600 - 3250K)
                17 => "Standard light A",
                18 => "Standard light B",
                19 => "Standard light C",
                20 => "D55",
                21 => "D65",
                22 => "D75",
                23 => "D50",
                24 => "ISO Studio Tungsten",
                255 => "Other",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetMeteringModeDescription()
        {
            // '0' means unknown, '1' average, '2' center weighted average, '3' spot
            // '4' multi-spot, '5' multi-segment, '6' partial, '255' other
            if (!Directory.TryGetInt32(TagMeteringMode, out int value))
                return null;

            return value switch
            {
                0 => "Unknown",
                1 => "Average",
                2 => "Center weighted average",
                3 => "Spot",
                4 => "Multi-spot",
                5 => "Multi-segment",
                6 => "Partial",
                255 => "(Other)",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetCompressionDescription()
        {
            if (!Directory.TryGetInt32(TagCompression, out int value))
                return null;

            return value switch
            {
                1 => "Uncompressed",
                2 => "CCITT 1D",
                3 => "T4/Group 3 Fax",
                4 => "T6/Group 4 Fax",
                5 => "LZW",
                6 => "JPEG (old-style)",
                7 => "JPEG",
                8 => "Adobe Deflate",
                9 => "JBIG B&W",
                10 => "JBIG Color",
                99 => "JPEG",
                262 => "Kodak 262",
                32766 => "Next",
                32767 => "Sony ARW Compressed",
                32769 => "Packed RAW",
                32770 => "Samsung SRW Compressed",
                32771 => "CCIRLEW",
                32772 => "Samsung SRW Compressed 2",
                32773 => "PackBits",
                32809 => "Thunderscan",
                32867 => "Kodak KDC Compressed",
                32895 => "IT8CTPAD",
                32896 => "IT8LW",
                32897 => "IT8MP",
                32898 => "IT8BL",
                32908 => "PixarFilm",
                32909 => "PixarLog",
                32946 => "Deflate",
                32947 => "DCS",
                34661 => "JBIG",
                34676 => "SGILog",
                34677 => "SGILog24",
                34712 => "JPEG 2000",
                34713 => "Nikon NEF Compressed",
                34715 => "JBIG2 TIFF FX",
                34718 => "Microsoft Document Imaging (MDI) Binary Level Codec",
                34719 => "Microsoft Document Imaging (MDI) Progressive Transform Codec",
                34720 => "Microsoft Document Imaging (MDI) Vector",
                34892 => "Lossy JPEG",
                65000 => "Kodak DCR Compressed",
                65535 => "Pentax PEF Compressed",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetSubjectDistanceDescription()
        {
            if (!Directory.TryGetRational(TagSubjectDistance, out Rational value))
                return null;
            if (value.Numerator == 0xFFFFFFFFL)
                return "Infinity";
            if (value.Numerator == 0)
                return "Unknown";
            return $"{value.ToDouble():0.0##} metres";
        }

        public string? GetCompressedAverageBitsPerPixelDescription()
        {
            if (!Directory.TryGetRational(TagCompressedAverageBitsPerPixel, out Rational value))
                return null;
            var ratio = value.ToSimpleString();
            return value.IsInteger && value.ToInt32() == 1 ? ratio + " bit/pixel" : ratio + " bits/pixel";
        }

        public string? GetExposureTimeDescription()
        {
            var value = Directory.GetString(TagExposureTime);
            return value == null ? null : value + " sec";
        }

        public string? GetShutterSpeedDescription()
        {
            return GetShutterSpeedDescription(TagShutterSpeed);
        }

        public string? GetFNumberDescription()
        {
            if (!Directory.TryGetRational(TagFNumber, out Rational value))
                return null;
            return GetFStopDescription(value.ToDouble());
        }

        public string? GetSensingMethodDescription()
        {
            // '1' Not defined, '2' One-chip color area sensor, '3' Two-chip color area sensor
            // '4' Three-chip color area sensor, '5' Color sequential area sensor
            // '7' Trilinear sensor '8' Color sequential linear sensor,  'Other' reserved
            return GetIndexedDescription(TagSensingMethod, 1,
                "(Not defined)",
                "One-chip color area sensor",
                "Two-chip color area sensor",
                "Three-chip color area sensor",
                "Color sequential area sensor",
                null,
                "Trilinear sensor",
                "Color sequential linear sensor");
        }

        public string? GetComponentConfigurationDescription()
        {
            var components = Directory.GetInt32Array(TagComponentsConfiguration);
            if (components == null)
                return null;
            var componentStrings = new[] { string.Empty, "Y", "Cb", "Cr", "R", "G", "B" };
            var componentConfig = new StringBuilder();
            for (var i = 0; i < Math.Min(4, components.Length); i++)
            {
                var j = components[i];
                if (j > 0 && j < componentStrings.Length)
                    componentConfig.Append(componentStrings[j]);
            }
            return componentConfig.ToString();
        }

        public string? GetJpegProcDescription()
        {
            if (!Directory.TryGetInt32(TagJpegProc, out int value))
                return null;

            return value switch
            {
                1 => "Baseline",
                14 => "Lossless",
                _ => "Unknown (" + value + ")",
            };
        }
    }
}
