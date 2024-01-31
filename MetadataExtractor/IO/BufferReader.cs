// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;

namespace MetadataExtractor.IO;

internal ref struct BufferReader(ReadOnlySpan<byte> bytes, bool isBigEndian)
{
    private readonly ReadOnlySpan<byte> _bytes = bytes;
    private int _position = 0;
    private bool _isBigEndian = isBigEndian;

    public long Position => _position;

    public byte GetByte()
    {
        if (_position >= _bytes.Length)
            throw new IOException("End of data reached.");

        return _bytes[_position++];
    }

    public void GetBytes(scoped Span<byte> bytes)
    {
        if (_position + bytes.Length > _bytes.Length)
            throw new IOException("End of data reached.");

        _bytes.Slice(_position, bytes.Length).CopyTo(bytes);
        _position += bytes.Length;
    }

    public byte[] GetBytes(int count)
    {
        if (_position + count > _bytes.Length)
            throw new IOException("End of data reached.");

        var bytes = new byte[count];

        _bytes.Slice(0, count).CopyTo(bytes);
        _position += count;
        return bytes;
    }

    public int Available()
    {
        return _bytes.Length - _position;
    }

    public void Skip(long n)
    {
        if (n < 0)
            throw new ArgumentException("n must be zero or greater.");

        if (_position + n > _bytes.Length)
            throw new IOException("End of data reached.");

        _position += unchecked((int)n);
    }

    public bool TrySkip(long n)
    {
        if (n < 0)
            throw new ArgumentException("n must be zero or greater.");

        _position += unchecked((int)n);

        if (_position > _bytes.Length)
        {
            _position = _bytes.Length;
            return false;
        }

        return true;
    }

    public sbyte GetSByte()
    {
        return unchecked((sbyte)GetByte());
    }

    public ushort GetUInt16()
    {
        Span<byte> bytes = stackalloc byte[2];

        GetBytes(bytes);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(bytes)
            : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public short GetInt16()
    {
        Span<byte> bytes = stackalloc byte[2];

        GetBytes(bytes);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(bytes)
            : BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public uint GetUInt32()
    {
        Span<byte> bytes = stackalloc byte[4];

        GetBytes(bytes);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(bytes)
            : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public int GetInt32()
    {
        Span<byte> bytes = stackalloc byte[4];

        GetBytes(bytes);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(bytes)
            : BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public long GetInt64()
    {
        Span<byte> bytes = stackalloc byte[8];
        GetBytes(bytes);

        return _isBigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(bytes)
            : BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    public ulong GetUInt64()
    {
        Span<byte> bytes = stackalloc byte[8];
        GetBytes(bytes);

        return _isBigEndian
            ? BinaryPrimitives.ReadUInt64BigEndian(bytes)
            : BinaryPrimitives.ReadUInt64LittleEndian(bytes);
    }

    public string GetString(int bytesRequested, Encoding encoding)
    {
        // This check is important on .NET Framework
        if (bytesRequested is 0)
            return "";

        Span<byte> bytes = bytesRequested < 256 ? stackalloc byte[bytesRequested] : new byte[bytesRequested];

        GetBytes(bytes);

        return encoding.GetString(bytes);
    }
}
