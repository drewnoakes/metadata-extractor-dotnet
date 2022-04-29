// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Obtains metadata from Photoshop's PSD files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class PsdMetadataReader
    {
        /// <exception cref="IOException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(new PsdReader().Extract(new SequentialStreamReader(stream)));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            return new PsdReader().Extract(new SequentialStreamReader(stream));
        }
    }
}
