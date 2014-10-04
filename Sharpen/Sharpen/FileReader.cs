namespace Sharpen
{
	using System;

	public class FileReader : InputStreamReader
	{
		public FileReader (FilePath f) : base(f.GetPath ())
		{
		}
	}
}
