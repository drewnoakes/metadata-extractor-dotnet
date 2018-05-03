using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.IO
{
    public class RandomAccessStream //: IDisposable
    {
        private Stream p_inputStream;
        private long p_streamLength = -1;

        private bool p_canSeek = false;
        //private readonly List<ReaderInfo> rdrList = new List<ReaderInfo>();

        private const int DefaultChunkLength = 4 * 1024;
        
        private readonly int p_chunkLength;
        public Dictionary<long, byte[]> p_chunks = new Dictionary<long, byte[]>();

        private bool p_isStreamFinished;

        public RandomAccessStream([NotNull] Stream stream)
        {
            p_inputStream = stream ?? throw new ArgumentNullException(nameof(stream));
            p_canSeek = stream.CanSeek;

            p_chunkLength = DefaultChunkLength;
        }

        public RandomAccessStream([NotNull] byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();

            // No need to wrap a MemoryStream around this with other checking later
            //p_inputStream = new MemoryStream(bytes, false);
            //p_canSeek = p_inputStream.CanSeek;
            p_canSeek = true;

            p_chunks.Add(0, bytes);
            p_chunkLength = bytes.Length;

            p_streamLength = bytes.Length;
            p_isStreamFinished = true;
        }

        public bool CanSeek => p_canSeek;

        public long Length
        {
            get
            {
                //return (CanSeek) ? p_inputStream.Length : long.MaxValue; // -1;
                // If finished and only one chunk, can bypass a lot of checks particularly when the input was a byte[]
                return (CanSeek) ? (p_isStreamFinished && p_chunks.Count == 1 ? p_streamLength : p_inputStream.Length) : long.MaxValue; // -1;
                //return (CanSeek) ? (p_isStreamFinished && p_chunks.Count == 1 ? p_chunks[0].Length : p_inputStream.Length) : long.MaxValue; // -1;
            }
        }

        //public ReaderInfo CreateReader(long startPosition = -1, long length = -1, bool isMotorolaByteOrder = true)
        public ReaderInfo CreateReader() => CreateReader(-1, -1, true);
        public ReaderInfo CreateReader(bool isMotorolaByteOrder) => CreateReader(-1, -1, isMotorolaByteOrder);
        public ReaderInfo CreateReader(long startPosition, long length, bool isMotorolaByteOrder)
        {
            var pos = startPosition >= 0 ? startPosition : 0;

            //var rdrInfo = new ReaderInfo(this, pos, 0, length, isMotorolaByteOrder);
            //rdrList.Add(rdrInfo);
            //return rdrInfo;
            return new ReaderInfo(this, pos, 0, length, isMotorolaByteOrder);
        }

        public void Seek(long index, SeekOrigin origin)
        {
            ValidateIndex(index, 0, false);
        }


        public int Read(long index, byte[] buffer, int offset, int count, bool isSequential)
        {
            ValidateIndex(index, count, isSequential);

            // This bypasses a lot of checks particularly when the input was a byte[]
            if (p_isStreamFinished && p_chunks.Count == 1)
            {
                Array.Copy(p_chunks[0], (int)index, buffer, 0, count);
                return count;
            }


            var remaining = count;                      // how many bytes are requested
            var fromOffset = (int)index;
            //var toIndex = 0;
            var toIndex = offset > 0 ? offset : 0;
            while (remaining != 0)
            {
                var fromChunkIndex = fromOffset / p_chunkLength;     // chunk integer key
                var fromInnerIndex = fromOffset % p_chunkLength;     // index inside the chunk to start reading
                var length = Math.Min(remaining, p_chunkLength - fromInnerIndex);
                var chunk = p_chunks[fromChunkIndex];
                Array.Copy(chunk, fromInnerIndex, buffer, toIndex, length);
                remaining -= length;
                fromOffset += length;
                toIndex += length;
            }

            return toIndex - offset;
        }

        public byte GetByte(long index, bool isSequential)
        {
            ValidateIndex(index, 1, isSequential);

            // This bypasses a lot of checks particularly when the input was a byte[]
            if (p_isStreamFinished && p_chunks.Count == 1)
                return p_chunks[0][index];


            var chunkIndex = index / p_chunkLength;
            var innerIndex = index % p_chunkLength;

            if (p_chunks.ContainsKey(chunkIndex))
                return p_chunks[chunkIndex][innerIndex];
            else
                return unchecked((byte)-1);
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public ushort GetUInt16(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (ushort)
                    (GetByte(index    , isSequential) << 8 | 
                     GetByte(index + 1, isSequential));
            }
            // Intel ordering - LSB first
            return (ushort)
                (GetByte(index + 1, isSequential) << 8 | 
                 GetByte(index    , isSequential));*/

            var bytes = new byte[2];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (ushort)
                    (bytes[0] << 8 |
                     bytes[1]);
            }
            // Intel ordering - LSB first
            return (ushort)
                (bytes[1] << 8 |
                 bytes[0]);
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public short GetInt16(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (short)
                    (GetByte(index    , isSequential) << 8 | 
                     GetByte(index + 1, isSequential));
            }
            // Intel ordering - LSB first
            return (short)
                (GetByte(index + 1, isSequential) << 8 | 
                 GetByte(index    , isSequential));*/

            var bytes = new byte[2];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (short)
                    (bytes[0] << 8 |
                     bytes[1]);
            }
            // Intel ordering - LSB first
            return (short)
                (bytes[1] << 8 |
                 bytes[0]);
        }

        public int GetInt24(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return
                    GetByte(index    , isSequential) << 16 |
                    GetByte(index + 1, isSequential) << 8 |
                    GetByte(index + 2, isSequential);
            }
            // Intel ordering - LSB first (little endian)
            return
                GetByte(index + 2, isSequential) << 16 |
                GetByte(index + 1, isSequential) << 8 |
                GetByte(index    , isSequential);*/

            var bytes = new byte[3];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return
                    bytes[0] << 16 |
                    bytes[1] << 8 |
                    bytes[2];
            }
            // Intel ordering - LSB first (little endian)
            return
                bytes[2] << 16 |
                bytes[1] << 8 |
                bytes[0];
        }

        public uint GetUInt32(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return (uint)
                    (GetByte(index    , isSequential) << 24 |
                     GetByte(index + 1, isSequential) << 16 |
                     GetByte(index + 2, isSequential) << 8 |
                     GetByte(index + 3, isSequential));
            }
            // Intel ordering - LSB first (little endian)
            return (uint)
                (GetByte(index + 3, isSequential) << 24 |
                 GetByte(index + 2, isSequential) << 16 |
                 GetByte(index + 1, isSequential) << 8 |
                 GetByte(index    , isSequential));*/

            var bytes = new byte[4];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return (uint)
                    (bytes[0] << 24 |
                     bytes[1] << 16 |
                     bytes[2] << 8 |
                     bytes[3]);
            }
            // Intel ordering - LSB first (little endian)
            return (uint)
                (bytes[3] << 24 |
                 bytes[2] << 16 |
                 bytes[1] << 8 |
                 bytes[0]);
        }

        public int GetInt32(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return
                    GetByte(index    , isSequential) << 24 |
                    GetByte(index + 1, isSequential) << 16 |
                    GetByte(index + 2, isSequential) << 8 |
                    GetByte(index + 3, isSequential);
            }
            // Intel ordering - LSB first (little endian)
            return
                GetByte(index + 3, isSequential) << 24 |
                GetByte(index + 2, isSequential) << 16 |
                GetByte(index + 1, isSequential) << 8 |
                GetByte(index    , isSequential);*/

            var bytes = new byte[4];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return
                    bytes[0] << 24 |
                    bytes[1] << 16 |
                    bytes[2] << 8 |
                    bytes[3];
            }
            // Intel ordering - LSB first (little endian)
            return
                bytes[3] << 24 |
                bytes[2] << 16 |
                bytes[1] << 8 |
                bytes[0];
        }

        public long GetInt64(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (long)GetByte(index    , isSequential) << 56 |
                    (long)GetByte(index + 1, isSequential) << 48 |
                    (long)GetByte(index + 2, isSequential) << 40 |
                    (long)GetByte(index + 3, isSequential) << 32 |
                    (long)GetByte(index + 4, isSequential) << 24 |
                    (long)GetByte(index + 5, isSequential) << 16 |
                    (long)GetByte(index + 6, isSequential) << 8 |
                          GetByte(index + 7, isSequential);
            }
            // Intel ordering - LSB first
            return
                (long)GetByte(index + 7, isSequential) << 56 |
                (long)GetByte(index + 6, isSequential) << 48 |
                (long)GetByte(index + 5, isSequential) << 40 |
                (long)GetByte(index + 4, isSequential) << 32 |
                (long)GetByte(index + 3, isSequential) << 24 |
                (long)GetByte(index + 2, isSequential) << 16 |
                (long)GetByte(index + 1, isSequential) << 8 |
                      GetByte(index    , isSequential);*/

            var bytes = new byte[8];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (long)bytes[0] << 56 |
                    (long)bytes[1] << 48 |
                    (long)bytes[2] << 40 |
                    (long)bytes[3] << 32 |
                    (long)bytes[4] << 24 |
                    (long)bytes[5] << 16 |
                    (long)bytes[6] << 8 |
                          bytes[7];
            }
            // Intel ordering - LSB first
            return
                (long)bytes[7] << 56 |
                (long)bytes[6] << 48 |
                (long)bytes[5] << 40 |
                (long)bytes[4] << 32 |
                (long)bytes[3] << 24 |
                (long)bytes[2] << 16 |
                (long)bytes[1] << 8 |
                      bytes[0];
        }

        public ulong GetUInt64(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (ulong)GetByte(index    , isSequential) << 56 |
                    (ulong)GetByte(index + 1, isSequential) << 48 |
                    (ulong)GetByte(index + 2, isSequential) << 40 |
                    (ulong)GetByte(index + 3, isSequential) << 32 |
                    (ulong)GetByte(index + 4, isSequential) << 24 |
                    (ulong)GetByte(index + 5, isSequential) << 16 |
                    (ulong)GetByte(index + 6, isSequential) << 8 |
                           GetByte(index + 7, isSequential);
            }
            // Intel ordering - LSB first
            return
                    (ulong)GetByte(index + 7, isSequential) << 56 |
                    (ulong)GetByte(index + 6, isSequential) << 48 |
                    (ulong)GetByte(index + 5, isSequential) << 40 |
                    (ulong)GetByte(index + 4, isSequential) << 32 |
                    (ulong)GetByte(index + 3, isSequential) << 24 |
                    (ulong)GetByte(index + 2, isSequential) << 16 |
                    (ulong)GetByte(index + 1, isSequential) << 8 |
                           GetByte(index    , isSequential);*/

            var bytes = new byte[8];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (ulong)bytes[0] << 56 |
                    (ulong)bytes[1] << 48 |
                    (ulong)bytes[2] << 40 |
                    (ulong)bytes[3] << 32 |
                    (ulong)bytes[4] << 24 |
                    (ulong)bytes[5] << 16 |
                    (ulong)bytes[6] << 8 |
                           bytes[7];
            }
            // Intel ordering - LSB first
            return
                (ulong)bytes[7] << 56 |
                (ulong)bytes[6] << 48 |
                (ulong)bytes[5] << 40 |
                (ulong)bytes[4] << 32 |
                (ulong)bytes[3] << 24 |
                (ulong)bytes[2] << 16 |
                (ulong)bytes[1] << 8 |
                       bytes[0];
        }

        public float GetS15Fixed16(long index, bool IsMotorolaByteOrder, bool isSequential)
        {
            /*var byte1 = GetByte(index    , isSequential);
            var byte2 = GetByte(index + 1, isSequential);
            var byte3 = GetByte(index + 2, isSequential);
            var byte4 = GetByte(index + 3, isSequential);

            if (IsMotorolaByteOrder)
            {
                float res = byte1 << 8 | byte2;
                var d = byte3 << 8 | byte4;
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = byte2 << 8 | byte1;
                float res = byte4 << 8 | byte3;
                return (float)(res + d / 65536.0);
            }*/
            
            var bytes = new byte[4];
            Read(index, bytes, 0, bytes.Length, isSequential);

            if (IsMotorolaByteOrder)
            {
                float res = bytes[0] << 8 | bytes[1];
                var d = bytes[2] << 8 | bytes[3];
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = bytes[1] << 8 | bytes[0];
                float res = bytes[3] << 8 | bytes[2];
                return (float)(res + d / 65536.0);
            }
        }


        public void ValidateIndex(long index, long bytesRequested, bool isSequential)
        {
            if (!IsValidIndex(index, bytesRequested))
            {
                if (index < 0)
                    throw new BufferBoundsException($"Attempt to read from buffer using a negative index ({index})");
                if (bytesRequested < 0)
                    throw new BufferBoundsException("Number of requested bytes must be zero or greater");
                if (index + bytesRequested - 1 > int.MaxValue)
                    throw new BufferBoundsException($"Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: {index}, requested count: {bytesRequested})");
                if (isSequential && (index + bytesRequested >= p_streamLength))
                    throw new IOException("End of data reached.");
                if (isSequential)
                    throw new IOException();

                //Debug.Assert(p_isStreamFinished);
                // TODO test that can continue using an instance of this type after this exception
                throw new BufferBoundsException(index, bytesRequested, p_streamLength);
            }
        }

        private bool IsValidIndex(long index, long bytesRequested)
        {
            if (index < 0 || bytesRequested < 0)
                return false;

            // if there's only one chunk, there's no need to calculate anything.
            // This bypasses a lot of checks particularly when the input was a byte[]
            if (p_isStreamFinished && p_chunks.Count == 1)
                return index + bytesRequested - 1 < p_streamLength;


            var endIndex = index + bytesRequested - 1;
            if (endIndex < 0) endIndex = 0;

            // Maybe don't check this?
            if (endIndex > int.MaxValue)
                return false;

            //if (p_isStreamFinished)
            //    return endIndex < p_streamLength;
            //    //return endIndex <= p_streamLength;

            
            // zero-based
            long chunkstart = index / p_chunkLength;
            long chunkend = ((index + bytesRequested) / p_chunkLength) + 1;

            
            if (!p_chunks.ContainsKey(chunkstart))
            {
                if(!CanSeek)
                    chunkstart = p_chunks.Count == 0 ? 0 : p_chunks.Keys.Max() + 1;
            }

            for (var i = chunkstart; i < chunkend; i++)
            {
                if (!p_chunks.ContainsKey(i))
                {
                    p_isStreamFinished = false;

                    // chunkstart can be anywhere. Try to seek
                    if (CanSeek)
                        p_inputStream.Seek(i * p_chunkLength, SeekOrigin.Begin);

                    byte[] chunk = new byte[p_chunkLength];

                    //Console.Write(" -- canseek=" + CanSeek + ";   index=" + index + "; bytesRequested=" + bytesRequested + "; reading chunk '" + i + "' with length=" + _chunkLength);



                    var totalBytesRead = 0;
                    while (!p_isStreamFinished && totalBytesRead != p_chunkLength)
                    //while (totalBytesRead != p_chunkLength)
                    {
                        var bytesRead = p_inputStream.Read(chunk, totalBytesRead, p_chunkLength - totalBytesRead);

                        if (bytesRead == 0)
                        {
                            // the stream has ended, which may be ok
                            p_isStreamFinished = true;
                            p_streamLength = i * p_chunkLength + totalBytesRead;

                            // check we have enough bytes for the requested index
                            if (endIndex >= p_streamLength)
                            {
                                p_totalBytesRead += totalBytesRead;
                                p_chunks.Add(i, chunk);
                                return false;
                            }
                        }
                        else
                        {
                            totalBytesRead += bytesRead;
                        }
                    }

                    p_totalBytesRead += totalBytesRead;

                    //Console.WriteLine("; totalBytesRead=" + totalBytesRead);

                    p_chunks.Add(i, chunk);
                }
                //else
                //    Console.WriteLine(" -- index=" + index + ";bytesRequested=" + bytesRequested + "; already read buffer '" + i + "' with length=" + _chunkLength);
            }

            //Console.WriteLine("validatechunk; index=" + index + "; bytesRequested=" + bytesRequested + "; start=" + chunkstart + "; position=" + Position);


            return true;
        }

        private long p_totalBytesRead = 0;
        public long TotalBytesRead => p_totalBytesRead;

    }
}
