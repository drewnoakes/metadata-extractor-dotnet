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

using MetadataExtractor.Formats.Iptc;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Iptc
{
    /// <summary>Unit tests for <see cref="Iso2022Converter"/>.</summary>
    public sealed class Iso2022ConverterTest
    {
        [Fact]
        public void ConvertEscapeSequenceToEncodingName()
        {
            Assert.Equal("UTF-8", Iso2022Converter.ConvertEscapeSequenceToEncodingName(new byte[] { 0x1B, 0x25, 0x47 }));
            Assert.Equal("ISO-8859-1", Iso2022Converter.ConvertEscapeSequenceToEncodingName(new byte[] { 0x1B, 0xE2, 0x80, 0xA2, 0x41 }));
            Assert.Null(Iso2022Converter.ConvertEscapeSequenceToEncodingName(new byte[] { 1, 2, 3, 4 }));
        }
    }
}
