/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using Com.Drew.Tools;
using JetBrains.Annotations;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>JUnit test case for class ExifReader.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifReaderTest
	{
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ProcessBytes(string filePath)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new ExifReader().Extract(FileUtil.ReadBytes(filePath), metadata, JpegSegmentType.App1);
			return metadata;
		}

		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static T ProcessBytes<T>(string filePath)
			where T : Com.Drew.Metadata.Directory
		{
			T directory = ProcessBytes(filePath).GetDirectory<T>();
			NUnit.Framework.Assert.IsNotNull(directory);
			return directory;
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExtractWithNullDataThrows()
		{
			try
			{
				new ExifReader().Extract(null, new Com.Drew.Metadata.Metadata(), JpegSegmentType.App1);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (ArgumentNullException)
			{
			}
		}

		// passed
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExtractWithNullMetadataThrows()
		{
			try
			{
				new ExifReader().Extract(new sbyte[10], null, JpegSegmentType.App1);
				NUnit.Framework.Assert.Fail("Exception expected");
			}
			catch (ArgumentNullException)
			{
			}
		}

		// passed
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestLoadFujifilmJpeg()
		{
			ExifSubIFDDirectory directory = ExifReaderTest.ProcessBytes<ExifSubIFDDirectory>("Tests/Data/withExif.jpg.app1");
			string description = directory.GetDescription(ExifSubIFDDirectory.TagIsoEquivalent);
			NUnit.Framework.Assert.IsNotNull(description);
			Sharpen.Tests.AreEqual("80", description);
		}

		// TODO decide if this should still be returned -- it was being calculated upon setting of a related tag
		//      assertEquals("F9", directory.getDescription(ExifSubIFDDirectory.TAG_APERTURE));
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestLoadJpegWithNoExifData()
		{
			sbyte[] badExifData = new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new ExifReader().Extract(badExifData, metadata, JpegSegmentType.App1);
			Sharpen.Tests.AreEqual(0, metadata.GetDirectoryCount());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestCrashRegressionTest()
		{
			// This image was created via a resize in ACDSee.
			// It seems to have a reference to an IFD starting outside the data segment.
			// I've noticed that ACDSee reports a Comment for this image, yet ExifReader doesn't report one.
			ExifSubIFDDirectory directory = ExifReaderTest.ProcessBytes<ExifSubIFDDirectory>("Tests/Data/crash01.jpg.app1");
			Sharpen.Tests.IsTrue(directory.GetTagCount() > 0);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestDateTime()
		{
			ExifIFD0Directory directory = ExifReaderTest.ProcessBytes<ExifIFD0Directory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			Sharpen.Tests.AreEqual("2002:11:27 18:00:35", directory.GetString(ExifIFD0Directory.TagDatetime));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestThumbnailXResolution()
		{
			ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			Rational rational = directory.GetRational(ExifThumbnailDirectory.TagXResolution);
			NUnit.Framework.Assert.IsNotNull(rational);
			Sharpen.Tests.AreEqual(72, rational.GetNumerator());
			Sharpen.Tests.AreEqual(1, rational.GetDenominator());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestThumbnailYResolution()
		{
			ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			Rational rational = directory.GetRational(ExifThumbnailDirectory.TagYResolution);
			NUnit.Framework.Assert.IsNotNull(rational);
			Sharpen.Tests.AreEqual(72, rational.GetNumerator());
			Sharpen.Tests.AreEqual(1, rational.GetDenominator());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestThumbnailOffset()
		{
			ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			Sharpen.Tests.AreEqual(192, directory.GetInt(ExifThumbnailDirectory.TagThumbnailOffset));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestThumbnailLength()
		{
			ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			Sharpen.Tests.AreEqual(2970, directory.GetInt(ExifThumbnailDirectory.TagThumbnailLength));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestThumbnailData()
		{
			ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			sbyte[] thumbnailData = directory.GetThumbnailData();
			NUnit.Framework.Assert.IsNotNull(thumbnailData);
			Sharpen.Tests.AreEqual(2970, thumbnailData.Length);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestThumbnailCompression()
		{
			ExifThumbnailDirectory directory = ExifReaderTest.ProcessBytes<ExifThumbnailDirectory>("Tests/Data/manuallyAddedThumbnail.jpg.app1");
			// 6 means JPEG compression
			Sharpen.Tests.AreEqual(6, directory.GetInt(ExifThumbnailDirectory.TagThumbnailCompression));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestStackOverflowOnRevisitationOfSameDirectory()
		{
			// An error has been discovered in Exif data segments where a directory is referenced
			// repeatedly.  Thanks to Alistair Dickie for providing the sample data used in this
			// unit test.
			Com.Drew.Metadata.Metadata metadata = ProcessBytes("Tests/Data/recursiveDirectories.jpg.app1");
			// Mostly we're just happy at this point that we didn't get stuck in an infinite loop.
			Sharpen.Tests.AreEqual(5, metadata.GetDirectoryCount());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestDifferenceImageAndThumbnailOrientations()
		{
			// This metadata contains different orientations for the thumbnail and the main image.
			// These values used to be merged into a single directory, causing errors.
			// This unit test demonstrates correct behaviour.
			Com.Drew.Metadata.Metadata metadata = ProcessBytes("Tests/Data/repeatedOrientationTagWithDifferentValues.jpg.app1");
			ExifIFD0Directory ifd0Directory = metadata.GetDirectory<ExifIFD0Directory>();
			ExifThumbnailDirectory thumbnailDirectory = metadata.GetDirectory<ExifThumbnailDirectory>();
			NUnit.Framework.Assert.IsNotNull(ifd0Directory);
			NUnit.Framework.Assert.IsNotNull(thumbnailDirectory);
			Sharpen.Tests.AreEqual(1, ifd0Directory.GetInt(ExifIFD0Directory.TagOrientation));
			Sharpen.Tests.AreEqual(8, thumbnailDirectory.GetInt(ExifThumbnailDirectory.TagOrientation));
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
