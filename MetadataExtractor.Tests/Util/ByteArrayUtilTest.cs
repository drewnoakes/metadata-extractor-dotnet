// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Xunit;

using MetadataExtractor.Util;

namespace MetadataExtractor.Tests.Util
{
    public class ByteArrayUtilTest
    {
        [Fact]
        public void StartsWith()
        {
            var bytes = new byte[] { 0, 1, 2, 3 };

            Assert.True(bytes.StartsWith(new byte[] { }));
            Assert.True(bytes.StartsWith(new byte[] { 0 }));
            Assert.True(bytes.StartsWith(new byte[] { 0, 1 }));
            Assert.True(bytes.StartsWith(new byte[] { 0, 1, 2 }));
            Assert.True(bytes.StartsWith(new byte[] { 0, 1, 2, 3 }));
            Assert.False(bytes.StartsWith(new byte[] { 0, 1, 2, 3, 4 }));
            Assert.False(bytes.StartsWith(new byte[] { 1 }));
            Assert.False(bytes.StartsWith(new byte[] { 1, 2 }));
        }
    }
}
