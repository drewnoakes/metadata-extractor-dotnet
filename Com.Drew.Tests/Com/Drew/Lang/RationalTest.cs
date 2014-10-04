/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using Com.Drew.Lang;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class RationalTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestCreateRational()
		{
			Rational rational = new Rational(1, 3);
			Sharpen.Tests.AreEqual(1, rational.GetNumerator());
			Sharpen.Tests.AreEqual(3, rational.GetDenominator());
			Sharpen.Tests.AreEqual(1d / 3d, rational.DoubleValue(), 0.0001);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestToString()
		{
			Rational rational = new Rational(1, 3);
			Sharpen.Tests.AreEqual("1/3", rational.ToString());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestToSimpleString()
		{
			Rational third1 = new Rational(1, 3);
			Rational third2 = new Rational(2, 6);
			Sharpen.Tests.AreEqual("1/3", third1.ToSimpleString(true));
			Sharpen.Tests.AreEqual("1/3", third2.ToSimpleString(true));
			Sharpen.Tests.AreEqual(third1, third2);
			Rational twoThirds = new Rational(10, 15);
			Sharpen.Tests.AreEqual("2/3", twoThirds.ToSimpleString(true));
			Rational two = new Rational(10, 5);
			Sharpen.Tests.IsTrue(two.IsInteger());
			Sharpen.Tests.AreEqual("2", two.ToSimpleString(true));
			Sharpen.Tests.AreEqual("2", two.ToSimpleString(false));
			Rational twoFifths = new Rational(4, 10);
			Sharpen.Tests.AreEqual("0.4", twoFifths.ToSimpleString(true));
			Sharpen.Tests.AreEqual("2/5", twoFifths.ToSimpleString(false));
			Rational threeEigths = new Rational(3, 8);
			Sharpen.Tests.AreEqual("3/8", threeEigths.ToSimpleString(true));
			Rational zero = new Rational(0, 8);
			Sharpen.Tests.IsTrue(zero.IsInteger());
			Sharpen.Tests.AreEqual("0", zero.ToSimpleString(true));
			Sharpen.Tests.AreEqual("0", zero.ToSimpleString(false));
			zero = new Rational(0, 0);
			Sharpen.Tests.IsTrue(zero.IsInteger());
			Sharpen.Tests.AreEqual("0", zero.ToSimpleString(true));
			Sharpen.Tests.AreEqual("0", zero.ToSimpleString(false));
		}

		// not sure this is a nice presentation of rationals.  won't implement it for now.
		//        Rational twoAndAHalf = new Rational(10,4);
		//        assertEquals("2 1/2", twoAndAHalf.toSimpleString());
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetReciprocal()
		{
			Rational rational = new Rational(1, 3);
			Rational reciprocal = rational.GetReciprocal();
			Sharpen.Tests.AreEqual("new rational should be reciprocal", new Rational(3, 1), reciprocal);
			Sharpen.Tests.AreEqual("original reciprocal should remain unchanged", new Rational(1, 3), rational);
		}
	}
}
