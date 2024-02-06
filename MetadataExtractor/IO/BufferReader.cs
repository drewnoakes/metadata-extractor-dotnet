// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.IO;

internal ref partial struct BufferReader(ReadOnlySpan<byte> bytes, bool isBigEndian)
{
    private readonly ReadOnlySpan<byte> _bytes = bytes;
    private readonly bool _isBigEndian = isBigEndian;

    private int _position = 0;

    public readonly int Available => _bytes.Length - _position;

    public readonly int Position => _position;

    public readonly bool IsBigEndian => _isBigEndian;
}
