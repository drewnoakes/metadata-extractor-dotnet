namespace Sharpen
{
	using System;
	using System.IO;

	public class FileLock
	{
		private FileStream s;

		public FileLock (FileStream s)
		{
			this.s = s;
		}

		public void Release ()
		{
			this.s.Unlock (0, int.MaxValue);
		}
	}
}
