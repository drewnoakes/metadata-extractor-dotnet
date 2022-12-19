// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
            Assert.Equal(31, age!.Years);
            Assert.Equal(7, age.Months);
            Assert.Equal(15, age.Days);
            Assert.Equal(0, age.Hours);
            Assert.Equal(0, age.Minutes);
            Assert.Equal(0, age.Seconds);
            Assert.Equal("0031:07:15 00:00:00", age.ToString());
            Assert.Equal("31 years 7 months 15 days", age.ToFriendlyString());
        }

        [Fact]
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
            Assert.False(age1!.Equals("Hello"));
            Assert.Equal(age1.GetHashCode(), age1.GetHashCode());
            Assert.Equal(age1.GetHashCode(), age2.GetHashCode());
            Assert.False(age1.GetHashCode() == age3.GetHashCode());
        }

        [Fact]
        public void ParseInvalid()
        {
            Assert.Throws<ArgumentNullException>(() => Age.FromPanasonicString(null!));

            Assert.Null(Age.FromPanasonicString(""));
            Assert.Null(Age.FromPanasonicString("9999:99:99 00:00:00"));
            Assert.Null(Age.FromPanasonicString("9999:99:99 00:00:00"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:00:0"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:00:"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:00"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:0"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 0"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 "));
            Assert.Null(Age.FromPanasonicString("0031:07:15"));
            Assert.Null(Age.FromPanasonicString("0031:07:1"));
            Assert.Null(Age.FromPanasonicString("0031:07:"));
            Assert.Null(Age.FromPanasonicString("0031:07"));
            Assert.Null(Age.FromPanasonicString("0031:0"));
            Assert.Null(Age.FromPanasonicString("0031:"));
            Assert.Null(Age.FromPanasonicString("0031"));
            Assert.Null(Age.FromPanasonicString("003"));
            Assert.Null(Age.FromPanasonicString("00"));
            Assert.Null(Age.FromPanasonicString("0"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:00:0a"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:00:a"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:0a:00"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 00:a:00"));
            Assert.Null(Age.FromPanasonicString("0031:07:15 0a:00:00"));
        }
    }
}
