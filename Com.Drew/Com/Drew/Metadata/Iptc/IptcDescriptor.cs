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
using Com.Drew.Metadata.Iptc;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Iptc
{
	/// <summary>Provides human-readable string representations of tag values stored in a <code>IptcDirectory</code>.</summary>
	/// <remarks>
	/// Provides human-readable string representations of tag values stored in a <code>IptcDirectory</code>.
	/// <p/>
	/// As the IPTC directory already stores values as strings, this class simply returns the tag's value.
	/// </remarks>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class IptcDescriptor : TagDescriptor<IptcDirectory>
	{
		public IptcDescriptor(IptcDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case IptcDirectory.TagFileFormat:
				{
					return GetFileFormatDescription();
				}

				case IptcDirectory.TagKeywords:
				{
					return GetKeywordsDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetFileFormatDescription()
		{
			int? value = _directory.GetInteger(IptcDirectory.TagFileFormat);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "No ObjectData";
				}

				case 1:
				{
					return "IPTC-NAA Digital Newsphoto Parameter Record";
				}

				case 2:
				{
					return "IPTC7901 Recommended Message Format";
				}

				case 3:
				{
					return "Tagged Image File Format (Adobe/Aldus Image data)";
				}

				case 4:
				{
					return "Illustrator (Adobe Graphics data)";
				}

				case 5:
				{
					return "AppleSingle (Apple Computer Inc)";
				}

				case 6:
				{
					return "NAA 89-3 (ANPA 1312)";
				}

				case 7:
				{
					return "MacBinary II";
				}

				case 8:
				{
					return "IPTC Unstructured Character Oriented File Format (UCOFF)";
				}

				case 9:
				{
					return "United Press International ANPA 1312 variant";
				}

				case 10:
				{
					return "United Press International Down-Load Message";
				}

				case 11:
				{
					return "JPEG File Interchange (JFIF)";
				}

				case 12:
				{
					return "Photo-CD Image-Pac (Eastman Kodak)";
				}

				case 13:
				{
					return "Bit Mapped Graphics File [.BMP] (Microsoft)";
				}

				case 14:
				{
					return "Digital Audio File [.WAV] (Microsoft & Creative Labs)";
				}

				case 15:
				{
					return "Audio plus Moving Video [.AVI] (Microsoft)";
				}

				case 16:
				{
					return "PC DOS/Windows Executable Files [.COM][.EXE]";
				}

				case 17:
				{
					return "Compressed Binary File [.ZIP] (PKWare Inc)";
				}

				case 18:
				{
					return "Audio Interchange File Format AIFF (Apple Computer Inc)";
				}

				case 19:
				{
					return "RIFF Wave (Microsoft Corporation)";
				}

				case 20:
				{
					return "Freehand (Macromedia/Aldus)";
				}

				case 21:
				{
					return "Hypertext Markup Language [.HTML] (The Internet Society)";
				}

				case 22:
				{
					return "MPEG 2 Audio Layer 2 (Musicom), ISO/IEC";
				}

				case 23:
				{
					return "MPEG 2 Audio Layer 3, ISO/IEC";
				}

				case 24:
				{
					return "Portable Document File [.PDF] Adobe";
				}

				case 25:
				{
					return "News Industry Text Format (NITF)";
				}

				case 26:
				{
					return "Tape Archive [.TAR]";
				}

				case 27:
				{
					return "Tidningarnas Telegrambyra NITF version (TTNITF DTD)";
				}

				case 28:
				{
					return "Ritzaus Bureau NITF version (RBNITF DTD)";
				}

				case 29:
				{
					return "Corel Draw [.CDR]";
				}
			}
			return Sharpen.Extensions.StringFormat("Unknown (%d)", value);
		}

		[CanBeNull]
		public virtual string GetByLineDescription()
		{
			return _directory.GetString(IptcDirectory.TagByLine);
		}

		[CanBeNull]
		public virtual string GetByLineTitleDescription()
		{
			return _directory.GetString(IptcDirectory.TagByLineTitle);
		}

		[CanBeNull]
		public virtual string GetCaptionDescription()
		{
			return _directory.GetString(IptcDirectory.TagCaption);
		}

		[CanBeNull]
		public virtual string GetCategoryDescription()
		{
			return _directory.GetString(IptcDirectory.TagCategory);
		}

		[CanBeNull]
		public virtual string GetCityDescription()
		{
			return _directory.GetString(IptcDirectory.TagCity);
		}

		[CanBeNull]
		public virtual string GetCopyrightNoticeDescription()
		{
			return _directory.GetString(IptcDirectory.TagCopyrightNotice);
		}

		[CanBeNull]
		public virtual string GetCountryOrPrimaryLocationDescription()
		{
			return _directory.GetString(IptcDirectory.TagCountryOrPrimaryLocationName);
		}

		[CanBeNull]
		public virtual string GetCreditDescription()
		{
			return _directory.GetString(IptcDirectory.TagCredit);
		}

		[CanBeNull]
		public virtual string GetDateCreatedDescription()
		{
			return _directory.GetString(IptcDirectory.TagDateCreated);
		}

		[CanBeNull]
		public virtual string GetHeadlineDescription()
		{
			return _directory.GetString(IptcDirectory.TagHeadline);
		}

		[CanBeNull]
		public virtual string GetKeywordsDescription()
		{
			string[] keywords = _directory.GetStringArray(IptcDirectory.TagKeywords);
			if (keywords == null)
			{
				return null;
			}
            return string.Join(";", keywords);
		}

		[CanBeNull]
		public virtual string GetObjectNameDescription()
		{
			return _directory.GetString(IptcDirectory.TagObjectName);
		}

		[CanBeNull]
		public virtual string GetOriginalTransmissionReferenceDescription()
		{
			return _directory.GetString(IptcDirectory.TagOriginalTransmissionReference);
		}

		[CanBeNull]
		public virtual string GetOriginatingProgramDescription()
		{
			return _directory.GetString(IptcDirectory.TagOriginatingProgram);
		}

		[CanBeNull]
		public virtual string GetProvinceOrStateDescription()
		{
			return _directory.GetString(IptcDirectory.TagProvinceOrState);
		}

		[CanBeNull]
		public virtual string GetRecordVersionDescription()
		{
			return _directory.GetString(IptcDirectory.TagApplicationRecordVersion);
		}

		[CanBeNull]
		public virtual string GetReleaseDateDescription()
		{
			return _directory.GetString(IptcDirectory.TagReleaseDate);
		}

		[CanBeNull]
		public virtual string GetReleaseTimeDescription()
		{
			return _directory.GetString(IptcDirectory.TagReleaseTime);
		}

		[CanBeNull]
		public virtual string GetSourceDescription()
		{
			return _directory.GetString(IptcDirectory.TagSource);
		}

		[CanBeNull]
		public virtual string GetSpecialInstructionsDescription()
		{
			return _directory.GetString(IptcDirectory.TagSpecialInstructions);
		}

		[CanBeNull]
		public virtual string GetSupplementalCategoriesDescription()
		{
			return _directory.GetString(IptcDirectory.TagSupplementalCategories);
		}

		[CanBeNull]
		public virtual string GetTimeCreatedDescription()
		{
			return _directory.GetString(IptcDirectory.TagTimeCreated);
		}

		[CanBeNull]
		public virtual string GetUrgencyDescription()
		{
			return _directory.GetString(IptcDirectory.TagUrgency);
		}

		[CanBeNull]
		public virtual string GetWriterDescription()
		{
			return _directory.GetString(IptcDirectory.TagCaptionWriter);
		}
	}
}
