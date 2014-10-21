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
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using Com.Drew.Metadata.Adobe;
using Com.Drew.Tools;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Adobe
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class AdobeJpegReaderTest
	{
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static AdobeJpegDirectory ProcessBytes(string filePath)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new AdobeJpegReader().Extract(new SequentialByteArrayReader(FileUtil.ReadBytes(filePath)), metadata);
			AdobeJpegDirectory directory = metadata.GetDirectory<AdobeJpegDirectory>();
			NUnit.Framework.Assert.IsNotNull(directory);
			return directory;
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestSegmentTypes()
		{
			AdobeJpegReader reader = new AdobeJpegReader();
			Sharpen.Tests.AreEqual(1, Iterables.ToList(reader.GetSegmentTypes()).Count);
			Sharpen.Tests.AreEqual(JpegSegmentType.Appe, Iterables.ToList(reader.GetSegmentTypes())[0]);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestReadAdobeJpegMetadata1()
		{
			AdobeJpegDirectory directory = ProcessBytes("Tests/Data/adobeJpeg1.jpg.appe");
			Sharpen.Tests.IsFalse(directory.GetErrors().ToString(), directory.HasErrors());
			Sharpen.Tests.AreEqual(4, directory.GetTagCount());
			Sharpen.Tests.AreEqual(1, directory.GetInt(AdobeJpegDirectory.TagColorTransform));
			Sharpen.Tests.AreEqual(25600, directory.GetInt(AdobeJpegDirectory.TagDctEncodeVersion));
			Sharpen.Tests.AreEqual(128, directory.GetInt(AdobeJpegDirectory.TagApp14Flags0));
			Sharpen.Tests.AreEqual(0, directory.GetInt(AdobeJpegDirectory.TagApp14Flags1));
		}
	}
}
