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

using System;
using JetBrains.Annotations;
#if !NETSTANDARD1_3
using System.Globalization;
using System.ComponentModel;
#endif

// TODO operator overloads

namespace MetadataExtractor
{
    /// <summary>Immutable type for representing a rational number.</summary>
    /// <remarks>
    /// Underlying values are stored as a numerator and denominator, each of type <see cref="long"/>.
    /// Note that any <see cref="Rational"/> with a numerator of zero will be treated as zero, even if the denominator is also zero.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NETSTANDARD1_3
    [Serializable]
    [TypeConverter(typeof(RationalConverter))]
#endif
    public struct Rational : IConvertible, IEquatable<Rational>
    {
        /// <summary>Gets the denominator.</summary>
        public long Denominator { get; }

        /// <summary>Gets the numerator.</summary>
        public long Numerator { get; }

        /// <summary>Initialises a new instance with the <paramref name="numerator"/> and <paramref name="denominator"/>.</summary>
        public Rational(long numerator, long denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        #region Conversion methods

        /// <summary>Returns the value of the specified number as a <see cref="double"/>.</summary>
        /// <remarks>This may involve rounding.</remarks>
        public double ToDouble() => Numerator == 0 ? 0.0 : Numerator/(double)Denominator;

        /// <summary>Returns the value of the specified number as a <see cref="float"/>.</summary>
        /// <remarks>May incur rounding.</remarks>
        public float ToSingle() => Numerator == 0 ? 0.0f : Numerator/(float)Denominator;

        /// <summary>Returns the value of the specified number as a <see cref="byte"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="byte"/>.
        /// </remarks>
        public byte ToByte() => (byte)ToDouble();

        /// <summary>Returns the value of the specified number as a <see cref="sbyte"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="sbyte"/>.
        /// </remarks>
        public sbyte ToSByte() => (sbyte)ToDouble();

        /// <summary>Returns the value of the specified number as an <see cref="int"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="int"/>.
        /// </remarks>
        public int ToInt32() => (int)ToDouble();

        /// <summary>Returns the value of the specified number as an <see cref="uint"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="uint"/>.
        /// </remarks>
        public uint ToUInt32() => (uint)ToDouble();

        /// <summary>Returns the value of the specified number as a <see cref="long"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="long"/>.
        /// </remarks>
        public long ToInt64() => (long)ToDouble();

        /// <summary>Returns the value of the specified number as a <see cref="ulong"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="ulong"/>.
        /// </remarks>
        public ulong ToUInt64() => (ulong)ToDouble();

        /// <summary>Returns the value of the specified number as a <see cref="short"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="short"/>.
        /// </remarks>
        public short ToInt16() => (short)ToDouble();

        /// <summary>Returns the value of the specified number as a <see cref="ushort"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="ushort"/>.
        /// </remarks>
        public ushort ToUInt16() => (ushort)ToDouble();

        /// <summary>Returns the value of the specified number as a <see cref="decimal"/>.</summary>
        /// <remarks>May incur truncation.</remarks>
        public decimal ToDecimal() => Denominator == 0 ? 0M : Numerator / (decimal)Denominator;

        /// <summary>Returns <c>true</c> if the value is non-zero, otherwise <c>false</c>.</summary>
        public bool ToBoolean() => Numerator != 0 && Denominator != 0;

        #region IConvertible

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

        bool IConvertible.ToBoolean(IFormatProvider provider) => ToBoolean();

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider) => ToSByte();

        byte IConvertible.ToByte(IFormatProvider provider) => ToByte();

        short IConvertible.ToInt16(IFormatProvider provider) => ToInt16();

        ushort IConvertible.ToUInt16(IFormatProvider provider) => ToUInt16();

        int IConvertible.ToInt32(IFormatProvider provider) => ToInt32();

        uint IConvertible.ToUInt32(IFormatProvider provider) => ToUInt32();

        long IConvertible.ToInt64(IFormatProvider provider) => ToInt64();

        ulong IConvertible.ToUInt64(IFormatProvider provider) => ToUInt64();

        float IConvertible.ToSingle(IFormatProvider provider) => ToSingle();

        double IConvertible.ToDouble(IFormatProvider provider) => ToDouble();

        decimal IConvertible.ToDecimal(IFormatProvider provider) => ToDecimal();

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        /// <summary>Gets the reciprocal value of this object as a new <see cref="Rational"/>.</summary>
        /// <value>the reciprocal in a new object</value>
        public Rational Reciprocal => new Rational(Denominator, Numerator);

        /// <summary>
        /// Checks if this <see cref="Rational"/> number is expressible as an integer, either positive or negative.
        /// </summary>
        public bool IsInteger => Denominator == 1 || (Denominator != 0 && Numerator%Denominator == 0) || (Denominator == 0 && Numerator == 0);

        /// <summary>
        /// True if either <see cref="Denominator"/> or <see cref="Numerator"/> are zero.
        /// </summary>
        public bool IsZero => Denominator == 0 || Numerator == 0;

