/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
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
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// Processes TIFF-formatted data, calling into client code via that
    /// <see cref="ITiffHandler"/>
    /// interface.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class TiffReader
    {
        /// <summary>Processes a TIFF data sequence.</summary>
        /// <param name="reader">
        /// the
        /// <see cref="IndexedReader"/>
        /// from which the data should be read
        /// </param>
        /// <param name="handler">
        /// the
        /// <see cref="ITiffHandler"/>
        /// that will coordinate processing and accept read values
        /// </param>
        /// <param name="tiffHeaderOffset">the offset within <c>reader</c> at which the TIFF header starts</param>
        /// <exception cref="TiffProcessingException">
        /// if an error occurred during the processing of TIFF data that could not be
        /// ignored or recovered from
        /// </exception>
        /// <exception cref="System.IO.IOException">an error occurred while accessing the required data</exception>
        /// <exception cref="TiffProcessingException"/>
        public void ProcessTiff([NotNull] IndexedReader reader, [NotNull] ITiffHandler handler, int tiffHeaderOffset)
        {
            // This must be either "MM" or "II".
            var byteOrderIdentifier = reader.GetInt16(tiffHeaderOffset);
            if (byteOrderIdentifier == unchecked(0x4d4d))
            {
                // "MM"
                reader.IsMotorolaByteOrder = true;
            }
            else
            {
                if (byteOrderIdentifier == unchecked(0x4949))
                {
                    // "II"
                    reader.IsMotorolaByteOrder = false;
                }
                else
                {
                    throw new TiffProcessingException("Unclear distinction between Motorola/Intel byte ordering: " + byteOrderIdentifier);
                }
            }
            // Check the next two values for correctness.
            int tiffMarker = reader.GetUInt16(2 + tiffHeaderOffset);
            handler.SetTiffMarker(tiffMarker);
            var firstIfdOffset = reader.GetInt32(4 + tiffHeaderOffset) + tiffHeaderOffset;
            // David Ekholm sent a digital camera image that has this problem
            // TODO getLength should be avoided as it causes IndexedCapturingReader to read to the end of the stream
            if (firstIfdOffset >= reader.GetLength() - 1)
            {
                handler.Warn("First IFD offset is beyond the end of the TIFF data segment -- trying default offset");
                // First directory normally starts immediately after the offset bytes, so try that
                firstIfdOffset = tiffHeaderOffset + 2 + 2 + 4;
            }
            ICollection<int?> processedIfdOffsets = new HashSet<int?>();
            ProcessIfd(handler, reader, processedIfdOffsets, firstIfdOffset, tiffHeaderOffset);
            handler.Completed(reader, tiffHeaderOffset);
        }

        /// <summary>Processes a TIFF IFD.</summary>
        /// <remarks>
        /// Processes a TIFF IFD.
        /// IFD Header:
        /// <list type="bullet">
        /// <item><b>2 bytes</b> number of tags</item>
        /// </list>
        /// Tag structure:
        /// <list type="bullet">
        /// <item><b>2 bytes</b> tag type</item>
        /// <item><b>2 bytes</b> format code (values 1 to 12, inclusive)</item>
        /// <item><b>4 bytes</b> component count</item>
        /// <item><b>4 bytes</b> inline value, or offset pointer if too large to fit in four bytes</item>
        /// </list>
        /// </remarks>
        /// <param name="handler">
        /// the
        /// <see cref="ITiffHandler"/>
        /// that will coordinate processing and accept read values
        /// </param>
        /// <param name="reader">
        /// the
        /// <see cref="IndexedReader"/>
        /// from which the data should be read
        /// </param>
        /// <param name="processedIfdOffsets">the set of visited IFD offsets, to avoid revisiting the same IFD in an endless loop</param>
        /// <param name="ifdOffset">the offset within <c>reader</c> at which the IFD data starts</param>
        /// <param name="tiffHeaderOffset">the offset within <c>reader</c> at which the TIFF header starts</param>
        /// <exception cref="System.IO.IOException">an error occurred while accessing the required data</exception>
        public static void ProcessIfd([NotNull] ITiffHandler handler, [NotNull] IndexedReader reader, [NotNull] ICollection<int?> processedIfdOffsets, int ifdOffset, int tiffHeaderOffset)
        {
            try
            {
                // check for directories we've already visited to avoid stack overflows when recursive/cyclic directory structures exist
                if (processedIfdOffsets.Contains(ifdOffset))
                {
                    return;
                }
                // remember that we've visited this directory so that we don't visit it again later
                processedIfdOffsets.Add(ifdOffset);
                if (ifdOffset >= reader.GetLength() || ifdOffset < 0)
                {
                    handler.Error("Ignored IFD marked to start outside data segment");
                    return;
                }
                // First two bytes in the IFD are the number of tags in this directory
                int dirTagCount = reader.GetUInt16(ifdOffset);
                var dirLength = (2 + (12 * dirTagCount) + 4);
                if (dirLength + ifdOffset > reader.GetLength())
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
                    int formatCode = reader.GetUInt16(tagOffset + 2);
                    var format = TiffDataFormat.FromTiffFormatCode(formatCode);
                    if (format == null)
                    {
                        // This error suggests that we are processing at an incorrect index and will generate
                        // rubbish until we go out of bounds (which may be a while).  Exit now.
                        handler.Error("Invalid TIFF tag format code: " + formatCode);
                        // TODO specify threshold as a parameter, or provide some other external control over this behaviour
                        if (++invalidTiffFormatCodeCount > 5)
                        {
                            handler.Error("Stopping processing as too many errors seen in TIFF IFD");
                            return;
                        }
                        continue;
                    }
                    // 4 bytes dictate the number of components in this tag's data
                    var componentCount = reader.GetInt32(tagOffset + 4);
                    if (componentCount < 0)
                    {
                        handler.Error("Negative TIFF tag component count");
                        continue;
                    }
                    var byteCount = componentCount * format.GetComponentSizeBytes();
                    int tagValueOffset;
                    if (byteCount > 4)
                    {
                        // If it's bigger than 4 bytes, the dir entry contains an offset.
                        var offsetVal = reader.GetInt32(tagOffset + 8);
                        if (offsetVal + byteCount > reader.GetLength())
                        {
                            // Bogus pointer offset and / or byteCount value
                            handler.Error("Illegal TIFF tag pointer offset");
                            continue;
                        }
                        tagValueOffset = tiffHeaderOffset + offsetVal;
                    }
                    else
                    {
                        // 4 bytes or less and value is in the dir entry itself.
                        tagValueOffset = tagOffset + 8;
                    }
                    if (tagValueOffset < 0 || tagValueOffset > reader.GetLength())
                    {
                        handler.Error("Illegal TIFF tag pointer offset");
                        continue;
                    }
                    // Check that this tag isn't going to allocate outside the bounds of the data array.
                    // This addresses an uncommon OutOfMemoryError.
                    if (byteCount < 0 || tagValueOffset + byteCount > reader.GetLength())
                    {
                        handler.Error("Illegal number of bytes for TIFF tag data: " + byteCount);
                        continue;
                    }
                    //
                    // Special handling for tags that point to other IFDs
                    //
                    if (byteCount == 4 && handler.IsTagIfdPointer(tagId))
                    {
                        var subDirOffset = tiffHeaderOffset + reader.GetInt32(tagValueOffset);
                        ProcessIfd(handler, reader, processedIfdOffsets, subDirOffset, tiffHeaderOffset);
                    }
                    else
                    {
                        if (!handler.CustomProcessTag(tagValueOffset, processedIfdOffsets, tiffHeaderOffset, reader, tagId, byteCount))
                        {
                            ProcessTag(handler, tagId, tagValueOffset, componentCount, formatCode, reader);
                        }
                    }
                }
                // at the end of each IFD is an optional link to the next IFD
                var finalTagOffset = CalculateTagOffset(ifdOffset, dirTagCount);
                var nextIfdOffset = reader.GetInt32(finalTagOffset);
                if (nextIfdOffset != 0)
                {
                    nextIfdOffset += tiffHeaderOffset;
                    if (nextIfdOffset >= reader.GetLength())
                    {
                        // Last 4 bytes of IFD reference another IFD with an address that is out of bounds
                        // Note this could have been caused by jhead 1.3 cropping too much
                        return;
                    }
                    else
                    {
                        if (nextIfdOffset < ifdOffset)
                        {
                            // TODO is this a valid restriction?
                            // Last 4 bytes of IFD reference another IFD with an address that is before the start of this directory
                            return;
                        }
                    }
                    if (handler.HasFollowerIfd())
                    {
                        ProcessIfd(handler, reader, processedIfdOffsets, nextIfdOffset, tiffHeaderOffset);
                    }
                }
            }
            finally
            {
                handler.EndingIfd();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void ProcessTag([NotNull] ITiffHandler handler, int tagId, int tagValueOffset, int componentCount, int formatCode, [NotNull] IndexedReader reader)
        {
            switch (formatCode)
            {
                case TiffDataFormat.CodeUndefined:
                {
                    // this includes exif user comments
                    handler.SetByteArray(tagId, reader.GetBytes(tagValueOffset, componentCount));
                    break;
                }

                case TiffDataFormat.CodeString:
                {
                    handler.SetString(tagId, reader.GetNullTerminatedString(tagValueOffset, componentCount));
                    break;
                }

                case TiffDataFormat.CodeRationalS:
                {
                    if (componentCount == 1)
                    {
                        handler.SetRational(tagId, new Rational(reader.GetInt32(tagValueOffset), reader.GetInt32(tagValueOffset + 4)));
                    }
                    else
                    {
                        if (componentCount > 1)
                        {
                            var array = new Rational[componentCount];
                            for (var i = 0; i < componentCount; i++)
                            {
                                array[i] = new Rational(reader.GetInt32(tagValueOffset + (8 * i)), reader.GetInt32(tagValueOffset + 4 + (8 * i)));
                            }
                            handler.SetRationalArray(tagId, array);
                        }
                    }
                    break;
                }

                case TiffDataFormat.CodeRationalU:
                {
                    if (componentCount == 1)
                    {
                        handler.SetRational(tagId, new Rational(reader.GetUInt32(tagValueOffset), reader.GetUInt32(tagValueOffset + 4)));
                    }
                    else
                    {
                        if (componentCount > 1)
                        {
                            var array = new Rational[componentCount];
                            for (var i = 0; i < componentCount; i++)
                            {
                                array[i] = new Rational(reader.GetUInt32(tagValueOffset + (8 * i)), reader.GetUInt32(tagValueOffset + 4 + (8 * i)));
                            }
                            handler.SetRationalArray(tagId, array);
                        }
                    }
                    break;
                }

                case TiffDataFormat.CodeSingle:
                {
                    if (componentCount == 1)
                    {
                        handler.SetFloat(tagId, reader.GetFloat32(tagValueOffset));
                    }
                    else
                    {
                        var array = new float[componentCount];
                        for (var i = 0; i < componentCount; i++)
                        {
                            array[i] = reader.GetFloat32(tagValueOffset + (i * 4));
                        }
                        handler.SetFloatArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeDouble:
                {
                    if (componentCount == 1)
                    {
                        handler.SetDouble(tagId, reader.GetDouble64(tagValueOffset));
                    }
                    else
                    {
                        var array = new double[componentCount];
                        for (var i = 0; i < componentCount; i++)
                        {
                            array[i] = reader.GetDouble64(tagValueOffset + (i * 4));
                        }
                        handler.SetDoubleArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeInt8S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt8S(tagId, reader.GetInt8(tagValueOffset));
                    }
                    else
                    {
                        var array = new sbyte[componentCount];
                        for (var i = 0; i < componentCount; i++)
                        {
                            array[i] = reader.GetInt8(tagValueOffset + i);
                        }
                        handler.SetInt8SArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeInt8U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt8U(tagId, reader.GetUInt8(tagValueOffset));
                    }
                    else
                    {
                        var array = new byte[componentCount];
                        for (var i = 0; i < componentCount; i++)
                        {
                            array[i] = reader.GetUInt8(tagValueOffset + i);
                        }
                        handler.SetInt8UArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeInt16S:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt16S(tagId, reader.GetInt16(tagValueOffset));
                    }
                    else
                    {
                        var array = new short[componentCount];
                        for (var i = 0; i < componentCount; i++)
                        {
                            array[i] = reader.GetInt16(tagValueOffset + (i * 2));
                        }
                        handler.SetInt16SArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeInt16U:
                {
                    if (componentCount == 1)
                    {
                        handler.SetInt16U(tagId, reader.GetUInt16(tagValueOffset));
                    }
                    else
                    {
                        var array = new ushort[componentCount];
                        for (var i = 0; i < componentCount; i++)
                        {
                            array[i] = reader.GetUInt16(tagValueOffset + (i * 2));
                        }
                        handler.SetInt16UArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeInt32S:
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
                        {
                            array[i] = reader.GetInt32(tagValueOffset + (i * 4));
                        }
                        handler.SetInt32SArray(tagId, array);
                    }
                    break;
                }

                case TiffDataFormat.CodeInt32U:
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
                        {
                            array[i] = reader.GetUInt32(tagValueOffset + (i * 4));
                        }
                        handler.SetInt32UArray(tagId, array);
                    }
                    break;
                }

                default:
                {
                    handler.Error(string.Format("Unknown format code {0} for tag {1}", formatCode, tagId));
                    break;
                }
            }
        }

        /// <summary>Determine the offset of a given tag within the specified IFD.</summary>
        /// <param name="ifdStartOffset">the offset at which the IFD starts</param>
        /// <param name="entryNumber">the zero-based entry number</param>
        private static int CalculateTagOffset(int ifdStartOffset, int entryNumber)
        {
            // Add 2 bytes for the tag count.
            // Each entry is 12 bytes.
            return ifdStartOffset + 2 + (12 * entryNumber);
        }
    }
}
