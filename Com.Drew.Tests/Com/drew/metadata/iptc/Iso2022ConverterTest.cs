using NUnit.Framework;

namespace Com.Drew.Metadata.Iptc
{
    public sealed class Iso2022ConverterTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public void TestConvertIso2022CharsetToJavaCharset()
        {
            Assert.AreEqual("UTF-8", Iso2022Converter.ConvertIso2022CharsetToJavaCharset(new sbyte[] { unchecked(0x1B), unchecked(0x25), unchecked(0x47) }));
            Assert.AreEqual("ISO-8859-1", Iso2022Converter.ConvertIso2022CharsetToJavaCharset(new sbyte[] { unchecked(0x1B), unchecked((sbyte)0xE2), unchecked((sbyte)0x80), unchecked((sbyte)0xA2), unchecked(0x41) }));
        }
    }
}
