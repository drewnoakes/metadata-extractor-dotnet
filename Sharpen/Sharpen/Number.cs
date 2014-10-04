using System;

namespace Sharpen
{
    public class Number
    {
        private readonly decimal m_value;

        private Number(decimal value)
        {
            m_value = value;
        }
        
        public Number()
        {
        }

        public virtual double DoubleValue()
        {
            return (double) m_value;
        }

        public virtual float FloatValue()
        {
            return (float) m_value;
        }

        public virtual sbyte ByteValue()
        {
            return (sbyte) m_value;
        }

        public virtual int IntValue()
        {
            return (int) m_value;
        }

        public virtual long LongValue()
        {
            return (long) m_value;
        }

        public virtual short ShortValue()
        {
            return (short) m_value;
        }

        public static Number GetInstance(object obj)
        {
            if (obj is Number)
            {
                return (Number)obj;
            }

            if (!IsNumberType(obj))
            {
                throw new ArgumentException();
            }

            return new Number(Convert.ToDecimal(obj));
        }

        private static bool IsNumberType(object obj)
        {
            return obj is sbyte
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