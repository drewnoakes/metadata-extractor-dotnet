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
using Com.Drew.Metadata;
using Com.Drew.Metadata.Iptc;
using Com.Drew.Tools;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Iptc
{
	/// <summary>
	/// Unit tests for
	/// <see cref="IptcReader"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class IptcReaderTest
	{
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static IptcDirectory ProcessBytes(string filePath)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			sbyte[] bytes = FileUtil.ReadBytes(filePath);
			new IptcReader().Extract(new SequentialByteArrayReader(bytes), metadata, bytes.Length);
			IptcDirectory directory = metadata.GetDirectory<IptcDirectory>();
			NUnit.Framework.Assert.IsNotNull(directory);
			return directory;
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestIptc1BytesFromFile()
		{
			IptcDirectory directory = ProcessBytes("Tests/Data/iptc1.jpg.appd");
			Sharpen.Tests.IsFalse(directory.GetErrors().ToString(), directory.HasErrors());
			Tag[] tags = Sharpen.Collections.ToArray(directory.GetTags(), new Tag[directory.GetTagCount()]);
			Sharpen.Tests.AreEqual(16, tags.Length);
			Sharpen.Tests.AreEqual(IptcDirectory.TagCategory, tags[0].GetTagType());
			NUnit.Framework.CollectionAssert.AreEqual(new string[] { "Supl. Category2", "Supl. Category1", "Cat" }, directory.GetStringArray(tags[0].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCopyrightNotice, tags[1].GetTagType());
			Sharpen.Tests.AreEqual("Copyright", directory.GetObject(tags[1].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagSpecialInstructions, tags[2].GetTagType());
			Sharpen.Tests.AreEqual("Special Instr.", directory.GetObject(tags[2].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagHeadline, tags[3].GetTagType());
			Sharpen.Tests.AreEqual("Headline", directory.GetObject(tags[3].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCaptionWriter, tags[4].GetTagType());
			Sharpen.Tests.AreEqual("CaptionWriter", directory.GetObject(tags[4].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCaption, tags[5].GetTagType());
			Sharpen.Tests.AreEqual("Caption", directory.GetObject(tags[5].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagOriginalTransmissionReference, tags[6].GetTagType());
			Sharpen.Tests.AreEqual("Transmission", directory.GetObject(tags[6].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCountryOrPrimaryLocationName, tags[7].GetTagType());
			Sharpen.Tests.AreEqual("Country", directory.GetObject(tags[7].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagProvinceOrState, tags[8].GetTagType());
			Sharpen.Tests.AreEqual("State", directory.GetObject(tags[8].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCity, tags[9].GetTagType());
			Sharpen.Tests.AreEqual("City", directory.GetObject(tags[9].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagDateCreated, tags[10].GetTagType());
			Sharpen.Tests.AreEqual(new Sharpen.GregorianCalendar(2000, 0, 1).GetTime(), directory.GetObject(tags[10].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagObjectName, tags[11].GetTagType());
			Sharpen.Tests.AreEqual("ObjectName", directory.GetObject(tags[11].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagSource, tags[12].GetTagType());
			Sharpen.Tests.AreEqual("Source", directory.GetObject(tags[12].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCredit, tags[13].GetTagType());
			Sharpen.Tests.AreEqual("Credits", directory.GetObject(tags[13].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagByLineTitle, tags[14].GetTagType());
			Sharpen.Tests.AreEqual("BylineTitle", directory.GetObject(tags[14].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagByLine, tags[15].GetTagType());
			Sharpen.Tests.AreEqual("Byline", directory.GetObject(tags[15].GetTagType()));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestIptc2Photoshop6BytesFromFile()
		{
			IptcDirectory directory = ProcessBytes("Tests/Data/iptc2-photoshop6.jpg.appd");
			Sharpen.Tests.IsFalse(directory.GetErrors().ToString(), directory.HasErrors());
			Tag[] tags = Sharpen.Collections.ToArray(directory.GetTags(), new Tag[directory.GetTagCount()]);
			Sharpen.Tests.AreEqual(17, tags.Length);
			Sharpen.Tests.AreEqual(IptcDirectory.TagApplicationRecordVersion, tags[0].GetTagType());
			Sharpen.Tests.AreEqual(2, directory.GetObject(tags[0].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCaption, tags[1].GetTagType());
			Sharpen.Tests.AreEqual("Caption PS6", directory.GetObject(tags[1].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCaptionWriter, tags[2].GetTagType());
			Sharpen.Tests.AreEqual("CaptionWriter", directory.GetObject(tags[2].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagHeadline, tags[3].GetTagType());
			Sharpen.Tests.AreEqual("Headline", directory.GetObject(tags[3].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagSpecialInstructions, tags[4].GetTagType());
			Sharpen.Tests.AreEqual("Special Instr.", directory.GetObject(tags[4].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagByLine, tags[5].GetTagType());
			Sharpen.Tests.AreEqual("Byline", directory.GetObject(tags[5].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagByLineTitle, tags[6].GetTagType());
			Sharpen.Tests.AreEqual("BylineTitle", directory.GetObject(tags[6].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCredit, tags[7].GetTagType());
			Sharpen.Tests.AreEqual("Credits", directory.GetObject(tags[7].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagSource, tags[8].GetTagType());
			Sharpen.Tests.AreEqual("Source", directory.GetObject(tags[8].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagObjectName, tags[9].GetTagType());
			Sharpen.Tests.AreEqual("ObjectName", directory.GetObject(tags[9].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCity, tags[10].GetTagType());
			Sharpen.Tests.AreEqual("City", directory.GetObject(tags[10].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagProvinceOrState, tags[11].GetTagType());
			Sharpen.Tests.AreEqual("State", directory.GetObject(tags[11].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCountryOrPrimaryLocationName, tags[12].GetTagType());
			Sharpen.Tests.AreEqual("Country", directory.GetObject(tags[12].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagOriginalTransmissionReference, tags[13].GetTagType());
			Sharpen.Tests.AreEqual("Transmission", directory.GetObject(tags[13].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCategory, tags[14].GetTagType());
			Sharpen.Tests.AreEqual("Cat", directory.GetObject(tags[14].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagSupplementalCategories, tags[15].GetTagType());
			NUnit.Framework.CollectionAssert.AreEqual(new string[] { "Supl. Category1", "Supl. Category2" }, directory.GetStringArray(tags[15].GetTagType()));
			Sharpen.Tests.AreEqual(IptcDirectory.TagCopyrightNotice, tags[16].GetTagType());
			Sharpen.Tests.AreEqual("Copyright", directory.GetObject(tags[16].GetTagType()));
		}
	}
}
