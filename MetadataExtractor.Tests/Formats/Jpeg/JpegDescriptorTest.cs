// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegDescriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegDescriptorTest
    {
        private readonly JpegDirectory _directory;
        private readonly JpegDescriptor _descriptor;

        public JpegDescriptorTest()
        {
            _directory = new JpegDirectory();
            _descriptor = new JpegDescriptor(_directory);
        }

        [Fact]
        public void GetComponentDataDescription_InvalidComponentNumber()
        {
            Assert.Null(_descriptor.GetComponentDataDescription(1));
        }

        [Fact]
        public void GetImageWidthDescription()
        {
            _directory.Set(JpegDirectory.TagImageWidth, 123);

            Assert.Equal("123 pixels", _descriptor.GetImageWidthDescription());
            Assert.Equal("123 pixels", _directory.GetDescription(JpegDirectory.TagImageWidth));
        }

        [Fact]
        public void GetImageHeightDescription()
        {
            _directory.Set(JpegDirectory.TagImageHeight, 123);

            Assert.Equal("123 pixels", _descriptor.GetImageHeightDescription());
            Assert.Equal("123 pixels", _directory.GetDescription(JpegDirectory.TagImageHeight));
        }

        [Fact]
        public void GetDataPrecisionDescription()
        {
            _directory.Set(JpegDirectory.TagDataPrecision, 8);

            Assert.Equal("8 bits", _descriptor.GetDataPrecisionDescription());
            Assert.Equal("8 bits", _directory.GetDescription(JpegDirectory.TagDataPrecision));
        }

        [Fact]
        public void GetComponentDescription()
        {
            _directory.Set(JpegDirectory.TagComponentData1, new JpegComponent(1, 0x22, 0));

            Assert.Equal("Y component: Quantization table 0, Sampling factors 2 horiz/2 vert", _directory.GetDescription(JpegDirectory.TagComponentData1));
            Assert.Equal("Y component: Quantization table 0, Sampling factors 2 horiz/2 vert", _descriptor.GetComponentDataDescription(0));
        }
    }
}
