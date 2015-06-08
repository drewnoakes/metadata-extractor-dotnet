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
                Assert.AreEqual("PNG chunk type identifier must be four bytes in length", ex.Message);
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
                Assert.AreEqual("PNG chunk type identifier must be four bytes in length", ex.Message);
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
                    Assert.AreEqual("PNG chunk type identifier may only contain alphabet characters", ex.Message);
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
            Assert.IsTrue(new PngChunkType("ABCD").IsCritical());
            Assert.IsFalse(new PngChunkType("aBCD").IsCritical());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsAncillary()
        {
            Assert.IsFalse(new PngChunkType("ABCD").IsAncillary());
            Assert.IsTrue(new PngChunkType("aBCD").IsAncillary());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsPrivate()
        {
            Assert.IsTrue(new PngChunkType("ABCD").IsPrivate());
            Assert.IsFalse(new PngChunkType("AbCD").IsPrivate());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsSafeToCopy()
        {
            Assert.IsFalse(new PngChunkType("ABCD").IsSafeToCopy());
            Assert.IsTrue(new PngChunkType("ABCd").IsSafeToCopy());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestAreMultipleAllowed()
        {
            Assert.IsFalse(new PngChunkType("ABCD").AreMultipleAllowed());
            Assert.IsFalse(new PngChunkType("ABCD", false).AreMultipleAllowed());
            Assert.IsTrue(new PngChunkType("ABCD", true).AreMultipleAllowed());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestEquality()
        {
            Assert.AreEqual(new PngChunkType("ABCD"), new PngChunkType("ABCD"));
            Assert.AreEqual(new PngChunkType("ABCD", true), new PngChunkType("ABCD", true));
            Assert.AreEqual(new PngChunkType("ABCD", false), new PngChunkType("ABCD", false));
            // NOTE we don't consider the 'allowMultiples' value in the equality test (or hash code)
            Assert.AreEqual(new PngChunkType("ABCD", true), new PngChunkType("ABCD", false));
            Assert.AreNotEqual(new PngChunkType("ABCD"), new PngChunkType("abcd"));
        }
    }
}
