#region License
//
// Copyright 2002-2017 Drew Noakes
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Decodes Exif binary data into potentially many <see cref="Directory"/> objects such as
    /// <see cref="ExifSubIfdDirectory"/>, <see cref="ExifThumbnailDirectory"/>, <see cref="ExifInteropDirectory"/>,
    /// <see cref="GpsDirectory"/>, camera makernote directories and more.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifReader : IJpegSegmentMetadataReader
    {
        /// <summary>Exif data stored in JPEG files' APP1 segment are preceded by this six character preamble.</summary>
        public const string JpegSegmentPreamble = "Exif\x0\x0";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.App1 };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            return segments
                .Where(segment => segment.Bytes.Length >= JpegSegmentPreamble.Length && Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length) == JpegSegmentPreamble)
                .SelectMany(segment => Extract(new ByteArrayReader(segment.Bytes, baseOffset: JpegSegmentPreamble.Length)))
                .ToList();
        }

        /// <summary>
        /// Reads TIFF formatted Exif data a specified offset within a <see cref="IndexedReader"/>.
        /// </summary>
        [NotNull]
        public DirectoryList Extract([NotNull] IndexedReader reader)
        {
            var directories = new List<Directory>();
            var exifTiffHandler = new ExifTiffHandler(directories);

            try
            {
                // Read the TIFF-formatted Exif data
                TiffReader.ProcessTiff(reader, exifTiffHandler);
            }
            catch (Exception e)
            {
                exifTiffHandler.Error("Exception processing TIFF data: " + e.Message);
            }

            return directories;
        }
    }
}
