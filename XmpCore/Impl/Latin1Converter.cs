// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System.Text;

namespace XmpCore.Impl
{
    /// <since>12.10.2006</since>
    public static class Latin1Converter
    {
        private const int StateStart = 0;

        private const int StateUtf8Char = 11;

        /// <summary>A converter that processes a byte buffer containing a mix of UTF8 and Latin-1/Cp1252 chars.</summary>
        /// <remarks>
        /// A converter that processes a byte buffer containing a mix of UTF8 and Latin-1/Cp1252 chars.
        /// The result is a buffer where those chars have been converted to UTF-8;
        /// that means it contains only valid UTF-8 chars.
        /// <para />
        /// <em>Explanation of the processing:</em> First the encoding of the buffer is detected looking
        /// at the first four bytes (that works only if the buffer starts with an ASCII-char,
        /// like xmls &apos;&lt;&apos;). UTF-16/32 flavours do not require further proccessing.
        /// <para />
        /// In the case, UTF-8 is detected, it assumes wrong UTF8 chars to be a sequence of
        /// Latin-1/Cp1252 encoded bytes and converts the chars to their corresponding UTF-8 byte
        /// sequence.
        /// <para />
        /// The 0x80..0x9F range is undefined in Latin-1, but is defined in Windows code
        /// page 1252. The bytes 0x81, 0x8D, 0x8F, 0x90, and 0x9D are formally undefined
        /// by Windows 1252. These are in XML's RestrictedChar set, so we map them to a
        /// space.
        /// <para />
        /// The official Latin-1 characters in the range 0xA0..0xFF are converted into
        /// the Unicode Latin Supplement range U+00A0 - U+00FF.
        /// <para />
        /// <em>Example:</em> If an Euro-symbol (€) appears in the byte buffer (0xE2, 0x82, 0xAC),
        /// it will be left as is. But if only the first two bytes are appearing,
        /// followed by an ASCII char a (0xE2 - 0x82 - 0x41), it will be converted to
        /// 0xC3, 0xA2 (â) - 0xE2, 0x80, 0x9A (‚) - 0x41 (a).
        /// </remarks>
        /// <param name="buffer">a byte buffer contain</param>
        /// <returns>Returns a new buffer containing valid UTF-8</returns>
        public static ByteBuffer Convert(ByteBuffer buffer)
        {
            if (ReferenceEquals(buffer.GetEncoding(), Encoding.UTF8))
            {
                // the buffer containing one UTF-8 char (up to 8 bytes)
                var readAheadBuffer = new byte[8];
                // the number of bytes read ahead.
                var readAhead = 0;
                // expected UTF8 bytes to come
                var expectedBytes = 0;
                // output buffer with estimated length
                var @out = new ByteBuffer(buffer.Length() * 4 / 3);
                var state = StateStart;
                for (var i = 0; i < buffer.Length(); i++)
                {
                    var b = buffer.CharAt(i);
                    switch (state)
                    {
                        case StateStart:
                        default:
                        {
                            if (b < 0x7F)
                            {
                                @out.Append(unchecked((byte)b));
                            }
                            else
                            {
                                if (b >= 0xC0)
                                {
                                    // start of UTF8 sequence
                                    expectedBytes = -1;
                                    var test = b;
                                    for (; expectedBytes < 8 && (test & 0x80) == 0x80; test = test << 1)
                                    {
                                        expectedBytes++;
                                    }
                                    readAheadBuffer[readAhead++] = unchecked((byte)b);
                                    state = StateUtf8Char;
                                }
                                else
                                {
                                    //  implicitly:  b >= 0x80  &&  b < 0xC0
                                    // invalid UTF8 start char, assume to be Latin-1
                                    var utf8 = ConvertToUtf8(unchecked((byte)b));
                                    @out.Append(utf8);
                                }
                            }
                            break;
                        }

                        case StateUtf8Char:
                        {
                            if (expectedBytes > 0 && (b & 0xC0) == 0x80)
                            {
                                // valid UTF8 char, add to readAheadBuffer
                                readAheadBuffer[readAhead++] = unchecked((byte)b);
                                expectedBytes--;
                                if (expectedBytes == 0)
                                {
                                    @out.Append(readAheadBuffer, 0, readAhead);
                                    readAhead = 0;
                                    state = StateStart;
                                }
                            }
                            else
                            {
                                // invalid UTF8 char:
                                // 1. convert first of seq to UTF8
                                var utf8 = ConvertToUtf8(readAheadBuffer[0]);
                                @out.Append(utf8);
                                // 2. continue processing at second byte of sequence
                                i = i - readAhead;
                                readAhead = 0;
                                state = StateStart;
                            }
                            break;
                        }
                    }
                }
                // loop ends with "half" Utf8 char --> assume that the bytes are Latin-1
                if (state == StateUtf8Char)
                {
                    for (var j = 0; j < readAhead; j++)
                    {
                        var b = readAheadBuffer[j];
                        var utf8 = ConvertToUtf8(b);
                        @out.Append(utf8);
                    }
                }
                return @out;
            }
            // Latin-1 fixing applies only to UTF-8
            return buffer;
        }

        /// <summary>
        /// Converts a Cp1252 char (contains all Latin-1 chars above 0x80) into a
        /// UTF-8 byte sequence.
        /// </summary>
        /// <remarks>
        /// Converts a Cp1252 char (contains all Latin-1 chars above 0x80) into a
        /// UTF-8 byte sequence. The bytes 0x81, 0x8D, 0x8F, 0x90, and 0x9D are
        /// formally undefined by Windows 1252 and therefore replaced by a space
        /// (0x20).
        /// </remarks>
        /// <param name="ch">an Cp1252 / Latin-1 byte</param>
        /// <returns>Returns a byte array containing a UTF-8 byte sequence.</returns>
        private static byte[] ConvertToUtf8(byte ch)
        {
            var c = ch & 0xFF;
            if (c >= 0x80)
            {
                if (c == 0x81 || c == 0x8D || c == 0x8F || c == 0x90 || c == 0x9D)
                {
                    return new byte[] { 0x20 };
                }
                // space for undefined
                // interpret byte as Windows Cp1252 char
                return Encoding.UTF8.GetBytes(Encoding.GetEncoding(1252).GetString(new[] { ch }));
            }
            return new[] { ch };
        }
    }
}
