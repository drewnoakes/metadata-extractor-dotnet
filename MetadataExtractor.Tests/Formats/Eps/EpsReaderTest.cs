// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MetadataExtractor.Formats.Eps;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Eps
{
    /// <summary>Unit tests for <see cref="EpsReader"/>.</summary>
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class EpsReaderTest
    {
        private static EpsDirectory ProcessBytes(string filePath)
        {
            using var stream = TestDataUtil.OpenRead(filePath);
            return new EpsReader().Extract(stream).OfType<EpsDirectory>().First();
        }

        [Fact]
        public void Test8x8x8bitGrayscale()
        {
            EpsDirectory directory = ProcessBytes("Data/8x4x8bit-Grayscale.eps");

            Assert.Equal(4334, directory.GetInt32(EpsDirectory.TagTiffPreviewSize));
            Assert.Equal(30, directory.GetInt32(EpsDirectory.TagTiffPreviewOffset));
            Assert.Equal(8, directory.GetInt32(EpsDirectory.TagImageWidth));
            Assert.Equal(4, directory.GetInt32(EpsDirectory.TagImageHeight));
            Assert.Equal(1, directory.GetInt32(EpsDirectory.TagColorType));
        }

        [Fact]
        public void TestAdobeJpeg1()
        {
            EpsDirectory directory = ProcessBytes("Data/adobeJpeg1.eps");

            Assert.Equal(41802, directory.GetInt32(EpsDirectory.TagTiffPreviewSize));
            Assert.Equal(30, directory.GetInt32(EpsDirectory.TagTiffPreviewOffset));
            Assert.Equal(275, directory.GetInt32(EpsDirectory.TagImageWidth));
            Assert.Equal(207, directory.GetInt32(EpsDirectory.TagImageHeight));
            Assert.Equal(3, directory.GetInt32(EpsDirectory.TagColorType));
        }
    }
}
