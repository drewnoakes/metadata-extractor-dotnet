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
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Jfif;
using Sharpen;

namespace Com.Drew.Metadata.Jfif
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JfifReaderTest
	{
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestRead()
		{
			sbyte[] jfifData = new sbyte[] { 74, 70, 73, 70, 0, 1, 2, 1, 0, 108, 0, 108, 0, 0 };
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			JfifReader reader = new JfifReader();
			reader.Extract(new ByteArrayReader(jfifData), metadata);
			Sharpen.Tests.AreEqual(1, metadata.GetDirectoryCount());
			JfifDirectory directory = metadata.GetDirectory<JfifDirectory>();
			NUnit.Framework.Assert.IsNotNull(directory);
			Sharpen.Tests.IsFalse(directory.GetErrors().ToString(), directory.HasErrors());
			Tag[] tags = Sharpen.Collections.ToArray(directory.GetTags(), new Tag[directory.GetTagCount()]);
			Sharpen.Tests.AreEqual(4, tags.Length);
			Sharpen.Tests.AreEqual(JfifDirectory.TagVersion, tags[0].GetTagType());
			Sharpen.Tests.AreEqual(unchecked((int)(0x0102)), directory.GetInt(tags[0].GetTagType()));
			Sharpen.Tests.AreEqual(JfifDirectory.TagUnits, tags[1].GetTagType());
			Sharpen.Tests.AreEqual(1, directory.GetInt(tags[1].GetTagType()));
			Sharpen.Tests.AreEqual(JfifDirectory.TagResx, tags[2].GetTagType());
			Sharpen.Tests.AreEqual(108, directory.GetInt(tags[2].GetTagType()));
			Sharpen.Tests.AreEqual(JfifDirectory.TagResy, tags[3].GetTagType());
			Sharpen.Tests.AreEqual(108, directory.GetInt(tags[3].GetTagType()));
		}
	}
}
