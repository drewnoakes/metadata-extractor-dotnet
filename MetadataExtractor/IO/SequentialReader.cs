// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;

namespace MetadataExtractor.IO
{
    /// <summary>Base class for reading sequentially through a sequence of data encoded in a byte stream.</summary>
    /// <remarks>
    /// Concrete implementations include:
    /// <list type="bullet">
    ///   <item><see cref="SequentialByteArrayReader"/></item>
    ///   <item><see cref="SequentialStreamReader"/></item>
    /// </list>
    /// By default, the reader operates with Motorola byte order (big endianness).  This can be changed by via
    /// <see cref="IsMotorolaByteOrder"/>.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class SequentialReader(bool isMotorolaByteOrder)
    {
        /// <summary>Get and set the byte order of this reader. <c>true</c> by default.</summary>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        ///   <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        /// <value><c>true</c> for Motorola/big endian, <c>false</c> for Intel/little endian</value>
        public bool IsMotorolaByteOrder { get; } = isMotorolaByteOrder;

        public abstract long Position { get; }

        public abstract SequentialReader WithByteOrder(bool isMotorolaByteOrder);

        /// <summary>Returns the required number of bytes from the sequence.</summary>
        /// <param name="count">The number of bytes to be returned</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="IOException"/>
        public abstract byte[] GetBytes(int count);

        public abstract void GetBytes(Span<byte> bytes);

        /// <summary>Retrieves bytes, writing them into a caller-provided buffer.</summary>
        /// <param name="buffer">The array to write bytes to.</param>
        /// <param name="offset">The starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="count">The number of bytes to be written.</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="IOException"/>
        public abstract void GetBytes(byte[] buffer, int offset, int count);

        /// <summary>Skips forward in the sequence.</summary>
        /// <remarks>
        /// Skips forward in the sequence. If the sequence ends, an <see cref="IOException"/> is thrown.
        /// </remarks>
        /// <param name="n">the number of byte to skip. Must be zero or greater.</param>
        /// <exception cref="IOException">the end of the sequence is reached.</exception>
        /// <exception cref="IOException">an error occurred reading from the underlying source.</exception>
        public abstract void Skip(long n);

        /// <summary>Skips forward in the sequence, returning a boolean indicating whether the skip succeeded, or whether the sequence ended.</summary>
        /// <param name="n">the number of byte to skip. Must be zero or greater.</param>
        /// <returns>a boolean indicating whether the skip succeeded, or whether the sequence ended.</returns>
        /// <exception cref="IOException">an error occurred reading from the underlying source.</exception>
        public abstract bool TrySkip(long n);

        /// <summary>
        /// Returns an estimate of the number of bytes that can be read (or skipped over)
        /// from this SequentialReader without blocking by the next
        /// invocation of a method for this input stream.A single read or skip of
        /// this many bytes will not block, but may read or skip fewer bytes.
        /// </summary>
        /// <remarks>
        /// Note that while some implementations of SequentialReader like
        /// SequentialByteArrayReader will return the total remaining number
        /// of bytes in the stream, others will not. It is never correct to use the
        /// return value of this method to allocate a buffer intended to hold all
        /// data in this stream.
        /// </remarks>
        /// <returns>
        /// an estimate of the number of bytes that can be read (or skipped
        /// over) from this SequentialReader without blocking or
        /// 0 when it reaches the end of the input stream.
        /// </returns>
        public abstract int Available();

        /// <summary>Returns the next unsigned byte from the sequence.</summary>
        /// <returns>the 8 bit int value, between 0 and 255</returns>
        /// <exception cref="IOException"/>
        public abstract byte GetByte();

        /// <summary>Returns a signed 8-bit int calculated from the next byte the sequence.</summary>
        /// <returns>the 8 bit int value, between 0x00 and 0xFF</returns>
        /// <exception cref="IOException"/>
        public sbyte GetSByte() => unchecked((sbyte)GetByte());

#pragma warning disable format

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="IOException"/>
        public ushort GetUInt16()
        {
            Span<byte> bytes = stackalloc byte[2];

            GetBytes(bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadUInt16BigEndian(bytes)
                : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request</exception>
        public short GetInt16()
        {
            Span<byte> bytes = stackalloc byte[2];

            GetBytes(bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadInt16BigEndian(bytes)
                : BinaryPrimitives.ReadInt16LittleEndian(bytes);
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request</exception>
        public uint GetUInt32()
        {
            Span<byte> bytes = stackalloc byte[4];

            GetBytes(bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadUInt32BigEndian(bytes)
                : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data.</summary>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request</exception>
        public int GetInt32()
        {
            Span<byte> bytes = stackalloc byte[4];

            GetBytes(bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadInt32BigEndian(bytes)
                : BinaryPrimitives.ReadInt32LittleEndian(bytes);
        }

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request</exception>
        public long GetInt64()
        {
            Span<byte> bytes = stackalloc byte[8];
            GetBytes(bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadInt64BigEndian(bytes)
                : BinaryPrimitives.ReadInt64LittleEndian(bytes);
        }

        /// <summary>Get an unsigned 64-bit integer from the buffer.</summary>
        /// <returns>the unsigned 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request</exception>
        public ulong GetUInt64()
        {
            Span<byte> bytes = stackalloc byte[8];
            GetBytes(bytes);

            return IsMotorolaByteOrder
                ? BinaryPrimitives.ReadUInt64BigEndian(bytes)
                : BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        }

#pragma warning restore format

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// Gets a s15.16 fixed point float from the buffer.
        /// <para />
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <exception cref="IOException">the buffer does not contain enough bytes to service the request</exception>
        public float GetS15Fixed16()
        {
            if (IsMotorolaByteOrder)
            {
                float res = GetByte() << 8 | GetByte();
                var d = GetByte() << 8 | GetByte();
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = GetByte() | GetByte() << 8;
                float res = GetByte() | GetByte() << 8;
                return (float)(res + d / 65536.0);
            }
        }

        /// <exception cref="IOException"/>
        public float GetFloat32() => BitConverter.ToSingle(BitConverter.GetBytes(GetInt32()), 0);

        /// <exception cref="IOException"/>
        public double GetDouble64() => BitConverter.Int64BitsToDouble(GetInt64());

        /// <exception cref="IOException"/>
        public string GetString(int bytesRequested, Encoding encoding)
        {
#if NETSTANDARD2_1
            Span<byte> bytes = bytesRequested > 2048 ? new byte[bytesRequested] : stackalloc byte[bytesRequested];
            GetBytes(bytes);
            return encoding.GetString(bytes);
#else
            var bytes = GetBytes(bytesRequested);
            return encoding.GetString(bytes, 0, bytes.Length);
#endif
        }

        public StringValue GetStringValue(int bytesRequested, Encoding? encoding = null)
        {
            return new(GetBytes(bytesRequested), encoding);
        }

        /// <summary>
        /// Creates a <see cref="string"/> from the stream, ending where <c>byte=='\0'</c> or where <c>length==maxLength</c>.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a <c>\0</c> byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <param name="encoding">An optional string encoding. If none is provided, <see cref="Encoding.UTF8"/> is used.</param>
        /// <param name="moveToMaxLength">
        /// If <see langword="true"/>, this reader will move forwards <paramref name="moveToMaxLength"/>
        /// bytes, regardless of whether a null byte is found or not. The returned array is not impacted
        /// by this value, and will always have trailing nulls removed.
        /// </param>
        /// <returns>The read <see cref="string"/></returns>
        /// <exception cref="IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public string GetNullTerminatedString(int maxLengthBytes, Encoding? encoding = null, bool moveToMaxLength = false)
        {
            var bytes = GetNullTerminatedBytes(maxLengthBytes, moveToMaxLength);

            return (encoding ?? Encoding.UTF8).GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Creates a <see cref="StringValue"/> from the stream, ending where <c>byte=='\0'</c> or where <c>length==maxLength</c>.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a <c>\0</c> byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <param name="encoding">An optional string encoding to use when interpreting bytes.</param>
        /// <param name="moveToMaxLength">
        /// If <see langword="true"/>, this reader will move forwards <paramref name="moveToMaxLength"/>
        /// bytes, regardless of whether a null byte is found or not. The returned array is not impacted
        /// by this value, and will always have trailing nulls removed.
        /// </param>
        /// <returns>The read string as a <see cref="StringValue"/>, excluding the null terminator.</returns>
        /// <exception cref="IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public StringValue GetNullTerminatedStringValue(int maxLengthBytes, Encoding? encoding = null, bool moveToMaxLength = false)
        {
            var bytes = GetNullTerminatedBytes(maxLengthBytes, moveToMaxLength);

            return new StringValue(bytes, encoding);
        }

        /// <summary>
        /// Returns the sequence of bytes punctuated by a <c>\0</c> value.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a <c>\0</c> byte is not reached within this limit,
        /// the returned array will be <paramref name="maxLengthBytes"/> long.
        /// </param>
        /// <param name="moveToMaxLength">
        /// If <see langword="true"/>, this reader will move forwards <paramref name="moveToMaxLength"/>
        /// bytes, regardless of whether a null byte is found or not. The returned array is not impacted
        /// by this value, and will always have trailing nulls removed.
        /// </param>
        /// <returns>The read byte array, excluding the null terminator.</returns>
        /// <exception cref="IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public byte[] GetNullTerminatedBytes(int maxLengthBytes, bool moveToMaxLength = false)
        {
            // The number of non-null bytes
            int length;

            byte[] buffer;

            if (moveToMaxLength)
            {
                buffer = GetBytes(maxLengthBytes);
                length = Array.IndexOf(buffer, (byte)'\0') switch
                {
                    -1 => maxLengthBytes,
                    int i => i
                };
            }
            else
            {
                buffer = new byte[maxLengthBytes];
                length = 0;

                while (length < buffer.Length && (buffer[length] = GetByte()) != 0)
                    length++;
            }

            if (length == 0)
                return Empty.ByteArray;
            if (length == maxLengthBytes)
                return buffer;
            var bytes = new byte[length];
            if (length > 0)
                Array.Copy(buffer, bytes, length);
            return bytes;
        }

        /// <summary>
        /// Returns true in case the stream supports length checking and distance to the end of the stream is less then number of bytes in parameter.
        /// Otherwise false.
        /// </summary>
        /// <param name="numberOfBytes"></param>
        /// <returns>True if we going to have an exception while reading next numberOfBytes bytes from the stream</returns>
        public virtual bool IsCloserToEnd(long numberOfBytes)
        {
            return false;
        }
    }
}
