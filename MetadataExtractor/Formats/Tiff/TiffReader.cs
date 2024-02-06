// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Runtime.InteropServices;

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// Processes TIFF-formatted data, calling into client code via that <see cref="ITiffHandler"/> interface.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class TiffReader
    {
        /// <summary>Processes a TIFF data sequence.</summary>
        /// <param name="reader">the <see cref="IndexedReader"/> from which the data should be read</param>
        /// <param name="handler">the <see cref="ITiffHandler"/> that will coordinate processing and accept read values</param>
        /// <exception cref="TiffProcessingException">if an error occurred during the processing of TIFF data that could not be ignored or recovered from</exception>
        /// <exception cref="IOException">an error occurred while accessing the required data</exception>
        /// <exception cref="TiffProcessingException"/>
        public static void ProcessTiff(IndexedReader reader, ITiffHandler handler)
        {
            // Standard TIFF
            //
            // TIFF Header:
            //   - 2 bytes: byte order (MM or II)
            //   - 2 bytes: version (always 42)
            //   - 4 bytes: offset to first IFD

            // Big TIFF
            //
            // TIFF Header:
            //   - 2 bytes: byte order (MM or II)
            //   - 2 bytes: version (always 43)
            //   - 2 bytes: byte size of offsets (always 8)
            //   - 2 bytes: reserved (always 0)
            //   - 8 bytes: offset to first IFD

            // Read byte order.
            var byteOrder = reader.GetInt16(0);

            reader = byteOrder switch
            {
                0x4d4d => reader.WithByteOrder(isMotorolaByteOrder: true),
                0x4949 => reader.WithByteOrder(isMotorolaByteOrder: false),
                _ => throw new TiffProcessingException("Unclear distinction between Motorola/Intel byte ordering: " + byteOrder),
            };

            // Check the next two values for correctness.
            var tiffMarker = reader.GetUInt16(2);
            var tiffStandard = handler.ProcessTiffMarker(tiffMarker);

            bool isBigTiff;

            int firstIfdOffset;

            switch (tiffStandard)
            {
                case TiffStandard.Tiff:
                    isBigTiff = false;
                    firstIfdOffset = checked((int)reader.GetUInt32(4));

                    // David Ekholm sent a digital camera image that has this problem
                    // TODO calling Length should be avoided as it causes IndexedCapturingReader to read to the end of the stream
                    if (firstIfdOffset >= reader.Length - 1)
                    {
                        handler.Warn("First IFD offset is beyond the end of the TIFF data segment -- trying default offset");
                        // First directory normally starts immediately after the offset bytes, so try that
                        firstIfdOffset = 2 + 2 + 4;
                    }

                    break;

                case TiffStandard.BigTiff:
                    isBigTiff = true;
                    var offsetByteSize = reader.GetInt16(4);

                    if (offsetByteSize != 8)
                    {
                        handler.Error($"Unsupported offset byte size: {offsetByteSize}");
                        return;
                    }

                    // There are two reserved bytes at offset 6, which are expected to have zero value.
                    // We skip without validation for now, but may change this in future.

                    firstIfdOffset = checked((int)reader.GetUInt64(8));
                    break;

                default:
                    handler.Error($"Unsupported TiffStandard {tiffStandard}.");
                    return;
            }

            var context = new TiffReaderContext(reader, reader.IsMotorolaByteOrder, isBigTiff);

            ProcessIfd(handler, context, firstIfdOffset);
        }

        /// <summary>
        /// Processes a TIFF IFD.
        /// </summary>
        /// <param name="handler">The <see cref="ITiffHandler"/> that will coordinate processing and accept read values.</param>
        /// <param name="context">Context for the TIFF read operation.</param>
        /// <param name="ifdOffset">The offset at which the IFD data starts.</param>
        /// <exception cref="IOException">An error occurred while accessing the required data.</exception>
        public static void ProcessIfd(ITiffHandler handler, TiffReaderContext context, int ifdOffset)
        {
            // Standard TIFF
            //
            // IFD Header:
            //   - 2 bytes: number of tags
            //
            // Tag structure:
            //   - 2 bytes: tag type
            //   - 2 bytes: format code (values 1 to 12, inclusive)
            //   - 4 bytes: component count
            //   - 4 bytes: inline value, or offset pointer if too large to fit in four bytes

            // BigTIFF
            //
            // IFD Header:
            //   - 8 bytes: number of tags
            //
            // Tag structure:
            //   - 2 bytes: tag type
            //   - 2 bytes: format code (values 1 to 12, inclusive)
            //   - 8 bytes: component count
            //   - 8 bytes: inline value, or offset pointer if too large to fit in eight bytes

            try
            {
                // Check for directories we've already visited to avoid stack overflows when recursive/cyclic directory structures exist.
                if (!context.TryVisitIfd(ifdOffset, handler.Kind))
                    return;

                // Validate IFD offset
                if (ifdOffset >= context.Reader.Length || ifdOffset < 0)
                {
                    handler.Error("Ignored IFD marked to start outside data segment");
                    return;
                }

                // The number of tags in this directory
                var dirTagCount = context.IsBigTiff
                    ? checked((int)context.Reader.GetUInt64(ifdOffset))
                    : context.Reader.GetUInt16(ifdOffset);

                // Some software modifies the byte order of the file, but misses some IFDs (such as makernotes).
                // The entire test image repository doesn't contain a single IFD with more than 255 entries.
                // Here we detect switched bytes that suggest this problem, and temporarily swap the byte order.
                // This was discussed in GitHub issue #136.
                if (!context.IsBigTiff && dirTagCount > 0xFF && (dirTagCount & 0xFF) == 0)
                {
                    dirTagCount >>= 8;
                    context = context.WithByteOrder(!context.Reader.IsMotorolaByteOrder);
                }

                var dirLength = context.IsBigTiff
                    ? 8 + 20 * dirTagCount + 8
                    : 2 + 12 * dirTagCount + 4;

                if (dirLength + ifdOffset > checked((int)context.Reader.Length))
                {
                    handler.Error("Illegally sized IFD");
                    return;
                }

                var inlineValueSize = context.IsBigTiff ? 8u : 4u;

                //
                // Handle each tag in this directory
                //
                var invalidTiffFormatCodeCount = 0;
                for (var tagNumber = 0; tagNumber < dirTagCount; tagNumber++)
                {
                    var tagOffset = CalculateTagOffset(ifdOffset, tagNumber, context.IsBigTiff);

                    int tagId = context.Reader.GetUInt16(tagOffset);

                    var formatCode = (TiffDataFormatCode)context.Reader.GetUInt16(tagOffset + 2);

                    var componentCount = context.IsBigTiff
                        ? context.Reader.GetUInt64(tagOffset + 4)
                        : context.Reader.GetUInt32(tagOffset + 4);

                    var format = TiffDataFormat.FromTiffFormatCode(formatCode, context.IsBigTiff);

                    ulong byteCount;
                    if (format is null)
                    {
                        if (!handler.TryCustomProcessFormat(tagId, formatCode, componentCount, out byteCount))
                        {
                            // This error suggests that we are processing at an incorrect index and will generate
                            // rubbish until we go out of bounds (which may be a while).  Exit now.
                            handler.Error($"Invalid TIFF tag format code {(int)formatCode} for tag 0x{tagId:X4}");
                            // TODO specify threshold as a parameter, or provide some other external control over this behaviour
                            if (++invalidTiffFormatCodeCount > 5)
                            {
                                handler.Error("Stopping processing as too many errors seen in TIFF IFD");
                                return;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        byteCount = checked(componentCount * format.ComponentSizeBytes);
                    }

                    uint tagValueOffset;
                    if (byteCount > inlineValueSize)
                    {
                        // Value(s) are too big to fit inline. Follow the pointer.
                        tagValueOffset = context.IsBigTiff
                            ? checked((uint)context.Reader.GetUInt64(tagOffset + 12))
                            : context.Reader.GetUInt32(tagOffset + 8);

                        if (tagValueOffset + byteCount > checked((ulong)context.Reader.Length))
                        {
                            // Bogus pointer offset and/or byteCount value
                            handler.Error("Illegal TIFF tag pointer offset");
                            continue;
                        }
                    }
                    else
                    {
                        // Value(s) can fit inline.
                        tagValueOffset = context.IsBigTiff
                            ? checked((uint)tagOffset + 12)
                            : checked((uint)tagOffset + 8);
                    }

                    if (tagValueOffset > context.Reader.Length)
                    {
                        handler.Error("Illegal TIFF tag pointer offset");
                        continue;
                    }

                    // Check that this tag isn't going to allocate outside the bounds of the data array.
                    // This addresses an uncommon OutOfMemoryError.
                    if (tagValueOffset + byteCount > checked((ulong)context.Reader.Length))
                    {
                        handler.Error("Illegal number of bytes for TIFF tag data: " + byteCount);
                        continue;
                    }

                    // Some tags point to one or more additional IFDs to process
                    var isIfdPointer = false;
                    if (byteCount == checked(4L * componentCount) || formatCode == TiffDataFormatCode.Ifd8)
                    {
                        for (ulong i = 0; i < componentCount; i++)
                        {
                            if (handler.TryEnterSubIfd(tagId))
                            {
                                isIfdPointer = true;
                                var subDirOffset = context.Reader.GetUInt32(checked((int)(tagValueOffset + i * 4)));
                                ProcessIfd(handler, context, (int)subDirOffset);
                            }
                        }
                    }

                    // If it wasn't an IFD pointer, allow custom tag processing to occur
                    if (!isIfdPointer && !handler.CustomProcessTag(context, tagId, (int)tagValueOffset, (int)byteCount))
                    {
                        // If no custom processing occurred, process the tag in the standard fashion
                        ProcessTag(handler, tagId, (int)tagValueOffset, (int)componentCount, formatCode, context.Reader);
                    }
                }

                // at the end of each IFD is an optional link to the next IFD
                var finalTagOffset = CalculateTagOffset(ifdOffset, dirTagCount, context.IsBigTiff);

                var nextIfdOffsetLong = context.IsBigTiff
                    ? context.Reader.GetUInt64(finalTagOffset)
                    : context.Reader.GetUInt32(finalTagOffset);

                if (nextIfdOffsetLong != 0 && nextIfdOffsetLong <= int.MaxValue)
                {
                    var nextIfdOffset = (int)nextIfdOffsetLong;

                    if (nextIfdOffset >= context.Reader.Length)
                    {
                        // Last 4 bytes of IFD reference another IFD with an address that is out of bounds
                        return;
                    }
                    else if (nextIfdOffset < ifdOffset)
                    {
                        // TODO is this a valid restriction?
                        // Last 4 bytes of IFD reference another IFD with an address that is before the start of this directory
                        return;
                    }

                    if (handler.HasFollowerIfd())
                    {
                        ProcessIfd(handler, context, nextIfdOffset);
                    }
                }
            }
            finally
            {
                handler.EndingIfd(in context);
            }
        }

        /// <exception cref="IOException"/>
        private static void ProcessTag(ITiffHandler handler, int tagId, int tagValueOffset, int componentCount, TiffDataFormatCode formatCode, IndexedReader reader)
        {
            switch (formatCode)
            {
                case TiffDataFormatCode.Undefined:
                {
                    // this includes exif user comments
                    handler.SetByteArray(tagId, reader.GetBytes(tagValueOffset, componentCount));
                    break;
                }
                case TiffDataFormatCode.String:
                {
                    handler.SetString(tagId, reader.GetNullTerminatedStringValue(tagValueOffset, componentCount));
                    break;
                }
                case TiffDataFormatCode.RationalS:
                {
                    if (componentCount == 1)
                    {
                        handler.SetRational(tagId, new Rational(reader.GetInt32(tagValueOffset), reader.GetInt32(tagValueOffset + 4)));
                    }
                    else if (componentCount > 1)
                    {
                        var array = new Rational[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = new Rational(reader.GetInt32(tagValueOffset + 8 * i), reader.GetInt32(tagValueOffset + 4 + 8 * i));
                        handler.SetRationalArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.RationalU:
                {
                    if (componentCount == 1)
                    {
                        handler.SetRational(tagId, new Rational(reader.GetUInt32(tagValueOffset), reader.GetUInt32(tagValueOffset + 4)));
                    }
                    else if (componentCount > 1)
                    {
                        var array = new Rational[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = new Rational(reader.GetUInt32(tagValueOffset + 8 * i), reader.GetUInt32(tagValueOffset + 4 + 8 * i));
                        handler.SetRationalArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Single:
                {
                    if (componentCount == 1)
                    {
                        handler.SetFloat(tagId, reader.GetFloat32(tagValueOffset));
                    }
                    else
                    {
                        var array = new float[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetFloat32(tagValueOffset + i * 4);
                        handler.SetFloatArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Double:
                {
                    if (componentCount == 1)
                    {
                        handler.SetDouble(tagId, reader.GetDouble64(tagValueOffset));
                    }
                    else
                    {
                        var array = new double[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetDouble64(tagValueOffset + i * 8);
                        handler.SetDoubleArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int8S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt8S(tagId, reader.GetSByte(tagValueOffset));
                    }
                    else
                    {
                        var array = new sbyte[componentCount];
                        var bytes = MemoryMarshal.Cast<sbyte, byte>(array);
                        reader.GetBytes(tagValueOffset, bytes);
                        handler.SetInt8SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int8U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt8U(tagId, reader.GetByte(tagValueOffset));
                    }
                    else
                    {
                        var array = new byte[componentCount];
                        reader.GetBytes(tagValueOffset, array);
                        handler.SetInt8UArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int16S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt16S(tagId, reader.GetInt16(tagValueOffset));
                    }
                    else
                    {
                        var array = new short[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetInt16(tagValueOffset + i * 2);
                        handler.SetInt16SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int16U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt16U(tagId, reader.GetUInt16(tagValueOffset));
                    }
                    else
                    {
                        var array = new ushort[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetUInt16(tagValueOffset + i * 2);
                        handler.SetInt16UArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int32S:
                {
                    // NOTE 'long' in this case means 32 bit, not 64
                    if (componentCount == 1)
                    {
                        handler.SetInt32S(tagId, reader.GetInt32(tagValueOffset));
                    }
                    else
                    {
                        var array = new int[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetInt32(tagValueOffset + i * 4);
                        handler.SetInt32SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int32U:
                {
                    // NOTE 'long' in this case means 32 bit, not 64
                    if (componentCount == 1)
                    {
                        handler.SetInt32U(tagId, reader.GetUInt32(tagValueOffset));
                    }
                    else
                    {
                        var array = new uint[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetUInt32(tagValueOffset + i * 4);
                        handler.SetInt32UArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int64S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt64S(tagId, reader.GetInt64(tagValueOffset));
                    }
                    else
                    {
                        var array = new long[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetInt64(tagValueOffset + i * 8);
                        handler.SetInt64SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int64U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt64U(tagId, reader.GetUInt64(tagValueOffset));
                    }
                    else
                    {
                        var array = new ulong[componentCount];
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetUInt64(tagValueOffset + i * 8);
                        handler.SetInt64UArray(tagId, array);
                    }
                    break;
                }
                default:
                {
                    handler.Error($"Invalid TIFF tag format code {(int)formatCode} for tag 0x{tagId:X4}");
                    break;
                }
            }
        }

        /// <summary>Determine the offset of a given tag within the specified IFD.</summary>
        /// <remarks>
        /// Add 2 bytes for the tag count.
        /// Each entry is 12 bytes for regular TIFF, or 20 bytes for BigTIFF.
        /// </remarks>
        /// <param name="ifdStartOffset">The offset at which the IFD starts.</param>
        /// <param name="entryNumber">The zero-based entry number.</param>
        /// <param name="isBigTiff">Whether we are using BigTIFF encoding.</param>
        private static int CalculateTagOffset(int ifdStartOffset, int entryNumber, bool isBigTiff)
        {
            return !isBigTiff
                ? ifdStartOffset + 2 + 12 * entryNumber
                : ifdStartOffset + 8 + 20 * entryNumber;
        }
    }
}
