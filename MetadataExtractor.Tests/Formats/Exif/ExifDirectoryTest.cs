/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.IO;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>
    /// Unit tests for <see cref="ExifSubIfdDirectory"/>, <see cref="ExifIfd0Directory"/>, <see cref="ExifThumbnailDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifDirectoryTest
    {

        [Test]
        public void TestGetDirectoryName()
        {
            Directory subIfdDirectory = new ExifSubIfdDirectory();
            Directory ifd0Directory = new ExifIfd0Directory();
            Directory thumbDirectory = new ExifThumbnailDirectory();
            Assert.IsFalse(subIfdDirectory.HasErrors());
            Assert.IsFalse(ifd0Directory.HasErrors());
            Assert.IsFalse(thumbDirectory.HasErrors());
            Assert.AreEqual("Exif IFD0", ifd0Directory.GetName());
            Assert.AreEqual("Exif SubIFD", subIfdDirectory.GetName());
            Assert.AreEqual("Exif Thumbnail", thumbDirectory.GetName());
        }


        [Test]
        public void TestGetThumbnailData()
        {
            var directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/withExif.jpg.app1");
            var thumbData = directory.GetThumbnailData();
            Assert.IsNotNull(thumbData);
            try
            {
                // attempt to read the thumbnail -- it should be a legal Jpeg file
                JpegSegmentReader.ReadSegments(new SequentialByteArrayReader(thumbData), null);
            }
            catch (JpegProcessingException)
            {
                Assert.Fail("Unable to construct JpegSegmentReader from thumbnail data");
            }
        }


        [Test]
        public void TestWriteThumbnail()
        {
            var directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            Assert.IsTrue(directory.HasThumbnailData());
            var thumbnailFile = Path.GetTempFileName();
            try
            {
                directory.WriteThumbnail(thumbnailFile);
                var filePath = new FileInfo(thumbnailFile);
                Assert.AreEqual(2970L, filePath.Length);
                Assert.IsTrue(filePath.Exists);
            }
            finally
            {
                File.Delete(thumbnailFile);
            }
        }

        //    @Test
        //    public void testContainsThumbnail()
        //    {
        //        ExifSubIFDDirectory exifDirectory = new ExifSubIFDDirectory();
        //
        //        assertTrue(!exifDirectory.hasThumbnailData());
        //
        //        exifDirectory.setObject(ExifSubIFDDirectory.TAG_THUMBNAIL_DATA, "foo");
        //
        //        assertTrue(exifDirectory.hasThumbnailData());
        //    }
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="MetadataException"/>
        [Test]
        public void TestResolution()
        {
            var metadata = ExifReaderTest.ProcessBytes("Tests/Data/withUncompressedRGBThumbnail.jpg.app1");
            var thumbnailDirectory = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
            Assert.IsNotNull(thumbnailDirectory);
            Assert.AreEqual(72, thumbnailDirectory.GetInt(ExifDirectoryBase.TagXResolution));
            var exifIfd0Directory = metadata.GetFirstDirectoryOfType<ExifIfd0Directory>();
            Assert.IsNotNull(exifIfd0Directory);
            Assert.AreEqual(216, exifIfd0Directory.GetInt(ExifDirectoryBase.TagXResolution));
        }
    }
}
