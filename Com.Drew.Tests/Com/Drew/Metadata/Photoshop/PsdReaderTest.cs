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
using Com.Drew.Lang;
using Com.Drew.Metadata.Photoshop;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PsdReaderTest
	{
		/// <exception cref="System.Exception"/>
		[NotNull]
		public static PsdHeaderDirectory ProcessBytes(string file)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			RandomAccessFile randomAccessFile = new RandomAccessFile(new FilePath(file), "r");
			new PsdReader().Extract(new RandomAccessFileReader(randomAccessFile), metadata);
			randomAccessFile.Close();
			PsdHeaderDirectory directory = metadata.GetDirectory<PsdHeaderDirectory>();
			NUnit.Framework.Assert.IsNotNull(directory);
			return directory;
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void Test8x8x8bitGrayscale()
		{
			PsdHeaderDirectory directory = ProcessBytes("Tests/Data/8x4x8bit-Grayscale.psd");
			Sharpen.Tests.AreEqual(8, directory.GetInt(PsdHeaderDirectory.TagImageWidth));
			Sharpen.Tests.AreEqual(4, directory.GetInt(PsdHeaderDirectory.TagImageHeight));
			Sharpen.Tests.AreEqual(8, directory.GetInt(PsdHeaderDirectory.TagBitsPerChannel));
			Sharpen.Tests.AreEqual(1, directory.GetInt(PsdHeaderDirectory.TagChannelCount));
			Sharpen.Tests.AreEqual(1, directory.GetInt(PsdHeaderDirectory.TagColorMode));
		}

		// 1 = grayscale
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void Test10x12x16bitCMYK()
		{
			PsdHeaderDirectory directory = ProcessBytes("Tests/Data/10x12x16bit-CMYK.psd");
			Sharpen.Tests.AreEqual(10, directory.GetInt(PsdHeaderDirectory.TagImageWidth));
			Sharpen.Tests.AreEqual(12, directory.GetInt(PsdHeaderDirectory.TagImageHeight));
			Sharpen.Tests.AreEqual(16, directory.GetInt(PsdHeaderDirectory.TagBitsPerChannel));
			Sharpen.Tests.AreEqual(4, directory.GetInt(PsdHeaderDirectory.TagChannelCount));
			Sharpen.Tests.AreEqual(4, directory.GetInt(PsdHeaderDirectory.TagColorMode));
		}
		// 4 = CMYK
	}
}
