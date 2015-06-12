/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Util;
using Sharpen;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Base class for several Exif format descriptor classes.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class ExifDescriptorBase<T> : TagDescriptor<T>
        where T : Directory
    {
        /// <summary>
        /// Dictates whether rational values will be represented in decimal format in instances
        /// where decimal notation is elegant (such as 1/2 -&gt; 0.5, but not 1/3).
        /// </summary>
        private readonly bool _allowDecimalRepresentationOfRationals = true;

        public ExifDescriptorBase([NotNull] T directory)
            : base(directory)
        {
        }

        // Note for the potential addition of brightness presentation in eV:
        // Brightness of taken subject. To calculate Exposure(Ev) from BrightnessValue(Bv),
        // you must add SensitivityValue(Sv).
        // Ev=BV+Sv   Sv=log2(ISOSpeedRating/3.125)
        // ISO100:Sv=5, ISO200:Sv=6, ISO400:Sv=7, ISO125:Sv=5.32.
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ExifDirectoryBase.TagInteropIndex:
                {
                    // TODO order case blocks and corresponding methods in the same order as the TAG_* values are defined
                    return GetInteropIndexDescription();
                }

                case ExifDirectoryBase.TagInteropVersion:
                {
                    return GetInteropVersionDescription();
                }

                case ExifDirectoryBase.TagOrientation:
                {
                    return GetOrientationDescription();
                }

                case ExifDirectoryBase.TagResolutionUnit:
                {
                    return GetResolutionDescription();
                }

                case ExifDirectoryBase.TagYcbcrPositioning:
                {
                    return GetYCbCrPositioningDescription();
                }

                case ExifDirectoryBase.TagXResolution:
                {
                    return GetXResolutionDescription();
                }

                case ExifDirectoryBase.TagYResolution:
                {
                    return GetYResolutionDescription();
                }

                case ExifDirectoryBase.TagImageWidth:
                {
                    return GetImageWidthDescription();
                }

                case ExifDirectoryBase.TagImageHeight:
                {
                    return GetImageHeightDescription();
                }

                case ExifDirectoryBase.TagBitsPerSample:
                {
                    return GetBitsPerSampleDescription();
                }

                case ExifDirectoryBase.TagPhotometricInterpretation:
                {
                    return GetPhotometricInterpretationDescription();
                }

                case ExifDirectoryBase.TagRowsPerStrip:
                {
                    return GetRowsPerStripDescription();
                }

                case ExifDirectoryBase.TagStripByteCounts:
                {
                    return GetStripByteCountsDescription();
                }

                case ExifDirectoryBase.TagSamplesPerPixel:
                {
                    return GetSamplesPerPixelDescription();
                }

                case ExifDirectoryBase.TagPlanarConfiguration:
                {
                    return GetPlanarConfigurationDescription();
                }

                case ExifDirectoryBase.TagYcbcrSubsampling:
                {
                    return GetYCbCrSubsamplingDescription();
                }

                case ExifDirectoryBase.TagReferenceBlackWhite:
                {
                    return GetReferenceBlackWhiteDescription();
                }

                case ExifDirectoryBase.TagWinAuthor:
                {
                    return GetWindowsAuthorDescription();
                }

                case ExifDirectoryBase.TagWinComment:
                {
                    return GetWindowsCommentDescription();
                }

                case ExifDirectoryBase.TagWinKeywords:
                {
                    return GetWindowsKeywordsDescription();
                }

                case ExifDirectoryBase.TagWinSubject:
                {
                    return GetWindowsSubjectDescription();
                }

                case ExifDirectoryBase.TagWinTitle:
                {
                    return GetWindowsTitleDescription();
                }

                case ExifDirectoryBase.TagNewSubfileType:
                {
                    return GetNewSubfileTypeDescription();
                }

                case ExifDirectoryBase.TagSubfileType:
                {
                    return GetSubfileTypeDescription();
                }

                case ExifDirectoryBase.TagThresholding:
                {
                    return GetThresholdingDescription();
                }

                case ExifDirectoryBase.TagFillOrder:
                {
                    return GetFillOrderDescription();
                }

                case ExifDirectoryBase.TagExposureTime:
                {
                    return GetExposureTimeDescription();
                }

                case ExifDirectoryBase.TagShutterSpeed:
                {
                    return GetShutterSpeedDescription();
                }

                case ExifDirectoryBase.TagFnumber:
                {
                    return GetFNumberDescription();
                }

                case ExifDirectoryBase.TagCompressedAverageBitsPerPixel:
                {
                    return GetCompressedAverageBitsPerPixelDescription();
                }

                case ExifDirectoryBase.TagSubjectDistance:
                {
                    return GetSubjectDistanceDescription();
                }

                case ExifDirectoryBase.TagMeteringMode:
                {
                    return GetMeteringModeDescription();
                }

                case ExifDirectoryBase.TagWhiteBalance:
                {
                    return GetWhiteBalanceDescription();
                }

                case ExifDirectoryBase.TagFlash:
                {
                    return GetFlashDescription();
                }

                case ExifDirectoryBase.TagFocalLength:
                {
                    return GetFocalLengthDescription();
                }

                case ExifDirectoryBase.TagColorSpace:
                {
                    return GetColorSpaceDescription();
                }

                case ExifDirectoryBase.TagExifImageWidth:
                {
                    return GetExifImageWidthDescription();
                }

                case ExifDirectoryBase.TagExifImageHeight:
                {
                    return GetExifImageHeightDescription();
                }

                case ExifDirectoryBase.TagFocalPlaneResolutionUnit:
                {
                    return GetFocalPlaneResolutionUnitDescription();
                }

                case ExifDirectoryBase.TagFocalPlaneXResolution:
                {
                    return GetFocalPlaneXResolutionDescription();
                }

                case ExifDirectoryBase.TagFocalPlaneYResolution:
                {
                    return GetFocalPlaneYResolutionDescription();
                }

                case ExifDirectoryBase.TagExposureProgram:
                {
                    return GetExposureProgramDescription();
                }

                case ExifDirectoryBase.TagAperture:
                {
                    return GetApertureValueDescription();
                }

                case ExifDirectoryBase.TagMaxAperture:
                {
                    return GetMaxApertureValueDescription();
                }

                case ExifDirectoryBase.TagSensingMethod:
                {
                    return GetSensingMethodDescription();
                }

                case ExifDirectoryBase.TagExposureBias:
                {
                    return GetExposureBiasDescription();
                }

                case ExifDirectoryBase.TagFileSource:
                {
                    return GetFileSourceDescription();
                }

                case ExifDirectoryBase.TagSceneType:
                {
                    return GetSceneTypeDescription();
                }

                case ExifDirectoryBase.TagComponentsConfiguration:
                {
                    return GetComponentConfigurationDescription();
                }

                case ExifDirectoryBase.TagExifVersion:
                {
                    return GetExifVersionDescription();
                }

                case ExifDirectoryBase.TagFlashpixVersion:
                {
                    return GetFlashPixVersionDescription();
                }

                case ExifDirectoryBase.TagIsoEquivalent:
                {
                    return GetIsoEquivalentDescription();
                }

                case ExifDirectoryBase.TagUserComment:
                {
                    return GetUserCommentDescription();
                }

                case ExifDirectoryBase.TagCustomRendered:
                {
                    return GetCustomRenderedDescription();
                }

                case ExifDirectoryBase.TagExposureMode:
                {
                    return GetExposureModeDescription();
                }

                case ExifDirectoryBase.TagWhiteBalanceMode:
                {
                    return GetWhiteBalanceModeDescription();
                }

                case ExifDirectoryBase.TagDigitalZoomRatio:
                {
                    return GetDigitalZoomRatioDescription();
                }

                case ExifDirectoryBase.Tag35MmFilmEquivFocalLength:
                {
                    return Get35MmFilmEquivFocalLengthDescription();
                }

                case ExifDirectoryBase.TagSceneCaptureType:
                {
                    return GetSceneCaptureTypeDescription();
                }

                case ExifDirectoryBase.TagGainControl:
                {
                    return GetGainControlDescription();
                }

                case ExifDirectoryBase.TagContrast:
                {
                    return GetContrastDescription();
                }

                case ExifDirectoryBase.TagSaturation:
                {
                    return GetSaturationDescription();
                }

                case ExifDirectoryBase.TagSharpness:
                {
                    return GetSharpnessDescription();
                }

                case ExifDirectoryBase.TagSubjectDistanceRange:
                {
                    return GetSubjectDistanceRangeDescription();
                }

                case ExifDirectoryBase.TagSensitivityType:
                {
                    return GetSensitivityTypeRangeDescription();
                }

                case ExifDirectoryBase.TagCompression:
                {
                    return GetCompressionDescription();
                }

                case ExifDirectoryBase.TagJpegProc:
                {
                    return GetJpegProcDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public virtual string GetInteropVersionDescription()
        {
            return GetVersionBytesDescription(ExifDirectoryBase.TagInteropVersion, 2);
        }

        [CanBeNull]
        public virtual string GetInteropIndexDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagInteropIndex);
            if (value == null)
            {
                return null;
            }
            return "R98".Equals (value.Trim(), StringComparison.CurrentCultureIgnoreCase) ? "Recommended Exif Interoperability Rules (ExifR98)" : "Unknown (" + value + ")";
        }

        [CanBeNull]
        public virtual string GetReferenceBlackWhiteDescription()
        {
            var ints = Directory.GetIntArray(ExifDirectoryBase.TagReferenceBlackWhite);
            if (ints == null || ints.Length < 6)
            {
                return null;
            }
            var blackR = ints[0];
            var whiteR = ints[1];
            var blackG = ints[2];
            var whiteG = ints[3];
            var blackB = ints[4];
            var whiteB = ints[5];
            return string.Format("[{0},{1},{2}] [{3},{4},{5}]",
                blackR, blackG, blackB,
                whiteR, whiteG, whiteB);
        }

        [CanBeNull]
        public virtual string GetYResolutionDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagYResolution);
            if (value == null)
            {
                return null;
            }
            var unit = GetResolutionDescription();
            return string.Format("{0} dots per {1}", value.ToSimpleString(_allowDecimalRepresentationOfRationals), unit == null ? "unit" : unit.ToLower());
        }

        [CanBeNull]
        public virtual string GetXResolutionDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagXResolution);
            if (value == null)
            {
                return null;
            }
            var unit = GetResolutionDescription();
            return string.Format("{0} dots per {1}", value.ToSimpleString(_allowDecimalRepresentationOfRationals), unit == null ? "unit" : unit.ToLower());
        }

        [CanBeNull]
        public virtual string GetYCbCrPositioningDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagYcbcrPositioning, 1, "Center of pixel array", "Datum point");
        }

        [CanBeNull]
        public virtual string GetOrientationDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagOrientation, 1, "Top, left side (Horizontal / normal)", "Top, right side (Mirror horizontal)", "Bottom, right side (Rotate 180)", "Bottom, left side (Mirror vertical)", "Left side, top (Mirror horizontal and rotate 270 CW)"
                , "Right side, top (Rotate 90 CW)", "Right side, bottom (Mirror horizontal and rotate 90 CW)", "Left side, bottom (Rotate 270 CW)");
        }

        [CanBeNull]
        public virtual string GetResolutionDescription()
        {
            // '1' means no-unit, '2' means inch, '3' means centimeter. Default value is '2'(inch)
            return GetIndexedDescription(ExifDirectoryBase.TagResolutionUnit, 1, "(No unit)", "Inch", "cm");
        }

        /// <summary>The Windows specific tags uses plain Unicode.</summary>
        [CanBeNull]
        private string GetUnicodeDescription(int tag)
        {
            var bytes = Directory.GetByteArray(tag);
            if (bytes == null)
            {
                return null;
            }
            try
            {
                // Decode the unicode string and trim the unicode zero "\0" from the end.
                return Encoding.Unicode.GetString(bytes).TrimEnd('\0');
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        public virtual string GetWindowsAuthorDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinAuthor);
        }

        [CanBeNull]
        public virtual string GetWindowsCommentDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinComment);
        }

        [CanBeNull]
        public virtual string GetWindowsKeywordsDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinKeywords);
        }

        [CanBeNull]
        public virtual string GetWindowsTitleDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinTitle);
        }

        [CanBeNull]
        public virtual string GetWindowsSubjectDescription()
        {
            return GetUnicodeDescription(ExifDirectoryBase.TagWinSubject);
        }

        [CanBeNull]
        public virtual string GetYCbCrSubsamplingDescription()
        {
            var positions = Directory.GetIntArray(ExifDirectoryBase.TagYcbcrSubsampling);
            if (positions == null || positions.Length < 2)
            {
                return null;
            }
            if (positions[0] == 2 && positions[1] == 1)
            {
                return "YCbCr4:2:2";
            }
            if (positions[0] == 2 && positions[1] == 2)
            {
                return "YCbCr4:2:0";
            }
            return "(Unknown)";
        }

        [CanBeNull]
        public virtual string GetPlanarConfigurationDescription()
        {
            // When image format is no compression YCbCr, this value shows byte aligns of YCbCr
            // data. If value is '1', Y/Cb/Cr value is chunky format, contiguous for each subsampling
            // pixel. If value is '2', Y/Cb/Cr value is separated and stored to Y plane/Cb plane/Cr
            // plane format.
            return GetIndexedDescription(ExifDirectoryBase.TagPlanarConfiguration, 1, "Chunky (contiguous for each subsampling pixel)", "Separate (Y-plane/Cb-plane/Cr-plane format)");
        }

        [CanBeNull]
        public virtual string GetSamplesPerPixelDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagSamplesPerPixel);
            return value == null ? null : value + " samples/pixel";
        }

        [CanBeNull]
        public virtual string GetRowsPerStripDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagRowsPerStrip);
            return value == null ? null : value + " rows/strip";
        }

        [CanBeNull]
        public virtual string GetStripByteCountsDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagStripByteCounts);
            return value == null ? null : value + " bytes";
        }

        [CanBeNull]
        public virtual string GetPhotometricInterpretationDescription()
        {
            // Shows the color space of the image data components
            var value = Directory.GetInteger(ExifDirectoryBase.TagPhotometricInterpretation);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "WhiteIsZero";
                }

                case 1:
                {
                    return "BlackIsZero";
                }

                case 2:
                {
                    return "RGB";
                }

                case 3:
                {
                    return "RGB Palette";
                }

                case 4:
                {
                    return "Transparency Mask";
                }

                case 5:
                {
                    return "CMYK";
                }

                case 6:
                {
                    return "YCbCr";
                }

                case 8:
                {
                    return "CIELab";
                }

                case 9:
                {
                    return "ICCLab";
                }

                case 10:
                {
                    return "ITULab";
                }

                case 32803:
                {
                    return "Color Filter Array";
                }

                case 32844:
                {
                    return "Pixar LogL";
                }

                case 32845:
                {
                    return "Pixar LogLuv";
                }

                case 32892:
                {
                    return "Linear Raw";
                }

                default:
                {
                    return "Unknown colour space";
                }
            }
        }

        [CanBeNull]
        public virtual string GetBitsPerSampleDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagBitsPerSample);
            return value == null ? null : value + " bits/component/pixel";
        }

        [CanBeNull]
        public virtual string GetImageWidthDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagImageWidth);
            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public virtual string GetImageHeightDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagImageHeight);
            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public virtual string GetNewSubfileTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagNewSubfileType, 1, "Full-resolution image", "Reduced-resolution image", "Single page of multi-page reduced-resolution image", "Transparency mask", "Transparency mask of reduced-resolution image"
                , "Transparency mask of multi-page image", "Transparency mask of reduced-resolution multi-page image");
        }

        [CanBeNull]
        public virtual string GetSubfileTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSubfileType, 1, "Full-resolution image", "Reduced-resolution image", "Single page of multi-page image");
        }

        [CanBeNull]
        public virtual string GetThresholdingDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagThresholding, 1, "No dithering or halftoning", "Ordered dither or halftone", "Randomized dither");
        }

        [CanBeNull]
        public virtual string GetFillOrderDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagFillOrder, 1, "Normal", "Reversed");
        }

        [CanBeNull]
        public virtual string GetSubjectDistanceRangeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSubjectDistanceRange, "Unknown", "Macro", "Close view", "Distant view");
        }

        [CanBeNull]
        public virtual string GetSensitivityTypeRangeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSensitivityType, "Unknown", "Standard Output Sensitivity", "Recommended Exposure Index", "ISO Speed", "Standard Output Sensitivity and Recommended Exposure Index", "Standard Output Sensitivity and ISO Speed"
                , "Recommended Exposure Index and ISO Speed", "Standard Output Sensitivity, Recommended Exposure Index and ISO Speed");
        }

        [CanBeNull]
        public virtual string GetSharpnessDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSharpness, "None", "Low", "Hard");
        }

        [CanBeNull]
        public virtual string GetSaturationDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSaturation, "None", "Low saturation", "High saturation");
        }

        [CanBeNull]
        public virtual string GetContrastDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagContrast, "None", "Soft", "Hard");
        }

        [CanBeNull]
        public virtual string GetGainControlDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagGainControl, "None", "Low gain up", "Low gain down", "High gain up", "High gain down");
        }

        [CanBeNull]
        public virtual string GetSceneCaptureTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSceneCaptureType, "Standard", "Landscape", "Portrait", "Night scene");
        }

        [CanBeNull]
        public virtual string Get35MmFilmEquivFocalLengthDescription()
        {
            var value = Directory.GetInteger(ExifDirectoryBase.Tag35MmFilmEquivFocalLength);
            return value == null ? null : value == 0 ? "Unknown" : string.Format("{0:0.#}mm", value);
        }

        [CanBeNull]
        public virtual string GetDigitalZoomRatioDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagDigitalZoomRatio);
            return value == null ? null : value.GetNumerator() == 0 ? "Digital zoom not used." : value.DoubleValue().ToString("0.#");
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceModeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagWhiteBalanceMode, "Auto white balance", "Manual white balance");
        }

        [CanBeNull]
        public virtual string GetExposureModeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagExposureMode, "Auto exposure", "Manual exposure", "Auto bracket");
        }

        [CanBeNull]
        public virtual string GetCustomRenderedDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagCustomRendered, "Normal process", "Custom process");
        }

        [CanBeNull]
        public virtual string GetUserCommentDescription()
        {
            var commentBytes = Directory.GetByteArray(ExifDirectoryBase.TagUserComment);
            if (commentBytes == null)
            {
                return null;
            }
            if (commentBytes.Length == 0)
            {
                return string.Empty;
            }
            IDictionary<string, Encoding> encodingMap = new Dictionary<string, Encoding>();
            encodingMap["ASCII"] = Encoding.ASCII;
            // Someone suggested "ISO-8859-1".
            encodingMap["UNICODE"] = Encoding.Unicode;
            encodingMap["JIS"] = Encoding.GetEncoding("Shift-JIS");
            // We assume this charset for now.  Another suggestion is "JIS".
            try
            {
                if (commentBytes.Length >= 10)
                {
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
                return Encoding.UTF8.GetString(commentBytes).Trim('\0', ' ');
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        public virtual string GetIsoEquivalentDescription()
        {
            // Have seen an exception here from files produced by ACDSEE that stored an int[] here with two values
            var isoEquiv = Directory.GetInteger(ExifDirectoryBase.TagIsoEquivalent);
            // There used to be a check here that multiplied ISO values < 50 by 200.
            // Issue 36 shows a smart-phone image from a Samsung Galaxy S2 with ISO-40.
            return isoEquiv != null ? ((int)isoEquiv).ToString() : null;
        }

        [CanBeNull]
        public virtual string GetExifVersionDescription()
        {
            return GetVersionBytesDescription(ExifDirectoryBase.TagExifVersion, 2);
        }

        [CanBeNull]
        public virtual string GetFlashPixVersionDescription()
        {
            return GetVersionBytesDescription(ExifDirectoryBase.TagFlashpixVersion, 2);
        }

        [CanBeNull]
        public virtual string GetSceneTypeDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagSceneType, 1, "Directly photographed image");
        }

        [CanBeNull]
        public virtual string GetFileSourceDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagFileSource, 1, "Film Scanner", "Reflection Print Scanner", "Digital Still Camera (DSC)");
        }

        [CanBeNull]
        public virtual string GetExposureBiasDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagExposureBias);
            if (value == null)
            {
                return null;
            }
            return value.ToSimpleString(true) + " EV";
        }

        [CanBeNull]
        public virtual string GetMaxApertureValueDescription()
        {
            var aperture = Directory.GetDoubleObject(ExifDirectoryBase.TagMaxAperture);
            if (aperture == null)
            {
                return null;
            }
            var fStop = PhotographicConversions.ApertureToFStop((double)aperture);
            return "f/" + fStop.ToString("0.0");
        }

        [CanBeNull]
        public virtual string GetApertureValueDescription()
        {
            var aperture = Directory.GetDoubleObject(ExifDirectoryBase.TagAperture);
            if (aperture == null)
            {
                return null;
            }
            var fStop = PhotographicConversions.ApertureToFStop((double)aperture);
            return "f/" + fStop.ToString("0.0");
        }

        [CanBeNull]
        public virtual string GetExposureProgramDescription()
        {
            return GetIndexedDescription(ExifDirectoryBase.TagExposureProgram, 1, "Manual control", "Program normal", "Aperture priority", "Shutter priority", "Program creative (slow program)", "Program action (high-speed program)", "Portrait mode", "Landscape mode"
                );
        }

        [CanBeNull]
        public virtual string GetFocalPlaneXResolutionDescription()
        {
            var rational = Directory.GetRational(ExifDirectoryBase.TagFocalPlaneXResolution);
            if (rational == null)
            {
                return null;
            }
            var unit = GetFocalPlaneResolutionUnitDescription();
            return rational.GetReciprocal().ToSimpleString(_allowDecimalRepresentationOfRationals) + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        [CanBeNull]
        public virtual string GetFocalPlaneYResolutionDescription()
        {
            var rational = Directory.GetRational(ExifDirectoryBase.TagFocalPlaneYResolution);
            if (rational == null)
            {
                return null;
            }
            var unit = GetFocalPlaneResolutionUnitDescription();
            return rational.GetReciprocal().ToSimpleString(_allowDecimalRepresentationOfRationals) + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        [CanBeNull]
        public virtual string GetFocalPlaneResolutionUnitDescription()
        {
            // Unit of FocalPlaneXResolution/FocalPlaneYResolution.
            // '1' means no-unit, '2' inch, '3' centimeter.
            return GetIndexedDescription(ExifDirectoryBase.TagFocalPlaneResolutionUnit, 1, "(No unit)", "Inches", "cm");
        }

        [CanBeNull]
        public virtual string GetExifImageWidthDescription()
        {
            var value = Directory.GetInteger(ExifDirectoryBase.TagExifImageWidth);
            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public virtual string GetExifImageHeightDescription()
        {
            var value = Directory.GetInteger(ExifDirectoryBase.TagExifImageHeight);
            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public virtual string GetColorSpaceDescription()
        {
            var value = Directory.GetInteger(ExifDirectoryBase.TagColorSpace);
            if (value == null)
            {
                return null;
            }
            if (value == 1)
            {
                return "sRGB";
            }
            if (value == 65535)
            {
                return "Undefined";
            }
            return "Unknown (" + value + ")";
        }

        [CanBeNull]
        public virtual string GetFocalLengthDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagFocalLength);
            return value == null ? null : string.Format("{00.0##} mm", value.DoubleValue());
        }

        [CanBeNull]
        public virtual string GetFlashDescription()
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
            var value = Directory.GetInteger(ExifDirectoryBase.TagFlash);
            if (value == null)
                return null;

            var sb = new StringBuilder();
            sb.Append((value & 0x1) != 0 ? "Flash fired" : "Flash did not fire");
            // check if we're able to detect a return, before we mention it
            if ((value & 0x4) != 0)
                sb.Append((value.Value & 0x2) != 0 ? ", return detected" : ", return not detected");
            if ((value & 0x10) != 0)
                sb.Append(", auto");
            if ((value & 0x40) != 0)
                sb.Append(", red-eye reduction");
            return sb.ToString();
        }

        [CanBeNull]
        public virtual string GetWhiteBalanceDescription()
        {
            // '0' means unknown, '1' daylight, '2' fluorescent, '3' tungsten, '10' flash,
            // '17' standard light A, '18' standard light B, '19' standard light C, '20' D55,
            // '21' D65, '22' D75, '255' other.
            var value = Directory.GetInteger(ExifDirectoryBase.TagWhiteBalance);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Unknown";
                }

                case 1:
                {
                    return "Daylight";
                }

                case 2:
                {
                    return "Florescent";
                }

                case 3:
                {
                    return "Tungsten";
                }

                case 10:
                {
                    return "Flash";
                }

                case 17:
                {
                    return "Standard light";
                }

                case 18:
                {
                    return "Standard light (B)";
                }

                case 19:
                {
                    return "Standard light (C)";
                }

                case 20:
                {
                    return "D55";
                }

                case 21:
                {
                    return "D65";
                }

                case 22:
                {
                    return "D75";
                }

                case 255:
                {
                    return "(Other)";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetMeteringModeDescription()
        {
            // '0' means unknown, '1' average, '2' center weighted average, '3' spot
            // '4' multi-spot, '5' multi-segment, '6' partial, '255' other
            var value = Directory.GetInteger(ExifDirectoryBase.TagMeteringMode);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Unknown";
                }

                case 1:
                {
                    return "Average";
                }

                case 2:
                {
                    return "Center weighted average";
                }

                case 3:
                {
                    return "Spot";
                }

                case 4:
                {
                    return "Multi-spot";
                }

                case 5:
                {
                    return "Multi-segment";
                }

                case 6:
                {
                    return "Partial";
                }

                case 255:
                {
                    return "(Other)";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetCompressionDescription()
        {
            var value = Directory.GetInteger(ExifDirectoryBase.TagCompression);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 1:
                {
                    return "Uncompressed";
                }

                case 2:
                {
                    return "CCITT 1D";
                }

                case 3:
                {
                    return "T4/Group 3 Fax";
                }

                case 4:
                {
                    return "T6/Group 4 Fax";
                }

                case 5:
                {
                    return "LZW";
                }

                case 6:
                {
                    return "JPEG (old-style)";
                }

                case 7:
                {
                    return "JPEG";
                }

                case 8:
                {
                    return "Adobe Deflate";
                }

                case 9:
                {
                    return "JBIG B&W";
                }

                case 10:
                {
                    return "JBIG Color";
                }

                case 99:
                {
                    return "JPEG";
                }

                case 262:
                {
                    return "Kodak 262";
                }

                case 32766:
                {
                    return "Next";
                }

                case 32767:
                {
                    return "Sony ARW Compressed";
                }

                case 32769:
                {
                    return "Packed RAW";
                }

                case 32770:
                {
                    return "Samsung SRW Compressed";
                }

                case 32771:
                {
                    return "CCIRLEW";
                }

                case 32772:
                {
                    return "Samsung SRW Compressed 2";
                }

                case 32773:
                {
                    return "PackBits";
                }

                case 32809:
                {
                    return "Thunderscan";
                }

                case 32867:
                {
                    return "Kodak KDC Compressed";
                }

                case 32895:
                {
                    return "IT8CTPAD";
                }

                case 32896:
                {
                    return "IT8LW";
                }

                case 32897:
                {
                    return "IT8MP";
                }

                case 32898:
                {
                    return "IT8BL";
                }

                case 32908:
                {
                    return "PixarFilm";
                }

                case 32909:
                {
                    return "PixarLog";
                }

                case 32946:
                {
                    return "Deflate";
                }

                case 32947:
                {
                    return "DCS";
                }

                case 34661:
                {
                    return "JBIG";
                }

                case 34676:
                {
                    return "SGILog";
                }

                case 34677:
                {
                    return "SGILog24";
                }

                case 34712:
                {
                    return "JPEG 2000";
                }

                case 34713:
                {
                    return "Nikon NEF Compressed";
                }

                case 34715:
                {
                    return "JBIG2 TIFF FX";
                }

                case 34718:
                {
                    return "Microsoft Document Imaging (MDI) Binary Level Codec";
                }

                case 34719:
                {
                    return "Microsoft Document Imaging (MDI) Progressive Transform Codec";
                }

                case 34720:
                {
                    return "Microsoft Document Imaging (MDI) Vector";
                }

                case 34892:
                {
                    return "Lossy JPEG";
                }

                case 65000:
                {
                    return "Kodak DCR Compressed";
                }

                case 65535:
                {
                    return "Pentax PEF Compressed";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public virtual string GetSubjectDistanceDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagSubjectDistance);
            return value == null ? null : string.Format("{0:0.0##} metres", value.DoubleValue());
        }

        [CanBeNull]
        public virtual string GetCompressedAverageBitsPerPixelDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagCompressedAverageBitsPerPixel);
            if (value == null)
            {
                return null;
            }
            var ratio = value.ToSimpleString(_allowDecimalRepresentationOfRationals);
            return value.IsInteger() && value.IntValue() == 1 ? ratio + " bit/pixel" : ratio + " bits/pixel";
        }

        [CanBeNull]
        public virtual string GetExposureTimeDescription()
        {
            var value = Directory.GetString(ExifDirectoryBase.TagExposureTime);
            return value == null ? null : value + " sec";
        }

        [CanBeNull]
        public virtual string GetShutterSpeedDescription()
        {
            // I believe this method to now be stable, but am leaving some alternative snippets of
            // code in here, to assist anyone who's looking into this (given that I don't have a public CVS).
            //        float apexValue = _directory.getFloat(ExifSubIFDDirectory.TAG_SHUTTER_SPEED);
            //        int apexPower = (int)Math.pow(2.0, apexValue);
            //        return "1/" + apexPower + " sec";
            // TODO test this method
            // thanks to Mark Edwards for spotting and patching a bug in the calculation of this
            // description (spotted bug using a Canon EOS 300D)
            // thanks also to Gli Blr for spotting this bug
            var apexValue = Directory.GetFloatObject(ExifDirectoryBase.TagShutterSpeed);
            if (apexValue == null)
            {
                return null;
            }
            if (apexValue <= 1)
            {
                var apexPower = (float)(1 / (Math.Exp((double)apexValue * Math.Log(2))));
                var apexPower10 = (long)Math.Round(apexPower * 10.0);
                var fApexPower = apexPower10 / 10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)((Math.Exp((double)apexValue * Math.Log(2))));
                return "1/" + apexPower + " sec";
            }
        }

