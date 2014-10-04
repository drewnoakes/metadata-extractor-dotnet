namespace Sharpen
{
	using System;

	public interface Future<T>
	{
		bool Cancel (bool mayInterruptIfRunning);
		T Get ();
	}
}
