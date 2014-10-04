using System;

namespace Sharpen
{
	public class JavaWeakReference<T>: WeakReference
	{
		public JavaWeakReference (T t): base (t)
		{
		}
		
		public T Get ()
		{
			return (T) Target;
		}
	}
}