/*
        // This alternative implementation offered by Bill Richards
        // TODO determine which is the correct / more-correct implementation
        double apexValue = _directory.getDouble(ExifSubIFDDirectory.TAG_SHUTTER_SPEED);
        double apexPower = Math.pow(2.0, apexValue);

        StringBuffer sb = new StringBuffer();
        if (apexPower > 1)
            apexPower = Math.floor(apexPower);

        if (apexPower < 1) {
            sb.append((int)Math.round(1/apexPower));
        } else {
            sb.append("1/");
            sb.append((int)apexPower);
        }
        sb.append(" sec");
        return sb.toString();
*/
        [CanBeNull]
        public virtual string GetFNumberDescription()
        {
            var value = Directory.GetRational(ExifDirectoryBase.TagFnumber);
            if (value == null)
            {
                return null;
            }
            return "f/" + value.DoubleValue().ToString("0.0");
        }

        [CanBeNull]
        public virtual string GetSensingMethodDescription()
        {
            // '1' Not defined, '2' One-chip color area sensor, '3' Two-chip color area sensor
            // '4' Three-chip color area sensor, '5' Color sequential area sensor
            // '7' Trilinear sensor '8' Color sequential linear sensor,  'Other' reserved
            return GetIndexedDescription(ExifDirectoryBase.TagSensingMethod, 1, "(Not defined)", "One-chip color area sensor", "Two-chip color area sensor", "Three-chip color area sensor", "Color sequential area sensor", null, "Trilinear sensor", "Color sequential linear sensor"
                );
        }

        [CanBeNull]
        public virtual string GetComponentConfigurationDescription()
        {
            var components = Directory.GetIntArray(ExifDirectoryBase.TagComponentsConfiguration);
            if (components == null)
            {
                return null;
            }
            var componentStrings = new[] { string.Empty, "Y", "Cb", "Cr", "R", "G", "B" };
            var componentConfig = new StringBuilder();
            for (var i = 0; i < Math.Min(4, components.Length); i++)
            {
                var j = components[i];
                if (j > 0 && j < componentStrings.Length)
                {
                    componentConfig.Append(componentStrings[j]);
                }
            }
            return componentConfig.ToString();
        }

        [CanBeNull]
        public virtual string GetJpegProcDescription()
        {
            var value = Directory.GetInteger(ExifDirectoryBase.TagJpegProc);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 1:
                {
                    return "Baseline";
                }

                case 14:
                {
                    return "Lossless";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }
    }
}
