// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Riff
{
    /// <summary>
    /// Interface of a class capable of handling an individual RIFF chunk.
    /// </summary>
    /// <author>Dmitry Shechtman</author>
    public interface IRiffChunkHandler
    {
        void ProcessChunk(string fourCc, byte[] payload);
    }
}
