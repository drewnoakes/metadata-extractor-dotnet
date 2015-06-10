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

using MetadataExtractor.Formats.Jpeg;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegmentDataTest
    {
        [Test]
        public void TestAddAndGetSegment()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes = new byte[] { 1, 2, 3 };
            segmentData.AddSegment(segmentType, segmentBytes);
            Assert.AreEqual(1, segmentData.GetSegmentCount(segmentType));
            CollectionAssert.AreEqual(segmentBytes, segmentData.GetSegment(segmentType));
        }

        [Test]
        public void TestContainsSegment()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes = new byte[] { 1, 2, 3 };
            Assert.IsTrue(!segmentData.ContainsSegment(segmentType));
            segmentData.AddSegment(segmentType, segmentBytes);
            Assert.IsTrue(segmentData.ContainsSegment(segmentType));
        }

        [Test]
        public void TestAddingMultipleSegments()
        {
            var segmentData = new JpegSegmentData();
            var segmentType1 = JpegSegmentType.App3;
            var segmentType2 = JpegSegmentType.App4;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType1, segmentBytes1);
            segmentData.AddSegment(segmentType2, segmentBytes2);
            Assert.AreEqual(1, segmentData.GetSegmentCount(segmentType1));
            Assert.AreEqual(1, segmentData.GetSegmentCount(segmentType2));
            CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentType1));
            CollectionAssert.AreEqual(segmentBytes2, segmentData.GetSegment(segmentType2));
        }

        [Test]
        public void TestSegmentWithMultipleOccurrences()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType, segmentBytes1);
            segmentData.AddSegment(segmentType, segmentBytes2);
            Assert.AreEqual(2, segmentData.GetSegmentCount(segmentType));
            CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentType));
            CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentType, 0));
            CollectionAssert.AreEqual(segmentBytes2, segmentData.GetSegment(segmentType, 1));
        }

        [Test]
        public void TestRemoveSegmentOccurrence()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType, segmentBytes1);
            segmentData.AddSegment(segmentType, segmentBytes2);
            Assert.AreEqual(2, segmentData.GetSegmentCount(segmentType));
            CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentType, 0));
            segmentData.RemoveSegmentOccurrence(segmentType, 0);
            CollectionAssert.AreEqual(segmentBytes2, segmentData.GetSegment(segmentType, 0));
        }

        [Test]
        public void TestRemoveSegment()
        {
            var segmentData = new JpegSegmentData();
            var segmentType = JpegSegmentType.App3;
            var segmentBytes1 = new byte[] { 1, 2, 3 };
            var segmentBytes2 = new byte[] { 3, 2, 1 };
            segmentData.AddSegment(segmentType, segmentBytes1);
            segmentData.AddSegment(segmentType, segmentBytes2);
            Assert.AreEqual(2, segmentData.GetSegmentCount(segmentType));
            Assert.IsTrue(segmentData.ContainsSegment(segmentType));
            CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentType, 0));
            segmentData.RemoveAllSegments(segmentType);
            Assert.IsTrue(!segmentData.ContainsSegment(segmentType));
            Assert.AreEqual(0, segmentData.GetSegmentCount(segmentType));
        }
    }
}
