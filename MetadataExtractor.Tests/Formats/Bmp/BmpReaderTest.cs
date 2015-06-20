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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class BmpReaderTest
    {
        [NotNull]
        public static BmpHeaderDirectory ProcessBytes([NotNull] string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
                return new BmpReader().Extract(new SequentialStreamReader(stream));
        }

        [Fact]
        public void TestMsPaint16Color()
        {
            var directory = ProcessBytes("Tests/Data/16color-10x10.bmp");
            Assert.False(directory.HasError);
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageWidth));
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageHeight));
            Assert.Equal(4, directory.GetInt32(BmpHeaderDirectory.TagBitsPerPixel));
            Assert.Equal("None", directory.GetDescription(BmpHeaderDirectory.TagCompression));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagXPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagYPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagPaletteColourCount));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagImportantColourCount));
            Assert.Equal(1, directory.GetInt32(BmpHeaderDirectory.TagColourPlanes));
            Assert.Equal(40, directory.GetInt32(BmpHeaderDirectory.TagHeaderSize));
        }

        [Fact]
        public void TestMsPaint24Bpp()
        {
            var directory = ProcessBytes("Tests/Data/24bpp-10x10.bmp");
            Assert.False(directory.HasError);
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageWidth));
            Assert.Equal(10, directory.GetInt32(BmpHeaderDirectory.TagImageHeight));
            Assert.Equal(24, directory.GetInt32(BmpHeaderDirectory.TagBitsPerPixel));
            Assert.Equal("None", directory.GetDescription(BmpHeaderDirectory.TagCompression));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagXPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagYPixelsPerMeter));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagPaletteColourCount));
            Assert.Equal(0, directory.GetInt32(BmpHeaderDirectory.TagImportantColourCount));
            Assert.Equal(1, directory.GetInt32(BmpHeaderDirectory.TagColourPlanes));
            Assert.Equal(40, directory.GetInt32(BmpHeaderDirectory.TagHeaderSize));
        }
    }
}
