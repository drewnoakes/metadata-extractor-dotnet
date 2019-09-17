// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Obtains all available metadata from JPEG formatted files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegMetadataReader
    {
        private static readonly ICollection<IJpegSegmentMetadataReader> _allReaders = new IJpegSegmentMetadataReader[]
        {
            new JpegReader(),
            new JpegCommentReader(),
            new JfifReader(),
            new JfxxReader(),
            new ExifReader(),
            new XmpReader(),
            new IccReader(),
            new PhotoshopReader(),
            new DuckyReader(),
            new IptcReader(),
            new AdobeJpegReader(),
            new JpegDhtReader(),
            new JpegDnlReader()
        };

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static DirectoryList ReadMetadata(Stream stream, ICollection<IJpegSegmentMetadataReader>? readers = null)
        {
            return Process(stream, readers);
        }

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static DirectoryList ReadMetadata(string filePath, ICollection<IJpegSegmentMetadataReader>? readers = null)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream, readers));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static DirectoryList Process(Stream stream, ICollection<IJpegSegmentMetadataReader>? readers = null)
        {
            if (readers == null)
                readers = _allReaders;

            // Build the union of segment types desired by all readers
            var segmentTypes = new HashSet<JpegSegmentType>(readers.SelectMany(reader => reader.SegmentTypes));

            // Read out those segments
            var segments = JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream), segmentTypes);

            // Process them
            return ProcessJpegSegments(readers, segments.ToList());
        }

        public static DirectoryList ProcessJpegSegments(IEnumerable<IJpegSegmentMetadataReader> readers, ICollection<JpegSegment> segments)
        {
            var directories = new List<Directory>();

            foreach (var reader in readers)
            {
                var readerSegmentTypes = reader.SegmentTypes;
                var readerSegments = segments.Where(s => readerSegmentTypes.Contains(s.Type));
                directories.AddRange(reader.ReadJpegSegments(readerSegments));
            }

            return directories;
        }
    }
}
