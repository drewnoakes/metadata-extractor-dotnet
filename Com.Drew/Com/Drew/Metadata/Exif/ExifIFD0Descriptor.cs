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
using System.IO;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="ExifIFD0Directory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifIFD0Descriptor : TagDescriptor<ExifIFD0Directory>
	{
		/// <summary>
		/// Dictates whether rational values will be represented in decimal format in instances
		/// where decimal notation is elegant (such as 1/2 -&gt; 0.5, but not 1/3).
		/// </summary>
		private readonly bool _allowDecimalRepresentationOfRationals = true;

		public ExifIFD0Descriptor(ExifIFD0Directory directory)
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
				case TagResolutionUnit:
				{
					return GetResolutionDescription();
				}

				case TagYcbcrPositioning:
				{
					return GetYCbCrPositioningDescription();
				}

				case TagXResolution:
				{
					return GetXResolutionDescription();
				}

				case TagYResolution:
				{
					return GetYResolutionDescription();
				}

				case TagReferenceBlackWhite:
				{
					return GetReferenceBlackWhiteDescription();
				}

				case TagOrientation:
				{
					return GetOrientationDescription();
				}

				case TagWinAuthor:
				{
					return GetWindowsAuthorDescription();
				}

				case TagWinComment:
				{
					return GetWindowsCommentDescription();
				}

				case TagWinKeywords:
				{
					return GetWindowsKeywordsDescription();
				}

				case TagWinSubject:
				{
					return GetWindowsSubjectDescription();
				}

				case TagWinTitle:
				{
					return GetWindowsTitleDescription();
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
			int[] ints = _directory.GetIntArray(TagReferenceBlackWhite);
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
		public virtual string GetYResolutionDescription()
		{
			Rational value = _directory.GetRational(TagYResolution);
			if (value == null)
			{
				return null;
			}
			string unit = GetResolutionDescription();
			return Sharpen.Extensions.StringFormat("%s dots per %s", value.ToSimpleString(_allowDecimalRepresentationOfRationals), unit == null ? "unit" : unit.ToLower());
		}

		[CanBeNull]
		public virtual string GetXResolutionDescription()
		{
			Rational value = _directory.GetRational(TagXResolution);
			if (value == null)
			{
				return null;
			}
			string unit = GetResolutionDescription();
			return Sharpen.Extensions.StringFormat("%s dots per %s", value.ToSimpleString(_allowDecimalRepresentationOfRationals), unit == null ? "unit" : unit.ToLower());
		}

		[CanBeNull]
		public virtual string GetYCbCrPositioningDescription()
		{
			return GetIndexedDescription(TagYcbcrPositioning, 1, "Center of pixel array", "Datum point");
		}

		[CanBeNull]
		public virtual string GetOrientationDescription()
		{
			return GetIndexedDescription(TagOrientation, 1, "Top, left side (Horizontal / normal)", "Top, right side (Mirror horizontal)", "Bottom, right side (Rotate 180)", "Bottom, left side (Mirror vertical)", "Left side, top (Mirror horizontal and rotate 270 CW)"
				, "Right side, top (Rotate 90 CW)", "Right side, bottom (Mirror horizontal and rotate 90 CW)", "Left side, bottom (Rotate 270 CW)");
		}

		[CanBeNull]
		public virtual string GetResolutionDescription()
		{
			// '1' means no-unit, '2' means inch, '3' means centimeter. Default value is '2'(inch)
			return GetIndexedDescription(TagResolutionUnit, 1, "(No unit)", "Inch", "cm");
		}

		/// <summary>The Windows specific tags uses plain Unicode.</summary>
		[CanBeNull]
		private string GetUnicodeDescription(int tag)
		{
			sbyte[] bytes = _directory.GetByteArray(tag);
			if (bytes == null)
			{
				return null;
			}
			try
			{
				// Decode the unicode string and trim the unicode zero "\0" from the end.
				return Sharpen.Extensions.Trim(Sharpen.Runtime.GetStringForBytes(bytes, "UTF-16LE"));
			}
			catch (UnsupportedEncodingException)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetWindowsAuthorDescription()
		{
			return GetUnicodeDescription(TagWinAuthor);
		}

		[CanBeNull]
		public virtual string GetWindowsCommentDescription()
		{
			return GetUnicodeDescription(TagWinComment);
		}

		[CanBeNull]
		public virtual string GetWindowsKeywordsDescription()
		{
			return GetUnicodeDescription(TagWinKeywords);
		}

		[CanBeNull]
		public virtual string GetWindowsTitleDescription()
		{
			return GetUnicodeDescription(TagWinTitle);
		}

		[CanBeNull]
		public virtual string GetWindowsSubjectDescription()
		{
			return GetUnicodeDescription(TagWinSubject);
		}
	}
}
