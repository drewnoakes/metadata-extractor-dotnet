// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Gif
{
    /// <summary>Unit tests for <see cref="GifReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GifReaderTest
    {
        private static IEnumerable<Directory> ProcessBytes(string file)
        {
            using var stream = TestDataUtil.OpenRead(file);
            return new GifReader().Extract(new SequentialStreamReader(stream));
        }

        [Fact]
        public void MsPaintGif()
        {
            var directory = ProcessBytes("Data/mspaint-10x10.gif").OfType<GifHeaderDirectory>().Single();

            Assert.False(directory.HasError);

            Assert.Equal("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Assert.Equal(10, directory.GetInt32(GifHeaderDirectory.TagImageWidth));
            Assert.Equal(10, directory.GetInt32(GifHeaderDirectory.TagImageHeight));
            Assert.Equal(256, directory.GetInt32(GifHeaderDirectory.TagColorTableSize));
            Assert.False(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Assert.Equal(8, directory.GetInt32(GifHeaderDirectory.TagBitsPerPixel));
            Assert.True(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Assert.Equal(0, directory.GetInt32(GifHeaderDirectory.TagBackgroundColorIndex));
        }

        [Fact]
        public void PhotoshopGif()
        {
            var directory = ProcessBytes("Data/photoshop-8x12-32colors-alpha.gif").OfType<GifHeaderDirectory>().Single();

            Assert.False(directory.HasError);

            Assert.Equal("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Assert.Equal(8, directory.GetInt32(GifHeaderDirectory.TagImageWidth));
            Assert.Equal(12, directory.GetInt32(GifHeaderDirectory.TagImageHeight));
            Assert.Equal(32, directory.GetInt32(GifHeaderDirectory.TagColorTableSize));
            Assert.False(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Assert.Equal(5, directory.GetInt32(GifHeaderDirectory.TagBitsPerPixel));
            Assert.True(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Assert.Equal(8, directory.GetInt32(GifHeaderDirectory.TagBackgroundColorIndex));
        }
    }
}
