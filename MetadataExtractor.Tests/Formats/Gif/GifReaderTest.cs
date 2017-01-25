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
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Gif
{
    /// <summary>Unit tests for <see cref="GifReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GifReaderTest
    {
        [NotNull]
        private static IEnumerable<Directory> ProcessBytes([NotNull] string file)
        {
            using (var stream = TestDataUtil.OpenRead(file))
                return new GifReader().Extract(new SequentialStreamReader(stream));
        }

        [Fact]
        public void MsPaintGif()
        {
            var directory = ProcessBytes("Data/mspaint-10x10.gif").OfType<GifHeaderDirectory>().Single();

            Assert.False(directory.HasError);

            Assert.Equal("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Assert.Equal(10, directory.GetInt32(GifHeaderDirectory.TagImageWidth));
            Assert.Equal(10, directory.GetInt32(GifHeaderDirectory.TagImageHeight));
            Assert.Equal(256, directory.GetInt32(GifHeaderDirectory.TagColorTableSize));
            Assert.False(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Assert.Equal(8, directory.GetInt32(GifHeaderDirectory.TagBitsPerPixel));
            Assert.True(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Assert.Equal(0, directory.GetInt32(GifHeaderDirectory.TagBackgroundColorIndex));
        }

        [Fact]
        public void PhotoshopGif()
        {
            var directory = ProcessBytes("Data/photoshop-8x12-32colors-alpha.gif").OfType<GifHeaderDirectory>().Single();

            Assert.False(directory.HasError);

            Assert.Equal("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Assert.Equal(8, directory.GetInt32(GifHeaderDirectory.TagImageWidth));
            Assert.Equal(12, directory.GetInt32(GifHeaderDirectory.TagImageHeight));
            Assert.Equal(32, directory.GetInt32(GifHeaderDirectory.TagColorTableSize));
            Assert.False(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Assert.Equal(5, directory.GetInt32(GifHeaderDirectory.TagBitsPerPixel));
            Assert.True(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Assert.Equal(8, directory.GetInt32(GifHeaderDirectory.TagBackgroundColorIndex));
        }
    }
}
