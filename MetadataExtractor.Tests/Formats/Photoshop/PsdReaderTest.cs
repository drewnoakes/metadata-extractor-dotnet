// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Photoshop
{
    /// <summary>Unit tests for <see cref="PsdReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PsdReaderTest
    {
        private static PsdHeaderDirectory ProcessBytes(string filePath)
        {
            using var stream = TestDataUtil.OpenRead(filePath);
            var directory = new PsdReader().Extract(new SequentialStreamReader(stream)).OfType<PsdHeaderDirectory>().FirstOrDefault();
            Assert.NotNull(directory);
            return directory;
        }

        [Fact]
        public void Psd8X8X8BitGrayscale()
        {
            var directory = ProcessBytes("Data/8x4x8bit-Grayscale.psd");
            Assert.Equal(8, directory.GetInt32(PsdHeaderDirectory.TagImageWidth));
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagImageHeight));
            Assert.Equal(8, directory.GetInt32(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.Equal(1, directory.GetInt32(PsdHeaderDirectory.TagChannelCount));
            // 1 = grayscale
            Assert.Equal(1, directory.GetInt32(PsdHeaderDirectory.TagColorMode));
        }

        [Fact]
        public void Psd10X12X16BitCmyk()
        {
            var directory = ProcessBytes("Data/10x12x16bit-CMYK.psd");
            Assert.Equal(10, directory.GetInt32(PsdHeaderDirectory.TagImageWidth));
            Assert.Equal(12, directory.GetInt32(PsdHeaderDirectory.TagImageHeight));
            Assert.Equal(16, directory.GetInt32(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagChannelCount));
            // 4 = CMYK
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagColorMode));
        }
    }
}
