/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using JetBrains.Annotations;

namespace Com.Drew.Lang
{
    /// <summary>
    /// Provides methods to read specific values from a byte array, with a consistent, checked exception structure for
    /// issues.
    /// </summary>
    /// <remarks>
    /// Provides methods to read specific values from a byte array, with a consistent, checked exception structure for
    /// issues.
    /// <para />
    /// By default, the reader operates with Motorola byte order (big endianness).  This can be changed by calling
    /// <c>setMotorolaByteOrder(boolean)</c>.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ByteArrayReader : IndexedReader
    {
        [NotNull]
        private readonly byte[] _buffer;

        public ByteArrayReader([NotNull] byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            _buffer = buffer;
        }

        public override long GetLength()
        {
            return _buffer.Length;
        }

        /// <exception cref="System.IO.IOException"/>
        protected override byte GetByte(int index)
        {
            return _buffer[index];
        }

        /// <exception cref="System.IO.IOException"/>
        protected override void ValidateIndex(int index, int bytesRequested)
        {
            if (!IsValidIndex(index, bytesRequested))
            {
                throw new BufferBoundsException(index, bytesRequested, _buffer.Length);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        protected override bool IsValidIndex(int index, int bytesRequested)
        {
            return bytesRequested >= 0 && index >= 0 && index + (long)bytesRequested - 1L < _buffer.Length;
        }

        /// <exception cref="System.IO.IOException"/>
        public override byte[] GetBytes(int index, int count)
        {
            ValidateIndex(index, count);
            byte[] bytes = new byte[count];
            Array.Copy(_buffer, index, bytes, 0, count);
            return bytes;
        }
    }
}
