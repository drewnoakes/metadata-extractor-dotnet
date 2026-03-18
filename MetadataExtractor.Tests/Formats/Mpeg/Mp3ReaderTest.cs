// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mpeg;

public sealed class Mp3ReaderTest
{
    [Fact]
    public void InvalidBitrate_DoesNotThrow()
    {
        // Header FF F4 01 30: bitrate=0 (free), which previously caused
        // IndexOutOfRangeException in SetBitrate (yPos = bitrate - 1 = -1)
        var bytes = new byte[] { 0xFF, 0xF4, 0x01, 0x30 };
        var reader = new SequentialByteArrayReader(bytes);
        var directory = new Mp3Reader().Extract(reader);

        Assert.False(directory.ContainsTag(Mp3Directory.TagBitrate));
        Assert.False(directory.ContainsTag(Mp3Directory.TagFrameSize));
    }

    [Fact]
    public void ReservedFrequency_DoesNotThrow()
    {
        // Header FF FF EC 30: frequency=3 (reserved), which previously caused
        // IndexOutOfRangeException on frequencyMapping[_, 3]
        var bytes = new byte[] { 0xFF, 0xFF, 0xEC, 0x30 };
        var reader = new SequentialByteArrayReader(bytes);
        var directory = new Mp3Reader().Extract(reader);

        Assert.False(directory.ContainsTag(Mp3Directory.TagFrequency));
        Assert.False(directory.ContainsTag(Mp3Directory.TagFrameSize));
    }
}
