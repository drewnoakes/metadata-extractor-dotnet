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
using System.Collections.Generic;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>Describes Exif tags from the IFD0 directory.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifIFD0Directory : Com.Drew.Metadata.Directory
	{
		public const int TagImageDescription = unchecked((int)(0x010E));

		public const int TagMake = unchecked((int)(0x010F));

		public const int TagModel = unchecked((int)(0x0110));

		public const int TagOrientation = unchecked((int)(0x0112));

		public const int TagXResolution = unchecked((int)(0x011A));

		public const int TagYResolution = unchecked((int)(0x011B));

		public const int TagResolutionUnit = unchecked((int)(0x0128));

		public const int TagSoftware = unchecked((int)(0x0131));

		public const int TagDatetime = unchecked((int)(0x0132));

		public const int TagArtist = unchecked((int)(0x013B));

		public const int TagWhitePoint = unchecked((int)(0x013E));

		public const int TagPrimaryChromaticities = unchecked((int)(0x013F));

		public const int TagYcbcrCoefficients = unchecked((int)(0x0211));

		public const int TagYcbcrPositioning = unchecked((int)(0x0213));

		public const int TagReferenceBlackWhite = unchecked((int)(0x0214));

		/// <summary>This tag is a pointer to the Exif SubIFD.</summary>
		public const int TagExifSubIfdOffset = unchecked((int)(0x8769));

		/// <summary>This tag is a pointer to the Exif GPS IFD.</summary>
		public const int TagGpsInfoOffset = unchecked((int)(0x8825));

		public const int TagCopyright = unchecked((int)(0x8298));

		/// <summary>Non-standard, but in use.</summary>
		public const int TagTimeZoneOffset = unchecked((int)(0x882a));

		/// <summary>The image title, as used by Windows XP.</summary>
		public const int TagWinTitle = unchecked((int)(0x9C9B));

		/// <summary>The image comment, as used by Windows XP.</summary>
		public const int TagWinComment = unchecked((int)(0x9C9C));

		/// <summary>The image author, as used by Windows XP (called Artist in the Windows shell).</summary>
		public const int TagWinAuthor = unchecked((int)(0x9C9D));

		/// <summary>The image keywords, as used by Windows XP.</summary>
		public const int TagWinKeywords = unchecked((int)(0x9C9E));

		/// <summary>The image subject, as used by Windows XP.</summary>
		public const int TagWinSubject = unchecked((int)(0x9C9F));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static ExifIFD0Directory()
		{
			_tagNameMap.Put(TagImageDescription, "Image Description");
			_tagNameMap.Put(TagMake, "Make");
			_tagNameMap.Put(TagModel, "Model");
			_tagNameMap.Put(TagOrientation, "Orientation");
			_tagNameMap.Put(TagXResolution, "X Resolution");
			_tagNameMap.Put(TagYResolution, "Y Resolution");
			_tagNameMap.Put(TagResolutionUnit, "Resolution Unit");
			_tagNameMap.Put(TagSoftware, "Software");
			_tagNameMap.Put(TagDatetime, "Date/Time");
			_tagNameMap.Put(TagArtist, "Artist");
			_tagNameMap.Put(TagWhitePoint, "White Point");
			_tagNameMap.Put(TagPrimaryChromaticities, "Primary Chromaticities");
			_tagNameMap.Put(TagYcbcrCoefficients, "YCbCr Coefficients");
			_tagNameMap.Put(TagYcbcrPositioning, "YCbCr Positioning");
			_tagNameMap.Put(TagReferenceBlackWhite, "Reference Black/White");
			_tagNameMap.Put(TagCopyright, "Copyright");
			_tagNameMap.Put(TagTimeZoneOffset, "Time Zone Offset");
			_tagNameMap.Put(TagWinAuthor, "Windows XP Author");
			_tagNameMap.Put(TagWinComment, "Windows XP Comment");
			_tagNameMap.Put(TagWinKeywords, "Windows XP Keywords");
			_tagNameMap.Put(TagWinSubject, "Windows XP Subject");
			_tagNameMap.Put(TagWinTitle, "Windows XP Title");
		}

		public ExifIFD0Directory()
		{
			this.SetDescriptor(new ExifIFD0Descriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Exif IFD0";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
