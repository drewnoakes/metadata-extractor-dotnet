// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using System.Buffers.Binary;

namespace MetadataExtractor.IO;

internal ref struct BufferReader(ReadOnlySpan<byte> bytes, bool isBigEndian)
{
    private readonly ReadOnlySpan<byte> _bytes = bytes;
    private readonly bool _isBigEndian = isBigEndian;

    private int _position = 0;

    public readonly int Available => _bytes.Length - _position;

    public readonly int Position => _position;

    public readonly bool IsBigEndian => _isBigEndian;

    public byte GetByte()
    {
        if (_position >= _bytes.Length)
            throw new IOException("End of data reached.");

        return _bytes[_position++];
    }

    public void GetBytes(scoped Span<byte> bytes)
    {
        var buffer = Advance(bytes.Length);
        buffer.CopyTo(bytes);
    }

    public byte[] GetBytes(int count)
    {
        var buffer = Advance(count);
        var bytes = new byte[count];

        buffer.CopyTo(bytes);
        return bytes;
    }

    private ReadOnlySpan<byte> Advance(int count)
    {
        Debug.Assert(count >= 0, "count must be zero or greater");

        if (_position + count > _bytes.Length)
            throw new IOException("End of data reached.");

        var span = _bytes.Slice(_position, count);

        _position += count;

        return span;
    }

    public void Skip(int count)
    {
        Debug.Assert(count >= 0, "count must be zero or greater");

        if (_position + count > _bytes.Length)
            throw new IOException("End of data reached.");

        _position += count;
    }

    public sbyte GetSByte()
    {
        return unchecked((sbyte)_bytes[_position++]);
    }

    public ushort GetUInt16()
    {
        var bytes = Advance(2);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(bytes)
            : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public short GetInt16()
    {
        var bytes = Advance(2);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(bytes)
            : BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public uint GetUInt32()
    {
        var bytes = Advance(4);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(bytes)
            : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public int GetInt32()
    {
        var bytes = Advance(4);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(bytes)
            : BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public long GetInt64()
    {
        var bytes = Advance(8);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(bytes)
            : BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    public ulong GetUInt64()
    {
        var bytes = Advance(8);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt64BigEndian(bytes)
            : BinaryPrimitives.ReadUInt64LittleEndian(bytes);
    }

    public string GetString(int bytesRequested, Encoding encoding)
    {
        // This check is important on .NET Framework
        if (bytesRequested is 0)
            return "";

        Span<byte> bytes = bytesRequested <= 256
            ? stackalloc byte[bytesRequested]
            : new byte[bytesRequested];

        GetBytes(bytes);

        return encoding.GetString(bytes);
    }

    // Indexed Methods -

    public void GetBytes(int index, scoped Span<byte> bytes)
    {
        ValidateIndex(index, bytes.Length);

        _bytes.Slice(index, bytes.Length).CopyTo(bytes);
    }

    public byte GetByte(int index)
    {
        ValidateIndex(index, 1);

        return _bytes[index];
    }

    public short GetInt16(int index)
    {
        ValidateIndex(index, 2);

        var bytes = _bytes.Slice(index, 2);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(bytes)
            : BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public ushort GetUInt16(int index)
    {
        ValidateIndex(index, 2);

        var bytes = _bytes.Slice(index, 2);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(bytes)
            : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public int GetInt24(int index)
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

    public int GetInt32(int index)
    {
        ValidateIndex(index, 4);

        var bytes = _bytes.Slice(index, 4);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(bytes)
            : BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public uint GetUInt32(int index)
    {
        ValidateIndex(index, 4);

        var bytes = _bytes.Slice(index, 4);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(bytes)
            : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public float GetS15Fixed16(int index)
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

    public long GetInt64(int index)
    {
        ValidateIndex(index, 8);

        var bytes = _bytes.Slice(index, 8);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(bytes)
            : BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    /// <exception cref="IOException"/>
    public float GetFloat32(int index)
    {
#if NET462 || NETSTANDARD1_3
        return BitConverter.ToSingle(BitConverter.GetBytes(GetInt32(index)), 0);
#else
        Span<byte> bytes = stackalloc byte[4];

        GetBytes(index, bytes);

        if (_isBigEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToSingle(bytes);
#endif
    }

    public double GetDouble64(int index)
    {
#if NET462 || NETSTANDARD1_3
        return BitConverter.Int64BitsToDouble(GetInt64(index));
#else
        Span<byte> bytes = stackalloc byte[8];

        GetBytes(index, bytes);

        if (_isBigEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToDouble(bytes);
#endif
    }

    public string GetString(int index, int bytesRequested, Encoding encoding)
    {
        // This check is important on .NET Framework
        if (bytesRequested is 0)
        {
            return "";
        }
        else if (bytesRequested < 256)
        {
            Span<byte> bytes = stackalloc byte[bytesRequested];

            GetBytes(index, bytes);

            return encoding.GetString(bytes);
        }
        else
        {
            byte[] bytes = ArrayPool<byte>.Shared.Rent(bytesRequested);

            Span<byte> span = bytes.AsSpan().Slice(0, bytesRequested);

            GetBytes(index, span);

            var s = encoding.GetString(span);

            ArrayPool<byte>.Shared.Return(bytes);

            return s;
        }
    }

    private void ValidateIndex(int index, int bytesRequested)
    {
        if (!IsValidIndex(index, bytesRequested))
            throw new BufferBoundsException(index, bytesRequested, _bytes.Length);
    }

    private bool IsValidIndex(int index, int bytesRequested)
    {
        return
            bytesRequested >= 0 &&
            index >= 0 &&
            index + (long)bytesRequested - 1L < _bytes.Length;
    }
}
