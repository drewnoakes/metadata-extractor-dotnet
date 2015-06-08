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

using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RationalTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestCreateRational()
        {
            Rational rational = new Rational(1, 3);
            Tests.AreEqual(1, rational.GetNumerator());
            Tests.AreEqual(3, rational.GetDenominator());
            Tests.AreEqual(1d / 3d, rational.DoubleValue(), 0.0001);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestToString()
        {
            Rational rational = new Rational(1, 3);
            Tests.AreEqual("1/3", Extensions.ConvertToString(rational));
        }

        /// <exception cref="System.Exception"/>
        [Test, SetCulture("en-GB")]
        public virtual void TestToSimpleString()
        {
            Rational third1 = new Rational(1, 3);
            Rational third2 = new Rational(2, 6);
            Tests.AreEqual("1/3", third1.ToSimpleString(true));
            Tests.AreEqual("1/3", third2.ToSimpleString(true));
            Tests.AreEqual(third1, third2);
            Rational twoThirds = new Rational(10, 15);
            Tests.AreEqual("2/3", twoThirds.ToSimpleString(true));
            Rational two = new Rational(10, 5);
            Tests.IsTrue(two.IsInteger());
            Tests.AreEqual("2", two.ToSimpleString(true));
            Tests.AreEqual("2", two.ToSimpleString(false));
            Rational twoFifths = new Rational(4, 10);
            Tests.AreEqual("0.4", twoFifths.ToSimpleString(true));
            Tests.AreEqual("2/5", twoFifths.ToSimpleString(false));
            Rational threeEighths = new Rational(3, 8);
            Tests.AreEqual("3/8", threeEighths.ToSimpleString(true));
            Rational zero = new Rational(0, 8);
            Tests.IsTrue(zero.IsInteger());
            Tests.AreEqual("0", zero.ToSimpleString(true));
            Tests.AreEqual("0", zero.ToSimpleString(false));
            zero = new Rational(0, 0);
            Tests.IsTrue(zero.IsInteger());
            Tests.AreEqual("0", zero.ToSimpleString(true));
            Tests.AreEqual("0", zero.ToSimpleString(false));
        }

        // not sure this is a nice presentation of rationals.  won't implement it for now.
        //        Rational twoAndAHalf = new Rational(10,4);
        //        assertEquals("2 1/2", twoAndAHalf.toSimpleString());
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetReciprocal()
        {
            Rational rational = new Rational(1, 3);
            Rational reciprocal = rational.GetReciprocal();
            Tests.AreEqual("new rational should be reciprocal", new Rational(3, 1), reciprocal);
            Tests.AreEqual("original reciprocal should remain unchanged", new Rational(1, 3), rational);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestZeroOverZero()
        {
            Tests.AreEqual(new Rational(0, 0), new Rational(0, 0).GetReciprocal());
            Tests.AreEqual(0.0d, new Rational(0, 0).DoubleValue(), 0.000000001);
            Tests.AreEqual(0, new Rational(0, 0).ByteValue());
            Tests.AreEqual(0.0f, new Rational(0, 0).FloatValue(), 0.000000001f);
            Tests.AreEqual(0, new Rational(0, 0).IntValue());
            Tests.AreEqual(0L, new Rational(0, 0).LongValue());
            Tests.IsTrue(new Rational(0, 0).IsInteger());
        }
    }
}
