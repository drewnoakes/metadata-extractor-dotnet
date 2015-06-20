using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkReaderTest
    {
        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static IList<PngChunk> ProcessFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
                return new PngChunkReader().Extract(new SequentialStreamReader(stream), null).ToList();
        }

        [Fact]
        public void TestExtractMSPaint()
        {
            var chunks = ProcessFile("Tests/Data/mspaint-8x10.png");
            Assert.Equal(6, chunks.Count);
            Assert.Equal(PngChunkType.Ihdr, chunks[0].ChunkType);
            Assert.Equal(13, chunks[0].Bytes.Length);
            Assert.Equal(PngChunkType.SRgb, chunks[1].ChunkType);
            Assert.Equal(1, chunks[1].Bytes.Length);
            Assert.Equal(PngChunkType.GAma, chunks[2].ChunkType);
            Assert.Equal(4, chunks[2].Bytes.Length);
            Assert.Equal(PngChunkType.PHYs, chunks[3].ChunkType);
            Assert.Equal(9, chunks[3].Bytes.Length);
            Assert.Equal(PngChunkType.Idat, chunks[4].ChunkType);
            Assert.Equal(17, chunks[4].Bytes.Length);
            Assert.Equal(PngChunkType.Iend, chunks[5].ChunkType);
            Assert.Equal(0, chunks[5].Bytes.Length);
        }

        [Fact]
        public void TestExtractPhotoshop()
        {
            var chunks = ProcessFile("Tests/Data/photoshop-8x12-rgba32.png");
            Assert.Equal(5, chunks.Count);
            Assert.Equal(PngChunkType.Ihdr, chunks[0].ChunkType);
            Assert.Equal(13, chunks[0].Bytes.Length);
            Assert.Equal(PngChunkType.TEXt, chunks[1].ChunkType);
            Assert.Equal(25, chunks[1].Bytes.Length);
            Assert.Equal(PngChunkType.ITXt, chunks[2].ChunkType);
            Assert.Equal(802, chunks[2].Bytes.Length);
            Assert.Equal(PngChunkType.Idat, chunks[3].ChunkType);
            Assert.Equal(130, chunks[3].Bytes.Length);
            Assert.Equal(PngChunkType.Iend, chunks[4].ChunkType);
            Assert.Equal(0, chunks[4].Bytes.Length);
        }
    }
}
