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
using Com.Drew.Metadata.Exif;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Unit tests for
	/// <see cref="ExifIFD0Descriptor"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifIFD0DescriptorTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestXResolutionDescription()
		{
			ExifIFD0Directory directory = new ExifIFD0Directory();
			directory.SetRational(ExifIFD0Directory.TagXResolution, new Rational(72, 1));
			// 2 is for 'Inch'
			directory.SetInt(ExifIFD0Directory.TagResolutionUnit, 2);
			ExifIFD0Descriptor descriptor = new ExifIFD0Descriptor(directory);
			Sharpen.Tests.AreEqual("72 dots per inch", descriptor.GetDescription(ExifIFD0Directory.TagXResolution));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestYResolutionDescription()
		{
			ExifIFD0Directory directory = new ExifIFD0Directory();
			directory.SetRational(ExifIFD0Directory.TagYResolution, new Rational(50, 1));
			// 3 is for 'cm'
			directory.SetInt(ExifIFD0Directory.TagResolutionUnit, 3);
			ExifIFD0Descriptor descriptor = new ExifIFD0Descriptor(directory);
			Sharpen.Tests.AreEqual("50 dots per cm", descriptor.GetDescription(ExifIFD0Directory.TagYResolution));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestWindowsXpFields()
		{
			ExifIFD0Directory directory = ExifReaderTest.ProcessBytes<ExifIFD0Directory>("Tests/Data/windowsXpFields.jpg.app1");
			Sharpen.Tests.AreEqual("Testing artist\x0", directory.GetString(ExifIFD0Directory.TagWinAuthor, "UTF-16LE"));
			Sharpen.Tests.AreEqual("Testing comments\x0", directory.GetString(ExifIFD0Directory.TagWinComment, "UTF-16LE"));
			Sharpen.Tests.AreEqual("Testing keywords\x0", directory.GetString(ExifIFD0Directory.TagWinKeywords, "UTF-16LE"));
			Sharpen.Tests.AreEqual("Testing subject\x0", directory.GetString(ExifIFD0Directory.TagWinSubject, "UTF-16LE"));
			Sharpen.Tests.AreEqual("Testing title\x0", directory.GetString(ExifIFD0Directory.TagWinTitle, "UTF-16LE"));
			ExifIFD0Descriptor descriptor = new ExifIFD0Descriptor(directory);
			Sharpen.Tests.AreEqual("Testing artist", descriptor.GetDescription(ExifIFD0Directory.TagWinAuthor));
			Sharpen.Tests.AreEqual("Testing comments", descriptor.GetDescription(ExifIFD0Directory.TagWinComment));
			Sharpen.Tests.AreEqual("Testing keywords", descriptor.GetDescription(ExifIFD0Directory.TagWinKeywords));
			Sharpen.Tests.AreEqual("Testing subject", descriptor.GetDescription(ExifIFD0Directory.TagWinSubject));
			Sharpen.Tests.AreEqual("Testing title", descriptor.GetDescription(ExifIFD0Directory.TagWinTitle));
		}
	}
}
