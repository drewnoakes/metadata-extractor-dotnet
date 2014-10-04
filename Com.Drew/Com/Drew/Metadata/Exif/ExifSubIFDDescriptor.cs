/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Drew.Imaging;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="ExifSubIFDDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifSubIFDDescriptor : TagDescriptor<ExifSubIFDDirectory>
	{
		/// <summary>
		/// Dictates whether rational values will be represented in decimal format in instances
		/// where decimal notation is elegant (such as 1/2 -&gt; 0.5, but not 1/3).
		/// </summary>
		private readonly bool _allowDecimalRepresentationOfRationals = true;

		[NotNull]
		private static readonly DecimalFormat SimpleDecimalFormatter = new DecimalFormat("0.#");

		public ExifSubIFDDescriptor(ExifSubIFDDirectory directory)
			: base(directory)
		{
		}

		// Note for the potential addition of brightness presentation in eV:
		// Brightness of taken subject. To calculate Exposure(Ev) from BrightnessValue(Bv),
		// you must add SensitivityValue(Sv).
		// Ev=BV+Sv   Sv=log2(ISOSpeedRating/3.125)
		// ISO100:Sv=5, ISO200:Sv=6, ISO400:Sv=7, ISO125:Sv=5.32.
		/// <summary>Returns a descriptive value of the specified tag for this image.</summary>
		/// <remarks>
		/// Returns a descriptive value of the specified tag for this image.
		/// Where possible, known values will be substituted here in place of the raw
		/// tokens actually kept in the Exif segment.  If no substitution is
		/// available, the value provided by getString(int) will be returned.
		/// </remarks>
		/// <param name="tagType">the tag to find a description for</param>
		/// <returns>
		/// a description of the image's value for the specified tag, or
		/// <code>null</code> if the tag hasn't been defined.
		/// </returns>
		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
                case ExifSubIFDDirectory.TagNewSubfileType:
				{
					return GetNewSubfileTypeDescription();
				}

                case ExifSubIFDDirectory.TagSubfileType:
				{
					return GetSubfileTypeDescription();
				}

                case ExifSubIFDDirectory.TagThresholding:
				{
					return GetThresholdingDescription();
				}

                case ExifSubIFDDirectory.TagFillOrder:
				{
					return GetFillOrderDescription();
				}

                case ExifSubIFDDirectory.TagExposureTime:
				{
					return GetExposureTimeDescription();
				}

                case ExifSubIFDDirectory.TagShutterSpeed:
				{
					return GetShutterSpeedDescription();
				}

                case ExifSubIFDDirectory.TagFnumber:
				{
					return GetFNumberDescription();
				}

                case ExifSubIFDDirectory.TagCompressedAverageBitsPerPixel:
				{
					return GetCompressedAverageBitsPerPixelDescription();
				}

                case ExifSubIFDDirectory.TagSubjectDistance:
				{
					return GetSubjectDistanceDescription();
				}

                case ExifSubIFDDirectory.TagMeteringMode:
				{
					return GetMeteringModeDescription();
				}

                case ExifSubIFDDirectory.TagWhiteBalance:
				{
					return GetWhiteBalanceDescription();
				}

                case ExifSubIFDDirectory.TagFlash:
				{
					return GetFlashDescription();
				}

                case ExifSubIFDDirectory.TagFocalLength:
				{
					return GetFocalLengthDescription();
				}

                case ExifSubIFDDirectory.TagColorSpace:
				{
					return GetColorSpaceDescription();
				}

                case ExifSubIFDDirectory.TagExifImageWidth:
				{
					return GetExifImageWidthDescription();
				}

                case ExifSubIFDDirectory.TagExifImageHeight:
				{
					return GetExifImageHeightDescription();
				}

                case ExifSubIFDDirectory.TagFocalPlaneResolutionUnit:
				{
					return GetFocalPlaneResolutionUnitDescription();
				}

                case ExifSubIFDDirectory.TagFocalPlaneXResolution:
				{
					return GetFocalPlaneXResolutionDescription();
				}

                case ExifSubIFDDirectory.TagFocalPlaneYResolution:
				{
					return GetFocalPlaneYResolutionDescription();
				}

                case ExifSubIFDDirectory.TagBitsPerSample:
				{
					return GetBitsPerSampleDescription();
				}

                case ExifSubIFDDirectory.TagPhotometricInterpretation:
				{
					return GetPhotometricInterpretationDescription();
				}

                case ExifSubIFDDirectory.TagRowsPerStrip:
				{
					return GetRowsPerStripDescription();
				}

                case ExifSubIFDDirectory.TagStripByteCounts:
				{
					return GetStripByteCountsDescription();
				}

                case ExifSubIFDDirectory.TagSamplesPerPixel:
				{
					return GetSamplesPerPixelDescription();
				}

                case ExifSubIFDDirectory.TagPlanarConfiguration:
				{
					return GetPlanarConfigurationDescription();
				}

                case ExifSubIFDDirectory.TagYcbcrSubsampling:
				{
					return GetYCbCrSubsamplingDescription();
				}

                case ExifSubIFDDirectory.TagExposureProgram:
				{
					return GetExposureProgramDescription();
				}

                case ExifSubIFDDirectory.TagAperture:
				{
					return GetApertureValueDescription();
				}

                case ExifSubIFDDirectory.TagMaxAperture:
				{
					return GetMaxApertureValueDescription();
				}

                case ExifSubIFDDirectory.TagSensingMethod:
				{
					return GetSensingMethodDescription();
				}

                case ExifSubIFDDirectory.TagExposureBias:
				{
					return GetExposureBiasDescription();
				}

                case ExifSubIFDDirectory.TagFileSource:
				{
					return GetFileSourceDescription();
				}

                case ExifSubIFDDirectory.TagSceneType:
				{
					return GetSceneTypeDescription();
				}

                case ExifSubIFDDirectory.TagComponentsConfiguration:
				{
					return GetComponentConfigurationDescription();
				}

                case ExifSubIFDDirectory.TagExifVersion:
				{
					return GetExifVersionDescription();
				}

                case ExifSubIFDDirectory.TagFlashpixVersion:
				{
					return GetFlashPixVersionDescription();
				}

                case ExifSubIFDDirectory.TagIsoEquivalent:
				{
					return GetIsoEquivalentDescription();
				}

                case ExifSubIFDDirectory.TagUserComment:
				{
					return GetUserCommentDescription();
				}

                case ExifSubIFDDirectory.TagCustomRendered:
				{
					return GetCustomRenderedDescription();
				}

                case ExifSubIFDDirectory.TagExposureMode:
				{
					return GetExposureModeDescription();
				}

                case ExifSubIFDDirectory.TagWhiteBalanceMode:
				{
					return GetWhiteBalanceModeDescription();
				}

                case ExifSubIFDDirectory.TagDigitalZoomRatio:
				{
					return GetDigitalZoomRatioDescription();
				}

                case ExifSubIFDDirectory.Tag35mmFilmEquivFocalLength:
				{
					return Get35mmFilmEquivFocalLengthDescription();
				}

                case ExifSubIFDDirectory.TagSceneCaptureType:
				{
					return GetSceneCaptureTypeDescription();
				}

                case ExifSubIFDDirectory.TagGainControl:
				{
					return GetGainControlDescription();
				}

                case ExifSubIFDDirectory.TagContrast:
				{
					return GetContrastDescription();
				}

                case ExifSubIFDDirectory.TagSaturation:
				{
					return GetSaturationDescription();
				}

                case ExifSubIFDDirectory.TagSharpness:
				{
					return GetSharpnessDescription();
				}

                case ExifSubIFDDirectory.TagSubjectDistanceRange:
				{
					return GetSubjectDistanceRangeDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetNewSubfileTypeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagNewSubfileType, 1, "Full-resolution image", "Reduced-resolution image", "Single page of multi-page reduced-resolution image", "Transparency mask", "Transparency mask of reduced-resolution image", "Transparency mask of multi-page image"
				, "Transparency mask of reduced-resolution multi-page image");
		}

		[CanBeNull]
		public virtual string GetSubfileTypeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagSubfileType, 1, "Full-resolution image", "Reduced-resolution image", "Single page of multi-page image");
		}

		[CanBeNull]
		public virtual string GetThresholdingDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagThresholding, 1, "No dithering or halftoning", "Ordered dither or halftone", "Randomized dither");
		}

		[CanBeNull]
		public virtual string GetFillOrderDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagFillOrder, 1, "Normal", "Reversed");
		}

		[CanBeNull]
		public virtual string GetSubjectDistanceRangeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagSubjectDistanceRange, "Unknown", "Macro", "Close view", "Distant view");
		}

		[CanBeNull]
		public virtual string GetSharpnessDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagSharpness, "None", "Low", "Hard");
		}

		[CanBeNull]
		public virtual string GetSaturationDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagSaturation, "None", "Low saturation", "High saturation");
		}

		[CanBeNull]
		public virtual string GetContrastDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagContrast, "None", "Soft", "Hard");
		}

		[CanBeNull]
		public virtual string GetGainControlDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagGainControl, "None", "Low gain up", "Low gain down", "High gain up", "High gain down");
		}

		[CanBeNull]
		public virtual string GetSceneCaptureTypeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagSceneCaptureType, "Standard", "Landscape", "Portrait", "Night scene");
		}

		[CanBeNull]
		public virtual string Get35mmFilmEquivFocalLengthDescription()
		{
            int? value = _directory.GetInteger(ExifSubIFDDirectory.Tag35mmFilmEquivFocalLength);
			return value == null ? null : value == 0 ? "Unknown" : SimpleDecimalFormatter.Format(value.Value) + "mm";
		}

		[CanBeNull]
		public virtual string GetDigitalZoomRatioDescription()
		{
            Rational value = _directory.GetRational(ExifSubIFDDirectory.TagDigitalZoomRatio);
			return value == null ? null : value.GetNumerator() == 0 ? "Digital zoom not used." : SimpleDecimalFormatter.Format(value.DoubleValue());
		}

		[CanBeNull]
		public virtual string GetWhiteBalanceModeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagWhiteBalanceMode, "Auto white balance", "Manual white balance");
		}

		[CanBeNull]
		public virtual string GetExposureModeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagExposureMode, "Auto exposure", "Manual exposure", "Auto bracket");
		}

		[CanBeNull]
		public virtual string GetCustomRenderedDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagCustomRendered, "Normal process", "Custom process");
		}

		[CanBeNull]
		public virtual string GetUserCommentDescription()
		{
            sbyte[] commentBytes = _directory.GetByteArray(ExifSubIFDDirectory.TagUserComment);
			if (commentBytes == null)
			{
				return null;
			}
			if (commentBytes.Length == 0)
			{
				return string.Empty;
			}
			IDictionary<string, string> encodingMap = new Dictionary<string, string>();
			encodingMap.Put("ASCII", Runtime.GetProperty("file.encoding"));
			// Someone suggested "ISO-8859-1".
			encodingMap.Put("UNICODE", "UTF-16LE");
			encodingMap.Put("JIS", "Shift-JIS");
			// We assume this charset for now.  Another suggestion is "JIS".
			try
			{
				if (commentBytes.Length >= 10)
				{
					string firstTenBytesString = Sharpen.Runtime.GetStringForBytes(commentBytes, 0, 10);
					// try each encoding name
					foreach (KeyValuePair<string, string> pair in encodingMap.EntrySet())
					{
						string encodingName = pair.Key;
						string charset = pair.Value;
						if (firstTenBytesString.StartsWith(encodingName))
						{
							// skip any null or blank characters commonly present after the encoding name, up to a limit of 10 from the start
							for (int j = encodingName.Length; j < 10; j++)
							{
								sbyte b = commentBytes[j];
								if (b != '\0' && b != ' ')
								{
									return Sharpen.Extensions.Trim(Sharpen.Runtime.GetStringForBytes(commentBytes, j, commentBytes.Length - j, charset));
								}
							}
							return Sharpen.Extensions.Trim(Sharpen.Runtime.GetStringForBytes(commentBytes, 10, commentBytes.Length - 10, charset));
						}
					}
				}
				// special handling fell through, return a plain string representation
				return Sharpen.Extensions.Trim(Sharpen.Runtime.GetStringForBytes(commentBytes, Runtime.GetProperty("file.encoding")));
			}
			catch (UnsupportedEncodingException)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetIsoEquivalentDescription()
		{
			// Have seen an exception here from files produced by ACDSEE that stored an int[] here with two values
            int? isoEquiv = _directory.GetInteger(ExifSubIFDDirectory.TagIsoEquivalent);
			// There used to be a check here that multiplied ISO values < 50 by 200.
			// Issue 36 shows a smart-phone image from a Samsung Galaxy S2 with ISO-40.
			return isoEquiv != null ? Sharpen.Extensions.ToString(isoEquiv) : null;
		}

		[CanBeNull]
		public virtual string GetExifVersionDescription()
		{
            return GetVersionBytesDescription(ExifSubIFDDirectory.TagExifVersion, 2);
		}

		[CanBeNull]
		public virtual string GetFlashPixVersionDescription()
		{
            return GetVersionBytesDescription(ExifSubIFDDirectory.TagFlashpixVersion, 2);
		}

		[CanBeNull]
		public virtual string GetSceneTypeDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagSceneType, 1, "Directly photographed image");
		}

		[CanBeNull]
		public virtual string GetFileSourceDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagFileSource, 1, "Film Scanner", "Reflection Print Scanner", "Digital Still Camera (DSC)");
		}

		[CanBeNull]
		public virtual string GetExposureBiasDescription()
		{
            Rational value = _directory.GetRational(ExifSubIFDDirectory.TagExposureBias);
			if (value == null)
			{
				return null;
			}
			return value.ToSimpleString(true) + " EV";
		}

		[CanBeNull]
		public virtual string GetMaxApertureValueDescription()
		{
            double? aperture = _directory.GetDoubleObject(ExifSubIFDDirectory.TagMaxAperture);
			if (aperture == null)
			{
				return null;
			}
			double fStop = PhotographicConversions.ApertureToFStop(aperture.Value);
			return "F" + SimpleDecimalFormatter.Format(fStop);
		}

		[CanBeNull]
		public virtual string GetApertureValueDescription()
		{
            double? aperture = _directory.GetDoubleObject(ExifSubIFDDirectory.TagAperture);
			if (aperture == null)
			{
				return null;
			}
			double fStop = PhotographicConversions.ApertureToFStop(aperture.Value);
			return "F" + SimpleDecimalFormatter.Format(fStop);
		}

		[CanBeNull]
		public virtual string GetExposureProgramDescription()
		{
            return GetIndexedDescription(ExifSubIFDDirectory.TagExposureProgram, 1, "Manual control", "Program normal", "Aperture priority", "Shutter priority", "Program creative (slow program)", "Program action (high-speed program)"
				, "Portrait mode", "Landscape mode");
		}

		[CanBeNull]
		public virtual string GetYCbCrSubsamplingDescription()
		{
            int[] positions = _directory.GetIntArray(ExifSubIFDDirectory.TagYcbcrSubsampling);
			if (positions == null)
			{
				return null;
			}
			if (positions[0] == 2 && positions[1] == 1)
			{
				return "YCbCr4:2:2";
			}
			else
			{
				if (positions[0] == 2 && positions[1] == 2)
				{
					return "YCbCr4:2:0";
				}
				else
				{
					return "(Unknown)";
				}
			}
		}

		[CanBeNull]
		public virtual string GetPlanarConfigurationDescription()
		{
			// When image format is no compression YCbCr, this value shows byte aligns of YCbCr
			// data. If value is '1', Y/Cb/Cr value is chunky format, contiguous for each subsampling
			// pixel. If value is '2', Y/Cb/Cr value is separated and stored to Y plane/Cb plane/Cr
			// plane format.
            return GetIndexedDescription(ExifSubIFDDirectory.TagPlanarConfiguration, 1, "Chunky (contiguous for each subsampling pixel)", "Separate (Y-plane/Cb-plane/Cr-plane format)");
		}

		[CanBeNull]
		public virtual string GetSamplesPerPixelDescription()
		{
            string value = _directory.GetString(ExifSubIFDDirectory.TagSamplesPerPixel);
			return value == null ? null : value + " samples/pixel";
		}

		[CanBeNull]
		public virtual string GetRowsPerStripDescription()
		{
            string value = _directory.GetString(ExifSubIFDDirectory.TagRowsPerStrip);
			return value == null ? null : value + " rows/strip";
		}

		[CanBeNull]
		public virtual string GetStripByteCountsDescription()
		{
            string value = _directory.GetString(ExifSubIFDDirectory.TagStripByteCounts);
			return value == null ? null : value + " bytes";
		}

		[CanBeNull]
		public virtual string GetPhotometricInterpretationDescription()
		{
			// Shows the color space of the image data components
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagPhotometricInterpretation);
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
            string value = _directory.GetString(ExifSubIFDDirectory.TagBitsPerSample);
			return value == null ? null : value + " bits/component/pixel";
		}

		[CanBeNull]
		public virtual string GetFocalPlaneXResolutionDescription()
		{
            Rational rational = _directory.GetRational(ExifSubIFDDirectory.TagFocalPlaneXResolution);
			if (rational == null)
			{
				return null;
			}
			string unit = GetFocalPlaneResolutionUnitDescription();
			return rational.GetReciprocal().ToSimpleString(_allowDecimalRepresentationOfRationals) + (unit == null ? string.Empty : " " + unit.ToLower());
		}

		[CanBeNull]
		public virtual string GetFocalPlaneYResolutionDescription()
		{
            Rational rational = _directory.GetRational(ExifSubIFDDirectory.TagFocalPlaneYResolution);
			if (rational == null)
			{
				return null;
			}
			string unit = GetFocalPlaneResolutionUnitDescription();
			return rational.GetReciprocal().ToSimpleString(_allowDecimalRepresentationOfRationals) + (unit == null ? string.Empty : " " + unit.ToLower());
		}

		[CanBeNull]
		public virtual string GetFocalPlaneResolutionUnitDescription()
		{
			// Unit of FocalPlaneXResolution/FocalPlaneYResolution.
			// '1' means no-unit, '2' inch, '3' centimeter.
            return GetIndexedDescription(ExifSubIFDDirectory.TagFocalPlaneResolutionUnit, 1, "(No unit)", "Inches", "cm");
		}

		[CanBeNull]
		public virtual string GetExifImageWidthDescription()
		{
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagExifImageWidth);
			return value == null ? null : value + " pixels";
		}

		[CanBeNull]
		public virtual string GetExifImageHeightDescription()
		{
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagExifImageHeight);
			return value == null ? null : value + " pixels";
		}

		[CanBeNull]
		public virtual string GetColorSpaceDescription()
		{
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagColorSpace);
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
            Rational value = _directory.GetRational(ExifSubIFDDirectory.TagFocalLength);
			if (value == null)
			{
				return null;
			}
			DecimalFormat formatter = new DecimalFormat("0.0##");
			return formatter.Format(value.DoubleValue()) + " mm";
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
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagFlash);
			if (value == null)
			{
				return null;
			}
			StringBuilder sb = new StringBuilder();
			if ((value & unchecked((int)(0x1))) != 0)
			{
				sb.Append("Flash fired");
			}
			else
			{
				sb.Append("Flash did not fire");
			}
			// check if we're able to detect a return, before we mention it
			if ((value & unchecked((int)(0x4))) != 0)
			{
				if ((value & unchecked((int)(0x2))) != 0)
				{
					sb.Append(", return detected");
				}
				else
				{
					sb.Append(", return not detected");
				}
			}
			if ((value & unchecked((int)(0x10))) != 0)
			{
				sb.Append(", auto");
			}
			if ((value & unchecked((int)(0x40))) != 0)
			{
				sb.Append(", red-eye reduction");
			}
			return sb.ToString();
		}

		[CanBeNull]
		public virtual string GetWhiteBalanceDescription()
		{
			// '0' means unknown, '1' daylight, '2' fluorescent, '3' tungsten, '10' flash,
			// '17' standard light A, '18' standard light B, '19' standard light C, '20' D55,
			// '21' D65, '22' D75, '255' other.
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagWhiteBalance);
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
            int? value = _directory.GetInteger(ExifSubIFDDirectory.TagMeteringMode);
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
					return string.Empty;
				}
			}
		}

		[CanBeNull]
		public virtual string GetSubjectDistanceDescription()
		{
            Rational value = _directory.GetRational(ExifSubIFDDirectory.TagSubjectDistance);
			if (value == null)
			{
				return null;
			}
			DecimalFormat formatter = new DecimalFormat("0.0##");
			return formatter.Format(value.DoubleValue()) + " metres";
		}

		[CanBeNull]
		public virtual string GetCompressedAverageBitsPerPixelDescription()
		{
            Rational value = _directory.GetRational(ExifSubIFDDirectory.TagCompressedAverageBitsPerPixel);
			if (value == null)
			{
				return null;
			}
			string ratio = value.ToSimpleString(_allowDecimalRepresentationOfRationals);
			return value.IsInteger() && value.IntValue() == 1 ? ratio + " bit/pixel" : ratio + " bits/pixel";
		}

		[CanBeNull]
		public virtual string GetExposureTimeDescription()
		{
            string value = _directory.GetString(ExifSubIFDDirectory.TagExposureTime);
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
            float? apexValue = _directory.GetFloatObject(ExifSubIFDDirectory.TagShutterSpeed);
			if (apexValue == null)
			{
				return null;
			}
			if (apexValue <= 1)
			{
				float apexPower = (float)(1 / (Math.Exp(apexValue.Value * Math.Log(2))));
				long apexPower10 = (long) Math.Round((double)apexPower * 10.0);
				float fApexPower = (float)apexPower10 / 10.0f;
				return fApexPower + " sec";
			}
			else
			{
				int apexPower = (int)((Math.Exp(apexValue.Value * Math.Log(2))));
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
            Rational value = _directory.GetRational(ExifSubIFDDirectory.TagFnumber);
			if (value == null)
			{
				return null;
			}
			return "F" + SimpleDecimalFormatter.Format(value.DoubleValue());
		}

		[CanBeNull]
		public virtual string GetSensingMethodDescription()
		{
			// '1' Not defined, '2' One-chip color area sensor, '3' Two-chip color area sensor
			// '4' Three-chip color area sensor, '5' Color sequential area sensor
			// '7' Trilinear sensor '8' Color sequential linear sensor,  'Other' reserved
            return GetIndexedDescription(ExifSubIFDDirectory.TagSensingMethod, 1, "(Not defined)", "One-chip color area sensor", "Two-chip color area sensor", "Three-chip color area sensor", "Color sequential area sensor", null, "Trilinear sensor", "Color sequential linear sensor"
				);
		}

		[CanBeNull]
		public virtual string GetComponentConfigurationDescription()
		{
            int[] components = _directory.GetIntArray(ExifSubIFDDirectory.TagComponentsConfiguration);
			if (components == null)
			{
				return null;
			}
			string[] componentStrings = new string[] { string.Empty, "Y", "Cb", "Cr", "R", "G", "B" };
			StringBuilder componentConfig = new StringBuilder();
			for (int i = 0; i < Math.Min(4, components.Length); i++)
			{
				int j = components[i];
				if (j > 0 && j < componentStrings.Length)
				{
					componentConfig.Append(componentStrings[j]);
				}
			}
			return componentConfig.ToString();
		}
	}
}
