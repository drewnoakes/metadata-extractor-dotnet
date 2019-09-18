// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for <see cref="ExifThumbnailDescriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifThumbnailDescriptorTest
    {
        [Fact]
        public void GetYCbCrSubsamplingDescription()
        {
            var directory = new ExifThumbnailDirectory();
            var descriptor = new ExifThumbnailDescriptor(directory);

            directory.Set(ExifDirectoryBase.TagYCbCrSubsampling, new[] { 2, 1 });
            Assert.Equal("YCbCr4:2:2", descriptor.GetDescription(ExifDirectoryBase.TagYCbCrSubsampling));
            Assert.Equal("YCbCr4:2:2", descriptor.GetYCbCrSubsamplingDescription());

            directory.Set(ExifDirectoryBase.TagYCbCrSubsampling, new[] { 2, 2 });
            Assert.Equal("YCbCr4:2:0", descriptor.GetDescription(ExifDirectoryBase.TagYCbCrSubsampling));
            Assert.Equal("YCbCr4:2:0", descriptor.GetYCbCrSubsamplingDescription());
        }
    }
}
