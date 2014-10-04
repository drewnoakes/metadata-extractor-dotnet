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
using System.Collections.Generic;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using Sharpen;

namespace Com.Drew.Imaging.Tiff
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public interface TiffHandler
	{
		/// <summary>Receives the 2-byte marker found in the TIFF header.</summary>
		/// <remarks>
		/// Receives the 2-byte marker found in the TIFF header.
		/// <p/>
		/// Implementations are not obligated to use this information for any purpose, though it may be useful for
		/// validation or perhaps differentiating the type of mapping to use for observed tags and IFDs.
		/// </remarks>
		/// <param name="marker">the 2-byte value found at position 2 of the TIFF header</param>
		/// <exception cref="Com.Drew.Imaging.Tiff.TiffProcessingException"/>
		void SetTiffMarker(int marker);

		bool IsTagIfdPointer(int tagType);

		bool HasFollowerIfd();

		void EndingIFD();

		void Completed(RandomAccessReader reader, int tiffHeaderOffset);

		/// <exception cref="System.IO.IOException"/>
		bool CustomProcessTag(int makernoteOffset, ICollection<int> processedIfdOffsets, int tiffHeaderOffset, RandomAccessReader reader, int tagId, int byteCount);

		void Warn(string message);

		void Error(string message);

		void SetByteArray(int tagId, sbyte[] bytes);

		void SetString(int tagId, string @string);

		void SetRational(int tagId, Rational rational);

		void SetRationalArray(int tagId, Rational[] array);

		void SetFloat(int tagId, float float32);

		void SetFloatArray(int tagId, float[] array);

		void SetDouble(int tagId, double double64);

		void SetDoubleArray(int tagId, double[] array);

		void SetInt8s(int tagId, sbyte int8s);

		void SetInt8sArray(int tagId, sbyte[] array);

		void SetInt8u(int tagId, short int8u);

		void SetInt8uArray(int tagId, short[] array);

		void SetInt16s(int tagId, int int16s);

		void SetInt16sArray(int tagId, short[] array);

		void SetInt16u(int tagId, int int16u);

		void SetInt16uArray(int tagId, int[] array);

		void SetInt32s(int tagId, int int32s);

		void SetInt32sArray(int tagId, int[] array);

		void SetInt32u(int tagId, long int32u);

		void SetInt32uArray(int tagId, long[] array);
	}
}
