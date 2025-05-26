// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileSystem;

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
        /// <exception cref="IOException"/>
        /// <exception cref="TiffProcessingException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess))
            {
                var handler = new ExifTiffHandler(directories, exifStartOffset: 0);
                TiffReader.ProcessTiff(new IndexedSeekingReader(stream), handler);
            }

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="IOException"/>
        /// <exception cref="TiffProcessingException"/>
        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();

            var handler = new ExifTiffHandler(directories, exifStartOffset: 0);

            // TIFF processing requires random access, as directories can be scattered throughout the byte sequence.
            // Stream does not support seeking backwards, so we wrap it with IndexedCapturingReader, which
            // buffers data from the stream as we seek forward.
            using var reader = new IndexedCapturingReader(stream);

            TiffReader.ProcessTiff(reader, handler);

            return directories;
        }
    }
}
