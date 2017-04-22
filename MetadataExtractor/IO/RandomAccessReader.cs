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
using JetBrains.Annotations;

namespace MetadataExtractor.IO
{
    /// <summary>Wrapper lass for reading randomly through a sequence of data encoded in a byte stream.</summary>
    /// <remarks>
    /// By default, the reader operates with Motorola byte order (big endianness).  This can be changed by via
    /// <see cref="IsMotorolaByteOrder"/>.
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class RandomAccessReader
    {
        [NotNull]
        private readonly Stream _stream;

        /// <summary>Get and set the byte order of this reader. <c>true</c> by default.</summary>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        ///   <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        /// <value><c>true</c> for Motorola/big endian, <c>false</c> for Intel/little endian</value>
        public bool IsMotorolaByteOrder { get; }

        public bool CanSeek => _stream.CanSeek;

        public long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public long Length => _stream.Length;

        public RandomAccessReader([NotNull] Stream stream, bool isMotorolaByteOrder = true, long initialOffset = 0)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            IsMotorolaByteOrder = isMotorolaByteOrder;

            if (initialOffset != 0)
                _stream.Position = initialOffset;
        }

        public int ReadByte() => _stream.ReadByte();

        public int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        /*public byte GetByte()
        {
            var value = _stream.ReadByte();
            return unchecked((byte)value);
        }*/

        public RandomAccessReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new RandomAccessReader(_stream, isMotorolaByteOrder, _stream.Position);


        public ushort GetUInt16()
        {
            var b1 = _stream.ReadByte();
            var b2 = _stream.ReadByte();
            if (b2 == -1)
                throw new IOException("Unexpected end of stream.");

            return GetUInt16(b1, b2);
        }

        public ushort GetUInt16(int b1, int b2)
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return unchecked((ushort)(b1 << 8 | b2));
            }
            // Intel ordering - LSB first
            return unchecked((ushort)(b1 | b2 << 8));
        }

    }
}
