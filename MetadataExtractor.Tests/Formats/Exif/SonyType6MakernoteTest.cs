// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for Sony (Type 6) maker notes.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SonyType6MakernoteTest
    {
        [Fact]
        public void File1()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<SonyType6MakernoteDirectory>("Data/sonyType6.jpg.app1.0", JpegSegmentType.App1);
            Assert.NotNull(directory);
            Assert.False(directory.HasError);
            var descriptor = new SonyType6MakernoteDescriptor(directory);
            Assert.Equal("2.00", descriptor.GetMakernoteThumbVersionDescription());
        }
    }
}
