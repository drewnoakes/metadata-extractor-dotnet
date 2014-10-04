/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System.IO;
using Com.Drew.Lang;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <summary>
	/// Base class for testing implementations of
	/// <see cref="RandomAccessReader"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public abstract class RandomAccessTestBase
	{
		protected internal abstract RandomAccessReader CreateReader(sbyte[] bytes);

		[NUnit.Framework.Test]
		public virtual void TestDefaultEndianness()
		{
			Sharpen.Tests.AreEqual(true, CreateReader(new sbyte[1]).IsMotorolaByteOrder());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInt8()
		{
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)unchecked((int)(0x7F))), unchecked((sbyte)unchecked((int)(0xFF))) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual(unchecked((sbyte)0), reader.GetInt8(0));
			Sharpen.Tests.AreEqual(unchecked((sbyte)1), reader.GetInt8(1));
			Sharpen.Tests.AreEqual(unchecked((sbyte)127), reader.GetInt8(2));
			Sharpen.Tests.AreEqual(unchecked((sbyte)255), reader.GetInt8(3));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetUInt8()
		{
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)unchecked((int)(0x7F))), unchecked((sbyte)unchecked((int)(0xFF))) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual(0, reader.GetUInt8(0));
			Sharpen.Tests.AreEqual(1, reader.GetUInt8(1));
			Sharpen.Tests.AreEqual(127, reader.GetUInt8(2));
			Sharpen.Tests.AreEqual(255, reader.GetUInt8(3));
		}

		[NUnit.Framework.Test]
		public virtual void TestGetUInt8_OutOfBounds()
		{
			try
			{
				RandomAccessReader reader = CreateReader(new sbyte[2]);
				reader.GetUInt8(2);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (IOException ex)
			{
				Sharpen.Tests.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 2, requested count: 1, max index: 1)", ex.Message);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInt16()
		{
			Sharpen.Tests.AreEqual(-1, CreateReader(new sbyte[] { unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))) }).GetInt16(0));
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)unchecked((int)(0x7F))), unchecked((sbyte)unchecked((int)(0xFF))) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual((short)unchecked((int)(0x0001)), reader.GetInt16(0));
			Sharpen.Tests.AreEqual((short)unchecked((int)(0x017F)), reader.GetInt16(1));
			Sharpen.Tests.AreEqual((short)unchecked((int)(0x7FFF)), reader.GetInt16(2));
			reader.SetMotorolaByteOrder(false);
			Sharpen.Tests.AreEqual((short)unchecked((int)(0x0100)), reader.GetInt16(0));
			Sharpen.Tests.AreEqual((short)unchecked((int)(0x7F01)), reader.GetInt16(1));
			Sharpen.Tests.AreEqual((short)unchecked((int)(0xFF7F)), reader.GetInt16(2));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetUInt16()
		{
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)unchecked((int)(0x7F))), unchecked((sbyte)unchecked((int)(0xFF))) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual(unchecked((int)(0x0001)), reader.GetUInt16(0));
			Sharpen.Tests.AreEqual(unchecked((int)(0x017F)), reader.GetUInt16(1));
			Sharpen.Tests.AreEqual(unchecked((int)(0x7FFF)), reader.GetUInt16(2));
			reader.SetMotorolaByteOrder(false);
			Sharpen.Tests.AreEqual(unchecked((int)(0x0100)), reader.GetUInt16(0));
			Sharpen.Tests.AreEqual(unchecked((int)(0x7F01)), reader.GetUInt16(1));
			Sharpen.Tests.AreEqual(unchecked((int)(0xFF7F)), reader.GetUInt16(2));
		}

		[NUnit.Framework.Test]
		public virtual void TestGetUInt16_OutOfBounds()
		{
			try
			{
				RandomAccessReader reader = CreateReader(new sbyte[2]);
				reader.GetUInt16(1);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (IOException ex)
			{
				Sharpen.Tests.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 1, requested count: 2, max index: 1)", ex.Message);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInt32()
		{
			Sharpen.Tests.AreEqual(-1, CreateReader(new sbyte[] { unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))) }).GetInt32(0));
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)unchecked((int)(0x7F))), unchecked((sbyte)unchecked((int)(0xFF))), unchecked((int)(0x02)), unchecked((int)(0x03)), unchecked((int)(0x04)) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual(unchecked((int)(0x00017FFF)), reader.GetInt32(0));
			Sharpen.Tests.AreEqual(unchecked((int)(0x017FFF02)), reader.GetInt32(1));
			Sharpen.Tests.AreEqual(unchecked((int)(0x7FFF0203)), reader.GetInt32(2));
			Sharpen.Tests.AreEqual(unchecked((int)(0xFF020304)), reader.GetInt32(3));
			reader.SetMotorolaByteOrder(false);
			Sharpen.Tests.AreEqual(unchecked((int)(0xFF7F0100)), reader.GetInt32(0));
			Sharpen.Tests.AreEqual(unchecked((int)(0x02FF7F01)), reader.GetInt32(1));
			Sharpen.Tests.AreEqual(unchecked((int)(0x0302FF7F)), reader.GetInt32(2));
			Sharpen.Tests.AreEqual(unchecked((int)(0x040302FF)), reader.GetInt32(3));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetUInt32()
		{
			Sharpen.Tests.AreEqual(4294967295L, CreateReader(new sbyte[] { unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xff))) }).GetUInt32
				(0));
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((sbyte)unchecked((int)(0x7F))), unchecked((sbyte)unchecked((int)(0xFF))), unchecked((int)(0x02)), unchecked((int)(0x03)), unchecked((int)(0x04)) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual(unchecked((long)(0x00017FFFL)), reader.GetUInt32(0));
			Sharpen.Tests.AreEqual(unchecked((long)(0x017FFF02L)), reader.GetUInt32(1));
			Sharpen.Tests.AreEqual(unchecked((long)(0x7FFF0203L)), reader.GetUInt32(2));
			Sharpen.Tests.AreEqual(unchecked((long)(0xFF020304L)), reader.GetUInt32(3));
			reader.SetMotorolaByteOrder(false);
			Sharpen.Tests.AreEqual(4286513408L, reader.GetUInt32(0));
			Sharpen.Tests.AreEqual(unchecked((long)(0x02FF7F01L)), reader.GetUInt32(1));
			Sharpen.Tests.AreEqual(unchecked((long)(0x0302FF7FL)), reader.GetUInt32(2));
			Sharpen.Tests.AreEqual(unchecked((long)(0x040302FFL)), reader.GetInt32(3));
		}

		[NUnit.Framework.Test]
		public virtual void TestGetInt32_OutOfBounds()
		{
			try
			{
				RandomAccessReader reader = CreateReader(new sbyte[3]);
				reader.GetInt32(0);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (IOException ex)
			{
				Sharpen.Tests.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 4, max index: 2)", ex.Message);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInt64()
		{
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x02)), unchecked((int)(0x03)), unchecked((int)(0x04)), unchecked((int)(0x05)), unchecked((int)(0x06)), unchecked((int)(0x07)), unchecked((sbyte)unchecked(
				(int)(0xFF))) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.AreEqual(unchecked((long)(0x0001020304050607L)), reader.GetInt64(0));
			Sharpen.Tests.AreEqual(unchecked((long)(0x01020304050607FFL)), reader.GetInt64(1));
			reader.SetMotorolaByteOrder(false);
			Sharpen.Tests.AreEqual(unchecked((long)(0x0706050403020100L)), reader.GetInt64(0));
			Sharpen.Tests.AreEqual(unchecked((long)(0xFF07060504030201L)), reader.GetInt64(1));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInt64_OutOfBounds()
		{
			try
			{
				RandomAccessReader reader = CreateReader(new sbyte[7]);
				reader.GetInt64(0);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (IOException ex)
			{
				Sharpen.Tests.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 8, max index: 6)", ex.Message);
			}
			try
			{
				RandomAccessReader reader = CreateReader(new sbyte[7]);
				reader.GetInt64(-1);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (IOException ex)
			{
				Sharpen.Tests.AreEqual("Attempt to read from buffer using a negative index (-1)", ex.Message);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetFloat32()
		{
			int nanBits = unchecked((int)(0x7fc00000));
			Sharpen.Tests.IsTrue(float.IsNaN(Sharpen.Extensions.IntBitsToFloat(nanBits)));
			sbyte[] buffer = new sbyte[] { unchecked((int)(0x7f)), unchecked((sbyte)unchecked((int)(0xc0))), unchecked((int)(0x00)), unchecked((int)(0x00)) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.IsTrue(float.IsNaN(reader.GetFloat32(0)));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetFloat64()
		{
			long nanBits = unchecked((long)(0xfff0000000000001L));
			Sharpen.Tests.IsTrue(double.IsNaN(Sharpen.Extensions.LongBitsToDouble(nanBits)));
			sbyte[] buffer = new sbyte[] { unchecked((sbyte)unchecked((int)(0xff))), unchecked((sbyte)unchecked((int)(0xf0))), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked(
				(int)(0x01)) };
			RandomAccessReader reader = CreateReader(buffer);
			Sharpen.Tests.IsTrue(double.IsNaN(reader.GetDouble64(0)));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetNullTerminatedString()
		{
			sbyte[] bytes = new sbyte[] { unchecked((int)(0x41)), unchecked((int)(0x42)), unchecked((int)(0x43)), unchecked((int)(0x44)), unchecked((int)(0x00)), unchecked((int)(0x45)), unchecked((int)(0x46)), unchecked((int)(0x47)) };
			RandomAccessReader reader = CreateReader(bytes);
			Sharpen.Tests.AreEqual(string.Empty, reader.GetNullTerminatedString(0, 0));
			Sharpen.Tests.AreEqual("A", reader.GetNullTerminatedString(0, 1));
			Sharpen.Tests.AreEqual("AB", reader.GetNullTerminatedString(0, 2));
			Sharpen.Tests.AreEqual("ABC", reader.GetNullTerminatedString(0, 3));
			Sharpen.Tests.AreEqual("ABCD", reader.GetNullTerminatedString(0, 4));
			Sharpen.Tests.AreEqual("ABCD", reader.GetNullTerminatedString(0, 5));
			Sharpen.Tests.AreEqual("ABCD", reader.GetNullTerminatedString(0, 6));
			Sharpen.Tests.AreEqual("BCD", reader.GetNullTerminatedString(1, 3));
			Sharpen.Tests.AreEqual("BCD", reader.GetNullTerminatedString(1, 4));
			Sharpen.Tests.AreEqual("BCD", reader.GetNullTerminatedString(1, 5));
			Sharpen.Tests.AreEqual(string.Empty, reader.GetNullTerminatedString(4, 3));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetString()
		{
			sbyte[] bytes = new sbyte[] { unchecked((int)(0x41)), unchecked((int)(0x42)), unchecked((int)(0x43)), unchecked((int)(0x44)), unchecked((int)(0x00)), unchecked((int)(0x45)), unchecked((int)(0x46)), unchecked((int)(0x47)) };
			RandomAccessReader reader = CreateReader(bytes);
			Sharpen.Tests.AreEqual(string.Empty, reader.GetString(0, 0));
			Sharpen.Tests.AreEqual("A", reader.GetString(0, 1));
			Sharpen.Tests.AreEqual("AB", reader.GetString(0, 2));
			Sharpen.Tests.AreEqual("ABC", reader.GetString(0, 3));
			Sharpen.Tests.AreEqual("ABCD", reader.GetString(0, 4));
			Sharpen.Tests.AreEqual("ABCD\x0", reader.GetString(0, 5));
			Sharpen.Tests.AreEqual("ABCD\x0E", reader.GetString(0, 6));
			Sharpen.Tests.AreEqual("BCD", reader.GetString(1, 3));
			Sharpen.Tests.AreEqual("BCD\x0", reader.GetString(1, 4));
			Sharpen.Tests.AreEqual("BCD\x0E", reader.GetString(1, 5));
			Sharpen.Tests.AreEqual("\x0EF", reader.GetString(4, 3));
		}

		[NUnit.Framework.Test]
		public virtual void TestIndexPlusCountExceedsIntMaxValue()
		{
			RandomAccessReader reader = CreateReader(new sbyte[10]);
			try
			{
				reader.GetBytes(unchecked((int)(0x6FFFFFFF)), unchecked((int)(0x6FFFFFFF)));
			}
			catch (IOException e)
			{
				Sharpen.Tests.AreEqual("Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: 1879048191, requested count: 1879048191)", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestOverflowBoundsCalculation()
		{
			RandomAccessReader reader = CreateReader(new sbyte[10]);
			try
			{
				reader.GetBytes(5, 10);
			}
			catch (IOException e)
			{
				Sharpen.Tests.AreEqual("Attempt to read from beyond end of underlying data source (requested index: 5, requested count: 10, max index: 9)", e.Message);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetBytesEOF()
		{
			CreateReader(new sbyte[50]).GetBytes(0, 50);
			RandomAccessReader reader = CreateReader(new sbyte[50]);
			reader.GetBytes(25, 25);
			try
			{
				CreateReader(new sbyte[50]).GetBytes(0, 51);
				NUnit.Framework.Assert.Fail("Expecting exception");
			}
			catch (IOException)
			{
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInt8EOF()
		{
			CreateReader(new sbyte[1]).GetInt8(0);
			RandomAccessReader reader = CreateReader(new sbyte[2]);
			reader.GetInt8(0);
			reader.GetInt8(1);
			try
			{
				reader = CreateReader(new sbyte[1]);
				reader.GetInt8(0);
				reader.GetInt8(1);
				NUnit.Framework.Assert.Fail("Expecting exception");
			}
			catch (IOException)
			{
			}
		}
	}
}
