using System;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PngChunkTypeTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestConstructorTooLong()
        {
            try
            {
                new PngChunkType("TooLong");
                Assert.Fail("Expecting exception");
            }
            catch (ArgumentException ex)
            {
                Tests.AreEqual("PNG chunk type identifier must be four bytes in length", ex.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestConstructorTooShort()
        {
            try
            {
                new PngChunkType("foo");
                Assert.Fail("Expecting exception");
            }
            catch (ArgumentException ex)
            {
                Tests.AreEqual("PNG chunk type identifier must be four bytes in length", ex.Message);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestConstructorInvalidBytes()
        {
            string[] invalidStrings = new string[] { "ABC1", "1234", "    ", "!£$%" };
            foreach (string invalidString in invalidStrings)
            {
                try
                {
                    new PngChunkType(invalidString);
                    Assert.Fail("Expecting exception");
                }
                catch (ArgumentException ex)
                {
                    Tests.AreEqual("PNG chunk type identifier may only contain alphabet characters", ex.Message);
                }
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestConstructorValidBytes()
        {
            string[] validStrings = new string[] { "ABCD", "abcd", "wxyz", "WXYZ", "lkjh", "LKJH" };
            foreach (string validString in validStrings)
            {
                new PngChunkType(validString);
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsCritical()
        {
            Tests.IsTrue(new PngChunkType("ABCD").IsCritical());
            Tests.IsFalse(new PngChunkType("aBCD").IsCritical());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsAncillary()
        {
            Tests.IsFalse(new PngChunkType("ABCD").IsAncillary());
            Tests.IsTrue(new PngChunkType("aBCD").IsAncillary());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsPrivate()
        {
            Tests.IsTrue(new PngChunkType("ABCD").IsPrivate());
            Tests.IsFalse(new PngChunkType("AbCD").IsPrivate());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsSafeToCopy()
        {
            Tests.IsFalse(new PngChunkType("ABCD").IsSafeToCopy());
            Tests.IsTrue(new PngChunkType("ABCd").IsSafeToCopy());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestAreMultipleAllowed()
        {
            Tests.IsFalse(new PngChunkType("ABCD").AreMultipleAllowed());
            Tests.IsFalse(new PngChunkType("ABCD", false).AreMultipleAllowed());
            Tests.IsTrue(new PngChunkType("ABCD", true).AreMultipleAllowed());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestEquality()
        {
            Tests.AreEqual(new PngChunkType("ABCD"), new PngChunkType("ABCD"));
            Tests.AreEqual(new PngChunkType("ABCD", true), new PngChunkType("ABCD", true));
            Tests.AreEqual(new PngChunkType("ABCD", false), new PngChunkType("ABCD", false));
            // NOTE we don't consider the 'allowMultiples' value in the equality test (or hash code)
            Tests.AreEqual(new PngChunkType("ABCD", true), new PngChunkType("ABCD", false));
            Assert.AreNotEqual(new PngChunkType("ABCD"), new PngChunkType("abcd"));
        }
    }
}
