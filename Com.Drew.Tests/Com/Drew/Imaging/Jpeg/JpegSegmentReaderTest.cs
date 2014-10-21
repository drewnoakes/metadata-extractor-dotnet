/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
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
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Tools;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
	/// <summary>
	/// Unit tests for
	/// <see cref="JpegSegmentReader"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegSegmentReaderTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestReadAllSegments()
		{
			JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/withExifAndIptc.jpg"), null);
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App0));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app0"), segmentData.GetSegment(JpegSegmentType.App0));
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App0, 1));
			Sharpen.Tests.AreEqual(2, segmentData.GetSegmentCount(JpegSegmentType.App1));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app1.0"), segmentData.GetSegment(JpegSegmentType.App1, 0));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app1.1"), segmentData.GetSegment(JpegSegmentType.App1, 1));
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App1, 2));
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App2));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app2"), segmentData.GetSegment(JpegSegmentType.App2));
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App2, 1));
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.Appd));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.appd"), segmentData.GetSegment(JpegSegmentType.Appd));
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.Appd, 1));
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.Appe));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.appe"), segmentData.GetSegment(JpegSegmentType.Appe));
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.Appe, 1));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App3));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App4));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App5));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App6));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App7));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App8));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App9));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appa));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appb));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appc));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appf));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Com));
			Sharpen.Tests.AreEqual(4, segmentData.GetSegmentCount(JpegSegmentType.Dht));
			Sharpen.Tests.AreEqual(2, segmentData.GetSegmentCount(JpegSegmentType.Dqt));
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.Sof0));
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App3, 0));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestReadSpecificSegments()
		{
			JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/withExifAndIptc.jpg"), Arrays.AsList(JpegSegmentType.App0, JpegSegmentType.App2).AsIterable());
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App0));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App1));
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App2));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appd));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appe));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App3));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App4));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App5));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App6));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App7));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App8));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App9));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appa));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appb));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appc));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appf));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Com));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Dht));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Sof0));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app0"), segmentData.GetSegment(JpegSegmentType.App0));
			NUnit.Framework.CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app2"), segmentData.GetSegment(JpegSegmentType.App2));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestLoadJpegWithoutExifDataReturnsNull()
		{
			JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/noExif.jpg"), null);
			NUnit.Framework.Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App1));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestWithNonJpegFile()
		{
			try
			{
				JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/test.txt"), null);
				NUnit.Framework.Assert.Fail("shouldn't be able to construct JpegSegmentReader with non-JPEG file");
			}
			catch (JpegProcessingException)
			{
			}
		}
		// expect exception
	}
}
