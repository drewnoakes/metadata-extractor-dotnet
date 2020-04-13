// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    public sealed class BitReaderTest
    {
        private static BitReader CreateReader(params byte[] sourceData)
        {
            var sr = new SequentialByteArrayReader(sourceData);
            return new BitReader(sr);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        [InlineData(3, 7)]
        [InlineData(4, 15)]
        [InlineData(5, 31)]
        [InlineData(6, 63)]
        [InlineData(7, 127)]
        [InlineData(8, 255)]
        [InlineData(9, 511)]
        public void GetOneBit(int bits, ushort result)
        {
            Assert.Equal(result, CreateReader(0xFF, 0xFF).GetUInt32(bits));

            Assert.Equal(0u, CreateReader(0, 0).GetUInt32(bits));
        }

        [Fact]
        public void ReadNibbles()
        {
            var reader = CreateReader(0x12, 0x34);

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(i + 1, reader.GetByte(4));
            }
        }
    }
}
