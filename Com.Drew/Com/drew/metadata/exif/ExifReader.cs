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
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>
    /// Decodes Exif binary data, populating a <see cref="Com.Drew.Metadata.Metadata"/> object with tag values in
    /// <see cref="ExifSubIfdDirectory"/>, <see cref="ExifThumbnailDirectory"/>, <see cref="ExifInteropDirectory"/>,
    /// <see cref="GpsDirectory"/> and one of the many camera makernote directories.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifReader : IJpegSegmentMetadataReader
    {
        /// <summary>Exif data stored in JPEG files' APP1 segment are preceded by this six character preamble.</summary>
        public const string JpegSegmentPreamble = "Exif\x0\x0";

        private bool _storeThumbnailBytes = true;

        public bool IsStoreThumbnailBytes()
        {
            return _storeThumbnailBytes;
        }

        public void SetStoreThumbnailBytes(bool storeThumbnailBytes)
        {
            _storeThumbnailBytes = storeThumbnailBytes;
        }

        [NotNull]
        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            return Arrays.AsList(JpegSegmentType.App1);
        }

        public void ReadJpegSegments([NotNull] IEnumerable<sbyte[]> segments, [NotNull] Metadata metadata, [NotNull] JpegSegmentType segmentType)
        {
            Debug.Assert((segmentType == JpegSegmentType.App1));
            foreach (sbyte[] segmentBytes in segments)
            {
                // Filter any segments containing unexpected preambles
                if (segmentBytes.Length < JpegSegmentPreamble.Length || !Runtime.GetStringForBytes(segmentBytes, 0, JpegSegmentPreamble.Length).Equals(JpegSegmentPreamble))
                {
                    continue;
                }
                Extract(new ByteArrayReader(segmentBytes), metadata, JpegSegmentPreamble.Length);
            }
        }

        /// <summary>
        /// Reads TIFF formatted Exif data from start of the specified <see cref="Com.Drew.Lang.RandomAccessReader"/>.
        /// </summary>
        public void Extract([NotNull] RandomAccessReader reader, [NotNull] Metadata metadata)
        {
            Extract(reader, metadata, 0);
        }

        /// <summary>
        /// Reads TIFF formatted Exif data a specified offset within a <see cref="Com.Drew.Lang.RandomAccessReader"/>.
        /// </summary>
        public void Extract([NotNull] RandomAccessReader reader, [NotNull] Metadata metadata, int readerOffset)
        {
            try
            {
                // Read the TIFF-formatted Exif data
                new TiffReader().ProcessTiff(reader, new ExifTiffHandler(metadata, _storeThumbnailBytes), readerOffset);
            }
            catch (TiffProcessingException e)
            {
                // TODO what do to with this error state?
                Runtime.PrintStackTrace(e, Console.Error);
            }
            catch (IOException e)
            {
                // TODO what do to with this error state?
                Runtime.PrintStackTrace(e, Console.Error);
            }
        }
    }
}
