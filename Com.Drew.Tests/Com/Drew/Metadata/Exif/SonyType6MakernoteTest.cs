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
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Exif.Makernotes;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SonyType6MakernoteTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestSonyType6Makernote()
		{
			SonyType6MakernoteDirectory directory = ExifReaderTest.ProcessBytes<SonyType6MakernoteDirectory>("Tests/Data/sonyType6.jpg.app1.0");
			NUnit.Framework.Assert.IsNotNull(directory);
			Sharpen.Tests.IsFalse(directory.HasErrors());
			SonyType6MakernoteDescriptor descriptor = new SonyType6MakernoteDescriptor(directory);
			Sharpen.Tests.AreEqual("2.00", descriptor.GetMakernoteThumbVersionDescription());
		}
	}
}
