// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using System.Runtime.CompilerServices;

namespace MetadataExtractor.IO;

/// <summary>Reads sequentially from seekable or forward-only streams, leaving the stream open.</summary>
/// <remarks>For forward-only streams, position is counted from construction and availability is unknown (reported as zero).</remarks>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class SequentialStreamReader(Stream stream, bool isMotorolaByteOrder = true)
    : SequentialReader(isMotorolaByteOrder)
{
    private readonly Stream _stream = stream ?? throw new ArgumentNullException(nameof(stream));
    private readonly StrongBox<long> _position = new();

    private SequentialStreamReader(Stream stream, bool isMotorolaByteOrder, StrongBox<long> position)
        : this(stream, isMotorolaByteOrder)
    {
        _position = position;
    }

    public override long Position => _stream.CanSeek ? _stream.Position : _position.Value;

    internal long AvailableBytes => _stream.Length - _stream.Position;

    public override byte GetByte()
    {
        var value = _stream.ReadByte();

        if (value == -1)
            throw new EndOfStreamException("End of data reached.");

        _position.Value++;
        return unchecked((byte)value);
    }

    public override SequentialReader WithByteOrder(bool isMotorolaByteOrder)
    {
        return isMotorolaByteOrder == IsMotorolaByteOrder
            ? this
            : new SequentialStreamReader(_stream, isMotorolaByteOrder, _position);
    }

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
            _position.Value += bytesRead;

            Debug.Assert(totalBytesRead <= count);
        }
    }

#if !NETSTANDARD2_1 && !NET8_0_OR_GREATER
    private readonly byte[] _buffer = new byte[2048];
#endif

    public override void GetBytes(Span<byte> bytes)
    {
        int totalBytesRead = 0;

        while (totalBytesRead != bytes.Length)
        {
            var target = bytes.Slice(totalBytesRead);
#if NETSTANDARD2_1 || NET8_0_OR_GREATER
            var bytesRead = _stream.Read(target);
#else
            var len = Math.Min(bytes.Length - totalBytesRead, _buffer.Length);

            var bytesRead = _stream.Read(_buffer, 0, len);

            _buffer.AsSpan(0, len).CopyTo(target);
#endif
            if (bytesRead == 0)
                throw new IOException("End of data reached.");

            totalBytesRead += bytesRead;
            _position.Value += bytesRead;

            Debug.Assert(totalBytesRead <= bytes.Length);
        }
    }

    public override void Skip(long n)
    {
        if (n < 0)
            throw new ArgumentException("n must be zero or greater.");

        if (!_stream.CanSeek)
        {
            if (n == 0)
                return;

            var buffer = ArrayPool<byte>.Shared.Rent(8192);
            try
            {
                while (n > 0)
                {
                    var count = _stream.Read(buffer, 0, (int)Math.Min(n, buffer.Length));
                    if (count == 0)
                        throw new IOException("End of data reached.");
                    _position.Value += count;
                    n -= count;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            return;
        }

        if ((ulong)(_stream.Position + n) > (ulong)_stream.Length)
            throw new IOException($"Unable to skip. Requested {n} bytes but only {_stream.Length - _stream.Position} remained.");

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
        return _stream.CanSeek ? (int)Math.Min(int.MaxValue, AvailableBytes) : 0;
    }

    public override bool IsCloserToEnd(long numberOfBytes)
    {
        return _stream.CanSeek && _stream.Position + numberOfBytes > _stream.Length;
    }
}
