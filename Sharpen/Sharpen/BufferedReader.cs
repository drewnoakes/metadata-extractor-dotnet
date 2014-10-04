namespace Sharpen
{
	using System;
	using System.IO;

	public class BufferedReader : StreamReader
	{
		public BufferedReader (InputStreamReader r) : base(r.BaseStream)
		{
		}
	}
}
