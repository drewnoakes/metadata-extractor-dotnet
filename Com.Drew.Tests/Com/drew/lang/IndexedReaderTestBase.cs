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
using NUnit.Framework;

namespace Com.Drew.Lang
{
    /// <summary>
    /// Base class for testing implementations of <see cref="IndexedReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class IndexedReaderTestBase
    {
        protected abstract IndexedReader CreateReader(byte[] bytes);

        [Test]
        public virtual void TestDefaultEndianness()
        {
            Assert.AreEqual(true, CreateReader(new byte[1]).IsMotorolaByteOrder);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetInt8()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(0, reader.GetInt8(0));
            Assert.AreEqual(1, reader.GetInt8(1));
            Assert.AreEqual(127, reader.GetInt8(2));
            Assert.AreEqual(-1, reader.GetInt8(3));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetUInt8()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(0, reader.GetUInt8(0));
            Assert.AreEqual(1, reader.GetUInt8(1));
            Assert.AreEqual(127, reader.GetUInt8(2));
            Assert.AreEqual(255, reader.GetUInt8(3));
        }

        [Test]
        public virtual void TestGetUInt8_OutOfBounds()
        {
            try
            {
                IndexedReader reader = CreateReader(new byte[2]);
                reader.GetUInt8(2);
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 2, requested count: 1, max index: 1)", ex.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetInt16()
        {
            Assert.AreEqual(-1, CreateReader(new[] { unchecked((byte)0xff), unchecked((byte)0xff) }).GetInt16(0));
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(0x0001, reader.GetInt16(0));
            Assert.AreEqual(0x017F, reader.GetInt16(1));
            Assert.AreEqual(0x7FFF, reader.GetInt16(2));
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(0x0100, reader.GetInt16(0));
            Assert.AreEqual(0x7F01, reader.GetInt16(1));
            Assert.AreEqual(unchecked((short)(0xFF7F)), reader.GetInt16(2));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetUInt16()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0x0001), reader.GetUInt16(0));
            Assert.AreEqual(unchecked(0x017F), reader.GetUInt16(1));
            Assert.AreEqual(unchecked(0x7FFF), reader.GetUInt16(2));
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(unchecked(0x0100), reader.GetUInt16(0));
            Assert.AreEqual(unchecked(0x7F01), reader.GetUInt16(1));
            Assert.AreEqual(unchecked(0xFF7F), reader.GetUInt16(2));
        }

