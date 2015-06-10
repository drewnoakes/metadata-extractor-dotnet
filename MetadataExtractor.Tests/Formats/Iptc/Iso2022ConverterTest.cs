using MetadataExtractor.Formats.Iptc;
using NUnit.Framework;

namespace Com.Drew.Metadata.Iptc
{
    public sealed class Iso2022ConverterTest
    {

        [Test]
        public void TestConvertIso2022CharsetToJavaCharset()
        {
            Assert.AreEqual("UTF-8", Iso2022Converter.ConvertIso2022CharsetToJavaCharset(new byte[] { unchecked(0x1B), unchecked(0x25), unchecked(0x47) }));
            Assert.AreEqual("ISO-8859-1", Iso2022Converter.ConvertIso2022CharsetToJavaCharset(new byte[] { unchecked(0x1B), unchecked((byte)0xE2), unchecked((byte)0x80), unchecked((byte)0xA2), unchecked(0x41) }));
        }
    }
}
