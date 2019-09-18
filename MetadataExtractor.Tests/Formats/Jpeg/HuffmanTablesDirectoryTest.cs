// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="HuffmanTablesDirectory"/>.</summary>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class HuffmanTablesDirectoryTest
    {
        private readonly HuffmanTablesDirectory _directory;

        public HuffmanTablesDirectoryTest()
        {
            _directory = new HuffmanTablesDirectory();
        }

        [Fact]
        public void TestSetAndGetValue()
        {
            _directory.Set(32, 8);
            Assert.Equal(8, _directory.GetInt32(32));
        }

        [Fact]
        public void TestGetComponent_NotAdded()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _directory.GetTable(1));
        }

        [Fact]
        public void TestGetNumberOfTables()
        {
            _directory.Set(HuffmanTablesDirectory.TagNumberOfTables, 9);
            Assert.Equal(9,_directory.GetNumberOfTables());
            Assert.Equal("9 Huffman tables", _directory.GetDescription(HuffmanTablesDirectory.TagNumberOfTables));
        }

        [Fact]
        public void TestIsTypical()
        {
            _directory.AddTable(new HuffmanTable(
                HuffmanTableClass.AC,
                0,
                HuffmanTablesDirectory.TypicalChrominanceAcLengths,
                HuffmanTablesDirectory.TypicalChrominanceAcValues
            ));
            _directory.AddTable(new HuffmanTable(
                HuffmanTableClass.DC,
                0,
                HuffmanTablesDirectory.TypicalLuminanceDcLengths,
                HuffmanTablesDirectory.TypicalLuminanceDcValues
            ));

            Assert.True(_directory.GetTable(0).IsTypical());
            Assert.False(_directory.GetTable(0).IsOptimized());
            Assert.True(_directory.GetTable(1).IsTypical());
            Assert.False(_directory.GetTable(1).IsOptimized());

            Assert.True(_directory.IsTypical());
            Assert.False(_directory.IsOptimized());
        }
    }
}
