using System;
using System.Collections.Generic;
using System.IO;
using Com.Drew.Lang;
using Com.Drew.Metadata.Png;
using JetBrains.Annotations;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class PngMetadataReaderTest
	{
		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		private static Com.Drew.Metadata.Metadata ProcessFile([NotNull] string filePath)
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
        [NUnit.Framework.Test, SetCulture("en-GB")]
		public virtual void TestGimpGreyscaleWithManyChunks()
		{
			TimeZoneInfo timeZone = System.TimeZoneInfo.Local;
			try
			{
				Com.Drew.Metadata.Metadata metadata = ProcessFile("Tests/Data/gimp-8x12-greyscale-alpha-time-background.png");
				ICollection<PngDirectory> directories = metadata.GetDirectoriesOfType<PngDirectory>();
				NUnit.Framework.Assert.IsNotNull(directories);
				Sharpen.Tests.AreEqual(6, directories.Count);
				PngDirectory[] dirs = new PngDirectory[directories.Count];
				Sharpen.Collections.ToArray(directories, dirs);
				Sharpen.Tests.AreEqual(PngChunkType.Ihdr, dirs[0].GetPngChunkType());
				Sharpen.Tests.AreEqual(8, dirs[0].GetInt(PngDirectory.TagImageWidth));
				Sharpen.Tests.AreEqual(12, dirs[0].GetInt(PngDirectory.TagImageHeight));
				Sharpen.Tests.AreEqual(8, dirs[0].GetInt(PngDirectory.TagBitsPerSample));
				Sharpen.Tests.AreEqual(4, dirs[0].GetInt(PngDirectory.TagColorType));
				Sharpen.Tests.AreEqual(0, dirs[0].GetInt(PngDirectory.TagCompressionType));
				Sharpen.Tests.AreEqual(0, dirs[0].GetInt(PngDirectory.TagFilterMethod));
				Sharpen.Tests.AreEqual(0, dirs[0].GetInt(PngDirectory.TagInterlaceMethod));
				Sharpen.Tests.AreEqual(PngChunkType.gAMA, dirs[1].GetPngChunkType());
				Sharpen.Tests.AreEqual(0.45455, dirs[1].GetDouble(PngDirectory.TagGamma), 0.00001);
				Sharpen.Tests.AreEqual(PngChunkType.bKGD, dirs[2].GetPngChunkType());
				NUnit.Framework.CollectionAssert.AreEqual(new sbyte[] { 0, 52 }, dirs[2].GetByteArray(PngDirectory.TagBackgroundColor));
				//noinspection ConstantConditions
				Sharpen.Tests.AreEqual(PngChunkType.pHYs, dirs[3].GetPngChunkType());
				Sharpen.Tests.AreEqual(1, dirs[3].GetInt(PngDirectory.TagUnitSpecifier));
				Sharpen.Tests.AreEqual(2835, dirs[3].GetInt(PngDirectory.TagPixelsPerUnitX));
				Sharpen.Tests.AreEqual(2835, dirs[3].GetInt(PngDirectory.TagPixelsPerUnitY));
				Sharpen.Tests.AreEqual(PngChunkType.tIME, dirs[4].GetPngChunkType());
			    //Sharpen.Tests.AreEqual("Tue Jan 01 04:08:30 GMT 2013", Sharpen.Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime)));
                var testString = CreateTestString(2013, 00, 01, 04, 08, 30);
                Sharpen.Tests.AreEqual(testString, Sharpen.Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime)));
				Sharpen.Tests.AreEqual(PngChunkType.iTXt, dirs[5].GetPngChunkType());
				IList<KeyValuePair> pairs = (IList<KeyValuePair>)dirs[5].GetObject(PngDirectory.TagTextualData);
				NUnit.Framework.Assert.IsNotNull(pairs);
				Sharpen.Tests.AreEqual(1, pairs.Count);
				Sharpen.Tests.AreEqual("Comment", pairs[0].GetKey());
				Sharpen.Tests.AreEqual("Created with GIMP", pairs[0].GetValue());
			}
			finally
			{
			}
		}

        private string CreateTestString(int year, int month, int day, int hourOfDay, int minute, int second)
	    {
            TimeZoneInfo gmt = Sharpen.Extensions.GetTimeZone("GMT");
            Calendar calendar = Calendar.GetInstance(gmt);
            calendar.Set(year, month, day, hourOfDay, minute, second);

	        return Sharpen.Extensions.ConvertToString(calendar.GetTime());
	    }
	}
}
