using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SequentialStreamReaderTest : SequentialAccessTestBase
    {
        [Fact]
        public void TestConstructWithNullStreamThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SequentialStreamReader(null));
        }

        protected override SequentialReader CreateReader(byte[] bytes)
        {
            return new SequentialStreamReader(new MemoryStream(bytes));
        }
    }
}
