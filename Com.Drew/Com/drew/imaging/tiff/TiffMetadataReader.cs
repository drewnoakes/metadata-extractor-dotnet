/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
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
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */
using System.IO;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.File;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Tiff
{
	/// <summary>Obtains all available metadata from TIFF formatted files.</summary>
	/// <remarks>
	/// Obtains all available metadata from TIFF formatted files.  Note that TIFF files include many digital camera RAW
	/// formats, including Canon (CRW, CR2), Nikon (NEF), Olympus (ORF) and Panasonic (RW2).
	/// </remarks>
	/// <author>Darren Salomons</author>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class TiffMetadataReader
	{
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Com.Drew.Imaging.Tiff.TiffProcessingException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata([NotNull] FilePath file)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			RandomAccessFile randomAccessFile = new RandomAccessFile(file, "r");
			try
			{
				ExifTiffHandler handler = new ExifTiffHandler(metadata, false);
				new TiffReader().ProcessTiff(new RandomAccessFileReader(randomAccessFile), handler, 0);
			}
			finally
			{
				randomAccessFile.Close();
			}
			new FileMetadataReader().Read(file, metadata);
			return metadata;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Com.Drew.Imaging.Tiff.TiffProcessingException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata([NotNull] InputStream inputStream)
		{
			// TIFF processing requires random access, as directories can be scattered throughout the byte sequence.
			// InputStream does not support seeking backwards, so we wrap it with RandomAccessStreamReader, which
			// buffers data from the stream as we seek forward.
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			ExifTiffHandler handler = new ExifTiffHandler(metadata, false);
			new TiffReader().ProcessTiff(new RandomAccessStreamReader(inputStream), handler, 0);
			return metadata;
		}
	}
}
