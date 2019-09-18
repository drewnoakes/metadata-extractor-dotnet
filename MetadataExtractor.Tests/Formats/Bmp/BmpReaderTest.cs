// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Bmp
{
    /// <summary>Unit tests for <see cref="BmpReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class BmpReaderTest
    {
        private static BmpHeaderDirectory ProcessBytes(string filePath)
        {
            using var stream = TestDataUtil.OpenRead(filePath);
            return new BmpReader().Extract(new SequentialStreamReader(stream)).OfType<BmpHeaderDirectory>().First();
        }

        [Fact]
        public void MsPaint16Color()
        {
            var directory = ProcessBytes("Data/16color-10x10.bmp");
            Assert.False(directory.HasError);
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageWidth));
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageHeight));
            Assert.Equal(4, directory.GetInt32(BmpHeaderDirectory.TagBitsPerPixel));
            Assert.Equal("None", directory.GetDescription(BmpHeaderDirectory.TagCompression));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagXPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagYPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagPaletteColourCount));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagImportantColourCount));
            Assert.Equal(1, directory.GetInt32(BmpHeaderDirectory.TagColourPlanes));
            Assert.Equal(40, directory.GetInt32(BmpHeaderDirectory.TagHeaderSize));
        }

        [Fact]
        public void MsPaint24Bpp()
        {
            var directory = ProcessBytes("Data/24bpp-10x10.bmp");
            Assert.False(directory.HasError);
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageWidth));
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageHeight));
            Assert.Equal(24, directory.GetInt32(BmpHeaderDirectory.TagBitsPerPixel));
            Assert.Equal("None", directory.GetDescription(BmpHeaderDirectory.TagCompression));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagXPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagYPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagPaletteColourCount));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagImportantColourCount));
            Assert.Equal(1, directory.GetInt32(BmpHeaderDirectory.TagColourPlanes));
            Assert.Equal(40, directory.GetInt32(BmpHeaderDirectory.TagHeaderSize));
        }
    }
}
