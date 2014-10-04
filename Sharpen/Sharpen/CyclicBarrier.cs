namespace Sharpen
{
	using System;

	public class CyclicBarrier
	{
		private CountDownLatch counter;

		public CyclicBarrier (int parties)
		{
			this.counter = new CountDownLatch (parties);
		}

		public void Await ()
		{
			this.counter.CountDown ();
			this.counter.Await ();
		}
	}
}
