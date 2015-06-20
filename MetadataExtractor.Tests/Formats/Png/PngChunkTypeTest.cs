using System;
using MetadataExtractor.Formats.Png;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkTypeTest
    {
        [Fact]
        public void TestConstructorTooLong()
        {
            var ex = Assert.Throws<ArgumentException>(() => new PngChunkType("TooLong"));
            Assert.Equal("PNG chunk type identifier must be four bytes in length", ex.Message);
        }

        [Fact]
        public void TestConstructorTooShort()
        {
            var ex = Assert.Throws<ArgumentException>(() => new PngChunkType("foo"));
            Assert.Equal("PNG chunk type identifier must be four bytes in length", ex.Message);
        }

        [Fact]
        public void TestConstructorInvalidBytes()
        {
            var invalidStrings = new[] { "ABC1", "1234", "    ", "!£$%" };
            foreach (var invalidString in invalidStrings)
            {
                var ex = Assert.Throws<ArgumentException>(() => new PngChunkType(invalidString));
                Assert.Equal("PNG chunk type identifier may only contain alphabet characters", ex.Message);
            }
        }

        [Fact]
        public void TestConstructorValidBytes()
        {
            var validStrings = new[] { "ABCD", "abcd", "wxyz", "WXYZ", "lkjh", "LKJH" };
            foreach (var validString in validStrings)
            {
                new PngChunkType(validString);
            }
        }

        [Fact]
        public void TestIsCritical()
        {
            Assert.True(new PngChunkType("ABCD").IsCritical());
            Assert.False(new PngChunkType("aBCD").IsCritical());
        }

        [Fact]
        public void TestIsAncillary()
        {
            Assert.False(new PngChunkType("ABCD").IsAncillary());
            Assert.True(new PngChunkType("aBCD").IsAncillary());
        }

        [Fact]
        public void TestIsPrivate()
        {
            Assert.True(new PngChunkType("ABCD").IsPrivate());
            Assert.False(new PngChunkType("AbCD").IsPrivate());
        }

        [Fact]
        public void TestIsSafeToCopy()
        {
            Assert.False(new PngChunkType("ABCD").IsSafeToCopy());
            Assert.True(new PngChunkType("ABCd").IsSafeToCopy());
        }

        [Fact]
        public void TestAreMultipleAllowed()
        {
            Assert.False(new PngChunkType("ABCD").AreMultipleAllowed());
            Assert.False(new PngChunkType("ABCD", false).AreMultipleAllowed());
            Assert.True(new PngChunkType("ABCD", true).AreMultipleAllowed());
        }

        [Fact]
        public void TestEquality()
        {
            Assert.Equal(new PngChunkType("ABCD"), new PngChunkType("ABCD"));
            Assert.Equal(new PngChunkType("ABCD", true), new PngChunkType("ABCD", true));
            Assert.Equal(new PngChunkType("ABCD", false), new PngChunkType("ABCD", false));
            // NOTE we don't consider the 'allowMultiples' value in the equality test (or hash code)
            Assert.Equal(new PngChunkType("ABCD", true), new PngChunkType("ABCD", false));
            Assert.NotEqual(new PngChunkType("ABCD"), new PngChunkType("abcd"));
        }
    }
}
