namespace Sharpen
{
	using System;
	using System.IO.Compression;

	public class GZIPOutputStream : OutputStream
	{
		public GZIPOutputStream (OutputStream os)
		{
			Wrapped = new GZipStream (os, CompressionMode.Compress);
		}
	}
}
