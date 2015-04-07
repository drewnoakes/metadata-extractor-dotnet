using Sharpen;

namespace Com.Drew.Metadata.Iptc
{
	public class Iso2022ConverterTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestConvertISO2022CharsetToJavaCharset()
		{
			Sharpen.Tests.AreEqual("UTF-8", Iso2022Converter.ConvertISO2022CharsetToJavaCharset(new sbyte[] { unchecked((int)(0x1B)), unchecked((int)(0x25)), unchecked((int)(0x47)) }));
			Sharpen.Tests.AreEqual("ISO-8859-1", Iso2022Converter.ConvertISO2022CharsetToJavaCharset(new sbyte[] { unchecked((int)(0x1B)), unchecked((sbyte)0xE2), unchecked((sbyte)0x80), unchecked((sbyte)0xA2), unchecked((int)(0x41)) }));
		}
	}
}
