// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using System.Linq;
using System.Collections.Generic;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Gif
{
    /// <summary>Obtains metadata from GIF files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class GifMetadataReader
    {
        /// <exception cref="IOException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>(2);

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            return new GifReader().Extract(new SequentialStreamReader(stream)).ToList();
        }
    }
}
