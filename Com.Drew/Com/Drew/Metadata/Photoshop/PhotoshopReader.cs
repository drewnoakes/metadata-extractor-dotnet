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
using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using Com.Drew.Metadata.Iptc;
using Com.Drew.Metadata.Photoshop;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
	/// <summary>Reads metadata created by Photoshop and stored in the APPD segment of JPEG files.</summary>
	/// <remarks>
	/// Reads metadata created by Photoshop and stored in the APPD segment of JPEG files.
	/// Note that IPTC data may be stored within this segment, in which case this reader will
	/// create both a
	/// <see cref="PhotoshopDirectory"/>
	/// and a
	/// <see cref="Com.Drew.Metadata.Iptc.IptcDirectory"/>
	/// .
	/// </remarks>
	/// <author>Yuri Binev, Drew Noakes http://drewnoakes.com</author>
	public class PhotoshopReader : JpegSegmentMetadataReader
	{
		[NotNull]
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			return Arrays.AsList(JpegSegmentType.Appd).AsIterable();
		}

		public virtual bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType)
		{
			return segmentBytes.Length > 12 && "Photoshop 3.0".Equals(Sharpen.Runtime.GetStringForBytes(segmentBytes, 0, 13));
		}

		public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
		{
			Extract(new ByteArrayReader(segmentBytes), metadata);
		}

		public virtual void Extract(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata)
		{
			PhotoshopDirectory directory = metadata.GetOrCreateDirectory<PhotoshopDirectory>();
			int pos;
			try
			{
				pos = reader.GetString(0, 13).Equals("Photoshop 3.0") ? 14 : 0;
			}
			catch (IOException)
			{
				directory.AddError("Unable to read header");
				return;
			}
			long length;
			try
			{
				length = reader.GetLength();
			}
			catch (IOException e)
			{
				directory.AddError("Unable to read Photoshop data: " + e.Message);
				return;
			}
			while (pos < length)
			{
				try
				{
					// 4 bytes for the signature.  Should always be "8BIM".
					//String signature = new String(data, pos, 4);
					pos += 4;
					// 2 bytes for the resource identifier (tag type).
					int tagType = reader.GetUInt16(pos);
					// segment type
					pos += 2;
					// A variable number of bytes holding a pascal string (two leading bytes for length).
					int descriptionLength = reader.GetUInt16(pos);
					pos += 2;
					// Some basic bounds checking
					if (descriptionLength < 0 || descriptionLength + pos > length)
					{
						return;
					}
					//String description = new String(data, pos, descriptionLength);
					pos += descriptionLength;
					// The number of bytes is padded with a trailing zero, if needed, to make the size even.
					if (pos % 2 != 0)
					{
						pos++;
					}
					// 4 bytes for the size of the resource data that follows.
					int byteCount = reader.GetInt32(pos);
					pos += 4;
					// The resource data.
					sbyte[] tagBytes = reader.GetBytes(pos, byteCount);
					pos += byteCount;
					// The number of bytes is padded with a trailing zero, if needed, to make the size even.
					if (pos % 2 != 0)
					{
						pos++;
					}
					directory.SetByteArray(tagType, tagBytes);
					// TODO allow rebasing the reader with a new zero-point, rather than copying data here
					if (tagType == PhotoshopDirectory.TagIptc)
					{
						new IptcReader().Extract(new SequentialByteArrayReader(tagBytes), metadata, tagBytes.Length);
					}
					if (tagType >= unchecked((int)(0x0fa0)) && tagType <= unchecked((int)(0x1387)))
					{
						PhotoshopDirectory._tagNameMap.Put(tagType, Sharpen.Extensions.StringFormat("Plug-in %d Data", tagType - unchecked((int)(0x0fa0)) + 1));
					}
				}
				catch (IOException ex)
				{
					directory.AddError(ex.Message);
					return;
				}
			}
		}
	}
}
