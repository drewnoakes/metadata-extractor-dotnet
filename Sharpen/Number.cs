using System;

namespace Sharpen
{
    public sealed class Number
    {
        private readonly decimal _mValue;

        private Number(decimal value)
        {
            _mValue = value;
        }

        public double DoubleValue()
        {
            return (double) _mValue;
        }

        public float FloatValue()
        {
            return (float) _mValue;
        }

        public byte ByteValue()
        {
            return (byte) _mValue;
        }

        public int IntValue()
        {
            return (int) _mValue;
        }

        public long LongValue()
        {
            return (long) _mValue;
        }

        public short ShortValue()
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
                || obj is sbyte
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