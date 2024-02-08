// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;

namespace MetadataExtractor.IO;

internal ref struct BufferScope
{
    private byte[]? _rentedBuffer;
    private readonly Span<byte> _span;

    public BufferScope(int size)
    {
        _rentedBuffer = ArrayPool<byte>.Shared.Rent(size);
        _span = _rentedBuffer.AsSpan(0, size);
    }

    public BufferScope(Span<byte> span)
    {
        _span = span;
    }

    public readonly Span<byte> Span => _span;

    public void Dispose()
    {
        if (_rentedBuffer is null) return;

        ArrayPool<byte>.Shared.Return(_rentedBuffer);

        _rentedBuffer = null;
    }
}
