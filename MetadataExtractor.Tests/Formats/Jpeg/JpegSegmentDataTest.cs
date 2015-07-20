#region License
//
// Copyright 2002-2015 Drew Noakes
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

using System.Diagnostics.CodeAnalysis;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    public sealed class JpegSegmentDataTest
    {
        [Fact]
        public void TestAddAndGetSegment()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes = new byte[] { 1, 2, 3 };
            segmentData.AddSegment(segmentType, segmentBytes);
            Assert.Equal(1, segmentData.GetSegmentCount(segmentType));
            Assert.Equal(segmentBytes, segmentData.GetSegment(segmentType));
        }

        [Fact]
        public void TestContainsSegment()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes = new byte[] { 1, 2, 3 };
            Assert.True(!segmentData.ContainsSegment(segmentType));
            segmentData.AddSegment(segmentType, segmentBytes);
            Assert.True(segmentData.ContainsSegment(segmentType));
        }

        [Fact]
        public void TestAddingMultipleSegments()
        {
            var segmentData = new JpegSegmentData();
            var segmentType1 = JpegSegmentType.App3;
            var segmentType2 = JpegSegmentType.App4;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType1, segmentBytes1);
            segmentData.AddSegment(segmentType2, segmentBytes2);
            Assert.Equal(1, segmentData.GetSegmentCount(segmentType1));
            Assert.Equal(1, segmentData.GetSegmentCount(segmentType2));
            Assert.Equal(segmentBytes1, segmentData.GetSegment(segmentType1));
            Assert.Equal(segmentBytes2, segmentData.GetSegment(segmentType2));
        }

        [Fact]
        public void TestSegmentWithMultipleOccurrences()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType, segmentBytes1);
            segmentData.AddSegment(segmentType, segmentBytes2);
            Assert.Equal(2, segmentData.GetSegmentCount(segmentType));
            Assert.Equal(segmentBytes1, segmentData.GetSegment(segmentType));
            Assert.Equal(segmentBytes2, segmentData.GetSegment(segmentType, occurrence: 1));
        }

        [Fact]
        public void TestRemoveSegmentOccurrence()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType, segmentBytes1);
            segmentData.AddSegment(segmentType, segmentBytes2);
            Assert.Equal(2, segmentData.GetSegmentCount(segmentType));
            Assert.Equal(segmentBytes1, segmentData.GetSegment(segmentType));
            segmentData.RemoveSegmentOccurrence(segmentType, 0);
            Assert.Equal(segmentBytes2, segmentData.GetSegment(segmentType));
        }

        [Fact]
        public void TestRemoveSegment()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType, segmentBytes1);
            segmentData.AddSegment(segmentType, segmentBytes2);
            Assert.Equal(2, segmentData.GetSegmentCount(segmentType));
            Assert.True(segmentData.ContainsSegment(segmentType));
            Assert.Equal(segmentBytes1, segmentData.GetSegment(segmentType));
            segmentData.RemoveAllSegments(segmentType);
            Assert.True(!segmentData.ContainsSegment(segmentType));
            Assert.Equal(0, segmentData.GetSegmentCount(segmentType));
        }
    }
}
