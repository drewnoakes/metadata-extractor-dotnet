/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
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
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="ExifThumbnailDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifThumbnailDescriptor : TagDescriptor<ExifThumbnailDirectory>
	{
		/// <summary>
		/// Dictates whether rational values will be represented in decimal format in instances
		/// where decimal notation is elegant (such as 1/2 -&gt; 0.5, but not 1/3).
		/// </summary>
		private readonly bool _allowDecimalRepresentationOfRationals = true;

		public ExifThumbnailDescriptor(ExifThumbnailDirectory directory)
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
				case ExifThumbnailDirectory.TagOrientation:
				{
					return GetOrientationDescription();
				}

                case ExifThumbnailDirectory.TagResolutionUnit:
				{
					return GetResolutionDescription();
				}

                case ExifThumbnailDirectory.TagYcbcrPositioning:
				{
					return GetYCbCrPositioningDescription();
				}

                case ExifThumbnailDirectory.TagXResolution:
				{
					return GetXResolutionDescription();
				}

                case ExifThumbnailDirectory.TagYResolution:
				{
					return GetYResolutionDescription();
				}

                case ExifThumbnailDirectory.TagThumbnailOffset:
				{
					return GetThumbnailOffsetDescription();
				}

                case ExifThumbnailDirectory.TagThumbnailLength:
				{
					return GetThumbnailLengthDescription();
				}

                case ExifThumbnailDirectory.TagThumbnailImageWidth:
				{
					return GetThumbnailImageWidthDescription();
				}

                case ExifThumbnailDirectory.TagThumbnailImageHeight:
				{
					return GetThumbnailImageHeightDescription();
				}

                case ExifThumbnailDirectory.TagBitsPerSample:
				{
					return GetBitsPerSampleDescription();
				}

                case ExifThumbnailDirectory.TagThumbnailCompression:
				{
					return GetCompressionDescription();
				}

                case ExifThumbnailDirectory.TagPhotometricInterpretation:
				{
					return GetPhotometricInterpretationDescription();
				}

                case ExifThumbnailDirectory.TagRowsPerStrip:
				{
					return GetRowsPerStripDescription();
				}

                case ExifThumbnailDirectory.TagStripByteCounts:
				{
					return GetStripByteCountsDescription();
				}

                case ExifThumbnailDirectory.TagSamplesPerPixel:
				{
					return GetSamplesPerPixelDescription();
				}

                case ExifThumbnailDirectory.TagPlanarConfiguration:
				{
					return GetPlanarConfigurationDescription();
				}

                case ExifThumbnailDirectory.TagYcbcrSubsampling:
				{
					return GetYCbCrSubsamplingDescription();
				}

                case ExifThumbnailDirectory.TagReferenceBlackWhite:
				{
					return GetReferenceBlackWhiteDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetReferenceBlackWhiteDescription()
		{
            int[] ints = _directory.GetIntArray(ExifThumbnailDirectory.TagReferenceBlackWhite);
			if (ints == null || ints.Length < 6)
			{
				return null;
			}
			int blackR = ints[0];
			int whiteR = ints[1];
			int blackG = ints[2];
			int whiteG = ints[3];
			int blackB = ints[4];
			int whiteB = ints[5];
			return Sharpen.Extensions.StringFormat("[%d,%d,%d] [%d,%d,%d]", blackR, blackG, blackB, whiteR, whiteG, whiteB);
		}

		[CanBeNull]
		public virtual string GetYCbCrSubsamplingDescription()
		{
            int[] positions = _directory.GetIntArray(ExifThumbnailDirectory.TagYcbcrSubsampling);
			if (positions == null || positions.Length < 2)
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
            return GetIndexedDescription(ExifThumbnailDirectory.TagPlanarConfiguration, 1, "Chunky (contiguous for each subsampling pixel)", "Separate (Y-plane/Cb-plane/Cr-plane format)");
		}

		[CanBeNull]
		public virtual string GetSamplesPerPixelDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagSamplesPerPixel);
			return value == null ? null : value + " samples/pixel";
		}

		[CanBeNull]
		public virtual string GetRowsPerStripDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagRowsPerStrip);
			return value == null ? null : value + " rows/strip";
		}

		[CanBeNull]
		public virtual string GetStripByteCountsDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagStripByteCounts);
			return value == null ? null : value + " bytes";
		}

		[CanBeNull]
		public virtual string GetPhotometricInterpretationDescription()
		{
			// Shows the color space of the image data components
            int? value = _directory.GetInteger(ExifThumbnailDirectory.TagPhotometricInterpretation);
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
		public virtual string GetCompressionDescription()
		{
            int? value = _directory.GetInteger(ExifThumbnailDirectory.TagThumbnailCompression);
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

				case 32766:
				{
					return "Next";
				}

				case 32771:
				{
					return "CCIRLEW";
				}

				case 32773:
				{
					return "PackBits";
				}

				case 32809:
				{
					return "Thunderscan";
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

				case 32661:
				{
					return "JBIG";
				}

				case 32676:
				{
					return "SGILog";
				}

				case 32677:
				{
					return "SGILog24";
				}

				case 32712:
				{
					return "JPEG 2000";
				}

				case 32713:
				{
					return "Nikon NEF Compressed";
				}

				default:
				{
					return "Unknown compression";
				}
			}
		}

		[CanBeNull]
		public virtual string GetBitsPerSampleDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagBitsPerSample);
			return value == null ? null : value + " bits/component/pixel";
		}

		[CanBeNull]
		public virtual string GetThumbnailImageWidthDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagThumbnailImageWidth);
			return value == null ? null : value + " pixels";
		}

		[CanBeNull]
		public virtual string GetThumbnailImageHeightDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagThumbnailImageHeight);
			return value == null ? null : value + " pixels";
		}

		[CanBeNull]
		public virtual string GetThumbnailLengthDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagThumbnailLength);
			return value == null ? null : value + " bytes";
		}

		[CanBeNull]
		public virtual string GetThumbnailOffsetDescription()
		{
            string value = _directory.GetString(ExifThumbnailDirectory.TagThumbnailOffset);
			return value == null ? null : value + " bytes";
		}

		[CanBeNull]
		public virtual string GetYResolutionDescription()
		{
            Rational value = _directory.GetRational(ExifThumbnailDirectory.TagYResolution);
			if (value == null)
			{
				return null;
			}
			string unit = GetResolutionDescription();
			return value.ToSimpleString(_allowDecimalRepresentationOfRationals) + " dots per " + (unit == null ? "unit" : unit.ToLower());
		}

		[CanBeNull]
		public virtual string GetXResolutionDescription()
		{
            Rational value = _directory.GetRational(ExifThumbnailDirectory.TagXResolution);
			if (value == null)
			{
				return null;
			}
			string unit = GetResolutionDescription();
			return value.ToSimpleString(_allowDecimalRepresentationOfRationals) + " dots per " + (unit == null ? "unit" : unit.ToLower());
		}

		[CanBeNull]
		public virtual string GetYCbCrPositioningDescription()
		{
            return GetIndexedDescription(ExifThumbnailDirectory.TagYcbcrPositioning, 1, "Center of pixel array", "Datum point");
		}

		[CanBeNull]
		public virtual string GetOrientationDescription()
		{
            return GetIndexedDescription(ExifThumbnailDirectory.TagOrientation, 1, "Top, left side (Horizontal / normal)", "Top, right side (Mirror horizontal)", "Bottom, right side (Rotate 180)", "Bottom, left side (Mirror vertical)", 
				"Left side, top (Mirror horizontal and rotate 270 CW)", "Right side, top (Rotate 90 CW)", "Right side, bottom (Mirror horizontal and rotate 90 CW)", "Left side, bottom (Rotate 270 CW)");
		}

		[CanBeNull]
		public virtual string GetResolutionDescription()
		{
			// '1' means no-unit, '2' means inch, '3' means centimeter. Default value is '2'(inch)
            return GetIndexedDescription(ExifThumbnailDirectory.TagResolutionUnit, 1, "(No unit)", "Inch", "cm");
		}
	}
}
