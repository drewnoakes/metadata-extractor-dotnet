using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.IO;
using NUnit.Framework;

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

        [Test]
        public void TestExtractMSPaint()
        {
            var chunks = ProcessFile("Tests/Data/mspaint-8x10.png");
            Assert.AreEqual(6, chunks.Count);
            Assert.AreEqual(PngChunkType.Ihdr, chunks[0].ChunkType);
            Assert.AreEqual(13, chunks[0].Bytes.Length);
            Assert.AreEqual(PngChunkType.SRgb, chunks[1].ChunkType);
            Assert.AreEqual(1, chunks[1].Bytes.Length);
            Assert.AreEqual(PngChunkType.GAma, chunks[2].ChunkType);
            Assert.AreEqual(4, chunks[2].Bytes.Length);
            Assert.AreEqual(PngChunkType.PHYs, chunks[3].ChunkType);
            Assert.AreEqual(9, chunks[3].Bytes.Length);
            Assert.AreEqual(PngChunkType.Idat, chunks[4].ChunkType);
            Assert.AreEqual(17, chunks[4].Bytes.Length);
            Assert.AreEqual(PngChunkType.Iend, chunks[5].ChunkType);
            Assert.AreEqual(0, chunks[5].Bytes.Length);
        }

        [Test]
        public void TestExtractPhotoshop()
        {
            var chunks = ProcessFile("Tests/Data/photoshop-8x12-rgba32.png");
            Assert.AreEqual(5, chunks.Count);
            Assert.AreEqual(PngChunkType.Ihdr, chunks[0].ChunkType);
            Assert.AreEqual(13, chunks[0].Bytes.Length);
            Assert.AreEqual(PngChunkType.TEXt, chunks[1].ChunkType);
            Assert.AreEqual(25, chunks[1].Bytes.Length);
            Assert.AreEqual(PngChunkType.ITXt, chunks[2].ChunkType);
            Assert.AreEqual(802, chunks[2].Bytes.Length);
            Assert.AreEqual(PngChunkType.Idat, chunks[3].ChunkType);
            Assert.AreEqual(130, chunks[3].Bytes.Length);
            Assert.AreEqual(PngChunkType.Iend, chunks[4].ChunkType);
            Assert.AreEqual(0, chunks[4].Bytes.Length);
        }
    }
}
