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
using System.Collections.Generic;
using Com.Drew.Imaging.Jpeg;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
	/// <summary>Holds a collection of JPEG data segments.</summary>
	/// <remarks>
	/// Holds a collection of JPEG data segments.  This need not necessarily be all segments
	/// within the JPEG. For example, it may be convenient to store only the non-image
	/// segments when analysing metadata.
	/// <p/>
	/// Segments are keyed via their
	/// <see cref="JpegSegmentType"/>
	/// . Where multiple segments use the
	/// same segment type, they will all be stored and available.
	/// <p/>
	/// Each segment type may contain multiple entries. Conceptually the model is:
	/// <code>Map&lt;JpegSegmentType, Collection&lt;byte[]&gt;&gt;</code>. This class provides
	/// convenience methods around that structure.
	/// </remarks>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegSegmentData
	{
		[NotNull]
		private readonly Dictionary<sbyte, IList<sbyte[]>> _segmentDataMap = new Dictionary<sbyte, IList<sbyte[]>>(10);

		// TODO key this on JpegSegmentType rather than Byte, and hopefully lose much of the use of 'byte' with this class
		/// <summary>Adds segment bytes to the collection.</summary>
		/// <param name="segmentType">the type of the segment being added</param>
		/// <param name="segmentBytes">the byte array holding data for the segment being added</param>
		public virtual void AddSegment(sbyte segmentType, sbyte[] segmentBytes)
		{
			GetOrCreateSegmentList(segmentType).Add(segmentBytes);
		}

		/// <summary>Gets the set of JPEG segment type identifiers.</summary>
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			ICollection<JpegSegmentType> segmentTypes = new HashSet<JpegSegmentType>();
			foreach (sbyte segmentTypeByte in _segmentDataMap.Keys)
			{
				JpegSegmentType segmentType = JpegSegmentType.FromByte(segmentTypeByte);
				if (segmentType == null)
				{
					throw new InvalidOperationException("Should not have a segmentTypeByte that is not in the enum: " + Sharpen.Extensions.ToHexString(segmentTypeByte));
				}
				segmentTypes.Add(segmentType);
			}
			return segmentTypes.AsIterable();
		}

		/// <summary>Gets the first JPEG segment data for the specified type.</summary>
		/// <param name="segmentType">the JpegSegmentType for the desired segment</param>
		/// <returns>a byte[] containing segment data or null if no data exists for that segment</returns>
		[CanBeNull]
		public virtual sbyte[] GetSegment(sbyte segmentType)
		{
			return GetSegment(segmentType, 0);
		}

		/// <summary>Gets the first JPEG segment data for the specified type.</summary>
		/// <param name="segmentType">the JpegSegmentType for the desired segment</param>
		/// <returns>a byte[] containing segment data or null if no data exists for that segment</returns>
		[CanBeNull]
		public virtual sbyte[] GetSegment(JpegSegmentType segmentType)
		{
			return GetSegment(segmentType.byteValue, 0);
		}

		/// <summary>Gets segment data for a specific occurrence and type.</summary>
		/// <remarks>
		/// Gets segment data for a specific occurrence and type.  Use this method when more than one occurrence
		/// of segment data for a given type exists.
		/// </remarks>
		/// <param name="segmentType">identifies the required segment</param>
		/// <param name="occurrence">the zero-based index of the occurrence</param>
		/// <returns>the segment data as a byte[], or null if no segment exists for the type & occurrence</returns>
		[CanBeNull]
		public virtual sbyte[] GetSegment(JpegSegmentType segmentType, int occurrence)
		{
			return GetSegment(segmentType.byteValue, occurrence);
		}

		/// <summary>Gets segment data for a specific occurrence and type.</summary>
		/// <remarks>
		/// Gets segment data for a specific occurrence and type.  Use this method when more than one occurrence
		/// of segment data for a given type exists.
		/// </remarks>
		/// <param name="segmentType">identifies the required segment</param>
		/// <param name="occurrence">the zero-based index of the occurrence</param>
		/// <returns>the segment data as a byte[], or null if no segment exists for the type & occurrence</returns>
		[CanBeNull]
		public virtual sbyte[] GetSegment(sbyte segmentType, int occurrence)
		{
			IList<sbyte[]> segmentList = GetSegmentList(segmentType);
			return segmentList != null && segmentList.Count > occurrence ? segmentList[occurrence] : null;
		}

		/// <summary>Returns all instances of a given JPEG segment.</summary>
		/// <remarks>Returns all instances of a given JPEG segment.  If no instances exist, an empty sequence is returned.</remarks>
		/// <param name="segmentType">a number which identifies the type of JPEG segment being queried</param>
		/// <returns>zero or more byte arrays, each holding the data of a JPEG segment</returns>
		[NotNull]
		public virtual Iterable<sbyte[]> GetSegments(JpegSegmentType segmentType)
		{
			return GetSegments(segmentType.byteValue);
		}

		/// <summary>Returns all instances of a given JPEG segment.</summary>
		/// <remarks>Returns all instances of a given JPEG segment.  If no instances exist, an empty sequence is returned.</remarks>
		/// <param name="segmentType">a number which identifies the type of JPEG segment being queried</param>
		/// <returns>zero or more byte arrays, each holding the data of a JPEG segment</returns>
		[NotNull]
		public virtual Iterable<sbyte[]> GetSegments(sbyte segmentType)
		{
			IList<sbyte[]> segmentList = GetSegmentList(segmentType);
			return (segmentList == null ? new AList<sbyte[]>() : segmentList).AsIterable();
		}

		[CanBeNull]
		private IList<sbyte[]> GetSegmentList(sbyte segmentType)
		{
			return _segmentDataMap.Get(segmentType);
		}

		[NotNull]
		private IList<sbyte[]> GetOrCreateSegmentList(sbyte segmentType)
		{
			IList<sbyte[]> segmentList;
			if (_segmentDataMap.ContainsKey(segmentType))
			{
				segmentList = _segmentDataMap.Get(segmentType);
			}
			else
			{
				segmentList = new AList<sbyte[]>();
				_segmentDataMap.Put(segmentType, segmentList);
			}
			return segmentList;
		}

		/// <summary>Returns the count of segment data byte arrays stored for a given segment type.</summary>
		/// <param name="segmentType">identifies the required segment</param>
		/// <returns>the segment count (zero if no segments exist).</returns>
		public virtual int GetSegmentCount(JpegSegmentType segmentType)
		{
			return GetSegmentCount(segmentType.byteValue);
		}

		/// <summary>Returns the count of segment data byte arrays stored for a given segment type.</summary>
		/// <param name="segmentType">identifies the required segment</param>
		/// <returns>the segment count (zero if no segments exist).</returns>
		public virtual int GetSegmentCount(sbyte segmentType)
		{
			IList<sbyte[]> segmentList = GetSegmentList(segmentType);
			return segmentList == null ? 0 : segmentList.Count;
		}

		/// <summary>Removes a specified instance of a segment's data from the collection.</summary>
		/// <remarks>
		/// Removes a specified instance of a segment's data from the collection.  Use this method when more than one
		/// occurrence of segment data exists for a given type exists.
		/// </remarks>
		/// <param name="segmentType">identifies the required segment</param>
		/// <param name="occurrence">the zero-based index of the segment occurrence to remove.</param>
		public virtual void RemoveSegmentOccurrence(JpegSegmentType segmentType, int occurrence)
		{
			RemoveSegmentOccurrence(segmentType.byteValue, occurrence);
		}

		/// <summary>Removes a specified instance of a segment's data from the collection.</summary>
		/// <remarks>
		/// Removes a specified instance of a segment's data from the collection.  Use this method when more than one
		/// occurrence of segment data exists for a given type exists.
		/// </remarks>
		/// <param name="segmentType">identifies the required segment</param>
		/// <param name="occurrence">the zero-based index of the segment occurrence to remove.</param>
		public virtual void RemoveSegmentOccurrence(sbyte segmentType, int occurrence)
		{
			IList<sbyte[]> segmentList = _segmentDataMap.Get(segmentType);
			segmentList.Remove(occurrence);
		}

		/// <summary>Removes all segments from the collection having the specified type.</summary>
		/// <param name="segmentType">identifies the required segment</param>
		public virtual void RemoveSegment(JpegSegmentType segmentType)
		{
			RemoveSegment(segmentType.byteValue);
		}

		/// <summary>Removes all segments from the collection having the specified type.</summary>
		/// <param name="segmentType">identifies the required segment</param>
		public virtual void RemoveSegment(sbyte segmentType)
		{
			Sharpen.Collections.Remove(_segmentDataMap, segmentType);
		}

		/// <summary>Determines whether data is present for a given segment type.</summary>
		/// <param name="segmentType">identifies the required segment</param>
		/// <returns>true if data exists, otherwise false</returns>
		public virtual bool ContainsSegment(JpegSegmentType segmentType)
		{
			return ContainsSegment(segmentType.byteValue);
		}

		/// <summary>Determines whether data is present for a given segment type.</summary>
		/// <param name="segmentType">identifies the required segment</param>
		/// <returns>true if data exists, otherwise false</returns>
		public virtual bool ContainsSegment(sbyte segmentType)
		{
			return _segmentDataMap.ContainsKey(segmentType);
		}
	}
}
