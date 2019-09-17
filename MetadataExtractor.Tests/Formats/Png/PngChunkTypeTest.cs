// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using MetadataExtractor.Formats.Png;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <summary>Unit tests for <see cref="PngChunkType"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkTypeTest
    {
        [Fact]
        public void ConstructorTooLong()
        {
            var ex = Assert.Throws<ArgumentException>(() => new PngChunkType("TooLong"));
            Assert.Equal("PNG chunk type identifier must be four bytes in length", ex.Message);
        }

        [Fact]
        public void ConstructorTooShort()
        {
            var ex = Assert.Throws<ArgumentException>(() => new PngChunkType("foo"));
            Assert.Equal("PNG chunk type identifier must be four bytes in length", ex.Message);
        }

        [Fact]
        public void ConstructorInvalidBytes()
        {
            var invalidStrings = new[] { "ABC1", "1234", "    ", "!&$%" };
            foreach (var invalidString in invalidStrings)
            {
                var ex = Assert.Throws<ArgumentException>(() => new PngChunkType(invalidString));
                Assert.Equal("PNG chunk type identifier may only contain alphabet characters", ex.Message);
            }
        }

        [Fact]
        public void ConstructorValidBytes()
        {
            var validStrings = new[] { "ABCD", "abcd", "wxyz", "WXYZ", "lkjh", "LKJH" };
            foreach (var validString in validStrings)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new PngChunkType(validString);
            }
        }

        [Fact]
        public void IsCritical()
        {
            Assert.True(new PngChunkType("ABCD").IsCritical);
            Assert.False(new PngChunkType("aBCD").IsCritical);
        }

        [Fact]
        public void IsAncillary()
        {
            Assert.False(new PngChunkType("ABCD").IsAncillary);
            Assert.True(new PngChunkType("aBCD").IsAncillary);
        }

        [Fact]
        public void IsPrivate()
        {
            Assert.True(new PngChunkType("ABCD").IsPrivate);
            Assert.False(new PngChunkType("AbCD").IsPrivate);
        }

        [Fact]
        public void IsSafeToCopy()
        {
            Assert.False(new PngChunkType("ABCD").IsSafeToCopy);
            Assert.True(new PngChunkType("ABCd").IsSafeToCopy);
        }

        [Fact]
        public void AreMultipleAllowed()
        {
            Assert.False(new PngChunkType("ABCD").AreMultipleAllowed);
            Assert.True(new PngChunkType("ABCD", multipleAllowed: true).AreMultipleAllowed);
        }

        [Fact]
        public void Equality()
        {
            Assert.Equal(new PngChunkType("ABCD"), new PngChunkType("ABCD"));
            Assert.Equal(new PngChunkType("ABCD", multipleAllowed: true), new PngChunkType("ABCD", multipleAllowed: true));
            // NOTE we don't consider the 'allowMultiples' value in the equality test (or hash code)
            Assert.Equal(new PngChunkType("ABCD", multipleAllowed: true), new PngChunkType("ABCD"));
            Assert.NotEqual(new PngChunkType("ABCD"), new PngChunkType("abcd"));
        }
    }
}
