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
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace MetadataExtractor.IO
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SequentialStreamReader : SequentialReader
    {
        [NotNull]
        private readonly Stream _stream;

        public override long Position => _stream.Position;

        public SequentialStreamReader([NotNull] Stream stream, bool isMotorolaByteOrder = true)
            : base(isMotorolaByteOrder)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public override byte GetByte()
        {
            var value = _stream.ReadByte();
            if (value == -1)
                throw new IOException("End of data reached.");

            return unchecked((byte)value);
        }

        public override SequentialReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new SequentialStreamReader(_stream, isMotorolaByteOrder);

        public override byte[] GetBytes(int count)
        {
            var bytes = new byte[count];
            GetBytes(bytes, 0, count);
            return bytes;
        }

        public override void GetBytes(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;
            while (totalBytesRead != count)
            {
                var bytesRead = _stream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                    throw new IOException("End of data reached.");
                totalBytesRead += bytesRead;
                Debug.Assert(totalBytesRead <= count);
            }
        }

        public override void Skip(long n)
        {
            if (n < 0)
                throw new ArgumentException("n must be zero or greater.");

            if (_stream.Position + n > _stream.Length)
                throw new IOException("Unable to skip past of end of file");

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
                // Stream ended, or error reading from underlying source
                return false;
            }
        }
    }
}
