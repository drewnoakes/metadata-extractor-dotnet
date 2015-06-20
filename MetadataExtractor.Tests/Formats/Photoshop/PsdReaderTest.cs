/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Photoshop
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PsdReaderTest
    {
        [NotNull]
        public static PsdHeaderDirectory ProcessBytes([NotNull] string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var directory = new PsdReader().Extract(new SequentialStreamReader(stream)).OfType<PsdHeaderDirectory>().FirstOrDefault();
                Assert.NotNull(directory);
                return directory;
            }
        }

        [Fact]
        public void Test8X8X8BitGrayscale()
        {
            var directory = ProcessBytes("Tests/Data/8x4x8bit-Grayscale.psd");
            Assert.Equal(8, directory.GetInt32(PsdHeaderDirectory.TagImageWidth));
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagImageHeight));
            Assert.Equal(8, directory.GetInt32(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.Equal(1, directory.GetInt32(PsdHeaderDirectory.TagChannelCount));
            // 1 = grayscale
            Assert.Equal(1, directory.GetInt32(PsdHeaderDirectory.TagColorMode));
        }

        [Fact]
        public void Test10X12X16BitCmyk()
        {
            var directory = ProcessBytes("Tests/Data/10x12x16bit-CMYK.psd");
            Assert.Equal(10, directory.GetInt32(PsdHeaderDirectory.TagImageWidth));
            Assert.Equal(12, directory.GetInt32(PsdHeaderDirectory.TagImageHeight));
            Assert.Equal(16, directory.GetInt32(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagChannelCount));
            // 4 = CMYK
            Assert.Equal(4, directory.GetInt32(PsdHeaderDirectory.TagColorMode));
        }
    }
}
