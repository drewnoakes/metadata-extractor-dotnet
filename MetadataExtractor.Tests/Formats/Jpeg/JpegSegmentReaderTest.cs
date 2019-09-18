// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegSegmentReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegmentReaderTest
    {
        private static IReadOnlyList<JpegSegment> ReadSegments(string fileName, ICollection<JpegSegmentType>? segmentTypes = null)
        {
            using var stream = TestDataUtil.OpenRead(fileName);
            return JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream), segmentTypes).ToList();
        }

        [Fact]
        public void ReadAllSegments()
        {
            var segments = ReadSegments("Data/withExifAndIptc.jpg");

            Assert.Equal(13, segments.Count);

            Assert.Equal(JpegSegmentType.App0, segments[0].Type);
            Assert.Equal(JpegSegmentType.App1, segments[1].Type);
            Assert.Equal(JpegSegmentType.AppD, segments[2].Type);
            Assert.Equal(JpegSegmentType.App1, segments[3].Type);
            Assert.Equal(JpegSegmentType.App2, segments[4].Type);
            Assert.Equal(JpegSegmentType.AppE, segments[5].Type);

            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app0"),   segments[0].Bytes);
            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app1.0"), segments[1].Bytes);
            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.appd"),   segments[2].Bytes);
            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app1.1"), segments[3].Bytes);
            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app2"),   segments[4].Bytes);
            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.appe"),   segments[5].Bytes);
        }

        [Fact]
        public void ReadSpecificSegments()
        {
            var segments = ReadSegments("Data/withExifAndIptc.jpg", new[] { JpegSegmentType.App0, JpegSegmentType.App2 });

            Assert.Equal(2, segments.Count);

            Assert.Equal(JpegSegmentType.App0, segments[0].Type);
            Assert.Equal(JpegSegmentType.App2, segments[1].Type);

            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app0"), segments[0].Bytes);
            Assert.Equal(TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app2"), segments[1].Bytes);
        }

        [Fact]
        public void LoadJpegWithoutExifDataReturnsNull()
        {
            Assert.DoesNotContain(
                ReadSegments("Data/noExif.jpg"),
                s => s.Type == JpegSegmentType.App1);
        }

        [Fact]
        public void WithNonJpegData()
        {
            var bytes = Enumerable.Range(1, 100).Select(i => (byte)i).ToArray();
            var ex = Assert.Throws<JpegProcessingException>(
                () => JpegSegmentReader.ReadSegments(new SequentialByteArrayReader(bytes)).ToList());

            Assert.Equal("JPEG data is expected to begin with 0xFFD8 (ÿØ) not 0x0102", ex.Message);
        }
    }
}
