#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

// TODO operator overloads

namespace MetadataExtractor
{
    /// <summary>Immutable type for representing a rational number.</summary>
    /// <remarks>
    /// Underlying values are stored as a numerator and denominator, each of type <see cref="long"/>.
    /// Note that any <see cref="Rational"/> with a numerator of zero will be treated as zero, even if the denominator is also zero.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    [TypeConverter(typeof(RationalConverter))]
    public sealed class Rational : IConvertible
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
        public double ToDouble()
        {
            return Numerator == 0 ? 0.0 : Numerator/(double)Denominator;
        }

        /// <summary>Returns the value of the specified number as a <see cref="float"/>.</summary>
        /// <remarks>May incur rounding.</remarks>
        public float ToSingle()
        {
            return Numerator == 0 ? 0.0f : Numerator/(float)Denominator;
        }

        /// <summary>Returns the value of the specified number as a <see cref="byte"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="byte"/>.
        /// </remarks>
        public byte ToByte()
        {
            return (byte)ToDouble();
        }

        /// <summary>Returns the value of the specified number as a <see cref="sbyte"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="sbyte"/>.
        /// </remarks>
        public sbyte ToSByte()
        {
            return (sbyte)ToDouble();
        }

        /// <summary>Returns the value of the specified number as an <see cref="int"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="int"/>.
        /// </remarks>
        public int ToInt32()
        {
            return (int)ToDouble();
        }

        /// <summary>Returns the value of the specified number as an <see cref="uint"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="uint"/>.
        /// </remarks>
        public uint ToUInt32()
        {
            return (uint)ToDouble();
        }

        /// <summary>Returns the value of the specified number as a <see cref="long"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="long"/>.
        /// </remarks>
        public long ToInt64()
        {
            return (long)ToDouble();
        }

        /// <summary>Returns the value of the specified number as a <see cref="ulong"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="ulong"/>.
        /// </remarks>
        public ulong ToUInt64()
        {
            return (ulong)ToDouble();
        }

        /// <summary>Returns the value of the specified number as a <see cref="short"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="short"/>.
        /// </remarks>
        public short ToInt16()
        {
            return (short)ToDouble();
        }

        /// <summary>Returns the value of the specified number as a <see cref="ushort"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="ToDouble"/> to <see cref="ushort"/>.
        /// </remarks>
        public ushort ToUInt16()
        {
            return (ushort)ToDouble();
        }

        /// <summary>Returns the value of the specified number as a <see cref="decimal"/>.</summary>
        /// <remarks>May incur truncation.</remarks>
        public decimal ToDecimal()
        {
            return Denominator == 0 ? 0M : Numerator / (decimal)Denominator;
        }

        /// <summary>Returns <c>true</c> if the value is non-zero, otherwise <c>false</c>.</summary>
        public bool ToBoolean()
        {
            return Numerator != 0 && Denominator != 0;
        }

