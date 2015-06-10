using JetBrains.Annotations;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunk
    {
        public PngChunk([NotNull] PngChunkType chunkType, [NotNull] byte[] bytes)
        {
            ChunkType = chunkType;
            Bytes = bytes;
        }

        [NotNull]
        public PngChunkType ChunkType { get; private set; }

        [NotNull]
        public byte[] Bytes { get; private set; }
    }
}
