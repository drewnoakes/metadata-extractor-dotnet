// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.FileType
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FileTypeDirectory : Directory
    {
        public const int TagDetectedFileTypeName = 1;
        public const int TagDetectedFileTypeLongName = 2;
        public const int TagDetectedFileMimeType = 3;
        public const int TagExpectedFileNameExtension = 4;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagDetectedFileTypeName, "Detected File Type Name" },
            { TagDetectedFileTypeLongName, "Detected File Type Long Name" },
            { TagDetectedFileMimeType, "Detected MIME Type" },
            { TagExpectedFileNameExtension, "Expected File Name Extension" },
        };

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public FileTypeDirectory(Util.FileType fileType)
        {
            SetDescriptor(new FileTypeDescriptor(this));

            var name = fileType.GetName();
                        
            Set(TagDetectedFileTypeName, name);
            Set(TagDetectedFileTypeLongName, fileType.GetLongName());

            var mimeType = fileType.GetMimeType();
            if (mimeType != null)
                Set(TagDetectedFileMimeType, mimeType);

            var extension = fileType.GetCommonExtension();
            if (extension != null)
                Set(TagExpectedFileNameExtension, extension);
        }

        public override string Name => "File Type";

        protected override bool TryGetTagName(int tagType, out string tagName) => _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
