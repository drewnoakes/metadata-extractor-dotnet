// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2001 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>
    /// A utility class to perform base64 encoding and decoding as specified
    /// in RFC-1521.
    /// </summary>
    /// <remarks>
    /// A utility class to perform base64 encoding and decoding as specified
    /// in RFC-1521. See also RFC 1421.
    /// </remarks>
    /// <version>$Revision: 1.4 $</version>
    public static class Base64
    {
        /// <summary>marker for invalid bytes</summary>
        private const byte Invalid = unchecked((byte)-1);

        /// <summary>marker for accepted whitespace bytes</summary>
        private const byte Whitespace = unchecked((byte)-2);

        /// <summary>marker for an equal symbol</summary>
        private const byte Equal = unchecked((byte)-3);

        private static readonly byte[] base64 = new byte[] { unchecked((byte)(byte)('A')), unchecked((byte)(byte)('B')), unchecked((byte)(byte)('C')), unchecked((byte)(byte)('D')), unchecked((byte)(byte)('E')), unchecked((byte)(byte)('F')), unchecked(
            (byte)(byte)('G')), unchecked((byte)(byte)('H')), unchecked((byte)(byte)('I')), unchecked((byte)(byte)('J')), unchecked((byte)(byte)('K')), unchecked((byte)(byte)('L')), unchecked((byte)(byte)('M')), unchecked((byte)(byte)('N')), unchecked(
            (byte)(byte)('O')), unchecked((byte)(byte)('P')), unchecked((byte)(byte)('Q')), unchecked((byte)(byte)('R')), unchecked((byte)(byte)('S')), unchecked((byte)(byte)('T')), unchecked((byte)(byte)('U')), unchecked((byte)(byte)('V')), unchecked(
            (byte)(byte)('W')), unchecked((byte)(byte)('X')), unchecked((byte)(byte)('Y')), unchecked((byte)(byte)('Z')), unchecked((byte)(byte)('a')), unchecked((byte)(byte)('b')), unchecked((byte)(byte)('c')), unchecked((byte)(byte)('d')), unchecked(
            (byte)(byte)('e')), unchecked((byte)(byte)('f')), unchecked((byte)(byte)('g')), unchecked((byte)(byte)('h')), unchecked((byte)(byte)('i')), unchecked((byte)(byte)('j')), unchecked((byte)(byte)('k')), unchecked((byte)(byte)('l')), unchecked(
            (byte)(byte)('m')), unchecked((byte)(byte)('n')), unchecked((byte)(byte)('o')), unchecked((byte)(byte)('p')), unchecked((byte)(byte)('q')), unchecked((byte)(byte)('r')), unchecked((byte)(byte)('s')), unchecked((byte)(byte)('t')), unchecked(
            (byte)(byte)('u')), unchecked((byte)(byte)('v')), unchecked((byte)(byte)('w')), unchecked((byte)(byte)('x')), unchecked((byte)(byte)('y')), unchecked((byte)(byte)('z')), unchecked((byte)(byte)('0')), unchecked((byte)(byte)('1')), unchecked(
            (byte)(byte)('2')), unchecked((byte)(byte)('3')), unchecked((byte)(byte)('4')), unchecked((byte)(byte)('5')), unchecked((byte)(byte)('6')), unchecked((byte)(byte)('7')), unchecked((byte)(byte)('8')), unchecked((byte)(byte)('9')), unchecked(
            (byte)(byte)('+')), unchecked((byte)(byte)('/')) };

        private static readonly byte[] Ascii = new byte[255];

        static Base64()
        {
            //  0 to  3
            //  4 to  7
            //  8 to 11
            // 11 to 15
            // 16 to 19
            // 20 to 23
            // 24 to 27
            // 28 to 31
            // 32 to 35
            // 36 to 39
            // 40 to 43
            // 44 to 47
            // 48 to 51
            // 52 to 55
            // 56 to 59
            // 60 to 63
            // not valid bytes
            for (int idx = 0; idx < 255; idx++)
            {
                Ascii[idx] = Invalid;
            }
            // valid bytes
            for (int idx1 = 0; idx1 < base64.Length; idx1++)
            {
                Ascii[base64[idx1]] = unchecked((byte)idx1);
            }
            // whitespaces
            Ascii[unchecked(0x09)] = Whitespace;
            Ascii[unchecked(0x0A)] = Whitespace;
            Ascii[unchecked(0x0D)] = Whitespace;
            Ascii[unchecked(0x20)] = Whitespace;
            // trailing equals
            Ascii[unchecked(0x3d)] = Equal;
        }

        /// <summary>Encode the given byte[].</summary>
        /// <param name="src">the source string.</param>
        /// <param name="lineFeed">
        /// a linefeed is added after <c>linefeed</c> characters;
        /// must be dividable by four; 0 means no linefeeds
        /// </param>
        /// <returns>the base64-encoded data.</returns>
        public static byte[] Encode(byte[] src, int lineFeed = 0)
        {
            // linefeed must be dividable by 4
            lineFeed = lineFeed / 4 * 4;
            if (lineFeed < 0)
            {
                lineFeed = 0;
            }
            // determine code length
            int codeLength = ((src.Length + 2) / 3) * 4;
            if (lineFeed > 0)
            {
                codeLength += (codeLength - 1) / lineFeed;
            }
            byte[] dst = new byte[codeLength];
            int bits24;
            int bits6;
            //
            // Do 3-byte to 4-byte conversion + 0-63 to ascii printable conversion
            //
            int didx = 0;
            int sidx = 0;
            int lf = 0;
            while (sidx + 3 <= src.Length)
            {
                bits24 = (src[sidx++] & unchecked(0xFF)) << 16;
                bits24 |= (src[sidx++] & unchecked(0xFF)) << 8;
                bits24 |= (src[sidx++] & unchecked(0xFF)) << 0;
                bits6 = (bits24 & unchecked(0x00FC0000)) >> 18;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked(0x0003F000)) >> 12;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked(0x00000FC0)) >> 6;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked(0x0000003F));
                dst[didx++] = base64[bits6];
                lf += 4;
                if (didx < codeLength && lineFeed > 0 && lf % lineFeed == 0)
                {
                    dst[didx++] = unchecked(0x0A);
                }
            }
            if (src.Length - sidx == 2)
            {
                bits24 = (src[sidx] & unchecked(0xFF)) << 16;
                bits24 |= (src[sidx + 1] & unchecked(0xFF)) << 8;
                bits6 = (bits24 & unchecked(0x00FC0000)) >> 18;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked(0x0003F000)) >> 12;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked(0x00000FC0)) >> 6;
                dst[didx++] = base64[bits6];
                dst[didx++] = unchecked((byte)(byte)('='));
            }
            else
            {
                if (src.Length - sidx == 1)
                {
                    bits24 = (src[sidx] & unchecked(0xFF)) << 16;
                    bits6 = (bits24 & unchecked(0x00FC0000)) >> 18;
                    dst[didx++] = base64[bits6];
                    bits6 = (bits24 & unchecked(0x0003F000)) >> 12;
                    dst[didx++] = base64[bits6];
                    dst[didx++] = unchecked((byte)(byte)('='));
                    dst[didx++] = unchecked((byte)(byte)('='));
                }
            }
            return dst;
        }

        /// <summary>Encode the given string.</summary>
        /// <param name="src">the source string.</param>
        /// <returns>the base64-encoded string.</returns>
        public static string Encode(string src)
        {
            return Runtime.GetStringForBytes(Encode(Runtime.GetBytesForString(src)));
        }

        /// <summary>Decode the given byte[].</summary>
        /// <param name="src">the base64-encoded data.</param>
        /// <returns>the decoded data.</returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the base 64 strings contains non-valid characters,
        /// beside the bas64 chars, LF, CR, tab and space are accepted.
        /// </exception>
        public static byte[] Decode(byte[] src)
        {
            //
            // Do ascii printable to 0-63 conversion.
            //
            int sidx;
            int srcLen = 0;
            for (sidx = 0; sidx < src.Length; sidx++)
            {
                byte val = Ascii[src[sidx]];
                if (val >= 0)
                {
                    src[srcLen++] = val;
                }
                else
                {
                    if (val == Invalid)
                    {
                        throw new ArgumentException("Invalid base 64 string");
                    }
                }
            }
            //
            // Trim any padding.
            //
            while (srcLen > 0 && src[srcLen - 1] == Equal)
            {
                srcLen--;
            }
            byte[] dst = new byte[srcLen * 3 / 4];
            //
            // Do 4-byte to 3-byte conversion.
            //
            int didx;
            for (sidx = 0, didx = 0; didx < dst.Length - 2; sidx += 4, didx += 3)
            {
                dst[didx] = unchecked((byte)(((src[sidx] << 2) & unchecked(0xFF)) | ((src[sidx + 1] >> 4) & unchecked(0x03))));
                dst[didx + 1] = unchecked((byte)(((src[sidx + 1] << 4) & unchecked(0xFF)) | ((src[sidx + 2] >> 2) & unchecked(0x0F))));
                dst[didx + 2] = unchecked((byte)(((src[sidx + 2] << 6) & unchecked(0xFF)) | ((src[sidx + 3]) & unchecked(0x3F))));
            }
            if (didx < dst.Length)
            {
                dst[didx] = unchecked((byte)(((src[sidx] << 2) & unchecked(0xFF)) | ((src[sidx + 1] >> 4) & unchecked(0x03))));
            }
            if (++didx < dst.Length)
            {
                dst[didx] = unchecked((byte)(((src[sidx + 1] << 4) & unchecked(0xFF)) | ((src[sidx + 2] >> 2) & unchecked(0x0F))));
            }
            return dst;
        }

        /// <summary>Decode the given string.</summary>
        /// <param name="src">the base64-encoded string.</param>
        /// <returns>the decoded string.</returns>
        public static string Decode(string src)
        {
            return Runtime.GetStringForBytes(Decode(Runtime.GetBytesForString(src)));
        }
    }
}
