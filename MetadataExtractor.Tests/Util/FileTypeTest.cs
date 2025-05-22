// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util
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

        public static IEnumerable<object[]> FileTypeMembers()
        {
            return Enum.GetValues(typeof(FileType)).Cast<FileType>().Select(fileType => new object[] { fileType });
        }
    }
}
