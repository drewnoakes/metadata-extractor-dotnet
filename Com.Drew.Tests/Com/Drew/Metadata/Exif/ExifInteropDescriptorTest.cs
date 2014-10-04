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
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Unit tests for
	/// <see cref="ExifInteropDescriptor"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifInteropDescriptorTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInteropVersionDescription()
		{
			ExifInteropDirectory directory = new ExifInteropDirectory();
			directory.SetIntArray(ExifInteropDirectory.TagInteropVersion, new int[] { 0, 1, 0, 0 });
			ExifInteropDescriptor descriptor = new ExifInteropDescriptor(directory);
			Sharpen.Tests.AreEqual("1.00", descriptor.GetDescription(ExifInteropDirectory.TagInteropVersion));
			Sharpen.Tests.AreEqual("1.00", descriptor.GetInteropVersionDescription());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetInteropIndexDescription()
		{
			ExifInteropDirectory directory = new ExifInteropDirectory();
			directory.SetString(ExifInteropDirectory.TagInteropIndex, "R98");
			ExifInteropDescriptor descriptor = new ExifInteropDescriptor(directory);
			Sharpen.Tests.AreEqual("Recommended Exif Interoperability Rules (ExifR98)", descriptor.GetDescription(ExifInteropDirectory.TagInteropIndex));
			Sharpen.Tests.AreEqual("Recommended Exif Interoperability Rules (ExifR98)", descriptor.GetInteropIndexDescription());
		}
	}
}
