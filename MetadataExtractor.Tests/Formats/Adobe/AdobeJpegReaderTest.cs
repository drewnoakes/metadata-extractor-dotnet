// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Adobe
{
    /// <summary>Unit tests for <see cref="AdobeJpegReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AdobeJpegReaderTest
    {
        private static AdobeJpegDirectory ProcessBytes(string filePath)
        {
            return new AdobeJpegReader()
                .Extract(new SequentialByteArrayReader(File.ReadAllBytes(TestDataUtil.GetPath(filePath))));
        }

        [Fact]
        public void SegmentTypes()
        {
            Assert.Equal(
                new[] { JpegSegmentType.AppE },
                ((IJpegSegmentMetadataReader)new AdobeJpegReader()).SegmentTypes);
        }

        [Fact]
        public void ReadAdobeJpegMetadata1()
        {
            var directory = ProcessBytes("Data/adobeJpeg1.jpg.appe");

            Assert.False(directory.HasError, directory.Errors.ToString());
            Assert.Equal(4, directory.TagCount);
            Assert.Equal(1, directory.GetInt32(AdobeJpegDirectory.TagColorTransform));
            Assert.Equal(25600, directory.GetInt32(AdobeJpegDirectory.TagDctEncodeVersion));
            Assert.Equal(128, directory.GetInt32(AdobeJpegDirectory.TagApp14Flags0));
            Assert.Equal(0, directory.GetInt32(AdobeJpegDirectory.TagApp14Flags1));
        }
    }
}
