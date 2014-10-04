using Com.Drew.Lang;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SequentialByteArrayReaderTest : SequentialAccessTestBase
	{
		public virtual void TestConstructWithNullStreamThrows()
		{
			new SequentialByteArrayReader(null);
		}

		protected internal override SequentialReader CreateReader(sbyte[] bytes)
		{
			return new SequentialByteArrayReader(bytes);
		}
	}
}
