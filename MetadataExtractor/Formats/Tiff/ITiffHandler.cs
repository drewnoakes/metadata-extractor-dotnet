#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Collections.Generic;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// Interface of an class capable of handling events raised during the reading of a TIFF file
    /// via <see cref="TiffReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public interface ITiffHandler
    {
        /// <summary>Receives the 2-byte marker found in the TIFF header.</summary>
        /// <remarks>
        /// Receives the 2-byte marker found in the TIFF header.
        /// <para />
        /// Implementations are not obligated to use this information for any purpose, though it may be useful for
        /// validation or perhaps differentiating the type of mapping to use for observed tags and IFDs.
        /// </remarks>
        /// <param name="marker">the 2-byte value found at position 2 of the TIFF header</param>
        /// <exception cref="TiffProcessingException"/>
        void SetTiffMarker(int marker);

        bool TryEnterSubIfd(int tagType);

        bool HasFollowerIfd();

        void EndingIfd();

        /// <exception cref="System.IO.IOException"/>
        bool CustomProcessTag(int tagOffset, [NotNull] ICollection<int> processedIfdOffsets, [NotNull] IndexedReader reader, int tagId, int byteCount);

        bool TryCustomProcessFormat(int tagId, TiffDataFormatCode formatCode, uint componentCount, out long byteCount);

        void Warn([NotNull] string message);

        void Error([NotNull] string message);

        void SetByteArray(int tagId, [NotNull] byte[] bytes);

        void SetString(int tagId, StringValue str);

        void SetRational(int tagId, Rational rational);

        void SetRationalArray(int tagId, [NotNull] Rational[] array);

        void SetFloat(int tagId, float float32);

        void SetFloatArray(int tagId, [NotNull] float[] array);

        void SetDouble(int tagId, double double64);

        void SetDoubleArray(int tagId, [NotNull] double[] array);

        void SetInt8S(int tagId, sbyte int8S);

        void SetInt8SArray(int tagId, [NotNull] sbyte[] array);

        void SetInt8U(int tagId, byte int8U);

        void SetInt8UArray(int tagId, [NotNull] byte[] array);

        void SetInt16S(int tagId, short int16S);

        void SetInt16SArray(int tagId, [NotNull] short[] array);

        void SetInt16U(int tagId, ushort int16U);

        void SetInt16UArray(int tagId, [NotNull] ushort[] array);

        void SetInt32S(int tagId, int int32S);

        void SetInt32SArray(int tagId, [NotNull] int[] array);

        void SetInt32U(int tagId, uint int32U);

        void SetInt32UArray(int tagId, [NotNull] uint[] array);
    }
}
