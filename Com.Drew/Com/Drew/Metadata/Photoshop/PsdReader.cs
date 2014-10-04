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
using Com.Drew.Metadata.Photoshop;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
	/// <summary>Reads metadata stored within PSD file format data.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PsdReader : MetadataReader
	{
		public virtual void Extract(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata)
		{
			PsdHeaderDirectory directory = metadata.GetOrCreateDirectory<PsdHeaderDirectory>();
			try
			{
				int signature = reader.GetInt32(0);
				if (signature != unchecked((int)(0x38425053)))
				{
					directory.AddError("Invalid PSD file signature");
					return;
				}
				int version = reader.GetUInt16(4);
				if (version != 1 && version != 2)
				{
					directory.AddError("Invalid PSD file version (must be 1 or 2)");
					return;
				}
				// 6 reserved bytes are skipped here.  They should be zero.
				int channelCount = reader.GetUInt16(12);
				directory.SetInt(PsdHeaderDirectory.TagChannelCount, channelCount);
				// even though this is probably an unsigned int, the max height in practice is 300,000
				int imageHeight = reader.GetInt32(14);
				directory.SetInt(PsdHeaderDirectory.TagImageHeight, imageHeight);
				// even though this is probably an unsigned int, the max width in practice is 300,000
				int imageWidth = reader.GetInt32(18);
				directory.SetInt(PsdHeaderDirectory.TagImageWidth, imageWidth);
				int bitsPerChannel = reader.GetUInt16(22);
				directory.SetInt(PsdHeaderDirectory.TagBitsPerChannel, bitsPerChannel);
				int colorMode = reader.GetUInt16(24);
				directory.SetInt(PsdHeaderDirectory.TagColorMode, colorMode);
			}
			catch (IOException)
			{
				directory.AddError("Unable to read PSD header");
			}
		}
	}
}
