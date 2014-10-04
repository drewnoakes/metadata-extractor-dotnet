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
	/// <summary>JUnit test case for class ExifSubIFDDescriptor.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifSubIFDDescriptorTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUserCommentDescription_EmptyEncoding()
		{
			sbyte[] commentBytes = Sharpen.Runtime.GetBytesForString("\x0\x0\x0\x0\x0\x0\x0\x0This is a comment");
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual("This is a comment", descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUserCommentDescription_AsciiHeaderAsciiEncoding()
		{
			sbyte[] commentBytes = Sharpen.Runtime.GetBytesForString("ASCII\x0\x0This is a comment");
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual("This is a comment", descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUserCommentDescription_BlankAscii()
		{
			sbyte[] commentBytes = Sharpen.Runtime.GetBytesForString("ASCII\x0\x0\x0          ");
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual(string.Empty, descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUserCommentDescription_ZeroLengthAscii1()
		{
			// the 10-byte encoding region is only partially full
			sbyte[] commentBytes = Sharpen.Runtime.GetBytesForString("ASCII\x0\x0\x0");
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual("ASCII", descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUserCommentDescription_ZeroLengthAscii2()
		{
			// fill the 10-byte encoding region
			sbyte[] commentBytes = Sharpen.Runtime.GetBytesForString("ASCII\x0\x0\x0\x0\x0");
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual(string.Empty, descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUnicodeComment_ActualBytes()
		{
			sbyte[] commentBytes = new sbyte[] { 85, 78, 73, 67, 79, 68, 69, 0, 84, 0, 104, 0, 105, 0, 115, 0, 32, 0, 109, 0, 97, 0, 114, 0, 109, 0, 111, 0, 116, 0, 32, 0, 105, 0, 115, 0, 32, 0, 103, 0, 101, 0, 116, 0, 116, 0, 105, 0, 110, 0, 103, 0, 32
				, 0, 99, 0, 108, 0, 111, 0, 115, 0, 101, 0, 46, 0, 46, 0, 46, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 
				32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32
				, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 
				0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0 };
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual("This marmot is getting close...", descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestUnicodeComment_Ascii()
		{
			sbyte[] commentBytes = new sbyte[] { 65, 83, 67, 73, 73, 0, 0, 0, 73, 32, 97, 109, 32, 97, 32, 99, 111, 109, 109, 101, 110, 116, 46, 32, 89, 101, 121, 46, 0 };
			ExifSubIFDDirectory directory = new ExifSubIFDDirectory();
			directory.SetByteArray(ExifSubIFDDirectory.TagUserComment, commentBytes);
			ExifSubIFDDescriptor descriptor = new ExifSubIFDDescriptor(directory);
			Sharpen.Tests.AreEqual("I am a comment. Yey.", descriptor.GetDescription(ExifSubIFDDirectory.TagUserComment));
		}
	}
}
