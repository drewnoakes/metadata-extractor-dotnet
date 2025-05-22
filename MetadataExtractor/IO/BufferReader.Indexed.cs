// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;

namespace MetadataExtractor.IO;

internal ref partial struct BufferReader
{
    public readonly bool GetBit(int index)
    {
        var byteIndex = index / 8;
        var bitIndex = index % 8;

        return ((GetByte(byteIndex) >> bitIndex) & 1) == 1;
    }

    public readonly void GetBytes(int index, scoped Span<byte> bytes)
    {
        ValidateIndex(index, bytes.Length);

        _bytes.Slice(index, bytes.Length).CopyTo(bytes);
    }

    public readonly byte GetByte(int index)
    {
        ValidateIndex(index, 1);

        return _bytes[index];
    }

    public readonly short GetInt16(int index)
    {
        ValidateIndex(index, 2);

        var bytes = _bytes.Slice(index, 2);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(bytes)
            : BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public readonly ushort GetUInt16(int index)
    {
        ValidateIndex(index, 2);

        var bytes = _bytes.Slice(index, 2);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(bytes)
            : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public readonly int GetInt24(int index)
    {
        Span<byte> bytes = stackalloc byte[3];

        GetBytes(index, bytes);

        if (_isBigEndian)
        {
            return
                bytes[0] << 16 |
                bytes[1] << 8 |
                bytes[2];
        }
        else
        {
            return
                bytes[2] << 16 |
                bytes[1] << 8 |
                bytes[0];
        }
    }

    public readonly int GetInt32(int index)
    {
        ValidateIndex(index, 4);

        var bytes = _bytes.Slice(index, 4);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(bytes)
            : BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public readonly uint GetUInt32(int index)
    {
        ValidateIndex(index, 4);

        var bytes = _bytes.Slice(index, 4);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(bytes)
            : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public readonly float GetS15Fixed16(int index)
    {
        ValidateIndex(index, 4);

        ReadOnlySpan<byte> bytes = _bytes.Slice(index, 4);

        if (_isBigEndian)
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

    public readonly long GetInt64(int index)
    {
        ValidateIndex(index, 8);

        var bytes = _bytes.Slice(index, 8);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(bytes)
            : BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    /// <exception cref="IOException"/>
    public readonly float GetFloat32(int index)
    {
#if NET462 || NETSTANDARD2_0
        return BitConverter.ToSingle(BitConverter.GetBytes(GetInt32(index)), 0);
#else
        Span<byte> bytes = stackalloc byte[4];

        GetBytes(index, bytes);

#if NET8_0_OR_GREATER
        return _isBigEndian
           ? BinaryPrimitives.ReadSingleBigEndian(bytes)
           : BinaryPrimitives.ReadSingleLittleEndian(bytes);
#else
        if (_isBigEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToSingle(bytes);
#endif
#endif
    }

    public readonly double GetDouble64(int index)
    {
#if NET462 || NETSTANDARD2_0
        return BitConverter.Int64BitsToDouble(GetInt64(index));
#else
        Span<byte> bytes = stackalloc byte[8];

        GetBytes(index, bytes);

#if NET8_0_OR_GREATER
        return _isBigEndian
           ? BinaryPrimitives.ReadDoubleBigEndian(bytes)
           : BinaryPrimitives.ReadDoubleLittleEndian(bytes);
#else
        if (_isBigEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToDouble(bytes);
#endif
#endif
    }

    public readonly string GetString(int index, int bytesRequested, Encoding encoding)
    {
        if (bytesRequested < 0)
            throw new ArgumentOutOfRangeException(nameof(bytesRequested), "Must be zero or greater.");

        // This check is important on .NET Framework
        if (bytesRequested is 0)
        {
            return "";
        }

        using var buffer = bytesRequested <= ScopedBuffer.MaxStackBufferSize
            ? new ScopedBuffer(stackalloc byte[bytesRequested])
            : new ScopedBuffer(bytesRequested);

        GetBytes(index, buffer);

        return encoding.GetString(buffer);
    }

    private readonly void ValidateIndex(int index, int bytesRequested)
    {
        if (!IsValidIndex(index, bytesRequested))
            throw new BufferBoundsException(index, bytesRequested, _bytes.Length);
    }

    private readonly bool IsValidIndex(int index, int bytesRequested)
    {
        return
            bytesRequested >= 0 &&
            index >= 0 &&
            index + (long)bytesRequested - 1L < _bytes.Length;
    }
}
