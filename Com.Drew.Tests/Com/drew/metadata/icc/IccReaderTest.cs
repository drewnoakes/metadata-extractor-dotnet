/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
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
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */
using Com.Drew.Lang;
using Com.Drew.Testing;
using Com.Drew.Tools;
using Sharpen;

namespace Com.Drew.Metadata.Icc
{
	public class IccReaderTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExtract()
		{
			sbyte[] app2Bytes = FileUtil.ReadBytes("Tests/Data/iccDataInvalid1.jpg.app2");
			// ICC data starts after a 14-byte preamble
			sbyte[] icc = TestHelper.SkipBytes(app2Bytes, 14);
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new IccReader().Extract(new ByteArrayReader(icc), metadata);
			IccDirectory directory = metadata.GetFirstDirectoryOfType<IccDirectory>();
			NUnit.Framework.Assert.IsNotNull(directory);
		}
		// TODO validate expected values
		//        for (Tag tag : directory.getTags()) {
		//            System.out.println(tag);
		//        }
	}
}
