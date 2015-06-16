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

using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>Obtains all available metadata from TIFF formatted files.</summary>
    /// <remarks>
    /// Obtains all available metadata from TIFF formatted files.  Note that TIFF files include many digital camera RAW
    /// formats, including Canon (CRW, CR2), Nikon (NEF), Olympus (ORF) and Panasonic (RW2).
    /// </remarks>
    /// <author>Darren Salomons</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class TiffMetadataReader
    {
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="TiffProcessingException"/>
        [NotNull]
        public static IReadOnlyList<Directory> ReadMetadata([NotNull] string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess))
            {
                var handler = new ExifTiffHandler(directories, storeThumbnailBytes: false);
                TiffReader.ProcessTiff(new IndexedSeekingReader(stream), handler);
            }

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="TiffProcessingException"/>
        [NotNull]
        public static IReadOnlyList<Directory> ReadMetadata([NotNull] Stream stream)
        {
            // TIFF processing requires random access, as directories can be scattered throughout the byte sequence.
            // Stream does not support seeking backwards, so we wrap it with IndexedCapturingReader, which
            // buffers data from the stream as we seek forward.
            var directories = new List<Directory>();

            var handler = new ExifTiffHandler(directories, false);
            TiffReader.ProcessTiff(new IndexedCapturingReader(stream), handler, 0);

            return directories;
        }
    }
}
