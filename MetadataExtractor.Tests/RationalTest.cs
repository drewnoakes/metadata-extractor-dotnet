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
using System.ComponentModel;
using NUnit.Framework;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for the <see cref="Rational"/> type.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class RationalTest
    {
        [Test]
        public void TestCreateRational()
        {
            var rational = new Rational(1, 3);

            Assert.AreEqual(1, rational.Numerator);
            Assert.AreEqual(3, rational.Denominator);
            Assert.AreEqual(1d / 3d, rational.ToDouble(), 0.0001);
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("1/3", new Rational(1, 3).ToString());
        }

        [Test, SetCulture("en-GB")]
        public void TestToSimpleString()
        {
            var third1 = new Rational(1, 3);
            var third2 = new Rational(2, 6);
            Assert.AreEqual("1/3", third1.ToSimpleString(allowDecimal: true));
            Assert.AreEqual("1/3", third2.ToSimpleString(allowDecimal: true));
            Assert.AreEqual(third1, third2);

            var twoThirds = new Rational(10, 15);
            Assert.AreEqual("2/3", twoThirds.ToSimpleString(allowDecimal: true));

            var two = new Rational(10, 5);
            Assert.IsTrue(two.IsInteger);
            Assert.AreEqual("2", two.ToSimpleString(allowDecimal: true));
            Assert.AreEqual("2", two.ToSimpleString(allowDecimal: false));

            var twoFifths = new Rational(4, 10);
            Assert.AreEqual("0.4", twoFifths.ToSimpleString(allowDecimal: true));
            Assert.AreEqual("2/5", twoFifths.ToSimpleString(allowDecimal: false));

            var threeEighths = new Rational(3, 8);
            Assert.AreEqual("3/8", threeEighths.ToSimpleString(allowDecimal: true));

            var zero = new Rational(0, 8);
            Assert.IsTrue(zero.IsInteger);
            Assert.AreEqual("0", zero.ToSimpleString(allowDecimal: true));
            Assert.AreEqual("0", zero.ToSimpleString(allowDecimal: false));

            zero = new Rational(0, 0);
            Assert.IsTrue(zero.IsInteger);
            Assert.AreEqual("0", zero.ToSimpleString(allowDecimal: true));
            Assert.AreEqual("0", zero.ToSimpleString(allowDecimal: false));
        }

        [Test]
        public void TestGetReciprocal()
        {
            var rational = new Rational(1, 3);
            var reciprocal = rational.Reciprocal;

            Assert.AreEqual(new Rational(3, 1), reciprocal, "new rational should be reciprocal");
            Assert.AreEqual(new Rational(1, 3), rational, "original should remain unchanged");
        }

        [Test]
        public void TestZeroOverZero()
        {
            Assert.AreEqual(new Rational(0, 0), new Rational(0, 0).Reciprocal);

            Assert.AreEqual(0.0d, new Rational(0, 0).ToDouble(), 0.000000001);
            Assert.AreEqual(0, new Rational(0, 0).ToByte());
            Assert.AreEqual(0.0f, new Rational(0, 0).ToSingle(), 0.000000001f);
            Assert.AreEqual(0, new Rational(0, 0).ToInt32());
            Assert.AreEqual(0L, (object)new Rational(0, 0).ToInt64());

            Assert.IsTrue(new Rational(0, 0).IsInteger);
        }

        [Test]
        public void TestTypeConverter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Rational));

            Assert.AreEqual(new Rational(1, 3), converter.ConvertFrom("1/3"));
            Assert.AreEqual(new Rational(1, 3), converter.ConvertFrom("1/3 "));

            Assert.AreEqual(new Rational(1, 3), converter.ConvertFrom(new Rational(1, 3)));

            Assert.AreEqual(new Rational(123, 1), converter.ConvertFrom(123));
            Assert.AreEqual(new Rational(123, 1), converter.ConvertFrom(123.0));
            Assert.AreEqual(new Rational(123, 1), converter.ConvertFrom(123L));
            Assert.AreEqual(new Rational(123, 1), converter.ConvertFrom(123u));

            Assert.AreEqual(new Rational(12, 34), converter.ConvertFrom(new[] { 12, 34 }));
            Assert.AreEqual(new Rational(12, 34), converter.ConvertFrom(new[] { 12u, 34u }));
            Assert.AreEqual(new Rational(13, 35), converter.ConvertFrom(new[] { 12.9, 34.9 })); // rounding

            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null));
        }

        [Test]
        public void TestIConvertible()
        {
            Assert.AreEqual(15,  Convert.ToByte (new Rational(150, 10)));
            Assert.AreEqual(15u, Convert.ToInt16(new Rational(150, 10)));
            Assert.AreEqual(15,  Convert.ToInt32(new Rational(150, 10)));
            Assert.AreEqual(15L, Convert.ToInt64(new Rational(150, 10)));

            Assert.AreEqual(15.5f, Convert.ToSingle(new Rational(155, 10)));
            Assert.AreEqual(15.5d, Convert.ToDouble(new Rational(155, 10)));
            Assert.AreEqual(15.5m, Convert.ToDecimal(new Rational(155, 10)));

            Assert.AreEqual("123/10", Convert.ToString(new Rational(123, 10)));

            Assert.Throws<NotSupportedException>(() => Convert.ToBoolean(new Rational(123, 10)));
            Assert.Throws<NotSupportedException>(() => Convert.ToChar(new Rational(123, 10)));
            Assert.Throws<NotSupportedException>(() => Convert.ToDateTime(new Rational(123, 10)));
        }
    }
}
