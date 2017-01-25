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
using MetadataExtractor.Formats.Png;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <summary>Unit tests for <see cref="PngChunkReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkReaderTest
    {
        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        private static IList<PngChunk> ProcessFile(string filePath)
        {
            using (var stream = TestDataUtil.OpenRead(filePath))
                return new PngChunkReader().Extract(new SequentialStreamReader(stream), null).ToList();
        }

        [Fact]
        public void Extract_MSPaint()
        {
            var chunks = ProcessFile("Data/mspaint-8x10.png");
            Assert.Equal(6, chunks.Count);
            Assert.Equal(PngChunkType.IHDR, chunks[0].ChunkType);
            Assert.Equal(13, chunks[0].Bytes.Length);
            Assert.Equal(PngChunkType.sRGB, chunks[1].ChunkType);
            Assert.Equal(1, chunks[1].Bytes.Length);
            Assert.Equal(PngChunkType.gAMA, chunks[2].ChunkType);
            Assert.Equal(4, chunks[2].Bytes.Length);
            Assert.Equal(PngChunkType.pHYs, chunks[3].ChunkType);
            Assert.Equal(9, chunks[3].Bytes.Length);
            Assert.Equal(PngChunkType.IDAT, chunks[4].ChunkType);
            Assert.Equal(17, chunks[4].Bytes.Length);
            Assert.Equal(PngChunkType.IEND, chunks[5].ChunkType);
            Assert.Equal(0, chunks[5].Bytes.Length);
        }

        [Fact]
        public void Extract_Photoshop()
        {
            var chunks = ProcessFile("Data/photoshop-8x12-rgba32.png");
            Assert.Equal(5, chunks.Count);
            Assert.Equal(PngChunkType.IHDR, chunks[0].ChunkType);
            Assert.Equal(13, chunks[0].Bytes.Length);
            Assert.Equal(PngChunkType.tEXt, chunks[1].ChunkType);
            Assert.Equal(25, chunks[1].Bytes.Length);
            Assert.Equal(PngChunkType.iTXt, chunks[2].ChunkType);
            Assert.Equal(802, chunks[2].Bytes.Length);
            Assert.Equal(PngChunkType.IDAT, chunks[3].ChunkType);
            Assert.Equal(130, chunks[3].Bytes.Length);
            Assert.Equal(PngChunkType.IEND, chunks[4].ChunkType);
            Assert.Equal(0, chunks[4].Bytes.Length);
        }
    }
}
