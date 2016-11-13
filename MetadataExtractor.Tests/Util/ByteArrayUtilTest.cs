using Xunit;

using MetadataExtractor.Util;

namespace MetadataExtractor.Tests.Util
{
    public class ByteArrayUtilTest
    {
        [Fact]
        public void StartsWith()
        {
            var bytes = new byte[] { 0, 1, 2, 3 };

            Assert.True(bytes.StartsWith(new byte[] { }));
            Assert.True(bytes.StartsWith(new byte[] { 0 }));
            Assert.True(bytes.StartsWith(new byte[] { 0, 1 }));
            Assert.True(bytes.StartsWith(new byte[] { 0, 1, 2 }));
            Assert.True(bytes.StartsWith(new byte[] { 0, 1, 2, 3 }));
            Assert.False(bytes.StartsWith(new byte[] { 0, 1, 2, 3, 4 }));
            Assert.False(bytes.StartsWith(new byte[] { 1 }));
            Assert.False(bytes.StartsWith(new byte[] { 1, 2 }));
        }
    }
}