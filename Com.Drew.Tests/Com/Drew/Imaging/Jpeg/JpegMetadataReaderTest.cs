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
using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Metadata.Exif;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegMetadataReaderTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExtractMetadata()
		{
			Validate(JpegMetadataReader.ReadMetadata(new FilePath("Tests/Data/withExif.jpg")));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExtractMetadataUsingInputStream()
		{
			Validate(JpegMetadataReader.ReadMetadata(new FileInputStream((new FilePath("Tests/Data/withExif.jpg")))));
		}

		private void Validate(Com.Drew.Metadata.Metadata metadata)
		{
			Com.Drew.Metadata.Directory directory = metadata.GetDirectory<ExifSubIFDDirectory>();
			NUnit.Framework.Assert.IsNotNull(directory);
			Sharpen.Tests.AreEqual("80", directory.GetString(ExifSubIFDDirectory.TagIsoEquivalent));
		}
	}
}
