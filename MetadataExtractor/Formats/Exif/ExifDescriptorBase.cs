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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Util;
using MetadataExtractor.IO;

// ReSharper disable MemberCanBePrivate.Global

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Base class for several Exif format descriptor classes.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract class ExifDescriptorBase<T> : TagDescriptor<T> where T : Directory
    {
        protected ExifDescriptorBase([NotNull] T directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            // TODO order case blocks and corresponding methods in the same order as the TAG_* values are defined

            switch (tagType)
            {
                case ExifDirectoryBase.TagInteropIndex:
                    return GetInteropIndexDescription();
                case ExifDirectoryBase.TagInteropVersion:
                    return GetInteropVersionDescription();
                case ExifDirectoryBase.TagOrientation:
                    return GetOrientationDescription();
                case ExifDirectoryBase.TagResolutionUnit:
                    return GetResolutionDescription();
                case ExifDirectoryBase.TagYCbCrPositioning:
                    return GetYCbCrPositioningDescription();
                case ExifDirectoryBase.TagXResolution:
                    return GetXResolutionDescription();
                case ExifDirectoryBase.TagYResolution:
                    return GetYResolutionDescription();
                case ExifDirectoryBase.TagImageWidth:
                    return GetImageWidthDescription();
                case ExifDirectoryBase.TagImageHeight:
                    return GetImageHeightDescription();
                case ExifDirectoryBase.TagBitsPerSample:
                    return GetBitsPerSampleDescription();
                case ExifDirectoryBase.TagPhotometricInterpretation:
                    return GetPhotometricInterpretationDescription();
                case ExifDirectoryBase.TagRowsPerStrip:
                    return GetRowsPerStripDescription();
                case ExifDirectoryBase.TagStripByteCounts:
                    return GetStripByteCountsDescription();
                case ExifDirectoryBase.TagSamplesPerPixel:
                    return GetSamplesPerPixelDescription();
                case ExifDirectoryBase.TagPlanarConfiguration:
                    return GetPlanarConfigurationDescription();
                case ExifDirectoryBase.TagYCbCrSubsampling:
                    return GetYCbCrSubsamplingDescription();
                case ExifDirectoryBase.TagReferenceBlackWhite:
                    return GetReferenceBlackWhiteDescription();
                case ExifDirectoryBase.TagWinAuthor:
                    return GetWindowsAuthorDescription();
                case ExifDirectoryBase.TagWinComment:
                    return GetWindowsCommentDescription();
                case ExifDirectoryBase.TagWinKeywords:
                    return GetWindowsKeywordsDescription();
                case ExifDirectoryBase.TagWinSubject:
                    return GetWindowsSubjectDescription();
                case ExifDirectoryBase.TagWinTitle:
                    return GetWindowsTitleDescription();
                case ExifDirectoryBase.TagNewSubfileType:
                    return GetNewSubfileTypeDescription();
                case ExifDirectoryBase.TagSubfileType:
                    return GetSubfileTypeDescription();
                case ExifDirectoryBase.TagThresholding:
                    return GetThresholdingDescription();
                case ExifDirectoryBase.TagFillOrder:
                    return GetFillOrderDescription();
                case ExifDirectoryBase.TagCfaPattern2:
                    return GetCfaPattern2Description();
                case ExifDirectoryBase.TagExposureTime:
                    return GetExposureTimeDescription();
                case ExifDirectoryBase.TagShutterSpeed:
                    return GetShutterSpeedDescription();
                case ExifDirectoryBase.TagFNumber:
                    return GetFNumberDescription();
                case ExifDirectoryBase.TagCompressedAverageBitsPerPixel:
                    return GetCompressedAverageBitsPerPixelDescription();
                case ExifDirectoryBase.TagSubjectDistance:
                    return GetSubjectDistanceDescription();
                case ExifDirectoryBase.TagMeteringMode:
                    return GetMeteringModeDescription();
                case ExifDirectoryBase.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case ExifDirectoryBase.TagFlash:
                    return GetFlashDescription();
                case ExifDirectoryBase.TagFocalLength:
                    return GetFocalLengthDescription();
                case ExifDirectoryBase.TagColorSpace:
                    return GetColorSpaceDescription();
                case ExifDirectoryBase.TagExifImageWidth:
                    return GetExifImageWidthDescription();
                case ExifDirectoryBase.TagExifImageHeight:
                    return GetExifImageHeightDescription();
                case ExifDirectoryBase.TagFocalPlaneResolutionUnit:
                    return GetFocalPlaneResolutionUnitDescription();
                case ExifDirectoryBase.TagFocalPlaneXResolution:
                    return GetFocalPlaneXResolutionDescription();
                case ExifDirectoryBase.TagFocalPlaneYResolution:
                    return GetFocalPlaneYResolutionDescription();
                case ExifDirectoryBase.TagExposureProgram:
                    return GetExposureProgramDescription();
                case ExifDirectoryBase.TagAperture:
                    return GetApertureValueDescription();
                case ExifDirectoryBase.TagMaxAperture:
                    return GetMaxApertureValueDescription();
                case ExifDirectoryBase.TagSensingMethod:
                    return GetSensingMethodDescription();
                case ExifDirectoryBase.TagExposureBias:
                    return GetExposureBiasDescription();
                case ExifDirectoryBase.TagFileSource:
                    return GetFileSourceDescription();
                case ExifDirectoryBase.TagSceneType:
                    return GetSceneTypeDescription();
                case ExifDirectoryBase.TagCfaPattern:
                    return GetCfaPatternDescription();
                case ExifDirectoryBase.TagComponentsConfiguration:
                    return GetComponentConfigurationDescription();
                case ExifDirectoryBase.TagExifVersion:
                    return GetExifVersionDescription();
                case ExifDirectoryBase.TagFlashpixVersion:
                    return GetFlashPixVersionDescription();
                case ExifDirectoryBase.TagIsoEquivalent:
                    return GetIsoEquivalentDescription();
                case ExifDirectoryBase.TagUserComment:
                    return GetUserCommentDescription();
                case ExifDirectoryBase.TagCustomRendered:
                    return GetCustomRenderedDescription();
                case ExifDirectoryBase.TagExposureMode:
                    return GetExposureModeDescription();
                case ExifDirectoryBase.TagWhiteBalanceMode:
                    return GetWhiteBalanceModeDescription();
                case ExifDirectoryBase.TagDigitalZoomRatio:
                    return GetDigitalZoomRatioDescription();
                case ExifDirectoryBase.Tag35MMFilmEquivFocalLength:
                    return Get35MMFilmEquivFocalLengthDescription();
                case ExifDirectoryBase.TagSceneCaptureType:
                    return GetSceneCaptureTypeDescription();
                case ExifDirectoryBase.TagGainControl:
                    return GetGainControlDescription();
                case ExifDirectoryBase.TagContrast:
                    return GetContrastDescription();
                case ExifDirectoryBase.TagSaturation:
                    return GetSaturationDescription();
                case ExifDirectoryBase.TagSharpness:
                    return GetSharpnessDescription();
                case ExifDirectoryBase.TagSubjectDistanceRange:
                    return GetSubjectDistanceRangeDescription();
                case ExifDirectoryBase.TagSensitivityType:
                    return GetSensitivityTypeDescription();
                case ExifDirectoryBase.TagCompression:
                    return GetCompressionDescription();
                case ExifDirectoryBase.TagJpegProc:
                    return GetJpegProcDescription();
                case ExifDirectoryBase.TagLensSpecification:
                    return GetLensSpecificationDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetInteropVersionDescription()
        {
            return GetVersionBytesDescription(ExifDirectoryBase.TagInteropVersion, 2);
        }

        [CanBeNull]
        public string GetInteropIndexDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagInteropIndex);
            if (value == null)
                return null;
            return string.Equals("R98", value.Trim(), StringComparison.OrdinalIgnoreCase)
                ? "Recommended Exif Interoperability Rules (ExifR98)"
                : "Unknown (" + value + ")";
        }

        [CanBeNull]
        public string GetReferenceBlackWhiteDescription()
        {
            var ints = Directory.GetInt32Array(ExifDirectoryBase.TagReferenceBlackWhite);
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

        [CanBeNull]
        public string GetYResolutionDescription()
        {
            var resolution = GetRationalOrDoubleString(ExifDirectoryBase.TagYResolution);
            if (resolution == null)
                return null;
            var unit = GetResolutionDescription();
            return $"{resolution} dots per {unit?.ToLower() ?? "unit"}";
        }

        [CanBeNull]
        public string GetXResolutionDescription()
        {
            var resolution = GetRationalOrDoubleString(ExifDirectoryBase.TagXResolution);
            if (resolution == null)
                return null;
            var unit = GetResolutionDescription();
            return $"{resolution} dots per {unit?.ToLower() ?? "unit"}";
        }

        [CanBeNull]
        public string GetYCbCrPositioningDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagYCbCrPositioning, 1,
                "Center of pixel array",
                "Datum point");
        }

        [CanBeNull]
        public string GetOrientationDescription()
        {
            return base.GetOrientationDescription(ExifDirectoryBase.TagOrientation);
        }

        [CanBeNull]
        public string GetResolutionDescription()
        {
            // '1' means no-unit, '2' means inch, '3' means centimeter. Default value is '2'(inch)
            return GetIndexedDescription(ExifDirectoryBase.TagResolutionUnit, 1,
                "(No unit)",
                "Inch",
                "cm");
        }

        /// <summary>The Windows specific tags uses plain Unicode.</summary>
        [CanBeNull]
        private string GetUnicodeDescription(int tag)
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

        [CanBeNull]
        public string GetWindowsAuthorDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinAuthor);
        }

        [CanBeNull]
        public string GetWindowsCommentDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinComment);
        }

        [CanBeNull]
        public string GetWindowsKeywordsDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinKeywords);
        }

        [CanBeNull]
        public string GetWindowsTitleDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinTitle);
        }

        [CanBeNull]
        public string GetWindowsSubjectDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinSubject);
        }

        [CanBeNull]
        public string GetYCbCrSubsamplingDescription()
        {
            var positions = Directory.GetInt32Array(ExifDirectoryBase.TagYCbCrSubsampling);
            if (positions == null || positions.Length < 2)
                return null;
            if (positions[0] == 2 && positions[1] == 1)
                return "YCbCr4:2:2";
            if (positions[0] == 2 && positions[1] == 2)
                return "YCbCr4:2:0";
            return "(Unknown)";
        }

        [CanBeNull]
        public string GetPlanarConfigurationDescription()
        {
            // When image format is no compression YCbCr, this value shows byte aligns of YCbCr
            // data. If value is '1', Y/Cb/Cr value is chunky format, contiguous for each subsampling
            // pixel. If value is '2', Y/Cb/Cr value is separated and stored to Y plane/Cb plane/Cr
            // plane format.
            return GetIndexedDescription(ExifDirectoryBase.TagPlanarConfiguration, 1, "Chunky (contiguous for each subsampling pixel)", "Separate (Y-plane/Cb-plane/Cr-plane format)");
        }

        [CanBeNull]
        public string GetSamplesPerPixelDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagSamplesPerPixel);
            return value == null ? null : value + " samples/pixel";
        }

        [CanBeNull]
        public string GetRowsPerStripDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagRowsPerStrip);
            return value == null ? null : value + " rows/strip";
        }

        [CanBeNull]
        public string GetStripByteCountsDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagStripByteCounts);
            return value == null ? null : value + " bytes";
        }

        [CanBeNull]
        public string GetPhotometricInterpretationDescription()
        {
            // Shows the color space of the image data components
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagPhotometricInterpretation, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "WhiteIsZero";
                case 1:
                    return "BlackIsZero";
                case 2:
                    return "RGB";
                case 3:
                    return "RGB Palette";
                case 4:
                    return "Transparency Mask";
                case 5:
                    return "CMYK";
                case 6:
                    return "YCbCr";
                case 8:
                    return "CIELab";
                case 9:
                    return "ICCLab";
                case 10:
                    return "ITULab";
                case 32803:
                    return "Color Filter Array";
                case 32844:
                    return "Pixar LogL";
                case 32845:
                    return "Pixar LogLuv";
                case 32892:
                    return "Linear Raw";
                default:
                    return "Unknown colour space";
            }
        }

        [CanBeNull]
        public string GetBitsPerSampleDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagBitsPerSample);
            return value == null ? null : value + " bits/component/pixel";
        }

        [CanBeNull]
        public string GetImageWidthDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagImageWidth);
            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public string GetImageHeightDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagImageHeight);
            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public string GetNewSubfileTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagNewSubfileType, 0,
                "Full-resolution image",
                "Reduced-resolution image",
                "Single page of multi-page image",
                "Single page of multi-page reduced-resolution image",
                "Transparency mask",
                "Transparency mask of reduced-resolution image",
                "Transparency mask of multi-page image",
                "Transparency mask of reduced-resolution multi-page image");
        }

        [CanBeNull]
        public string GetSubfileTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSubfileType, 1,
                "Full-resolution image",
                "Reduced-resolution image",
                "Single page of multi-page image");
        }

        [CanBeNull]
        public string GetThresholdingDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagThresholding, 1,
                "No dithering or halftoning",
                "Ordered dither or halftone",
                "Randomized dither");
        }

        [CanBeNull]
        public string GetFillOrderDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagFillOrder, 1,
                "Normal",
                "Reversed");
        }

        [CanBeNull]
        public string GetSubjectDistanceRangeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSubjectDistanceRange,
                "Unknown",
                "Macro",
                "Close view",
                "Distant view");
        }

        [CanBeNull]
        public string GetSensitivityTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSensitivityType,
                "Unknown",
                "Standard Output Sensitivity",
                "Recommended Exposure Index",
                "ISO Speed",
                "Standard Output Sensitivity and Recommended Exposure Index",
                "Standard Output Sensitivity and ISO Speed",
                "Recommended Exposure Index and ISO Speed",
                "Standard Output Sensitivity, Recommended Exposure Index and ISO Speed");
        }

        [CanBeNull]
        public string GetLensSpecificationDescription()
        {
            return GetLensSpecificationDescription(ExifDirectoryBase.TagLensSpecification);
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSharpness,
                "None",
                "Low",
                "Hard");
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSaturation,
                "None",
                "Low saturation",
                "High saturation");
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagContrast,
                "None",
                "Soft",
                "Hard");
        }

        [CanBeNull]
        public string GetGainControlDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagGainControl,
                "None",
                "Low gain up",
                "Low gain down",
                "High gain up",
                "High gain down");
        }

        [CanBeNull]
        public string GetSceneCaptureTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSceneCaptureType,
                "Standard",
                "Landscape",
                "Portrait",
                "Night scene");
        }

        [CanBeNull]
        public string Get35MMFilmEquivFocalLengthDescription()
        {
            if (!Directory.TryGetInt32(ExifDirectoryBase.Tag35MMFilmEquivFocalLength, out int value))
                return null;
            return value == 0 ? "Unknown" : GetFocalLengthDescription(value);
        }

        [CanBeNull]
        public string GetDigitalZoomRatioDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagDigitalZoomRatio, out Rational value))
                return null;
            return value.Numerator == 0
                ? "Digital zoom not used"
                : value.ToDouble().ToString("0.#");
        }

        [CanBeNull]
        public string GetWhiteBalanceModeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagWhiteBalanceMode,
                "Auto white balance",
                "Manual white balance");
        }

        [CanBeNull]
        public string GetExposureModeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagExposureMode,
                "Auto exposure",
                "Manual exposure",
                "Auto bracket");
        }

        [CanBeNull]
        public string GetCustomRenderedDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagCustomRendered,
                "Normal process",
                "Custom process");
        }

        [CanBeNull]
        public string GetUserCommentDescription()
        {
            var commentBytes = Directory.GetByteArray(ExifDirectoryBase.TagUserComment);

            if (commentBytes == null)
                return null;

            if (commentBytes.Length == 0)
                return string.Empty;

            // TODO use ByteTrie here
            // Someone suggested "ISO-8859-1".
            var encodingMap = new Dictionary<string, Encoding>
            {
                ["ASCII"] = Encoding.ASCII,
                ["UTF8"] = Encoding.UTF8,
                ["UTF7"] = Encoding.UTF7,
                ["UTF32"] = Encoding.UTF32,
                ["UNICODE"] = Encoding.Unicode,
                ["JIS"] = Encoding.GetEncoding("Shift-JIS")
            };

            try
            {
                if (commentBytes.Length >= 10)
                {
                    // TODO no guarantee bytes after the UTF8 name are valid UTF8 -- only read as many as needed
                    var firstTenBytesString = Encoding.UTF8.GetString(commentBytes, 0, 10);
                    // try each encoding name
                    foreach (var pair in encodingMap)
                    {
                        var encodingName = pair.Key;
                        var encoding = pair.Value;
                        if (firstTenBytesString.StartsWith(encodingName))
                        {
                            // skip any null or blank characters commonly present after the encoding name, up to a limit of 10 from the start
                            for (var j = encodingName.Length; j < 10; j++)
                            {
                                var b = commentBytes[j];
                                if (b != '\0' && b != ' ')
                                {
                                    return encoding.GetString(commentBytes, j, commentBytes.Length - j).Trim('\0', ' ');
                                }
                            }
                            return encoding.GetString(commentBytes, 10, commentBytes.Length - 10).Trim('\0', ' ');
                        }
                    }
                }
                // special handling fell through, return a plain string representation
                return Encoding.UTF8.GetString(commentBytes, 0, commentBytes.Length).Trim('\0', ' ');
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        public string GetIsoEquivalentDescription()
        {
            // Have seen an exception here from files produced by ACDSEE that stored an int[] here with two values
            // There used to be a check here that multiplied ISO values < 50 by 200.
            // Issue 36 shows a smart-phone image from a Samsung Galaxy S2 with ISO-40.
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagIsoEquivalent, out int value))
                return null;
            return value.ToString();
        }

        [CanBeNull]
        public string GetExifVersionDescription()
        {
            return GetVersionBytesDescription(ExifDirectoryBase.TagExifVersion, 2);
        }

        [CanBeNull]
        public string GetFlashPixVersionDescription()
        {
            return GetVersionBytesDescription(ExifDirectoryBase.TagFlashpixVersion, 2);
        }

        [CanBeNull]
        public string GetSceneTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSceneType, 1,
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
        [CanBeNull]
        public string GetCfaPatternDescription()
        {
            return FormatCFAPattern(DecodeCFAPattern(ExifDirectoryBase.TagCfaPattern));
        }

        /// <summary>
        /// String description of CFA Pattern
        /// </summary>
        /// <remarks>
        /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.
        /// It does not apply to all sensing methods.
        ///
        /// <see cref="ExifDirectoryBase.TagCfaPattern2"/> holds only the pixel pattern. <see cref="ExifDirectoryBase.TagCfaRepeatPatternDim"/> is expected to exist and pass
        /// some conditional tests.
        /// </remarks>
        [CanBeNull]
        public string GetCfaPattern2Description()
        {
            var values = Directory.GetByteArray(ExifDirectoryBase.TagCfaPattern2);
            if (values == null)
                return null;

            var repeatPattern = Directory.GetObject(ExifDirectoryBase.TagCfaRepeatPatternDim) as ushort[];
            if (repeatPattern == null)
                return $"Repeat Pattern not found for CFAPattern ({base.GetDescription(ExifDirectoryBase.TagCfaPattern2)})";

            if (repeatPattern.Length == 2 && values.Length == (repeatPattern[0] * repeatPattern[1]))
            {
                var intpattern = new int[2 + values.Length];
                intpattern[0] = repeatPattern[0];
                intpattern[1] = repeatPattern[1];

                Array.Copy(values, 0, intpattern, 2, values.Length);

                return FormatCFAPattern(intpattern);
            }

            return $"Unknown Pattern ({base.GetDescription(ExifDirectoryBase.TagCfaPattern2)})";
        }

        [CanBeNull]
        private static string FormatCFAPattern(int[] pattern)
        {
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
                else if(pos != end - 1)
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
        private int[] DecodeCFAPattern(int tagType)
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

        [CanBeNull]
        public string GetFileSourceDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagFileSource, 1,
                "Film Scanner",
                "Reflection Print Scanner",
                "Digital Still Camera (DSC)");
        }

        [CanBeNull]
        public string GetExposureBiasDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagExposureBias, out Rational value))
                return null;
            return value.ToSimpleString() + " EV";
        }

        [CanBeNull]
        public string GetMaxApertureValueDescription()
        {
            if (!Directory.TryGetDouble(ExifDirectoryBase.TagMaxAperture, out double aperture))
                return null;
            return GetFStopDescription(PhotographicConversions.ApertureToFStop(aperture));
        }

        [CanBeNull]
        public string GetApertureValueDescription()
        {
            if (!Directory.TryGetDouble(ExifDirectoryBase.TagAperture, out double aperture))
                return null;
            return GetFStopDescription(PhotographicConversions.ApertureToFStop(aperture));
        }

        [CanBeNull]
        public string GetExposureProgramDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagExposureProgram, 1,
                "Manual control",
                "Program normal",
                "Aperture priority",
                "Shutter priority",
                "Program creative (slow program)",
                "Program action (high-speed program)",
                "Portrait mode",
                "Landscape mode");
        }

        [CanBeNull]
        public string GetFocalPlaneXResolutionDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagFocalPlaneXResolution, out Rational value))
                return null;
            var unit = GetFocalPlaneResolutionUnitDescription();
            return value.Reciprocal.ToSimpleString() + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        [CanBeNull]
        public string GetFocalPlaneYResolutionDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagFocalPlaneYResolution, out Rational value))
                return null;
            var unit = GetFocalPlaneResolutionUnitDescription();
            return value.Reciprocal.ToSimpleString() + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        [CanBeNull]
        public string GetFocalPlaneResolutionUnitDescription()
        {
            // Unit of FocalPlaneXResolution/FocalPlaneYResolution.
            // '1' means no-unit, '2' inch, '3' centimeter.
            return GetIndexedDescription(ExifDirectoryBase.TagFocalPlaneResolutionUnit, 1,
                "(No unit)",
                "Inches",
                "cm");
        }

        [CanBeNull]
        public string GetExifImageWidthDescription()
        {
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagExifImageWidth, out int value))
                return null;
            return value + " pixels";
        }

        [CanBeNull]
        public string GetExifImageHeightDescription()
        {
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagExifImageHeight, out int value))
                return null;
            return value + " pixels";
        }

        [CanBeNull]
        public string GetColorSpaceDescription()
        {
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagColorSpace, out int value))
                return null;
            if (value == 1)
                return "sRGB";
            if (value == 65535)
                return "Undefined";
            return "Unknown (" + value + ")";
        }

        [CanBeNull]
        public string GetFocalLengthDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagFocalLength, out Rational value))
                return null;
            return GetFocalLengthDescription(value.ToDouble());
        }

        [CanBeNull]
        public string GetFlashDescription()
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
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagFlash, out int value))
                return null;

            var sb = new StringBuilder();
            sb.Append((value & 0x1) != 0 ? "Flash fired" : "Flash did not fire");
            // check if we're able to detect a return, before we mention it
            if ((value & 0x4) != 0)
                sb.Append((value & 0x2) != 0 ? ", return detected" : ", return not detected");
            if ((value & 0x10) != 0)
                sb.Append(", auto");
            if ((value & 0x40) != 0)
                sb.Append(", red-eye reduction");
            return sb.ToString();
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            // See http://web.archive.org/web/20131018091152/http://exif.org/Exif2-2.PDF page 35

            if (!Directory.TryGetInt32(ExifDirectoryBase.TagWhiteBalance, out int value))
                return null;

            switch (value)
            {
                case 0: return "Unknown";
                case 1: return "Daylight";
                case 2: return "Florescent";
                case 3: return "Tungsten";
                case 4: return "Flash";
                case 9: return "Fine Weather";
                case 10: return "Cloudy";
                case 11: return "Shade";
                case 12: return "Daylight Fluorescent";
                case 13: return "Day White Fluorescent";
                case 14: return "Cool White Fluorescent";
                case 15: return "White Fluorescent";
                case 16: return "Warm White Fluorescent";
                case 17: return "Standard light";
                case 18: return "Standard light (B)";
                case 19: return "Standard light (C)";
                case 20: return "D55";
                case 21: return "D65";
                case 22: return "D75";
                case 23: return "D50";
                case 24: return "Studio Tungsten";
                case 255: return "(Other)";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetMeteringModeDescription()
        {
            // '0' means unknown, '1' average, '2' center weighted average, '3' spot
            // '4' multi-spot, '5' multi-segment, '6' partial, '255' other
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagMeteringMode, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Unknown";
                case 1:
                    return "Average";
                case 2:
                    return "Center weighted average";
                case 3:
                    return "Spot";
                case 4:
                    return "Multi-spot";
                case 5:
                    return "Multi-segment";
                case 6:
                    return "Partial";
                case 255:
                    return "(Other)";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetCompressionDescription()
        {
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagCompression, out int value))
                return null;

            switch (value)
            {
                case 1:
                    return "Uncompressed";
                case 2:
                    return "CCITT 1D";
                case 3:
                    return "T4/Group 3 Fax";
                case 4:
                    return "T6/Group 4 Fax";
                case 5:
                    return "LZW";
                case 6:
                    return "JPEG (old-style)";
                case 7:
                    return "JPEG";
                case 8:
                    return "Adobe Deflate";
                case 9:
                    return "JBIG B&W";
                case 10:
                    return "JBIG Color";
                case 99:
                    return "JPEG";
                case 262:
                    return "Kodak 262";
                case 32766:
                    return "Next";
                case 32767:
                    return "Sony ARW Compressed";
                case 32769:
                    return "Packed RAW";
                case 32770:
                    return "Samsung SRW Compressed";
                case 32771:
                    return "CCIRLEW";
                case 32772:
                    return "Samsung SRW Compressed 2";
                case 32773:
                    return "PackBits";
                case 32809:
                    return "Thunderscan";
                case 32867:
                    return "Kodak KDC Compressed";
                case 32895:
                    return "IT8CTPAD";
                case 32896:
                    return "IT8LW";
                case 32897:
                    return "IT8MP";
                case 32898:
                    return "IT8BL";
                case 32908:
                    return "PixarFilm";
                case 32909:
                    return "PixarLog";
                case 32946:
                    return "Deflate";
                case 32947:
                    return "DCS";
                case 34661:
                    return "JBIG";
                case 34676:
                    return "SGILog";
                case 34677:
                    return "SGILog24";
                case 34712:
                    return "JPEG 2000";
                case 34713:
                    return "Nikon NEF Compressed";
                case 34715:
                    return "JBIG2 TIFF FX";
                case 34718:
                    return "Microsoft Document Imaging (MDI) Binary Level Codec";
                case 34719:
                    return "Microsoft Document Imaging (MDI) Progressive Transform Codec";
                case 34720:
                    return "Microsoft Document Imaging (MDI) Vector";
                case 34892:
                    return "Lossy JPEG";
                case 65000:
                    return "Kodak DCR Compressed";
                case 65535:
                    return "Pentax PEF Compressed";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSubjectDistanceDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagSubjectDistance, out Rational value))
                return null;
            return $"{value.ToDouble():0.0##} metres";
        }

        [CanBeNull]
        public string GetCompressedAverageBitsPerPixelDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagCompressedAverageBitsPerPixel, out Rational value))
                return null;
            var ratio = value.ToSimpleString();
            return value.IsInteger && value.ToInt32() == 1 ? ratio + " bit/pixel" : ratio + " bits/pixel";
        }

        [CanBeNull]
        public string GetExposureTimeDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagExposureTime);
            return value == null ? null : value + " sec";
        }

        [CanBeNull]
        public string GetShutterSpeedDescription()
        {
            return GetShutterSpeedDescription(ExifDirectoryBase.TagShutterSpeed);
        }

        [CanBeNull]
        public string GetFNumberDescription()
        {
            if (!Directory.TryGetRational(ExifDirectoryBase.TagFNumber, out Rational value))
                return null;
            return GetFStopDescription(value.ToDouble());
        }

        [CanBeNull]
        public string GetSensingMethodDescription()
        {
            // '1' Not defined, '2' One-chip color area sensor, '3' Two-chip color area sensor
            // '4' Three-chip color area sensor, '5' Color sequential area sensor
            // '7' Trilinear sensor '8' Color sequential linear sensor,  'Other' reserved
            return GetIndexedDescription(ExifDirectoryBase.TagSensingMethod, 1,
                "(Not defined)",
                "One-chip color area sensor",
                "Two-chip color area sensor",
                "Three-chip color area sensor",
                "Color sequential area sensor",
                null,
                "Trilinear sensor",
                "Color sequential linear sensor");
        }

        [CanBeNull]
        public string GetComponentConfigurationDescription()
        {
            var components = Directory.GetInt32Array(ExifDirectoryBase.TagComponentsConfiguration);
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

        [CanBeNull]
        public string GetJpegProcDescription()
        {
            if (!Directory.TryGetInt32(ExifDirectoryBase.TagJpegProc, out int value))
                return null;

            switch (value)
            {
                case 1:
                    return "Baseline";
                case 14:
                    return "Lossless";
                default:
                    return "Unknown (" + value + ")";
            }
        }
    }
}
