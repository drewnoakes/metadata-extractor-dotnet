using Sharpen;
using System;
using NUnit.Framework;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class StreamReaderTest : SequentialAccessTestBase
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public virtual void TestConstructWithNullStreamThrows()
        {
            new StreamReader(null);
        }

        protected override SequentialReader CreateReader(byte[] bytes)
        {
            return new StreamReader(new ByteArrayInputStream(bytes));
        }
    }
}
