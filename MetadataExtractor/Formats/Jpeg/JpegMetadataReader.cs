#region License
//
// Copyright 2002-2016 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
#if !PORTABLE
using MetadataExtractor.Formats.FileSystem;
#endif
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Obtains all available metadata from JPEG formatted files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegMetadataReader
    {
        private static readonly IEnumerable<IJpegSegmentMetadataReader> _allReaders = new IJpegSegmentMetadataReader[]
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
            new AdobeJpegReader()
        };

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static
#if NET35 || PORTABLE
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadMetadata([NotNull] Stream stream, [CanBeNull] IEnumerable<IJpegSegmentMetadataReader> readers = null)
        {
            return Process(stream, readers);
        }

#if !PORTABLE
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static
#if NET35
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadMetadata([NotNull] string filePath, [CanBeNull] IEnumerable<IJpegSegmentMetadataReader> readers = null)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream, readers));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }
#endif

        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static
#if NET35 || PORTABLE
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            Process([NotNull] Stream stream, [CanBeNull] IEnumerable<IJpegSegmentMetadataReader> readers = null)
        {
            if (readers == null)
                readers = _allReaders;

            var segmentTypes = new HashSet<JpegSegmentType>(readers.SelectMany(reader => reader.GetSegmentTypes()));
            var segmentData = JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream), segmentTypes);
            return ProcessJpegSegmentData(readers, segmentData);
        }

        public static
#if NET35 || PORTABLE
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ProcessJpegSegmentData(IEnumerable<IJpegSegmentMetadataReader> readers, JpegSegmentData segmentData)
        {
            // Pass the appropriate byte arrays to each reader.
            return (from reader in readers
                    from segmentType in reader.GetSegmentTypes()
                    from directory in reader.ReadJpegSegments(segmentData.GetSegments(segmentType), segmentType)
                    select directory)
                    .ToList();
        }
    }
}
