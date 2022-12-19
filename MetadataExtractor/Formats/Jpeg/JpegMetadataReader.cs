// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Flir;
using MetadataExtractor.Formats.Xmp;

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
        private static readonly ICollection<IJpegSegmentMetadataReader> _allReaders = AllReaders.ToList();

        public static IEnumerable<IJpegSegmentMetadataReader> AllReaders
        {
            get
            {
                yield return new JpegReader();
                yield return new JpegCommentReader();
                yield return new JfifReader();
                yield return new JfxxReader();
                yield return new ExifReader();
                yield return new XmpReader();
                yield return new IccReader();
                yield return new PhotoshopReader();
                yield return new DuckyReader();
                yield return new IptcReader();
                yield return new AdobeJpegReader();
                yield return new JpegDhtReader();
                yield return new JpegDnlReader();
                yield return new FlirReader();
            }
        }

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>
        public static DirectoryList ReadMetadata(Stream stream, ICollection<IJpegSegmentMetadataReader>? readers = null)
        {
            return Process(stream, readers);
        }

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>
        public static DirectoryList ReadMetadata(string filePath, ICollection<IJpegSegmentMetadataReader>? readers = null)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream, readers));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>
        public static DirectoryList Process(Stream stream, ICollection<IJpegSegmentMetadataReader>? readers = null)
        {
            readers ??= _allReaders;

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
