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

using System;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Iptc
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
            var bytes = File.ReadAllBytes(filePath);
            var directory = new IptcReader().Extract(new SequentialByteArrayReader(bytes), bytes.Length);
            Assert.NotNull(directory);
            return directory;
        }

        [Fact]
        public void TestIptc1BytesFromFile()
        {
            var directory = ProcessBytes("Tests/Data/iptc1.jpg.appd");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(16, tags.Count);
            Assert.Equal(IptcDirectory.TagCategory, tags[0].TagType);
            Assert.Equal(new[] { "Supl. Category2", "Supl. Category1", "Cat" }, directory.GetStringArray(tags[0].TagType));
            Assert.Equal(IptcDirectory.TagCopyrightNotice, tags[1].TagType);
            Assert.Equal("Copyright", directory.GetObject(tags[1].TagType));
            Assert.Equal(IptcDirectory.TagSpecialInstructions, tags[2].TagType);
            Assert.Equal("Special Instr.", directory.GetObject(tags[2].TagType));
            Assert.Equal(IptcDirectory.TagHeadline, tags[3].TagType);
            Assert.Equal("Headline", directory.GetObject(tags[3].TagType));
            Assert.Equal(IptcDirectory.TagCaptionWriter, tags[4].TagType);
            Assert.Equal("CaptionWriter", directory.GetObject(tags[4].TagType));
            Assert.Equal(IptcDirectory.TagCaption, tags[5].TagType);
            Assert.Equal("Caption", directory.GetObject(tags[5].TagType));
            Assert.Equal(IptcDirectory.TagOriginalTransmissionReference, tags[6].TagType);
            Assert.Equal("Transmission", directory.GetObject(tags[6].TagType));
            Assert.Equal(IptcDirectory.TagCountryOrPrimaryLocationName, tags[7].TagType);
            Assert.Equal("Country", directory.GetObject(tags[7].TagType));
            Assert.Equal(IptcDirectory.TagProvinceOrState, tags[8].TagType);
            Assert.Equal("State", directory.GetObject(tags[8].TagType));
            Assert.Equal(IptcDirectory.TagCity, tags[9].TagType);
            Assert.Equal("City", directory.GetObject(tags[9].TagType));
            Assert.Equal(IptcDirectory.TagDateCreated, tags[10].TagType);
            Assert.Equal(new DateTime(2000, 1, 1), directory.GetObject(tags[10].TagType));
            Assert.Equal(IptcDirectory.TagObjectName, tags[11].TagType);
            Assert.Equal("ObjectName", directory.GetObject(tags[11].TagType));
            Assert.Equal(IptcDirectory.TagSource, tags[12].TagType);
            Assert.Equal("Source", directory.GetObject(tags[12].TagType));
            Assert.Equal(IptcDirectory.TagCredit, tags[13].TagType);
            Assert.Equal("Credits", directory.GetObject(tags[13].TagType));
            Assert.Equal(IptcDirectory.TagByLineTitle, tags[14].TagType);
            Assert.Equal("BylineTitle", directory.GetObject(tags[14].TagType));
            Assert.Equal(IptcDirectory.TagByLine, tags[15].TagType);
            Assert.Equal("Byline", directory.GetObject(tags[15].TagType));
        }


        [Fact]
        public void TestIptc2Photoshop6BytesFromFile()
        {
            var directory = ProcessBytes("Tests/Data/iptc2-photoshop6.jpg.appd");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(17, tags.Count);
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[0].TagType);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].TagType));
            Assert.Equal(IptcDirectory.TagCaption, tags[1].TagType);
            Assert.Equal("Caption PS6", directory.GetObject(tags[1].TagType));
            Assert.Equal(IptcDirectory.TagCaptionWriter, tags[2].TagType);
            Assert.Equal("CaptionWriter", directory.GetObject(tags[2].TagType));
            Assert.Equal(IptcDirectory.TagHeadline, tags[3].TagType);
            Assert.Equal("Headline", directory.GetObject(tags[3].TagType));
            Assert.Equal(IptcDirectory.TagSpecialInstructions, tags[4].TagType);
            Assert.Equal("Special Instr.", directory.GetObject(tags[4].TagType));
            Assert.Equal(IptcDirectory.TagByLine, tags[5].TagType);
            Assert.Equal("Byline", directory.GetObject(tags[5].TagType));
            Assert.Equal(IptcDirectory.TagByLineTitle, tags[6].TagType);
            Assert.Equal("BylineTitle", directory.GetObject(tags[6].TagType));
            Assert.Equal(IptcDirectory.TagCredit, tags[7].TagType);
            Assert.Equal("Credits", directory.GetObject(tags[7].TagType));
            Assert.Equal(IptcDirectory.TagSource, tags[8].TagType);
            Assert.Equal("Source", directory.GetObject(tags[8].TagType));
            Assert.Equal(IptcDirectory.TagObjectName, tags[9].TagType);
            Assert.Equal("ObjectName", directory.GetObject(tags[9].TagType));
            Assert.Equal(IptcDirectory.TagCity, tags[10].TagType);
            Assert.Equal("City", directory.GetObject(tags[10].TagType));
            Assert.Equal(IptcDirectory.TagProvinceOrState, tags[11].TagType);
            Assert.Equal("State", directory.GetObject(tags[11].TagType));
            Assert.Equal(IptcDirectory.TagCountryOrPrimaryLocationName, tags[12].TagType);
            Assert.Equal("Country", directory.GetObject(tags[12].TagType));
            Assert.Equal(IptcDirectory.TagOriginalTransmissionReference, tags[13].TagType);
            Assert.Equal("Transmission", directory.GetObject(tags[13].TagType));
            Assert.Equal(IptcDirectory.TagCategory, tags[14].TagType);
            Assert.Equal("Cat", directory.GetObject(tags[14].TagType));
            Assert.Equal(IptcDirectory.TagSupplementalCategories, tags[15].TagType);
            Assert.Equal(new[] { "Supl. Category1", "Supl. Category2" }, directory.GetStringArray(tags[15].TagType));
            Assert.Equal(IptcDirectory.TagCopyrightNotice, tags[16].TagType);
            Assert.Equal("Copyright", directory.GetObject(tags[16].TagType));
        }


        [Fact]
        public void TestIptcEncodingUtf8()
        {
            var directory = ProcessBytes("Tests/Data/iptc-encoding-defined-utf8.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(4, tags.Count);
            Assert.Equal(IptcDirectory.TagEnvelopeRecordVersion, tags[0].TagType);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].TagType));
            Assert.Equal(IptcDirectory.TagCodedCharacterSet, tags[1].TagType);
            Assert.Equal("UTF-8", directory.GetObject(tags[1].TagType));
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[2].TagType);
            Assert.Equal((ushort)2, directory.GetObject(tags[2].TagType));
            Assert.Equal(IptcDirectory.TagCaption, tags[3].TagType);
            Assert.Equal("In diesem Text sind Umlaute enthalten, nämlich öfter als üblich: ÄÖÜäöüß\r", directory.GetObject(tags[3].TagType));
        }


        [Fact]
        public void TestIptcEncodingUndefinedIso()
        {
            var directory = ProcessBytes("Tests/Data/iptc-encoding-undefined-iso.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(3, tags.Count);
            Assert.Equal(IptcDirectory.TagEnvelopeRecordVersion, tags[0].TagType);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].TagType));
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[1].TagType);
            Assert.Equal((ushort)2, directory.GetObject(tags[1].TagType));
            Assert.Equal(IptcDirectory.TagCaption, tags[2].TagType);
            Assert.Equal("In diesem Text sind Umlaute enthalten, nämlich öfter als üblich: ÄÖÜäöüß\r", directory.GetObject(tags[2].TagType));
        }


        [Fact]
        public void TestIptcEncodingUnknown()
        {
            var directory = ProcessBytes("Tests/Data/iptc-encoding-unknown.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(3, tags.Count);
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[0].TagType);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].TagType));
            Assert.Equal(IptcDirectory.TagCaption, tags[1].TagType);
            Assert.Equal("Das Encoding dieser Metadaten ist nicht deklariert und lässt sich nur schwer erkennen.", directory.GetObject(tags[1].TagType));
            Assert.Equal(IptcDirectory.TagKeywords, tags[2].TagType);
            Assert.Equal(new[] { "häufig", "üblich", "Lösung", "Spaß" }, directory.GetStringArray(tags[2].TagType));
        }


        [Fact]
        public void TestIptcEncodingUnknown2()
        {
            // This metadata has an encoding of three characters [ \ESC '%' '5' ]
            // It's not clear what to do with this, so it should be ignored.
            // Version 2.7.0 tripped up on this and threw an exception.
            var directory = ProcessBytes("Tests/Data/iptc-encoding-unknown-2.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(37, tags.Count);
            Assert.Equal("MEDWAS,MEDLON,MEDTOR,RONL,ASIA,AONL,APC,USA,CAN,SAM,BIZ", directory.GetString(IptcDirectory.TagDestination));
        }
    }
}
