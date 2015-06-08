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
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>Immutable class for holding a rational number without loss of precision.</summary>
    /// <remarks>
    /// Immutable class for holding a rational number without loss of precision.  Provides
    /// a familiar representation via <see cref="ToString"/> in form <c>numerator/denominator</c>.
    /// Note that any value with a numerator of zero will be treated as zero, even if the
    /// denominator is also zero.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public sealed class Rational : Number
    {
        /// <summary>Holds the numerator.</summary>
        private readonly long _numerator;

        /// <summary>Holds the denominator.</summary>
        private readonly long _denominator;

        /// <summary>Creates a new instance of Rational.</summary>
        /// <remarks>
        /// Creates a new instance of Rational.  Rational objects are immutable, so
        /// once you've set your numerator and denominator values here, you're stuck
        /// with them!
        /// </remarks>
        public Rational(long numerator, long denominator)
        {
            _numerator = numerator;
            _denominator = denominator;
        }

        /// <summary>Returns the value of the specified number as a <c>double</c>.</summary>
        /// <remarks>
        /// Returns the value of the specified number as a <c>double</c>.
        /// This may involve rounding.
        /// </remarks>
        /// <returns>
        /// the numeric value represented by this object after conversion
        /// to type <c>double</c>.
        /// </returns>
        public override double DoubleValue()
        {
            return _numerator == 0 ? 0.0 : _numerator / (double)_denominator;
        }

        /// <summary>Returns the value of the specified number as a <c>float</c>.</summary>
        /// <remarks>
        /// Returns the value of the specified number as a <c>float</c>.
        /// This may involve rounding.
        /// </remarks>
        /// <returns>
        /// the numeric value represented by this object after conversion
        /// to type <c>float</c>.
        /// </returns>
        public override float FloatValue()
        {
            return _numerator == 0 ? 0.0f : _numerator / (float)_denominator;
        }

        /// <summary>Returns the value of the specified number as a <c>byte</c>.</summary>
        /// <remarks>
        /// Returns the value of the specified number as a <c>byte</c>.
        /// This may involve rounding or truncation.  This implementation simply
        /// casts the result of
        /// <see cref="DoubleValue()"/>
        /// to <c>byte</c>.
        /// </remarks>
        /// <returns>
        /// the numeric value represented by this object after conversion
        /// to type <c>byte</c>.
        /// </returns>
        public sealed override sbyte ByteValue()
        {
            return unchecked((sbyte)DoubleValue());
        }

        /// <summary>Returns the value of the specified number as an <c>int</c>.</summary>
        /// <remarks>
        /// Returns the value of the specified number as an <c>int</c>.
        /// This may involve rounding or truncation.  This implementation simply
        /// casts the result of
        /// <see cref="DoubleValue()"/>
        /// to <c>int</c>.
        /// </remarks>
        /// <returns>
        /// the numeric value represented by this object after conversion
        /// to type <c>int</c>.
        /// </returns>
        public sealed override int IntValue()
        {
            return (int)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <c>long</c>.</summary>
        /// <remarks>
        /// Returns the value of the specified number as a <c>long</c>.
        /// This may involve rounding or truncation.  This implementation simply
        /// casts the result of
        /// <see cref="DoubleValue()"/>
        /// to <c>long</c>.
        /// </remarks>
        /// <returns>
        /// the numeric value represented by this object after conversion
        /// to type <c>long</c>.
        /// </returns>
        public sealed override long LongValue()
        {
            return (long)DoubleValue();
        }

        /// <summary>Returns the value of the specified number as a <c>short</c>.</summary>
        /// <remarks>
        /// Returns the value of the specified number as a <c>short</c>.
        /// This may involve rounding or truncation.  This implementation simply
        /// casts the result of
        /// <see cref="DoubleValue()"/>
        /// to <c>short</c>.
        /// </remarks>
        /// <returns>
        /// the numeric value represented by this object after conversion
        /// to type <c>short</c>.
        /// </returns>
        public sealed override short ShortValue()
        {
            return (short)DoubleValue();
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
        /// Checks if this
        /// <see cref="Rational"/>
        /// number is an Integer, either positive or negative.
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
        /// Returns the simplest representation of this
        /// <see cref="Rational"/>
        /// 's value possible.
        /// </summary>
        [NotNull]
        public string ToSimpleString(bool allowDecimal)
        {
            if (_denominator == 0 && _numerator != 0)
            {
                return Extensions.ConvertToString(this);
            }
            if (IsInteger())
            {
                return Extensions.ConvertToString(IntValue());
            }
            if (_numerator != 1 && _denominator % _numerator == 0)
            {
                // common factor between denominator and numerator
                long newDenominator = _denominator / _numerator;
                return new Rational(1, newDenominator).ToSimpleString(allowDecimal);
            }
            Rational simplifiedInstance = GetSimplifiedInstance();
            if (allowDecimal)
            {
                string doubleString = Extensions.ConvertToString(simplifiedInstance.DoubleValue());
                if (doubleString.Length < 5)
                {
                    return doubleString;
                }
            }
            return Extensions.ConvertToString(simplifiedInstance);
        }

        /// <summary>
        /// Decides whether a brute-force simplification calculation should be avoided
        /// by comparing the maximum number of possible calculations with some threshold.
        /// </summary>
        /// <returns>true if the simplification should be performed, otherwise false</returns>
        private bool TooComplexForSimplification()
        {
            double maxPossibleCalculations = (((Math.Min(_denominator, _numerator) - 1) / 5d) + 2);
            int maxSimplificationCalculations = 1000;
            return maxPossibleCalculations > maxSimplificationCalculations;
        }

        /// <summary>
        /// Compares two
        /// <see cref="Rational"/>
        /// instances, returning true if they are mathematically
        /// equivalent.
        /// </summary>
        /// <param name="obj">
        /// the
        /// <see cref="Rational"/>
        /// to compare this instance to.
        /// </param>
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
            Rational that = (Rational)obj;
            return DoubleValue() == that.DoubleValue();
        }

        public override int GetHashCode()
        {
            return (23 * (int)_denominator) + (int)_numerator;
        }

        /// <summary>
        /// <para>Simplifies the <see cref="Rational"/> number.</para>
        /// <para>
        /// Prime number series: 1, 2, 3, 5, 7, 9, 11, 13, 17</para>
        /// <para>
        /// To reduce a rational, need to see if both numerator and denominator are divisible
        /// by a common factor.  Using the prime number series in ascending order guarantees
        /// the minimum number of checks required.</para>
        /// <para>
        /// However, generating the prime number series seems to be a hefty task.  Perhaps
        /// it's simpler to check if both d &amp; n are divisible by all numbers from 2
        /// <literal>-&gt;</literal>
        /// (Math.min(denominator, numerator) / 2).  In doing this, one can check for 2
        /// and 5 once, then ignore all even numbers, and all numbers ending in 0 or 5.
        /// This leaves four numbers from every ten to check.</para>
        /// <para>
        /// Therefore, the max number of pairs of modulus divisions required will be:</para>
        /// <pre><c>
        ///  4   Math.min(denominator, numerator) - 1
        /// -- * ------------------------------------ + 2
        /// 10                    2
        ///      Math.min(denominator, numerator) - 1
        ///    = ------------------------------------ + 2
        ///                       5
        /// </c></pre>
        /// </summary>
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
            for (int factor = 2; factor <= Math.Min(_denominator, _numerator); factor++)
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
