// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;

namespace MetadataExtractor.Formats.FileSystem
{
    public sealed class FileMetadataReader
    {
        /// <exception cref="System.IO.IOException"/>
        public FileMetadataDirectory Read(string file)
        {
            var attr = File.GetAttributes(file);

            if ((attr & FileAttributes.Directory) != 0)
                throw new IOException("File object must reference a file");

            var fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
                throw new IOException("File does not exist");

            var directory = new FileMetadataDirectory();

            directory.Set(FileMetadataDirectory.TagFileName, Path.GetFileName(file));
            directory.Set(FileMetadataDirectory.TagFileSize, fileInfo.Length);
            directory.Set(FileMetadataDirectory.TagFileModifiedDate, fileInfo.LastWriteTime);

            return directory;
        }
    }
}
