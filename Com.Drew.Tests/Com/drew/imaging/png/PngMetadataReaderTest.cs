using System;
using System.Collections.Generic;
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
        private static Metadata.Metadata ProcessFile([NotNull] string filePath)
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
        [Test, SetCulture("en-GB")]
        public virtual void TestGimpGreyscaleWithManyChunks()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.Local;
            try
            {
                Metadata.Metadata metadata = ProcessFile("Tests/Data/gimp-8x12-greyscale-alpha-time-background.png");
                ICollection<PngDirectory> directories = metadata.GetDirectoriesOfType<PngDirectory>();
                Assert.IsNotNull(directories);
                Tests.AreEqual(6, directories.Count);
                PngDirectory[] dirs = new PngDirectory[directories.Count];
                Collections.ToArray(directories, dirs);
                Tests.AreEqual(PngChunkType.Ihdr, dirs[0].GetPngChunkType());
                Tests.AreEqual(8, dirs[0].GetInt(PngDirectory.TagImageWidth));
                Tests.AreEqual(12, dirs[0].GetInt(PngDirectory.TagImageHeight));
                Tests.AreEqual(8, dirs[0].GetInt(PngDirectory.TagBitsPerSample));
                Tests.AreEqual(4, dirs[0].GetInt(PngDirectory.TagColorType));
                Tests.AreEqual(0, dirs[0].GetInt(PngDirectory.TagCompressionType));
                Tests.AreEqual(0, dirs[0].GetInt(PngDirectory.TagFilterMethod));
                Tests.AreEqual(0, dirs[0].GetInt(PngDirectory.TagInterlaceMethod));
                Tests.AreEqual(PngChunkType.gAMA, dirs[1].GetPngChunkType());
                Tests.AreEqual(0.45455, dirs[1].GetDouble(PngDirectory.TagGamma), 0.00001);
                Tests.AreEqual(PngChunkType.bKGD, dirs[2].GetPngChunkType());
                CollectionAssert.AreEqual(new sbyte[] { 0, 52 }, dirs[2].GetByteArray(PngDirectory.TagBackgroundColor));
                //noinspection ConstantConditions
                Tests.AreEqual(PngChunkType.pHYs, dirs[3].GetPngChunkType());
                Tests.AreEqual(1, dirs[3].GetInt(PngDirectory.TagUnitSpecifier));
                Tests.AreEqual(2835, dirs[3].GetInt(PngDirectory.TagPixelsPerUnitX));
                Tests.AreEqual(2835, dirs[3].GetInt(PngDirectory.TagPixelsPerUnitY));
                Tests.AreEqual(PngChunkType.tIME, dirs[4].GetPngChunkType());
                //Sharpen.Tests.AreEqual("Tue Jan 01 04:08:30 GMT 2013", Sharpen.Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime)));
                var testString = CreateTestString(2013, 00, 01, 04, 08, 30);
                Tests.AreEqual(testString, Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime)));
                Tests.AreEqual(PngChunkType.iTXt, dirs[5].GetPngChunkType());
                IList<KeyValuePair> pairs = (IList<KeyValuePair>)dirs[5].GetObject(PngDirectory.TagTextualData);
                Assert.IsNotNull(pairs);
                Tests.AreEqual(1, pairs.Count);
                Tests.AreEqual("Comment", pairs[0].GetKey());
                Tests.AreEqual("Created with GIMP", pairs[0].GetValue());
            }
            finally
            {
            }
        }

        private string CreateTestString(int year, int month, int day, int hourOfDay, int minute, int second)
        {
            TimeZoneInfo gmt = Extensions.GetTimeZone("GMT");
            Calendar calendar = Calendar.GetInstance(gmt);
            calendar.Set(year, month, day, hourOfDay, minute, second);

            return Extensions.ConvertToString(calendar.GetTime());
        }
    }
}
