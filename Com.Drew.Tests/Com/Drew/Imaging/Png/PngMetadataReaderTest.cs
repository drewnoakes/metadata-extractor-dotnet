using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging.Png;
using Com.Drew.Lang;
using Com.Drew.Metadata.Png;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngMetadataReaderTest
	{
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		[NotNull]
		public static T ProcessFile<T>(string filePath)
			where T : Com.Drew.Metadata.Directory
		{
			System.Type directoryClass = typeof(T);
			T directory = ProcessFile(filePath).GetDirectory(directoryClass);
			NUnit.Framework.Assert.IsNotNull(directory);
			return directory;
		}

		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		private static Com.Drew.Metadata.Metadata ProcessFile(string filePath)
		{
			FileInputStream inputStream = null;
			try
			{
				inputStream = new FileInputStream(filePath);
				return PngMetadataReader.ReadMetadata(inputStream);
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
		public virtual void TestGimpGreyscaleWithManyChunks()
		{
			PngDirectory directory = ProcessFile<PngDirectory>("Tests/Data/gimp-8x12-greyscale-alpha-time-background.png");
			Sharpen.Tests.AreEqual(8, directory.GetInt(PngDirectory.TagImageWidth));
			Sharpen.Tests.AreEqual(12, directory.GetInt(PngDirectory.TagImageHeight));
			Sharpen.Tests.AreEqual(8, directory.GetInt(PngDirectory.TagBitsPerSample));
			Sharpen.Tests.AreEqual(4, directory.GetInt(PngDirectory.TagColorType));
			Sharpen.Tests.AreEqual(0, directory.GetInt(PngDirectory.TagCompressionType));
			Sharpen.Tests.AreEqual(0, directory.GetInt(PngDirectory.TagFilterMethod));
			Sharpen.Tests.AreEqual(0, directory.GetInt(PngDirectory.TagInterlaceMethod));
			Sharpen.Tests.AreEqual(0.45455, directory.GetDouble(PngDirectory.TagGamma), 0.00001);
			NUnit.Framework.CollectionAssert.AreEqual(new sbyte[] { 0, 52 }, directory.GetByteArray(PngDirectory.TagBackgroundColor));
			//noinspection ConstantConditions
			Sharpen.Tests.AreEqual("Tue Jan 01 04:08:30 GMT 2013", directory.GetDate(PngDirectory.TagLastModificationTime).ToString());
			IList<KeyValuePair> pairs = (IList<KeyValuePair>)directory.GetObject(PngDirectory.TagTextualData);
			NUnit.Framework.Assert.IsNotNull(pairs);
			Sharpen.Tests.AreEqual(1, pairs.Count);
			Sharpen.Tests.AreEqual("Comment", pairs[0].GetKey());
			Sharpen.Tests.AreEqual("Created with GIMP", pairs[0].GetValue());
		}
	}
}
