// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegComponent"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegComponentTest
    {
        [Fact]
        public void GetComponentCharacter()
        {
            Assert.Equal("Y",  new JpegComponent(1, 2, 3).Name);
            Assert.Equal("Cb", new JpegComponent(2, 2, 3).Name);
            Assert.Equal("Cr", new JpegComponent(3, 2, 3).Name);
            Assert.Equal("I",  new JpegComponent(4, 2, 3).Name);
            Assert.Equal("Q",  new JpegComponent(5, 2, 3).Name);
        }
    }
}
