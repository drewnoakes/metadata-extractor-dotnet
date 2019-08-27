#region License
//
// Copyright 2002-2019 Drew Noakes
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
