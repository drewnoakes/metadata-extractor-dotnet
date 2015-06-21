#region License
//
// Copyright 2002-2015 Drew Noakes
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
            Assert.True(new PngChunkType("ABCD").IsCritical);
            Assert.False(new PngChunkType("aBCD").IsCritical);
        }

        [Fact]
        public void TestIsAncillary()
        {
            Assert.False(new PngChunkType("ABCD").IsAncillary);
            Assert.True(new PngChunkType("aBCD").IsAncillary);
        }

        [Fact]
        public void TestIsPrivate()
        {
            Assert.True(new PngChunkType("ABCD").IsPrivate);
            Assert.False(new PngChunkType("AbCD").IsPrivate);
        }

        [Fact]
        public void TestIsSafeToCopy()
        {
            Assert.False(new PngChunkType("ABCD").IsSafeToCopy);
            Assert.True(new PngChunkType("ABCd").IsSafeToCopy);
        }

        [Fact]
        public void TestAreMultipleAllowed()
        {
            Assert.False(new PngChunkType("ABCD").AreMultipleAllowed);
            Assert.False(new PngChunkType("ABCD", false).AreMultipleAllowed);
            Assert.True(new PngChunkType("ABCD", true).AreMultipleAllowed);
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
