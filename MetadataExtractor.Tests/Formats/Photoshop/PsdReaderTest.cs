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

using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Photoshop
{
    /// <summary>Unit tests for <see cref="PsdReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PsdReaderTest
    {
        [NotNull]
        private static PsdHeaderDirectory ProcessBytes([NotNull] string filePath)
        {
            using (var stream = TestDataUtil.OpenRead(filePath))
            {
                var directory = new PsdReader().Extract(new SequentialStreamReader(stream)).OfType<PsdHeaderDirectory>().FirstOrDefault();
                Assert.NotNull(directory);
                return directory;
            }
        }

        [Fact]
        public void Psd8X8X8BitGrayscale()
        {
            var directory = ProcessBytes("Data/8x4x8bit-Grayscale.psd");
            Assert.Equal(8, directory.GetInt32(PsdHeaderDirectory.TagImageWidth));
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagImageHeight));
            Assert.Equal(8, directory.GetInt32(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.Equal(1, directory.GetInt32(PsdHeaderDirectory.TagChannelCount));
            // 1 = grayscale
            Assert.Equal(1, directory.GetInt32(PsdHeaderDirectory.TagColorMode));
        }

        [Fact]
        public void Psd10X12X16BitCmyk()
        {
            var directory = ProcessBytes("Data/10x12x16bit-CMYK.psd");
            Assert.Equal(10, directory.GetInt32(PsdHeaderDirectory.TagImageWidth));
            Assert.Equal(12, directory.GetInt32(PsdHeaderDirectory.TagImageHeight));
            Assert.Equal(16, directory.GetInt32(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagChannelCount));
            // 4 = CMYK
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagColorMode));
        }
    }
}
