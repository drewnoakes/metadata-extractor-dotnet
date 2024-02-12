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
                _ => throw new TiffProcessingException("Unclear distinction between Motorola/Intel byte ordering: " + byteOrder)
            };

            // Check the next two values for correctness.
            var tiffMarker = reader.GetUInt16(2);
            TiffStandard tiffStandard = handler.ProcessTiffMarker(tiffMarker);

            bool? isBigTiff = tiffStandard switch
            {
                TiffStandard.Tiff => false,
                TiffStandard.BigTiff => true,
                _ => null
            };

            if (isBigTiff is null)
            {
                handler.Error($"Unsupported TiffStandard {tiffStandard}.");
                return;
            }

            int firstIfdOffset;

            if (!isBigTiff.Value)
            {
                firstIfdOffset = checked((int)reader.GetUInt32(4));

                // David Ekholm sent a digital camera image that has this problem
                // TODO calling Length should be avoided as it causes IndexedCapturingReader to read to the end of the stream -- add reader.TryValidatePosition(int offset) and have implementations return true if they cannot quickly determine the answer, or just buffer until that position
                if (firstIfdOffset >= reader.Length - 1)
                {
                    handler.Warn("First IFD offset is beyond the end of the TIFF data segment -- trying default offset");
                    // First directory normally starts immediately after the offset bytes, so try that
                    firstIfdOffset = 2 + 2 + 4;
                }
            }
            else
            {
                var offsetByteSize = reader.GetInt16(4);

                if (offsetByteSize != 8)
                {
                    handler.Error($"Unsupported offset byte size: {offsetByteSize}");
                    return;
                }

                // There are two reserved bytes at offset 6, which are expected to have zero value.
                // We skip without validation for now, but may change this in future.

                firstIfdOffset = checked((int)reader.GetUInt64(8));
            }

            var context = new TiffReaderContext(reader, reader.IsMotorolaByteOrder, isBigTiff.Value);

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

                // The number of tags in this directory, and various sizes (determined by BigTIFF).
#pragma warning disable format
                (int tagCount, int tagCountLength, int entryLength, int followerPointerLength, int entryValueOffset, uint inlineValueLength) = context.IsBigTiff
                    ? (checked((int)context.Reader.GetUInt64(ifdOffset)), ByteCounts.TagCount_BigTiff, ByteCounts.Entry_BigTiff, ByteCounts.FollowerIfdPointer_BiffTiff, ByteCounts.EntryValueOffset_BigTiff, 8u)
                    : (context.Reader.GetUInt16(ifdOffset),               ByteCounts.TagCount,         ByteCounts.Entry,         ByteCounts.FollowerIfdPointer,          ByteCounts.EntryValueOffset,         4u);
