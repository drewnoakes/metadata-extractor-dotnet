// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace MetadataExtractor.IO
{
    /// <summary>
    /// Provides methods to read data types from a <see cref="Stream"/> by indexing into the data.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class IndexedSeekingReader : IndexedReader
    {
        private readonly Stream _stream;

        private readonly int _baseOffset;

        public IndexedSeekingReader(Stream stream, int baseOffset = 0, bool isMotorolaByteOrder = true)
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
