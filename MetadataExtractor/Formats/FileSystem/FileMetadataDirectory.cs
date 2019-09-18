// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.FileSystem
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FileMetadataDirectory : Directory
    {
        public const int TagFileName = 1;
        public const int TagFileSize = 2;
        public const int TagFileModifiedDate = 3;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagFileName, "File Name" },
            { TagFileSize, "File Size" },
            { TagFileModifiedDate, "File Modified Date" }
        };

        public FileMetadataDirectory() => SetDescriptor(new FileMetadataDescriptor(this));

        public override string Name => "File";

        protected override bool TryGetTagName(int tagType, out string tagName) => _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
