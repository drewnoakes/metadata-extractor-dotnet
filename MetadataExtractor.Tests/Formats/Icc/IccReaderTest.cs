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
using System.Linq;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Icc
{
    /// <summary>Unit tests for <see cref="IccReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IccReaderTest
    {
        // TODO add a test with well-formed ICC data and assert output values are correct

        [Fact]
        public void Extract_InvalidData()
        {
            var app2Bytes = TestDataUtil.GetBytes("Data/iccDataInvalid1.jpg.app2");

            // When in an APP2 segment, ICC data starts after a 14-byte preamble
            var icc = TestHelper.SkipBytes(app2Bytes, 14);
            var directory = new IccReader().Extract(new ByteArrayReader(icc));
            Assert.NotNull(directory);
            Assert.True(directory.HasError);
        }

        [Fact]
        public void ReadJpegSegments_InvalidData()
        {
            var app2 = new JpegSegment(JpegSegmentType.App2, TestDataUtil.GetBytes("Data/iccDataInvalid1.jpg.app2"), offset: 0);
            var directory = new IccReader().ReadJpegSegments(new[] { app2 });
            Assert.NotNull(directory);
            Assert.True(directory.Single().HasError);
        }

        [Fact]
        public void GetStringFromUInt32()
        {
            Assert.Equal("ABCD", IccReader.GetStringFromUInt32(0x41424344u));
        }

        [Fact]
        public void Extract_ProfileDateTime()
        {
            var app2 = new JpegSegment(JpegSegmentType.App2, TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app2"), offset: 0);

            var directory = new IccReader()
                .ReadJpegSegments(new[] { app2 })
                .OfType<IccDirectory>()
                .Single();

            Assert.NotNull(directory);
//            Assert.Equal("1998:02:09 06:49:00", directory.GetString(IccDirectory.TagProfileDateTime));
            Assert.Equal(new DateTime(1998, 2, 9, 6, 49, 0), directory.GetDateTime(IccDirectory.TagProfileDateTime));
        }
    }
}
