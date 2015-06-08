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
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SequentialByteArrayReader : SequentialReader
    {
        [NotNull]
        private readonly sbyte[] _bytes;

        private int _index;

        public SequentialByteArrayReader([NotNull] sbyte[] bytes, int baseIndex = 0)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException();
            }
            _bytes = bytes;
            _index = baseIndex;
        }

        /// <exception cref="System.IO.IOException"/>
        protected override sbyte GetByte()
        {
            if (_index >= _bytes.Length)
            {
                throw new EofException("End of data reached.");
            }
            return _bytes[_index++];
        }

        /// <exception cref="System.IO.IOException"/>
        public override sbyte[] GetBytes(int count)
        {
            if (_index + count > _bytes.Length)
            {
                throw new EofException("End of data reached.");
            }
            sbyte[] bytes = new sbyte[count];
            Array.Copy(_bytes, _index, bytes, 0, count);
            _index += count;
            return bytes;
        }

        /// <exception cref="System.IO.IOException"/>
        public override void Skip(long n)
        {
            if (n < 0)
            {
                throw new ArgumentException("n must be zero or greater.");
            }
            if (_index + n > _bytes.Length)
            {
                throw new EofException("End of data reached.");
            }
            _index += unchecked((int)(n));
        }

        /// <exception cref="System.IO.IOException"/>
        public override bool TrySkip(long n)
        {
            if (n < 0)
            {
                throw new ArgumentException("n must be zero or greater.");
            }
            _index += unchecked((int)(n));
            if (_index > _bytes.Length)
            {
                _index = _bytes.Length;
                return false;
            }
            return true;
        }
    }
}
