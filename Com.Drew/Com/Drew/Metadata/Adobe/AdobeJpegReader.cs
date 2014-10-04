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
using Com.Drew.Metadata.Adobe;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Adobe
{
	/// <summary>Decodes Adobe formatted data stored in JPEG files, normally in the APPE (App14) segment.</summary>
	/// <author>Philip, Drew Noakes http://drewnoakes.com</author>
	public class AdobeJpegReader : JpegSegmentMetadataReader
	{
		[NotNull]
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			return Arrays.AsList(JpegSegmentType.Appe);
		}

		public virtual bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType)
		{
			return segmentBytes.Length == 12 && Sharpen.Runtime.EqualsIgnoreCase("Adobe", Sharpen.Runtime.GetStringForBytes(segmentBytes, 0, 5));
		}

		public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
		{
			Extract(new SequentialByteArrayReader(segmentBytes), metadata);
		}

		public virtual void Extract(SequentialReader reader, Com.Drew.Metadata.Metadata metadata)
		{
			Com.Drew.Metadata.Directory directory = metadata.GetOrCreateDirectory<AdobeJpegDirectory>();
			try
			{
				reader.SetMotorolaByteOrder(false);
				if (!reader.GetString(5).Equals("Adobe"))
				{
					directory.AddError("Invalid Adobe JPEG data header.");
					return;
				}
				directory.SetInt(AdobeJpegDirectory.TagDctEncodeVersion, reader.GetUInt16());
				directory.SetInt(AdobeJpegDirectory.TagApp14Flags0, reader.GetUInt16());
				directory.SetInt(AdobeJpegDirectory.TagApp14Flags1, reader.GetUInt16());
				directory.SetInt(AdobeJpegDirectory.TagColorTransform, reader.GetInt8());
			}
			catch (IOException ex)
			{
				directory.AddError("IO exception processing data: " + ex.Message);
			}
		}
	}
}
