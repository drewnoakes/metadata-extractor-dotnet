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

using System;
using System.IO;
using System.Text;

using JetBrains.Annotations;

namespace MetadataExtractor.IO
{
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ReaderInfo
    {
        // this flag is compared to index inputs and indicates sequential access
        private const int SequentialFlag = int.MinValue;

        private RandomAccessStream p_ras = null;
        private long p_length = -1;

        public ReaderInfo(RandomAccessStream parent, long startPosition = 0, long localPosition = 0, long length = -1, bool isMotorolaByteOrder = true)
        {
            p_ras = parent;
            StartPosition = startPosition;
            LocalPosition = localPosition;
            p_length = length;

            IsMotorolaByteOrder = isMotorolaByteOrder;
        }

        private long GlobalPosition => StartPosition + LocalPosition;
        public long StartPosition { get; private set; }
        public long LocalPosition { get; private set; }

        public long Length
        {
            get
            {
                return (p_length != -1) ? p_length : (p_ras.Length == long.MaxValue ? long.MaxValue : p_ras.Length - StartPosition);
            }
        }

        /// <summary>Get and set the byte order of this reader. <c>true</c> by default.</summary>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        ///   <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        /// <value><c>true</c> for Motorola/big endian, <c>false</c> for Intel/little endian</value>
        public bool IsMotorolaByteOrder { get; set; }

        /// <summary>
        /// Creates a new <see cref="ReaderInfo"/> with the current properties of this reader
        /// </summary>
        /// <returns></returns>
        public ReaderInfo Clone() => Clone(0, -1, true);
        public ReaderInfo Clone(bool useByteOrder) => Clone(0, useByteOrder);
        public ReaderInfo Clone(long length) => Clone(0, length, true);
        public ReaderInfo Clone(long offset, long length) => Clone(offset, length, true);
        public ReaderInfo Clone(long offset, bool useByteOrder) => Clone(offset, -1, useByteOrder);
        //public ReaderInfo Clone(long length = -1, long offset = 0, bool useByteOrder = true)
        public ReaderInfo Clone(long offset, long length, bool useByteOrder)
        {
            //return p_ras.CreateReader(GlobalPosition + offset, (length > -1 ? length : Length), useByteOrder ? IsMotorolaByteOrder : !IsMotorolaByteOrder);
            return p_ras.CreateReader(GlobalPosition + offset, (length > -1 ? length : Length - offset), useByteOrder ? IsMotorolaByteOrder : !IsMotorolaByteOrder);
        }

        /// <summary>Seeks forward or backward in the sequence.</summary>
        /// <remarks>
        /// Skips forward in the sequence. If the sequence ends, an <see cref="IOException"/> is thrown.
        /// </remarks>
        /// <param name="offset">the number of bytes to seek, in either direction.</param>
        /// <exception cref="IOException">the end of the sequence is reached.</exception>
        /// <exception cref="IOException">an error occurred reading from the underlying source.</exception>
        public void Seek(long offset)
        {
            if (offset + LocalPosition < 0)
                offset = -LocalPosition;

            p_ras.Seek(LocalPosition + offset);
            LocalPosition += offset;
        }

        /// <summary>Seeks forward or backward in the sequence, returning a boolean indicating whether the seek succeeded, or whether the sequence ended.</summary>
        /// <param name="n">the number of bytes to seek, in either direction.</param>
        /// <returns>a boolean indicating whether the skip succeeded, or whether the sequence ended.</returns>
        /// <exception cref="IOException">an error occurred reading from the underlying source.</exception>
        public bool TrySeek(long n)
        {
            try
            {
                Seek(n);
                return true;
            }
            catch (IOException)
            {
                // Stream ended, or error reading from underlying source
                return false;
            }
        }

        /// <summary>Retrieves bytes, writing them into a caller-provided buffer.</summary>
        /// <remarks>SequentialFlag as index indicates this call should read sequentially</remarks>
        /// <param name="buffer">array to write bytes to.</param>
        /// <param name="offset">starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="count">number of bytes to be written.</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public int Read(byte[] buffer, int offset, int count) => Read(buffer, offset, SequentialFlag, count);

        /// <summary>Retrieves bytes, writing them into a caller-provided buffer.</summary>
        /// <remarks>Sequential access to the next byte is indicated by setting index to SequntialFlag</remarks>
        /// <param name="buffer">array to write bytes to.</param>
        /// <param name="offset">starting position within <paramref name="buffer"/> to write to.</param>
        /// <param name="index">position within the data buffer to read byte.</param>
        /// <param name="count">number of bytes to be written.</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="BufferBoundsException"/>
        /// <exception cref="IOException"/>
        public int Read(byte[] buffer, int offset, long index, int count)
        {
            int read = -1;
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if (!isSeq)
                readat = StartPosition + index;

            read = p_ras.Read(readat, buffer, offset, count, index == SequentialFlag);
            
            if (isSeq && read > 0)
                LocalPosition += read;

            return read;
        }

        /// <summary>
        /// Determine if the next bytes match the input pattern. Internal sequential variables are unaffected
        /// </summary>
        /// <param name="pattern">the byte pattern to match</param>
        /// <returns></returns>
        public bool StartsWith(byte[] pattern)
        {
            if (Length < pattern.Length)
                return false;

            var ret = true;
            int i = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (i = 0; i < pattern.Length; i++)
            {
                if (GetByte(i) != pattern[i])
                {
                    ret = false;
                    break;
                }
            }

            if (i > 0)
                Seek(-i);

            return ret;
        }

        /// <summary>Gets the byte value at the next sequential byte <c>index</c>.</summary>
        /// <returns>The read byte value</returns>
        /// <exception cref="BufferBoundsException">if the requested byte is beyond the end of the underlying data source</exception>
        /// <exception cref="IOException">if the byte is unable to be read</exception>
        public byte GetByte() => GetByte(SequentialFlag);

        /// <summary>Gets the byte value at the specified byte <c>index</c>.</summary>
        /// <param name="index">The index from which to read the byte</param>
        /// <returns>The read byte value</returns>
        /// <exception cref="BufferBoundsException">if the requested byte is beyond the end of the underlying data source</exception>
        /// <exception cref="IOException">if the byte is unable to be read</exception>
        public byte GetByte(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            var read = p_ras.GetByte(readat, isSeq);

            if (isSeq)
                LocalPosition++;

            return read;
        }

        /// <summary>Returns the required number of bytes sequentially from the underlying source.</summary>
        /// <param name="count">The number of bytes to be returned</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="BufferBoundsException">if the requested bytes extend beyond the end of the underlying data source</exception>
        /// <exception cref="System.IO.IOException">if the byte is unable to be read</exception>
        public byte[] GetBytes(int count) => GetBytes(SequentialFlag, count);

        /// <summary>Returns the required number of bytes from the specified index from the underlying source.</summary>
        /// <param name="index">The index from which the bytes begins in the underlying source</param>
        /// <param name="count">The number of bytes to be returned</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="BufferBoundsException">if the requested bytes extend beyond the end of the underlying data source</exception>
        /// <exception cref="IOException">if the byte is unable to be read</exception>
        public byte[] GetBytes(long index, int count)
        {
            var bytes = new byte[count];
            Read(bytes, 0, index, count);

            return bytes;
        }

        /// <summary>Gets whether a bit at a specific index is set or not sequentially.</summary>
        /// <returns>true if the bit is set, otherwise false</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public bool GetBit() => GetBit(SequentialFlag);

        /// <summary>Gets whether a bit at a specific index is set or not.</summary>
        /// <param name="index">the number of bits at which to test</param>
        /// <returns>true if the bit is set, otherwise false</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public bool GetBit(int index)
        {
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            p_ras.ValidateIndex(byteIndex, 1, index == SequentialFlag);
            var b = GetByte(byteIndex);
            return ((b >> bitIndex) & 1) == 1;
        }

        /// <summary>Returns a signed 8-bit int calculated from one byte of data sequentially.</summary>
        /// <returns>the 8 bit signed byte value</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public sbyte GetSByte() => GetSByte(SequentialFlag);

        /// <summary>Returns a signed 8-bit int calculated from one byte of data at the specified index.</summary>
        /// <param name="index">position within the data buffer to read byte</param>
        /// <returns>the 8 bit signed byte value</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public sbyte GetSByte(long index)
        {
            return unchecked((sbyte)GetByte(index));
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public ushort GetUInt16() => GetUInt16(SequentialFlag);

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <param name="index">position within the data buffer to read byte</param>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public ushort GetUInt16(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 2, isSeq);

            if (isSeq)
                LocalPosition += 2;

            return p_ras.GetUInt16(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public ushort GetUInt16(int b1, int b2)
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return unchecked((ushort)(b1 << 8 | b2));
            }
            // Intel ordering - LSB first
            return unchecked((ushort)(b2 << 8 | b1));
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public short GetInt16() => GetInt16(SequentialFlag);

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public short GetInt16(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 2, isSeq);

            if (isSeq)
                LocalPosition += sizeof(short);

            return p_ras.GetInt16(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Get a 24-bit unsigned integer from the buffer sequentially, returning it as an int.</summary>
        /// <returns>the unsigned 24-bit int value as a long, between 0x00000000 and 0x00FFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public int GetInt24() => GetInt24(SequentialFlag);

        /// <summary>Get a 24-bit unsigned integer from the buffer, returning it as an int.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the unsigned 24-bit int value as a long, between 0x00000000 and 0x00FFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public int GetInt24(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 3, isSeq);

            if(isSeq)
                LocalPosition += 3; // advance the sequential position

            return p_ras.GetInt24(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer sequentially, returning it as a long.</summary>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public uint GetUInt32() => GetUInt32(SequentialFlag);

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public uint GetUInt32(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 4, isSeq);

            if(isSeq)
                LocalPosition += 4; // advance the sequential position

            return p_ras.GetUInt32(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data sequentially.</summary>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public int GetInt32() => GetInt32(SequentialFlag);

        /// <summary>Returns a signed 32-bit integer from four bytes of data at the specified index the buffer.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public int GetInt32(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 4, isSeq);

            if (isSeq)
                LocalPosition += 4; // advance the sequential position

            return p_ras.GetInt32(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Get a signed 64-bit integer from the buffer sequentially.</summary>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public long GetInt64() => GetInt64(SequentialFlag);

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <param name="index">position within the data buffer to read first byte</param>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public long GetInt64(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;
            
            p_ras.ValidateIndex(readat, 8, isSeq);

            if (isSeq)
                LocalPosition += 8; // advance the sequential position

            return p_ras.GetInt64(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Get an usigned 64-bit integer from the buffer sequentially.</summary>
        /// <returns>the unsigned 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public ulong GetUInt64() => GetUInt64(SequentialFlag);

        /// <summary>Get an usigned 64-bit integer from the buffer.</summary>
        /// <returns>the unsigned 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public ulong GetUInt64(long index)
        {
            var isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if (!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 8, isSeq);

            if (isSeq)
                LocalPosition += 8; // advance the sequential position

            return p_ras.GetUInt64(readat, IsMotorolaByteOrder, isSeq);
        }

        /// <summary>Gets a s15.16 fixed point float from the buffer sequentially.</summary>
        /// <remarks>
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public float GetS15Fixed16() => GetS15Fixed16(SequentialFlag);

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request, or index is negative</exception>
        public float GetS15Fixed16(long index)
        {
            bool isSeq = index == SequentialFlag;

            long readat = GlobalPosition;
            if(!isSeq)
                readat = StartPosition + index;

            p_ras.ValidateIndex(readat, 4, isSeq);

            if (isSeq)
                LocalPosition += 4; // advance the sequential position

            return p_ras.GetS15Fixed16(readat, IsMotorolaByteOrder, isSeq);
        }


        public float GetFloat32() => GetFloat32(SequentialFlag);

        /// <exception cref="System.IO.IOException"/>
        public float GetFloat32(long index) => BitConverter.ToSingle(BitConverter.GetBytes(GetInt32(index)), 0);

        public double GetDouble64() => GetDouble64(SequentialFlag);

        /// <exception cref="System.IO.IOException"/>
        public double GetDouble64(long index) => BitConverter.Int64BitsToDouble(GetInt64(index));


        [NotNull]
        public string GetString(int bytesRequested, [NotNull] Encoding encoding) => GetString(SequentialFlag, bytesRequested, encoding);

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public string GetString(long index, int bytesRequested, [NotNull] Encoding encoding)
        {
            var bytes = GetBytes(index, bytesRequested);
            return encoding.GetString(bytes, 0, bytes.Length);
        }

        public StringValue GetStringValue(int bytesRequested) => GetStringValue(bytesRequested, null);
        public StringValue GetStringValue(int bytesRequested, Encoding encoding)
        {
            return new StringValue(GetBytes(bytesRequested), encoding);
        }

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <returns>The read <see cref="string"/></returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public string GetNullTerminatedString(int maxLengthBytes) => GetNullTerminatedString(SequentialFlag, maxLengthBytes);

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="index">The index within the buffer at which to start reading the string.</param>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <returns>The read <see cref="string"/></returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public string GetNullTerminatedString(int index, int maxLengthBytes) => GetNullTerminatedString(index, maxLengthBytes, null);

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
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public string GetNullTerminatedString(int index, int maxLengthBytes, Encoding encoding)
        {
            var bytes = GetNullTerminatedBytes(index, maxLengthBytes);

            return (encoding ?? Encoding.UTF8).GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <returns>The read <see cref="StringValue"/></returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public StringValue GetNullTerminatedStringValue(int maxLengthBytes) => GetNullTerminatedStringValue(SequentialFlag, maxLengthBytes);

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="index">The index within the buffer at which to start reading the string.</param>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <returns>The read <see cref="StringValue"/></returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public StringValue GetNullTerminatedStringValue(int index, int maxLengthBytes) => GetNullTerminatedStringValue(index, maxLengthBytes, null);

        /// <summary>
        /// Creates a string starting at the specified index, and ending where either <c>byte=='\0'</c> or
        /// <c>length==maxLength</c>.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <param name="encoding">An optional string encoding to use when interpreting bytes.</param>
        /// <returns>The read <see cref="StringValue"/></returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public StringValue GetNullTerminatedStringValue(int maxLengthBytes, Encoding encoding) => GetNullTerminatedStringValue(SequentialFlag, maxLengthBytes, encoding);

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
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        public StringValue GetNullTerminatedStringValue(int index, int maxLengthBytes, Encoding encoding)
        {
            var bytes = GetNullTerminatedBytes(index, maxLengthBytes);

            return new StringValue(bytes, encoding);
        }

        /// <summary>
        /// Returns the sequence of bytes punctuated by a <c>\0</c> value.
        /// </summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a <c>\0</c> byte is not reached within this limit,
        /// the returned array will be <paramref name="maxLengthBytes"/> long.
        /// </param>
        /// <returns>The read byte array.</returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public byte[] GetNullTerminatedBytes(int maxLengthBytes) => GetNullTerminatedBytes(SequentialFlag, maxLengthBytes);

        /// <summary>
        /// Returns the sequence of bytes punctuated by a <c>\0</c> value.
        /// </summary>
        /// <param name="index">The index to start reading from.</param>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a <c>\0</c> byte is not reached within this limit,
        /// the returned array will be <paramref name="maxLengthBytes"/> long.
        /// </param>
        /// <returns>The read byte array.</returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public byte[] GetNullTerminatedBytes(int index, int maxLengthBytes)
        {
            var isSeq = index == SequentialFlag;

            var buffer = !isSeq ? GetBytes(index, maxLengthBytes) : new byte[maxLengthBytes];

            // Count the number of non-null bytes
            var length = 0;
            while (length < buffer.Length && (!isSeq ? buffer[length] : buffer[length] = GetByte()) != 0)
                length++;

            if (length == maxLengthBytes)
                return buffer;

            var bytes = new byte[length];
            if (length > 0)
                Array.Copy(buffer, bytes, length);
            return bytes;
        }

        /// <summary>Returns the bytes described by this particular reader</summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return GetBytes(0, (int)Length);
        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                if (LocalPosition == Length)
                    break;

                int ch = GetByte();
                if (ch == -1) break;
                if (ch == '\r' || ch == '\n')
                {
                    byte nextbyte = 0;
                    if(GlobalPosition + 1 < Length)
                        nextbyte = GetByte();
                    if (!(ch == '\r' && nextbyte == '\n'))
                        Seek(-1);

                    return sb.ToString();
                }
                sb.Append((char)ch);
            }
            if (sb.Length > 0) return sb.ToString();
            return null;
        }

        /// <summary>
        /// Returns true in case the sequence supports length checking and distance to the end of the stream is less then number of bytes in parameter.
        /// Otherwise false.
        /// </summary>
        /// <param name="numberOfBytes"></param>
        /// <returns>True if we are going to have an exception while reading next numberOfBytes bytes from the stream</returns>
        public bool IsCloserToEnd(long numberOfBytes)
        {
            return LocalPosition + numberOfBytes > Length;
        }

    }
}
