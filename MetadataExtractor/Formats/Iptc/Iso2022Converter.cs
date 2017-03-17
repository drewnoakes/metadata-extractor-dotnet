#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Iptc
{
    public static class Iso2022Converter
    {
        private const int Dot = 0xe280a2;
        private const byte LatinCapitalA = (byte)'A';
        private const byte LatinCapitalG = (byte)'G';
        private const byte PercentSign = (byte)'%';
        private const byte Esc = 0x1B;

        /// <summary>Attempts to convert the given ISO2022 escape sequence to an encoding name.</summary>
        [CanBeNull]
        public static string ConvertEscapeSequenceToEncodingName([NotNull] byte[] bytes)
        {
            if (bytes.Length > 2 && bytes[0] == Esc && bytes[1] == PercentSign && bytes[2] == LatinCapitalG)
                return "UTF-8";

            if (bytes.Length > 3 && bytes[0] == Esc && (bytes[3] | (bytes[2] << 8) | (bytes[1] << 16)) == Dot && bytes[4] == LatinCapitalA)
                return "ISO-8859-1";

            return null;
        }

        /// <summary>Attempts to guess the encoding of a string provided as a byte array.</summary>
        /// <remarks>
        /// Encodings trialled are, in order:
        /// <list type="bullet">
        ///   <item>UTF-8</item>
        ///   <item>ISO-8859-1</item>
        ///   <item>ASCII</item>
        /// </list>
        /// <para />
        /// Its only purpose is to guess the encoding if and only if iptc tag coded character set is not set. If the
        /// encoding is not UTF-8, the tag should be set. Otherwise it is bad practice. This method tries to
        /// workaround this issue since some metadata manipulating tools do not prevent such bad practice.
        /// <para />
        /// About the reliability of this method: The check if some bytes are UTF-8 or not has a very high reliability.
        /// The two other checks are less reliable.
        /// </remarks>
        /// <param name="bytes">some text as bytes</param>
        /// <returns>the name of the encoding or null if none could be guessed</returns>
        [CanBeNull]
        internal static Encoding GuessEncoding([NotNull] byte[] bytes)
        {
            // First, give ASCII a shot
            var ascii = true;
            foreach (var b in bytes)
            {
                if (b < 0x20 || b > 0x7f)
                {
                    ascii = false;
                    break;
                }
            }

            if (ascii)
            {
#if NETSTANDARD1_3
                return Encoding.UTF8;
#else
                return Encoding.ASCII;
#endif
            }

            var utf8 = false;
            var i = 0;
            while (i < bytes.Length - 4)
            {
                if (bytes[i] <= 0x7F) { i++; continue; }
                if (bytes[i] >= 0xC2 && bytes[i] <= 0xDF && bytes[i + 1] >= 0x80 && bytes[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (bytes[i] >= 0xE0 && bytes[i] <= 0xF0 && bytes[i + 1] >= 0x80 && bytes[i + 1] < 0xC0 && bytes[i + 2] >= 0x80 && bytes[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (bytes[i] >= 0xF0 && bytes[i] <= 0xF4 && bytes[i + 1] >= 0x80 && bytes[i + 1] < 0xC0 && bytes[i + 2] >= 0x80 && bytes[i + 2] < 0xC0 && bytes[i + 3] >= 0x80 && bytes[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false;
                break;
            }

            if (utf8)
                return Encoding.UTF8;

            Encoding[] encodings =
            {
                Encoding.UTF8,
                Encoding.GetEncoding("iso-8859-1") // Latin-1
            };

            foreach (var encoding in encodings)
            {
                Debug.Assert(encoding != null);
                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    var s = encoding.GetString(bytes, 0, bytes.Length);
                    if (s.IndexOf((char)65533) != -1)
                        continue;
                    return encoding;
                }
                catch
                {
                    // fall through...
                }
            }

            // No encodings succeeded. Return null.
            return null;
        }
    }
}
