using System;
using System.Text;

using JetBrains.Annotations;

namespace MetadataExtractor
{
    public sealed class StringValue : IConvertible
    {
        private readonly byte[] _buffer;
        private Encoding _encoding = Encoding.UTF8;

        public StringValue([NotNull] byte[] bytes)
        {
            _buffer = bytes;
        }

        public StringValue([NotNull] byte[] bytes, Encoding defaultEncoding)
        {
            _buffer = bytes;
            _encoding = defaultEncoding;
        }

        public byte[] Bytes
        {
            get { return _buffer; }
        }

        public void SetEncodingByName(string encodingName)
        {
            _encoding = Encoding.GetEncoding(encodingName);
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public int Length
        {
            get { return (_buffer != null) ? _buffer.Length : 0; }
        }

        #region Conversion methods

        public double ToDouble()
        {
            double output;
            if (!double.TryParse(ToString(), out output))
                throw new FormatException();

            return output;
        }

        public float ToSingle()
        {
            return (float)ToDouble();
        }

        public string ToString(IFormatProvider provider) => ToString();

        #region IConvertible
        double IConvertible.ToDouble(IFormatProvider provider) => ToDouble();

        decimal IConvertible.ToDecimal(IFormatProvider prodiver) => (decimal)ToDouble();

        float IConvertible.ToSingle(IFormatProvider provider) => ToSingle();

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Formatting

        public override string ToString()
        {
            return ToString(_encoding);
        }
        public string ToString(Encoding encoder)
        {
            return encoder.GetString(_buffer, 0, Length);
        }

        #endregion

        // user-defined conversion from StringValue to Double
        /*public static implicit operator double(StringValue strval)
        {
            double value;
            if (double.TryParse(strval.ToString(), out value))
                return value;

            throw new MetadataException($"Tag cannot be converted to {typeof(double).Name}.");
        }*/

        // user-defined conversion from StringValue to string
        /*public static implicit operator string(StringValue strval)
        {
            return strval.ToString();
        }*/

        // user-defined conversion from string to StringValue
        /*public static implicit operator StringValue(string str)
        {
            return new StringValue(Encoding.UTF8.GetBytes(str));
        }*/

    }
}
