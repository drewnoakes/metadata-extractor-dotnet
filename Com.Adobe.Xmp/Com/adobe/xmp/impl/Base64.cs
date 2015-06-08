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
    public class Base64
    {
        /// <summary>marker for invalid bytes</summary>
        private const sbyte Invalid = unchecked((sbyte)(-1));

        /// <summary>marker for accepted whitespace bytes</summary>
        private const sbyte Whitespace = unchecked((sbyte)(-2));

        /// <summary>marker for an equal symbol</summary>
        private const sbyte Equal = unchecked((sbyte)(-3));

        private static sbyte[] base64 = new sbyte[] { unchecked((sbyte)(byte)('A')), unchecked((sbyte)(byte)('B')), unchecked((sbyte)(byte)('C')), unchecked((sbyte)(byte)('D')), unchecked((sbyte)(byte)('E')), unchecked((sbyte)(byte)('F')), unchecked(
            (sbyte)(byte)('G')), unchecked((sbyte)(byte)('H')), unchecked((sbyte)(byte)('I')), unchecked((sbyte)(byte)('J')), unchecked((sbyte)(byte)('K')), unchecked((sbyte)(byte)('L')), unchecked((sbyte)(byte)('M')), unchecked((sbyte)(byte)('N')), unchecked(
            (sbyte)(byte)('O')), unchecked((sbyte)(byte)('P')), unchecked((sbyte)(byte)('Q')), unchecked((sbyte)(byte)('R')), unchecked((sbyte)(byte)('S')), unchecked((sbyte)(byte)('T')), unchecked((sbyte)(byte)('U')), unchecked((sbyte)(byte)('V')), unchecked(
            (sbyte)(byte)('W')), unchecked((sbyte)(byte)('X')), unchecked((sbyte)(byte)('Y')), unchecked((sbyte)(byte)('Z')), unchecked((sbyte)(byte)('a')), unchecked((sbyte)(byte)('b')), unchecked((sbyte)(byte)('c')), unchecked((sbyte)(byte)('d')), unchecked(
            (sbyte)(byte)('e')), unchecked((sbyte)(byte)('f')), unchecked((sbyte)(byte)('g')), unchecked((sbyte)(byte)('h')), unchecked((sbyte)(byte)('i')), unchecked((sbyte)(byte)('j')), unchecked((sbyte)(byte)('k')), unchecked((sbyte)(byte)('l')), unchecked(
            (sbyte)(byte)('m')), unchecked((sbyte)(byte)('n')), unchecked((sbyte)(byte)('o')), unchecked((sbyte)(byte)('p')), unchecked((sbyte)(byte)('q')), unchecked((sbyte)(byte)('r')), unchecked((sbyte)(byte)('s')), unchecked((sbyte)(byte)('t')), unchecked(
            (sbyte)(byte)('u')), unchecked((sbyte)(byte)('v')), unchecked((sbyte)(byte)('w')), unchecked((sbyte)(byte)('x')), unchecked((sbyte)(byte)('y')), unchecked((sbyte)(byte)('z')), unchecked((sbyte)(byte)('0')), unchecked((sbyte)(byte)('1')), unchecked(
            (sbyte)(byte)('2')), unchecked((sbyte)(byte)('3')), unchecked((sbyte)(byte)('4')), unchecked((sbyte)(byte)('5')), unchecked((sbyte)(byte)('6')), unchecked((sbyte)(byte)('7')), unchecked((sbyte)(byte)('8')), unchecked((sbyte)(byte)('9')), unchecked(
            (sbyte)(byte)('+')), unchecked((sbyte)(byte)('/')) };

        private static sbyte[] ascii = new sbyte[255];

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
                ascii[idx] = Invalid;
            }
            // valid bytes
            for (int idx_1 = 0; idx_1 < base64.Length; idx_1++)
            {
                ascii[base64[idx_1]] = unchecked((sbyte)idx_1);
            }
            // whitespaces
            ascii[unchecked((int)(0x09))] = Whitespace;
            ascii[unchecked((int)(0x0A))] = Whitespace;
            ascii[unchecked((int)(0x0D))] = Whitespace;
            ascii[unchecked((int)(0x20))] = Whitespace;
            // trailing equals
            ascii[unchecked((int)(0x3d))] = Equal;
        }

        /// <summary>Encode the given byte[].</summary>
        /// <param name="src">the source string.</param>
        /// <returns>the base64-encoded data.</returns>
        public static sbyte[] Encode(sbyte[] src)
        {
            return Encode(src, 0);
        }

        /// <summary>Encode the given byte[].</summary>
        /// <param name="src">the source string.</param>
        /// <param name="lineFeed">
        /// a linefeed is added after <code>linefeed</code> characters;
        /// must be dividable by four; 0 means no linefeeds
        /// </param>
        /// <returns>the base64-encoded data.</returns>
        public static sbyte[] Encode(sbyte[] src, int lineFeed)
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
            sbyte[] dst = new sbyte[codeLength];
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
                bits24 = (src[sidx++] & unchecked((int)(0xFF))) << 16;
                bits24 |= (src[sidx++] & unchecked((int)(0xFF))) << 8;
                bits24 |= (src[sidx++] & unchecked((int)(0xFF))) << 0;
                bits6 = (bits24 & unchecked((int)(0x00FC0000))) >> 18;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked((int)(0x0003F000))) >> 12;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked((int)(0x00000FC0))) >> 6;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked((int)(0x0000003F)));
                dst[didx++] = base64[bits6];
                lf += 4;
                if (didx < codeLength && lineFeed > 0 && lf % lineFeed == 0)
                {
                    dst[didx++] = unchecked((int)(0x0A));
                }
            }
            if (src.Length - sidx == 2)
            {
                bits24 = (src[sidx] & unchecked((int)(0xFF))) << 16;
                bits24 |= (src[sidx + 1] & unchecked((int)(0xFF))) << 8;
                bits6 = (bits24 & unchecked((int)(0x00FC0000))) >> 18;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked((int)(0x0003F000))) >> 12;
                dst[didx++] = base64[bits6];
                bits6 = (bits24 & unchecked((int)(0x00000FC0))) >> 6;
                dst[didx++] = base64[bits6];
                dst[didx++] = unchecked((sbyte)(byte)('='));
            }
            else
            {
                if (src.Length - sidx == 1)
                {
                    bits24 = (src[sidx] & unchecked((int)(0xFF))) << 16;
                    bits6 = (bits24 & unchecked((int)(0x00FC0000))) >> 18;
                    dst[didx++] = base64[bits6];
                    bits6 = (bits24 & unchecked((int)(0x0003F000))) >> 12;
                    dst[didx++] = base64[bits6];
                    dst[didx++] = unchecked((sbyte)(byte)('='));
                    dst[didx++] = unchecked((sbyte)(byte)('='));
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
        public static sbyte[] Decode(sbyte[] src)
        {
            //
            // Do ascii printable to 0-63 conversion.
            //
            int sidx;
            int srcLen = 0;
            for (sidx = 0; sidx < src.Length; sidx++)
            {
                sbyte val = ascii[src[sidx]];
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
            sbyte[] dst = new sbyte[srcLen * 3 / 4];
            //
            // Do 4-byte to 3-byte conversion.
            //
            int didx;
            for (sidx = 0, didx = 0; didx < dst.Length - 2; sidx += 4, didx += 3)
            {
                dst[didx] = unchecked((sbyte)(((src[sidx] << 2) & unchecked((int)(0xFF))) | ((src[sidx + 1] >> 4) & unchecked((int)(0x03)))));
                dst[didx + 1] = unchecked((sbyte)(((src[sidx + 1] << 4) & unchecked((int)(0xFF))) | ((src[sidx + 2] >> 2) & unchecked((int)(0x0F)))));
                dst[didx + 2] = unchecked((sbyte)(((src[sidx + 2] << 6) & unchecked((int)(0xFF))) | ((src[sidx + 3]) & unchecked((int)(0x3F)))));
            }
            if (didx < dst.Length)
            {
                dst[didx] = unchecked((sbyte)(((src[sidx] << 2) & unchecked((int)(0xFF))) | ((src[sidx + 1] >> 4) & unchecked((int)(0x03)))));
            }
            if (++didx < dst.Length)
            {
                dst[didx] = unchecked((sbyte)(((src[sidx + 1] << 4) & unchecked((int)(0xFF))) | ((src[sidx + 2] >> 2) & unchecked((int)(0x0F)))));
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
