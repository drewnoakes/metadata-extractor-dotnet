// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegMetadataReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegMetadataReaderTest
    {
        [Fact]
        public void ExtractMetadataUsingPath()
        {
            Validate(JpegMetadataReader.ReadMetadata("Data/withExif.jpg"));
        }

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
