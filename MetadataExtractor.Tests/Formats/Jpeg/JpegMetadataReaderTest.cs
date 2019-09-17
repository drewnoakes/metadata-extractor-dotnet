// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegMetadataReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegMetadataReaderTest
    {
#if !NETCOREAPP1_0
        [Fact]
        public void ExtractMetadataUsingPath()
        {
            Validate(JpegMetadataReader.ReadMetadata("Data/withExif.jpg"));
        }
#endif

        [Fact]
        public void ExtractMetadataUsingStream()
        {
            using var stream = TestDataUtil.OpenRead("Data/withExif.jpg");
            Validate(JpegMetadataReader.ReadMetadata(stream));
        }

        private static void Validate(IEnumerable<Directory> metadata)
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            Assert.NotNull(directory);
            Assert.Equal("80", directory.GetString(ExifDirectoryBase.TagIsoEquivalent));
        }
    }
}
