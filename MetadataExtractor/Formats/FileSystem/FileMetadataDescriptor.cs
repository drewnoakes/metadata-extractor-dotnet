// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.FileSystem
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FileMetadataDescriptor : TagDescriptor<FileMetadataDirectory>
    {
        public FileMetadataDescriptor(FileMetadataDirectory directory)
            : base(directory)
        {
        }

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
