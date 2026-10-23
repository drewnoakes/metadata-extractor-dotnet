// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util;

public sealed class FileTypeDetectorTest
{
    [Theory]
    [InlineData(new byte[] { 0x49, 0x44, 0x33 }, FileType.Mp3)]
    [InlineData(new byte[] { 0x52, 0x49, 0x46, 0x46, 0, 0, 0, 0, 0x57, 0x41, 0x56, 0x45 }, FileType.Wav)]
    [InlineData(new byte[] { 0, 0, 0, 16, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D, 0, 0, 0, 0 }, FileType.Mp4)]
    [InlineData(new byte[] { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 24, 0 }, FileType.Tga)]
    public void DetectsCheckerFormatsWithShortReadsAndRestoresPosition(byte[] header, FileType expected)
    {
        var bytes = new byte[7].Concat(header).ToArray();
        using var stream = new ShortReadStream(bytes, chunkSize: 1);
        stream.Position = 7;

        Assert.Equal(expected, FileTypeDetector.DetectFileType(stream));

        Assert.Equal(7, stream.Position);
        Assert.Equal(header[0], stream.ReadByte());
    }

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
