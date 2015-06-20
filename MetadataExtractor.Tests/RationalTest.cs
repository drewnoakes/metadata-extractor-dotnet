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
using Xunit;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for the <see cref="Rational"/> type.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class RationalTest
    {
        [Fact]
        public void TestCreateRational()
        {
            var rational = new Rational(1, 3);

            Assert.Equal(1, rational.Numerator);
            Assert.Equal(3, rational.Denominator);
            Assert.Equal(1d / 3d, rational.ToDouble(), 4);
        }

        [Fact]
        public void TestToString()
        {
            Assert.Equal("1/3", new Rational(1, 3).ToString());
        }

        [Fact, UseCulture("en-GB")]
        public void TestToSimpleString()
        {
            var third1 = new Rational(1, 3);
            var third2 = new Rational(2, 6);
            Assert.Equal("1/3", third1.ToSimpleString(allowDecimal: true));
            Assert.Equal("1/3", third2.ToSimpleString(allowDecimal: true));
            Assert.Equal(third1, third2);

            var twoThirds = new Rational(10, 15);
            Assert.Equal("2/3", twoThirds.ToSimpleString(allowDecimal: true));

            var two = new Rational(10, 5);
            Assert.True(two.IsInteger);
            Assert.Equal("2", two.ToSimpleString(allowDecimal: true));
            Assert.Equal("2", two.ToSimpleString(allowDecimal: false));

            var twoFifths = new Rational(4, 10);
            Assert.Equal("0.4", twoFifths.ToSimpleString(allowDecimal: true));
            Assert.Equal("2/5", twoFifths.ToSimpleString(allowDecimal: false));

            var threeEighths = new Rational(3, 8);
            Assert.Equal("3/8", threeEighths.ToSimpleString(allowDecimal: true));

            var zero = new Rational(0, 8);
            Assert.True(zero.IsInteger);
            Assert.Equal("0", zero.ToSimpleString(allowDecimal: true));
            Assert.Equal("0", zero.ToSimpleString(allowDecimal: false));

            zero = new Rational(0, 0);
            Assert.True(zero.IsInteger);
            Assert.Equal("0", zero.ToSimpleString(allowDecimal: true));
            Assert.Equal("0", zero.ToSimpleString(allowDecimal: false));
        }

        [Fact]
        public void TestGetReciprocal()
        {
            var rational = new Rational(1, 3);
            var reciprocal = rational.Reciprocal;
            Assert.Equal(new Rational(3, 1), reciprocal);
            Assert.Equal(new Rational(1, 3), rational);
        }

        [Fact]
        public void TestZeroOverZero()
        {
            Assert.Equal(new Rational(0, 0), new Rational(0, 0).Reciprocal);

            Assert.Equal(0.0d, new Rational(0, 0).ToDouble(), 15);
            Assert.Equal(0, new Rational(0, 0).ToByte());
            Assert.Equal(0.0f, new Rational(0, 0).ToSingle(), 15);
            Assert.Equal(0, new Rational(0, 0).ToInt32());
            Assert.Equal(0L, (object)new Rational(0, 0).ToInt64());

            Assert.True(new Rational(0, 0).IsInteger);
        }

        [Fact]
        public void TestTypeConverter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Rational));

            Assert.Equal(new Rational(1, 3), converter.ConvertFrom("1/3"));
            Assert.Equal(new Rational(1, 3), converter.ConvertFrom("1/3 "));

            Assert.Equal(new Rational(1, 3), converter.ConvertFrom(new Rational(1, 3)));

            Assert.Equal(new Rational(123, 1), converter.ConvertFrom(123));
            Assert.Equal(new Rational(123, 1), converter.ConvertFrom(123.0));
            Assert.Equal(new Rational(123, 1), converter.ConvertFrom(123L));
            Assert.Equal(new Rational(123, 1), converter.ConvertFrom(123u));

            Assert.Equal(new Rational(12, 34), converter.ConvertFrom(new[] { 12, 34 }));
            Assert.Equal(new Rational(12, 34), converter.ConvertFrom(new[] { 12u, 34u }));
            Assert.Equal(new Rational(13, 35), converter.ConvertFrom(new[] { 12.9, 34.9 })); // rounding

            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null));
        }

        [Fact]
        public void TestIConvertible()
        {
            Assert.Equal(15,  Convert.ToByte (new Rational(150, 10)));
            Assert.Equal(15,  Convert.ToInt16(new Rational(150, 10)));
            Assert.Equal(15,  Convert.ToInt32(new Rational(150, 10)));
            Assert.Equal(15L, Convert.ToInt64(new Rational(150, 10)));

            Assert.Equal(15,   Convert.ToSByte (new Rational(150, 10)));
            Assert.Equal(15,   Convert.ToUInt16(new Rational(150, 10)));
            Assert.Equal(15u,  Convert.ToUInt32(new Rational(150, 10)));
            Assert.Equal(15Lu, Convert.ToUInt64(new Rational(150, 10)));

            Assert.Equal(15.5f, Convert.ToSingle(new Rational(155, 10)));
            Assert.Equal(15.5d, Convert.ToDouble(new Rational(155, 10)));
            Assert.Equal(15.5m, Convert.ToDecimal(new Rational(155, 10)));

            Assert.Equal("123/10", Convert.ToString(new Rational(123, 10)));

            Assert.True(Convert.ToBoolean(new Rational(123, 10)));
            Assert.False(Convert.ToBoolean(new Rational(1, 0)));
            Assert.False(Convert.ToBoolean(new Rational(0, 1)));
            Assert.False(Convert.ToBoolean(new Rational(0, 0)));

            Assert.Throws<NotSupportedException>(() => Convert.ToChar(new Rational(123, 10)));
            Assert.Throws<NotSupportedException>(() => Convert.ToDateTime(new Rational(123, 10)));
        }
    }
}
