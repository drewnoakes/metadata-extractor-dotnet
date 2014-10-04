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
using Com.Drew.Metadata.Jpeg;
using Sharpen;

namespace Com.Drew.Metadata.Jpeg
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegDescriptorTest
	{
		private JpegDirectory _directory;

		private JpegDescriptor _descriptor;

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			_directory = new JpegDirectory();
			_descriptor = new JpegDescriptor(_directory);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetComponentDataDescription_InvalidComponentNumber()
		{
			NUnit.Framework.Assert.IsNull(_descriptor.GetComponentDataDescription(1));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetImageWidthDescription()
		{
			_directory.SetInt(JpegDirectory.TagImageWidth, 123);
			Sharpen.Tests.AreEqual("123 pixels", _descriptor.GetImageWidthDescription());
			Sharpen.Tests.AreEqual("123 pixels", _directory.GetDescription(JpegDirectory.TagImageWidth));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetImageHeightDescription()
		{
			_directory.SetInt(JpegDirectory.TagImageHeight, 123);
			Sharpen.Tests.AreEqual("123 pixels", _descriptor.GetImageHeightDescription());
			Sharpen.Tests.AreEqual("123 pixels", _directory.GetDescription(JpegDirectory.TagImageHeight));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetDataPrecisionDescription()
		{
			_directory.SetInt(JpegDirectory.TagDataPrecision, 8);
			Sharpen.Tests.AreEqual("8 bits", _descriptor.GetDataPrecisionDescription());
			Sharpen.Tests.AreEqual("8 bits", _directory.GetDescription(JpegDirectory.TagDataPrecision));
		}

		/// <exception cref="Com.Drew.Metadata.MetadataException"/>
		[NUnit.Framework.Test]
		public virtual void TestGetComponentDescription()
		{
			JpegComponent component1 = new JpegComponent(1, unchecked((int)(0x22)), 0);
			_directory.SetObject(JpegDirectory.TagComponentData1, component1);
			Sharpen.Tests.AreEqual("Y component: Quantization table 0, Sampling factors 2 horiz/2 vert", _directory.GetDescription(JpegDirectory.TagComponentData1));
			Sharpen.Tests.AreEqual("Y component: Quantization table 0, Sampling factors 2 horiz/2 vert", _descriptor.GetComponentDataDescription(0));
		}
	}
}
