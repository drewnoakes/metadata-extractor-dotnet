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
using Com.Drew.Imaging.Psd;
using Com.Drew.Lang;
using Com.Drew.Metadata.Photoshop;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Psd
{
	/// <summary>Obtains metadata from Photoshop's PSD files.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PsdMetadataReader
	{
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(FilePath file)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			RandomAccessFile randomAccessFile = new RandomAccessFile(file, "r");
			try
			{
				new PsdReader().Extract(new RandomAccessFileReader(randomAccessFile), metadata);
			}
			finally
			{
				randomAccessFile.Close();
			}
			return metadata;
		}

		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(InputStream inputStream)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new PsdReader().Extract(new RandomAccessStreamReader(inputStream), metadata);
			return metadata;
		}
	}
}
