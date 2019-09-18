// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for <see cref="ExifInteropDescriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifInteropDescriptorTest
    {
        [Fact]
        public void GetInteropVersionDescription()
        {
            var directory = new ExifInteropDirectory();
            directory.Set(ExifDirectoryBase.TagInteropVersion, new[] { 0, 1, 0, 0 });

            var descriptor = new ExifInteropDescriptor(directory);
            Assert.Equal("1.00", descriptor.GetDescription(ExifDirectoryBase.TagInteropVersion));
            Assert.Equal("1.00", descriptor.GetInteropVersionDescription());
        }

        [Fact]
        public void GetInteropIndexDescription()
        {
            var directory = new ExifInteropDirectory();
            directory.Set(ExifDirectoryBase.TagInteropIndex, "R98");

            var descriptor = new ExifInteropDescriptor(directory);
            Assert.Equal("Recommended Exif Interoperability Rules (ExifR98)", descriptor.GetDescription(ExifDirectoryBase.TagInteropIndex));
            Assert.Equal("Recommended Exif Interoperability Rules (ExifR98)", descriptor.GetInteropIndexDescription());
        }
    }
}
