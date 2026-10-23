// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.IO;

public sealed class SequentialStreamReaderStreamingTest
{
    [Fact]
    public void TracksPositionAcrossByteArrayAndSpanReadsWithoutSeeking()
    {
        using var stream = new ForwardOnlyStream([0x80, 1, 2, 3, 4, 5, 6, 7, 8]);
        var reader = new SequentialStreamReader(stream);

        Assert.Equal(0, reader.Position);
        Assert.Equal(0x80, reader.GetByte());
        Assert.Equal(1, reader.Position);
        Assert.Equal(new byte[] { 1, 2 }, reader.GetBytes(2));
        Assert.Equal(3, reader.Position);
        byte[] buffer = [0xFF, 0xFF, 0xFF, 0xFF];
        reader.GetBytes(buffer, 1, 2);
        Assert.Equal(new byte[] { 0xFF, 3, 4, 0xFF }, buffer);
        Assert.Equal(5, reader.Position);
        Span<byte> span = stackalloc byte[3];
        reader.GetBytes(span);
        Assert.Equal(new byte[] { 5, 6, 7 }, span.ToArray());
        Assert.Equal(8, reader.Position);
        Assert.Equal(8, reader.GetByte());
        Assert.Equal(9, reader.Position);
    }

    [Fact]
    public void ByteOrderViewsSharePositionOnForwardOnlyStream()
    {
        using var stream = new ForwardOnlyStream([0xFF, 1, 2, 3, 4, 5, 6]);
        var bigEndian = new SequentialStreamReader(stream);
        Assert.Equal(0xFF, bigEndian.GetByte());
        var littleEndian = bigEndian.WithByteOrder(false);

        Assert.Equal(1, littleEndian.Position);
        Assert.Equal(0x0201, littleEndian.GetUInt16());
        Assert.Equal(3, bigEndian.Position);
        Assert.Equal(0x0304, bigEndian.GetUInt16());
        Assert.Equal(5, littleEndian.Position);
        Assert.Equal(0x0605, littleEndian.GetUInt16());
        Assert.Equal(7, bigEndian.Position);
        Assert.Same(littleEndian, littleEndian.WithByteOrder(false));
    }

    [Fact]
    public void SkipsForwardOnlyDataAndCountsPartialSkipAtEof()
    {
        using var stream = new ForwardOnlyStream([0, 1, 2, 3, 4, 5]);
        var reader = new SequentialStreamReader(stream);

        reader.Skip(3);
        Assert.Equal(3, reader.Position);
        Assert.Equal(3, reader.GetByte());
        Assert.False(reader.TrySkip(3));
        Assert.Equal(6, reader.Position);
        Assert.True(reader.TrySkip(0));
        Assert.Throws<EndOfStreamException>(() => reader.GetByte());
        Assert.Equal(6, reader.Position);
    }

    [Fact]
    public void ForwardOnlyAvailableIsNotMistakenForEof()
    {
        using var stream = new ForwardOnlyStream([42]);
        var reader = new SequentialStreamReader(stream);

        Assert.Equal(0, reader.Available());
        Assert.False(reader.IsCloserToEnd(1));
        Assert.Equal(42, reader.GetByte());
        Assert.Equal(1, reader.Position);
    }

    [Theory]
    [CombinatorialData]
    public void CountsBytesConsumedBeforeTruncatedRead(bool spanRead)
    {
        using var stream = new ForwardOnlyStream([1, 2, 3]);
        var reader = new SequentialStreamReader(stream);
        var buffer = new byte[4];

        if (spanRead)
            Assert.ThrowsAny<IOException>(() => reader.GetBytes(buffer.AsSpan()));
        else
            Assert.ThrowsAny<IOException>(() => reader.GetBytes(buffer, 0, buffer.Length));

        Assert.Equal(3, reader.Position);
        Assert.Equal(new byte[] { 1, 2, 3 }, buffer.Take(3));
        Assert.Throws<EndOfStreamException>(() => reader.GetByte());
        Assert.Equal(3, reader.Position);
    }

    [Fact]
    public void RejectsNegativeSkipWithoutConsumingData()
    {
        using var stream = new ForwardOnlyStream([1, 2]);
        var reader = new SequentialStreamReader(stream);

        Assert.Throws<ArgumentException>(() => reader.Skip(-1));
        Assert.Throws<ArgumentException>(() => reader.TrySkip(-1));
        Assert.Equal(0, reader.Position);
        Assert.Equal(1, reader.GetByte());
    }

    [Fact]
    public void RejectsSeekPastEofEvenWhenUnderlyingStreamAllowsIt()
    {
        using var stream = new MemoryStream([1, 2, 3]);
        stream.Position = 1;
        var reader = new SequentialStreamReader(stream);

        Assert.Equal(1, reader.Position);
        Assert.False(reader.TrySkip(3));
        Assert.Equal(1, stream.Position);
        reader.Skip(2);
        Assert.Equal(3, reader.Position);
        Assert.False(reader.TrySkip(1));
        Assert.Equal(3, stream.Position);
    }

    private sealed class ForwardOnlyStream(byte[] bytes) : Stream
    {
        private int _position;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException("Length is not available.");
        public override long Position
        {
            get => throw new NotSupportedException("Position is not available.");
            set => throw new NotSupportedException("Position is not available.");
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Deliberately short reads exercise both the span fallback and array read loops.
            int read = Math.Min(1, Math.Min(count, bytes.Length - _position));
            Array.Copy(bytes, _position, buffer, offset, read);
            _position += read;
            return read;
        }
        public override int ReadByte() => _position == bytes.Length ? -1 : bytes[_position++];
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
