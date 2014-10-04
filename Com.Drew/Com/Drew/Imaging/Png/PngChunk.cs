using Com.Drew.Imaging.Png;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngChunk
	{
		[NotNull]
		private readonly PngChunkType _chunkType;

		[NotNull]
		private readonly sbyte[] _bytes;

		public PngChunk(PngChunkType chunkType, sbyte[] bytes)
		{
			_chunkType = chunkType;
			_bytes = bytes;
		}

		[NotNull]
		public virtual PngChunkType GetChunkType()
		{
			return _chunkType;
		}

		[NotNull]
		public virtual sbyte[] GetBytes()
		{
			return _bytes;
		}
	}
}
