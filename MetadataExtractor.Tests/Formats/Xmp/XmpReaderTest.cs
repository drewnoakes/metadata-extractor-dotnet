// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Xmp
{
    /// <summary>Unit tests for <see cref="XmpReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpReaderTest
    {
        private const int expectedPropertyCount = 167;

        private readonly XmpDirectory _directory;

        public XmpReaderTest()
        {
            var jpegSegments = new [] { new JpegSegment(JpegSegmentType.App1, TestDataUtil.GetBytes("Data/withXmpAndIptc.jpg.app1.1"), offset: 0) };
            var directories = new XmpReader().ReadJpegSegments(jpegSegments);
            _directory = directories.OfType<XmpDirectory>().ToList().Single();
            Assert.False(_directory.HasError);
        }

        [Fact]
        public void Extract_HasXMPMeta()
        {
            Assert.NotNull(_directory.XmpMeta);
        }

        [Fact]
        public void Extract_PropertyCount()
        {
            Assert.Equal(expectedPropertyCount, _directory.GetInt32(XmpDirectory.TagXmpValueCount));
        }

        [Fact]
        public void GetXmpProperties()
        {
            var propertyMap = _directory.GetXmpProperties();

            Assert.Equal(expectedPropertyCount, propertyMap.Count);

            Assert.True(propertyMap.ContainsKey("photoshop:Country"));
            Assert.Equal("Deutschland", propertyMap["photoshop:Country"]);

            Assert.True(propertyMap.ContainsKey("tiff:ImageLength"));
            Assert.Equal("900", propertyMap["tiff:ImageLength"]);
        }
    }
}
