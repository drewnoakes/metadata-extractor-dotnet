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
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SequentialByteArrayReader : SequentialReader
    {
        [NotNull]
        private readonly byte[] _bytes;

        private int _index;

        public override long Position => _index;

        public SequentialByteArrayReader([NotNull] byte[] bytes, int baseIndex = 0, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            _index = baseIndex;
        }

        public override byte GetByte()
        {
            if (_index >= _bytes.Length)
                throw new IOException("End of data reached.");

            return _bytes[_index++];
        }

        public override SequentialReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new SequentialByteArrayReader(_bytes, _index, isMotorolaByteOrder);

        public override byte[] GetBytes(int count)
        {
            if (_index + count > _bytes.Length)
                throw new IOException("End of data reached.");

            var bytes = new byte[count];
            Array.Copy(_bytes, _index, bytes, 0, count);
            _index += count;
            return bytes;
        }

        public override void GetBytes(byte[] buffer, int offset, int count)
        {
            if (_index + count > _bytes.Length)
                throw new IOException("End of data reached.");

            Array.Copy(_bytes, _index, buffer, offset, count);
            _index += count;
        }

        public override void Skip(long n)
        {
            if (n < 0)
                throw new ArgumentException("n must be zero or greater.");

            if (_index + n > _bytes.Length)
                throw new IOException("End of data reached.");

            _index += unchecked((int)n);
        }

        public override bool TrySkip(long n)
        {
            if (n < 0)
                throw new ArgumentException("n must be zero or greater.");

            _index += unchecked((int)n);

            if (_index > _bytes.Length)
            {
                _index = _bytes.Length;
                return false;
            }

            return true;
        }
    }
}
