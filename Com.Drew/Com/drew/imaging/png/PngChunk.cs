using JetBrains.Annotations;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunk
    {
        [NotNull]
        private readonly PngChunkType _chunkType;

        [NotNull]
        private readonly byte[] _bytes;

        public PngChunk([NotNull] PngChunkType chunkType, [NotNull] byte[] bytes)
        {
            _chunkType = chunkType;
            _bytes = bytes;
        }

        [NotNull]
        public PngChunkType GetChunkType()
        {
            return _chunkType;
        }

        [NotNull]
        public byte[] GetBytes()
        {
            return _bytes;
        }
    }
}
