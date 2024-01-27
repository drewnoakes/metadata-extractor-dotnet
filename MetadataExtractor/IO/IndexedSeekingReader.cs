// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
            if (stream is null)
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

#if !NETSTANDARD2_1
        private readonly byte[] _buffer = new byte[2048];
#endif

        public override void GetBytes(int index, Span<byte> bytes)
        {
            int count = bytes.Length;

            ValidateIndex(index, count);

            if (index + _baseOffset != _stream.Position)
                Seek(index);

            int totalBytesRead = 0;

            while (totalBytesRead != bytes.Length)
            {
                var target = bytes.Slice(totalBytesRead);
#if NETSTANDARD2_1
                var bytesRead = _stream.Read(target);
#else
                var len = Math.Min(bytes.Length - totalBytesRead, _buffer.Length);

                var bytesRead = _stream.Read(_buffer, 0, len);

                _buffer.AsSpan(0, len).CopyTo(target);
#endif
                if (bytesRead == 0)
                    throw new IOException("End of data reached.");

                totalBytesRead += bytesRead;

                Debug.Assert(totalBytesRead <= bytes.Length);
            }

        }

        private void Seek(int index)
        {
            var streamIndex = index + _baseOffset;
            if (streamIndex == _stream.Position)
                return;

            _stream.Seek(streamIndex, SeekOrigin.Begin);
        }

        private bool IsValidIndex(int index, int bytesRequested)
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
