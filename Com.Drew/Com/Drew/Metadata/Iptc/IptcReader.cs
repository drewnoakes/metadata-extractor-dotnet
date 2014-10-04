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
using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using Com.Drew.Metadata.Iptc;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Iptc
{
	/// <summary>
	/// Decodes IPTC binary data, populating a
	/// <see cref="Com.Drew.Metadata.Metadata"/>
	/// object with tag values in an
	/// <see cref="IptcDirectory"/>
	/// .
	/// <p/>
	/// http://www.iptc.org/std/IIM/4.1/specification/IIMV4.1.pdf
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class IptcReader : JpegSegmentMetadataReader
	{
		// TODO consider breaking the IPTC section up into multiple directories and providing segregation of each IPTC directory
		/*
    public static final int DIRECTORY_IPTC = 2;

    public static final int ENVELOPE_RECORD = 1;
    public static final int APPLICATION_RECORD_2 = 2;
    public static final int APPLICATION_RECORD_3 = 3;
    public static final int APPLICATION_RECORD_4 = 4;
    public static final int APPLICATION_RECORD_5 = 5;
    public static final int APPLICATION_RECORD_6 = 6;
    public static final int PRE_DATA_RECORD = 7;
    public static final int DATA_RECORD = 8;
    public static final int POST_DATA_RECORD = 9;
*/
		[NotNull]
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			return Arrays.AsList(JpegSegmentType.Appd);
		}

		public virtual bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType)
		{
			// Check whether the first byte resembles
			return segmentBytes.Length != 0 && segmentBytes[0] == unchecked((int)(0x1c));
		}

		public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
		{
			Extract(new SequentialByteArrayReader(segmentBytes), metadata, segmentBytes.Length);
		}

		/// <summary>
		/// Performs the IPTC data extraction, adding found values to the specified instance of
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// .
		/// </summary>
		public virtual void Extract(SequentialReader reader, Com.Drew.Metadata.Metadata metadata, long length)
		{
			IptcDirectory directory = metadata.GetOrCreateDirectory<IptcDirectory>();
			int offset = 0;
			// for each tag
			while (offset < length)
			{
				// identifies start of a tag
				short startByte;
				try
				{
					startByte = reader.GetUInt8();
					offset++;
				}
				catch (IOException)
				{
					directory.AddError("Unable to read starting byte of IPTC tag");
					return;
				}
				if (startByte != unchecked((int)(0x1c)))
				{
					directory.AddError("Invalid start to IPTC tag");
					return;
				}
				// we need at least five bytes left to read a tag
				if (offset + 5 >= length)
				{
					directory.AddError("Too few bytes remain for a valid IPTC tag");
					return;
				}
				int directoryType;
				int tagType;
				int tagByteCount;
				try
				{
					directoryType = reader.GetUInt8();
					tagType = reader.GetUInt8();
					tagByteCount = reader.GetUInt16();
					offset += 4;
				}
				catch (IOException)
				{
					directory.AddError("IPTC data segment ended mid-way through tag descriptor");
					return;
				}
				if (offset + tagByteCount > length)
				{
					directory.AddError("Data for tag extends beyond end of IPTC segment");
					return;
				}
				try
				{
					ProcessTag(reader, directory, directoryType, tagType, tagByteCount);
				}
				catch (IOException)
				{
					directory.AddError("Error processing IPTC tag");
					return;
				}
				offset += tagByteCount;
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void ProcessTag(SequentialReader reader, Com.Drew.Metadata.Directory directory, int directoryType, int tagType, int tagByteCount)
		{
			int tagIdentifier = tagType | (directoryType << 8);
			string @string = null;
			switch (tagIdentifier)
			{
				case IptcDirectory.TagApplicationRecordVersion:
				{
					// short
					int shortValue = reader.GetUInt16();
					reader.Skip(tagByteCount - 2);
					directory.SetInt(tagIdentifier, shortValue);
					return;
				}

				case IptcDirectory.TagUrgency:
				{
					// byte
					directory.SetInt(tagIdentifier, reader.GetUInt8());
					reader.Skip(tagByteCount - 1);
					return;
				}

				case IptcDirectory.TagReleaseDate:
				case IptcDirectory.TagDateCreated:
				{
					// Date object
					if (tagByteCount >= 8)
					{
						@string = reader.GetString(tagByteCount);
						try
						{
							int year = System.Convert.ToInt32(Sharpen.Runtime.Substring(@string, 0, 4));
							int month = System.Convert.ToInt32(Sharpen.Runtime.Substring(@string, 4, 6)) - 1;
							int day = System.Convert.ToInt32(Sharpen.Runtime.Substring(@string, 6, 8));
							DateTime date = new Sharpen.GregorianCalendar(year, month, day).GetTime();
							directory.SetDate(tagIdentifier, date);
							return;
						}
						catch (FormatException)
						{
						}
					}
					else
					{
						// fall through and we'll process the 'string' value below
						reader.Skip(tagByteCount);
					}
					goto case IptcDirectory.TagReleaseTime;
				}

				case IptcDirectory.TagReleaseTime:
				case IptcDirectory.TagTimeCreated:
				default:
				{
					break;
				}
			}
			// time...
			// fall through
			// If we haven't returned yet, treat it as a string
			// NOTE that there's a chance we've already loaded the value as a string above, but failed to parse the value
			if (@string == null)
			{
				@string = reader.GetString(tagByteCount, Runtime.GetProperty("file.encoding"));
			}
			// "ISO-8859-1"
			if (directory.ContainsTag(tagIdentifier))
			{
				// this fancy string[] business avoids using an ArrayList for performance reasons
				string[] oldStrings = directory.GetStringArray(tagIdentifier);
				string[] newStrings;
				if (oldStrings == null)
				{
					newStrings = new string[1];
				}
				else
				{
					newStrings = new string[oldStrings.Length + 1];
					System.Array.Copy(oldStrings, 0, newStrings, 0, oldStrings.Length);
				}
				newStrings[newStrings.Length - 1] = @string;
				directory.SetStringArray(tagIdentifier, newStrings);
			}
			else
			{
				directory.SetString(tagIdentifier, @string);
			}
		}
	}
}
