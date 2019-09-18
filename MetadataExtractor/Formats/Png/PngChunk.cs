// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunk
    {
        public PngChunk(PngChunkType chunkType, byte[] bytes)
        {
            ChunkType = chunkType;
            Bytes = bytes;
        }

        public PngChunkType ChunkType { get; }

        public byte[] Bytes { get; }
    }
}
