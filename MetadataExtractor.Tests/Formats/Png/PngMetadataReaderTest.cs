using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Png;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngMetadataReaderTest
    {
        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        private static IReadOnlyList<Directory> ProcessFile([NotNull] string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
                return PngMetadataReader.ReadMetadata(stream);
        }

        [Fact, UseCulture("en-GB")]
        public void TestGimpGreyscaleWithManyChunks()
        {
            var directories = ProcessFile("Tests/Data/gimp-8x12-greyscale-alpha-time-background.png").OfType<PngDirectory>().ToList();
            Assert.NotNull(directories);
            Assert.Equal(6, directories.Count);
            Assert.Equal(PngChunkType.Ihdr, directories[0].GetPngChunkType());
            Assert.Equal(8, directories[0].GetInt32(PngDirectory.TagImageWidth));
            Assert.Equal(12, directories[0].GetInt32(PngDirectory.TagImageHeight));
            Assert.Equal(8, directories[0].GetInt32(PngDirectory.TagBitsPerSample));
            Assert.Equal(4, directories[0].GetInt32(PngDirectory.TagColorType));
            Assert.Equal(0, directories[0].GetInt32(PngDirectory.TagCompressionType));
            Assert.Equal(0, directories[0].GetInt32(PngDirectory.TagFilterMethod));
            Assert.Equal(0, directories[0].GetInt32(PngDirectory.TagInterlaceMethod));
            Assert.Equal(PngChunkType.GAma, directories[1].GetPngChunkType());
            Assert.Equal(0.45455, directories[1].GetDouble(PngDirectory.TagGamma), 5);
            Assert.Equal(PngChunkType.BKgd, directories[2].GetPngChunkType());
            Assert.Equal(new byte[] { 0, 52 }, directories[2].GetByteArray(PngDirectory.TagBackgroundColor));
            Assert.Equal(PngChunkType.PHYs, directories[3].GetPngChunkType());
            Assert.Equal(1, directories[3].GetInt32(PngDirectory.TagUnitSpecifier));
            Assert.Equal(2835, directories[3].GetInt32(PngDirectory.TagPixelsPerUnitX));
            Assert.Equal(2835, directories[3].GetInt32(PngDirectory.TagPixelsPerUnitY));
            Assert.Equal(PngChunkType.TIme, directories[4].GetPngChunkType());
            //Sharpen.Tests.AreEqual("Tue Jan 01 04:08:30 GMT 2013", Sharpen.Extensions.ConvertToString(dirs[4].GetDateTimeNullable(PngDirectory.TagLastModificationTime)));
            var testString = CreateTestString(2013, 01, 01, 04, 08, 30);
            Assert.Equal(testString, directories[4].GetDateTimeNullable(PngDirectory.TagLastModificationTime).Value.ToString("ddd MMM dd HH:mm:ss zzz yyyy"));
            Assert.Equal(PngChunkType.ITXt, directories[5].GetPngChunkType());
            var pairs = (IList<KeyValuePair>)directories[5].GetObject(PngDirectory.TagTextualData);
            Assert.NotNull(pairs);
            Assert.Equal(1, pairs.Count);
            Assert.Equal("Comment", pairs[0].Key);
            Assert.Equal("Created with GIMP", pairs[0].Value);
        }

        private static string CreateTestString(int year, int month, int day, int hourOfDay, int minute, int second)
        {
            return new DateTime(year, month, day, hourOfDay, minute, second)
                .ToString("ddd MMM dd HH:mm:ss zzz yyyy");
        }
    }
}
