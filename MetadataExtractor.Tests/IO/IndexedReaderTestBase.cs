/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>
    /// Base class for testing implementations of <see cref="IndexedReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class IndexedReaderTestBase
    {
        protected abstract IndexedReader CreateReader(byte[] bytes);

        [Fact]
        public void TestDefaultEndianness()
        {
            Assert.Equal(true, CreateReader(new byte[1]).IsMotorolaByteOrder);
        }

        [Fact]
        public void TestGetSByte()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.Equal(0, reader.GetSByte(0));
            Assert.Equal(1, reader.GetSByte(1));
            Assert.Equal(127, reader.GetSByte(2));
            Assert.Equal(-1, reader.GetSByte(3));
        }

        [Fact]
        public void TestGetByte()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.Equal(0, reader.GetByte(0));
            Assert.Equal(1, reader.GetByte(1));
            Assert.Equal(127, reader.GetByte(2));
            Assert.Equal(255, reader.GetByte(3));
        }

        [Fact]
        public void TestGetByte_OutOfBounds()
        {
            var reader = CreateReader(new byte[2]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetByte(2));
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 2, requested count: 1, max index: 1)", ex.Message);
        }


        [Fact]
        public void TestGetInt16()
        {
            Assert.Equal(-1, CreateReader(new[] { (byte)0xff, (byte)0xff }).GetInt16(0));
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.Equal(0x0001, reader.GetInt16(0));
            Assert.Equal(0x017F, reader.GetInt16(1));
            Assert.Equal(0x7FFF, reader.GetInt16(2));
            reader.IsMotorolaByteOrder = false;
            Assert.Equal(0x0100, reader.GetInt16(0));
            Assert.Equal(0x7F01, reader.GetInt16(1));
            Assert.Equal(unchecked((short)(0xFF7F)), reader.GetInt16(2));
        }


        [Fact]
        public void TestGetUInt16()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.Equal(0x0001, reader.GetUInt16(0));
            Assert.Equal(0x017F, reader.GetUInt16(1));
            Assert.Equal(0x7FFF, reader.GetUInt16(2));
            reader.IsMotorolaByteOrder = false;
            Assert.Equal(0x0100, reader.GetUInt16(0));
            Assert.Equal(0x7F01, reader.GetUInt16(1));
            Assert.Equal(0xFF7F, reader.GetUInt16(2));
        }

        [Fact]
        public void TestGetUInt16_OutOfBounds()
        {
            var reader = CreateReader(new byte[2]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetUInt16(1));
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 1, requested count: 2, max index: 1)", ex.Message);
        }


        [Fact]
        public void TestGetInt32()
        {
            Assert.Equal(-1, CreateReader(new[] { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff }).GetInt32(0));
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF, 0x02, 0x03, 0x04 };
            var reader = CreateReader(buffer);
            Assert.Equal(0x00017FFF, reader.GetInt32(0));
            Assert.Equal(0x017FFF02, reader.GetInt32(1));
            Assert.Equal(0x7FFF0203, reader.GetInt32(2));
            Assert.Equal(unchecked((int)(0xFF020304)), reader.GetInt32(3));
            reader.IsMotorolaByteOrder = false;
            Assert.Equal(unchecked((int)(0xFF7F0100)), reader.GetInt32(0));
            Assert.Equal(0x02FF7F01, reader.GetInt32(1));
            Assert.Equal(0x0302FF7F, reader.GetInt32(2));
            Assert.Equal(0x040302FF, reader.GetInt32(3));
        }

        [Fact]
        public void TestGetUInt32()
        {
            Assert.Equal(4294967295u, CreateReader(new byte[] { 0xff, 0xff, 0xff, 0xff }).GetUInt32(0));

            var reader = CreateReader(new byte[] { 0x00, 0x01, 0x7F, 0xFF, 0x02, 0x03, 0x04 });

            Assert.Equal(0x00017FFFu, reader.GetUInt32(0));
            Assert.Equal(0x017FFF02u, reader.GetUInt32(1));
            Assert.Equal(0x7FFF0203u, reader.GetUInt32(2));
            Assert.Equal(0xFF020304u, reader.GetUInt32(3));

            reader.IsMotorolaByteOrder = false;

            Assert.Equal(4286513408u, reader.GetUInt32(0));
            Assert.Equal(0x02FF7F01u, reader.GetUInt32(1));
            Assert.Equal(0x0302FF7Fu, reader.GetUInt32(2));
            Assert.Equal(0x040302FFu, reader.GetUInt32(3));
        }

        [Fact]
        public void TestGetInt32_OutOfBounds()
        {
            var reader = CreateReader(new byte[3]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetInt32(0));
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 4, max index: 2)", ex.Message);
        }

        /// <exception cref="System.IO.IOException"/>
        [Fact]
        public void TestGetInt64()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, unchecked(0xFF
                ) };
            var reader = CreateReader(buffer);
            Assert.Equal(unchecked(0x0001020304050607L), (object)reader.GetInt64(0));
            Assert.Equal(unchecked(0x01020304050607FFL), (object)reader.GetInt64(1));
            reader.IsMotorolaByteOrder = false;
            Assert.Equal(unchecked(0x0706050403020100L), (object)reader.GetInt64(0));
            Assert.Equal(unchecked((long)(0xFF07060504030201L)), (object)reader.GetInt64(1));
        }

        [Fact]
        public void TestGetInt64_OutOfBounds()
        {
            var reader = CreateReader(new byte[7]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetInt64(0));
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 8, max index: 6)", ex.Message);

            reader = CreateReader(new byte[7]);
            ex = Assert.Throws<BufferBoundsException>(() => reader.GetInt64(-1));
            Assert.Equal("Attempt to read from buffer using a negative index (-1)", ex.Message);
        }

        [Fact]
        public void TestGetFloat32()
        {
            var nanBits = 0x7fc00000;
            Assert.True(float.IsNaN(BitConverter.ToSingle(BitConverter.GetBytes(nanBits), 0)));
            var buffer = new byte[] { 0x7f, 0xc0, 0x00, 0x00 };
            var reader = CreateReader(buffer);
            Assert.True(float.IsNaN(reader.GetFloat32(0)));
        }

        [Fact]
        public void TestGetFloat64()
        {
            var nanBits = unchecked((long)(0xfff0000000000001L));
            Assert.True(double.IsNaN(BitConverter.Int64BitsToDouble(nanBits)));
            var buffer = new byte[] { 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
            var reader = CreateReader(buffer);
            Assert.True(double.IsNaN(reader.GetDouble64(0)));
        }

        [Fact]
        public void TestGetNullTerminatedString()
        {
            var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47 };
            var reader = CreateReader(bytes);
            Assert.Equal(string.Empty, reader.GetNullTerminatedString(0, 0));
            Assert.Equal("A", reader.GetNullTerminatedString(0, 1));
            Assert.Equal("AB", reader.GetNullTerminatedString(0, 2));
            Assert.Equal("ABC", reader.GetNullTerminatedString(0, 3));
            Assert.Equal("ABCD", reader.GetNullTerminatedString(0, 4));
            Assert.Equal("ABCD", reader.GetNullTerminatedString(0, 5));
            Assert.Equal("ABCD", reader.GetNullTerminatedString(0, 6));
            Assert.Equal("BCD", reader.GetNullTerminatedString(1, 3));
            Assert.Equal("BCD", reader.GetNullTerminatedString(1, 4));
            Assert.Equal("BCD", reader.GetNullTerminatedString(1, 5));
            Assert.Equal(string.Empty, reader.GetNullTerminatedString(4, 3));
        }

        [Fact]
        public void TestGetString()
        {
            var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47 };
            var reader = CreateReader(bytes);
            Assert.Equal(string.Empty, reader.GetString(0, 0));
            Assert.Equal("A", reader.GetString(0, 1));
            Assert.Equal("AB", reader.GetString(0, 2));
            Assert.Equal("ABC", reader.GetString(0, 3));
            Assert.Equal("ABCD", reader.GetString(0, 4));
            Assert.Equal("ABCD\x0", reader.GetString(0, 5));
            Assert.Equal("ABCD\x0000E", reader.GetString(0, 6));
            Assert.Equal("BCD", reader.GetString(1, 3));
            Assert.Equal("BCD\x0", reader.GetString(1, 4));
            Assert.Equal("BCD\x0000E", reader.GetString(1, 5));
            Assert.Equal("\x0000EF", reader.GetString(4, 3));
        }

        [Fact]
        public void TestIndexPlusCountExceedsIntMaxValue()
        {
            var reader = CreateReader(new byte[10]);
            try
            {
                reader.GetBytes(0x6FFFFFFF, 0x6FFFFFFF);
            }
            catch (IOException e)
            {
                Assert.Equal("Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: 1879048191, requested count: 1879048191)", e.Message);
            }
        }

        [Fact]
        public void TestOverflowBoundsCalculation()
        {
            var reader = CreateReader(new byte[10]);
            try
            {
                reader.GetBytes(5, 10);
            }
            catch (IOException e)
            {
                Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 5, requested count: 10, max index: 9)", e.Message);
            }
        }

        [Fact]
        public void TestGetBytesEof()
        {
            CreateReader(new byte[50]).GetBytes(0, 50);

            var reader = CreateReader(new byte[50]);
            reader.GetBytes(25, 25);

            Assert.Throws<BufferBoundsException>(() => CreateReader(new byte[50]).GetBytes(0, 51));
        }

        [Fact]
        public void TestGetByteEof()
        {
            CreateReader(new byte[1]).GetByte(0);

            var reader = CreateReader(new byte[2]);
            reader.GetByte(0);
            reader.GetByte(1);

            reader = CreateReader(new byte[1]);
            reader.GetByte(0);
            Assert.Throws<BufferBoundsException>(() => reader.GetByte(1));
        }
    }
}
