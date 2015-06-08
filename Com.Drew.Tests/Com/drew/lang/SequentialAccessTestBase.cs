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
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>
    /// Base class for testing implementations of
    /// <see cref="SequentialReader"/>
    /// .
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class SequentialAccessTestBase
    {
        protected internal abstract SequentialReader CreateReader(sbyte[] bytes);

        [NUnit.Framework.Test]
        public virtual void TestDefaultEndianness()
        {
            Sharpen.Tests.AreEqual(true, CreateReader(new sbyte[1]).IsMotorolaByteOrder());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetInt8()
        {
            sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)0x7F), unchecked((sbyte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual(unchecked((sbyte)0), reader.GetInt8());
            Sharpen.Tests.AreEqual(unchecked((sbyte)1), reader.GetInt8());
            Sharpen.Tests.AreEqual(unchecked((sbyte)127), reader.GetInt8());
            Sharpen.Tests.AreEqual(unchecked((sbyte)255), reader.GetInt8());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetUInt8()
        {
            sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)0x7F), unchecked((sbyte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual(0, reader.GetUInt8());
            Sharpen.Tests.AreEqual(1, reader.GetUInt8());
            Sharpen.Tests.AreEqual(127, reader.GetUInt8());
            Sharpen.Tests.AreEqual(255, reader.GetUInt8());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetUInt8_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new sbyte[1]);
                reader.GetUInt8();
                reader.GetUInt8();
                NUnit.Framework.Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Sharpen.Tests.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetInt16()
        {
            Sharpen.Tests.AreEqual(-1, CreateReader(new sbyte[] { unchecked((sbyte)0xff), unchecked((sbyte)0xff) }).GetInt16());
            sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)0x7F), unchecked((sbyte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual((short)0x0001, reader.GetInt16());
            Sharpen.Tests.AreEqual((short)0x7FFF, reader.GetInt16());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Sharpen.Tests.AreEqual((short)0x0100, reader.GetInt16());
            Sharpen.Tests.AreEqual(unchecked((short)(0xFF7F)), reader.GetInt16());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetUInt16()
        {
            sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)0x7F), unchecked((sbyte)0xFF) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual(unchecked((int)(0x0001)), reader.GetUInt16());
            Sharpen.Tests.AreEqual(unchecked((int)(0x7FFF)), reader.GetUInt16());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Sharpen.Tests.AreEqual(unchecked((int)(0x0100)), reader.GetUInt16());
            Sharpen.Tests.AreEqual(unchecked((int)(0xFF7F)), reader.GetUInt16());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetUInt16_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new sbyte[1]);
                reader.GetUInt16();
                NUnit.Framework.Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Sharpen.Tests.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetInt32()
        {
            Sharpen.Tests.AreEqual(-1, CreateReader(new sbyte[] { unchecked((sbyte)0xff), unchecked((sbyte)0xff), unchecked((sbyte)0xff), unchecked((sbyte)0xff) }).GetInt32());
            sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x02)), unchecked((int)(0x03)), unchecked((int)(0x04)), unchecked((int)(0x05)), unchecked((int)(0x06)), unchecked((int)(0x07)) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual(unchecked((int)(0x00010203)), reader.GetInt32());
            Sharpen.Tests.AreEqual(unchecked((int)(0x04050607)), reader.GetInt32());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Sharpen.Tests.AreEqual(unchecked((int)(0x03020100)), reader.GetInt32());
            Sharpen.Tests.AreEqual(unchecked((int)(0x07060504)), reader.GetInt32());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetUInt32()
        {
            Sharpen.Tests.AreEqual(4294967295L, CreateReader(new sbyte[] { unchecked((sbyte)0xff), unchecked((sbyte)0xff), unchecked((sbyte)0xff), unchecked((sbyte)0xff) }).GetUInt32());
            sbyte[] buffer = new sbyte[] { unchecked((sbyte)0xFF), unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x02)), unchecked((int)(0x03)), unchecked((int)(0x04)), unchecked((int)(0x05)), unchecked((int)(0x06)) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual(unchecked((long)(0xFF000102L)), reader.GetUInt32());
            Sharpen.Tests.AreEqual(unchecked((long)(0x03040506L)), reader.GetUInt32());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Sharpen.Tests.AreEqual(unchecked((long)(0x020100FFL)), reader.GetUInt32());
            // 0x0010200FF
            Sharpen.Tests.AreEqual(unchecked((long)(0x06050403L)), reader.GetUInt32());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetInt32_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new sbyte[3]);
                reader.GetInt32();
                NUnit.Framework.Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Sharpen.Tests.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetInt64()
        {
            sbyte[] buffer = new sbyte[] { unchecked((sbyte)0xFF), unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x02)), unchecked((int)(0x03)), unchecked((int)(0x04)), unchecked((int)(0x05)), unchecked((int)(0x06)), unchecked((int)(0x07
                )) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.AreEqual(unchecked((long)(0xFF00010203040506L)), reader.GetInt64());
            reader = CreateReader(buffer);
            reader.SetMotorolaByteOrder(false);
            Sharpen.Tests.AreEqual(unchecked((long)(0x06050403020100FFL)), reader.GetInt64());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetInt64_OutOfBounds()
        {
            try
            {
                SequentialReader reader = CreateReader(new sbyte[7]);
                reader.GetInt64();
                NUnit.Framework.Assert.Fail("Exception expected");
            }
            catch (IOException ex)
            {
                Sharpen.Tests.AreEqual("End of data reached.", ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetFloat32()
        {
            int nanBits = unchecked((int)(0x7fc00000));
            Sharpen.Tests.IsTrue(float.IsNaN(Sharpen.Extensions.IntBitsToFloat(nanBits)));
            sbyte[] buffer = new sbyte[] { unchecked((int)(0x7f)), unchecked((sbyte)0xc0), unchecked((int)(0x00)), unchecked((int)(0x00)) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.IsTrue(float.IsNaN(reader.GetFloat32()));
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetFloat64()
        {
            long nanBits = unchecked((long)(0xfff0000000000001L));
            Sharpen.Tests.IsTrue(double.IsNaN(Sharpen.Extensions.LongBitsToDouble(nanBits)));
            sbyte[] buffer = new sbyte[] { unchecked((sbyte)0xff), unchecked((sbyte)0xf0), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x01)) };
            SequentialReader reader = CreateReader(buffer);
            Sharpen.Tests.IsTrue(double.IsNaN(reader.GetDouble64()));
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetNullTerminatedString()
        {
            sbyte[] bytes = new sbyte[] { unchecked((int)(0x41)), unchecked((int)(0x42)), unchecked((int)(0x43)), unchecked((int)(0x44)), unchecked((int)(0x45)), unchecked((int)(0x46)), unchecked((int)(0x47)) };
            // Test max length
            for (int i = 0; i < bytes.Length; i++)
            {
                Sharpen.Tests.AreEqual(Sharpen.Runtime.Substring("ABCDEFG", 0, i), CreateReader(bytes).GetNullTerminatedString(i));
            }
            Sharpen.Tests.AreEqual(string.Empty, CreateReader(new sbyte[] { 0 }).GetNullTerminatedString(10));
            Sharpen.Tests.AreEqual("A", CreateReader(new sbyte[] { unchecked((int)(0x41)), 0 }).GetNullTerminatedString(10));
            Sharpen.Tests.AreEqual("AB", CreateReader(new sbyte[] { unchecked((int)(0x41)), unchecked((int)(0x42)), 0 }).GetNullTerminatedString(10));
            Sharpen.Tests.AreEqual("AB", CreateReader(new sbyte[] { unchecked((int)(0x41)), unchecked((int)(0x42)), 0, unchecked((int)(0x43)) }).GetNullTerminatedString(10));
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetString()
        {
            sbyte[] bytes = new sbyte[] { unchecked((int)(0x41)), unchecked((int)(0x42)), unchecked((int)(0x43)), unchecked((int)(0x44)), unchecked((int)(0x45)), unchecked((int)(0x46)), unchecked((int)(0x47)) };
            string expected = Sharpen.Runtime.GetStringForBytes(bytes);
            Sharpen.Tests.AreEqual(bytes.Length, expected.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                Sharpen.Tests.AreEqual(Sharpen.Runtime.Substring("ABCDEFG", 0, i), CreateReader(bytes).GetString(i));
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetBytes()
        {
            sbyte[] bytes = new sbyte[] { 0, 1, 2, 3, 4, 5 };
            for (int i = 0; i < bytes.Length; i++)
            {
                SequentialReader reader = CreateReader(bytes);
                sbyte[] readBytes = reader.GetBytes(i);
                for (int j = 0; j < i; j++)
                {
                    Sharpen.Tests.AreEqual(bytes[j], readBytes[j]);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestOverflowBoundsCalculation()
        {
            SequentialReader reader = CreateReader(new sbyte[10]);
            try
            {
                reader.GetBytes(15);
            }
            catch (IOException e)
            {
                Sharpen.Tests.AreEqual("End of data reached.", e.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestGetBytesEOF()
        {
            CreateReader(new sbyte[50]).GetBytes(50);
            SequentialReader reader = CreateReader(new sbyte[50]);
            reader.GetBytes(25);
            reader.GetBytes(25);
            try
            {
                CreateReader(new sbyte[50]).GetBytes(51);
                NUnit.Framework.Assert.Fail("Expecting exception");
            }
            catch (EOFException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestGetInt8EOF()
        {
            CreateReader(new sbyte[1]).GetInt8();
            SequentialReader reader = CreateReader(new sbyte[2]);
            reader.GetInt8();
            reader.GetInt8();
            try
            {
                reader = CreateReader(new sbyte[1]);
                reader.GetInt8();
                reader.GetInt8();
                NUnit.Framework.Assert.Fail("Expecting exception");
            }
            catch (EOFException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestSkipEOF()
        {
            CreateReader(new sbyte[1]).Skip(1);
            SequentialReader reader = CreateReader(new sbyte[2]);
            reader.Skip(1);
            reader.Skip(1);
            try
            {
                reader = CreateReader(new sbyte[1]);
                reader.Skip(1);
                reader.Skip(1);
                NUnit.Framework.Assert.Fail("Expecting exception");
            }
            catch (EOFException)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestTrySkipEOF()
        {
            Sharpen.Tests.IsTrue(CreateReader(new sbyte[1]).TrySkip(1));
            SequentialReader reader = CreateReader(new sbyte[2]);
            Sharpen.Tests.IsTrue(reader.TrySkip(1));
            Sharpen.Tests.IsTrue(reader.TrySkip(1));
            Sharpen.Tests.IsFalse(reader.TrySkip(1));
        }
    }
}
