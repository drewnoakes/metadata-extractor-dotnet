// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Png
{
    /// <summary>Unit tests for <see cref="PngChunkReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkReaderTest
    {
        /// <exception cref="PngProcessingException"/>
        /// <exception cref="IOException"/>
        private static IList<PngChunk> ProcessFile(string filePath)
        {
            using var stream = TestDataUtil.OpenRead(filePath);
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
            Assert.Single(chunks[1].Bytes);
            Assert.Equal(PngChunkType.gAMA, chunks[2].ChunkType);
            Assert.Equal(4, chunks[2].Bytes.Length);
            Assert.Equal(PngChunkType.pHYs, chunks[3].ChunkType);
            Assert.Equal(9, chunks[3].Bytes.Length);
            Assert.Equal(PngChunkType.IDAT, chunks[4].ChunkType);
            Assert.Equal(17, chunks[4].Bytes.Length);
            Assert.Equal(PngChunkType.IEND, chunks[5].ChunkType);
            Assert.Empty(chunks[5].Bytes);
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
            Assert.Empty(chunks[4].Bytes);
        }
    }
}
