using System;

namespace Sharpen
{
    public class Number
    {
        private readonly decimal _mValue;

        private Number(decimal value)
        {
            _mValue = value;
        }

        public Number()
        {
        }

        public virtual double DoubleValue()
        {
            return (double) _mValue;
        }

        public virtual float FloatValue()
        {
            return (float) _mValue;
        }

        public virtual byte ByteValue()
        {
            return (byte) _mValue;
        }

        public virtual int IntValue()
        {
            return (int) _mValue;
        }

        public virtual long LongValue()
        {
            return (long) _mValue;
        }

        public virtual short ShortValue()
        {
            return (short) _mValue;
        }

        public static Number GetInstance(object obj)
        {
            var number = obj as Number;
            if (number != null)
            {
                return number;
            }

            if (!IsNumberType(obj))
            {
                throw new ArgumentException();
            }

            return new Number(Convert.ToDecimal(obj));
        }

        private static bool IsNumberType(object obj)
        {
            return obj is byte
                || obj is byte
                || obj is short
                || obj is ushort
                || obj is int
                || obj is uint
                || obj is long
                || obj is ulong
                || obj is float
                || obj is double
                || obj is decimal;
        }
    }
}