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

namespace Com.Drew.Metadata
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AgeTest
    {
        [Test]
        public void TestParse()
        {
            var age = Age.FromPanasonicString("0031:07:15 00:00:00");
            Assert.IsNotNull(age);
            Assert.AreEqual(31, age.GetYears());
            Assert.AreEqual(7, age.GetMonths());
            Assert.AreEqual(15, age.GetDays());
            Assert.AreEqual(0, age.GetHours());
            Assert.AreEqual(0, age.GetMinutes());
            Assert.AreEqual(0, age.GetSeconds());
            Assert.AreEqual("0031:07:15 00:00:00", age.ToString());
            Assert.AreEqual("31 years 7 months 15 days", age.ToFriendlyString());
        }

        [Test]
        public void TestEqualsAndHashCode()
        {
            var age1 = new Age(10, 11, 12, 13, 14, 15);
            var age2 = new Age(10, 11, 12, 13, 14, 15);
            var age3 = new Age(0, 0, 0, 0, 0, 0);
            Assert.AreEqual(age1, age1);
            Assert.AreEqual(age1, age2);
            Assert.AreEqual(age2, age1);
            Assert.IsTrue(age1.Equals(age1));
            Assert.IsTrue(age1.Equals(age2));
            Assert.IsFalse(age1.Equals(age3));
            Assert.IsFalse(age1.Equals(null));
            Assert.IsFalse(age1.Equals("Hello"));
            Assert.AreEqual(age1.GetHashCode(), age1.GetHashCode());
            Assert.AreEqual(age1.GetHashCode(), age2.GetHashCode());
            Assert.IsFalse(age1.GetHashCode() == age3.GetHashCode());
        }
    }
}
