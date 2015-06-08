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
    public class IptcDirectory : Directory
    {
        public const int TagEnvelopeRecordVersion = unchecked((int)(0x0100));

        public const int TagDestination = unchecked((int)(0x0105));

        public const int TagFileFormat = unchecked((int)(0x0114));

        public const int TagFileVersion = unchecked((int)(0x0116));

        public const int TagServiceId = unchecked((int)(0x011E));

        public const int TagEnvelopeNumber = unchecked((int)(0x0128));

        public const int TagProductId = unchecked((int)(0x0132));

        public const int TagEnvelopePriority = unchecked((int)(0x013C));

        public const int TagDateSent = unchecked((int)(0x0146));

        public const int TagTimeSent = unchecked((int)(0x0150));

        public const int TagCodedCharacterSet = unchecked((int)(0x015A));

        public const int TagUniqueObjectName = unchecked((int)(0x0164));

        public const int TagArmIdentifier = unchecked((int)(0x0178));

        public const int TagArmVersion = unchecked((int)(0x017a));

        public const int TagApplicationRecordVersion = unchecked((int)(0x0200));

        public const int TagObjectTypeReference = unchecked((int)(0x0203));

        public const int TagObjectAttributeReference = unchecked((int)(0x0204));

        public const int TagObjectName = unchecked((int)(0x0205));

        public const int TagEditStatus = unchecked((int)(0x0207));

        public const int TagEditorialUpdate = unchecked((int)(0x0208));

        public const int TagUrgency = 0X020A;

        public const int TagSubjectReference = 0X020C;

        public const int TagCategory = unchecked((int)(0x020F));

        public const int TagSupplementalCategories = unchecked((int)(0x0214));

        public const int TagFixtureId = unchecked((int)(0x0216));

        public const int TagKeywords = unchecked((int)(0x0219));

        public const int TagContentLocationCode = unchecked((int)(0x021A));

        public const int TagContentLocationName = unchecked((int)(0x021B));

        public const int TagReleaseDate = 0X021E;

        public const int TagReleaseTime = unchecked((int)(0x0223));

        public const int TagExpirationDate = unchecked((int)(0x0225));

        public const int TagExpirationTime = unchecked((int)(0x0226));

        public const int TagSpecialInstructions = unchecked((int)(0x0228));

        public const int TagActionAdvised = unchecked((int)(0x022A));

        public const int TagReferenceService = unchecked((int)(0x022D));

        public const int TagReferenceDate = unchecked((int)(0x022F));

        public const int TagReferenceNumber = unchecked((int)(0x0232));

        public const int TagDateCreated = unchecked((int)(0x0237));

        public const int TagTimeCreated = 0X023C;

        public const int TagDigitalDateCreated = unchecked((int)(0x023E));

        public const int TagDigitalTimeCreated = unchecked((int)(0x023F));

        public const int TagOriginatingProgram = unchecked((int)(0x0241));

        public const int TagProgramVersion = unchecked((int)(0x0246));

        public const int TagObjectCycle = unchecked((int)(0x024B));

        public const int TagByLine = unchecked((int)(0x0250));

        public const int TagByLineTitle = unchecked((int)(0x0255));

        public const int TagCity = unchecked((int)(0x025A));

        public const int TagSubLocation = unchecked((int)(0x025C));

        public const int TagProvinceOrState = unchecked((int)(0x025F));

        public const int TagCountryOrPrimaryLocationCode = unchecked((int)(0x0264));

        public const int TagCountryOrPrimaryLocationName = unchecked((int)(0x0265));

        public const int TagOriginalTransmissionReference = unchecked((int)(0x0267));

        public const int TagHeadline = unchecked((int)(0x0269));

        public const int TagCredit = unchecked((int)(0x026E));

        public const int TagSource = unchecked((int)(0x0273));

        public const int TagCopyrightNotice = unchecked((int)(0x0274));

        public const int TagContact = unchecked((int)(0x0276));

        public const int TagCaption = unchecked((int)(0x0278));

        public const int TagLocalCaption = unchecked((int)(0x0279));

        public const int TagCaptionWriter = unchecked((int)(0x027A));

        public const int TagRasterizedCaption = unchecked((int)(0x027D));

        public const int TagImageType = unchecked((int)(0x0282));

        public const int TagImageOrientation = unchecked((int)(0x0283));

        public const int TagLanguageIdentifier = unchecked((int)(0x0287));

        public const int TagAudioType = unchecked((int)(0x0296));

        public const int TagAudioSamplingRate = unchecked((int)(0x0297));

        public const int TagAudioSamplingResolution = unchecked((int)(0x0298));

        public const int TagAudioDuration = unchecked((int)(0x0299));

        public const int TagAudioOutcue = unchecked((int)(0x029A));

        public const int TagJobId = unchecked((int)(0x02B8));

        public const int TagMasterDocumentId = unchecked((int)(0x02B9));

        public const int TagShortDocumentId = unchecked((int)(0x02BA));

        public const int TagUniqueDocumentId = unchecked((int)(0x02BB));

        public const int TagOwnerId = unchecked((int)(0x02BC));

        public const int TagObjectPreviewFileFormat = unchecked((int)(0x02C8));

        public const int TagObjectPreviewFileFormatVersion = unchecked((int)(0x02C9));

        public const int TagObjectPreviewData = unchecked((int)(0x02CA));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

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
            _tagNameMap.Put(TagEnvelopeRecordVersion, "Enveloped Record Version");
            _tagNameMap.Put(TagDestination, "Destination");
            _tagNameMap.Put(TagFileFormat, "File Format");
            _tagNameMap.Put(TagFileVersion, "File Version");
            _tagNameMap.Put(TagServiceId, "Service Identifier");
            _tagNameMap.Put(TagEnvelopeNumber, "Envelope Number");
            _tagNameMap.Put(TagProductId, "Product Identifier");
            _tagNameMap.Put(TagEnvelopePriority, "Envelope Priority");
            _tagNameMap.Put(TagDateSent, "Date Sent");
            _tagNameMap.Put(TagTimeSent, "Time Sent");
            _tagNameMap.Put(TagCodedCharacterSet, "Coded Character Set");
            _tagNameMap.Put(TagUniqueObjectName, "Unique Object Name");
            _tagNameMap.Put(TagArmIdentifier, "ARM Identifier");
            _tagNameMap.Put(TagArmVersion, "ARM Version");
            _tagNameMap.Put(TagApplicationRecordVersion, "Application Record Version");
            _tagNameMap.Put(TagObjectTypeReference, "Object Type Reference");
            _tagNameMap.Put(TagObjectAttributeReference, "Object Attribute Reference");
            _tagNameMap.Put(TagObjectName, "Object Name");
            _tagNameMap.Put(TagEditStatus, "Edit Status");
            _tagNameMap.Put(TagEditorialUpdate, "Editorial Update");
            _tagNameMap.Put(TagUrgency, "Urgency");
            _tagNameMap.Put(TagSubjectReference, "Subject Reference");
            _tagNameMap.Put(TagCategory, "Category");
            _tagNameMap.Put(TagSupplementalCategories, "Supplemental Category(s)");
            _tagNameMap.Put(TagFixtureId, "Fixture Identifier");
            _tagNameMap.Put(TagKeywords, "Keywords");
            _tagNameMap.Put(TagContentLocationCode, "Content Location Code");
            _tagNameMap.Put(TagContentLocationName, "Content Location Name");
            _tagNameMap.Put(TagReleaseDate, "Release Date");
            _tagNameMap.Put(TagReleaseTime, "Release Time");
            _tagNameMap.Put(TagExpirationDate, "Expiration Date");
            _tagNameMap.Put(TagExpirationTime, "Expiration Time");
            _tagNameMap.Put(TagSpecialInstructions, "Special Instructions");
            _tagNameMap.Put(TagActionAdvised, "Action Advised");
            _tagNameMap.Put(TagReferenceService, "Reference Service");
            _tagNameMap.Put(TagReferenceDate, "Reference Date");
            _tagNameMap.Put(TagReferenceNumber, "Reference Number");
            _tagNameMap.Put(TagDateCreated, "Date Created");
            _tagNameMap.Put(TagTimeCreated, "Time Created");
            _tagNameMap.Put(TagDigitalDateCreated, "Digital Date Created");
            _tagNameMap.Put(TagDigitalTimeCreated, "Digital Time Created");
            _tagNameMap.Put(TagOriginatingProgram, "Originating Program");
            _tagNameMap.Put(TagProgramVersion, "Program Version");
            _tagNameMap.Put(TagObjectCycle, "Object Cycle");
            _tagNameMap.Put(TagByLine, "By-line");
            _tagNameMap.Put(TagByLineTitle, "By-line Title");
            _tagNameMap.Put(TagCity, "City");
            _tagNameMap.Put(TagSubLocation, "Sub-location");
            _tagNameMap.Put(TagProvinceOrState, "Province/State");
            _tagNameMap.Put(TagCountryOrPrimaryLocationCode, "Country/Primary Location Code");
            _tagNameMap.Put(TagCountryOrPrimaryLocationName, "Country/Primary Location Name");
            _tagNameMap.Put(TagOriginalTransmissionReference, "Original Transmission Reference");
            _tagNameMap.Put(TagHeadline, "Headline");
            _tagNameMap.Put(TagCredit, "Credit");
            _tagNameMap.Put(TagSource, "Source");
            _tagNameMap.Put(TagCopyrightNotice, "Copyright Notice");
            _tagNameMap.Put(TagContact, "Contact");
            _tagNameMap.Put(TagCaption, "Caption/Abstract");
            _tagNameMap.Put(TagLocalCaption, "Local Caption");
            _tagNameMap.Put(TagCaptionWriter, "Caption Writer/Editor");
            _tagNameMap.Put(TagRasterizedCaption, "Rasterized Caption");
            _tagNameMap.Put(TagImageType, "Image Type");
            _tagNameMap.Put(TagImageOrientation, "Image Orientation");
            _tagNameMap.Put(TagLanguageIdentifier, "Language Identifier");
            _tagNameMap.Put(TagAudioType, "Audio Type");
            _tagNameMap.Put(TagAudioSamplingRate, "Audio Sampling Rate");
            _tagNameMap.Put(TagAudioSamplingResolution, "Audio Sampling Resolution");
            _tagNameMap.Put(TagAudioDuration, "Audio Duration");
            _tagNameMap.Put(TagAudioOutcue, "Audio Outcue");
            _tagNameMap.Put(TagJobId, "Job Identifier");
            _tagNameMap.Put(TagMasterDocumentId, "Master Document Identifier");
            _tagNameMap.Put(TagShortDocumentId, "Short Document Identifier");
            _tagNameMap.Put(TagUniqueDocumentId, "Unique Document Identifier");
            _tagNameMap.Put(TagOwnerId, "Owner Identifier");
            _tagNameMap.Put(TagObjectPreviewFileFormat, "Object Data Preview File Format");
            _tagNameMap.Put(TagObjectPreviewFileFormatVersion, "Object Data Preview File Format Version");
            _tagNameMap.Put(TagObjectPreviewData, "Object Data Preview Data");
        }

        public IptcDirectory()
        {
            this.SetDescriptor(new IptcDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "IPTC";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }

        /// <summary>Returns any keywords contained in the IPTC data.</summary>
        /// <remarks>Returns any keywords contained in the IPTC data.  This value may be <code>null</code>.</remarks>
        [CanBeNull]
        public virtual IList<string> GetKeywords()
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
