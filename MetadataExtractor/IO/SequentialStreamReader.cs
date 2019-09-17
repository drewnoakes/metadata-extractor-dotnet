// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace MetadataExtractor.IO
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SequentialStreamReader : SequentialReader
    {
        private readonly Stream _stream;

        public override long Position => _stream.Position;

        public SequentialStreamReader(Stream stream, bool isMotorolaByteOrder = true)
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

        public override int Available()
        {
            return (int)(_stream.Length - _stream.Position);
        }

        public override bool IsCloserToEnd(long numberOfBytes)
        {
            return _stream.Position + numberOfBytes > _stream.Length;
        }
    }
}
