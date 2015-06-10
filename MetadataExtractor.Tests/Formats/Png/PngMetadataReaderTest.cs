using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.Png.png;
using NUnit.Framework;
using Sharpen;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngMetadataReaderTest
    {
        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        private static Metadata ProcessFile([NotNull] string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open))
                return PngMetadataReader.ReadMetadata(stream);
        }

        [Test, SetCulture("en-GB")]
        public void TestGimpGreyscaleWithManyChunks()
        {
            var timeZone = TimeZoneInfo.Local;
            try
            {
                var metadata = ProcessFile("Tests/Data/gimp-8x12-greyscale-alpha-time-background.png");
                var directories = metadata.GetDirectoriesOfType<PngDirectory>();
                Assert.IsNotNull(directories);
                Assert.AreEqual(6, directories.Count);
                Assert.AreEqual(PngChunkType.Ihdr, directories[0].GetPngChunkType());
                Assert.AreEqual(8, directories[0].GetInt(PngDirectory.TagImageWidth));
                Assert.AreEqual(12, directories[0].GetInt(PngDirectory.TagImageHeight));
                Assert.AreEqual(8, directories[0].GetInt(PngDirectory.TagBitsPerSample));
                Assert.AreEqual(4, directories[0].GetInt(PngDirectory.TagColorType));
                Assert.AreEqual(0, directories[0].GetInt(PngDirectory.TagCompressionType));
                Assert.AreEqual(0, directories[0].GetInt(PngDirectory.TagFilterMethod));
                Assert.AreEqual(0, directories[0].GetInt(PngDirectory.TagInterlaceMethod));
                Assert.AreEqual(PngChunkType.GAma, directories[1].GetPngChunkType());
                Assert.AreEqual(0.45455, directories[1].GetDouble(PngDirectory.TagGamma), 0.00001);
                Assert.AreEqual(PngChunkType.BKgd, directories[2].GetPngChunkType());
                CollectionAssert.AreEqual(new byte[] { 0, 52 }, directories[2].GetByteArray(PngDirectory.TagBackgroundColor));
                //noinspection ConstantConditions
                Assert.AreEqual(PngChunkType.PHYs, directories[3].GetPngChunkType());
                Assert.AreEqual(1, directories[3].GetInt(PngDirectory.TagUnitSpecifier));
                Assert.AreEqual(2835, directories[3].GetInt(PngDirectory.TagPixelsPerUnitX));
                Assert.AreEqual(2835, directories[3].GetInt(PngDirectory.TagPixelsPerUnitY));
                Assert.AreEqual(PngChunkType.TIme, directories[4].GetPngChunkType());
                //Sharpen.Tests.AreEqual("Tue Jan 01 04:08:30 GMT 2013", Sharpen.Extensions.ConvertToString(dirs[4].GetDate(PngDirectory.TagLastModificationTime)));
                var testString = CreateTestString(2013, 00, 01, 04, 08, 30);
                Assert.AreEqual(testString, directories[4].GetDate(PngDirectory.TagLastModificationTime).Value.ToString("ddd MMM dd HH:mm:ss zzz yyyy"));
                Assert.AreEqual(PngChunkType.ITXt, directories[5].GetPngChunkType());
                var pairs = (IList<KeyValuePair>)directories[5].GetObject(PngDirectory.TagTextualData);
                Assert.IsNotNull(pairs);
                Assert.AreEqual(1, pairs.Count);
                Assert.AreEqual("Comment", pairs[0].Key);
                Assert.AreEqual("Created with GIMP", pairs[0].Value);
            }
            finally
            {
            }
        }

        private static string CreateTestString(int year, int month, int day, int hourOfDay, int minute, int second)
        {
            var gmt = Extensions.GetTimeZone("GMT");
            var calendar = Calendar.GetInstance(gmt);
            calendar.Set(year, month, day, hourOfDay, minute, second);

            return calendar.GetTime().ToString("ddd MMM dd HH:mm:ss zzz yyyy");
        }
    }
}
