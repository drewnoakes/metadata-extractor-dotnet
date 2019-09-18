// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
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
        bool CustomProcessTag(int tagOffset, ICollection<int> processedIfdOffsets, IndexedReader reader, int tagId, int byteCount);

        bool TryCustomProcessFormat(int tagId, TiffDataFormatCode formatCode, uint componentCount, out long byteCount);

        void Warn(string message);

        void Error(string message);

        void SetByteArray(int tagId, byte[] bytes);

        void SetString(int tagId, StringValue str);

        void SetRational(int tagId, Rational rational);

        void SetRationalArray(int tagId, Rational[] array);

        void SetFloat(int tagId, float float32);

        void SetFloatArray(int tagId, float[] array);

        void SetDouble(int tagId, double double64);

        void SetDoubleArray(int tagId, double[] array);

        void SetInt8S(int tagId, sbyte int8S);

        void SetInt8SArray(int tagId, sbyte[] array);

        void SetInt8U(int tagId, byte int8U);

        void SetInt8UArray(int tagId, byte[] array);

        void SetInt16S(int tagId, short int16S);

        void SetInt16SArray(int tagId, short[] array);

        void SetInt16U(int tagId, ushort int16U);

        void SetInt16UArray(int tagId, ushort[] array);

        void SetInt32S(int tagId, int int32S);

        void SetInt32SArray(int tagId, int[] array);

        void SetInt32U(int tagId, uint int32U);

        void SetInt32UArray(int tagId, uint[] array);
    }
}
