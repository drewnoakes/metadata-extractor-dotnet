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

using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class SequentialReader
    {
        private bool _isMotorolaByteOrder = true;

        // TODO review whether the masks are needed (in both this and RandomAccessReader)
        /// <summary>Gets the next byte in the sequence.</summary>
        /// <returns>The read byte value</returns>
        /// <exception cref="System.IO.IOException"/>
        protected abstract sbyte GetByte();

        /// <summary>Returns the required number of bytes from the sequence.</summary>
        /// <param name="count">The number of bytes to be returned</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public abstract sbyte[] GetBytes(int count);

        /// <summary>Skips forward in the sequence.</summary>
        /// <remarks>
        /// Skips forward in the sequence. If the sequence ends, an
        /// <see cref="EofException"/>
        /// is thrown.
        /// </remarks>
        /// <param name="n">the number of byte to skip. Must be zero or greater.</param>
        /// <exception cref="EofException">the end of the sequence is reached.</exception>
        /// <exception cref="System.IO.IOException">an error occurred reading from the underlying source.</exception>
        public abstract void Skip(long n);

        /// <summary>Skips forward in the sequence, returning a boolean indicating whether the skip succeeded, or whether the sequence ended.</summary>
        /// <param name="n">the number of byte to skip. Must be zero or greater.</param>
        /// <returns>a boolean indicating whether the skip succeeded, or whether the sequence ended.</returns>
        /// <exception cref="System.IO.IOException">an error occurred reading from the underlying source.</exception>
        public abstract bool TrySkip(long n);

        /// <summary>Sets the endianness of this reader.</summary>
        /// <remarks>
        /// Sets the endianness of this reader.
        /// <list type="bullet">
        /// <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        /// <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        /// <param name="motorolaByteOrder"><c>true</c> for Motorola/big endian, <c>false</c> for Intel/little endian</param>
        public virtual void SetMotorolaByteOrder(bool motorolaByteOrder)
        {
            _isMotorolaByteOrder = motorolaByteOrder;
        }

        /// <summary>Gets the endianness of this reader.</summary>
        /// <remarks>
        /// Gets the endianness of this reader.
        /// <list type="bullet">
        /// <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        /// <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        public virtual bool IsMotorolaByteOrder()
        {
            return _isMotorolaByteOrder;
        }

        /// <summary>Returns an unsigned 8-bit int calculated from the next byte of the sequence.</summary>
        /// <returns>the 8 bit int value, between 0 and 255</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual short GetUInt8()
        {
            return (short)(GetByte() & unchecked(0xFF));
        }

        /// <summary>Returns a signed 8-bit int calculated from the next byte the sequence.</summary>
        /// <returns>the 8 bit int value, between 0x00 and 0xFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual sbyte GetInt8()
        {
            return GetByte();
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual int GetUInt16()
        {
            if (_isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (GetByte() << 8 & unchecked(0xFF00)) | (GetByte() & unchecked(0xFF));
            }
            // Intel ordering - LSB first
            return (GetByte() & unchecked(0xFF)) | (GetByte() << 8 & unchecked(0xFF00));
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public virtual short GetInt16()
        {
            if (_isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (short)((GetByte() << 8 & unchecked((short)(0xFF00))) | (GetByte() & 0xFF));
            }
            // Intel ordering - LSB first
            return (short)((GetByte() & 0xFF) | (GetByte() << 8 & unchecked((short)(0xFF00))));
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public virtual long GetUInt32()
        {
            if (_isMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return (((long)GetByte()) << 24 & unchecked(0xFF000000L)) | (((long)GetByte()) << 16 & unchecked(0xFF0000L)) | (((long)GetByte()) << 8 & unchecked(0xFF00L)) | (GetByte() & unchecked(0xFFL));
            }
            // Intel ordering - LSB first (little endian)
            return (GetByte() & unchecked(0xFFL)) | (((long)GetByte()) << 8 & unchecked(0xFF00L)) | (((long)GetByte()) << 16 & unchecked(0xFF0000L)) | (((long)GetByte()) << 24 & unchecked(0xFF000000L));
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data.</summary>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public virtual int GetInt32()
        {
            if (_isMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return (GetByte() << 24 & unchecked((int)(0xFF000000))) | (GetByte() << 16 & unchecked(0xFF0000)) | (GetByte() << 8 & unchecked(0xFF00)) | (GetByte() & unchecked(0xFF));
            }
            // Intel ordering - LSB first (little endian)
            return (GetByte() & unchecked(0xFF)) | (GetByte() << 8 & unchecked(0xFF00)) | (GetByte() << 16 & unchecked(0xFF0000)) | (GetByte() << 24 & unchecked((int)(0xFF000000)));
        }

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public virtual long GetInt64()
        {
            if (_isMotorolaByteOrder)
            {
                // Motorola - MSB first
                return ((long)GetByte() << 56 & unchecked((long)(0xFF00000000000000L))) | ((long)GetByte() << 48 & unchecked(0xFF000000000000L)) | ((long)GetByte() << 40 & unchecked(0xFF0000000000L)) | ((long)GetByte() << 32 & unchecked(0xFF00000000L)) | ((long)GetByte() << 24 & unchecked(0xFF000000L)) | ((long)GetByte() << 16 & unchecked(0xFF0000L)) | ((long)GetByte() << 8 & unchecked(0xFF00L)) | (GetByte() & unchecked(0xFFL));
            }
            // Intel ordering - LSB first
            return (GetByte() & unchecked(0xFFL)) | ((long)GetByte() << 8 & unchecked(0xFF00L)) | ((long)GetByte() << 16 & unchecked(0xFF0000L)) | ((long)GetByte() << 24 & unchecked(0xFF000000L)) | ((long)GetByte()
                                                                                                                                                                                                                                             << 32 & unchecked(0xFF00000000L)) | ((long)GetByte() << 40 & unchecked(0xFF0000000000L)) | ((long)GetByte() << 48 & unchecked(0xFF000000000000L)) | ((long)GetByte() << 56 & unchecked((long)(0xFF00000000000000L)));
        }

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// Gets a s15.16 fixed point float from the buffer.
        /// <para>
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public virtual float GetS15Fixed16()
        {
            if (_isMotorolaByteOrder)
            {
                float res = (GetByte() & unchecked(0xFF)) << 8 | (GetByte() & unchecked(0xFF));
                int d = (GetByte() & unchecked(0xFF)) << 8 | (GetByte() & unchecked(0xFF));
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                int d = (GetByte() & unchecked(0xFF)) | (GetByte() & unchecked(0xFF)) << 8;
                float res = (GetByte() & unchecked(0xFF)) | (GetByte() & unchecked(0xFF)) << 8;
                return (float)(res + d / 65536.0);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual float GetFloat32()
        {
            return Extensions.IntBitsToFloat(GetInt32());
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual double GetDouble64()
        {
            return Extensions.LongBitsToDouble(GetInt64());
        }

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public virtual string GetString(int bytesRequested)
        {
            return Runtime.GetStringForBytes(GetBytes(bytesRequested));
        }

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public virtual string GetString(int bytesRequested, string charset)
        {
            sbyte[] bytes = GetBytes(bytesRequested);
            try
            {
                return Runtime.GetStringForBytes(bytes, charset);
            }
            catch (UnsupportedEncodingException)
            {
                return Runtime.GetStringForBytes(bytes);
            }
        }

        /// <summary>Creates a String from the stream, ending where <c>byte=='\0'</c> or where <c>length==maxLength</c>.</summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <returns>The read string.</returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public virtual string GetNullTerminatedString(int maxLengthBytes)
        {
            // NOTE currently only really suited to single-byte character strings
            sbyte[] bytes = new sbyte[maxLengthBytes];
            // Count the number of non-null bytes
            int length = 0;
            while (length < bytes.Length && (bytes[length] = GetByte()) != '\0')
            {
                length++;
            }
            return Runtime.GetStringForBytes(bytes, 0, length);
        }
    }
}
