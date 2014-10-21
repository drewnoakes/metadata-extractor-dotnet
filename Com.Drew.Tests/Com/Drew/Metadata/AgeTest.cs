/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
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
using Com.Drew.Metadata;
using Sharpen;

namespace Com.Drew.Metadata
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class AgeTest
	{
		[NUnit.Framework.Test]
		public virtual void TestParse()
		{
			Age age = Age.FromPanasonicString("0031:07:15 00:00:00");
			NUnit.Framework.Assert.IsNotNull(age);
			Sharpen.Tests.AreEqual(31, age.GetYears());
			Sharpen.Tests.AreEqual(7, age.GetMonths());
			Sharpen.Tests.AreEqual(15, age.GetDays());
			Sharpen.Tests.AreEqual(0, age.GetHours());
			Sharpen.Tests.AreEqual(0, age.GetMinutes());
			Sharpen.Tests.AreEqual(0, age.GetSeconds());
			Sharpen.Tests.AreEqual("0031:07:15 00:00:00", age.ToString());
			Sharpen.Tests.AreEqual("31 years 7 months 15 days", age.ToFriendlyString());
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsAndHashCode()
		{
			Age age1 = new Age(10, 11, 12, 13, 14, 15);
			Age age2 = new Age(10, 11, 12, 13, 14, 15);
			Age age3 = new Age(0, 0, 0, 0, 0, 0);
			Sharpen.Tests.AreEqual(age1, age1);
			Sharpen.Tests.AreEqual(age1, age2);
			Sharpen.Tests.AreEqual(age2, age1);
			Sharpen.Tests.IsTrue(age1.Equals(age1));
			Sharpen.Tests.IsTrue(age1.Equals(age2));
			Sharpen.Tests.IsFalse(age1.Equals(age3));
			Sharpen.Tests.IsFalse(age1.Equals(null));
			Sharpen.Tests.IsFalse(age1.Equals("Hello"));
			Sharpen.Tests.AreEqual(age1.GetHashCode(), age1.GetHashCode());
			Sharpen.Tests.AreEqual(age1.GetHashCode(), age2.GetHashCode());
			Sharpen.Tests.IsFalse(age1.GetHashCode() == age3.GetHashCode());
		}
	}
}
