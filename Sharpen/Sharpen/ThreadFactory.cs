using System;

namespace Sharpen
{
	public class ThreadFactory
	{
		public Thread NewThread (Runnable r)
		{
			Thread t = new Thread (r);
			t.SetDaemon (true);
			t.Start ();
			return t;
		}
	}
}
