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
    public sealed class PngMetadataReaderTest
    {
        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        private static Metadata.Metadata ProcessFile([NotNull] string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open))
                return PngMetadataReader.ReadMetadata(stream);
        }

        /// <exception cref="System.Exception"/>
        [Test, SetCulture("en-GB")]
        public void TestGimpGreyscaleWithManyChunks()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.Local;
            try
            {
                Metadata.Metadata metadata = ProcessFile("Tests/Data/gimp-8x12-greyscale-alpha-time-background.png");
                ICollection<PngDirectory> directories = metadata.GetDirectoriesOfType<PngDirectory>();
                Assert.IsNotNull(directories);
                Assert.AreEqual(6, directories.Count);
                PngDirectory[] dirs = new PngDirectory[directories.Count];
                Collections.ToArray(directories, dirs);
                Assert.AreEqual(PngChunkType.Ihdr, dirs[0].GetPngChunkType());
                Assert.AreEqual(8, dirs[0].GetInt(PngDirectory.TagImageWidth));
                Assert.AreEqual(12, dirs[0].GetInt(PngDirectory.TagImageHeight));
                Assert.AreEqual(8, dirs[0].GetInt(PngDirectory.TagBitsPerSample));
                Assert.AreEqual(4, dirs[0].GetInt(PngDirectory.TagColorType));
                Assert.AreEqual(0, dirs[0].GetInt(PngDirectory.TagCompressionType));
                Assert.AreEqual(0, dirs[0].GetInt(PngDirectory.TagFilterMethod));
                Assert.AreEqual(0, dirs[0].GetInt(PngDirectory.TagInterlaceMethod));
                Assert.AreEqual(PngChunkType.GAma, dirs[1].GetPngChunkType());
                Assert.AreEqual(0.45455, dirs[1].GetDouble(PngDirectory.TagGamma), 0.00001);
                Assert.AreEqual(PngChunkType.BKgd, dirs[2].GetPngChunkType());
                CollectionAssert.AreEqual(new byte[] { 0, 52 }, dirs[2].GetByteArray(PngDirectory.TagBackgroundColor));
                //noinspection ConstantConditions
                Assert.AreEqual(PngChunkType.PHYs, dirs[3].GetPngChunkType());
                Assert.AreEqual(1, dirs[3].GetInt(PngDirectory.TagUnitSpecifier));
                Assert.AreEqual(2835, dirs[3].GetInt(PngDirectory.TagPixelsPerUnitX));
                Assert.AreEqual(2835, dirs[3].GetInt(PngDirectory.TagPixelsPerUnitY));
                Assert.AreEqual(PngChunkType.TIme, dirs[4].GetPngChunkType());
                //Sharpen.Tests.AreEqual("Tue Jan 01 04:08:30 GMT 2013", Sharpen.Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime)));
                var testString = CreateTestString(2013, 00, 01, 04, 08, 30);
                Assert.AreEqual(testString, Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime).Value));
                Assert.AreEqual(PngChunkType.ITXt, dirs[5].GetPngChunkType());
                IList<KeyValuePair> pairs = (IList<KeyValuePair>)dirs[5].GetObject(PngDirectory.TagTextualData);
                Assert.IsNotNull(pairs);
                Assert.AreEqual(1, pairs.Count);
                Assert.AreEqual("Comment", pairs[0].GetKey());
                Assert.AreEqual("Created with GIMP", pairs[0].GetValue());
            }
            finally
            {
            }
        }

        private static string CreateTestString(int year, int month, int day, int hourOfDay, int minute, int second)
        {
            TimeZoneInfo gmt = Extensions.GetTimeZone("GMT");
            Calendar calendar = Calendar.GetInstance(gmt);
            calendar.Set(year, month, day, hourOfDay, minute, second);

            return calendar.GetTime().ToString("ddd MMM dd HH:mm:ss zzz yyyy");
        }
    }
}
