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
    /// Processes TIFF-formatted data, calling into client code via that <see cref="ITiffHandler"/> interface.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class TiffReader
    {
        /// <summary>Processes a TIFF data sequence.</summary>
        /// <param name="reader">the <see cref="IndexedReader"/> from which the data should be read</param>
        /// <param name="handler">the <see cref="ITiffHandler"/> that will coordinate processing and accept read values</param>
        /// <exception cref="TiffProcessingException">if an error occurred during the processing of TIFF data that could not be ignored or recovered from</exception>
        /// <exception cref="System.IO.IOException">an error occurred while accessing the required data</exception>
        /// <exception cref="TiffProcessingException"/>
        public static void ProcessTiff([NotNull] IndexedReader reader, [NotNull] ITiffHandler handler)
        {
            // Read byte order.
            var byteOrder = reader.GetInt16(0);
            switch (byteOrder)
            {
                case 0x4d4d: // MM
                    reader = reader.WithByteOrder(isMotorolaByteOrder: true);
                    break;
                case 0x4949: // II
                    reader = reader.WithByteOrder(isMotorolaByteOrder: false);
                    break;
                default:
                    throw new TiffProcessingException("Unclear distinction between Motorola/Intel byte ordering: " + reader.GetInt16(0));
            }

            // Check the next two values for correctness.
            int tiffMarker = reader.GetUInt16(2);
            handler.SetTiffMarker(tiffMarker);

            var firstIfdOffset = reader.GetInt32(4);

            // David Ekholm sent a digital camera image that has this problem
            // TODO calling Length should be avoided as it causes IndexedCapturingReader to read to the end of the stream
            if (firstIfdOffset >= reader.Length - 1)
            {
                handler.Warn("First IFD offset is beyond the end of the TIFF data segment -- trying default offset");
                // First directory normally starts immediately after the offset bytes, so try that
                firstIfdOffset = 2 + 2 + 4;
            }

            var processedIfdOffsets = new HashSet<int>();

            ProcessIfd(handler, reader, processedIfdOffsets, firstIfdOffset);
        }

        /// <summary>Processes a TIFF IFD.</summary>
        /// <remarks>
        /// IFD Header:
        /// <list type="bullet">
        ///   <item><b>2 bytes</b> number of tags</item>
        /// </list>
        /// Tag structure:
        /// <list type="bullet">
        ///   <item><b>2 bytes</b> tag type</item>
        ///   <item><b>2 bytes</b> format code (values 1 to 12, inclusive)</item>
        ///   <item><b>4 bytes</b> component count</item>
        ///   <item><b>4 bytes</b> inline value, or offset pointer if too large to fit in four bytes</item>
        /// </list>
        /// </remarks>
        /// <param name="handler">the <see cref="ITiffHandler"/> that will coordinate processing and accept read values</param>
        /// <param name="reader">the <see cref="IndexedReader"/> from which the data should be read</param>
        /// <param name="processedGlobalIfdOffsets">the set of visited IFD offsets, to avoid revisiting the same IFD in an endless loop</param>
        /// <param name="ifdOffset">the offset within <c>reader</c> at which the IFD data starts</param>
        /// <exception cref="System.IO.IOException">an error occurred while accessing the required data</exception>
        public static void ProcessIfd([NotNull] ITiffHandler handler, [NotNull] IndexedReader reader, [NotNull] ICollection<int> processedGlobalIfdOffsets, int ifdOffset)
        {
            try
            {
                // Check for directories we've already visited to avoid stack overflows when recursive/cyclic directory structures exist.
                // Note that we track these offsets in the global frame, not the reader's local frame.
                var globalIfdOffset = reader.ToUnshiftedOffset(ifdOffset);
                if (processedGlobalIfdOffsets.Contains(globalIfdOffset))
                    return;

                // Remember that we've visited this directory so that we don't visit it again later
                processedGlobalIfdOffsets.Add(globalIfdOffset);

                // Validate IFD offset
                if (ifdOffset >= reader.Length || ifdOffset < 0)
                {
                    handler.Error("Ignored IFD marked to start outside data segment");
                    return;
                }

                // First two bytes in the IFD are the number of tags in this directory
                int dirTagCount = reader.GetUInt16(ifdOffset);

                // Some software modifies the byte order of the file, but misses some IFDs (such as makernotes).
                // The entire test image repository doesn't contain a single IFD with more than 255 entries.
                // Here we detect switched bytes that suggest this problem, and temporarily swap the byte order.
                // This was discussed in GitHub issue #136.
                if (dirTagCount > 0xFF && (dirTagCount & 0xFF) == 0)
                {
                    dirTagCount >>= 8;
                    reader = reader.WithByteOrder(!reader.IsMotorolaByteOrder);
                }

                var dirLength = 2 + 12*dirTagCount + 4;
                if (dirLength + ifdOffset > reader.Length)
                {
                    handler.Error("Illegally sized IFD");
                    return;
                }

                //
                // Handle each tag in this directory
                //
                var invalidTiffFormatCodeCount = 0;
                for (var tagNumber = 0; tagNumber < dirTagCount; tagNumber++)
                {
                    var tagOffset = CalculateTagOffset(ifdOffset, tagNumber);

                    // 2 bytes for the tag id
                    int tagId = reader.GetUInt16(tagOffset);

                    // 2 bytes for the format code
                    var formatCode = (TiffDataFormatCode)reader.GetUInt16(tagOffset + 2);

                    // 4 bytes dictate the number of components in this tag's data
                    var componentCount = reader.GetUInt32(tagOffset + 4);

                    var format = TiffDataFormat.FromTiffFormatCode(formatCode);

                    long byteCount;
                    if (format == null)
                    {
                        if (!handler.TryCustomProcessFormat(tagId, formatCode, componentCount, out byteCount))
                        {
                            // This error suggests that we are processing at an incorrect index and will generate
                            // rubbish until we go out of bounds (which may be a while).  Exit now.
                            handler.Error($"Invalid TIFF tag format code {formatCode} for tag 0x{tagId:X4}");
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
                        byteCount = componentCount * format.ComponentSizeBytes;
                    }

                    long tagValueOffset;
                    if (byteCount > 4)
                    {
                        // If it's bigger than 4 bytes, the dir entry contains an offset.
                        tagValueOffset = reader.GetUInt32(tagOffset + 8);
                        if (tagValueOffset + byteCount > reader.Length)
                        {
                            // Bogus pointer offset and / or byteCount value
                            handler.Error("Illegal TIFF tag pointer offset");
                            continue;
                        }
                    }
                    else
                    {
                        // 4 bytes or less and value is in the dir entry itself.
                        tagValueOffset = tagOffset + 8;
                    }

                    if (tagValueOffset < 0 || tagValueOffset > reader.Length)
                    {
                        handler.Error("Illegal TIFF tag pointer offset");
                        continue;
                    }

                    // Check that this tag isn't going to allocate outside the bounds of the data array.
                    // This addresses an uncommon OutOfMemoryError.
                    if (byteCount < 0 || tagValueOffset + byteCount > reader.Length)
                    {
                        handler.Error("Illegal number of bytes for TIFF tag data: " + byteCount);
                        continue;
                    }

                    // Some tags point to one or more additional IFDs to process
                    var isIfdPointer = false;
                    if (byteCount == 4*componentCount)
                    {
                        for (var i = 0; i < componentCount; i++)
                        {
                            if (handler.TryEnterSubIfd(tagId))
                            {
                                isIfdPointer = true;
                                var subDirOffset = reader.GetUInt32((int)(tagValueOffset + i*4));
                                ProcessIfd(handler, reader, processedGlobalIfdOffsets, (int)subDirOffset);
                            }
                        }
                    }

                    // If it wasn't an IFD pointer, allow custom tag processing to occur
                    if (!isIfdPointer && !handler.CustomProcessTag((int)tagValueOffset, processedGlobalIfdOffsets, reader, tagId, (int)byteCount))
                    {
                        // If no custom processing occurred, process the tag in the standard fashion
                        ProcessTag(handler, tagId, (int)tagValueOffset, (int)componentCount, formatCode, reader);
                    }
                }

                // at the end of each IFD is an optional link to the next IFD
                var finalTagOffset = CalculateTagOffset(ifdOffset, dirTagCount);
                var nextIfdOffset = reader.GetInt32(finalTagOffset);
                if (nextIfdOffset != 0)
                {
                    if (nextIfdOffset >= reader.Length)
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
                        ProcessIfd(handler, reader, processedGlobalIfdOffsets, nextIfdOffset);
                }
            }
            finally
            {
                handler.EndingIfd();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void ProcessTag([NotNull] ITiffHandler handler, int tagId, int tagValueOffset, int componentCount, TiffDataFormatCode formatCode, [NotNull] IndexedReader reader)
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
                            array[i] = new Rational(reader.GetInt32(tagValueOffset + 8*i), reader.GetInt32(tagValueOffset + 4 + 8*i));
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
                            array[i] = new Rational(reader.GetUInt32(tagValueOffset + 8*i), reader.GetUInt32(tagValueOffset + 4 + 8*i));
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
                            array[i] = reader.GetFloat32(tagValueOffset + i*4);
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
                            array[i] = reader.GetDouble64(tagValueOffset + i*4);
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
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetSByte(tagValueOffset + i);
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
                        for (var i = 0; i < componentCount; i++)
                            array[i] = reader.GetByte(tagValueOffset + i);
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
                            array[i] = reader.GetInt16(tagValueOffset + i*2);
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
                            array[i] = reader.GetUInt16(tagValueOffset + i*2);
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
                            array[i] = reader.GetInt32(tagValueOffset + i*4);
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
                            array[i] = reader.GetUInt32(tagValueOffset + i*4);
                        handler.SetInt32UArray(tagId, array);
                    }
                    break;
                }
                default:
                {
                    handler.Error($"Invalid TIFF tag format code {formatCode} for tag 0x{tagId:X4}");
                    break;
                }
            }
        }

        /// <summary>Determine the offset of a given tag within the specified IFD.</summary>
        /// <remarks>
        /// Add 2 bytes for the tag count.
        /// Each entry is 12 bytes.
        /// </remarks>
        /// <param name="ifdStartOffset">the offset at which the IFD starts</param>
        /// <param name="entryNumber">the zero-based entry number</param>
        private static int CalculateTagOffset(int ifdStartOffset, int entryNumber) => ifdStartOffset + 2 + 12*entryNumber;
    }
}
