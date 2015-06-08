using JetBrains.Annotations;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunk
    {
        [NotNull]
        private readonly PngChunkType _chunkType;

        [NotNull]
        private readonly sbyte[] _bytes;

        public PngChunk([NotNull] PngChunkType chunkType, [NotNull] sbyte[] bytes)
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
        public sbyte[] GetBytes()
        {
            return _bytes;
        }
    }
}
