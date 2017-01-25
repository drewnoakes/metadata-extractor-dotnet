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

using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegComponent"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegComponentTest
    {
        [Fact]
        public void GetComponentCharacter()
        {
            Assert.Equal("Y",  new JpegComponent(1, 2, 3).Name);
            Assert.Equal("Cb", new JpegComponent(2, 2, 3).Name);
            Assert.Equal("Cr", new JpegComponent(3, 2, 3).Name);
            Assert.Equal("I",  new JpegComponent(4, 2, 3).Name);
            Assert.Equal("Q",  new JpegComponent(5, 2, 3).Name);
        }
    }
}