        #region IConvertible

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ToBoolean();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ToSByte();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ToByte();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ToInt16();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ToUInt16();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ToInt32();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ToUInt32();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ToInt64();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ToUInt64();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ToSingle();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ToDouble();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ToDecimal();
        }

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
        [NotNull]
        public Rational Reciprocal => new Rational(Denominator, Numerator);

        /// <summary>
        /// Checks if this <see cref="Rational"/> number is expressible as an integer, either positive or negative.
        /// </summary>
        public bool IsInteger => Denominator == 1 || (Denominator != 0 && (Numerator%Denominator == 0)) || (Denominator == 0 && Numerator == 0);

        #region Formatting

        /// <summary>Returns a string representation of the object of form <c>numerator/denominator</c>.</summary>
        /// <returns>a string representation of the object.</returns>
        public override string ToString()
        {
            return Numerator + "/" + Denominator;
        }

        public string ToString(IFormatProvider provider)
        {
            return Numerator.ToString(provider) + "/" + Denominator.ToString(provider);
        }

        /// <summary>
        /// Returns the simplest representation of this <see cref="Rational"/>'s value possible.
        /// </summary>
        [NotNull]
        public string ToSimpleString(bool allowDecimal = true)
        {
            if (Denominator == 0 && Numerator != 0)
                return ToString();

            if (IsInteger)
                return ToInt32().ToString();

            if (Numerator != 1 && Denominator%Numerator == 0)
            {
                // common factor between denominator and numerator
                var newDenominator = Denominator/Numerator;
                return new Rational(1, newDenominator).ToSimpleString(allowDecimal);
            }

            var simplifiedInstance = GetSimplifiedInstance();
            if (allowDecimal)
            {
                var doubleString = simplifiedInstance.ToDouble().ToString();
                if (doubleString.Length < 5)
                    return doubleString;
            }

            return simplifiedInstance.ToString();
        }

        /// <summary>
        /// Decides whether a brute-force simplification calculation should be avoided
        /// by comparing the maximum number of possible calculations with some threshold.
        /// </summary>
        /// <returns>true if the simplification should be performed, otherwise false</returns>
        private bool TooComplexForSimplification()
        {
            var maxPossibleCalculations = (((Math.Min(Denominator, Numerator) - 1)/5d) + 2);
            const int maxSimplificationCalculations = 1000;
            return maxPossibleCalculations > maxSimplificationCalculations;
        }

        #endregion

        #region Equality and hashing

        private bool Equals(Rational other)
        {
            return Denominator == other.Denominator && Numerator == other.Numerator;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is Rational && ((Rational)obj).ToDecimal().Equals(ToDecimal());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Denominator.GetHashCode()*397) ^ Numerator.GetHashCode();
            }
        }

        #endregion

        /// <summary>
        /// Simplifies the <see cref="Rational"/> number.
        /// </summary>
        /// <remarks>
        /// Prime number series: 1, 2, 3, 5, 7, 9, 11, 13, 17
        /// <para />
        /// To reduce a rational, need to see if both numerator and denominator are divisible
        /// by a common factor.  Using the prime number series in ascending order guarantees
        /// the minimum number of checks required.
        /// <para />
        /// However, generating the prime number series seems to be a hefty task.  Perhaps
        /// it's simpler to check if both d &amp; n are divisible by all numbers from 2
        /// -&gt; (Math.min(denominator, numerator) / 2). In doing this, one can check for 2
        /// and 5 once, then ignore all even numbers, and all numbers ending in 0 or 5.
        /// This leaves four numbers from every ten to check.
        /// <para />
        /// Therefore, the max number of pairs of modulus divisions required will be:
        /// <code>
        ///  4   Math.min(denominator, numerator) - 1
        /// -- * ------------------------------------ + 2
        /// 10                    2
        ///      Math.min(denominator, numerator) - 1
        ///    = ------------------------------------ + 2
        ///                       5
        /// </code>
        /// </remarks>
        /// <returns>
        /// A simplified instance, or if the Rational could not be simplified,
        /// returns itself unchanged.
        /// </returns>
        [NotNull]
        public Rational GetSimplifiedInstance()
        {
            if (TooComplexForSimplification())
                return this;

            for (var factor = 2; factor <= Math.Min(Denominator, Numerator); factor++)
            {
                if ((factor%2 == 0 && factor > 2) || (factor%5 == 0 && factor > 5))
                    continue;

                if (Denominator % factor == 0 && Numerator % factor == 0)
                {
                    // found a common factor
                    return new Rational(Numerator / factor, Denominator / factor);
                }
            }

            return this;
        }

        #region RationalConverter

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
                    return base.ConvertFrom(context, culture, value);

                var type = value.GetType();

                if (type == typeof(string))
                {
                    var v = ((string)value).Split('/');
                    long numerator;
                    long denominator;
                    if (v.Length == 2 && long.TryParse(v[0], out numerator) && long.TryParse(v[1], out denominator))
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

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return false;
            }
        }

        #endregion
    }
}
