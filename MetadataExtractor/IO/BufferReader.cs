// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.IO;

/// <summary>
/// Stack-based reader for decoding values from byte spans.
/// Supports sequential and indexed access.
/// Supports little-endian and big-endian values.
/// </summary>
/// <param name="bytes">The byte buffer to decode from.</param>
/// <param name="isBigEndian">The byte ordering to use for multi-byte values.</param>
internal ref partial struct BufferReader(ReadOnlySpan<byte> bytes, bool isBigEndian)
{
    private readonly ReadOnlySpan<byte> _bytes = bytes;
    private readonly bool _isBigEndian = isBigEndian;

    private int _position = 0;

    /// <summary>
    /// Gets the number of bytes remaining in the buffer from the current
    /// <see cref="Position"/> until the end of the buffer.
    /// </summary>
    /// <remarks>
    /// This value only makes sense when performing sequential access.
    /// </remarks>
    public readonly int Available => _bytes.Length - _position;

    /// <summary>
    /// Gets the current position in the buffer. The next value will be read from this position.
    /// </summary>
    /// <remarks>
    /// Only applies to sequential access. Indexed access does not update this value.
    /// </remarks>
    public readonly int Position => _position;

    /// <summary>
    /// Gets the byte ordering this reader uses for multi-byte values.
    /// </summary>
    public readonly bool IsBigEndian => _isBigEndian;
}
