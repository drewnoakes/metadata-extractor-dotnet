#region License
//
// Copyright 2002-2015 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>
    /// Unit tests for <see cref="ExifSubIfdDirectory"/>, <see cref="ExifIfd0Directory"/>, <see cref="ExifThumbnailDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifDirectoryTest
    {
        [Fact]
        public void TestGetDirectoryName()
        {
            Directory subIfdDirectory = new ExifSubIfdDirectory();
            Directory ifd0Directory = new ExifIfd0Directory();
            Directory thumbDirectory = new ExifThumbnailDirectory();
            Assert.False(subIfdDirectory.HasError);
            Assert.False(ifd0Directory.HasError);
            Assert.False(thumbDirectory.HasError);
            Assert.Equal("Exif IFD0", ifd0Directory.Name);
            Assert.Equal("Exif SubIFD", subIfdDirectory.Name);
            Assert.Equal("Exif Thumbnail", thumbDirectory.Name);
        }

        [Fact]
        public void TestGetThumbnailData()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<ExifThumbnailDirectory>("Tests/Data/withExif.jpg.app1");
            var thumbData = directory.ThumbnailData;

            Assert.NotNull(thumbData);

            // attempt to read the thumbnail -- it should be a legal Jpeg file
            JpegSegmentReader.ReadSegments(new SequentialByteArrayReader(thumbData));
        }

        [Fact]
        public void TestWriteThumbnail()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            Assert.True(directory.HasThumbnailData);
            var thumbnailFile = Path.GetTempFileName();
            try
            {
                directory.WriteThumbnail(thumbnailFile);
                var filePath = new FileInfo(thumbnailFile);
                Assert.Equal(2970L, filePath.Length);
                Assert.True(filePath.Exists);
            }
            finally
            {
                File.Delete(thumbnailFile);
            }
        }

//    public void TestContainsThumbnail()
//    {
//        ExifSubIFDDirectory exifDirectory = new ExifSubIFDDirectory();
//
//        assertTrue(!exifDirectory.hasThumbnailData());
//
//        exifDirectory.setObject(ExifSubIFDDirectory.TAG_THUMBNAIL_DATA, "foo");
//
//        assertTrue(exifDirectory.hasThumbnailData());
//    }

        [Fact]
        public void TestResolution()
        {
            var directories = ExifReaderTest.ProcessSegmentBytes("Tests/Data/withUncompressedRGBThumbnail.jpg.app1");

            var thumbnailDirectory = directories.OfType<ExifThumbnailDirectory>().FirstOrDefault();
            Assert.NotNull(thumbnailDirectory);
            Assert.Equal(72, thumbnailDirectory.GetInt32(ExifDirectoryBase.TagXResolution));

            var exifIfd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            Assert.NotNull(exifIfd0Directory);
            Assert.Equal(216, exifIfd0Directory.GetInt32(ExifDirectoryBase.TagXResolution));
        }
    }
}
