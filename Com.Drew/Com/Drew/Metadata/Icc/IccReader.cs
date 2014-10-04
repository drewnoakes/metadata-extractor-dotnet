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
using Com.Drew.Metadata.Icc;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Icc
{
	/// <summary>Reads an ICC profile.</summary>
	/// <remarks>
	/// Reads an ICC profile.
	/// <ul>
	/// <li>http://en.wikipedia.org/wiki/ICC_profile</li>
	/// <li>http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/ICC_Profile.html</li>
	/// </ul>
	/// </remarks>
	/// <author>Yuri Binev</author>
	/// <author>Drew Noakes</author>
	public class IccReader : JpegSegmentMetadataReader
	{
		[NotNull]
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			return Arrays.AsList(JpegSegmentType.App2).AsIterable();
		}

		public virtual bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType)
		{
			return segmentBytes.Length > 10 && Sharpen.Runtime.EqualsIgnoreCase("ICC_PROFILE", Sharpen.Runtime.GetStringForBytes(segmentBytes, 0, 11));
		}

		public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
		{
			// skip the first 14 bytes
			sbyte[] iccProfileBytes = new sbyte[segmentBytes.Length - 14];
			System.Array.Copy(segmentBytes, 14, iccProfileBytes, 0, segmentBytes.Length - 14);
			Extract(new ByteArrayReader(iccProfileBytes), metadata);
		}

		public virtual void Extract(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata)
		{
			// TODO review whether the 'tagPtr' values below really do require ICC processing to work with a RandomAccessReader
			IccDirectory directory = metadata.GetOrCreateDirectory<IccDirectory>();
			try
			{
				directory.SetInt(IccDirectory.TagProfileByteCount, reader.GetInt32(IccDirectory.TagProfileByteCount));
				// For these tags, the int value of the tag is in fact it's offset within the buffer.
				Set4ByteString(directory, IccDirectory.TagCmmType, reader);
				SetInt32(directory, IccDirectory.TagProfileVersion, reader);
				Set4ByteString(directory, IccDirectory.TagProfileClass, reader);
				Set4ByteString(directory, IccDirectory.TagColorSpace, reader);
				Set4ByteString(directory, IccDirectory.TagProfileConnectionSpace, reader);
				SetDate(directory, IccDirectory.TagProfileDatetime, reader);
				Set4ByteString(directory, IccDirectory.TagSignature, reader);
				Set4ByteString(directory, IccDirectory.TagPlatform, reader);
				SetInt32(directory, IccDirectory.TagCmmFlags, reader);
				Set4ByteString(directory, IccDirectory.TagDeviceMake, reader);
				int temp = reader.GetInt32(IccDirectory.TagDeviceModel);
				if (temp != 0)
				{
					if (temp <= unchecked((int)(0x20202020)))
					{
						directory.SetInt(IccDirectory.TagDeviceModel, temp);
					}
					else
					{
						directory.SetString(IccDirectory.TagDeviceModel, GetStringFromInt32(temp));
					}
				}
				SetInt32(directory, IccDirectory.TagRenderingIntent, reader);
				SetInt64(directory, IccDirectory.TagDeviceAttr, reader);
				float[] xyz = new float[] { reader.GetS15Fixed16(IccDirectory.TagXyzValues), reader.GetS15Fixed16(IccDirectory.TagXyzValues + 4), reader.GetS15Fixed16(IccDirectory.TagXyzValues + 8) };
				directory.SetObject(IccDirectory.TagXyzValues, xyz);
				// Process 'ICC tags'
				int tagCount = reader.GetInt32(IccDirectory.TagTagCount);
				directory.SetInt(IccDirectory.TagTagCount, tagCount);
				for (int i = 0; i < tagCount; i++)
				{
					int pos = IccDirectory.TagTagCount + 4 + i * 12;
					int tagType = reader.GetInt32(pos);
					int tagPtr = reader.GetInt32(pos + 4);
					int tagLen = reader.GetInt32(pos + 8);
					sbyte[] b = reader.GetBytes(tagPtr, tagLen);
					directory.SetByteArray(tagType, b);
				}
			}
			catch (IOException ex)
			{
				directory.AddError("Exception reading ICC profile: " + ex.Message);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void Set4ByteString(Com.Drew.Metadata.Directory directory, int tagType, RandomAccessReader reader)
		{
			int i = reader.GetInt32(tagType);
			if (i != 0)
			{
				directory.SetString(tagType, GetStringFromInt32(i));
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void SetInt32(Com.Drew.Metadata.Directory directory, int tagType, RandomAccessReader reader)
		{
			int i = reader.GetInt32(tagType);
			if (i != 0)
			{
				directory.SetInt(tagType, i);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void SetInt64(Com.Drew.Metadata.Directory directory, int tagType, RandomAccessReader reader)
		{
			long l = reader.GetInt64(tagType);
			if (l != 0)
			{
				directory.SetLong(tagType, l);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void SetDate(IccDirectory directory, int tagType, RandomAccessReader reader)
		{
			int y = reader.GetUInt16(tagType);
			int m = reader.GetUInt16(tagType + 2);
			int d = reader.GetUInt16(tagType + 4);
			int h = reader.GetUInt16(tagType + 6);
			int M = reader.GetUInt16(tagType + 8);
			int s = reader.GetUInt16(tagType + 10);
			//        final Date value = new Date(Date.UTC(y - 1900, m - 1, d, h, M, s));
			Sharpen.Calendar calendar = Sharpen.Calendar.GetInstance(Sharpen.Extensions.GetTimeZone("UTC"));
			calendar.Set(y, m, d, h, M, s);
			DateTime value = calendar.GetTime();
			directory.SetDate(tagType, value);
		}

		[NotNull]
		public static string GetStringFromInt32(int d)
		{
			// MSB
			sbyte[] b = new sbyte[] { unchecked((sbyte)((d & unchecked((int)(0xFF000000))) >> 24)), unchecked((sbyte)((d & unchecked((int)(0x00FF0000))) >> 16)), unchecked((sbyte)((d & unchecked((int)(0x0000FF00))
				) >> 8)), unchecked((sbyte)((d & unchecked((int)(0x000000FF))))) };
			return Sharpen.Runtime.GetStringForBytes(b);
		}
	}
}
