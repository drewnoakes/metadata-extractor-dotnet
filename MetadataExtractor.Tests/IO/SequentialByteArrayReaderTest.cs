// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.IO;

/// <summary>Unit tests for <see cref="SequentialByteArrayReader"/>.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class SequentialByteArrayReaderTest : SequentialReaderTestBase
{
    [Fact]
    public void ConstructWithNullStreamThrows()
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        Assert.Throws<ArgumentNullException>(() => new SequentialByteArrayReader(null!));
    }

    [Theory]
    [CombinatorialData]
    public void HugeTrySkipReturnsFalseAndStopsAtArrayEnd([CombinatorialValues(0x100000000L, 0x100000001L, long.MaxValue)] long count)
    {
        var reader = new SequentialByteArrayReader([0, 1, 2, 3], baseIndex: 1);

        Assert.False(reader.TrySkip(count));

        Assert.Equal(4, reader.Position);
        Assert.Equal(0, reader.Available());
        Assert.Throws<EndOfStreamException>(() => reader.GetByte());
        Assert.Equal(4, reader.Position);
    }

    [Theory]
    [CombinatorialData]
    public void HugeSkipThrowsWithoutChangingPosition([CombinatorialValues(0x100000000L, 0x100000001L, long.MaxValue)] long count)
    {
        var reader = new SequentialByteArrayReader([0, 1, 2, 3], baseIndex: 1);

        var exception = Assert.Throws<IOException>(() => reader.Skip(count));

        Assert.Equal("End of data reached.", exception.Message);
        Assert.Equal(1, reader.Position);
        Assert.Equal(3, reader.Available());
        Assert.Equal(1, reader.GetByte());
    }

    [Theory]
    [CombinatorialData]
    public void HugeGetBytesFromNonzeroIndexThrowsBeforeAllocationOrCopy(bool callerProvidedBuffer)
    {
        var reader = new SequentialByteArrayReader([0, 1, 2], baseIndex: 1);
        byte[] buffer = [0xAA, 0xBB, 0xCC];

        var exception = callerProvidedBuffer
            ? Assert.Throws<IOException>(() => reader.GetBytes(buffer, 1, int.MaxValue))
            : Assert.Throws<IOException>(() => reader.GetBytes(int.MaxValue));

        Assert.Equal("End of data reached.", exception.Message);
        Assert.Equal(1, reader.Position);
        Assert.Equal(new byte[] { 0xAA, 0xBB, 0xCC }, buffer);
        Assert.Equal(1, reader.GetByte());
    }

    protected override SequentialReader CreateReader(byte[] bytes)
    {
        return new SequentialByteArrayReader(bytes);
    }
}
