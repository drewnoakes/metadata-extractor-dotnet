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

using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>
    /// Unit tests for
    /// <see cref="ExifSubIFDDirectory"/>
    /// ,
    /// <see cref="ExifIFD0Directory"/>
    /// ,
    /// <see cref="ExifThumbnailDirectory"/>
    /// .
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifDirectoryTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetDirectoryName()
        {
            Directory subIFDDirectory = new ExifSubIFDDirectory();
            Directory ifd0Directory = new ExifIFD0Directory();
            Directory thumbDirectory = new ExifThumbnailDirectory();
            Assert.IsFalse(subIFDDirectory.HasErrors());
            Assert.IsFalse(ifd0Directory.HasErrors());
            Assert.IsFalse(thumbDirectory.HasErrors());
            Assert.AreEqual("Exif IFD0", ifd0Directory.GetName());
            Assert.AreEqual("Exif SubIFD", subIFDDirectory.GetName());
            Assert.AreEqual("Exif Thumbnail", thumbDirectory.GetName());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetThumbnailData()
        {
            ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/withExif.jpg.app1");
            sbyte[] thumbData = directory.GetThumbnailData();
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

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestWriteThumbnail()
        {
            ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            Assert.IsTrue(directory.HasThumbnailData());
            FilePath thumbnailFile = FilePath.CreateTempFile("thumbnail", ".jpg");
            try
            {
                directory.WriteThumbnail(thumbnailFile.GetAbsolutePath());
                FilePath file = new FilePath(thumbnailFile.GetAbsolutePath());
                Assert.AreEqual(2970, (object)file.Length());
                Assert.IsTrue(file.Exists());
            }
            finally
            {
                if (!thumbnailFile.Delete())
                {
                    Assert.Fail("Unable to delete temp thumbnail file.");
                }
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
        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        [Test]
        public virtual void TestResolution()
        {
            Metadata metadata = ExifReaderTest.ProcessBytes("Tests/Data/withUncompressedRGBThumbnail.jpg.app1");
            ExifThumbnailDirectory thumbnailDirectory = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
            Assert.IsNotNull(thumbnailDirectory);
            Assert.AreEqual(72, thumbnailDirectory.GetInt(ExifThumbnailDirectory.TagXResolution));
            ExifIFD0Directory exifIFD0Directory = metadata.GetFirstDirectoryOfType<ExifIFD0Directory>();
            Assert.IsNotNull(exifIFD0Directory);
            Assert.AreEqual(216, exifIFD0Directory.GetInt(ExifIFD0Directory.TagXResolution));
        }
    }
}
