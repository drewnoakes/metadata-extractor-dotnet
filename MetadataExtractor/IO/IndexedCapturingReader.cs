// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;

namespace MetadataExtractor.IO
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IndexedCapturingReader : IndexedReader, IDisposable
    {
        private const int DefaultChunkLength = 2 * 1024;

        private readonly Stream _stream;
        private readonly int _chunkLength;
        private readonly List<byte[]> _chunks = [];
        private bool _isStreamFinished;
        private int _streamLength;
        private bool _streamLengthThrewException;

        public IndexedCapturingReader(Stream stream, int chunkLength = DefaultChunkLength, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            if (chunkLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkLength), "Must be greater than zero.");

            _chunkLength = chunkLength;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        /// Returns the length of the data stream this reader is reading from.
        /// </summary>
        /// <remarks>
        /// If the underlying stream's <see cref="Stream.Length"/> property does not throw <see cref="NotSupportedException"/> then it can be used directly.
        /// However if it does throw, then this class has no alternative but to reads to the end of the stream in order to determine the total number of bytes.
        /// <para />
        /// In general, this is not a good idea for this implementation of <see cref="IndexedReader"/>.
        /// </remarks>
        /// <value>The length of the data source, in bytes.</value>
        /// <exception cref="BufferBoundsException"/>
        public override long Length
        {
            get
            {
                if (!_streamLengthThrewException)
                {
                    try
                    {
                        return _stream.Length;
                    }
                    catch (NotSupportedException)
                    {
                        _streamLengthThrewException = true;
                    }
                }

                IsValidIndex(int.MaxValue, 1);
                Debug.Assert(_isStreamFinished);
                return _streamLength;
            }
        }

        /// <summary>Ensures that the buffered bytes extend to cover the specified index. If not, an attempt is made
        /// to read to that point.</summary>
        /// <remarks>If the stream ends before the point is reached, a <see cref="BufferBoundsException"/> is raised.</remarks>
        /// <param name="index">the index from which the required bytes start</param>
        /// <param name="bytesRequested">the number of bytes which are required</param>
        /// <exception cref="BufferBoundsException">if the stream ends before the required number of bytes are acquired</exception>
        protected override void ValidateIndex(int index, int bytesRequested)
        {
            if (!IsValidIndex(index, bytesRequested))
            {
                if (index < 0)
                    throw new BufferBoundsException($"Attempt to read from buffer using a negative index ({index})");
                if (bytesRequested < 0)
                    throw new BufferBoundsException("Number of requested bytes must be zero or greater");
                if ((long)index + bytesRequested - 1 > int.MaxValue)
                    throw new BufferBoundsException($"Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: {index}, requested count: {bytesRequested})");

                Debug.Assert(_isStreamFinished);
                // TODO test that can continue using an instance of this type after this exception
                throw new BufferBoundsException(ToUnshiftedOffset(index), bytesRequested, _streamLength);
            }
        }

        private bool IsValidIndex(int index, int bytesRequested)
        {
            if (index < 0 || bytesRequested < 0)
                return false;

            var endIndexLong = (long)index + bytesRequested - 1;
            if (endIndexLong > int.MaxValue)
                return false;

            var endIndex = (int)endIndexLong;
            if (_isStreamFinished)
                return endIndex < _streamLength;

            var chunkIndex = endIndex / _chunkLength;

            while (chunkIndex >= _chunks.Count)
            {
                Debug.Assert(!_isStreamFinished);

                var chunk = ArrayPool<byte>.Shared.Rent(_chunkLength);
                var totalBytesRead = 0;
                while (!_isStreamFinished && totalBytesRead != _chunkLength)
                {
                    var bytesRead = _stream.Read(chunk, totalBytesRead, _chunkLength - totalBytesRead);

                    if (bytesRead == 0)
                    {
                        // the stream has ended, which may be ok
                        _isStreamFinished = true;
                        _streamLength = _chunks.Count * _chunkLength + totalBytesRead;
                        // check we have enough bytes for the requested index
                        if (endIndex >= _streamLength)
                        {
                            _chunks.Add(chunk);
                            return false;
                        }
                    }
                    else
                    {
                        totalBytesRead += bytesRead;
                    }
                }

                _chunks.Add(chunk);
            }

            return true;
        }

        public void Dispose()
        {
            foreach (var chunk in _chunks)
            {
                ArrayPool<byte>.Shared.Return(chunk);
            }
        }

        public override int ToUnshiftedOffset(int localOffset) => localOffset;

        private void GetPosition(int index, out int chunkIndex, out int innerIndex)
        {
#if NET462 || NETSTANDARD2_1
            chunkIndex = Math.DivRem(index, _chunkLength, out innerIndex);
#elif NET6_0_OR_GREATER
            (chunkIndex, innerIndex) = Math.DivRem(index, _chunkLength);
#else
            chunkIndex = index / _chunkLength;
            innerIndex = index % _chunkLength;
#endif
        }

        public override void GetBytes(int index, Span<byte> bytes)
        {
            ValidateIndex(index, bytes.Length);

            var remaining = bytes.Length;
            var fromIndex = index;
            var toIndex = 0;
            while (remaining != 0)
            {
                GetPosition(fromIndex, out int fromChunkIndex, out int fromInnerIndex);
                var length = Math.Min(remaining, _chunkLength - fromInnerIndex);
                var chunk = _chunks[fromChunkIndex];
                chunk.AsSpan().Slice(fromInnerIndex, length).CopyTo(bytes.Slice(toIndex, length));
                remaining -= length;
                fromIndex += length;
                toIndex += length;
            }
        }

        public override IndexedReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new ShiftedIndexedCapturingReader(this, 0, isMotorolaByteOrder);

        public override IndexedReader WithShiftedBaseOffset(int shift) => shift == 0 ? this : new ShiftedIndexedCapturingReader(this, shift, IsMotorolaByteOrder);

        private sealed class ShiftedIndexedCapturingReader : IndexedReader
        {
            private readonly IndexedCapturingReader _baseReader;
            private readonly int _baseOffset;

            public ShiftedIndexedCapturingReader(IndexedCapturingReader baseReader, int baseOffset, bool isMotorolaByteOrder)
                : base(isMotorolaByteOrder)
            {
                if (baseOffset < 0)
                    throw new ArgumentOutOfRangeException(nameof(baseOffset), "Must be zero or greater.");

                _baseReader = baseReader;
                _baseOffset = baseOffset;
            }

            public override IndexedReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new ShiftedIndexedCapturingReader(_baseReader, _baseOffset, isMotorolaByteOrder);

            public override IndexedReader WithShiftedBaseOffset(int shift) => shift == 0 ? this : new ShiftedIndexedCapturingReader(_baseReader, _baseOffset + shift, IsMotorolaByteOrder);

            public override int ToUnshiftedOffset(int localOffset) => localOffset + _baseOffset;

            public override void GetBytes(int index, Span<byte> bytes) => _baseReader.GetBytes(_baseOffset + index, bytes);

            protected override void ValidateIndex(int index, int bytesRequested) => _baseReader.ValidateIndex(index + _baseOffset, bytesRequested);

            public override long Length => _baseReader.Length - _baseOffset;
        }
    }
}
