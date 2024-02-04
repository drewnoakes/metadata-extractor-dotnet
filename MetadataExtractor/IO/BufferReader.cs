// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;

namespace MetadataExtractor.IO;

internal ref struct BufferReader(ReadOnlySpan<byte> bytes, bool isBigEndian)
{
    private readonly ReadOnlySpan<byte> _bytes = bytes;
    private readonly bool _isBigEndian = isBigEndian;

    private int _position = 0;

    public readonly int Available => _bytes.Length - _position;

    public readonly int Position => _position;

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
}
