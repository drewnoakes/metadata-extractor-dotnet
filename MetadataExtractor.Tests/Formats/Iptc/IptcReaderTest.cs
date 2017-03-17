#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Iptc
{
    /// <summary>Unit tests for <see cref="IptcReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcReaderTest
    {
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        private static IptcDirectory ProcessBytes([NotNull] string filePath)
        {
            var bytes = TestDataUtil.GetBytes(filePath);
            var directory = new IptcReader().Extract(new SequentialByteArrayReader(bytes), bytes.Length);
            Assert.NotNull(directory);
            return directory;
        }

        [Fact]
        public void Iptc1BytesFromFile()
        {
            var directory = ProcessBytes("Data/iptc1.jpg.appd");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags.ToList();
            Assert.Equal(16, tags.Count);
            Assert.Equal(IptcDirectory.TagCategory, tags[0].Type);
            Assert.Equal(new[] { "Supl. Category2", "Supl. Category1", "Cat" }, directory.GetStringArray(tags[0].Type));
            Assert.Equal(IptcDirectory.TagCopyrightNotice, tags[1].Type);
            Assert.Equal("Copyright", directory.GetString(tags[1].Type));
            Assert.Equal(IptcDirectory.TagSpecialInstructions, tags[2].Type);
            Assert.Equal("Special Instr.", directory.GetString(tags[2].Type));
            Assert.Equal(IptcDirectory.TagHeadline, tags[3].Type);
            Assert.Equal("Headline", directory.GetString(tags[3].Type));
            Assert.Equal(IptcDirectory.TagCaptionWriter, tags[4].Type);
            Assert.Equal("CaptionWriter", directory.GetString(tags[4].Type));
            Assert.Equal(IptcDirectory.TagCaption, tags[5].Type);
            Assert.Equal("Caption", directory.GetString(tags[5].Type));
            Assert.Equal(IptcDirectory.TagOriginalTransmissionReference, tags[6].Type);
            Assert.Equal("Transmission", directory.GetString(tags[6].Type));
            Assert.Equal(IptcDirectory.TagCountryOrPrimaryLocationName, tags[7].Type);
            Assert.Equal("Country", directory.GetString(tags[7].Type));
            Assert.Equal(IptcDirectory.TagProvinceOrState, tags[8].Type);
            Assert.Equal("State", directory.GetString(tags[8].Type));
            Assert.Equal(IptcDirectory.TagCity, tags[9].Type);
            Assert.Equal("City", directory.GetString(tags[9].Type));
            Assert.Equal(IptcDirectory.TagDateCreated, tags[10].Type);
            Assert.Equal("20000101", directory.GetString(tags[10].Type));
            Assert.Equal(IptcDirectory.TagObjectName, tags[11].Type);
            Assert.Equal("ObjectName", directory.GetString(tags[11].Type));
            Assert.Equal(IptcDirectory.TagSource, tags[12].Type);
            Assert.Equal("Source", directory.GetString(tags[12].Type));
            Assert.Equal(IptcDirectory.TagCredit, tags[13].Type);
            Assert.Equal("Credits", directory.GetString(tags[13].Type));
            Assert.Equal(IptcDirectory.TagByLineTitle, tags[14].Type);
            Assert.Equal("BylineTitle", directory.GetString(tags[14].Type));
            Assert.Equal(IptcDirectory.TagByLine, tags[15].Type);
            Assert.Equal("Byline", directory.GetString(tags[15].Type));
        }

        [Fact]
        public void Iptc2Photoshop6BytesFromFile()
        {
            var directory = ProcessBytes("Data/iptc2-photoshop6.jpg.appd");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags.ToList();
            Assert.Equal(17, tags.Count);
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[0].Type);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].Type));
            Assert.Equal(IptcDirectory.TagCaption, tags[1].Type);
            Assert.Equal("Caption PS6", directory.GetString(tags[1].Type));
            Assert.Equal(IptcDirectory.TagCaptionWriter, tags[2].Type);
            Assert.Equal("CaptionWriter", directory.GetString(tags[2].Type));
            Assert.Equal(IptcDirectory.TagHeadline, tags[3].Type);
            Assert.Equal("Headline", directory.GetString(tags[3].Type));
            Assert.Equal(IptcDirectory.TagSpecialInstructions, tags[4].Type);
            Assert.Equal("Special Instr.", directory.GetString(tags[4].Type));
            Assert.Equal(IptcDirectory.TagByLine, tags[5].Type);
            Assert.Equal("Byline", directory.GetString(tags[5].Type));
            Assert.Equal(IptcDirectory.TagByLineTitle, tags[6].Type);
            Assert.Equal("BylineTitle", directory.GetString(tags[6].Type));
            Assert.Equal(IptcDirectory.TagCredit, tags[7].Type);
            Assert.Equal("Credits", directory.GetString(tags[7].Type));
            Assert.Equal(IptcDirectory.TagSource, tags[8].Type);
            Assert.Equal("Source", directory.GetString(tags[8].Type));
            Assert.Equal(IptcDirectory.TagObjectName, tags[9].Type);
            Assert.Equal("ObjectName", directory.GetString(tags[9].Type));
            Assert.Equal(IptcDirectory.TagCity, tags[10].Type);
            Assert.Equal("City", directory.GetString(tags[10].Type));
            Assert.Equal(IptcDirectory.TagProvinceOrState, tags[11].Type);
            Assert.Equal("State", directory.GetString(tags[11].Type));
            Assert.Equal(IptcDirectory.TagCountryOrPrimaryLocationName, tags[12].Type);
            Assert.Equal("Country", directory.GetString(tags[12].Type));
            Assert.Equal(IptcDirectory.TagOriginalTransmissionReference, tags[13].Type);
            Assert.Equal("Transmission", directory.GetString(tags[13].Type));
            Assert.Equal(IptcDirectory.TagCategory, tags[14].Type);
            Assert.Equal("Cat", directory.GetString(tags[14].Type));
            Assert.Equal(IptcDirectory.TagSupplementalCategories, tags[15].Type);
            Assert.Equal(new[] { "Supl. Category1", "Supl. Category2" }, directory.GetStringArray(tags[15].Type));
            Assert.Equal(IptcDirectory.TagCopyrightNotice, tags[16].Type);
            Assert.Equal("Copyright", directory.GetString(tags[16].Type));
        }

        [Fact]
        public void IptcEncodingUtf8()
        {
            var directory = ProcessBytes("Data/iptc-encoding-defined-utf8.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags.ToList();
            Assert.Equal(4, tags.Count);
            Assert.Equal(IptcDirectory.TagEnvelopeRecordVersion, tags[0].Type);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].Type));
            Assert.Equal(IptcDirectory.TagCodedCharacterSet, tags[1].Type);
            Assert.Equal("UTF-8", directory.GetString(tags[1].Type));
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[2].Type);
            Assert.Equal((ushort)2, directory.GetObject(tags[2].Type));
            Assert.Equal(IptcDirectory.TagCaption, tags[3].Type);
            Assert.Equal("In diesem Text sind Umlaute enthalten, nämlich öfter als üblich: ÄÖÜäöüß\r", directory.GetString(tags[3].Type));
        }

        [Fact]
        public void IptcEncodingUndefinedIso()
        {
            var directory = ProcessBytes("Data/iptc-encoding-undefined-iso.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags.ToList();
            Assert.Equal(3, tags.Count);
            Assert.Equal(IptcDirectory.TagEnvelopeRecordVersion, tags[0].Type);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].Type));
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[1].Type);
            Assert.Equal((ushort)2, directory.GetObject(tags[1].Type));
            Assert.Equal(IptcDirectory.TagCaption, tags[2].Type);
            Assert.Equal("In diesem Text sind Umlaute enthalten, nämlich öfter als üblich: ÄÖÜäöüß\r", directory.GetString(tags[2].Type));
        }

        [Fact]
        public void IptcEncodingUnknown()
        {
            var directory = ProcessBytes("Data/iptc-encoding-unknown.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags.ToList();
            Assert.Equal(3, tags.Count);
            Assert.Equal(IptcDirectory.TagApplicationRecordVersion, tags[0].Type);
            Assert.Equal((ushort)2, directory.GetObject(tags[0].Type));
            Assert.Equal(IptcDirectory.TagCaption, tags[1].Type);
            Assert.Equal("Das Encoding dieser Metadaten ist nicht deklariert und lässt sich nur schwer erkennen.", directory.GetString(tags[1].Type));
            Assert.Equal(IptcDirectory.TagKeywords, tags[2].Type);
            Assert.Equal(new[] { "häufig", "üblich", "Lösung", "Spaß" }, directory.GetStringArray(tags[2].Type));
        }

        [Fact]
        public void IptcEncodingUnknown2()
        {
            // This metadata has an encoding of three characters [ \ESC '%' '5' ]
            // It's not clear what to do with this, so it should be ignored.
            // Version 2.7.0 tripped up on this and threw an exception.
            var directory = ProcessBytes("Data/iptc-encoding-unknown-2.bytes");
            Assert.False(directory.HasError, directory.Errors.ToString());
            var tags = directory.Tags;
            Assert.Equal(37, tags.Count);
            Assert.Equal("MEDWAS,MEDLON,MEDTOR,RONL,ASIA,AONL,APC,USA,CAN,SAM,BIZ", directory.GetString(IptcDirectory.TagDestination));
        }
    }
}
