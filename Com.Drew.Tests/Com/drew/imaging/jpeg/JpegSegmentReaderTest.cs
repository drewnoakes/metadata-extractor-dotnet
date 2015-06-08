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
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class JpegSegmentReaderTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestReadAllSegments()
        {
            JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/withExifAndIptc.jpg"), null);
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App0));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app0"), segmentData.GetSegment(JpegSegmentType.App0));
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App0, 1));
            Assert.AreEqual(2, segmentData.GetSegmentCount(JpegSegmentType.App1));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app1.0"), segmentData.GetSegment(JpegSegmentType.App1, 0));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app1.1"), segmentData.GetSegment(JpegSegmentType.App1, 1));
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App1, 2));
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App2));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app2"), segmentData.GetSegment(JpegSegmentType.App2));
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App2, 1));
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.Appd));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.appd"), segmentData.GetSegment(JpegSegmentType.Appd));
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.Appd, 1));
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.Appe));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.appe"), segmentData.GetSegment(JpegSegmentType.Appe));
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.Appe, 1));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App3));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App4));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App5));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App6));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App7));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App8));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App9));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appa));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appb));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appc));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appf));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Com));
            Assert.AreEqual(4, segmentData.GetSegmentCount(JpegSegmentType.Dht));
            Assert.AreEqual(2, segmentData.GetSegmentCount(JpegSegmentType.Dqt));
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.Sof0));
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App3, 0));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestReadSpecificSegments()
        {
            JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/withExifAndIptc.jpg"), Arrays.AsList(JpegSegmentType.App0, JpegSegmentType.App2).AsIterable());
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App0));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App1));
            Assert.AreEqual(1, segmentData.GetSegmentCount(JpegSegmentType.App2));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appd));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appe));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App3));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App4));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App5));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App6));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App7));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App8));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.App9));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appa));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appb));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appc));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Appf));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Com));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Dht));
            Assert.AreEqual(0, segmentData.GetSegmentCount(JpegSegmentType.Sof0));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app0"), segmentData.GetSegment(JpegSegmentType.App0));
            CollectionAssert.AreEqual(FileUtil.ReadBytes("Tests/Data/withExifAndIptc.jpg.app2"), segmentData.GetSegment(JpegSegmentType.App2));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestLoadJpegWithoutExifDataReturnsNull()
        {
            JpegSegmentData segmentData = JpegSegmentReader.ReadSegments(new FilePath("Tests/Data/noExif.jpg"), null);
            Assert.IsNull(segmentData.GetSegment(JpegSegmentType.App1));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestWithNonJpegFile()
        {
            try
            {
                JpegSegmentReader.ReadSegments(new FilePath("MetadataExtractor.Tests.dll"), null);
                Assert.Fail("shouldn't be able to construct JpegSegmentReader with non-JPEG file");
            }
            catch (JpegProcessingException)
            {
            }
        }
        // expect exception
    }
}
