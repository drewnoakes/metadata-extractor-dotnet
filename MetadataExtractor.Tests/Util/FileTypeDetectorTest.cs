// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util;

public sealed class FileTypeDetectorTest
{
    [Theory]
    [CombinatorialData]
    public void HandlesShortFilesAndShortReadsWithoutConsumingInput(
        [CombinatorialValues(0, 1, 2, 4, 18, 64)] int length,
        [CombinatorialValues(1, 3, 32)] int chunkSize,
        bool nonzeroPosition)
    {
        int start = nonzeroPosition ? 7 : 0;
        var bytes = new byte[start + length];
        if (length > 0)
            bytes[start] = 0xFF;
        if (length > 1)
            bytes[start + 1] = 0xD8;
        using var stream = new ShortReadStream(bytes, chunkSize);
        stream.Position = start;

        Assert.Equal(length >= 2 ? FileType.Jpeg : FileType.Unknown, FileTypeDetector.DetectFileType(stream));

        Assert.Equal(start, stream.Position);
        Assert.Equal(length > 0 ? 0xFF : -1, stream.ReadByte());
    }

    private sealed class ShortReadStream(byte[] bytes, int chunkSize) : MemoryStream(bytes)
    {
        public override int Read(byte[] buffer, int offset, int count)
            => base.Read(buffer, offset, Math.Min(count, chunkSize));

#if NET8_0_OR_GREATER
        public override int Read(Span<byte> buffer)
            => base.Read(buffer.Slice(0, Math.Min(buffer.Length, chunkSize)));
#endif
    }
}
