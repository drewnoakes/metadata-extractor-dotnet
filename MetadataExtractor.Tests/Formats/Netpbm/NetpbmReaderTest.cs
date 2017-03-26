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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using MetadataExtractor.Formats.Netpbm;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Netpbm
{
    /// <summary>Unit tests for <see cref="NetpbmReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public sealed class NetpbmReaderTest
    {
        [Fact]
        public void HeaderParsing()
        {
            Verify("P1 1024 768 255 ", formatType: 1, width: 1024, height: 768, maxVal: null);
            Verify("P2 1024 768 255 ", formatType: 2, width: 1024, height: 768, maxVal: null);
            Verify("P3 1024 768 255 ", formatType: 3, width: 1024, height: 768, maxVal: null);
            Verify("P4 1024 768 255 ", formatType: 4, width: 1024, height: 768, maxVal: null);
            Verify("P5 1024 768 255 ", formatType: 5, width: 1024, height: 768, maxVal: null);
            Verify("P6 1024 768 255 ", formatType: 6, width: 1024, height: 768, maxVal: null);
            Verify("P2\t1024\t768\t255 ", formatType: 2, width: 1024, height: 768, maxVal: 255);
            Verify("P2\n1024\n768\n255\n", formatType: 2, width: 1024, height: 768, maxVal: 255);
            Verify("P2\r\n1024\r\n768\r\n255\n", formatType: 2, width: 1024, height: 768, maxVal: 255);
            Verify("P2\r\n# comment\r\n1024\r\n768\r\n255\n", formatType: 2, width: 1024, height: 768, maxVal: 255);
            Verify("P2\r\n1024\r\n# comment\r\n768\r\n255\n", formatType: 2, width: 1024, height: 768, maxVal: 255);
            Verify("P2\r\n1024 # comment\r\n768\r\n255\n", formatType: 2, width: 1024, height: 768, maxVal: 255);
        }

        private static void Verify(string content, int formatType, int width, int height, int? maxVal)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var directory = new NetpbmReader().Extract(stream);

            Assert.Equal(formatType, directory.GetInt32(NetpbmHeaderDirectory.TagFormatType));
            Assert.Equal(width, directory.GetInt32(NetpbmHeaderDirectory.TagWidth));
            Assert.Equal(height, directory.GetInt32(NetpbmHeaderDirectory.TagHeight));

            if (maxVal != null)
                Assert.Equal(maxVal, directory.GetInt32(NetpbmHeaderDirectory.TagMaximumValue));
        }
    }
}