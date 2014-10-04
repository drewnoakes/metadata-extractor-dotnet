using System.IO;
using NUnit.Framework;

namespace Sharpen.InternalTests
{
    [TestFixture]
    public class TestJavaStringFormatConversion
    {
        [NUnit.Framework.Test]
        /// This function tests that all format strings exisiting in Com.Drew project is supported
        public void TestAllFormatStringsIsSupported()
        {
            var stringsReader = new StreamReader("InternalTests\\TestStrings.txt");
            while (!stringsReader.EndOfStream)
            {
                Extensions.ConvertStringFormat(stringsReader.ReadLine().Trim('"'));
            }
        }
    }
}