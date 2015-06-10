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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Iptc
{
    /// <summary>Describes tags used by the International Press Telecommunications Council (IPTC) metadata format.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcDirectory : Directory
    {
        public const int TagEnvelopeRecordVersion = unchecked(0x0100);

        public const int TagDestination = unchecked(0x0105);

        public const int TagFileFormat = unchecked(0x0114);

        public const int TagFileVersion = unchecked(0x0116);

        public const int TagServiceId = unchecked(0x011E);

        public const int TagEnvelopeNumber = unchecked(0x0128);

        public const int TagProductId = unchecked(0x0132);

        public const int TagEnvelopePriority = unchecked(0x013C);

        public const int TagDateSent = unchecked(0x0146);

        public const int TagTimeSent = unchecked(0x0150);

        public const int TagCodedCharacterSet = unchecked(0x015A);

        public const int TagUniqueObjectName = unchecked(0x0164);

        public const int TagArmIdentifier = unchecked(0x0178);

        public const int TagArmVersion = unchecked(0x017a);

        public const int TagApplicationRecordVersion = unchecked(0x0200);

        public const int TagObjectTypeReference = unchecked(0x0203);

        public const int TagObjectAttributeReference = unchecked(0x0204);

        public const int TagObjectName = unchecked(0x0205);

        public const int TagEditStatus = unchecked(0x0207);

        public const int TagEditorialUpdate = unchecked(0x0208);

        public const int TagUrgency = 0X020A;

        public const int TagSubjectReference = 0X020C;

        public const int TagCategory = unchecked(0x020F);

        public const int TagSupplementalCategories = unchecked(0x0214);

        public const int TagFixtureId = unchecked(0x0216);

        public const int TagKeywords = unchecked(0x0219);

        public const int TagContentLocationCode = unchecked(0x021A);

        public const int TagContentLocationName = unchecked(0x021B);

        public const int TagReleaseDate = 0X021E;

        public const int TagReleaseTime = unchecked(0x0223);

        public const int TagExpirationDate = unchecked(0x0225);

        public const int TagExpirationTime = unchecked(0x0226);

        public const int TagSpecialInstructions = unchecked(0x0228);

        public const int TagActionAdvised = unchecked(0x022A);

        public const int TagReferenceService = unchecked(0x022D);

        public const int TagReferenceDate = unchecked(0x022F);

        public const int TagReferenceNumber = unchecked(0x0232);

        public const int TagDateCreated = unchecked(0x0237);

        public const int TagTimeCreated = 0X023C;

        public const int TagDigitalDateCreated = unchecked(0x023E);

        public const int TagDigitalTimeCreated = unchecked(0x023F);

        public const int TagOriginatingProgram = unchecked(0x0241);

        public const int TagProgramVersion = unchecked(0x0246);

        public const int TagObjectCycle = unchecked(0x024B);

        public const int TagByLine = unchecked(0x0250);

        public const int TagByLineTitle = unchecked(0x0255);

        public const int TagCity = unchecked(0x025A);

        public const int TagSubLocation = unchecked(0x025C);

        public const int TagProvinceOrState = unchecked(0x025F);

        public const int TagCountryOrPrimaryLocationCode = unchecked(0x0264);

        public const int TagCountryOrPrimaryLocationName = unchecked(0x0265);

        public const int TagOriginalTransmissionReference = unchecked(0x0267);

        public const int TagHeadline = unchecked(0x0269);

        public const int TagCredit = unchecked(0x026E);

        public const int TagSource = unchecked(0x0273);

        public const int TagCopyrightNotice = unchecked(0x0274);

        public const int TagContact = unchecked(0x0276);

        public const int TagCaption = unchecked(0x0278);

        public const int TagLocalCaption = unchecked(0x0279);

        public const int TagCaptionWriter = unchecked(0x027A);

        public const int TagRasterizedCaption = unchecked(0x027D);

        public const int TagImageType = unchecked(0x0282);

        public const int TagImageOrientation = unchecked(0x0283);

        public const int TagLanguageIdentifier = unchecked(0x0287);

        public const int TagAudioType = unchecked(0x0296);

        public const int TagAudioSamplingRate = unchecked(0x0297);

        public const int TagAudioSamplingResolution = unchecked(0x0298);

        public const int TagAudioDuration = unchecked(0x0299);

        public const int TagAudioOutcue = unchecked(0x029A);

        public const int TagJobId = unchecked(0x02B8);

        public const int TagMasterDocumentId = unchecked(0x02B9);

        public const int TagShortDocumentId = unchecked(0x02BA);

        public const int TagUniqueDocumentId = unchecked(0x02BB);

        public const int TagOwnerId = unchecked(0x02BC);

        public const int TagObjectPreviewFileFormat = unchecked(0x02C8);

        public const int TagObjectPreviewFileFormatVersion = unchecked(0x02C9);

        public const int TagObjectPreviewData = unchecked(0x02CA);

        [NotNull] private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static IptcDirectory()
        {
            // IPTC EnvelopeRecord Tags
            // 0 + 0x0100
            // 5
            // 20
            // 22
            // 30
            // 40
            // 50
            // 60
            // 70
            // 80
            // 90
            // 100
            // 120
            // 122
            // IPTC ApplicationRecord Tags
            // 0 + 0x0200
            // 3
            // 4
            // 5
            // 7
            // 8
            // 10
            // 12
            // 15
            // 20
            // 22
            // 25
            // 26
            // 27
            // 30
            // 35
            // 37
            // 38
            // 40
            // 42
            // 45
            // 47
            // 50
            // 55
            // 60
            // 62
            // 63
            // 65
            // 70
            // 75
            // 80
            // 85
            // 90
            // 92
            // 95
            // 100
            // 101
            // 103
            // 105
            // 110
            // 115
            // 116
            // 118
            // 120
            // 121
            // 122
            // 125
            // 130
            // 131
            // 135
            // 150
            // 151
            // 152
            // 153
            // 154
            // 184
            // 185
            // 186
            // 187
            // 188
            // 200
            // 201
            // 202
            TagNameMap[TagEnvelopeRecordVersion] = "Enveloped Record Version";
            TagNameMap[TagDestination] = "Destination";
            TagNameMap[TagFileFormat] = "File Format";
            TagNameMap[TagFileVersion] = "File Version";
            TagNameMap[TagServiceId] = "Service Identifier";
            TagNameMap[TagEnvelopeNumber] = "Envelope Number";
            TagNameMap[TagProductId] = "Product Identifier";
            TagNameMap[TagEnvelopePriority] = "Envelope Priority";
            TagNameMap[TagDateSent] = "Date Sent";
            TagNameMap[TagTimeSent] = "Time Sent";
            TagNameMap[TagCodedCharacterSet] = "Coded Character Set";
            TagNameMap[TagUniqueObjectName] = "Unique Object Name";
            TagNameMap[TagArmIdentifier] = "ARM Identifier";
            TagNameMap[TagArmVersion] = "ARM Version";
            TagNameMap[TagApplicationRecordVersion] = "Application Record Version";
            TagNameMap[TagObjectTypeReference] = "Object Type Reference";
            TagNameMap[TagObjectAttributeReference] = "Object Attribute Reference";
            TagNameMap[TagObjectName] = "Object Name";
            TagNameMap[TagEditStatus] = "Edit Status";
            TagNameMap[TagEditorialUpdate] = "Editorial Update";
            TagNameMap[TagUrgency] = "Urgency";
            TagNameMap[TagSubjectReference] = "Subject Reference";
            TagNameMap[TagCategory] = "Category";
            TagNameMap[TagSupplementalCategories] = "Supplemental Category(s)";
            TagNameMap[TagFixtureId] = "Fixture Identifier";
            TagNameMap[TagKeywords] = "Keywords";
            TagNameMap[TagContentLocationCode] = "Content Location Code";
            TagNameMap[TagContentLocationName] = "Content Location Name";
            TagNameMap[TagReleaseDate] = "Release Date";
            TagNameMap[TagReleaseTime] = "Release Time";
            TagNameMap[TagExpirationDate] = "Expiration Date";
            TagNameMap[TagExpirationTime] = "Expiration Time";
            TagNameMap[TagSpecialInstructions] = "Special Instructions";
            TagNameMap[TagActionAdvised] = "Action Advised";
            TagNameMap[TagReferenceService] = "Reference Service";
            TagNameMap[TagReferenceDate] = "Reference Date";
            TagNameMap[TagReferenceNumber] = "Reference Number";
            TagNameMap[TagDateCreated] = "Date Created";
            TagNameMap[TagTimeCreated] = "Time Created";
            TagNameMap[TagDigitalDateCreated] = "Digital Date Created";
            TagNameMap[TagDigitalTimeCreated] = "Digital Time Created";
            TagNameMap[TagOriginatingProgram] = "Originating Program";
            TagNameMap[TagProgramVersion] = "Program Version";
            TagNameMap[TagObjectCycle] = "Object Cycle";
            TagNameMap[TagByLine] = "By-line";
            TagNameMap[TagByLineTitle] = "By-line Title";
            TagNameMap[TagCity] = "City";
            TagNameMap[TagSubLocation] = "Sub-location";
            TagNameMap[TagProvinceOrState] = "Province/State";
            TagNameMap[TagCountryOrPrimaryLocationCode] = "Country/Primary Location Code";
            TagNameMap[TagCountryOrPrimaryLocationName] = "Country/Primary Location Name";
            TagNameMap[TagOriginalTransmissionReference] = "Original Transmission Reference";
            TagNameMap[TagHeadline] = "Headline";
            TagNameMap[TagCredit] = "Credit";
            TagNameMap[TagSource] = "Source";
            TagNameMap[TagCopyrightNotice] = "Copyright Notice";
            TagNameMap[TagContact] = "Contact";
            TagNameMap[TagCaption] = "Caption/Abstract";
            TagNameMap[TagLocalCaption] = "Local Caption";
            TagNameMap[TagCaptionWriter] = "Caption Writer/Editor";
            TagNameMap[TagRasterizedCaption] = "Rasterized Caption";
            TagNameMap[TagImageType] = "Image Type";
            TagNameMap[TagImageOrientation] = "Image Orientation";
            TagNameMap[TagLanguageIdentifier] = "Language Identifier";
            TagNameMap[TagAudioType] = "Audio Type";
            TagNameMap[TagAudioSamplingRate] = "Audio Sampling Rate";
            TagNameMap[TagAudioSamplingResolution] = "Audio Sampling Resolution";
            TagNameMap[TagAudioDuration] = "Audio Duration";
            TagNameMap[TagAudioOutcue] = "Audio Outcue";
            TagNameMap[TagJobId] = "Job Identifier";
            TagNameMap[TagMasterDocumentId] = "Master Document Identifier";
            TagNameMap[TagShortDocumentId] = "Short Document Identifier";
            TagNameMap[TagUniqueDocumentId] = "Unique Document Identifier";
            TagNameMap[TagOwnerId] = "Owner Identifier";
            TagNameMap[TagObjectPreviewFileFormat] = "Object Data Preview File Format";
            TagNameMap[TagObjectPreviewFileFormatVersion] = "Object Data Preview File Format Version";
            TagNameMap[TagObjectPreviewData] = "Object Data Preview Data";
        }

        public IptcDirectory()
        {
            SetDescriptor(new IptcDescriptor(this));
        }

        public override string Name
        {
            get { return "IPTC"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        /// <summary>Returns any keywords contained in the IPTC data.</summary>
        /// <remarks>Returns any keywords contained in the IPTC data.  This value may be <c>null</c>.</remarks>
        [CanBeNull]
        public IList<string> GetKeywords()
        {
            var array = GetStringArray(TagKeywords);
            if (array == null)
            {
                return null;
            }
            return array.ToList();
        }
    }
}
