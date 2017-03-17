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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace MetadataExtractor.IO
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IndexedCapturingReader : IndexedReader
    {
        private const int DefaultChunkLength = 2 * 1024;

        [NotNull]
        private readonly Stream _stream;
        private readonly int _chunkLength;
        private readonly List<byte[]> _chunks = new List<byte[]>();
        private bool _isStreamFinished;
        private int _streamLength;

        public IndexedCapturingReader([NotNull] Stream stream, int chunkLength = DefaultChunkLength, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            if (chunkLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkLength), "Must be greater than zero.");

            _chunkLength = chunkLength;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>Reads to the end of the stream, in order to determine the total number of bytes.</summary>
        /// <remarks>In general, this is not a good idea for this implementation of <see cref="IndexedReader"/>.</remarks>
        /// <value>the length of the data source, in bytes.</value>
        /// <exception cref="BufferBoundsException"/>
        public override long Length
        {
            get
            {
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

        protected override bool IsValidIndex(int index, int bytesRequested)
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

            // TODO test loading several chunks for a single request
            while (chunkIndex >= _chunks.Count)
            {
                Debug.Assert(!_isStreamFinished);

                var chunk = new byte[_chunkLength];
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

        public override int ToUnshiftedOffset(int localOffset) => localOffset;

        public override byte GetByte(int index)
        {
            ValidateIndex(index, 1);

            var chunkIndex = index / _chunkLength;
            var innerIndex = index % _chunkLength;
            var chunk = _chunks[chunkIndex];
            return chunk[innerIndex];
        }

        public override byte[] GetBytes(int index, int count)
        {
            ValidateIndex(index, count);

            var bytes = new byte[count];
            var remaining = count;
            var fromIndex = index;
            var toIndex = 0;
            while (remaining != 0)
            {
                var fromChunkIndex = fromIndex / _chunkLength;
                var fromInnerIndex = fromIndex % _chunkLength;
                var length = Math.Min(remaining, _chunkLength - fromInnerIndex);
                var chunk = _chunks[fromChunkIndex];
                Array.Copy(chunk, fromInnerIndex, bytes, toIndex, length);
                remaining -= length;
                fromIndex += length;
                toIndex += length;
            }
            return bytes;
        }

        public override IndexedReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? (IndexedReader)this : new ShiftedIndexedCapturingReader(this, 0, isMotorolaByteOrder);

        public override IndexedReader WithShiftedBaseOffset(int shift) => shift == 0 ? (IndexedReader)this : new ShiftedIndexedCapturingReader(this, shift, IsMotorolaByteOrder);

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

            public override byte GetByte(int index) => _baseReader.GetByte(_baseOffset + index);

            public override byte[] GetBytes(int index, int count) => _baseReader.GetBytes(_baseOffset + index, count);

            protected override void ValidateIndex(int index, int bytesRequested) => _baseReader.ValidateIndex(index + _baseOffset, bytesRequested);

            protected override bool IsValidIndex(int index, int bytesRequested) => _baseReader.IsValidIndex(index + _baseOffset, bytesRequested);

            public override long Length => _baseReader.Length - _baseOffset;
        }
    }
}
