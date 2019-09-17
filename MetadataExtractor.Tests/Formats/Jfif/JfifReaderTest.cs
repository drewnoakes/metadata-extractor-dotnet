// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jfif
{
    /// <summary>Unit tests for <see cref="JfifReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JfifReaderTest
    {
        [Fact]
        public void Extract()
        {
            var jfifData = new byte[] { 74, 70, 73, 70, 0, 1, 2, 1, 0, 108, 0, 108, 0, 0 };

            var directory = new JfifReader().Extract(new ByteArrayReader(jfifData));

            Assert.NotNull(directory);
            Assert.False(directory.HasError, directory.Errors.ToString());

            var tags = directory.Tags.ToList();

            Assert.Equal(6, tags.Count);
            Assert.Equal(JfifDirectory.TagVersion, tags[0].Type);
            Assert.Equal(0x0102, directory.GetInt32(tags[0].Type));
            Assert.Equal(JfifDirectory.TagUnits, tags[1].Type);
            Assert.Equal(1, directory.GetInt32(tags[1].Type));
            Assert.Equal(JfifDirectory.TagResX, tags[2].Type);
            Assert.Equal(108, directory.GetInt32(tags[2].Type));
            Assert.Equal(JfifDirectory.TagResY, tags[3].Type);
            Assert.Equal(108, directory.GetInt32(tags[3].Type));
            Assert.Equal(JfifDirectory.TagThumbWidth, tags[4].Type);
            Assert.Equal(0, directory.GetInt32(tags[4].Type));
            Assert.Equal(JfifDirectory.TagThumbHeight, tags[5].Type);
            Assert.Equal(0, directory.GetInt32(tags[5].Type));
        }
    }
}
