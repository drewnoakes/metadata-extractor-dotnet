using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Iptc
{
    public static class Iso2022Converter
    {
        private const string Iso88591 = "ISO-8859-1";

        private const string Utf8 = "UTF-8";

        private const byte LatinCapitalA = 0x41;

        private const int Dot = 0xe280a2;

        private const byte LatinCapitalG = 0x47;

        private const byte PercentSign = 0x25;

        private const byte Esc = 0x1B;

        /// <summary>Converts the given ISO2022 char set to a Java charset name.</summary>
        /// <param name="bytes">string data encoded using ISO2022</param>
        /// <returns>the Java charset name as a string, or <c>null</c> if the conversion was not possible</returns>
        [CanBeNull]
        public static string ConvertIso2022CharsetToJavaCharset([NotNull] byte[] bytes)
        {
            if (bytes.Length > 2 && bytes[0] == Esc && bytes[1] == PercentSign && bytes[2] == LatinCapitalG)
            {
                return Utf8;
            }
            if (bytes.Length > 3 && bytes[0] == Esc && (bytes[3] & 0xFF | ((bytes[2] & 0xFF) << 8) | ((bytes[1] & 0xFF) << 16)) == Dot && bytes[4] == LatinCapitalA)
            {
                return Iso88591;
            }
            return null;
        }

        /// <summary>Attempts to guess the encoding of a string provided as a byte array.</summary>
        /// <remarks>
        /// Attempts to guess the encoding of a string provided as a byte array.
        /// <p/>
        /// Encodings trialled are, in order:
        /// <list type="bullet">
        /// <item>UTF-8</item>
        /// <item>ASCII</item>
        /// <item>ISO-8859-1</item>
        /// </list>
        /// <p/>
        /// Its only purpose is to guess the encoding if and only if iptc tag coded character set is not set. If the
        /// encoding is not UTF-8, the tag should be set. Otherwise it is bad practice. This method tries to
        /// workaround this issue since some metadata manipulating tools do not prevent such bad practice.
        /// <p/>
        /// About the reliability of this method: The check if some bytes are UTF-8 or not has a very high reliability.
        /// The two other checks are less reliable.
        /// </remarks>
        /// <param name="bytes">some text as bytes</param>
        /// <returns>the name of the encoding or null if none could be guessed</returns>
        [CanBeNull]
        internal static Encoding GuessEncoding([NotNull] byte[] bytes)
        {
            Encoding[] encodings = { Encoding.UTF8, Encoding.ASCII, Encoding.GetEncoding("iso-8859-1") };
            foreach (var encoding in encodings)
            {
                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    encoding.GetString(bytes);
                    return encoding;
                }
                catch
                {
                }
            }
            // fall through...
            // No encodings succeeded. Return null.
            return null;
        }
    }
}
