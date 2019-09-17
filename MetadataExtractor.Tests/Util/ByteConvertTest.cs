// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Util;
using Xunit;

namespace MetadataExtractor.Tests.Util
{
    /// <summary>Unit tests for <see cref="ByteConvert"/>.</summary>
    public class ByteConvertTest
    {
        [Fact]
        public void ToInt32BigEndian()
        {
            Assert.Equal(0x01020304, ByteConvert.ToInt32BigEndian(new byte[] { 1, 2, 3, 4 }));
            Assert.Equal(0x01020304, ByteConvert.ToInt32BigEndian(new byte[] { 1, 2, 3, 4, 5 }));
        }

        [Fact]
        public void ToInt32LittleEndian()
        {
            Assert.Equal(0x04030201, ByteConvert.ToInt32LittleEndian(new byte[] { 1, 2, 3, 4 }));
            Assert.Equal(0x04030201, ByteConvert.ToInt32LittleEndian(new byte[] { 1, 2, 3, 4, 5 }));
        }
    }
}
