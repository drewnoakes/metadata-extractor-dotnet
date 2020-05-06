// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.IO
{
    /// <summary>Reads and buffers data in chunks and provides methods for reading data types</summary>
    /// <remarks>
    /// This class implements buffered reading of data typically for use with <see cref="ReaderInfo"/> 
    /// objects and provides methods for reading data types from it. Data is captured in configurable 
    /// chunks for efficiency. Both seekable and non-seekable streams are supported.
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RandomAccessStream
    {
        public const long UnknownLengthValue = long.MaxValue;

        private Stream? p_inputStream;
        private long p_streamLength = -1;

        //private readonly List<ReaderInfo> rdrList = new List<ReaderInfo>();
        private bool p_isStreamFinished;

        private const int DefaultChunkLength = 4 * 1024;
        private readonly int p_chunkLength;
        public Dictionary<long, byte[]> p_chunks = new Dictionary<long, byte[]>();

        public RandomAccessStream(Stream stream, long streamLength = -1)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (streamLength == -1)
                streamLength = stream.CanSeek ? stream.Length : UnknownLengthValue;

            p_inputStream = stream;
            CanSeek = stream.CanSeek;

            p_chunkLength = DefaultChunkLength;
            p_streamLength = streamLength;
        }

        public RandomAccessStream(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            CanSeek = true;

            // Setting these values makes p_inputStream irrelevant
            // TODO: break the byte array up into DefaultChunkLength chunks
            p_chunks.Add(0, bytes);
            p_chunkLength = bytes.Length;

            p_streamLength = bytes.Length;
            p_isStreamFinished = true;
        }

        public bool CanSeek { get; private set; } = false;

        /// <summary>
        /// Returns the length of the underlying data source
        /// </summary>
        /// <remarks>
        /// Length is always known when the data source is an array. For data sources derived from <see cref="Stream"/>,
        /// the CanSeek property is checked. If the value is true, the <see cref="Stream"/>'s Length property is used.
        /// If the value is false, it is assumed the length cannot be determined from the <see cref="Stream"/> itself and
        /// long.MaxValue is used instead.
        /// </remarks>
        public long Length
        {
            get
            {
                // If finished and only one chunk, can bypass a lot of checks particularly when the input was a byte[]
                //return (CanSeek) ? (p_isStreamFinished && p_chunks.Count == 1 ? p_streamLength : p_inputStream.Length) : (long)int.MaxValue;
                return p_streamLength;
            }
        }

        public ReaderInfo CreateReader() => CreateReader(-1, -1, true);
        public ReaderInfo CreateReader(bool isMotorolaByteOrder) => CreateReader(-1, -1, isMotorolaByteOrder);
        public ReaderInfo CreateReader(long startPosition, long length, bool isMotorolaByteOrder)
        {
            var pos = startPosition >= 0 ? startPosition : 0;
            return new ReaderInfo(this, pos, 0, length, isMotorolaByteOrder);
        }

        /// <summary>Retrieves bytes, writing them into a caller-provided buffer.</summary>
        /// <param name="index">position within the data buffer to read byte.</param>
        /// <param name="buffer">array to write bytes to.</param>
        /// <param name="offset">starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="count">number of bytes to be written.</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <returns>The requested bytes, or as many as can be retrieved</returns>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public int Read(long index, byte[] buffer, int offset, int count, bool isSequential)
        {
            return Read(index, buffer, offset, count, isSequential, true);
        }

        /// <summary>Retrieves bytes, writing them into a caller-provided buffer.</summary>
        /// <param name="index">position within the data buffer to read byte.</param>
        /// <param name="buffer">array to write bytes to.</param>
        /// <param name="offset">starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="count">number of bytes to be written.</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <param name="allowPartial">flag indicating whether count should be enforced when validating the index</param>
        /// <returns>The requested bytes, or as many as can be retrieved if <paramref name="allowPartial"/> is true</returns>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public int Read(long index, byte[] buffer, int offset, int count, bool isSequential, bool allowPartial)
        {
            count = (int)ValidateIndex(index, count, isSequential, allowPartial);

            // This bypasses a lot of checks particularly when the input was a byte[]
            // TODO: good spot to try Span<T>
            if (p_isStreamFinished && p_chunks.Count == 1)
            {
                Array.Copy(p_chunks[0], (int)index, buffer, 0, count);
                return count;
            }

            var remaining = count;                      // how many bytes are requested
            var fromOffset = (int)index;
            var toIndex = offset > 0 ? offset : 0;
            while (remaining != 0)
            {
                var fromChunkIndex = fromOffset / p_chunkLength;     // chunk integer key
                var fromInnerIndex = fromOffset % p_chunkLength;     // index inside the chunk to start reading
                var length = Math.Min(remaining, p_chunkLength - fromInnerIndex);
                var chunk = p_chunks[fromChunkIndex];
                Array.Copy(chunk, fromInnerIndex, buffer, toIndex, length);
                remaining -= length;
                fromOffset += length;
                toIndex += length;
            }

            return toIndex - offset;
        }

        /// <summary>Returns an unsigned byte at an index in the sequence.</summary>
        /// <returns>the 8 bit int value, between 0 and 255</returns>
        /// <param name="index">position within the data buffer to read byte</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public byte GetByte(long index, bool isSequential)
        {
            return GetByte(index, isSequential, true);
        }

        /// <summary>Returns an unsigned byte at an index in the sequence.</summary>
        /// <returns>the 8 bit int value, between 0 and 255</returns>
        /// <param name="index">position within the data buffer to read byte</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <param name="validateIndex">allows for skipping validation if already done by the caller</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        private byte GetByte(long index, bool isSequential, bool validateIndex)
        {
            if(validateIndex)
                ValidateIndex(index, 1, isSequential);

            // This bypasses a lot of checks particularly when the input was a byte[]
            if (p_isStreamFinished && p_chunks.Count == 1)
                return p_chunks[0][index];

            var chunkIndex = index / p_chunkLength;
            var innerIndex = index % p_chunkLength;

            if (p_chunks.ContainsKey(chunkIndex))
                return p_chunks[chunkIndex][innerIndex];
            else
                return unchecked((byte)-1);
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public ushort GetUInt16(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 2, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (ushort)
                    (GetByte(index, isSequential, false) << 8 |
                     GetByte(index + 1, isSequential, false));
            }
            // Intel ordering - LSB first
            return (ushort)
                (GetByte(index + 1, isSequential, false) << 8 |
                 GetByte(index    , isSequential, false));
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public short GetInt16(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 2, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (short)
                    (GetByte(index    , isSequential, false) << 8 |
                     GetByte(index + 1, isSequential, false));
            }
            // Intel ordering - LSB first
            return (short)
                (GetByte(index + 1, isSequential, false) << 8 |
                 GetByte(index    , isSequential, false));
        }

        /// <summary>Get a 24-bit unsigned integer from the buffer, returning it as an int.</summary>
        /// <returns>the unsigned 24-bit int value as a long, between 0x00000000 and 0x00FFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public int GetInt24(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 3, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    GetByte(index    , isSequential, false) << 16 |
                    GetByte(index + 1, isSequential, false) << 8 |
                    GetByte(index + 2, isSequential, false);
            }
            // Intel ordering - LSB first
            return
                GetByte(index + 2, isSequential, false) << 16 |
                GetByte(index + 1, isSequential, false) << 8 |
                GetByte(index    , isSequential, false);
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public uint GetUInt32(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 4, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (uint)
                    (GetByte(index    , isSequential, false) << 24 |
                     GetByte(index + 1, isSequential, false) << 16 |
                     GetByte(index + 2, isSequential, false) << 8 |
                     GetByte(index + 3, isSequential, false));
            }
            // Intel ordering - LSB first
            return (uint)
                (GetByte(index + 3, isSequential, false) << 24 |
                 GetByte(index + 2, isSequential, false) << 16 |
                 GetByte(index + 1, isSequential, false) << 8 |
                 GetByte(index    , isSequential, false));
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data.</summary>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public int GetInt32(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 4, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    GetByte(index    , isSequential, false) << 24 |
                    GetByte(index + 1, isSequential, false) << 16 |
                    GetByte(index + 2, isSequential, false) << 8 |
                    GetByte(index + 3, isSequential, false);
            }
            // Intel ordering - LSB first
            return
                GetByte(index + 3, isSequential, false) << 24 |
                GetByte(index + 2, isSequential, false) << 16 |
                GetByte(index + 1, isSequential, false) << 8 |
                GetByte(index    , isSequential, false);
        }

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public long GetInt64(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 8, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (long)GetByte(index    , isSequential, false) << 56 |
                    (long)GetByte(index + 1, isSequential, false) << 48 |
                    (long)GetByte(index + 2, isSequential, false) << 40 |
                    (long)GetByte(index + 3, isSequential, false) << 32 |
                    (long)GetByte(index + 4, isSequential, false) << 24 |
                    (long)GetByte(index + 5, isSequential, false) << 16 |
                    (long)GetByte(index + 6, isSequential, false) << 8 |
                          GetByte(index + 7, isSequential, false);
            }
            // Intel ordering - LSB first
            return
                (long)GetByte(index + 7, isSequential, false) << 56 |
                (long)GetByte(index + 6, isSequential, false) << 48 |
                (long)GetByte(index + 5, isSequential, false) << 40 |
                (long)GetByte(index + 4, isSequential, false) << 32 |
                (long)GetByte(index + 3, isSequential, false) << 24 |
                (long)GetByte(index + 2, isSequential, false) << 16 |
                (long)GetByte(index + 1, isSequential, false) << 8 |
                      GetByte(index    , isSequential, false);
        }

        /// <summary>Get an usigned 64-bit integer from the buffer.</summary>
        /// <returns>the unsigned 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public ulong GetUInt64(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 8, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (ulong)GetByte(index    , isSequential, false) << 56 |
                    (ulong)GetByte(index + 1, isSequential, false) << 48 |
                    (ulong)GetByte(index + 2, isSequential, false) << 40 |
                    (ulong)GetByte(index + 3, isSequential, false) << 32 |
                    (ulong)GetByte(index + 4, isSequential, false) << 24 |
                    (ulong)GetByte(index + 5, isSequential, false) << 16 |
                    (ulong)GetByte(index + 6, isSequential, false) << 8 |
                           GetByte(index + 7, isSequential, false);
            }
            // Intel ordering - LSB first
            return
                (ulong)GetByte(index + 7, isSequential, false) << 56 |
                (ulong)GetByte(index + 6, isSequential, false) << 48 |
                (ulong)GetByte(index + 5, isSequential, false) << 40 |
                (ulong)GetByte(index + 4, isSequential, false) << 32 |
                (ulong)GetByte(index + 3, isSequential, false) << 24 |
                (ulong)GetByte(index + 2, isSequential, false) << 16 |
                (ulong)GetByte(index + 1, isSequential, false) << 8 |
                       GetByte(index    , isSequential, false);
        }

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// Gets a s15.16 fixed point float from the buffer.
        /// <para />
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="IsMotorolaByteOrder">byte order for returning the result</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public float GetS15Fixed16(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            ValidateIndex(index, 4, isSequential);
            if (IsMotorolaByteOrder)
            {
                float res = GetByte(index, isSequential, false) << 8 | GetByte(index + 1, isSequential, false);
                var d = GetByte(index + 2, isSequential, false) << 8 | GetByte(index + 3, isSequential, false);
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = GetByte(index + 1, isSequential, false) << 8 | GetByte(index, isSequential, false);
                float res = GetByte(index + 3, isSequential, false) << 8 | GetByte(index + 2, isSequential, false);
                return (float)(res + d / 65536.0);
            }
        }

        /// <summary>Seeks to an index in the sequence.</summary>
        /// <remarks>
        /// Seeks to an index in the sequence. If the sequence can't satisfy the request, exceptions are thrown.
        /// </remarks>
        /// <param name="index">position within the data buffer to seek to</param>
        /// <param name="isSequential">optional for testing to indicate whether sequential access was used.</param>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public void Seek(long index, bool isSequential = false)
        {
            ValidateIndex((index == 0) ? 0 : (index - 1), 1, isSequential);
        }

        /// <summary>
        /// Ensures that the buffered bytes extend to cover the specified index. If not, an attempt is made
        /// to read to that point.
        /// </summary>
        /// <returns>The number of bytes available out of the number of bytes requested</returns>
        /// <remarks>
        /// If the stream ends before the point is reached, a <see cref="BufferBoundsException"/> is raised.
        /// Requesting more bytes than available raises an exception if <paramref name="allowPartial"/> is false
        /// </remarks>
        /// <param name="index">the index from which the required bytes start</param>
        /// <param name="bytesRequested">the number of bytes which are required</param>
        /// <param name="isSequential">flag indicating if caller is using sequential access</param>
        /// <param name="allowPartial">flag indicating whether count should be enforced when validating the index</param>
        /// <exception cref="BufferBoundsException">negative index, less than 0 bytes, or too many bytes are requested</exception>
        /// <exception cref="IOException">if the stream ends before the required number of bytes are acquired</exception>
        internal long ValidateIndex(long index, long bytesRequested, bool isSequential, bool allowPartial = false)
        {
            long available = BytesAvailable(index, bytesRequested);
            if (available != bytesRequested && !allowPartial)
            {
                if (index < 0)
                    throw new BufferBoundsException($"Attempt to read from buffer using a negative index ({index})");
                if (bytesRequested < 0)
                    throw new BufferBoundsException("Number of requested bytes must be zero or greater");
                if (index + bytesRequested - 1 > int.MaxValue)
                    throw new BufferBoundsException($"Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: {index}, requested count: {bytesRequested})");
                if (index + bytesRequested >= p_streamLength)
                {
                    if (isSequential)
                        throw new IOException("End of data reached.");
                    else
                        throw new BufferBoundsException(index, bytesRequested, p_streamLength);
                }

                // TODO test that can continue using an instance of this type after this exception
                throw new BufferBoundsException(index, bytesRequested, p_streamLength);
            }

            return available;
        }

        private long BytesAvailable(long index, long bytesRequested)
        {
            if (index < 0 || bytesRequested < 0)
                return 0;

            // if there's only one chunk, there's no need to calculate anything.
            // This bypasses a lot of checks particularly when the input was a byte[]
            if (p_isStreamFinished && p_chunks.Count == 1)
            {
                if ((index + bytesRequested) < p_streamLength)
                    return bytesRequested;
                else if (index > p_streamLength)
                    return 0;
                else
                    return p_streamLength - index;
            }


            var endIndex = index + bytesRequested - 1;
            if (endIndex < 0) endIndex = 0;

            // Maybe don't check this?
            if (endIndex > int.MaxValue)
                return 0;

            // zero-based
            long chunkstart = index / p_chunkLength;
            long chunkend = ((index + bytesRequested) / p_chunkLength) + 1;


            if (!p_chunks.ContainsKey(chunkstart))
            {
                if (!CanSeek)
                    chunkstart = p_chunks.Count == 0 ? 0 : p_chunks.Keys.Max() + 1;
            }

            for (var i = chunkstart; i < chunkend; i++)
            {
                if (!p_chunks.ContainsKey(i))
                {
                    p_isStreamFinished = false;

                    // chunkstart can be anywhere. Try to seek
                    if (CanSeek)
                        p_inputStream.Seek(i * p_chunkLength, SeekOrigin.Begin);

                    byte[] chunk = new byte[p_chunkLength];

                    var totalBytesRead = 0;
                    while (!p_isStreamFinished && totalBytesRead != p_chunkLength)
                    {
                        var bytesRead = p_inputStream.Read(chunk, totalBytesRead, p_chunkLength - totalBytesRead);

                        if (bytesRead == 0)
                        {
                            // the stream has ended, which may be ok
                            p_isStreamFinished = true;
                            p_streamLength = i * p_chunkLength + totalBytesRead;

                            // check we have enough bytes for the requested index
                            if (endIndex >= p_streamLength)
                            {
                                TotalBytesRead += totalBytesRead;
                                p_chunks.Add(i, chunk);
                                return (index + bytesRequested) <= p_streamLength ? bytesRequested : p_streamLength - index;
                            }
                        }
                        else
                        {
                            totalBytesRead += bytesRead;
                        }
                    }

                    TotalBytesRead += totalBytesRead;
                    p_chunks.Add(i, chunk);
                }
            }

            if (p_isStreamFinished)
                return (index + bytesRequested) <= p_streamLength ? bytesRequested : 0;
            else
                return bytesRequested;
        }

        /// <summary>
        /// Records the total bytes buffered
        /// </summary>
        public long TotalBytesRead { get; private set; } = 0;

        public byte[] ToArray(long index, int count)
        {
            byte[] buffer;
            // if this was a byte array and asking for the whole thing...
            if (p_isStreamFinished &&
                p_chunks.Count == 1 &&
                index == 0 &&
                count == Length)
            {
                buffer = p_chunks[0];
            }
            else
            {
                buffer = new byte[count];
                Read(index, buffer, 0, count, false);
            }

            return buffer;
        }
    }
}
