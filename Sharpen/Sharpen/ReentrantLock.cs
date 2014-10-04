namespace Sharpen
{
	using System;
	using System.Threading;

	public class ReentrantLock
	{
		public void Lock ()
		{
			Monitor.Enter (this);
		}

		public bool TryLock ()
		{
			return Monitor.TryEnter (this);
		}

		public void Unlock ()
		{
			Monitor.Exit (this);
		}
	}
}
