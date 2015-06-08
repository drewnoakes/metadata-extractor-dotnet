using System.Collections.Generic;
using System.Linq;
using Com.Drew.Lang;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PngChunkReaderTest
    {
        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public static IList<PngChunk> ProcessFile(string filePath)
        {
            FileInputStream inputStream = null;
            try
            {
                inputStream = new FileInputStream(filePath);
                return new PngChunkReader().Extract(new StreamReader(inputStream), null).ToList();
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                }
            }
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestExtractMspaint()
        {
            IList<PngChunk> chunks = ProcessFile("Tests/Data/mspaint-8x10.png");
            Assert.AreEqual(6, chunks.Count);
            Assert.AreEqual(PngChunkType.Ihdr, chunks[0].GetChunkType());
            Assert.AreEqual(13, chunks[0].GetBytes().Length);
            Assert.AreEqual(PngChunkType.SRgb, chunks[1].GetChunkType());
            Assert.AreEqual(1, chunks[1].GetBytes().Length);
            Assert.AreEqual(PngChunkType.GAma, chunks[2].GetChunkType());
            Assert.AreEqual(4, chunks[2].GetBytes().Length);
            Assert.AreEqual(PngChunkType.PHYs, chunks[3].GetChunkType());
            Assert.AreEqual(9, chunks[3].GetBytes().Length);
            Assert.AreEqual(PngChunkType.Idat, chunks[4].GetChunkType());
            Assert.AreEqual(17, chunks[4].GetBytes().Length);
            Assert.AreEqual(PngChunkType.Iend, chunks[5].GetChunkType());
            Assert.AreEqual(0, chunks[5].GetBytes().Length);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestExtractPhotoshop()
        {
            IList<PngChunk> chunks = ProcessFile("Tests/Data/photoshop-8x12-rgba32.png");
            Assert.AreEqual(5, chunks.Count);
            Assert.AreEqual(PngChunkType.Ihdr, chunks[0].GetChunkType());
            Assert.AreEqual(13, chunks[0].GetBytes().Length);
            Assert.AreEqual(PngChunkType.TEXt, chunks[1].GetChunkType());
            Assert.AreEqual(25, chunks[1].GetBytes().Length);
            Assert.AreEqual(PngChunkType.ITXt, chunks[2].GetChunkType());
            Assert.AreEqual(802, chunks[2].GetBytes().Length);
            Assert.AreEqual(PngChunkType.Idat, chunks[3].GetChunkType());
            Assert.AreEqual(130, chunks[3].GetBytes().Length);
            Assert.AreEqual(PngChunkType.Iend, chunks[4].GetChunkType());
            Assert.AreEqual(0, chunks[4].GetBytes().Length);
        }
    }
}
