// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using System.Collections;
using System.Text;
using Sharpen;

namespace Com.Adobe.Xmp.Impl.Xpath
{
	/// <summary>Representates an XMP XMPPath with segment accessor methods.</summary>
	/// <since>28.02.2006</since>
	public class XMPPath
	{
		/// <summary>Marks a struct field step , also for top level nodes (schema "fields").</summary>
		public const int StructFieldStep = unchecked((int)(0x01));

		/// <summary>Marks a qualifier step.</summary>
		/// <remarks>
		/// Marks a qualifier step.
		/// Note: Order is significant to separate struct/qual from array kinds!
		/// </remarks>
		public const int QualifierStep = unchecked((int)(0x02));

		/// <summary>Marks an array index step</summary>
		public const int ArrayIndexStep = unchecked((int)(0x03));

		public const int ArrayLastStep = unchecked((int)(0x04));

		public const int QualSelectorStep = unchecked((int)(0x05));

		public const int FieldSelectorStep = unchecked((int)(0x06));

		public const int SchemaNode = unchecked((int)(0x80000000));

		public const int StepSchema = 0;

		public const int StepRootProp = 1;

		/// <summary>stores the segments of an XMPPath</summary>
		private IList segments = new ArrayList(5);

		// Bits for XPathStepInfo options.
		// 
		/// <summary>Append a path segment</summary>
		/// <param name="segment">the segment to add</param>
		public virtual void Add(XMPPathSegment segment)
		{
			segments.Add(segment);
		}

		/// <param name="index">the index of the segment to return</param>
		/// <returns>Returns a path segment.</returns>
		public virtual XMPPathSegment GetSegment(int index)
		{
			return (XMPPathSegment)segments[index];
		}

		/// <returns>Returns the size of the xmp path.</returns>
		public virtual int Size()
		{
			return segments.Count;
		}

		/// <summary>Serializes the normalized XMP-path.</summary>
		/// <seealso cref="object.ToString()"/>
		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			int index = 1;
			while (index < Size())
			{
				result.Append(GetSegment(index));
				if (index < Size() - 1)
				{
					int kind = GetSegment(index + 1).GetKind();
					if (kind == StructFieldStep || kind == QualifierStep)
					{
						// all but last and array indices
						result.Append('/');
					}
				}
				index++;
			}
			return result.ToString();
		}
	}
}
