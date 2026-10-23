// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.IO;

/// <author>Drew Noakes https://drewnoakes.com</author>
public class SequentialByteArrayReader : SequentialReader
{
    private readonly byte[] _bytes;

    private int _index;

    public override long Position => _index;

    public SequentialByteArrayReader(byte[] bytes, int baseIndex = 0, bool isMotorolaByteOrder = true)
        : base(isMotorolaByteOrder)
    {
        _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        _index = baseIndex;
    }

    public override byte GetByte()
    {
        if (_index >= _bytes.Length)
            throw new EndOfStreamException("End of data reached.");

        return _bytes[_index++];
    }

    public override SequentialReader WithByteOrder(bool isMotorolaByteOrder) => isMotorolaByteOrder == IsMotorolaByteOrder ? this : new SequentialByteArrayReader(_bytes, _index, isMotorolaByteOrder);

    public override byte[] GetBytes(int count)
    {
        if (count > _bytes.Length - _index)
            throw new IOException("End of data reached.");

        var bytes = new byte[count];
        Array.Copy(_bytes, _index, bytes, 0, count);
        _index += count;
        return bytes;
    }

    public override void GetBytes(Span<byte> bytes)
    {
        if (bytes.Length > _bytes.Length - _index)
            throw new IOException("End of data reached.");

        _bytes.AsSpan(_index, bytes.Length).CopyTo(bytes);
        _index += bytes.Length;
    }

    public override void GetBytes(byte[] buffer, int offset, int count)
    {
        if (count > _bytes.Length - _index)
            throw new IOException("End of data reached.");

        Array.Copy(_bytes, _index, buffer, offset, count);
        _index += count;
    }

    public override void Skip(long n)
    {
        if (n < 0)
            throw new ArgumentException("n must be zero or greater.");

        if (n > _bytes.Length - (long)_index)
            throw new IOException("End of data reached.");

        _index += unchecked((int)n);
    }

    public override bool TrySkip(long n)
    {
        if (n < 0)
            throw new ArgumentException("n must be zero or greater.");

        if (n > _bytes.Length - (long)_index)
        {
            _index = _bytes.Length;
            return false;
        }

        _index += (int)n;
        return true;
    }

    public override int Available()
    {
        return _bytes.Length - _index;
    }

    public override bool IsCloserToEnd(long numberOfBytes)
    {
        return _index + numberOfBytes > _bytes.Length;
    }
}
