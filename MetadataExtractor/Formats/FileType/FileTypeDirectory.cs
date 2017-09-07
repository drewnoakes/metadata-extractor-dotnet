#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
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