        [Test]
        public virtual void TestGetUInt16_OutOfBounds()
        {
            try
            {
                IndexedReader reader = CreateReader(new byte[2]);
                reader.GetUInt16(1);
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 1, requested count: 2, max index: 1)", ex.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetInt32()
        {
            Assert.AreEqual(-1, CreateReader(new[] { unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff) }).GetInt32(0));
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF), unchecked(0x02), unchecked(0x03), unchecked(0x04) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0x00017FFF), reader.GetInt32(0));
            Assert.AreEqual(unchecked(0x017FFF02), reader.GetInt32(1));
            Assert.AreEqual(unchecked(0x7FFF0203), reader.GetInt32(2));
            Assert.AreEqual(unchecked((int)(0xFF020304)), reader.GetInt32(3));
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(unchecked((int)(0xFF7F0100)), reader.GetInt32(0));
            Assert.AreEqual(unchecked(0x02FF7F01), reader.GetInt32(1));
            Assert.AreEqual(unchecked(0x0302FF7F), reader.GetInt32(2));
            Assert.AreEqual(unchecked(0x040302FF), reader.GetInt32(3));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetUInt32()
        {
            Assert.AreEqual(4294967295L, (object)CreateReader(new[] { unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff) }).GetUInt32(0));
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF), unchecked(0x02), unchecked(0x03), unchecked(0x04) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0x00017FFFL), (object)reader.GetUInt32(0));
            Assert.AreEqual(unchecked(0x017FFF02L), (object)reader.GetUInt32(1));
            Assert.AreEqual(unchecked(0x7FFF0203L), (object)reader.GetUInt32(2));
            Assert.AreEqual(unchecked(0xFF020304L), (object)reader.GetUInt32(3));
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(4286513408L, (object)reader.GetUInt32(0));
            Assert.AreEqual(unchecked(0x02FF7F01L), (object)reader.GetUInt32(1));
            Assert.AreEqual(unchecked(0x0302FF7FL), (object)reader.GetUInt32(2));
            Assert.AreEqual(unchecked(0x040302FFL), (object)reader.GetInt32(3));
        }

        [Test]
        public virtual void TestGetInt32_OutOfBounds()
        {
            try
            {
                IndexedReader reader = CreateReader(new byte[3]);
                reader.GetInt32(0);
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 4, max index: 2)", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetInt64()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x02), unchecked(0x03), unchecked(0x04), unchecked(0x05), unchecked(0x06), unchecked(0x07), unchecked((byte)0xFF
                ) };
            IndexedReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0x0001020304050607L), (object)reader.GetInt64(0));
            Assert.AreEqual(unchecked(0x01020304050607FFL), (object)reader.GetInt64(1));
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(unchecked(0x0706050403020100L), (object)reader.GetInt64(0));
            Assert.AreEqual(unchecked((long)(0xFF07060504030201L)), (object)reader.GetInt64(1));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetInt64_OutOfBounds()
        {
            try
            {
                IndexedReader reader = CreateReader(new byte[7]);
                reader.GetInt64(0);
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 8, max index: 6)", ex.Message);
            }
            try
            {
                IndexedReader reader = CreateReader(new byte[7]);
                reader.GetInt64(-1);
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("Attempt to read from buffer using a negative index (-1)", ex.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetFloat32()
        {
            int nanBits = unchecked(0x7fc00000);
            Assert.IsTrue(float.IsNaN(BitConverter.ToSingle(BitConverter.GetBytes(nanBits), 0)));
            byte[] buffer = new byte[] { unchecked(0x7f), unchecked((byte)0xc0), unchecked(0x00), unchecked(0x00) };
            IndexedReader reader = CreateReader(buffer);
            Assert.IsTrue(float.IsNaN(reader.GetFloat32(0)));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetFloat64()
        {
            long nanBits = unchecked((long)(0xfff0000000000001L));
            Assert.IsTrue(double.IsNaN(BitConverter.Int64BitsToDouble(nanBits)));
            byte[] buffer = new byte[] { unchecked((byte)0xff), unchecked((byte)0xf0), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x01) };
            IndexedReader reader = CreateReader(buffer);
            Assert.IsTrue(double.IsNaN(reader.GetDouble64(0)));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetNullTerminatedString()
        {
            byte[] bytes = new byte[] { unchecked(0x41), unchecked(0x42), unchecked(0x43), unchecked(0x44), unchecked(0x00), unchecked(0x45), unchecked(0x46), unchecked(0x47) };
            IndexedReader reader = CreateReader(bytes);
            Assert.AreEqual(string.Empty, reader.GetNullTerminatedString(0, 0));
            Assert.AreEqual("A", reader.GetNullTerminatedString(0, 1));
            Assert.AreEqual("AB", reader.GetNullTerminatedString(0, 2));
            Assert.AreEqual("ABC", reader.GetNullTerminatedString(0, 3));
            Assert.AreEqual("ABCD", reader.GetNullTerminatedString(0, 4));
            Assert.AreEqual("ABCD", reader.GetNullTerminatedString(0, 5));
            Assert.AreEqual("ABCD", reader.GetNullTerminatedString(0, 6));
            Assert.AreEqual("BCD", reader.GetNullTerminatedString(1, 3));
            Assert.AreEqual("BCD", reader.GetNullTerminatedString(1, 4));
            Assert.AreEqual("BCD", reader.GetNullTerminatedString(1, 5));
            Assert.AreEqual(string.Empty, reader.GetNullTerminatedString(4, 3));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetString()
        {
            byte[] bytes = new byte[] { unchecked(0x41), unchecked(0x42), unchecked(0x43), unchecked(0x44), unchecked(0x00), unchecked(0x45), unchecked(0x46), unchecked(0x47) };
            IndexedReader reader = CreateReader(bytes);
            Assert.AreEqual(string.Empty, reader.GetString(0, 0));
            Assert.AreEqual("A", reader.GetString(0, 1));
            Assert.AreEqual("AB", reader.GetString(0, 2));
            Assert.AreEqual("ABC", reader.GetString(0, 3));
            Assert.AreEqual("ABCD", reader.GetString(0, 4));
            Assert.AreEqual("ABCD\x0", reader.GetString(0, 5));
            Assert.AreEqual("ABCD\x0000E", reader.GetString(0, 6));
            Assert.AreEqual("BCD", reader.GetString(1, 3));
            Assert.AreEqual("BCD\x0", reader.GetString(1, 4));
            Assert.AreEqual("BCD\x0000E", reader.GetString(1, 5));
            Assert.AreEqual("\x0000EF", reader.GetString(4, 3));
        }

        [Test]
        public virtual void TestIndexPlusCountExceedsIntMaxValue()
        {
            IndexedReader reader = CreateReader(new byte[10]);
            try
            {
                reader.GetBytes(unchecked(0x6FFFFFFF), unchecked(0x6FFFFFFF));
            }
            catch (IOException e)
            {
                Assert.AreEqual("Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: 1879048191, requested count: 1879048191)", e.Message);
            }
        }

        [Test]
        public virtual void TestOverflowBoundsCalculation()
        {
            IndexedReader reader = CreateReader(new byte[10]);
            try
            {
                reader.GetBytes(5, 10);
            }
            catch (IOException e)
            {
                Assert.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 5, requested count: 10, max index: 9)", e.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetBytesEof()
        {
            CreateReader(new byte[50]).GetBytes(0, 50);
            IndexedReader reader = CreateReader(new byte[50]);
            reader.GetBytes(25, 25);
            try
            {
                CreateReader(new byte[50]).GetBytes(0, 51);
                Assert.Fail("Expecting exception");
            }
            catch (IOException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetInt8Eof()
        {
            CreateReader(new byte[1]).GetInt8(0);
            IndexedReader reader = CreateReader(new byte[2]);
            reader.GetInt8(0);
            reader.GetInt8(1);
            try
            {
                reader = CreateReader(new byte[1]);
                reader.GetInt8(0);
                reader.GetInt8(1);
                Assert.Fail("Expecting exception");
            }
            catch (IOException)
            {
            }
        }
    }
}
