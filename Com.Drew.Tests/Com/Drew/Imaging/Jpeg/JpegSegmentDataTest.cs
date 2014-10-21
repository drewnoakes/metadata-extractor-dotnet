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
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegSegmentDataTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestAddAndGetSegment()
		{
			JpegSegmentData segmentData = new JpegSegmentData();
			sbyte segmentMarker = unchecked((sbyte)12);
			sbyte[] segmentBytes = new sbyte[] { 1, 2, 3 };
			segmentData.AddSegment(segmentMarker, segmentBytes);
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(segmentMarker));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes, segmentData.GetSegment(segmentMarker));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestContainsSegment()
		{
			JpegSegmentData segmentData = new JpegSegmentData();
			sbyte segmentMarker = unchecked((sbyte)12);
			sbyte[] segmentBytes = new sbyte[] { 1, 2, 3 };
			Sharpen.Tests.IsTrue(!segmentData.ContainsSegment(segmentMarker));
			segmentData.AddSegment(segmentMarker, segmentBytes);
			Sharpen.Tests.IsTrue(segmentData.ContainsSegment(segmentMarker));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestAddingMultipleSegments()
		{
			JpegSegmentData segmentData = new JpegSegmentData();
			sbyte segmentMarker1 = unchecked((sbyte)12);
			sbyte segmentMarker2 = unchecked((sbyte)21);
			sbyte[] segmentBytes1 = new sbyte[] { 1, 2, 3 };
			sbyte[] segmentBytes2 = new sbyte[] { 3, 2, 1 };
			segmentData.AddSegment(segmentMarker1, segmentBytes1);
			segmentData.AddSegment(segmentMarker2, segmentBytes2);
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(segmentMarker1));
			Sharpen.Tests.AreEqual(1, segmentData.GetSegmentCount(segmentMarker2));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentMarker1));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes2, segmentData.GetSegment(segmentMarker2));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestSegmentWithMultipleOccurrences()
		{
			JpegSegmentData segmentData = new JpegSegmentData();
			sbyte segmentMarker = unchecked((sbyte)12);
			sbyte[] segmentBytes1 = new sbyte[] { 1, 2, 3 };
			sbyte[] segmentBytes2 = new sbyte[] { 3, 2, 1 };
			segmentData.AddSegment(segmentMarker, segmentBytes1);
			segmentData.AddSegment(segmentMarker, segmentBytes2);
			Sharpen.Tests.AreEqual(2, segmentData.GetSegmentCount(segmentMarker));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentMarker));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentMarker, 0));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes2, segmentData.GetSegment(segmentMarker, 1));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestRemoveSegmentOccurrence()
		{
			JpegSegmentData segmentData = new JpegSegmentData();
			sbyte segmentMarker = unchecked((sbyte)12);
			sbyte[] segmentBytes1 = new sbyte[] { 1, 2, 3 };
			sbyte[] segmentBytes2 = new sbyte[] { 3, 2, 1 };
			segmentData.AddSegment(segmentMarker, segmentBytes1);
			segmentData.AddSegment(segmentMarker, segmentBytes2);
			Sharpen.Tests.AreEqual(2, segmentData.GetSegmentCount(segmentMarker));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentMarker, 0));
			segmentData.RemoveSegmentOccurrence(segmentMarker, 0);
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes2, segmentData.GetSegment(segmentMarker, 0));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestRemoveSegment()
		{
			JpegSegmentData segmentData = new JpegSegmentData();
			sbyte segmentMarker = unchecked((sbyte)12);
			sbyte[] segmentBytes1 = new sbyte[] { 1, 2, 3 };
			sbyte[] segmentBytes2 = new sbyte[] { 3, 2, 1 };
			segmentData.AddSegment(segmentMarker, segmentBytes1);
			segmentData.AddSegment(segmentMarker, segmentBytes2);
			Sharpen.Tests.AreEqual(2, segmentData.GetSegmentCount(segmentMarker));
			Sharpen.Tests.IsTrue(segmentData.ContainsSegment(segmentMarker));
			NUnit.Framework.CollectionAssert.AreEqual(segmentBytes1, segmentData.GetSegment(segmentMarker, 0));
			segmentData.RemoveSegment(segmentMarker);
			Sharpen.Tests.IsTrue(!segmentData.ContainsSegment(segmentMarker));
			Sharpen.Tests.AreEqual(0, segmentData.GetSegmentCount(segmentMarker));
		}
	}
}
