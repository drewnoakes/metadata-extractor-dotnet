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
using System.IO;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>
    /// Provides methods to read specific values from a
    /// <see cref="System.IO.RandomAccessFile"/>
    /// , with a consistent, checked exception structure for
    /// issues.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RandomAccessFileReader : RandomAccessReader
    {
        [NotNull]
        private readonly RandomAccessFile _file;

        private readonly long _length;

        private int _currentIndex;

        /// <exception cref="System.IO.IOException"/>
        public RandomAccessFileReader([NotNull] RandomAccessFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException();
            }
            _file = file;
            _length = _file.Length();
        }

        public override long GetLength()
        {
            return _length;
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal override sbyte GetByte(int index)
        {
            if (index != _currentIndex)
            {
                Seek(index);
            }
            int b = _file.Read();
            if (b < 0)
            {
                throw new BufferBoundsException("Unexpected end of file encountered.");
            }
            System.Diagnostics.Debug.Assert((b <= unchecked((int)(0xff))));
            _currentIndex++;
            return unchecked((sbyte)b);
        }

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public override sbyte[] GetBytes(int index, int count)
        {
            ValidateIndex(index, count);
            if (index != _currentIndex)
            {
                Seek(index);
            }
            sbyte[] bytes = new sbyte[count];
            int bytesRead = _file.Read(bytes);
            _currentIndex += bytesRead;
            if (bytesRead != count)
            {
                throw new BufferBoundsException("Unexpected end of file encountered.");
            }
            return bytes;
        }

        /// <exception cref="System.IO.IOException"/>
        private void Seek(int index)
        {
            if (index == _currentIndex)
            {
                return;
            }
            _file.Seek(index);
            _currentIndex = index;
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal override bool IsValidIndex(int index, int bytesRequested)
        {
            return bytesRequested >= 0 && index >= 0 && (long)index + (long)bytesRequested - 1L < _length;
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal override void ValidateIndex(int index, int bytesRequested)
        {
            if (!IsValidIndex(index, bytesRequested))
            {
                throw new BufferBoundsException(index, bytesRequested, _length);
            }
        }
    }
}
