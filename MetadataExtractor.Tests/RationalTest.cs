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

using MetadataExtractor;
using NUnit.Framework;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class RationalTest
    {

        [Test]
        public void TestCreateRational()
        {
            var rational = new Rational(1, 3);
            Assert.AreEqual(1, (object)rational.GetNumerator());
            Assert.AreEqual(3, (object)rational.GetDenominator());
            Assert.AreEqual(1d / 3d, rational.DoubleValue(), 0.0001);
        }


        [Test]
        public void TestToString()
        {
            var rational = new Rational(1, 3);
            Assert.AreEqual("1/3", rational.ToString());
        }


        [Test, SetCulture("en-GB")]
        public void TestToSimpleString()
        {
            var third1 = new Rational(1, 3);
            var third2 = new Rational(2, 6);
            Assert.AreEqual("1/3", third1.ToSimpleString(true));
            Assert.AreEqual("1/3", third2.ToSimpleString(true));
            Assert.AreEqual(third1, third2);
            var twoThirds = new Rational(10, 15);
            Assert.AreEqual("2/3", twoThirds.ToSimpleString(true));
            var two = new Rational(10, 5);
            Assert.IsTrue(two.IsInteger());
            Assert.AreEqual("2", two.ToSimpleString(true));
            Assert.AreEqual("2", two.ToSimpleString(false));
            var twoFifths = new Rational(4, 10);
            Assert.AreEqual("0.4", twoFifths.ToSimpleString(true));
            Assert.AreEqual("2/5", twoFifths.ToSimpleString(false));
            var threeEighths = new Rational(3, 8);
            Assert.AreEqual("3/8", threeEighths.ToSimpleString(true));
            var zero = new Rational(0, 8);
            Assert.IsTrue(zero.IsInteger());
            Assert.AreEqual("0", zero.ToSimpleString(true));
            Assert.AreEqual("0", zero.ToSimpleString(false));
            zero = new Rational(0, 0);
            Assert.IsTrue(zero.IsInteger());
            Assert.AreEqual("0", zero.ToSimpleString(true));
            Assert.AreEqual("0", zero.ToSimpleString(false));
        }

        // not sure this is a nice presentation of rationals.  won't implement it for now.
        //        Rational twoAndAHalf = new Rational(10,4);
        //        assertEquals("2 1/2", twoAndAHalf.toSimpleString());

        [Test]
        public void TestGetReciprocal()
        {
            var rational = new Rational(1, 3);
            var reciprocal = rational.GetReciprocal();
            Assert.AreEqual(new Rational(3, 1), reciprocal, "new rational should be reciprocal");
            Assert.AreEqual(new Rational(1, 3), rational, "original reciprocal should remain unchanged");
        }


        [Test]
        public void TestZeroOverZero()
        {
            Assert.AreEqual(new Rational(0, 0), new Rational(0, 0).GetReciprocal());
            Assert.AreEqual(0.0d, new Rational(0, 0).DoubleValue(), 0.000000001);
            Assert.AreEqual(0, new Rational(0, 0).ByteValue());
            Assert.AreEqual(0.0f, new Rational(0, 0).FloatValue(), 0.000000001f);
            Assert.AreEqual(0, new Rational(0, 0).IntValue());
            Assert.AreEqual(0L, (object)new Rational(0, 0).LongValue());
            Assert.IsTrue(new Rational(0, 0).IsInteger());
        }
    }
}
