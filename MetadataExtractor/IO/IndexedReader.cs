// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Buffers.Binary;

namespace MetadataExtractor.IO
{
    /// <summary>Base class for random access data reading operations of common data types.</summary>
    /// <remarks>
    /// Concrete implementations include:
    /// <list type="bullet">
    ///   <item><see cref="ByteArrayReader"/></item>
    ///   <item><see cref="IndexedSeekingReader"/></item>
    ///   <item><see cref="IndexedCapturingReader"/></item>
    /// </list>
    /// By default, the reader operates with Motorola byte order (big endianness).  This can be changed by via
    /// <see cref="IsMotorolaByteOrder"/>.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class IndexedReader(bool isMotorolaByteOrder)
    {
        /// <summary>Get the byte order of this reader.</summary>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        ///   <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        public bool IsMotorolaByteOrder { get; } = isMotorolaByteOrder;

        public abstract IndexedReader WithByteOrder(bool isMotorolaByteOrder);

        public abstract IndexedReader WithShiftedBaseOffset(int shift);

        public abstract int ToUnshiftedOffset(int localOffset);

        /// <summary>Returns the required number of bytes from the specified index from the underlying source.</summary>
        /// <param name="index">The index from which the bytes begins in the underlying source</param>
        /// <param name="count">The number of bytes to be returned</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="ArgumentException"><c>index</c> or <c>count</c> are negative</exception>
        /// <exception cref="BufferBoundsException">if the requested bytes extend beyond the end of the underlying data source</exception>
        /// <exception cref="IOException">if the byte is unable to be read</exception>
        public byte[] GetBytes(int index, int count)
        {
            ValidateIndex(index, count);

            var bytes = new byte[count];

            GetBytes(index, bytes.AsSpan());

            return bytes;
        }

        /// <summary>
        /// Copies bytes from the underlying source into <paramref name="bytes"/>.
        /// </summary>
        /// <param name="index">The index from which the bytes begins in the underlying source</param>
        /// <param name="bytes">A span of bytes to copy to. The length of this span determines how many bytes will be copied.</param>
        public abstract void GetBytes(int index, Span<byte> bytes);

        /// <summary>
        /// Ensures that the buffered bytes extend to cover the specified index. If not, an attempt is made
        /// to read to that point.
        /// </summary>
        /// <remarks>
        /// If the stream ends before the point is reached, a <see cref="BufferBoundsException"/> is raised.
        /// </remarks>
        /// <param name="index">the index from which the required bytes start</param>
        /// <param name="bytesRequested">the number of bytes which are required</param>
        /// <exception cref="IOException">if the stream ends before the required number of bytes are acquired</exception>
        protected abstract void ValidateIndex(int index, int bytesRequested);

        /// <summary>Returns the length of the data source in bytes.</summary>
        /// <remarks>
        /// This is a simple operation for implementations (such as <see cref="IndexedSeekingReader"/> and
        /// <see cref="ByteArrayReader"/>) that have the entire data source available.
        /// <para />
        /// Users of this method must be aware that sequentially accessed implementations such as
        /// <see cref="IndexedCapturingReader"/>
        /// will have to read and buffer the entire data source in order to determine the length.
        /// </remarks>
        /// <value>the length of the data source, in bytes.</value>
        /// <exception cref="IOException"/>
        public abstract long Length { get; }

        /// <summary>Gets whether a bit at a specific index is set or not.</summary>
        /// <param name="index">the number of bits at which to test</param>
        /// <returns>true if the bit is set, otherwise false</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public bool GetBit(int index)
        {
            var byteIndex = index / 8;
            var bitIndex = index % 8;

            Span<byte> bytes = stackalloc byte[1];

            GetBytes(byteIndex, bytes);

            return ((bytes[0] >> bitIndex) & 1) == 1;
        }

        /// <summary>Gets the byte value at the specified byte <c>index</c>.</summary>
        /// <remarks>
        /// Implementations must validate <paramref name="index"/> by calling <see cref="ValidateIndex"/>.
        /// </remarks>
        /// <param name="index">The index from which to read the byte</param>
        /// <returns>The read byte value</returns>
        /// <exception cref="ArgumentException"><c>index</c> is negative</exception>
        /// <exception cref="BufferBoundsException">if the requested byte is beyond the end of the underlying data source</exception>
        /// <exception cref="IOException">if the byte is unable to be read</exception>
        public byte GetByte(int index)
        {
            Span<byte> bytes = stackalloc byte[1];

            GetBytes(index, bytes);

            return bytes[0];
        }

        /// <summary>Returns a signed 8-bit int calculated from one byte of data at the specified index.</summary>
        /// <param name="index">position within the data buffer to read byte</param>
        /// <returns>the 8 bit signed byte value</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public sbyte GetSByte(int index)
        {
            return unchecked((sbyte)GetByte(index));
        }

#pragma warning disable format

        /// <summary>Returns an unsigned 16-bit int calculated from two bytes of data at the specified index.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public ushort GetUInt16(int index)
        {
            Span<byte> bytes = stackalloc byte[2];

            GetBytes(index, bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadUInt16BigEndian(bytes)
                : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data at the specified index (MSB, LSB).</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public short GetInt16(int index)
        {
            Span<byte> bytes = stackalloc byte[2];

            GetBytes(index, bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadInt16BigEndian(bytes)
                : BinaryPrimitives.ReadInt16LittleEndian(bytes);
        }

        /// <summary>Get a 24-bit unsigned integer from the buffer, returning it as an int.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the unsigned 24-bit int value as a long, between 0x00000000 and 0x00FFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public int GetInt24(int index)
        {
            Span<byte> bytes = stackalloc byte[3];

            GetBytes(index, bytes);

            if (IsMotorolaByteOrder)
            {
                return
                    bytes[0] << 16 |
                    bytes[1] << 8 |
                    bytes[2];
            }
            else
            {
                return
                    bytes[2] << 16 |
                    bytes[1] << 8 |
                    bytes[0];
            }
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public uint GetUInt32(int index)
        {
            Span<byte> bytes = stackalloc byte[4];

            GetBytes(index, bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadUInt32BigEndian(bytes)
                : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data at the specified index the buffer.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public int GetInt32(int index)
        {
            Span<byte> bytes = stackalloc byte[4];

            GetBytes(index, bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadInt32BigEndian(bytes)
                : BinaryPrimitives.ReadInt32LittleEndian(bytes);
        }

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public long GetInt64(int index)
        {
            Span<byte> bytes = stackalloc byte[8];

            GetBytes(index, bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadInt64BigEndian(bytes)
                : BinaryPrimitives.ReadInt64LittleEndian(bytes);
        }

        /// <summary>Get an unsigned 64-bit integer from the buffer.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public ulong GetUInt64(int index)
        {
            Span<byte> bytes = stackalloc byte[8];

            GetBytes(index, bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadUInt64BigEndian(bytes)
                : BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        }

#pragma warning restore format

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public float GetS15Fixed16(int index)
        {
            Span<byte> bytes = stackalloc byte[4];

            GetBytes(index, bytes);

            if (IsMotorolaByteOrder)
            {
                float res = bytes[0] << 8 | bytes[1];
                var d = bytes[2] << 8 | bytes[3];
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = bytes[1] << 8 | bytes[0];
                float res = bytes[3] << 8 | bytes[2];
                return (float)(res + d / 65536.0);
            }
        }

        /// <exception cref="IOException"/>
        public float GetFloat32(int index)
        {
#if NET45 || NETSTANDARD1_3
            return BitConverter.ToSingle(BitConverter.GetBytes(GetInt32(index)), 0);
#else
            Span<byte> bytes = stackalloc byte[4];

            GetBytes(index, bytes);

            if (IsMotorolaByteOrder)
            {
                bytes.Reverse();
            }

            return BitConverter.ToSingle(bytes);
#endif
        }

        /// <exception cref="IOException"/>
        public double GetDouble64(int index)
        {
#if NET45 || NETSTANDARD1_3
            return BitConverter.Int64BitsToDouble(GetInt64(index));
#else
            Span<byte> bytes = stackalloc byte[8];

            GetBytes(index, bytes);

            if (IsMotorolaByteOrder)
            {
                bytes.Reverse();
            }

            return BitConverter.ToDouble(bytes);
#endif
        }

        /// <exception cref="IOException"/>
        public string GetString(int index, int bytesRequested, Encoding encoding)
        {
#if NET45 || NETSTANDARD1_3
            var bytes = GetBytes(index, bytesRequested);

            return encoding.GetString(bytes, 0, bytes.Length);
#else
            Span<byte> bytes = bytesRequested < 256 ? stackalloc byte[bytesRequested] : new byte[bytesRequested];

            GetBytes(index, bytes);

            return encoding.GetString(bytes);
#endif
        }

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="index">The index within the buffer at which to start reading the string.</param>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <param name="encoding">An optional string encoding. If none is provided, <see cref="Encoding.UTF8"/> is used.</param>
        /// <returns>The read <see cref="string"/></returns>
        /// <exception cref="IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public string GetNullTerminatedString(int index, int maxLengthBytes, Encoding? encoding = null)
        {
            var bytes = GetNullTerminatedBytes(index, maxLengthBytes);

            return (encoding ?? Encoding.UTF8).GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="index">The index within the buffer at which to start reading the string.</param>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <param name="encoding">An optional string encoding to use when interpreting bytes.</param>
        /// <returns>The read <see cref="StringValue"/></returns>
        /// <exception cref="IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public StringValue GetNullTerminatedStringValue(int index, int maxLengthBytes, Encoding? encoding = null)
        {
            var bytes = GetNullTerminatedBytes(index, maxLengthBytes);

            return new StringValue(bytes, encoding);
        }

        /// <summary>
        /// Returns the sequence of bytes punctuated by a <c>\0</c> value.
        /// </summary>
        /// <param name="index">The index to start reading from.</param>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a <c>\0</c> byte is not reached within this limit,
        /// the returned array will be <paramref name="maxLengthBytes"/> long.
        /// </param>
        /// <returns>The read byte array.</returns>
        /// <exception cref="IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public byte[] GetNullTerminatedBytes(int index, int maxLengthBytes)
        {
            var buffer = GetBytes(index, maxLengthBytes);

            // Count the number of non-null bytes
            var length = 0;
            while (length < buffer.Length && buffer[length] != 0)
                length++;

            if (length == 0)
                return Empty.ByteArray;
            if (length == maxLengthBytes)
                return buffer;

            var bytes = new byte[length];
            if (length > 0)
                Array.Copy(buffer, 0, bytes, 0, length);
            return bytes;
        }
    }
}
