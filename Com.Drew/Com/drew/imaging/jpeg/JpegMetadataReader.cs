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

using System;
using System.Collections.Generic;
using Com.Drew.Lang;
using Com.Drew.Metadata.Adobe;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.File;
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
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegMetadataReader
    {
        public static readonly Iterable<JpegSegmentMetadataReader> AllReaders = Arrays.AsList<JpegSegmentMetadataReader>(new JpegReader(), new JpegCommentReader(), new JfifReader(), new ExifReader(), new XmpReader(), new IccReader(), new PhotoshopReader(), new IptcReader(), new
            AdobeJpegReader()).AsIterable();

        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] InputStream inputStream, [CanBeNull] Iterable<JpegSegmentMetadataReader> readers = null)
        {
            Metadata.Metadata metadata = new Metadata.Metadata();
            Process(metadata, inputStream, readers);
            return metadata;
        }

        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] FilePath file, [CanBeNull] Iterable<JpegSegmentMetadataReader> readers = null)
        {
            InputStream inputStream = new FileInputStream(file);
            Metadata.Metadata metadata;
            try
            {
                metadata = ReadMetadata(inputStream, readers);
            }
            finally
            {
                inputStream.Close();
            }
            new FileMetadataReader().Read(file, metadata);
            return metadata;
        }

        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static void Process([NotNull] Metadata.Metadata metadata, [NotNull] InputStream inputStream, [CanBeNull] Iterable<JpegSegmentMetadataReader> readers = null)
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
            JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new StreamReader(inputStream), segmentTypes.AsIterable());
            ProcessJpegSegmentData(metadata, readers, segmentData);
        }

        public static void ProcessJpegSegmentData(Metadata.Metadata metadata, Iterable<JpegSegmentMetadataReader> readers, JpegSegmentData segmentData)
        {
            // Pass the appropriate byte arrays to each reader.
            foreach (JpegSegmentMetadataReader reader in readers)
            {
                foreach (JpegSegmentType segmentType in reader.GetSegmentTypes())
                {
                    reader.ReadJpegSegments(segmentData.GetSegments(segmentType), metadata, segmentType);
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
