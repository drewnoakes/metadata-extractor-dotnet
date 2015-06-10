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
using JetBrains.Annotations;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Iptc
{
    /// <summary>
    /// Unit tests for <see cref="IptcReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcReaderTest
    {
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static IptcDirectory ProcessBytes([NotNull] string filePath)
        {
            var metadata = new Metadata();
            var bytes = System.IO.File.ReadAllBytes(filePath);
            new IptcReader().Extract(new SequentialByteArrayReader(bytes), metadata, bytes.Length);
            var directory = metadata.GetFirstDirectoryOfType<IptcDirectory>();
            Assert.IsNotNull(directory);
            return directory;
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestIptc1BytesFromFile()
        {
            var directory = ProcessBytes("Tests/Data/iptc1.jpg.appd");
            Assert.IsFalse(directory.HasErrors(), directory.GetErrors().ToString());
            var tags = directory.GetTags();
            Assert.AreEqual(16, tags.Count);
            Assert.AreEqual(IptcDirectory.TagCategory, tags[0].TagType);
            CollectionAssert.AreEqual(new[] { "Supl. Category2", "Supl. Category1", "Cat" }, directory.GetStringArray(tags[0].TagType));
            Assert.AreEqual(IptcDirectory.TagCopyrightNotice, tags[1].TagType);
            Assert.AreEqual("Copyright", directory.GetObject(tags[1].TagType));
            Assert.AreEqual(IptcDirectory.TagSpecialInstructions, tags[2].TagType);
            Assert.AreEqual("Special Instr.", directory.GetObject(tags[2].TagType));
            Assert.AreEqual(IptcDirectory.TagHeadline, tags[3].TagType);
            Assert.AreEqual("Headline", directory.GetObject(tags[3].TagType));
            Assert.AreEqual(IptcDirectory.TagCaptionWriter, tags[4].TagType);
            Assert.AreEqual("CaptionWriter", directory.GetObject(tags[4].TagType));
            Assert.AreEqual(IptcDirectory.TagCaption, tags[5].TagType);
            Assert.AreEqual("Caption", directory.GetObject(tags[5].TagType));
            Assert.AreEqual(IptcDirectory.TagOriginalTransmissionReference, tags[6].TagType);
            Assert.AreEqual("Transmission", directory.GetObject(tags[6].TagType));
            Assert.AreEqual(IptcDirectory.TagCountryOrPrimaryLocationName, tags[7].TagType);
            Assert.AreEqual("Country", directory.GetObject(tags[7].TagType));
            Assert.AreEqual(IptcDirectory.TagProvinceOrState, tags[8].TagType);
            Assert.AreEqual("State", directory.GetObject(tags[8].TagType));
            Assert.AreEqual(IptcDirectory.TagCity, tags[9].TagType);
            Assert.AreEqual("City", directory.GetObject(tags[9].TagType));
            Assert.AreEqual(IptcDirectory.TagDateCreated, tags[10].TagType);
            Assert.AreEqual(new GregorianCalendar(2000, 0, 1).GetTime(), directory.GetObject(tags[10].TagType));
            Assert.AreEqual(IptcDirectory.TagObjectName, tags[11].TagType);
            Assert.AreEqual("ObjectName", directory.GetObject(tags[11].TagType));
            Assert.AreEqual(IptcDirectory.TagSource, tags[12].TagType);
            Assert.AreEqual("Source", directory.GetObject(tags[12].TagType));
            Assert.AreEqual(IptcDirectory.TagCredit, tags[13].TagType);
            Assert.AreEqual("Credits", directory.GetObject(tags[13].TagType));
            Assert.AreEqual(IptcDirectory.TagByLineTitle, tags[14].TagType);
            Assert.AreEqual("BylineTitle", directory.GetObject(tags[14].TagType));
            Assert.AreEqual(IptcDirectory.TagByLine, tags[15].TagType);
            Assert.AreEqual("Byline", directory.GetObject(tags[15].TagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestIptc2Photoshop6BytesFromFile()
        {
            var directory = ProcessBytes("Tests/Data/iptc2-photoshop6.jpg.appd");
            Assert.IsFalse(directory.HasErrors(), directory.GetErrors().ToString());
            var tags = directory.GetTags();
            Assert.AreEqual(17, tags.Count);
            Assert.AreEqual(IptcDirectory.TagApplicationRecordVersion, tags[0].TagType);
            Assert.AreEqual(2, directory.GetObject(tags[0].TagType));
            Assert.AreEqual(IptcDirectory.TagCaption, tags[1].TagType);
            Assert.AreEqual("Caption PS6", directory.GetObject(tags[1].TagType));
            Assert.AreEqual(IptcDirectory.TagCaptionWriter, tags[2].TagType);
            Assert.AreEqual("CaptionWriter", directory.GetObject(tags[2].TagType));
            Assert.AreEqual(IptcDirectory.TagHeadline, tags[3].TagType);
            Assert.AreEqual("Headline", directory.GetObject(tags[3].TagType));
            Assert.AreEqual(IptcDirectory.TagSpecialInstructions, tags[4].TagType);
            Assert.AreEqual("Special Instr.", directory.GetObject(tags[4].TagType));
            Assert.AreEqual(IptcDirectory.TagByLine, tags[5].TagType);
            Assert.AreEqual("Byline", directory.GetObject(tags[5].TagType));
            Assert.AreEqual(IptcDirectory.TagByLineTitle, tags[6].TagType);
            Assert.AreEqual("BylineTitle", directory.GetObject(tags[6].TagType));
            Assert.AreEqual(IptcDirectory.TagCredit, tags[7].TagType);
            Assert.AreEqual("Credits", directory.GetObject(tags[7].TagType));
            Assert.AreEqual(IptcDirectory.TagSource, tags[8].TagType);
            Assert.AreEqual("Source", directory.GetObject(tags[8].TagType));
            Assert.AreEqual(IptcDirectory.TagObjectName, tags[9].TagType);
            Assert.AreEqual("ObjectName", directory.GetObject(tags[9].TagType));
            Assert.AreEqual(IptcDirectory.TagCity, tags[10].TagType);
            Assert.AreEqual("City", directory.GetObject(tags[10].TagType));
            Assert.AreEqual(IptcDirectory.TagProvinceOrState, tags[11].TagType);
            Assert.AreEqual("State", directory.GetObject(tags[11].TagType));
            Assert.AreEqual(IptcDirectory.TagCountryOrPrimaryLocationName, tags[12].TagType);
            Assert.AreEqual("Country", directory.GetObject(tags[12].TagType));
            Assert.AreEqual(IptcDirectory.TagOriginalTransmissionReference, tags[13].TagType);
            Assert.AreEqual("Transmission", directory.GetObject(tags[13].TagType));
            Assert.AreEqual(IptcDirectory.TagCategory, tags[14].TagType);
            Assert.AreEqual("Cat", directory.GetObject(tags[14].TagType));
            Assert.AreEqual(IptcDirectory.TagSupplementalCategories, tags[15].TagType);
            CollectionAssert.AreEqual(new[] { "Supl. Category1", "Supl. Category2" }, directory.GetStringArray(tags[15].TagType));
            Assert.AreEqual(IptcDirectory.TagCopyrightNotice, tags[16].TagType);
            Assert.AreEqual("Copyright", directory.GetObject(tags[16].TagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestIptcEncodingUtf8()
        {
            var directory = ProcessBytes("Tests/Data/iptc-encoding-defined-utf8.bytes");
            Assert.IsFalse(directory.HasErrors(), directory.GetErrors().ToString());
            var tags = directory.GetTags();
            Assert.AreEqual(4, tags.Count);
            Assert.AreEqual(IptcDirectory.TagEnvelopeRecordVersion, tags[0].TagType);
            Assert.AreEqual(2, directory.GetObject(tags[0].TagType));
            Assert.AreEqual(IptcDirectory.TagCodedCharacterSet, tags[1].TagType);
            Assert.AreEqual("UTF-8", directory.GetObject(tags[1].TagType));
            Assert.AreEqual(IptcDirectory.TagApplicationRecordVersion, tags[2].TagType);
            Assert.AreEqual(2, directory.GetObject(tags[2].TagType));
            Assert.AreEqual(IptcDirectory.TagCaption, tags[3].TagType);
            Assert.AreEqual("In diesem Text sind Umlaute enthalten, nämlich öfter als üblich: ÄÖÜäöüß\r", directory.GetObject(tags[3].TagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestIptcEncodingUndefinedIso()
        {
            var directory = ProcessBytes("Tests/Data/iptc-encoding-undefined-iso.bytes");
            Assert.IsFalse(directory.HasErrors(), directory.GetErrors().ToString());
            var tags = directory.GetTags();
            Assert.AreEqual(3, tags.Count);
            Assert.AreEqual(IptcDirectory.TagEnvelopeRecordVersion, tags[0].TagType);
            Assert.AreEqual(2, directory.GetObject(tags[0].TagType));
            Assert.AreEqual(IptcDirectory.TagApplicationRecordVersion, tags[1].TagType);
            Assert.AreEqual(2, directory.GetObject(tags[1].TagType));
            Assert.AreEqual(IptcDirectory.TagCaption, tags[2].TagType);
            Assert.AreEqual("In diesem Text sind Umlaute enthalten, nämlich öfter als üblich: ÄÖÜäöüß\r", directory.GetObject(tags[2].TagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestIptcEncodingUnknown()
        {
            var directory = ProcessBytes("Tests/Data/iptc-encoding-unknown.bytes");
            Assert.IsFalse(directory.HasErrors(), directory.GetErrors().ToString());
            var tags = directory.GetTags();
            Assert.AreEqual(3, tags.Count);
            Assert.AreEqual(IptcDirectory.TagApplicationRecordVersion, tags[0].TagType);
            Assert.AreEqual(2, directory.GetObject(tags[0].TagType));
            Assert.AreEqual(IptcDirectory.TagCaption, tags[1].TagType);
            Assert.AreEqual("Das Encoding dieser Metadaten ist nicht deklariert und lässt sich nur schwer erkennen.", directory.GetObject(tags[1].TagType));
            Assert.AreEqual(IptcDirectory.TagKeywords, tags[2].TagType);
            CollectionAssert.AreEqual(new[] { "häufig", "üblich", "Lösung", "Spaß" }, directory.GetStringArray(tags[2].TagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestIptcEncodingUnknown2()
        {
            // This metadata has an encoding of three characters [ \ESC '%' '5' ]
            // It's not clear what to do with this, so it should be ignored.
            // Version 2.7.0 tripped up on this and threw an exception.
            var directory = ProcessBytes("Tests/Data/iptc-encoding-unknown-2.bytes");
            Assert.IsFalse(directory.HasErrors(), directory.GetErrors().ToString());
            var tags = directory.GetTags();
            Assert.AreEqual(37, tags.Count);
            Assert.AreEqual("MEDWAS,MEDLON,MEDTOR,RONL,ASIA,AONL,APC,USA,CAN,SAM,BIZ", directory.GetString(IptcDirectory.TagDestination));
        }
    }
}
