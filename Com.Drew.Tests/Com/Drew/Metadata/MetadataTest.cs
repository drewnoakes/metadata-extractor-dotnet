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
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Iptc;
using Sharpen;

namespace Com.Drew.Metadata
{
	/// <summary>JUnit test case for class Metadata.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class MetadataTest
	{
		[NUnit.Framework.Test]
		public virtual void TestGetDirectoryWhenNotExists()
		{
			NUnit.Framework.Assert.IsNull(new Com.Drew.Metadata.Metadata().GetDirectory<ExifSubIFDDirectory>());
		}

		[NUnit.Framework.Test]
		public virtual void TestGetOrCreateDirectoryWhenNotExists()
		{
			NUnit.Framework.Assert.IsNotNull(new Com.Drew.Metadata.Metadata().GetOrCreateDirectory<ExifSubIFDDirectory>());
		}

		[NUnit.Framework.Test]
		public virtual void TestGetDirectoryReturnsSameInstance()
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			Com.Drew.Metadata.Directory directory = metadata.GetOrCreateDirectory<ExifSubIFDDirectory>();
			NUnit.Framework.Assert.AreSame(directory, metadata.GetDirectory<ExifSubIFDDirectory>());
		}

		[NUnit.Framework.Test]
		public virtual void TestGetOrCreateDirectoryReturnsSameInstance()
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			Com.Drew.Metadata.Directory directory = metadata.GetOrCreateDirectory<ExifSubIFDDirectory>();
			NUnit.Framework.Assert.AreSame(directory, metadata.GetOrCreateDirectory<ExifSubIFDDirectory>());
			NUnit.Framework.Assert.AreNotSame(directory, metadata.GetOrCreateDirectory<IptcDirectory>());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestHasErrors()
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			Sharpen.Tests.IsFalse(metadata.HasErrors());
			ExifSubIFDDirectory directory = metadata.GetOrCreateDirectory<ExifSubIFDDirectory>();
			directory.AddError("Test Error 1");
			Sharpen.Tests.IsTrue(metadata.HasErrors());
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetErrors()
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			Sharpen.Tests.IsFalse(metadata.HasErrors());
			ExifSubIFDDirectory directory = metadata.GetOrCreateDirectory<ExifSubIFDDirectory>();
			directory.AddError("Test Error 1");
			Sharpen.Tests.IsTrue(metadata.HasErrors());
		}
	}
}
