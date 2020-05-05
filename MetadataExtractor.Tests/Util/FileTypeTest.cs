// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Util;
using Xunit;

namespace MetadataExtractor.Tests.Util
{
    public class FileTypeTest
    {
        [Theory]
        [MemberData(nameof(FileTypeMembers))]
        public void GetName(FileType member)
        {
            member.GetName();
        }

        [Theory]
        [MemberData(nameof(FileTypeMembers))]
        public void GetLongName(FileType member)
        {
            member.GetLongName();
        }

        [Theory]
        [MemberData(nameof(FileTypeMembers))]
        public void GetMimeType(FileType member)
        {
            member.GetMimeType();
        }

        [Theory]
        [MemberData(nameof(FileTypeMembers))]
        public void GetCommonExtension(FileType member)
        {
            member.GetCommonExtension();
        }

        [Theory]
        [MemberData(nameof(FileTypeMembers))]
        public void GetAllExtensions(FileType member)
        {
            member.GetAllExtensions();
        }

        private static IEnumerable<object[]> FileTypeMembers()
        {
            return Enum.GetValues(typeof(FileType)).Cast<FileType>().Select(fileType => new object[] { fileType });
        }
    }
}
