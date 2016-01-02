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

using System.IO;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>
    /// Unit tests for <see cref="JpegSegmentReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegmentReaderTest
    {
        [Fact]
        public void TestReadAllSegments()
        {
            string fileName = "Tests/Data/withExifAndIptc.jpg";
#if PORTABLE
            JpegSegmentData segmentData = null;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                segmentData = JpegSegmentReader.ReadSegments(new MetadataExtractor.IO.SequentialStreamReader(stream), null);
#else
            var segmentData = JpegSegmentReader.ReadSegments(fileName, null);
#endif
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.App0));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.app0"), segmentData.GetSegment(JpegSegmentType.App0));
            Assert.Null(segmentData.GetSegment(JpegSegmentType.App0, 1));
            Assert.Equal(2, segmentData.GetSegmentCount(JpegSegmentType.App1));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.app1.0"), segmentData.GetSegment(JpegSegmentType.App1));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.app1.1"), segmentData.GetSegment(JpegSegmentType.App1, occurrence: 1));
            Assert.Null(segmentData.GetSegment(JpegSegmentType.App1, 2));
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.App2));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.app2"), segmentData.GetSegment(JpegSegmentType.App2));
            Assert.Null(segmentData.GetSegment(JpegSegmentType.App2, 1));
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.AppD));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.appd"), segmentData.GetSegment(JpegSegmentType.AppD));
            Assert.Null(segmentData.GetSegment(JpegSegmentType.AppD, 1));
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.AppE));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.appe"), segmentData.GetSegment(JpegSegmentType.AppE));
            Assert.Null(segmentData.GetSegment(JpegSegmentType.AppE, 1));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App3));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App4));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App5));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App6));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App7));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App8));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App9));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppA));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppB));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppC));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppF));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.Com));
            Assert.Equal(4, segmentData.GetSegmentCount(JpegSegmentType.Dht));
            Assert.Equal(2, segmentData.GetSegmentCount(JpegSegmentType.Dqt));
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.Sof0));
            Assert.Null(segmentData.GetSegment(JpegSegmentType.App3));
        }

        [Fact]
        public void TestReadSpecificSegments()
        {
            string fileName = "Tests/Data/withExifAndIptc.jpg";
#if PORTABLE
            JpegSegmentData segmentData = null;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                segmentData = JpegSegmentReader.ReadSegments(new MetadataExtractor.IO.SequentialStreamReader(stream), new[] { JpegSegmentType.App0, JpegSegmentType.App2 });
#else
            var segmentData = JpegSegmentReader.ReadSegments(fileName, new[] { JpegSegmentType.App0, JpegSegmentType.App2 });
#endif
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.App0));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App1));
            Assert.Equal(1, segmentData.GetSegmentCount(JpegSegmentType.App2));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppD));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppE));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App3));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App4));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App5));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App6));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App7));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App8));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.App9));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppA));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppB));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppC));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.AppF));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.Com));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.Dht));
            Assert.Equal(0, segmentData.GetSegmentCount(JpegSegmentType.Sof0));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.app0"), segmentData.GetSegment(JpegSegmentType.App0));
            Assert.Equal(File.ReadAllBytes("Tests/Data/withExifAndIptc.jpg.app2"), segmentData.GetSegment(JpegSegmentType.App2));
        }

        [Fact]
        public void TestLoadJpegWithoutExifDataReturnsNull()
        {
            string fileName = "Tests/Data/noExif.jpg";
#if PORTABLE
            JpegSegmentData segmentData = null;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                segmentData = JpegSegmentReader.ReadSegments(new MetadataExtractor.IO.SequentialStreamReader(stream), null);
#else
            var segmentData = JpegSegmentReader.ReadSegments(fileName, null);
#endif
            Assert.Null(segmentData.GetSegment(JpegSegmentType.App1));
        }

        [Fact]
        public void TestWithNonJpegFile()
        {
#if PORTABLE
            string fileName = "MetadataExtractor.Portable.Tests.dll";
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                Assert.Throws<JpegProcessingException>(() => JpegSegmentReader.ReadSegments(new MetadataExtractor.IO.SequentialStreamReader(stream), null));
#else
            string fileName = "MetadataExtractor.Tests.dll";
            Assert.Throws<JpegProcessingException>(() => JpegSegmentReader.ReadSegments(fileName, null));
#endif
        }
    }
}
