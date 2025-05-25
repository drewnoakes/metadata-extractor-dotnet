// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeReaderExtensionsTests
{
    [Fact]
    public void Get4ccString()
    {
        byte[] bytes = [0x66, 0x74, 0x79, 0x70];

        SequentialReader reader = new SequentialByteArrayReader(bytes);

        Assert.Equal("ftyp", reader.Get4ccString());
    }
}
