﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor
{
    /// <summary>
    /// Wraps a byte array with an <see cref="Encoding"/>. Allows consumers to override the encoding if required.
    /// </summary>
    /// <remarks>
    /// String data is often in the incorrect format, and many issues have been raised in the past related to string
    /// encoding. Metadata Extractor used to decode string bytes at read-time, after which it was not possible to
    /// override the encoding at a later time by the user.
    /// <para />
    /// The introduction of this type allows full transparency and control over the use of string data extracted
    /// by the library during the read phase.
    /// </remarks>
    public readonly struct StringValue(byte[] bytes, Encoding? encoding = null) : IConvertible
    {
        /// <summary>
        /// The encoding used when decoding a <see cref="StringValue"/> that does not specify its encoding.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public byte[] Bytes { get; } = bytes;

        public Encoding? Encoding { get; } = encoding;

        #region IConvertible

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

        string IConvertible.ToString(IFormatProvider? provider) => ToString();

        double IConvertible.ToDouble(IFormatProvider? provider) => double.Parse(ToString());

        decimal IConvertible.ToDecimal(IFormatProvider? provider) => decimal.Parse(ToString());

        float IConvertible.ToSingle(IFormatProvider? provider) => float.Parse(ToString());

        bool IConvertible.ToBoolean(IFormatProvider? provider) => bool.Parse(ToString());

        byte IConvertible.ToByte(IFormatProvider? provider) => byte.Parse(ToString());

        char IConvertible.ToChar(IFormatProvider? provider)
        {
            var s = ToString();
            if (s.Length != 1)
                throw new FormatException();
            return s[0];
        }

        DateTime IConvertible.ToDateTime(IFormatProvider? provider) => DateTime.Parse(ToString());

        short IConvertible.ToInt16(IFormatProvider? provider) => short.Parse(ToString());

        int IConvertible.ToInt32(IFormatProvider? provider)
        {
            try
            {
#if NETSTANDARD2_0
                return int.Parse(ToString());
#else
                if (Bytes.Length < 100)
                {
                    var encoding = Encoding ?? DefaultEncoding;
                    int charCount = encoding.GetCharCount(Bytes);
                    Span<char> chars = stackalloc char[charCount];
                    encoding.GetChars(Bytes, chars);
                    return int.Parse(chars);
                }
                else
                {
                    return int.Parse(ToString());
                }
#endif
            }
            catch (Exception)
            {
                long val = 0;
                foreach (var b in Bytes)
                {
                    val <<= 8;
                    val += b;
                }
                return (int)val;
            }
        }

        long IConvertible.ToInt64(IFormatProvider? provider) => long.Parse(ToString());

        sbyte IConvertible.ToSByte(IFormatProvider? provider) => sbyte.Parse(ToString());

        ushort IConvertible.ToUInt16(IFormatProvider? provider) => ushort.Parse(ToString());

        uint IConvertible.ToUInt32(IFormatProvider? provider)
        {
            try
            {
#if NETSTANDARD2_0
                return uint.Parse(ToString());
#else
                if (Bytes.Length < 100)
                {
                    var encoding = Encoding ?? DefaultEncoding;
                    int charCount = encoding.GetCharCount(Bytes);
                    Span<char> chars = stackalloc char[charCount];
                    encoding.GetChars(Bytes, chars);
                    return uint.Parse(chars);
                }
                else
                {
                    return uint.Parse(ToString());
                }
#endif
            }
            catch (Exception)
            {
                ulong val = 0;
                foreach (var b in Bytes)
                {
                    val <<= 8;
                    val += b;
                }
                return (uint)val;
            }
        }

        ulong IConvertible.ToUInt64(IFormatProvider? provider) => ulong.Parse(ToString());

        object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(ToString(), conversionType, provider);

        #endregion

        #region Formatting

        public override string ToString() => ToString(Encoding ?? DefaultEncoding);

        public string ToString(Encoding encoding) => encoding.GetString(Bytes, 0, Bytes.Length);

        public string ToString(int index, int count) => (Encoding ?? DefaultEncoding).GetString(Bytes, index, count);

        #endregion
    }
}
