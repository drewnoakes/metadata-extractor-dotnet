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
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Tiff
{
	/// <summary>Obtains all available metadata from TIFF formatted files.</summary>
	/// <remarks>
	/// Obtains all available metadata from TIFF formatted files.  Note that TIFF files include many digital camera RAW
	/// formats, including Canon (CRW, CR2), Nikon (NEF), Olympus (ORF) and Panasonic (RW2).
	/// </remarks>
	/// <author>Darren Salomons, Drew Noakes http://drewnoakes.com</author>
	public class TiffMetadataReader
	{
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(FilePath file)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			RandomAccessFile randomAccessFile = new RandomAccessFile(file, "r");
			try
			{
				new ExifReader().ExtractTiff(new RandomAccessFileReader(randomAccessFile), metadata);
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
			// TIFF processing requires random access, as directories can be scattered throughout the byte sequence.
			// InputStream does not support seeking backwards, and so is not a viable option for TIFF processing.
			// We use RandomAccessStreamReader, which buffers data from the stream as we seek forward.
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new ExifReader().ExtractTiff(new RandomAccessStreamReader(inputStream), metadata);
			return metadata;
		}
	}
}
