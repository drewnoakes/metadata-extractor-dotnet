using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    public sealed class RasUsageTests
    {
        /// <summary>
        /// Wraps normal usage with ReaderInfo static methods, bypassing some complexity of a RandomAccessStream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private ReaderInfo CreateReader(params byte[] bytes)
        {
            return ReaderInfo.CreateFromStream(new MemoryStream(bytes));
        }

        [Fact]
        public void TestRasByteArray()
        {
            var bytes = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            var ras = new RandomAccessStream(bytes);
            // byte array inputs should always be seekable
            Assert.True(ras.CanSeek);
            // same byte array reference, so lengths should match
            Assert.Equal(bytes.Length, ras.Length);

            // bytes two thru five in motorola byte order
            Assert.Equal(33752069, ras.GetInt32(2, true, false));

            // bytes two thru five in non-motorola byte order
            Assert.Equal(84148994, ras.GetInt32(2, false, false));
        }

        [Fact]
        public void TestRasMemoryStream()
        {
            var ms = new MemoryStream(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });

            var ras = new RandomAccessStream(ms);
            // memory stream inputs should always be seekable
            Assert.True(ras.CanSeek);
            // same byte array reference, so lengths should match
            Assert.Equal(ms.Length, ras.Length);

            // first four bytes in motorola byte order
            Assert.Equal(66051, ras.GetInt32(0, true, false));

            // first four bytes in non-motorola byte order
            Assert.Equal(50462976, ras.GetInt32(0, false, false));
        }

        [Fact]
        public void TestNonSeekableRasStreamUnknownLength()
        {
            var nss = new NonSeekableStream(new MemoryStream(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }));

            // don't supply a length
            var ras = new RandomAccessStream(nss);

            // nonseekable stream inputs shouldn't be seekable
            Assert.False(ras.CanSeek);
            // can't know the length of a nonseekable stream if it isn't supplied
            Assert.Equal(RandomAccessStream.UnknownLengthValue, ras.Length);
        }

        [Fact]
        public void TestNonSeekableRasStreamKnownLength()
        {
            var ms = new MemoryStream(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
            var nss = new NonSeekableStream(ms);

            // supply a length
            var ras = new RandomAccessStream(nss, ms.Length);

            // nonseekable stream inputs shouldn't be seekable
            Assert.False(ras.CanSeek);
            // length of a nonseekable stream was supplied to the RAS constructor
            Assert.Equal(ms.Length, ras.Length);
        }

        [Fact]
        public void LocalPositionUnchangedIndexed()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);

            // indexed read should not change the local position
            var _ = reader.GetBytes(0, 4);
            Assert.Equal(0, reader.LocalPosition);
        }

        [Fact]
        public void LocalPositionChangedSequential()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);

            // sequential reads should change the local position by the number of bytes read
            var _ = reader.GetBytes(4);
            Assert.Equal(4, reader.LocalPosition);
        }

        [Fact]
        public void SkipSequential()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);

            // move local position
            reader.Skip(6);

            // sequential reads should change the local position by the number of bytes read
            Assert.Equal(0x06, reader.GetByte());
            Assert.Equal(7, reader.LocalPosition);
        }

        [Fact]
        public void IndexedAndSequentialReadsOnSameReader()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);

            // sequential read should change the local position by the number of bytes read
            _ = reader.GetBytes(5);

            Assert.Equal(5, reader.LocalPosition);

            // indexed read should not use or change the local position
            Assert.Equal(0x02, reader.GetByte(2));

            Assert.Equal(5, reader.LocalPosition);
        }

        [Fact]
        public void CloneReader()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
            var clone = reader.Clone();

            // both readers should have the same starting (global) position
            Assert.Equal(0, reader.StartPosition);
            Assert.Equal(reader.StartPosition, clone.StartPosition);
        }

        [Fact]
        public void CloneReaderOffset()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
            var clone = reader.Clone(4, true);

            // readers should have different starting (global) positions
            Assert.Equal(0, reader.StartPosition);
            Assert.Equal(4, clone.StartPosition);

            Assert.NotEqual(reader.StartPosition, clone.StartPosition);
        }

        [Fact]
        public void CloneReaderOffsetCheckLength()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
            var clone = reader.Clone(7, true);

            // clone should start at a different index and have a different length
            Assert.Equal(0, reader.StartPosition);
            Assert.Equal(7, clone.StartPosition);

            Assert.Equal(2, clone.Length);

            // bytes orders should be the same because of the Clone overload used
            Assert.Equal(reader.IsMotorolaByteOrder, clone.IsMotorolaByteOrder);
        }

        [Fact]
        public void CloneReaderEnsureDistinct()
        {
            var reader = CreateReader(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
            var clone = reader.Clone(4, true);

            Assert.Equal(0, clone.LocalPosition);
            // sequentially move the reader's local position
            reader.Skip(2);

            // reader's local position should be changed
            Assert.Equal(2, reader.LocalPosition);

            // clone's local position should be unchanged
            Assert.Equal(0, clone.LocalPosition);
        }



        [Fact]
        public void GetNullTerminatedStringWithClone()
        {
            var reader = CreateReader(0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47);
            // clone it from a different index
            var clone = reader.Clone(1, true);

            Assert.Equal("A", reader.GetNullTerminatedString(0, 1));
            Assert.Equal("B", clone.GetNullTerminatedString(0, 1));

            Assert.Equal("BCD", reader.GetNullTerminatedString(1, 3));
            Assert.Equal("BCD", clone.GetNullTerminatedString(0, 3));

            Assert.Equal(reader.GetNullTerminatedString(1, 3), clone.GetNullTerminatedString(0, 3));
        }

    }
}
