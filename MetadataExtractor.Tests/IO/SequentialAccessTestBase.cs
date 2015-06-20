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
using System.Text;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>
    /// Base class for testing implementations of <see cref="SequentialReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class SequentialAccessTestBase
    {
        protected abstract SequentialReader CreateReader(byte[] bytes);

        [Test]
        public void TestDefaultEndianness()
        {
            Assert.AreEqual(true, CreateReader(new byte[1]).IsMotorolaByteOrder);
        }


        [Test]
        public void TestGetSByte()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.AreEqual(0, reader.GetSByte());
            Assert.AreEqual(1, reader.GetSByte());
            Assert.AreEqual(127, reader.GetSByte());
            Assert.AreEqual(-1, reader.GetSByte());
        }


        [Test]
        public void TestGetByte()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.AreEqual(0, reader.GetByte());
            Assert.AreEqual(1, reader.GetByte());
            Assert.AreEqual(127, reader.GetByte());
            Assert.AreEqual(255, reader.GetByte());
        }

        [Test]
        public void TestGetByte_OutOfBounds()
        {
            try
            {
                var reader = CreateReader(new byte[1]);
                reader.GetByte();
                reader.GetByte();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }


        [Test]
        public void TestGetInt16()
        {
            Assert.AreEqual(-1, CreateReader(new[] { (byte)0xff, (byte)0xff }).GetInt16());
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.AreEqual(0x0001, reader.GetInt16());
            Assert.AreEqual(0x7FFF, reader.GetInt16());
            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(0x0100, reader.GetInt16());
            Assert.AreEqual(unchecked((short)(0xFF7F)), reader.GetInt16());
        }


        [Test]
        public void TestGetUInt16()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.AreEqual(0x0001, reader.GetUInt16());
            Assert.AreEqual(0x7FFF, reader.GetUInt16());
            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(0x0100, reader.GetUInt16());
            Assert.AreEqual(0xFF7F, reader.GetUInt16());
        }

        [Test]
        public void TestGetUInt16_OutOfBounds()
        {
            try
            {
                var reader = CreateReader(new byte[1]);
                reader.GetUInt16();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }


        [Test]
        public void TestGetInt32()
        {
            Assert.AreEqual(-1, CreateReader(new[] { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff }).GetInt32());
            var buffer = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            var reader = CreateReader(buffer);
            Assert.AreEqual(0x00010203, reader.GetInt32());
            Assert.AreEqual(0x04050607, reader.GetInt32());
            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(0x03020100, reader.GetInt32());
            Assert.AreEqual(0x07060504, reader.GetInt32());
        }


        [Test]
        public void TestGetUInt32()
        {
            Assert.AreEqual(4294967295L, (object)CreateReader(new[] { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff }).GetUInt32());
            var buffer = new byte[] { 0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            var reader = CreateReader(buffer);
            Assert.AreEqual(unchecked(0xFF000102L), (object)reader.GetUInt32());
            Assert.AreEqual(unchecked(0x03040506L), (object)reader.GetUInt32());
            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(unchecked(0x020100FFL), (object)reader.GetUInt32());
            // 0x0010200FF
            Assert.AreEqual(unchecked(0x06050403L), (object)reader.GetUInt32());
        }

        [Test]
        public void TestGetInt32_OutOfBounds()
        {
            try
            {
                var reader = CreateReader(new byte[3]);
                reader.GetInt32();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }


        [Test]
        public void TestGetInt64()
        {
            var buffer = new byte[] { 0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            var reader = CreateReader(buffer);
            Assert.AreEqual(unchecked((long)(0xFF00010203040506L)), (object)reader.GetInt64());
            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;
            Assert.AreEqual(unchecked(0x06050403020100FFL), (object)reader.GetInt64());
        }

        [Test]
        public void TestGetInt64_OutOfBounds()
        {
            try
            {
                var reader = CreateReader(new byte[7]);
                reader.GetInt64();
                Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Assert.AreEqual("End of data reached.", ex.Message);
            }
        }


        [Test]
        public void TestGetFloat32()
        {
            var nanBits = 0x7fc00000;
            Assert.IsTrue(float.IsNaN(BitConverter.ToSingle(BitConverter.GetBytes(nanBits), 0)));
            var buffer = new byte[] { 0x7f, 0xc0, 0x00, 0x00 };
            var reader = CreateReader(buffer);
            Assert.IsTrue(float.IsNaN(reader.GetFloat32()));
        }


        [Test]
        public void TestGetFloat64()
        {
            var nanBits = unchecked((long)(0xfff0000000000001L));
            Assert.IsTrue(double.IsNaN(BitConverter.Int64BitsToDouble(nanBits)));
            var buffer = new byte[] { 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
            var reader = CreateReader(buffer);
            Assert.IsTrue(double.IsNaN(reader.GetDouble64()));
        }


        [Test]
        public void TestGetNullTerminatedString()
        {
            var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47 };
            // Test max length
            for (var i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual("ABCDEFG".Substring (0, i - 0), CreateReader(bytes).GetNullTerminatedString(i));
            }
            Assert.AreEqual(string.Empty, CreateReader(new byte[] { 0 }).GetNullTerminatedString(10));
            Assert.AreEqual("A", CreateReader(new byte[] { 0x41, 0 }).GetNullTerminatedString(10));
            Assert.AreEqual("AB", CreateReader(new byte[] { 0x41, 0x42, 0 }).GetNullTerminatedString(10));
            Assert.AreEqual("AB", CreateReader(new byte[] { 0x41, 0x42, 0, 0x43 }).GetNullTerminatedString(10));
        }


        [Test]
        public void TestGetString()
        {
            var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47 };
            var expected = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(bytes.Length, expected.Length);
            for (var i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual("ABCDEFG".Substring (0, i - 0), CreateReader(bytes).GetString(i));
            }
        }


        [Test]
        public void TestGetBytes()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5 };
            for (var i = 0; i < bytes.Length; i++)
            {
                var reader = CreateReader(bytes);
                var readBytes = reader.GetBytes(i);
                for (var j = 0; j < i; j++)
                {
                    Assert.AreEqual(bytes[j], readBytes[j]);
                }
            }
        }

        [Test]
        public void TestOverflowBoundsCalculation()
        {
            var reader = CreateReader(new byte[10]);
            try
            {
                reader.GetBytes(15);
            }
            catch (IOException e)
            {
                Assert.AreEqual("End of data reached.", e.Message);
            }
        }


        [Test]
        public void TestGetBytesEof()
        {
            CreateReader(new byte[50]).GetBytes(50);

            var reader = CreateReader(new byte[50]);
            reader.GetBytes(25);
            reader.GetBytes(25);
            try
            {
                CreateReader(new byte[50]).GetBytes(51);
                Assert.Fail("Expecting exception");
            }
            catch (IOException)
            {
            }
        }


        [Test]
        public void TestGetByteEof()
        {
            CreateReader(new byte[1]).GetByte();

            var reader = CreateReader(new byte[2]);
            reader.GetByte();
            reader.GetByte();

            reader = CreateReader(new byte[1]);
            reader.GetByte();
            try
            {
                reader.GetByte();
                Assert.Fail("Expecting exception");
            }
            catch (IOException)
            {
            }
        }


        [Test]
        public void TestSkipEof()
        {
            CreateReader(new byte[1]).Skip(1);
            var reader = CreateReader(new byte[2]);
            reader.Skip(1);
            reader.Skip(1);
            reader = CreateReader(new byte[1]);
            reader.Skip(1);
            try
            {
                reader.Skip(1);
                Assert.Fail("Expecting exception");
            }
            catch (IOException)
            {
            }
        }


        [Test]
        public void TestTrySkipEof()
        {
            Assert.IsTrue(CreateReader(new byte[1]).TrySkip(1));
            var reader = CreateReader(new byte[2]);
            Assert.IsTrue(reader.TrySkip(1));
            Assert.IsTrue(reader.TrySkip(1));
            Assert.IsFalse(reader.TrySkip(1));
        }
    }
}
