using System.IO;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class StreamReaderTest : SequentialAccessTestBase
	{
		public virtual void TestConstructWithNullStreamThrows()
		{
			new Com.Drew.Lang.StreamReader(null);
		}

		protected internal override SequentialReader CreateReader(sbyte[] bytes)
		{
			return new Com.Drew.Lang.StreamReader(new ByteArrayInputStream(bytes));
		}
	}
}
