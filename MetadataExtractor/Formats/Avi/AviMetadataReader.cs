// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Riff;

namespace MetadataExtractor.Formats.Avi
{
    /// <summary>Obtains metadata from Avi files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class AviMetadataReader
    {
        /// <exception cref="IOException"/>
        /// <exception cref="RiffProcessingException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="IOException"/>
        /// <exception cref="RiffProcessingException"/>
        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();
            new RiffReader().ProcessRiff(new SequentialStreamReader(stream), new AviRiffHandler(directories));
            return directories;
        }
    }
}
