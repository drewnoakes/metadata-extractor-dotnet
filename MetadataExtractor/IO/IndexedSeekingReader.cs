#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.Diagnostics;
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

        private readonly long _length;

        private int _currentIndex;

        public IndexedSeekingReader([NotNull] Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();
            if (!stream.CanSeek)
                throw new ArgumentException("Must be capable of seeking.", "stream");

            _stream = stream;
            _length = _stream.Length;
        }

        public override long Length => _length;

        /// <exception cref="System.IO.IOException"/>
        public override byte GetByte(int index)
        {
            ValidateIndex(index, 1);
            if (index != _currentIndex)
                Seek(index);

            var b = _stream.ReadByte();
            if (b < 0)
                throw new BufferBoundsException("Unexpected end of file encountered.");

            Debug.Assert(b <= 0xff);
            _currentIndex++;
            return unchecked((byte)b);
        }

        /// <exception cref="System.IO.IOException"/>
        public override byte[] GetBytes(int index, int count)
        {
            ValidateIndex(index, count);
            if (index != _currentIndex)
                Seek(index);

            var bytes = new byte[count];
            var bytesRead = _stream.Read(bytes, 0, count);
            _currentIndex += bytesRead;

            if (bytesRead != count)
                throw new BufferBoundsException("Unexpected end of file encountered.");

            return bytes;
        }

        /// <exception cref="System.IO.IOException"/>
        private void Seek(int index)
        {
            if (index == _currentIndex)
                return;

            _stream.Seek(index, SeekOrigin.Begin);
            _currentIndex = index;
        }

        /// <exception cref="System.IO.IOException"/>
        protected override bool IsValidIndex(int index, int bytesRequested)
        {
            return bytesRequested >= 0 && index >= 0 && index + (long)bytesRequested - 1L < _length;
        }

        /// <exception cref="System.IO.IOException"/>
        protected override void ValidateIndex(int index, int bytesRequested)
        {
            if (!IsValidIndex(index, bytesRequested))
                throw new BufferBoundsException(index, bytesRequested, _length);
        }
    }
}
