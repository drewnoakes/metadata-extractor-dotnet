using MetadataExtractor.Formats.Iptc;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Iptc
{
    public sealed class Iso2022ConverterTest
    {
        [Test]
        public void TestConvertIso2022CharsetToJavaCharset()
        {
            Assert.AreEqual("UTF-8", Iso2022Converter.ConvertIso2022CharsetToJavaCharset(new byte[] { 0x1B, 0x25, 0x47 }));
            Assert.AreEqual("ISO-8859-1", Iso2022Converter.ConvertIso2022CharsetToJavaCharset(new byte[] { 0x1B, 0xE2, 0x80, 0xA2, 0x41 }));
        }
    }
}