        #region Formatting

        /// <summary>Returns a string representation of the object of form <c>numerator/denominator</c>.</summary>
        /// <returns>a string representation of the object.</returns>
        public override string ToString() => Numerator + "/" + Denominator;

        public string ToString(IFormatProvider provider) => Numerator.ToString(provider) + "/" + Denominator.ToString(provider);

        /// <summary>
        /// Returns the simplest representation of this <see cref="Rational"/>'s value possible.
        /// </summary>
        [NotNull]
        public string ToSimpleString(bool allowDecimal = true, IFormatProvider provider = null)
        {
            if (Denominator == 0 && Numerator != 0)
                return ToString(provider);

            if (IsInteger)
                return ToInt32().ToString(provider);

            if (Numerator != 1 && Denominator%Numerator == 0)
            {
                // common factor between denominator and numerator
                var newDenominator = Denominator/Numerator;
                return new Rational(1, newDenominator).ToSimpleString(allowDecimal, provider);
            }

            var simplifiedInstance = GetSimplifiedInstance();
            if (allowDecimal)
            {
                var doubleString = simplifiedInstance.ToDouble().ToString(provider);
                if (doubleString.Length < 5)
                    return doubleString;
            }

            return simplifiedInstance.ToString(provider);
        }

        #endregion

        #region Equality and hashing

        /// <summary>
        /// Indicates whether this instance and <paramref name="other"/> are numerically equal,
        /// even if their representations differ.
        /// </summary>
        /// <remarks>
        /// For example, <c>1/2</c> is equal to <c>10/20</c> by this method.
        /// Similarly, <c>1/0</c> is equal to <c>100/0</c> by this method.
        /// To test equal representations, use <see cref="EqualsExact"/>.
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Rational other) => other.ToDecimal().Equals(ToDecimal());

        /// <summary>
        /// Indicates whether this instance and <paramref name="other"/> have identical
        /// <see cref="Numerator"/> and <see cref="Denominator"/>.
        /// </summary>
        /// <remarks>
        /// For example, <c>1/2</c> is not equal to <c>10/20</c> by this method.
        /// Similarly, <c>1/0</c> is not equal to <c>100/0</c> by this method.
        /// To test numerically equivalence, use <see cref="Equals(MetadataExtractor.Rational)"/>.
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsExact(Rational other) => Denominator == other.Denominator && Numerator == other.Numerator;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Rational rational && Equals(rational);
        }

        public override int GetHashCode() => unchecked(Denominator.GetHashCode()*397) ^ Numerator.GetHashCode();

        #endregion

        /// <summary>
        /// Simplifies the representation of this <see cref="Rational"/> number.
        /// </summary>
        /// <remarks>
        /// For example, <c>5/10</c> simplifies to <c>1/2</c> because both <see cref="Numerator"/>
        /// and <see cref="Denominator"/> share a common factor of 5.
        /// <para />
        /// Uses the Euclidean Algorithm to find the greatest common divisor.
        /// </remarks>
        /// <returns>
        /// A simplified instance if one exists, otherwise a copy of the original value.
        /// </returns>
        public Rational GetSimplifiedInstance()
        {
            long GCD(long a, long b)
            {
                if (a < 0)
                    a = -a;
                if (b < 0)
                    b = -b;

                while (a != 0 && b != 0)
                {
                    if (a > b)
                        a %= b;
                    else
                        b %= a;
                }

                return a == 0 ? b : a;
            }

            var gcd = GCD(Numerator, Denominator);

            return new Rational(Numerator / gcd, Denominator / gcd);
        }

        #region Equality operators

        public static bool operator==(Rational a, Rational b)
        {
            return Equals(a, b);
        }

        public static bool operator!=(Rational a, Rational b)
        {
            return !Equals(a, b);
        }

        #endregion

        #region RationalConverter

#if !NETSTANDARD1_3
        private sealed class RationalConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string) ||
                    sourceType == typeof(Rational) ||
                    typeof(IConvertible).IsAssignableFrom(sourceType) ||
                    (sourceType.IsArray && typeof(IConvertible).IsAssignableFrom(sourceType.GetElementType())))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null)
                    return base.ConvertFrom(context, culture, null);

                var type = value.GetType();

                if (type == typeof(string))
                {
                    var v = ((string)value).Split('/');
                    if (v.Length == 2 && long.TryParse(v[0], out long numerator) && long.TryParse(v[1], out long denominator))
                        return new Rational(numerator, denominator);
                }

                if (type == typeof(Rational))
                    return value;

                if (type.IsArray)
                {
                    var array = (Array)value;
                    if (array.Rank == 1 && (array.Length == 1 || array.Length == 2))
                    {
                        return new Rational(
                            numerator: Convert.ToInt64(array.GetValue(0)),
                            denominator: array.Length == 2 ? Convert.ToInt64(array.GetValue(1)) : 1);
                    }
                }

                return new Rational(Convert.ToInt64(value), 1);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => false;
        }
#endif

        #endregion
    }
}
