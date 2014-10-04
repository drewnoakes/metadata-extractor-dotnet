using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging.Png;
using Com.Drew.Lang;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
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
				return Iterables.ToList(new PngChunkReader().Extract(new Com.Drew.Lang.StreamReader(inputStream), null));
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
		[NUnit.Framework.Test]
		public virtual void TestExtractMspaint()
		{
			IList<PngChunk> chunks = ProcessFile("Tests/Data/mspaint-8x10.png");
			Sharpen.Tests.AreEqual(6, chunks.Count);
			Sharpen.Tests.AreEqual(PngChunkType.Ihdr, chunks[0].GetType());
			Sharpen.Tests.AreEqual(13, chunks[0].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.sRGB, chunks[1].GetType());
			Sharpen.Tests.AreEqual(1, chunks[1].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.gAMA, chunks[2].GetType());
			Sharpen.Tests.AreEqual(4, chunks[2].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.pHYs, chunks[3].GetType());
			Sharpen.Tests.AreEqual(9, chunks[3].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.Idat, chunks[4].GetType());
			Sharpen.Tests.AreEqual(17, chunks[4].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.Iend, chunks[5].GetType());
			Sharpen.Tests.AreEqual(0, chunks[5].GetBytes().Length);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExtractPhotoshop()
		{
			IList<PngChunk> chunks = ProcessFile("Tests/Data/photoshop-8x12-rgba32.png");
			Sharpen.Tests.AreEqual(5, chunks.Count);
			Sharpen.Tests.AreEqual(PngChunkType.Ihdr, chunks[0].GetType());
			Sharpen.Tests.AreEqual(13, chunks[0].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.tEXt, chunks[1].GetType());
			Sharpen.Tests.AreEqual(25, chunks[1].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.iTXt, chunks[2].GetType());
			Sharpen.Tests.AreEqual(802, chunks[2].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.Idat, chunks[3].GetType());
			Sharpen.Tests.AreEqual(130, chunks[3].GetBytes().Length);
			Sharpen.Tests.AreEqual(PngChunkType.Iend, chunks[4].GetType());
			Sharpen.Tests.AreEqual(0, chunks[4].GetBytes().Length);
		}
	}
}
