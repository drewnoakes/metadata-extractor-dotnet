// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegReaderTest
    {
        private readonly JpegDirectory _directory;

        public JpegReaderTest()
        {
            var sof0 = new JpegSegment(JpegSegmentType.Sof0, TestDataUtil.GetBytes("Data/simple.jpg.sof0"), offset: 0);

            _directory = new JpegReader().Extract(sof0);
        }

        [Fact]
        public void Extract_Width()
        {
            Assert.Equal(800, _directory.GetInt32(JpegDirectory.TagImageWidth));
        }

        [Fact]
        public void Extract_Height()
        {
            Assert.Equal(600, _directory.GetInt32(JpegDirectory.TagImageHeight));
        }

        [Fact]
        public void Extract_DataPrecision()
        {
            Assert.Equal(8, _directory.GetInt32(JpegDirectory.TagDataPrecision));
        }

        [Fact]
        public void Extract_NumberOfComponents()
        {
            Assert.Equal(3, _directory.GetInt32(JpegDirectory.TagNumberOfComponents));
        }

        [Fact]
        public void ComponentData1()
        {
            var component = (JpegComponent?)_directory.GetObject(JpegDirectory.TagComponentData1);
            Assert.NotNull(component);
            Assert.Equal("Y", component!.Name);
            Assert.Equal(1, component.Id);
            Assert.Equal(0, component.QuantizationTableNumber);
            Assert.Equal(2, component.HorizontalSamplingFactor);
            Assert.Equal(2, component.VerticalSamplingFactor);
        }

        [Fact]
        public void ComponentData2()
        {
            var component = (JpegComponent?)_directory.GetObject(JpegDirectory.TagComponentData2);
            Assert.NotNull(component);
            Assert.Equal("Cb", component!.Name);
            Assert.Equal(2, component.Id);
            Assert.Equal(1, component.QuantizationTableNumber);
            Assert.Equal(1, component.HorizontalSamplingFactor);
            Assert.Equal(1, component.VerticalSamplingFactor);
            Assert.Equal("Cb component: Quantization table 1, Sampling factors 1 horiz/1 vert", _directory.GetDescription(JpegDirectory.TagComponentData2));
        }

        [Fact]
        public void ComponentData3()
        {
            var component = (JpegComponent?)_directory.GetObject(JpegDirectory.TagComponentData3);
            Assert.NotNull(component);
            Assert.Equal("Cr", component!.Name);
            Assert.Equal(3, component.Id);
            Assert.Equal(1, component.QuantizationTableNumber);
            Assert.Equal(1, component.HorizontalSamplingFactor);
            Assert.Equal(1, component.VerticalSamplingFactor);
            Assert.Equal("Cr component: Quantization table 1, Sampling factors 1 horiz/1 vert", _directory.GetDescription(JpegDirectory.TagComponentData3));
        }
    }
}
