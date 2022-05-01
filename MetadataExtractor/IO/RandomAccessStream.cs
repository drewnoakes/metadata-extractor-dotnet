// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

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
    public class RandomAccessStream : IRandomAccessStream
    {
        public const long UnknownLengthValue = long.MaxValue;

        private readonly Stream p_inputStream;
        private long p_streamLength = -1;

        //private readonly List<ReaderInfo> rdrList = new List<ReaderInfo>();
        private bool p_isStreamFinished;

        private const int DefaultChunkLength = 4 * 1024;
        private readonly int p_chunkLength;

        private List<byte[]> p_chunks = new List<byte[]>();

        public RandomAccessStream(Stream stream, long streamLength = -1)
        {
            p_inputStream = stream ?? throw new ArgumentNullException(nameof(stream));
            CanSeek = stream.CanSeek;

            if (streamLength == -1)
            {
                // Make sure a stream that can seek is also able to report a Length.
                // This is an uncommon situation; don't know of any as of this writing
                try
                {
                    streamLength = stream.CanSeek ? stream.Length : UnknownLengthValue;
                }
                catch(NotSupportedException)
                {
                    streamLength = UnknownLengthValue;
                    CanSeek = false;
                }
            }

            // TODO: allow a different chunk length either through this constructor or read from a context object
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
            p_chunks.Add(bytes);
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
        /// <param name="index">position within the data buffer to start reading.</param>
        /// <param name="buffer">array to write bytes to.</param>
        /// <param name="offset">starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="count">number of bytes to be written.</param>
        /// <returns>The requested bytes, or as many as can be retrieved</returns>
        /// <exception cref="BufferBoundsException"/>
        public int Read(long index, byte[] buffer, int offset, int count)
        {
            return Read(index, buffer, offset, count, true);
        }

        /// <summary>Retrieves bytes, writing them into a caller-provided buffer.</summary>
        /// <param name="index">position within the data buffer to start reading.</param>
        /// <param name="buffer">array to write bytes to.</param>
        /// <param name="offset">starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="count">number of bytes to be written.</param>
        /// <param name="allowPartial">flag indicating whether count should be enforced when validating the index</param>
        /// <returns>The requested bytes read into <paramref name="buffer"/>, and either the count of bytes read or as many as can be retrieved if <paramref name="allowPartial"/> is true</returns>
        /// <exception cref="BufferBoundsException"/>
        public int Read(long index, byte[] buffer, int offset, int count, bool allowPartial)
        {
            if (allowPartial)
                count = (int)BytesAvailable(index, count);  // skips validation overhead
            else
                count = (int)ValidateRange(index, count);

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
        /// <exception cref="BufferBoundsException"/>
        public byte GetByte(long index)
        {
            ValidateRange(index, 1);

            return GetByteNoValidation(index);
        }

        /// <summary>Returns an unsigned byte at an index in the sequence. The index is not validated before reading</summary>
        /// <returns>the 8 bit int value, between 0 and 255</returns>
        /// <param name="index">position within the data buffer to read byte</param>
        /// <exception cref="BufferBoundsException"/>
        private byte GetByteNoValidation(long index)
        {
            // This bypasses a lot of checks particularly when the input was a byte[]
            if (p_isStreamFinished && p_chunks.Count == 1)
                return p_chunks[0][index];

            // micro-optimization for benchmarks
            //var intIndex = (int)index;

            //var chunkIndex = intIndex / p_chunkLength;
            //var innerIndex = intIndex % p_chunkLength;

            var chunkIndex = (int)(index / p_chunkLength);
            var innerIndex = (int)(index % p_chunkLength);

            if (p_chunks.Count - 1 >= chunkIndex)
                return p_chunks[chunkIndex][innerIndex];
            else
                throw new IOException("End of data reached.");
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public ushort GetUInt16(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 2);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (ushort)
                    (GetByteNoValidation(index    ) << 8 |
                     GetByteNoValidation(index + 1));
            }
            // Intel ordering - LSB first
            return (ushort)
                (GetByteNoValidation(index + 1) << 8 |
                 GetByteNoValidation(index    ));
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public short GetInt16(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 2);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (short)
                    (GetByteNoValidation(index    ) << 8 |
                     GetByteNoValidation(index + 1));
            }
            // Intel ordering - LSB first
            return (short)
                (GetByteNoValidation(index + 1) << 8 |
                 GetByteNoValidation(index    ));
        }

        /// <summary>Get a 24-bit unsigned integer from the buffer, returning it as an int.</summary>
        /// <returns>the unsigned 24-bit int value as a long, between 0x00000000 and 0x00FFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public int GetInt24(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 3);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    GetByteNoValidation(index    ) << 16 |
                    GetByteNoValidation(index + 1) << 8 |
                    GetByteNoValidation(index + 2);
            }
            // Intel ordering - LSB first
            return
                GetByteNoValidation(index + 2) << 16 |
                GetByteNoValidation(index + 1) << 8 |
                GetByteNoValidation(index    );
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public uint GetUInt32(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 4);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (uint)
                    (GetByteNoValidation(index    ) << 24 |
                     GetByteNoValidation(index + 1) << 16 |
                     GetByteNoValidation(index + 2) << 8 |
                     GetByteNoValidation(index + 3));
            }
            // Intel ordering - LSB first
            return (uint)
                (GetByteNoValidation(index + 3) << 24 |
                 GetByteNoValidation(index + 2) << 16 |
                 GetByteNoValidation(index + 1) << 8 |
                 GetByteNoValidation(index    ));
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data.</summary>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public int GetInt32(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 4);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    GetByteNoValidation(index    ) << 24 |
                    GetByteNoValidation(index + 1) << 16 |
                    GetByteNoValidation(index + 2) << 8 |
                    GetByteNoValidation(index + 3);
            }
            // Intel ordering - LSB first
            return
                GetByteNoValidation(index + 3) << 24 |
                GetByteNoValidation(index + 2) << 16 |
                GetByteNoValidation(index + 1) << 8 |
                GetByteNoValidation(index    );
        }

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public long GetInt64(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 8);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (long)GetByteNoValidation(index    ) << 56 |
                    (long)GetByteNoValidation(index + 1) << 48 |
                    (long)GetByteNoValidation(index + 2) << 40 |
                    (long)GetByteNoValidation(index + 3) << 32 |
                    (long)GetByteNoValidation(index + 4) << 24 |
                    (long)GetByteNoValidation(index + 5) << 16 |
                    (long)GetByteNoValidation(index + 6) << 8 |
                          GetByteNoValidation(index + 7);
            }
            // Intel ordering - LSB first
            return
                (long)GetByteNoValidation(index + 7) << 56 |
                (long)GetByteNoValidation(index + 6) << 48 |
                (long)GetByteNoValidation(index + 5) << 40 |
                (long)GetByteNoValidation(index + 4) << 32 |
                (long)GetByteNoValidation(index + 3) << 24 |
                (long)GetByteNoValidation(index + 2) << 16 |
                (long)GetByteNoValidation(index + 1) << 8 |
                      GetByteNoValidation(index    );
        }

        /// <summary>Get an usigned 64-bit integer from the buffer.</summary>
        /// <returns>the unsigned 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public ulong GetUInt64(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 8);

            if (isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (ulong)GetByteNoValidation(index    ) << 56 |
                    (ulong)GetByteNoValidation(index + 1) << 48 |
                    (ulong)GetByteNoValidation(index + 2) << 40 |
                    (ulong)GetByteNoValidation(index + 3) << 32 |
                    (ulong)GetByteNoValidation(index + 4) << 24 |
                    (ulong)GetByteNoValidation(index + 5) << 16 |
                    (ulong)GetByteNoValidation(index + 6) << 8 |
                           GetByteNoValidation(index + 7);
            }
            // Intel ordering - LSB first
            return
                (ulong)GetByteNoValidation(index + 7) << 56 |
                (ulong)GetByteNoValidation(index + 6) << 48 |
                (ulong)GetByteNoValidation(index + 5) << 40 |
                (ulong)GetByteNoValidation(index + 4) << 32 |
                (ulong)GetByteNoValidation(index + 3) << 24 |
                (ulong)GetByteNoValidation(index + 2) << 16 |
                (ulong)GetByteNoValidation(index + 1) << 8 |
                       GetByteNoValidation(index    );
        }

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// Gets a s15.16 fixed point float from the buffer.
        /// <para />
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <param name="isMotorolaByteOrder">byte order for returning the result</param>
        /// <exception cref="BufferBoundsException"/>
        public float GetS15Fixed16(long index, bool isMotorolaByteOrder)
        {
            ValidateRange(index, 4);
            if (isMotorolaByteOrder)
            {
                float res = GetByteNoValidation(index) << 8 | GetByteNoValidation(index + 1);
                var d = GetByteNoValidation(index + 2) << 8 | GetByteNoValidation(index + 3);
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = GetByteNoValidation(index + 1) << 8 | GetByteNoValidation(index);
                float res = GetByteNoValidation(index + 3) << 8 | GetByteNoValidation(index + 2);
                return (float)(res + d / 65536.0);
            }
        }

        /// <summary>Seeks to an index in the sequence.</summary>
        /// <remarks>
        /// Seeks to an index in the sequence. If the sequence can't satisfy the request, exceptions are thrown.
        /// </remarks>
        /// <param name="index">position within the data buffer to seek to</param>
        /// <exception cref="BufferBoundsException"/>
        public void Seek(long index)
        {
            ValidateRange((index == 0) ? 0 : (index - 1), 1);
        }

        /// <summary>
        /// Ensures that the buffered bytes extend to cover the specified index. If not, an attempt is made
        /// to read to that point.
        /// </summary>
        /// <returns>The number of bytes available out of the number of bytes requested</returns>
        /// <remarks>
        /// If the stream ends before the point is reached, a <see cref="BufferBoundsException"/> is raised.
        /// </remarks>
        /// <param name="index">the index from which the required bytes start</param>
        /// <param name="bytesRequested">the number of bytes which are required</param>
        /// <exception cref="BufferBoundsException">negative index, less than 0 bytes, or too many bytes are requested</exception>
        public int ValidateRange(long index, int bytesRequested)
        {
            var available = BytesAvailable(index, bytesRequested);
            if (available != bytesRequested)
            {
                if (index < 0)
                    throw new BufferBoundsException($"Attempt to read from buffer using a negative index ({index})");
                if (bytesRequested < 0)
                    throw new BufferBoundsException("Number of requested bytes must be zero or greater");
                if (index + bytesRequested - 1 > int.MaxValue)
                    throw new BufferBoundsException($"Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: {index}, requested count: {bytesRequested})");
                if (index + bytesRequested >= p_streamLength)
                    throw new BufferBoundsException(index, bytesRequested, p_streamLength);

                // TODO test that can continue using an instance of this type after this exception
                throw new BufferBoundsException(index, bytesRequested, p_streamLength);
            }

            return available;
        }

        private int BytesAvailable(long index, int bytesRequested)
        {
            if (index < 0L || bytesRequested < 0)
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
                    return (int)(p_streamLength - index);
            }


            var endIndex = index + bytesRequested - 1L;
            if (endIndex < 0L) endIndex = 0L;

            // Maybe don't check this?
            if (endIndex > int.MaxValue)
                return 0;

            // micro-optimization for benchmarks
            //var intIndex = (int)index;

            // zero-based
            //int chunkstart = intIndex / p_chunkLength;
            //int chunkend = ((intIndex + bytesRequested) / p_chunkLength) + 1;
            int chunkstart = (int)(index / p_chunkLength);
            int chunkend = chunkstart + (bytesRequested / p_chunkLength) + 1;

            if (p_chunks.Count - 1 < chunkstart || p_chunks[chunkstart] == null)
            {
                if (!CanSeek)
                    chunkstart = p_chunks.Count;
            }

            // fill the chunks List with enough nulls to cover the start/end range
            if (p_chunks.Count < chunkend)
            {
                for (int i = p_chunks.Count; i < chunkend; i++)
                {
                    p_chunks.Add(null);
                }
            }

            for (var i = chunkstart; i < chunkend; i++)
            {
                if (p_chunks[i] == null)
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
#if DEBUG
                                TotalBytesRead += totalBytesRead;
#endif
                                p_chunks[i] = chunk;
                                return (index + bytesRequested) <= p_streamLength ? bytesRequested : (int)(p_streamLength - index);
                            }
                        }
                        else
                        {
                            totalBytesRead += bytesRead;
                        }
                    }

#if DEBUG
                    TotalBytesRead += totalBytesRead;
#endif
                    p_chunks[i] = chunk;
                }
            }

            if (p_isStreamFinished)
                return (index + bytesRequested) <= p_streamLength ? bytesRequested : 0;
            else
                return bytesRequested;
        }

#if DEBUG
        /// <summary>
        /// Records the total bytes buffered
        /// </summary>
        public long TotalBytesRead { get; private set; } = 0;
#endif

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
