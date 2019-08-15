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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegSegment"/>.</summary>
    /// <author>Michael Osthege</author>
    public sealed class JpegSegmentTest
    {
        [Fact]
        public void TestEncodeSegmentLength()
        {
            // we are just interested in the payload length
            int expected = 42802 - 2;
            // the encoded value is 42802 because it includes the length mark
            byte[] encoded = new byte[] { 0xA7, 0x32 };
            int decoded = JpegSegment.DecodePayloadLength(encoded[0], encoded[1]);

            Assert.Equal(expected, decoded);
        }

        [Fact]
        public void TestDecodeSegmentLength()
        {
            int decoded = 42802 - 2;
            byte[] expected = new byte[] { 0xA7, 0x32 };
            byte[] encoded = JpegSegment.EncodePayloadLength(decoded);

            Assert.True(encoded.SequenceEqual(expected));
        }
    }
}
