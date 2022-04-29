// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.Formats.FileSystem;

namespace MetadataExtractor.Formats.Eps
{
    /// <summary>Obtains metadata from EPS files.</summary>
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public static class EpsMetadataReader
    {
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(new EpsReader().Extract(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            return new EpsReader().Extract(stream);
        }
    }
}
