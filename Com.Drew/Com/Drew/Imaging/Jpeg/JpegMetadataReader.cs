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
using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Metadata.Adobe;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Icc;
using Com.Drew.Metadata.Iptc;
using Com.Drew.Metadata.Jfif;
using Com.Drew.Metadata.Jpeg;
using Com.Drew.Metadata.Photoshop;
using Com.Drew.Metadata.Xmp;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
	/// <summary>Obtains all available metadata from JPEG formatted files.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegMetadataReader
	{
		public static readonly Iterable<JpegSegmentMetadataReader> AllReaders = Arrays.AsList(new JpegReader(), new JpegCommentReader(), new JfifReader(), new ExifReader(), new XmpReader(), new IccReader(), new PhotoshopReader(), new IptcReader(), new 
			AdobeJpegReader());

		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(InputStream inputStream, Iterable<JpegSegmentMetadataReader> readers)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			Process(metadata, inputStream, readers);
			return metadata;
		}

		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(InputStream inputStream)
		{
			return ReadMetadata(inputStream, null);
		}

		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(FilePath file, Iterable<JpegSegmentMetadataReader> readers)
		{
			InputStream inputStream = null;
			try
			{
				inputStream = new FileInputStream(file);
				return ReadMetadata(inputStream, readers);
			}
			finally
			{
				if (inputStream != null)
				{
					inputStream.Close();
				}
			}
		}

		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(FilePath file)
		{
			return ReadMetadata(file, null);
		}

		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		public static void Process(Com.Drew.Metadata.Metadata metadata, InputStream inputStream)
		{
			Process(metadata, inputStream, null);
		}

		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		public static void Process(Com.Drew.Metadata.Metadata metadata, InputStream inputStream, Iterable<JpegSegmentMetadataReader> readers)
		{
			if (readers == null)
			{
				readers = AllReaders;
			}
			ICollection<JpegSegmentType> segmentTypes = new HashSet<JpegSegmentType>();
			foreach (JpegSegmentMetadataReader reader in readers)
			{
				foreach (JpegSegmentType type in reader.GetSegmentTypes())
				{
					segmentTypes.Add(type);
				}
			}
			JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new Com.Drew.Lang.StreamReader(inputStream), segmentTypes.AsIterable());
			ProcessJpegSegmentData(metadata, readers, segmentData);
		}

		public static void ProcessJpegSegmentData(Com.Drew.Metadata.Metadata metadata, Iterable<JpegSegmentMetadataReader> readers, JpegSegmentData segmentData)
		{
			// Pass the appropriate byte arrays to each reader.
			foreach (JpegSegmentMetadataReader reader in readers)
			{
				foreach (JpegSegmentType segmentType in reader.GetSegmentTypes())
				{
					foreach (sbyte[] segmentBytes in segmentData.GetSegments(segmentType))
					{
						if (reader.CanProcess(segmentBytes, segmentType))
						{
							reader.Extract(segmentBytes, metadata, segmentType);
						}
					}
				}
			}
		}

		/// <exception cref="System.Exception"/>
		private JpegMetadataReader()
		{
			throw new Exception("Not intended for instantiation");
		}
	}
}
