using System;
using Com.Drew.Imaging.Png;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngChunkTypeTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestConstructorTooLong()
		{
			try
			{
				new PngChunkType("TooLong");
				NUnit.Framework.Assert.Fail("Expecting exception");
			}
			catch (ArgumentException ex)
			{
				Sharpen.Tests.AreEqual("PNG chunk type identifier must be four bytes in length", ex.Message);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestConstructorTooShort()
		{
			try
			{
				new PngChunkType("foo");
				NUnit.Framework.Assert.Fail("Expecting exception");
			}
			catch (ArgumentException ex)
			{
				Sharpen.Tests.AreEqual("PNG chunk type identifier must be four bytes in length", ex.Message);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestConstructorInvalidBytes()
		{
			string[] invalidStrings = new string[] { "ABC1", "1234", "    ", "!ВЈ$%" };
			foreach (string invalidString in invalidStrings)
			{
				try
				{
					new PngChunkType(invalidString);
					NUnit.Framework.Assert.Fail("Expecting exception");
				}
				catch (ArgumentException ex)
				{
					Sharpen.Tests.AreEqual("PNG chunk type identifier may only contain alphabet characters", ex.Message);
				}
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestConstructorValidBytes()
		{
			string[] validStrings = new string[] { "ABCD", "abcd", "wxyz", "WXYZ", "lkjh", "LKJH" };
			foreach (string validString in validStrings)
			{
				new PngChunkType(validString);
			}
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestIsCritical()
		{
			Sharpen.Tests.IsTrue(new PngChunkType("ABCD").IsCritical());
			Sharpen.Tests.IsFalse(new PngChunkType("aBCD").IsCritical());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestIsAncillary()
		{
			Sharpen.Tests.IsFalse(new PngChunkType("ABCD").IsAncillary());
			Sharpen.Tests.IsTrue(new PngChunkType("aBCD").IsAncillary());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestIsPrivate()
		{
			Sharpen.Tests.IsTrue(new PngChunkType("ABCD").IsPrivate());
			Sharpen.Tests.IsFalse(new PngChunkType("AbCD").IsPrivate());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestIsSafeToCopy()
		{
			Sharpen.Tests.IsFalse(new PngChunkType("ABCD").IsSafeToCopy());
			Sharpen.Tests.IsTrue(new PngChunkType("ABCd").IsSafeToCopy());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestAreMultipleAllowed()
		{
			Sharpen.Tests.IsFalse(new PngChunkType("ABCD").AreMultipleAllowed());
			Sharpen.Tests.IsFalse(new PngChunkType("ABCD", false).AreMultipleAllowed());
			Sharpen.Tests.IsTrue(new PngChunkType("ABCD", true).AreMultipleAllowed());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestEquality()
		{
			Sharpen.Tests.AreEqual(new PngChunkType("ABCD"), new PngChunkType("ABCD"));
			Sharpen.Tests.AreEqual(new PngChunkType("ABCD", true), new PngChunkType("ABCD", true));
			Sharpen.Tests.AreEqual(new PngChunkType("ABCD", false), new PngChunkType("ABCD", false));
			// NOTE we don't consider the 'allowMultiples' value in the equality test (or hash code)
			Sharpen.Tests.AreEqual(new PngChunkType("ABCD", true), new PngChunkType("ABCD", false));
			NUnit.Framework.Assert.AreNotEqual(new PngChunkType("ABCD"), new PngChunkType("abcd"));
		}
	}
}
