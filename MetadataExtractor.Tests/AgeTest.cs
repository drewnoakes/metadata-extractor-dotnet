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

using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for <see cref="Age"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AgeTest
    {
        [Fact]
        public void Parse()
        {
            var age = Age.FromPanasonicString("0031:07:15 00:00:00");
            Assert.NotNull(age);
            Assert.Equal(31, age.Years);
            Assert.Equal(7, age.Months);
            Assert.Equal(15, age.Days);
            Assert.Equal(0, age.Hours);
            Assert.Equal(0, age.Minutes);
            Assert.Equal(0, age.Seconds);
            Assert.Equal("0031:07:15 00:00:00", age.ToString());
            Assert.Equal("31 years 7 months 15 days", age.ToFriendlyString());
        }

        [Fact]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public void EqualsAndHashCode()
        {
            var age1 = new Age(10, 11, 12, 13, 14, 15);
            var age2 = new Age(10, 11, 12, 13, 14, 15);
            var age3 = new Age(0, 0, 0, 0, 0, 0);
            Assert.Equal(age1, age1);
            Assert.Equal(age1, age2);
            Assert.Equal(age2, age1);
            Assert.True(age1.Equals(age1));
            Assert.True(age1.Equals(age2));
            Assert.False(age1.Equals(age3));
            Assert.False(age1.Equals(null));
            Assert.False(age1.Equals("Hello"));
            Assert.Equal(age1.GetHashCode(), age1.GetHashCode());
            Assert.Equal(age1.GetHashCode(), age2.GetHashCode());
            Assert.False(age1.GetHashCode() == age3.GetHashCode());
        }
    }
}
