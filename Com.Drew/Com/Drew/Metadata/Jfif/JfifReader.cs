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
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using Com.Drew.Metadata.Jfif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Jfif
{
	/// <summary>Reader for JFIF data, found in the APP0 JPEG segment.</summary>
	/// <remarks>
	/// Reader for JFIF data, found in the APP0 JPEG segment.
	/// <p/>
	/// More info at: http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format
	/// </remarks>
	/// <author>Yuri Binev, Drew Noakes, Markus Meyer</author>
	public class JfifReader : JpegSegmentMetadataReader
	{
		[NotNull]
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			return Arrays.AsList(JpegSegmentType.App0);
		}

		public virtual bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType)
		{
			return segmentBytes.Length > 3 && "JFIF".Equals(Sharpen.Runtime.GetStringForBytes(segmentBytes, 0, 4));
		}

		public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
		{
			Extract(new ByteArrayReader(segmentBytes), metadata);
		}

		/// <summary>
		/// Performs the Jfif data extraction, adding found values to the specified
		/// instance of
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// .
		/// </summary>
		public virtual void Extract(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata)
		{
			JfifDirectory directory = metadata.GetOrCreateDirectory<JfifDirectory>();
			try
			{
				// For JFIF, the tag number is also the offset into the segment
				int ver = reader.GetUInt16(JfifDirectory.TagVersion);
				directory.SetInt(JfifDirectory.TagVersion, ver);
				int units = reader.GetUInt8(JfifDirectory.TagUnits);
				directory.SetInt(JfifDirectory.TagUnits, units);
				int height = reader.GetUInt16(JfifDirectory.TagResx);
				directory.SetInt(JfifDirectory.TagResx, height);
				int width = reader.GetUInt16(JfifDirectory.TagResy);
				directory.SetInt(JfifDirectory.TagResy, width);
			}
			catch (IOException me)
			{
				directory.AddError(me.Message);
			}
		}
	}
}
