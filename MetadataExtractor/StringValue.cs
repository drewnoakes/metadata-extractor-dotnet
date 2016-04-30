using System;
using System.Text;

using JetBrains.Annotations;

namespace MetadataExtractor
{
    public sealed class StringValue : IConvertible
    {
        /// <summary>
        /// The encoding used when decoding a <see cref="StringValue"/> that does not specify its encoding.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public StringValue([NotNull] byte[] bytes, Encoding encoding = null)
        {
            Bytes = bytes;
            Encoding = encoding;
        }

        public byte[] Bytes { get; }

        [CanBeNull]
        public Encoding Encoding { get; }

        #region IConvertible

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

        string IConvertible.ToString(IFormatProvider provider) => ToString();

        double IConvertible.ToDouble(IFormatProvider provider) => double.Parse(ToString());

        decimal IConvertible.ToDecimal(IFormatProvider prodiver) => decimal.Parse(ToString());

        float IConvertible.ToSingle(IFormatProvider provider) => float.Parse(ToString());

        bool IConvertible.ToBoolean(IFormatProvider provider) => bool.Parse(ToString());

        byte IConvertible.ToByte(IFormatProvider provider) => byte.Parse(ToString());

        char IConvertible.ToChar(IFormatProvider provider)
        {
            var s = ToString();
            if (s.Length != 1)
                throw new FormatException();
            return s[0];
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => DateTime.Parse(ToString());

        short IConvertible.ToInt16(IFormatProvider provider) => short.Parse(ToString());

        int IConvertible.ToInt32(IFormatProvider provider) => int.Parse(ToString());

        long IConvertible.ToInt64(IFormatProvider provider) => long.Parse(ToString());

        sbyte IConvertible.ToSByte(IFormatProvider provider) => sbyte.Parse(ToString());

        ushort IConvertible.ToUInt16(IFormatProvider provider) => ushort.Parse(ToString());

        uint IConvertible.ToUInt32(IFormatProvider provider) => uint.Parse(ToString());

        ulong IConvertible.ToUInt64(IFormatProvider provider) => ulong.Parse(ToString());

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(ToString(), conversionType, provider);

        #endregion

        #region Formatting

        public override string ToString()
        {
            return ToString(Encoding ?? DefaultEncoding);
        }

        public string ToString([NotNull] Encoding encoder)
        {
            return encoder.GetString(Bytes, 0, Bytes.Length);
        }

        #endregion
    }
}
