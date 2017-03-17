#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

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
