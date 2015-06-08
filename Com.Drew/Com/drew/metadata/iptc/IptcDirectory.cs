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
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Iptc
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
            TagNameMap.Put(TagEnvelopeRecordVersion, "Enveloped Record Version");
            TagNameMap.Put(TagDestination, "Destination");
            TagNameMap.Put(TagFileFormat, "File Format");
            TagNameMap.Put(TagFileVersion, "File Version");
            TagNameMap.Put(TagServiceId, "Service Identifier");
            TagNameMap.Put(TagEnvelopeNumber, "Envelope Number");
            TagNameMap.Put(TagProductId, "Product Identifier");
            TagNameMap.Put(TagEnvelopePriority, "Envelope Priority");
            TagNameMap.Put(TagDateSent, "Date Sent");
            TagNameMap.Put(TagTimeSent, "Time Sent");
            TagNameMap.Put(TagCodedCharacterSet, "Coded Character Set");
            TagNameMap.Put(TagUniqueObjectName, "Unique Object Name");
            TagNameMap.Put(TagArmIdentifier, "ARM Identifier");
            TagNameMap.Put(TagArmVersion, "ARM Version");
            TagNameMap.Put(TagApplicationRecordVersion, "Application Record Version");
            TagNameMap.Put(TagObjectTypeReference, "Object Type Reference");
            TagNameMap.Put(TagObjectAttributeReference, "Object Attribute Reference");
            TagNameMap.Put(TagObjectName, "Object Name");
            TagNameMap.Put(TagEditStatus, "Edit Status");
            TagNameMap.Put(TagEditorialUpdate, "Editorial Update");
            TagNameMap.Put(TagUrgency, "Urgency");
            TagNameMap.Put(TagSubjectReference, "Subject Reference");
            TagNameMap.Put(TagCategory, "Category");
            TagNameMap.Put(TagSupplementalCategories, "Supplemental Category(s)");
            TagNameMap.Put(TagFixtureId, "Fixture Identifier");
            TagNameMap.Put(TagKeywords, "Keywords");
            TagNameMap.Put(TagContentLocationCode, "Content Location Code");
            TagNameMap.Put(TagContentLocationName, "Content Location Name");
            TagNameMap.Put(TagReleaseDate, "Release Date");
            TagNameMap.Put(TagReleaseTime, "Release Time");
            TagNameMap.Put(TagExpirationDate, "Expiration Date");
            TagNameMap.Put(TagExpirationTime, "Expiration Time");
            TagNameMap.Put(TagSpecialInstructions, "Special Instructions");
            TagNameMap.Put(TagActionAdvised, "Action Advised");
            TagNameMap.Put(TagReferenceService, "Reference Service");
            TagNameMap.Put(TagReferenceDate, "Reference Date");
            TagNameMap.Put(TagReferenceNumber, "Reference Number");
            TagNameMap.Put(TagDateCreated, "Date Created");
            TagNameMap.Put(TagTimeCreated, "Time Created");
            TagNameMap.Put(TagDigitalDateCreated, "Digital Date Created");
            TagNameMap.Put(TagDigitalTimeCreated, "Digital Time Created");
            TagNameMap.Put(TagOriginatingProgram, "Originating Program");
            TagNameMap.Put(TagProgramVersion, "Program Version");
            TagNameMap.Put(TagObjectCycle, "Object Cycle");
            TagNameMap.Put(TagByLine, "By-line");
            TagNameMap.Put(TagByLineTitle, "By-line Title");
            TagNameMap.Put(TagCity, "City");
            TagNameMap.Put(TagSubLocation, "Sub-location");
            TagNameMap.Put(TagProvinceOrState, "Province/State");
            TagNameMap.Put(TagCountryOrPrimaryLocationCode, "Country/Primary Location Code");
            TagNameMap.Put(TagCountryOrPrimaryLocationName, "Country/Primary Location Name");
            TagNameMap.Put(TagOriginalTransmissionReference, "Original Transmission Reference");
            TagNameMap.Put(TagHeadline, "Headline");
            TagNameMap.Put(TagCredit, "Credit");
            TagNameMap.Put(TagSource, "Source");
            TagNameMap.Put(TagCopyrightNotice, "Copyright Notice");
            TagNameMap.Put(TagContact, "Contact");
            TagNameMap.Put(TagCaption, "Caption/Abstract");
            TagNameMap.Put(TagLocalCaption, "Local Caption");
            TagNameMap.Put(TagCaptionWriter, "Caption Writer/Editor");
            TagNameMap.Put(TagRasterizedCaption, "Rasterized Caption");
            TagNameMap.Put(TagImageType, "Image Type");
            TagNameMap.Put(TagImageOrientation, "Image Orientation");
            TagNameMap.Put(TagLanguageIdentifier, "Language Identifier");
            TagNameMap.Put(TagAudioType, "Audio Type");
            TagNameMap.Put(TagAudioSamplingRate, "Audio Sampling Rate");
            TagNameMap.Put(TagAudioSamplingResolution, "Audio Sampling Resolution");
            TagNameMap.Put(TagAudioDuration, "Audio Duration");
            TagNameMap.Put(TagAudioOutcue, "Audio Outcue");
            TagNameMap.Put(TagJobId, "Job Identifier");
            TagNameMap.Put(TagMasterDocumentId, "Master Document Identifier");
            TagNameMap.Put(TagShortDocumentId, "Short Document Identifier");
            TagNameMap.Put(TagUniqueDocumentId, "Unique Document Identifier");
            TagNameMap.Put(TagOwnerId, "Owner Identifier");
            TagNameMap.Put(TagObjectPreviewFileFormat, "Object Data Preview File Format");
            TagNameMap.Put(TagObjectPreviewFileFormatVersion, "Object Data Preview File Format Version");
            TagNameMap.Put(TagObjectPreviewData, "Object Data Preview Data");
        }

        public IptcDirectory()
        {
            SetDescriptor(new IptcDescriptor(this));
        }

        public override string GetName()
        {
            return "IPTC";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        /// <summary>Returns any keywords contained in the IPTC data.</summary>
        /// <remarks>Returns any keywords contained in the IPTC data.  This value may be <c>null</c>.</remarks>
        [CanBeNull]
        public IList<string> GetKeywords()
        {
            string[] array = GetStringArray(TagKeywords);
            if (array == null)
            {
                return null;
            }
            return Arrays.AsList(array);
        }
    }
}
