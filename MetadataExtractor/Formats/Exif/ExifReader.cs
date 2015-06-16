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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.IO;

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
        private const string JpegSegmentPreamble = "Exif\x0\x0";

        public ExifReader()
        {
            StoreThumbnailBytes = true;
        }

        public bool StoreThumbnailBytes { get; set; }

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.App1;
        }

        public IReadOnlyList<Directory> ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
        {
            Debug.Assert(segmentType == JpegSegmentType.App1);

            return segments
                .Where(segment => segment.Length >= JpegSegmentPreamble.Length && Encoding.UTF8.GetString(segment, 0, JpegSegmentPreamble.Length) == JpegSegmentPreamble)
                .SelectMany(segment => Extract(new ByteArrayReader(segment), JpegSegmentPreamble.Length))
                .ToList();
        }

        /// <summary>
        /// Reads TIFF formatted Exif data a specified offset within a <see cref="IndexedReader"/>.
        /// </summary>
        public IReadOnlyList<Directory> Extract(IndexedReader reader, int readerOffset = 0)
        {
            var directories = new List<Directory>();

            try
            {
                // Read the TIFF-formatted Exif data
                TiffReader.ProcessTiff(reader, new ExifTiffHandler(directories, StoreThumbnailBytes), readerOffset);
            }
            catch (TiffProcessingException e)
            {
                // TODO what do to with this error state?
                Console.Error.WriteLine (e);
            }
            catch (IOException e)
            {
                // TODO what do to with this error state?
                Console.Error.WriteLine (e);
            }

            return directories;
        }
    }
}
