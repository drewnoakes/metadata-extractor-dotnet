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

using JetBrains.Annotations;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Bmp
{
    /// <summary>Unit tests for <see cref="BmpReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class BmpReaderTest
    {
        [NotNull]
        private static BmpHeaderDirectory ProcessBytes([NotNull] string filePath)
        {
            using (var stream = TestDataUtil.OpenRead(filePath))
                return new BmpReader().Extract(new SequentialStreamReader(stream));
        }

        [Fact]
        public void MsPaint16Color()
        {
            var directory = ProcessBytes("Data/16color-10x10.bmp");
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
        public void MsPaint24Bpp()
        {
            var directory = ProcessBytes("Data/24bpp-10x10.bmp");
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
