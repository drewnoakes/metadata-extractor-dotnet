using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataExtractor.IO
{
    public interface IRandomAccessStream
    {
        ReaderInfo CreateReader();
        ReaderInfo CreateReader(bool isMotorolaByteOrder);
        ReaderInfo CreateReader(long startPosition, long length, bool isMotorolaByteOrder);

        byte GetByte(long index);
        ushort GetUInt16(long index, bool isMotorolaByteOrder);
        short GetInt16(long index, bool isMotorolaByteOrder);
        int GetInt24(long index, bool isMotorolaByteOrder);
        uint GetUInt32(long index, bool isMotorolaByteOrder);
        int GetInt32(long index, bool isMotorolaByteOrder);
        long GetInt64(long index, bool isMotorolaByteOrder);
        ulong GetUInt64(long index, bool isMotorolaByteOrder);
        float GetS15Fixed16(long index, bool isMotorolaByteOrder);


        long Length { get; }

        int Read(long index, byte[] buffer, int offset, int count);
        int Read(long index, byte[] buffer, int offset, int count, bool allowPartial);

        void Seek(long index);
        byte[] ToArray(long index, int count);

        int ValidateRange(long index, int bytesRequested);
    }
}
