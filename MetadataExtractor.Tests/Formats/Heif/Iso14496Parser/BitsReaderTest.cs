using System;
using System.Runtime.InteropServices;
using MetadataExtractor.Formats.Heif.Iso14496Parser;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Heif.Iso14496Parser
{
    public class BitsReaderTest
    {
        public BitsReader CreateReader(params byte[] sourceData)
        {
            var sr = new SequentialByteArrayReader(sourceData);
            return new BitsReader(sr);
        }
        [Theory]
        [InlineData(1,1)]
        [InlineData(2,3)]
        [InlineData(3,7)]
        [InlineData(4,15)]
        [InlineData(5,31)]
        [InlineData(6,63)]
        [InlineData(7,127)]
        [InlineData(8,255)]
        [InlineData(9,511)]
        public void GetOneBit(int bits, UInt32 result)
        {
            var reader = CreateReader(0xFF, 0xFF);
            Assert.Equal(result, reader.GetUInt32(bits));

            var reader2 = CreateReader(0, 0);
            Assert.Equal(0u, reader2.GetUInt32(bits));
        }

        [Fact]
        public void ReadNibbles()
        {
            var reader = CreateReader(0x12, 0x34);
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(i+1, reader.GetByte(4));

            }
        }
    }
}
