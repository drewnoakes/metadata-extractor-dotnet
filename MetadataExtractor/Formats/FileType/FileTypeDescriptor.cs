// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.FileType
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FileTypeDescriptor : TagDescriptor<FileTypeDirectory>
    {
        public FileTypeDescriptor(FileTypeDirectory directory)
            : base(directory)
        {
        }
    }
}
