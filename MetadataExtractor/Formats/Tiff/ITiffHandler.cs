// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// Interface of an class capable of handling events raised during the reading of a TIFF file
    /// via <see cref="TiffReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public interface ITiffHandler
    {
        /// <summary>
        /// Receives the 2-byte marker found in the TIFF header.
        /// </summary>
        /// <remarks>
        /// Implementations are not obligated to use this information for any purpose, though it may be useful for
        /// validation or perhaps differentiating the type of mapping to use for observed tags and IFDs.
        /// </remarks>
        /// <param name="marker">The 2-byte value found at position 2 of the TIFF header.</param>
        /// <returns>The TIFF standard via which to interpret the data stream.</returns>
        /// <exception cref="TiffProcessingException">The value of <paramref name="marker"/> is not supported.</exception>
        TiffStandard ProcessTiffMarker(ushort marker);

        /// <summary>
        /// Gets an object that represents the kind of IFD being processed currently.
        /// </summary>
        /// <remarks>
        /// Used, along with the data offset, to prevent processing a single IFD more than once.
        /// We used to only use the IFD's offset, but we have observed a file where the same
        /// offset was used more than once in different contexts, and where that directory
        /// contained tags appropriate to both contexts. By including this "kind" when considering
        /// duplicate IFDs, we can process the same data in multiple contexts and still prevent
        /// against endless loops.
        /// </remarks>
        /// <seealso cref="IfdIdentity" />
        object? Kind { get; }

        bool TryEnterSubIfd(int tagType);

        bool HasFollowerIfd();

        void EndingIfd(in TiffReaderContext context);

        /// <summary>
        /// Allows handlers to provide custom logic for a given tag.
        /// </summary>
        /// <param name="context">Context for the TIFF read operation.</param>
        /// <param name="tagId">The ID of the tag being processed.</param>
        /// <param name="valueOffset">The offset into the data stream at which the tag's value starts.</param>
        /// <param name="byteCount">The number of bytes that the tag's value spans.</param>
        /// <returns><see langword="true"/> if processing was successful and default processing should be suppressed, otherwise <see langword="false"/>.</returns>
        /// <exception cref="IOException"/>
        bool CustomProcessTag(in TiffReaderContext context, int tagId, int valueOffset, int byteCount);

        bool TryCustomProcessFormat(int tagId, TiffDataFormatCode formatCode, ulong componentCount, out ulong byteCount);

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

        void SetInt64S(int tagId, long int64S);

        void SetInt64SArray(int tagId, long[] array);

        void SetInt64U(int tagId, ulong int64U);

        void SetInt64UArray(int tagId, ulong[] array);
    }
}
