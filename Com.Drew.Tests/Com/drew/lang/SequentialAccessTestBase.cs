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

using System.IO;
using System.Text;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>
    /// Base class for testing implementations of <see cref="SequentialReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class SequentialAccessTestBase
    {
        protected abstract SequentialReader CreateReader(byte[] bytes);

        [Test]
        public virtual void TestDefaultEndianness()
        {
            Assert.AreEqual(true, CreateReader(new byte[1]).IsMotorolaByteOrder());
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetInt8()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0), reader.GetInt8());
            Assert.AreEqual(unchecked(1), reader.GetInt8());
            Assert.AreEqual(unchecked(127), reader.GetInt8());
            Assert.AreEqual(unchecked((byte)255), reader.GetInt8());
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetUInt8()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(0, reader.GetUInt8());
            Assert.AreEqual(1, reader.GetUInt8());
            Assert.AreEqual(127, reader.GetUInt8());
            Assert.AreEqual(255, reader.GetUInt8());
        }

        [Test]
        public virtual void TestGetUInt8_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new byte[1]);
                reader.GetUInt8();
                reader.GetUInt8();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetInt16()
        {
            Assert.AreEqual(-1, CreateReader(new byte[] { unchecked((byte)0xff), unchecked((byte)0xff) }).GetInt16());
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(0x0001, reader.GetInt16());
            Assert.AreEqual(0x7FFF, reader.GetInt16());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Assert.AreEqual(0x0100, reader.GetInt16());
            Assert.AreEqual(unchecked((short)(0xFF7F)), reader.GetInt16());
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetUInt16()
        {
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x7F), unchecked((byte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0x0001), reader.GetUInt16());
            Assert.AreEqual(unchecked(0x7FFF), reader.GetUInt16());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Assert.AreEqual(unchecked(0x0100), reader.GetUInt16());
            Assert.AreEqual(unchecked(0xFF7F), reader.GetUInt16());
        }

        [Test]
        public virtual void TestGetUInt16_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new byte[1]);
                reader.GetUInt16();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetInt32()
        {
            Assert.AreEqual(-1, CreateReader(new byte[] { unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff) }).GetInt32());
            byte[] buffer = new byte[] { unchecked(0x00), unchecked(0x01), unchecked(0x02), unchecked(0x03), unchecked(0x04), unchecked(0x05), unchecked(0x06), unchecked(0x07) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0x00010203), reader.GetInt32());
            Assert.AreEqual(unchecked(0x04050607), reader.GetInt32());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Assert.AreEqual(unchecked(0x03020100), reader.GetInt32());
            Assert.AreEqual(unchecked(0x07060504), reader.GetInt32());
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetUInt32()
        {
            Assert.AreEqual(4294967295L, (object)CreateReader(new byte[] { unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff), unchecked((byte)0xff) }).GetUInt32());
            byte[] buffer = new byte[] { unchecked((byte)0xFF), unchecked(0x00), unchecked(0x01), unchecked(0x02), unchecked(0x03), unchecked(0x04), unchecked(0x05), unchecked(0x06) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0xFF000102L), (object)reader.GetUInt32());
            Assert.AreEqual(unchecked(0x03040506L), (object)reader.GetUInt32());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Assert.AreEqual(unchecked(0x020100FFL), (object)reader.GetUInt32());
            // 0x0010200FF
            Assert.AreEqual(unchecked(0x06050403L), (object)reader.GetUInt32());
        }

        [Test]
        public virtual void TestGetInt32_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new byte[3]);
                reader.GetInt32();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetInt64()
        {
            byte[] buffer = new byte[] { unchecked((byte)0xFF), unchecked(0x00), unchecked(0x01), unchecked(0x02), unchecked(0x03), unchecked(0x04), unchecked(0x05), unchecked(0x06), unchecked(0x07) };
            SequentialReader reader = CreateReader(buffer);
            Assert.AreEqual(unchecked((long)(0xFF00010203040506L)), (object)reader.GetInt64());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Assert.AreEqual(unchecked(0x06050403020100FFL), (object)reader.GetInt64());
        }

        [Test]
        public virtual void TestGetInt64_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new byte[7]);
                reader.GetInt64();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetFloat32()
        {
            int nanBits = unchecked(0x7fc00000);
            Assert.IsTrue(float.IsNaN(Extensions.IntBitsToFloat(nanBits)));
            byte[] buffer = new byte[] { unchecked(0x7f), unchecked((byte)0xc0), unchecked(0x00), unchecked(0x00) };
            SequentialReader reader = CreateReader(buffer);
            Assert.IsTrue(float.IsNaN(reader.GetFloat32()));
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetFloat64()
        {
            long nanBits = unchecked((long)(0xfff0000000000001L));
            Assert.IsTrue(double.IsNaN(Extensions.LongBitsToDouble(nanBits)));
            byte[] buffer = new byte[] { unchecked((byte)0xff), unchecked((byte)0xf0), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x01) };
            SequentialReader reader = CreateReader(buffer);
            Assert.IsTrue(double.IsNaN(reader.GetDouble64()));
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetNullTerminatedString()
        {
            byte[] bytes = new byte[] { unchecked(0x41), unchecked(0x42), unchecked(0x43), unchecked(0x44), unchecked(0x45), unchecked(0x46), unchecked(0x47) };
            // Test max length
            for (int i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual(Runtime.Substring("ABCDEFG", 0, i), CreateReader(bytes).GetNullTerminatedString(i));
            }
            Assert.AreEqual(string.Empty, CreateReader(new byte[] { 0 }).GetNullTerminatedString(10));
            Assert.AreEqual("A", CreateReader(new byte[] { unchecked(0x41), 0 }).GetNullTerminatedString(10));
            Assert.AreEqual("AB", CreateReader(new byte[] { unchecked(0x41), unchecked(0x42), 0 }).GetNullTerminatedString(10));
            Assert.AreEqual("AB", CreateReader(new byte[] { unchecked(0x41), unchecked(0x42), 0, unchecked(0x43) }).GetNullTerminatedString(10));
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetString()
        {
            byte[] bytes = new byte[] { unchecked(0x41), unchecked(0x42), unchecked(0x43), unchecked(0x44), unchecked(0x45), unchecked(0x46), unchecked(0x47) };
            string expected = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(bytes.Length, expected.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual(Runtime.Substring("ABCDEFG", 0, i), CreateReader(bytes).GetString(i));
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [Test]
        public virtual void TestGetBytes()
        {
            byte[] bytes = new byte[] { 0, 1, 2, 3, 4, 5 };
            for (int i = 0; i < bytes.Length; i++)
            {
                SequentialReader reader = CreateReader(bytes);
                byte[] readBytes = reader.GetBytes(i);
                for (int j = 0; j < i; j++)
                {
                    Assert.AreEqual(bytes[j], readBytes[j]);
                }
            }
        }

        [Test]
        public virtual void TestOverflowBoundsCalculation()
        {
            SequentialReader reader = CreateReader(new byte[10]);
            try
            {
                reader.GetBytes(15);
            }
            catch (IOException e)
            {
                Assert.AreEqual("End of data reached.", e.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetBytesEof()
        {
            CreateReader(new byte[50]).GetBytes(50);
            SequentialReader reader = CreateReader(new byte[50]);
            reader.GetBytes(25);
            reader.GetBytes(25);
            try
            {
                CreateReader(new byte[50]).GetBytes(51);
                Assert.Fail("Expecting exception");
            }
            catch (EofException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetInt8Eof()
        {
            CreateReader(new byte[1]).GetInt8();
            SequentialReader reader = CreateReader(new byte[2]);
            reader.GetInt8();
            reader.GetInt8();
            reader = CreateReader(new byte[1]);
            reader.GetInt8();
            try
            {
                reader.GetInt8();
                Assert.Fail("Expecting exception");
            }
            catch (EofException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSkipEof()
        {
            CreateReader(new byte[1]).Skip(1);
            SequentialReader reader = CreateReader(new byte[2]);
            reader.Skip(1);
            reader.Skip(1);
            reader = CreateReader(new byte[1]);
            reader.Skip(1);
            try
            {
                reader.Skip(1);
                Assert.Fail("Expecting exception");
            }
            catch (EofException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestTrySkipEof()
        {
            Assert.IsTrue(CreateReader(new byte[1]).TrySkip(1));
            SequentialReader reader = CreateReader(new byte[2]);
            Assert.IsTrue(reader.TrySkip(1));
            Assert.IsTrue(reader.TrySkip(1));
            Assert.IsFalse(reader.TrySkip(1));
        }
    }
}
