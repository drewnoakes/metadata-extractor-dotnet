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
        private const int DefaultChunkLength = 4 * 1024;

        [NotNull]
        private readonly Stream _stream;
        private readonly int _chunkLength;
        private readonly Dictionary<int, byte[]> _chunks;
        private int _maxChunkLoaded = -1;
        private int _streamLength = -1;
        private readonly bool _contiguousBufferMode;

        public IndexedCapturingReader([NotNull] Stream stream, int chunkLength = DefaultChunkLength, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            if (chunkLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkLength), "Must be greater than zero.");

            _chunkLength = chunkLength;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                // For some reason, FileStreams are faster in contiguous mode. Since this is such a a commont case, we
                // specifically check for it.
                if (_stream is FileStream)
                {
                    _contiguousBufferMode = true;
                }
                else
                {
                    // If the stream is both seekable and has a length, switch to non-contiguous buffering mode. This
                    // will use Seek operations to access data that is far beyond the reach of what has been buffered,
                    // rather than reading the entire file into memory in this case.
                    _contiguousBufferMode = !(_stream.Length > 0 && _stream.CanSeek);
                }
            }
            catch (NotSupportedException)
            {
                // Streams that don't support the Length property have to be handled in contiguous mode.
                _contiguousBufferMode = true;
            }

            if (!_contiguousBufferMode)
            {
                // If we know the length of the stream ahead of time, we can allocate a Dictionary with enough slots
                // for all the chunks. We 2X it to try to avoid hash collisions.
                var chunksCapacity = 2 * (_stream.Length / chunkLength);
                _chunks = new Dictionary<int, byte[]>((int) chunksCapacity);
            }
            else
            {
                _chunks = new Dictionary<int, byte[]>();
            }
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
                if (_contiguousBufferMode)
                {
                    if (_streamLength != -1)
                    {
                        IsValidIndex(int.MaxValue, 1);
                    }
                    return _streamLength;
                }

                return _stream.Length;
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

                // TODO test that can continue using an instance of this type after this exception
                throw new BufferBoundsException(ToUnshiftedOffset(index), bytesRequested, _streamLength);
            }
        }

        /// <summary>
        /// Helper method for GetChunk. This will load the next chunk of data from the input stream. If non contiguous
        /// buffering mode is being used, this method relies on the called (GetChunk) to set the stream's position
        /// correctly. In contiguous buffer mode, this will simply be the next chunk in sequence (the stream's Position
        /// field will just advance monotonically).
        /// </summary>
        /// <returns></returns>
        private byte[] LoadNextChunk()
        {
            var chunk = new byte[_chunkLength];
            var totalBytesRead = 0;

            while (totalBytesRead != _chunkLength)
            {
                var bytesRead = _stream.Read(chunk, totalBytesRead, _chunkLength - totalBytesRead);
                totalBytesRead += bytesRead;
                
                // if no bytes were read at all, we've reached the end of the file.
                if (bytesRead == 0 && totalBytesRead == 0)
                {
                    return null;
                }
                
                // If this read didn't produce any bytes, but a previous read did, we've hit the end of the file, so
                // shrink the chunk down to the number of bytes we actually have.
                if (bytesRead == 0)
                {
                    var shrunkChunk = new byte[totalBytesRead];
                    Buffer.BlockCopy(chunk, 0, shrunkChunk, 0, totalBytesRead);
                    return shrunkChunk;
                }
            }

            return chunk;
        }

        // GetChunk is substantially slower for random accesses owing to needing to use a Dictionary, rather than a
        // List. However, the typical access pattern isn't very random at all -- you generally read a whole series of
        // bytes from the same chunk. So we just cache the last chunk that was read and return that directly if it's
        // requested again. This is about 15% faster than going straight to the Dictionary.
        private int _lastChunkIdx = -1;
        private byte[] _lastChunkData = null;

        /// <summary>
        /// Load the data for the given chunk (if necessary), and return it. Chunks are identified by their index,
        /// which is their start offset divided by the chunk length. eg: offset 10 will typically refer to chunk
        /// index 0. See DoGetChunk() for implementation -- this function adds simple memoization.
        /// </summary>
        /// <param name="chunkIndex">The index of the chunk to get</param>
        private byte[] GetChunk(int chunkIndex)
        {
            if (chunkIndex == _lastChunkIdx)
            {
                return _lastChunkData;
            }
            
            var result = DoGetChunk(chunkIndex);
            _lastChunkIdx = chunkIndex;
            _lastChunkData = result;

            return result;
        }
        
        private byte[] DoGetChunk(int chunkIndex)
        {
            byte[] result;
            if (_chunks.TryGetValue(chunkIndex, out result))
            {
                return result;
            }

            if (!_contiguousBufferMode)
            {
                var chunkStart = chunkIndex * _chunkLength;
                
                // Often we will be reading long contiguous blocks, even in non-contiguous mode. Don't issue Seeks in
                // that case, so as to avoid unnecessary syscalls.
                if (chunkStart != _stream.Position)
                {
                    _stream.Seek(chunkStart, SeekOrigin.Begin);
                }
                
                var nextChunk = LoadNextChunk();
                if (nextChunk != null)
                {
                    _chunks[chunkIndex] = nextChunk;
                    var newStreamLen = (chunkIndex * _chunkLength) + nextChunk.Length;
                    _streamLength = newStreamLen > _streamLength ? newStreamLen : _streamLength;
                }

                return nextChunk;
            }

            byte[] curChunk = null;
            while (_maxChunkLoaded < chunkIndex)
            {
                var curChunkIdx = _maxChunkLoaded + 1;
                curChunk = LoadNextChunk();
                if (curChunk != null)
                {
                    _chunks[curChunkIdx] = curChunk;
                    var newStreamLen = (curChunkIdx * _chunkLength) + curChunk.Length;
                    _streamLength = newStreamLen > _streamLength ? newStreamLen : _streamLength;
                }
                else
                {
                    return null;
                }

                _maxChunkLoaded = curChunkIdx;
            }

            return curChunk;
        }

        protected override bool IsValidIndex(int index, int bytesRequested)
        {
            if (index < 0 || bytesRequested < 0)
                return false;

            var endIndexLong = (long)index + bytesRequested - 1;
            if (endIndexLong > int.MaxValue)
                return false;

            if (!_contiguousBufferMode)
            {
                return endIndexLong < _stream.Length;
            }
            
            var endIndex = (int)endIndexLong;

            var chunkIndex = endIndex / _chunkLength;

            var endChunk = GetChunk(chunkIndex);
            if (endChunk == null)
            {
                return false;
            }

            return endChunk.Length > (endIndex % _chunkLength);
        }

        public override int ToUnshiftedOffset(int localOffset) => localOffset;

        public override byte GetByte(int index)
        {
            ValidateIndex(index, 1);

            var chunkIndex = index / _chunkLength;
            var innerIndex = index % _chunkLength;
            var chunk = GetChunk(chunkIndex);
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
                var chunk = GetChunk(fromChunkIndex);
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
