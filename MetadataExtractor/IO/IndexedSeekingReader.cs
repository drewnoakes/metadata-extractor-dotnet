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
    /// <summary>
    /// Provides methods to read data types from a <see cref="Stream"/> by indexing into the data.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class IndexedSeekingReader : IndexedReader
    {
        [NotNull]
        private readonly Stream _stream;

        private readonly int _baseOffset;

        public IndexedSeekingReader([NotNull] Stream stream, int baseOffset = 0, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek)
                throw new ArgumentException("Must be capable of seeking.", nameof(stream));
            if (baseOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(baseOffset), "Must be zero or greater.");

            var actualLength = stream.Length;
            var availableLength = actualLength - baseOffset;

            if (availableLength < 0)
                throw new ArgumentOutOfRangeException(nameof(baseOffset), "Cannot be greater than the stream's length.");

            _stream = stream;
            _baseOffset = baseOffset;
            Length = availableLength;
        }

        public override IndexedReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new IndexedSeekingReader(_stream, _baseOffset, isMotorolaByteOrder);

        public override IndexedReader WithShiftedBaseOffset(int shift) => shift == 0 ? this : new IndexedSeekingReader(_stream, _baseOffset + shift, IsMotorolaByteOrder);

        public override int ToUnshiftedOffset(int localOffset) => localOffset + _baseOffset;

        public override long Length { get; }

        public override byte GetByte(int index)
        {
            ValidateIndex(index, 1);

            if (index + _baseOffset != _stream.Position)
                Seek(index);

            var b = _stream.ReadByte();

            if (b < 0)
                throw new BufferBoundsException("Unexpected end of file encountered.");

            return unchecked((byte)b);
        }

        public override byte[] GetBytes(int index, int count)
        {
            ValidateIndex(index, count);

            if (index + _baseOffset != _stream.Position)
                Seek(index);

            var bytes = new byte[count];
            var bytesRead = _stream.Read(bytes, 0, count);

            if (bytesRead != count)
                throw new BufferBoundsException("Unexpected end of file encountered.");

            return bytes;
        }

        private void Seek(int index)
        {
            var streamIndex = index + _baseOffset;
            if (streamIndex == _stream.Position)
                return;

            _stream.Seek(streamIndex, SeekOrigin.Begin);
        }

        protected override bool IsValidIndex(int index, int bytesRequested)
        {
            return
                bytesRequested >= 0 &&
                index >= 0 &&
                index + (long)bytesRequested - 1L < Length;
        }

        protected override void ValidateIndex(int index, int bytesRequested)
        {
            if (!IsValidIndex(index, bytesRequested))
                throw new BufferBoundsException(ToUnshiftedOffset(index), bytesRequested, _stream.Length);
        }
    }
}
