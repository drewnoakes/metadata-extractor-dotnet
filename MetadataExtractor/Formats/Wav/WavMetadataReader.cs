// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Riff;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Wav
{
    /// <summary>Obtains metadata from WAV files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class WavMetadataReader
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
            new RiffReader().ProcessRiff(new SequentialStreamReader(stream), new WavRiffHandler(directories));
            return directories;
        }
    }
}