#pragma warning restore format

                // Some software modifies the byte order of the file, but misses some IFDs (such as makernotes).
                // The entire test image repository doesn't contain a single IFD with more than 255 entries.
                // Here we detect switched bytes that suggest this problem, and temporarily swap the byte order.
                // This was discussed in GitHub issue #136.
                if (!context.IsBigTiff && tagCount > 0xFF && (tagCount & 0xFF) == 0)
                {
                    tagCount >>= 8;
                    context = context.WithByteOrder(!context.Reader.IsMotorolaByteOrder);
                }

                // The IFD starts after the tag count.
                var tagTableOffset = ifdOffset + tagCountLength;

                // The IFD table is stored using fixed-size elements.
                // When a value is larger than the allotted space, a pointer is stored.
                //
                // Per tag: 12 bytes / BigTIFF 20 bytes (see above docs for breakdown).
                // Finally, a pointer to a "follower" IFD (optional)
                var tagTableLength = (tagCount * entryLength);

                if (tagTableOffset + tagTableLength > checked((int)context.Reader.Length))
                {
                    handler.Error("Illegally sized IFD");
                    return;
                }

                // TODO better approach here? stack overflow? if backed by array, just want to slice without copy? other stuff? what about longer tables?
                Span<byte> tagTableBytes = stackalloc byte[tagTableLength];
                context.Reader.GetBytes(tagTableOffset, tagTableBytes);
                BufferReader reader = new(tagTableBytes, isBigEndian: context.Reader.IsMotorolaByteOrder);

                // We will track how many invalid formats we see, and stop processing when we meet a threshold.
                var invalidTiffFormatCodeCount = 0;
                const int InvalidTiffFormatCodeCountThreshold = 5;

                //
                // Handle each tag in this directory
                //
                for (var tagNumber = 0; tagNumber < tagCount; tagNumber++)
                {
                    Debug.Assert(reader.Position % entryLength == 0, "Misaligned read of IFD entry");

                    // Tag identifier (2 bytes)
                    ushort tagId = reader.GetUInt16();

                    // Format code (2 bytes)
                    var formatCode = (TiffDataFormatCode)reader.GetUInt16();

                    // Number of components (4 bytes / BigTIFF 8 bytes)
                    ulong componentCount = context.IsBigTiff
                        ? reader.GetUInt64()
                        : reader.GetUInt32();

                    var format = TiffDataFormat.FromTiffFormatCode(formatCode, context.IsBigTiff);

                    ulong valueLength;
                    if (format is { ComponentSizeBytes: byte componentSize })
                    {
                        valueLength = checked(componentCount * componentSize);
                    }
                    else if (!handler.TryCustomProcessFormat(tagId, formatCode, componentCount, out valueLength))
                    {
                        // This error suggests that we are processing at an incorrect index and will generate
                        // rubbish until we go out of bounds (which may be a while).  Exit now.
                        handler.Error($"Invalid TIFF tag format code {(int)formatCode} for tag 0x{tagId:X4}");

                        // TODO specify threshold as a parameter, or provide some other external control over this behaviour
                        if (++invalidTiffFormatCodeCount > InvalidTiffFormatCodeCountThreshold)
                        {
                            handler.Error("Stopping processing as too many errors seen in TIFF IFD");
                            return;
                        }

                        reader.Skip(checked((int)inlineValueLength));
                        continue;
                    }

                    uint valueOffset;
                    if (valueLength > inlineValueLength)
                    {
                        // Value(s) are too big to fit inline. Follow the pointer.
                        valueOffset = context.IsBigTiff
                            ? checked((uint)reader.GetUInt64())
                            : reader.GetUInt32();

                        if (valueOffset + valueLength > checked((ulong)context.Reader.Length))
                        {
                            // Bogus pointer offset and/or byteCount value
                            handler.Error("Illegal TIFF tag pointer offset");
                            continue;
                        }
                    }
                    else
                    {
                        // Value(s) can fit inline.
                        int tagOffset = CalculateTagOffset(tagNumber);
                        valueOffset = checked((uint)(tagOffset + entryValueOffset));
                        reader.Skip(checked((int)inlineValueLength));
                    }

                    Debug.Assert(checked(valueOffset + valueLength) <= checked((ulong)context.Reader.Length));

                    bool isIfdPointer = false;

                    // TODO is the following IFD8 check correct? IFD8 is 8-bytes in length, and the following assumed 4-bytes.
                    const ulong IfdPointerLength = 4;

                    // Some tags point to one or more additional IFDs to process
                    if (valueLength == checked(componentCount * IfdPointerLength) || formatCode == TiffDataFormatCode.Ifd8)
                    {
                        // There may be multiple IFD pointers, so we try to enter and process an IFD for each.
                        // They will all have the same TagId, and therefore it's likely they'll be the same
                        // kind of directory.
                        for (ulong i = 0; i < componentCount; i++)
                        {
                            if (handler.TryEnterSubIfd(tagId))
                            {
                                isIfdPointer = true;

                                int subDirOffsetOffset = checked((int)(valueOffset + (i * IfdPointerLength)));

                                uint subDirOffset = context.Reader.GetUInt32(subDirOffsetOffset);

                                ProcessIfd(handler, context, (int)subDirOffset);
                            }
                            else
                            {
                                // There's no point trying to enter the same tag ID again.
                                break;
                            }
                        }
                    }

                    // If it wasn't an IFD pointer, allow custom tag processing to occur
                    if (!isIfdPointer && !handler.CustomProcessTag(context, tagId, (int)valueOffset, (int)valueLength))
                    {
                        // If no custom processing occurred, process the tag in the standard fashion
                        ProcessTag(handler, tagId, (int)valueOffset, (int)componentCount, formatCode, context.Reader);
                    }
                }

                // at the end of each IFD is an optional link to the next IFD
                int finalTagOffset = CalculateTagOffset(tagIndex: tagCount);

                if ((long)finalTagOffset + followerPointerLength <= context.Reader.Length)
                {
                    ulong nextIfdOffsetLong = context.IsBigTiff
                        ? context.Reader.GetUInt64(finalTagOffset)
                        : context.Reader.GetUInt32(finalTagOffset);

                    if (nextIfdOffsetLong != 0 && nextIfdOffsetLong <= int.MaxValue)
                    {
                        int nextIfdOffset = (int)nextIfdOffsetLong;

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

                return;

                int CalculateTagOffset(int tagIndex)
                {
                    return checked(ifdOffset + tagCountLength + (entryLength * tagIndex));
                }
            }
            finally
            {
                handler.EndingIfd(in context);
            }
        }

        /// <exception cref="IOException"/>
        private static void ProcessTag(ITiffHandler handler, int tagId, in int valueOffset, int componentCount, TiffDataFormatCode formatCode, IndexedReader reader)
        {
            switch (formatCode)
            {
                case TiffDataFormatCode.Undefined:
                {
                    // this includes exif user comments
                    handler.SetByteArray(tagId, reader.GetBytes(valueOffset, componentCount));
                    break;
                }
                case TiffDataFormatCode.String:
                {
                    handler.SetString(tagId, reader.GetNullTerminatedStringValue(valueOffset, componentCount));
                    break;
                }
                case TiffDataFormatCode.RationalS:
                {
                    if (componentCount == 1)
                    {
                        handler.SetRational(tagId, new Rational(reader.GetInt32(valueOffset), reader.GetInt32(valueOffset + 4)));
                    }
                    else if (componentCount > 1)
                    {
                        var array = new Rational[componentCount];
                        for (int i = 0, componentValueOffset = valueOffset; i < componentCount; i++, componentValueOffset += 8)
                            array[i] = new Rational(reader.GetInt32(componentValueOffset), reader.GetInt32(componentValueOffset + 4));
                        handler.SetRationalArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.RationalU:
                {
                    if (componentCount == 1)
                    {
                        handler.SetRational(tagId, new Rational(reader.GetUInt32(valueOffset), reader.GetUInt32(valueOffset + 4)));
                    }
                    else if (componentCount > 1)
                    {
                        var array = new Rational[componentCount];
                        for (int i = 0, componentValueOffset = valueOffset; i < componentCount; i++, componentValueOffset += 8)
                            array[i] = new Rational(reader.GetUInt32(componentValueOffset), reader.GetUInt32(componentValueOffset + 4));
                        handler.SetRationalArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Single:
                {
                    if (componentCount == 1)
                    {
                        handler.SetFloat(tagId, reader.GetFloat32(valueOffset));
                    }
                    else
                    {
                        var array = new float[componentCount];
                        for (int i = 0; i < componentCount; i++)
                            array[i] = reader.GetFloat32(valueOffset + i * 4);
                        handler.SetFloatArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Double:
                {
                    if (componentCount == 1)
                    {
                        handler.SetDouble(tagId, reader.GetDouble64(valueOffset));
                    }
                    else
                    {
                        var array = new double[componentCount];
                        for (int i = 0; i < componentCount; i++)
                            array[i] = reader.GetDouble64(valueOffset + i * 8);
                        handler.SetDoubleArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int8S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt8S(tagId, reader.GetSByte(valueOffset));
                    }
                    else
                    {
                        var array = new sbyte[componentCount];
                        var bytes = MemoryMarshal.Cast<sbyte, byte>(array);
                        reader.GetBytes(valueOffset, bytes);
                        handler.SetInt8SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int8U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt8U(tagId, reader.GetByte(valueOffset));
                    }
                    else
                    {
                        var array = new byte[componentCount];
                        reader.GetBytes(valueOffset, array);
                        handler.SetInt8UArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int16S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt16S(tagId, reader.GetInt16(valueOffset));
                    }
                    else
                    {
                        var array = new short[componentCount];
                        for (int i = 0, componentOffset = valueOffset; i < componentCount; i++, componentOffset += 2)
                            array[i] = reader.GetInt16(componentOffset);
                        handler.SetInt16SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int16U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt16U(tagId, reader.GetUInt16(valueOffset));
                    }
                    else
                    {
                        var array = new ushort[componentCount];
                        for (int i = 0, componentOffset = valueOffset; i < componentCount; i++, componentOffset += 2)
                            array[i] = reader.GetUInt16(componentOffset);
                        handler.SetInt16UArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int32S:
                {
                    // NOTE 'long' in this case means 32 bit, not 64
                    if (componentCount == 1)
                    {
                        handler.SetInt32S(tagId, reader.GetInt32(valueOffset));
                    }
                    else
                    {
                        var array = new int[componentCount];
                        for (int i = 0, componentOffset = valueOffset; i < componentCount; i++, componentOffset += 4)
                            array[i] = reader.GetInt32(componentOffset);
                        handler.SetInt32SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int32U:
                {
                    // NOTE 'long' in this case means 32 bit, not 64
                    if (componentCount == 1)
                    {
                        handler.SetInt32U(tagId, reader.GetUInt32(valueOffset));
                    }
                    else
                    {
                        var array = new uint[componentCount];
                        for (int i = 0, componentOffset = valueOffset; i < componentCount; i++, componentOffset += 4)
                            array[i] = reader.GetUInt32(componentOffset);
                        handler.SetInt32UArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int64S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt64S(tagId, reader.GetInt64(valueOffset));
                    }
                    else
                    {
                        var array = new long[componentCount];
                        for (int i = 0, componentOffset = valueOffset; i < componentCount; i++, componentOffset += 8)
                            array[i] = reader.GetInt64(componentOffset);
                        handler.SetInt64SArray(tagId, array);
                    }
                    break;
                }
                case TiffDataFormatCode.Int64U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt64U(tagId, reader.GetUInt64(valueOffset));
                    }
                    else
                    {
                        var array = new ulong[componentCount];
                        for (int i = 0, componentOffset = valueOffset; i < componentCount; i++, componentOffset += 8)
                            array[i] = reader.GetUInt64(componentOffset);
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

        private static class ByteCounts
        {
            public const int TagCount = 2;
            public const int TagCount_BigTiff = 8;

            public const int TagId = 2;
            public const int FormatCode = 2;

            public const int ComponentCount = 4;
            public const int ComponentCount_BigTiff = 8;

            public const int Value = 4;
            public const int Value_BigTiff = 8;

            public const int EntryValueOffset = TagId + FormatCode + ComponentCount;
            public const int EntryValueOffset_BigTiff = TagId + FormatCode + ComponentCount_BigTiff;

            public const int Entry = TagId + FormatCode + ComponentCount + Value;
            public const int Entry_BigTiff = TagId + FormatCode + ComponentCount_BigTiff + Value_BigTiff;

            public const int FollowerIfdPointer = 4;
            public const int FollowerIfdPointer_BiffTiff = 8;
        }
    }
}
