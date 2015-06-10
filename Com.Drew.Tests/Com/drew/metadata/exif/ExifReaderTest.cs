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

using System;
using System.Collections.Generic;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>JUnit test case for class ExifReader.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifReaderTest
    {
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata ProcessBytes([NotNull] string filePath)
        {
            var metadata = new Metadata();
            var bytes = System.IO.File.ReadAllBytes(filePath);
            new ExifReader().Extract(new ByteArrayReader(bytes), metadata, ExifReader.JpegSegmentPreamble.Length);
            return metadata;
        }

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static T ProcessBytes<T>([NotNull] string filePath)
            where T : Directory
        {
            var directory = ProcessBytes(filePath).GetFirstDirectoryOfType<T>();
            Assert.IsNotNull(directory);
            return directory;
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestExtractWithNullDataThrows()
        {
            try
            {
                new ExifReader().ReadJpegSegments(null, new Metadata(), JpegSegmentType.App1);
                Assert.Fail("Exception expected");
            }
            catch (NullReferenceException)
            {
            }
        }

        // passed
        /// <exception cref="System.Exception"/>
        [Test]
        public void TestLoadFujifilmJpeg()
        {
            var directory = ProcessBytes<ExifSubIfdDirectory>("Tests/Data/withExif.jpg.app1");
            var description = directory.GetDescription(ExifDirectoryBase.TagIsoEquivalent);
            Assert.IsNotNull(description);
            Assert.AreEqual("80", description);
        }

        // TODO decide if this should still be returned -- it was being calculated upon setting of a related tag
        //      assertEquals("F9", directory.getDescription(ExifSubIFDDirectory.TAG_APERTURE));
        /// <exception cref="System.Exception"/>
        [Test]
        public void TestReadJpegSegmentWithNoExifData()
        {
            var badExifData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var metadata = new Metadata();
            var segments = new List<byte[]>();
            segments.Add(badExifData);
            new ExifReader().ReadJpegSegments(segments, metadata, JpegSegmentType.App1);
            Assert.AreEqual(0, metadata.GetDirectoryCount());
            Assert.IsFalse(metadata.HasErrors());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestCrashRegressionTest()
        {
            // This image was created via a resize in ACDSee.
            // It seems to have a reference to an IFD starting outside the data segment.
            // I've noticed that ACDSee reports a Comment for this image, yet ExifReader doesn't report one.
            var directory = ProcessBytes<ExifSubIfdDirectory>("Tests/Data/crash01.jpg.app1");
            Assert.IsTrue(directory.GetTagCount() > 0);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestDateTime()
        {
            var directory = ProcessBytes<ExifIfd0Directory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            Assert.AreEqual("2002:11:27 18:00:35", directory.GetString(ExifDirectoryBase.TagDatetime));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestThumbnailXResolution()
        {
            var directory = ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            var rational = directory.GetRational(ExifDirectoryBase.TagXResolution);
            Assert.IsNotNull(rational);
            Assert.AreEqual(72, (object)rational.GetNumerator());
            Assert.AreEqual(1, (object)rational.GetDenominator());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestThumbnailYResolution()
        {
            var directory = ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            var rational = directory.GetRational(ExifDirectoryBase.TagYResolution);
            Assert.IsNotNull(rational);
            Assert.AreEqual(72, (object)rational.GetNumerator());
            Assert.AreEqual(1, (object)rational.GetDenominator());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestThumbnailOffset()
        {
            var directory = ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            Assert.AreEqual(192, directory.GetInt(ExifThumbnailDirectory.TagThumbnailOffset));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestThumbnailLength()
        {
            var directory = ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            Assert.AreEqual(2970, directory.GetInt(ExifThumbnailDirectory.TagThumbnailLength));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestThumbnailData()
        {
            var directory = ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            var thumbnailData = directory.GetThumbnailData();
            Assert.IsNotNull(thumbnailData);
            Assert.AreEqual(2970, thumbnailData.Length);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestThumbnailCompression()
        {
            var directory = ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
            // 6 means JPEG compression
            Assert.AreEqual(6, directory.GetInt(ExifThumbnailDirectory.TagThumbnailCompression));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestStackOverflowOnRevisitationOfSameDirectory()
        {
            // An error has been discovered in Exif data segments where a directory is referenced
            // repeatedly.  Thanks to Alistair Dickie for providing the sample data used in this
            // unit test.
            var metadata = ProcessBytes("Tests/Data/recursiveDirectories.jpg.app1");
            // Mostly we're just happy at this point that we didn't get stuck in an infinite loop.
            Assert.AreEqual(5, metadata.GetDirectoryCount());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestDifferenceImageAndThumbnailOrientations()
        {
            // This metadata contains different orientations for the thumbnail and the main image.
            // These values used to be merged into a single directory, causing errors.
            // This unit test demonstrates correct behaviour.
            var metadata = ProcessBytes("Tests/Data/repeatedOrientationTagWithDifferentValues.jpg.app1");
            var ifd0Directory = metadata.GetFirstDirectoryOfType<ExifIfd0Directory>();
            var thumbnailDirectory = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
            Assert.IsNotNull(ifd0Directory);
            Assert.IsNotNull(thumbnailDirectory);
            Assert.AreEqual(1, ifd0Directory.GetInt(ExifDirectoryBase.TagOrientation));
            Assert.AreEqual(8, thumbnailDirectory.GetInt(ExifDirectoryBase.TagOrientation));
        }
/*
    public void testUncompressedYCbCrThumbnail() throws Exception
    {
        String fileName = "withUncompressedYCbCrThumbnail.jpg";
        String thumbnailFileName = "withUncompressedYCbCrThumbnail.bmp";
        Metadata metadata = new ExifReader(new File(fileName)).extract();
        ExifSubIFDDirectory directory = (ExifSubIFDDirectory)metadata.getOrCreateDirectory(ExifSubIFDDirectory.class);
        directory.writeThumbnail(thumbnailFileName);

        fileName = "withUncompressedYCbCrThumbnail2.jpg";
        thumbnailFileName = "withUncompressedYCbCrThumbnail2.bmp";
        metadata = new ExifReader(new File(fileName)).extract();
        directory = (ExifSubIFDDirectory)metadata.getOrCreateDirectory(ExifSubIFDDirectory.class);
        directory.writeThumbnail(thumbnailFileName);
        fileName = "withUncompressedYCbCrThumbnail3.jpg";
        thumbnailFileName = "withUncompressedYCbCrThumbnail3.bmp";
        metadata = new ExifReader(new File(fileName)).extract();
        directory = (ExifSubIFDDirectory)metadata.getOrCreateDirectory(ExifSubIFDDirectory.class);
        directory.writeThumbnail(thumbnailFileName);
        fileName = "withUncompressedYCbCrThumbnail4.jpg";
        thumbnailFileName = "withUncompressedYCbCrThumbnail4.bmp";
        metadata = new ExifReader(new File(fileName)).extract();
        directory = (ExifSubIFDDirectory)metadata.getOrCreateDirectory(ExifSubIFDDirectory.class);
        directory.writeThumbnail(thumbnailFileName);
    }

    public void testUncompressedRGBThumbnail() throws Exception
    {
        String fileName = "withUncompressedRGBThumbnail.jpg";
        String thumbnailFileName = "withUncompressedRGBThumbnail.bmp";
        Metadata metadata = new ExifReader(new File(fileName)).extract();
        ExifSubIFDDirectory directory = (ExifSubIFDDirectory)metadata.getOrCreateDirectory(ExifSubIFDDirectory.class);
        directory.writeThumbnail(thumbnailFileName);
    }
*/
    }
}
