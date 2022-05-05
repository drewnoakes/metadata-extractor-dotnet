// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor.Formats.FileSystem;

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Obtains metadata from TGA (Truevision) files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class TgaMetadataReader
    {
        /// <exception cref="IOException"/>
        /// <exception cref="ArgumentException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }

        /// <exception cref="IOException"/>
        /// <exception cref="ArgumentException"/>
        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Must support seek", nameof(stream));
            var list = new List<Directory>
            {
                new TgaHeaderReader().Extract(stream)
            };
            new TgaFooterReader().TryGetOffsets(stream, out int extOffset, out int devOffset);
            if (extOffset > 0)
                list.Add(new TgaExtensionReader().Extract(stream, extOffset));
            if (devOffset > 0)
                list.Add(new TgaDeveloperReader().Extract(stream, devOffset));
            return list;
        }
    }
}
