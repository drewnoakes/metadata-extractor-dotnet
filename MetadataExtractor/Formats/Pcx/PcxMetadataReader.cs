// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using System.Collections.Generic;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Pcx
{
    /// <summary>Obtains metadata from PCX image files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class PcxMetadataReader
    {
        /// <exception cref="IOException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.Add(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        public static PcxDirectory ReadMetadata(Stream stream)
        {
            return new PcxReader().Extract(new SequentialStreamReader(stream));
        }
    }
}
