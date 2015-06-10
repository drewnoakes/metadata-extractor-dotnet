using System.Collections.Generic;
using System.IO;
using System.Linq;
using Com.Drew.Lang;
using NUnit.Framework;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkReaderTest
    {
        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static IList<PngChunk> ProcessFile(string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open))
                return new PngChunkReader().Extract(new SequentialStreamReader(stream), null).ToList();
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestExtractMspaint()
        {
            IList<PngChunk> chunks = ProcessFile("Tests/Data/mspaint-8x10.png");
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

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestExtractPhotoshop()
        {
            IList<PngChunk> chunks = ProcessFile("Tests/Data/photoshop-8x12-rgba32.png");
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
