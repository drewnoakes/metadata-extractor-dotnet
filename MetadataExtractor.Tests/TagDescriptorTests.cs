// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using Xunit;

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for <see cref="TagDescriptor{T}"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class TagDescriptorTests
    {
        [Fact]
        public void GetDescription_SimplifiedLongCollections()
        {
            var directory = new MockDirectory();
            var descriptor = new TagDescriptor<MockDirectory>(directory);

            const int tagType = 1;

            directory.Set(tagType, Enumerable.Range(0, 16).Select(i => i).ToArray());

            Assert.Equal("0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15", descriptor.GetDescription(tagType));

            directory.Set(tagType, Enumerable.Range(0, 17).Select(i => i).ToArray());

            Assert.Equal("[17 values]", descriptor.GetDescription(tagType));
        }

        [Fact]
        public void ConvertBytesToVersionString()
        {
            Assert.Null(TagDescriptor<MockDirectory>.ConvertBytesToVersionString(null, 1));
            Assert.Null(TagDescriptor<MockDirectory>.ConvertBytesToVersionString(Array.Empty<int>(), 1));

            Assert.Equal("1.00", TagDescriptor<MockDirectory>.ConvertBytesToVersionString(new[] { 0, 1, 0, 0 }, 2));
            Assert.Equal(".100", TagDescriptor<MockDirectory>.ConvertBytesToVersionString(new[] { 0, 1, 0, 0 }, 1));

            Assert.Equal("2.10", TagDescriptor<MockDirectory>.ConvertBytesToVersionString(new[] { 0x30, 0x32, 0x31, 0x30 }, 2));
            Assert.Equal(".210", TagDescriptor<MockDirectory>.ConvertBytesToVersionString(new[] { 0x30, 0x32, 0x31, 0x30 }, 1));
            Assert.Equal("21.0", TagDescriptor<MockDirectory>.ConvertBytesToVersionString(new[] { 0x30, 0x32, 0x31, 0x30 }, 3));
        }
    }
}
