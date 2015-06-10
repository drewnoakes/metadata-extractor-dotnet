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
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SequentialStreamReader : SequentialReader
    {
        [NotNull]
        private readonly Stream _stream;

        public SequentialStreamReader([NotNull] Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();
            _stream = stream;
        }

        /// <exception cref="System.IO.IOException"/>
        protected override byte GetByte()
        {
            var value = _stream.ReadByte();
            if (value == -1)
            {
                throw new EofException("End of data reached.");
            }
            return unchecked((byte)value);
        }

        /// <exception cref="System.IO.IOException"/>
        public override byte[] GetBytes(int count)
        {
            var bytes = new byte[count];
            var totalBytesRead = 0;
            while (totalBytesRead != count)
            {
                var bytesRead = _stream.Read(bytes, totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    throw new EofException("End of data reached.");
                }
                totalBytesRead += bytesRead;
                Debug.Assert((totalBytesRead <= count));
            }
            return bytes;
        }

        public override void Skip(long n)
        {
            if (n < 0)
                throw new ArgumentException("n must be zero or greater.");

            if (_stream.Position + n > _stream.Length)
                throw new EofException("Unable to skip past of end of file");

            _stream.Seek(n, SeekOrigin.Current);
        }

        public override bool TrySkip(long n)
        {
            try
            {
                Skip(n);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
