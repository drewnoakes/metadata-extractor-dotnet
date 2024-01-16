// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.FileSystem;

namespace MetadataExtractor.Formats.Netpbm
{
    /// <summary>Obtains metadata from BMP files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class NetpbmMetadataReader
    {
        /// <exception cref="IOException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>(2);

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.Add(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        public static NetpbmHeaderDirectory ReadMetadata(Stream stream)
        {
            return new NetpbmReader().Extract(stream);
        }
    }
}
