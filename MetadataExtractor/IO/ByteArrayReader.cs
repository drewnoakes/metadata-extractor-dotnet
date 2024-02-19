// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.IO
{
    /// <summary>
    /// Reads values of various data types from a byte array, accessed by index.
    /// </summary>
    /// <remarks>
    /// By default, the reader operates with Motorola byte order (big endianness).  This can be changed by calling
    /// <see cref="IndexedReader.IsMotorolaByteOrder"/>.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ByteArrayReader : IndexedReader
    {
        private readonly byte[] _buffer;
        private readonly int _baseOffset;

        public ByteArrayReader(byte[] buffer, int baseOffset = 0, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            if (baseOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(baseOffset), "Must be zero or greater.");

            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _baseOffset = baseOffset;
        }

        public override IndexedReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new ByteArrayReader(_buffer, _baseOffset, isMotorolaByteOrder);

        public override IndexedReader WithShiftedBaseOffset(int shift) => shift == 0 ? this : new ByteArrayReader(_buffer, _baseOffset + shift, IsMotorolaByteOrder);

        public override int ToUnshiftedOffset(int localOffset) => localOffset + _baseOffset;

        public override long Length => _buffer.Length - _baseOffset;

        public override void GetBytes(int index, Span<byte> bytes)
        {
            ValidateIndex(index, bytes.Length);

            _buffer.AsSpan(index + _baseOffset, bytes.Length).CopyTo(bytes);
        }

        protected override void ValidateIndex(int index, int bytesRequested)
        {
            if (!IsValidIndex(index, bytesRequested))
                throw new BufferBoundsException(ToUnshiftedOffset(index), bytesRequested, _buffer.Length);
        }

        private bool IsValidIndex(int index, int bytesRequested)
        {
            return
                bytesRequested >= 0 &&
                index >= 0 &&
                index + (long)bytesRequested - 1L < Length;
        }
    }
}
