using System;
using System.IO;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.IO
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SequentialStreamReaderTest : SequentialAccessTestBase
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructWithNullStreamThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            new SequentialStreamReader(null);
        }

        protected override SequentialReader CreateReader(byte[] bytes)
        {
            return new SequentialStreamReader(new MemoryStream(bytes));
        }
    }
}
