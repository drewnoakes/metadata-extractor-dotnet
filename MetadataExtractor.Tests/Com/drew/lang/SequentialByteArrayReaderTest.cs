using System;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SequentialByteArrayReaderTest : SequentialAccessTestBase
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructWithNullStreamThrows()
        {
            new SequentialByteArrayReader(null);
        }

        protected override SequentialReader CreateReader(byte[] bytes)
        {
            return new SequentialByteArrayReader(bytes);
        }
    }
}
