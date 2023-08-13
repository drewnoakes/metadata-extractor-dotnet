// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.FileType
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class FileTypeDescriptor : TagDescriptor<FileTypeDirectory>
    {
        public FileTypeDescriptor(FileTypeDirectory directory)
            : base(directory)
        {
        }
    }
}
