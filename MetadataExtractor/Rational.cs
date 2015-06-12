/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using JetBrains.Annotations;

namespace MetadataExtractor
{
    /// <summary>Immutable type for representing a rational number.</summary>
    /// <remarks>
    /// Underlying values are stored as a numerator and denominator, each of type <see cref="long"/>.
    /// Note that any <see cref="Rational"/> with a numerator of zero will be treated as zero, even if the denominator is also zero.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public sealed class Rational
    {
        /// <summary>Holds the numerator.</summary>
        private readonly long _numerator;

        /// <summary>Holds the denominator.</summary>
        private readonly long _denominator;

        /// <summary>Initialises a new instance with the <paramref name="numerator"/> and <paramref name="denominator"/>.</summary>
        public Rational(long numerator, long denominator)
        {
            _numerator = numerator;
            _denominator = denominator;
        }

        /// <summary>Returns the value of the specified number as a <see cref="double"/>.</summary>
        /// <remarks>This may involve rounding.</remarks>
        public double DoubleValue()
        {
            return _numerator == 0 ? 0.0 : _numerator / (double)_denominator;
        }

        /// <summary>Returns the value of the specified number as a <see cref="float"/>.</summary>
        /// <remarks>May incur rounding.</remarks>
        public float FloatValue()
        {
            return _numerator == 0 ? 0.0f : _numerator / (float)_denominator;
        }

        /// <summary>Returns the value of the specified number as a <see cref="byte"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="byte"/>.
        /// </remarks>
        public byte ByteValue()
        {
            return (byte)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <see cref="sbyte"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="sbyte"/>.
        /// </remarks>
        public sbyte SByteValue()
        {
            return (sbyte)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as an <see cref="int"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="int"/>.
        /// </remarks>
        public int IntValue()
        {
            return (int)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as an <see cref="uint"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="uint"/>.
        /// </remarks>
        public uint UIntValue()
        {
            return (uint)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <see cref="long"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="long"/>.
        /// </remarks>
        public long LongValue()
        {
            return (long)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <see cref="ulong"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="ulong"/>.
        /// </remarks>
        public ulong ULongValue()
        {
            return (ulong)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <see cref="short"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="short"/>.
        /// </remarks>
        public short ShortValue()
        {
            return (short)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <see cref="ushort"/>.</summary>
        /// <remarks>
        /// May incur rounding or truncation.  This implementation simply
        /// casts the result of <see cref="DoubleValue()"/> to <see cref="ushort"/>.
        /// </remarks>
        public ushort UShortValue()
        {
            return (ushort)DoubleValue();
        }

        /// <summary>Returns the denominator.</summary>
        public long GetDenominator()
        {
            return _denominator;
        }

        /// <summary>Returns the numerator.</summary>
        public long GetNumerator()
        {
            return _numerator;
        }

        /// <summary>Returns the reciprocal value of this object as a new Rational.</summary>
        /// <returns>the reciprocal in a new object</returns>
        [NotNull]
        public Rational GetReciprocal()
        {
            return new Rational(_denominator, _numerator);
        }

        /// <summary>
        /// Checks if this <see cref="Rational"/> number is an Integer, either positive or negative.
        /// </summary>
        public bool IsInteger()
        {
            return _denominator == 1 || (_denominator != 0 && (_numerator % _denominator == 0)) || (_denominator == 0 && _numerator == 0);
        }

        /// <summary>Returns a string representation of the object of form <c>numerator/denominator</c>.</summary>
        /// <returns>a string representation of the object.</returns>
        public override string ToString()
        {
            return _numerator + "/" + _denominator;
        }

        /// <summary>
        /// Returns the simplest representation of this <see cref="Rational"/>'s value possible.
        /// </summary>
        [NotNull]
        public string ToSimpleString(bool allowDecimal)
        {
            if (_denominator == 0 && _numerator != 0)
            {
                return this.ToString();
            }
            if (IsInteger())
            {
                return IntValue().ToString();
            }
            if (_numerator != 1 && _denominator % _numerator == 0)
            {
                // common factor between denominator and numerator
                var newDenominator = _denominator / _numerator;
                return new Rational(1, newDenominator).ToSimpleString(allowDecimal);
            }
            var simplifiedInstance = GetSimplifiedInstance();
            if (allowDecimal)
            {
                var doubleString = ((object)simplifiedInstance.DoubleValue()).ToString();
                if (doubleString.Length < 5)
                {
                    return doubleString;
                }
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
            var maxPossibleCalculations = (((Math.Min(_denominator, _numerator) - 1) / 5d) + 2);
            const int maxSimplificationCalculations = 1000;
            return maxPossibleCalculations > maxSimplificationCalculations;
        }

        /// <summary>
        /// Compares two <see cref="Rational"/> instances, returning true if they are mathematically equivalent.
        /// </summary>
        /// <param name="obj">the <see cref="Rational"/> to compare this instance to.</param>
        /// <returns>
        /// true if instances are mathematically equivalent, otherwise false.  Will also
        /// return false if <c>obj</c> is not an instance of <see cref="Rational"/>.
        /// </returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (obj == null || !(obj is Rational))
            {
                return false;
            }
            var that = (Rational)obj;
            return DoubleValue() == that.DoubleValue();
        }

        public override int GetHashCode()
        {
            return (23 * (int)_denominator) + (int)_numerator;
        }

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
        /// <literal>-&gt;</literal>
        /// (Math.min(denominator, numerator) / 2).  In doing this, one can check for 2
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
        /// a simplified instance, or if the Rational could not be simplified,
        /// returns itself (unchanged)
        /// </returns>
        [NotNull]
        public Rational GetSimplifiedInstance()
        {
            if (TooComplexForSimplification())
            {
                return this;
            }
            for (var factor = 2; factor <= Math.Min(_denominator, _numerator); factor++)
            {
                if ((factor % 2 == 0 && factor > 2) || (factor % 5 == 0 && factor > 5))
                {
                    continue;
                }
                if (_denominator % factor == 0 && _numerator % factor == 0)
                {
                    // found a common factor
                    return new Rational(_numerator / factor, _denominator / factor);
                }
            }
            return this;
        }
    }
}
