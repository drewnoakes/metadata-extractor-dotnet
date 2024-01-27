// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.FileSystem
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class FileMetadataDescriptor(FileMetadataDirectory directory)
        : TagDescriptor<FileMetadataDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                FileMetadataDirectory.TagFileSize => GetFileSizeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        private string? GetFileSizeDescription()
        {
            return Directory.TryGetInt64(FileMetadataDirectory.TagFileSize, out long size)
                ? size + " bytes"
                : null;
        }
    }
}
