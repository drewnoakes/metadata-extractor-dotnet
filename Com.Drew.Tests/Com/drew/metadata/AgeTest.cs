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

namespace Com.Drew.Metadata
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class AgeTest
    {
        [Test]
        public virtual void TestParse()
        {
            Age age = Age.FromPanasonicString("0031:07:15 00:00:00");
            Assert.IsNotNull(age);
            Tests.AreEqual(31, age.GetYears());
            Tests.AreEqual(7, age.GetMonths());
            Tests.AreEqual(15, age.GetDays());
            Tests.AreEqual(0, age.GetHours());
            Tests.AreEqual(0, age.GetMinutes());
            Tests.AreEqual(0, age.GetSeconds());
            Tests.AreEqual("0031:07:15 00:00:00", Extensions.ConvertToString(age));
            Tests.AreEqual("31 years 7 months 15 days", age.ToFriendlyString());
        }

        [Test]
        public virtual void TestEqualsAndHashCode()
        {
            Age age1 = new Age(10, 11, 12, 13, 14, 15);
            Age age2 = new Age(10, 11, 12, 13, 14, 15);
            Age age3 = new Age(0, 0, 0, 0, 0, 0);
            Tests.AreEqual(age1, age1);
            Tests.AreEqual(age1, age2);
            Tests.AreEqual(age2, age1);
            Tests.IsTrue(age1.Equals(age1));
            Tests.IsTrue(age1.Equals(age2));
            Tests.IsFalse(age1.Equals(age3));
            Tests.IsFalse(age1.Equals(null));
            Tests.IsFalse(age1.Equals("Hello"));
            Tests.AreEqual(age1.GetHashCode(), age1.GetHashCode());
            Tests.AreEqual(age1.GetHashCode(), age2.GetHashCode());
            Tests.IsFalse(age1.GetHashCode() == age3.GetHashCode());
        }
    }
}
