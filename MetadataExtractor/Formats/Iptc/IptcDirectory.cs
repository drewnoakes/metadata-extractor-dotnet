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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Iptc
{
    /// <summary>Describes tags used by the International Press Telecommunications Council (IPTC) metadata format.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Akihiko Kusanagi https://github.com/nagix</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class IptcDirectory : Directory
    {
        public const int TagEnvelopeRecordVersion = 0x0100;
        public const int TagDestination = 0x0105;
        public const int TagFileFormat = 0x0114;
        public const int TagFileVersion = 0x0116;
        public const int TagServiceId = 0x011E;
        public const int TagEnvelopeNumber = 0x0128;
        public const int TagProductId = 0x0132;
        public const int TagEnvelopePriority = 0x013C;
        public const int TagDateSent = 0x0146;
        public const int TagTimeSent = 0x0150;
        public const int TagCodedCharacterSet = 0x015A;
        public const int TagUniqueObjectName = 0x0164;
        public const int TagArmIdentifier = 0x0178;
        public const int TagArmVersion = 0x017a;
        public const int TagApplicationRecordVersion = 0x0200;
        public const int TagObjectTypeReference = 0x0203;
        public const int TagObjectAttributeReference = 0x0204;
        public const int TagObjectName = 0x0205;
        public const int TagEditStatus = 0x0207;
        public const int TagEditorialUpdate = 0x0208;
        public const int TagUrgency = 0X020A;
        public const int TagSubjectReference = 0X020C;
        public const int TagCategory = 0x020F;
        public const int TagSupplementalCategories = 0x0214;
        public const int TagFixtureId = 0x0216;
        public const int TagKeywords = 0x0219;
        public const int TagContentLocationCode = 0x021A;
        public const int TagContentLocationName = 0x021B;
        public const int TagReleaseDate = 0X021E;
        public const int TagReleaseTime = 0x0223;
        public const int TagExpirationDate = 0x0225;
        public const int TagExpirationTime = 0x0226;
        public const int TagSpecialInstructions = 0x0228;
        public const int TagActionAdvised = 0x022A;
        public const int TagReferenceService = 0x022D;
        public const int TagReferenceDate = 0x022F;
        public const int TagReferenceNumber = 0x0232;
        public const int TagDateCreated = 0x0237;
        public const int TagTimeCreated = 0X023C;
        public const int TagDigitalDateCreated = 0x023E;
        public const int TagDigitalTimeCreated = 0x023F;
        public const int TagOriginatingProgram = 0x0241;
        public const int TagProgramVersion = 0x0246;
        public const int TagObjectCycle = 0x024B;
        public const int TagByLine = 0x0250;
        public const int TagByLineTitle = 0x0255;
        public const int TagCity = 0x025A;
        public const int TagSubLocation = 0x025C;
        public const int TagProvinceOrState = 0x025F;
        public const int TagCountryOrPrimaryLocationCode = 0x0264;
        public const int TagCountryOrPrimaryLocationName = 0x0265;
        public const int TagOriginalTransmissionReference = 0x0267;
        public const int TagHeadline = 0x0269;
        public const int TagCredit = 0x026E;
        public const int TagSource = 0x0273;
        public const int TagCopyrightNotice = 0x0274;
        public const int TagContact = 0x0276;
        public const int TagCaption = 0x0278;
        public const int TagLocalCaption = 0x0279;
        public const int TagCaptionWriter = 0x027A;
        public const int TagRasterizedCaption = 0x027D;
        public const int TagImageType = 0x0282;
        public const int TagImageOrientation = 0x0283;
        public const int TagLanguageIdentifier = 0x0287;
        public const int TagAudioType = 0x0296;
        public const int TagAudioSamplingRate = 0x0297;
        public const int TagAudioSamplingResolution = 0x0298;
        public const int TagAudioDuration = 0x0299;
        public const int TagAudioOutcue = 0x029A;
        public const int TagJobId = 0x02B8;
        public const int TagMasterDocumentId = 0x02B9;
        public const int TagShortDocumentId = 0x02BA;
        public const int TagUniqueDocumentId = 0x02BB;
        public const int TagOwnerId = 0x02BC;
        public const int TagObjectPreviewFileFormat = 0x02C8;
        public const int TagObjectPreviewFileFormatVersion = 0x02C9;
        public const int TagObjectPreviewData = 0x02CA;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagEnvelopeRecordVersion, "Enveloped Record Version" },
            { TagDestination, "Destination" },
            { TagFileFormat, "File Format" },
            { TagFileVersion, "File Version" },
            { TagServiceId, "Service Identifier" },
            { TagEnvelopeNumber, "Envelope Number" },
            { TagProductId, "Product Identifier" },
            { TagEnvelopePriority, "Envelope Priority" },
            { TagDateSent, "Date Sent" },
            { TagTimeSent, "Time Sent" },
            { TagCodedCharacterSet, "Coded Character Set" },
            { TagUniqueObjectName, "Unique Object Name" },
            { TagArmIdentifier, "ARM Identifier" },
            { TagArmVersion, "ARM Version" },
            { TagApplicationRecordVersion, "Application Record Version" },
            { TagObjectTypeReference, "Object Type Reference" },
            { TagObjectAttributeReference, "Object Attribute Reference" },
            { TagObjectName, "Object Name" },
            { TagEditStatus, "Edit Status" },
            { TagEditorialUpdate, "Editorial Update" },
            { TagUrgency, "Urgency" },
            { TagSubjectReference, "Subject Reference" },
            { TagCategory, "Category" },
            { TagSupplementalCategories, "Supplemental Category(s)" },
            { TagFixtureId, "Fixture Identifier" },
            { TagKeywords, "Keywords" },
            { TagContentLocationCode, "Content Location Code" },
            { TagContentLocationName, "Content Location Name" },
            { TagReleaseDate, "Release Date" },
            { TagReleaseTime, "Release Time" },
            { TagExpirationDate, "Expiration Date" },
            { TagExpirationTime, "Expiration Time" },
            { TagSpecialInstructions, "Special Instructions" },
            { TagActionAdvised, "Action Advised" },
            { TagReferenceService, "Reference Service" },
            { TagReferenceDate, "Reference Date" },
            { TagReferenceNumber, "Reference Number" },
            { TagDateCreated, "Date Created" },
            { TagTimeCreated, "Time Created" },
            { TagDigitalDateCreated, "Digital Date Created" },
            { TagDigitalTimeCreated, "Digital Time Created" },
            { TagOriginatingProgram, "Originating Program" },
            { TagProgramVersion, "Program Version" },
            { TagObjectCycle, "Object Cycle" },
            { TagByLine, "By-line" },
            { TagByLineTitle, "By-line Title" },
            { TagCity, "City" },
            { TagSubLocation, "Sub-location" },
            { TagProvinceOrState, "Province/State" },
            { TagCountryOrPrimaryLocationCode, "Country/Primary Location Code" },
            { TagCountryOrPrimaryLocationName, "Country/Primary Location Name" },
            { TagOriginalTransmissionReference, "Original Transmission Reference" },
            { TagHeadline, "Headline" },
            { TagCredit, "Credit" },
            { TagSource, "Source" },
            { TagCopyrightNotice, "Copyright Notice" },
            { TagContact, "Contact" },
            { TagCaption, "Caption/Abstract" },
            { TagLocalCaption, "Local Caption" },
            { TagCaptionWriter, "Caption Writer/Editor" },
            { TagRasterizedCaption, "Rasterized Caption" },
            { TagImageType, "Image Type" },
            { TagImageOrientation, "Image Orientation" },
            { TagLanguageIdentifier, "Language Identifier" },
            { TagAudioType, "Audio Type" },
            { TagAudioSamplingRate, "Audio Sampling Rate" },
            { TagAudioSamplingResolution, "Audio Sampling Resolution" },
            { TagAudioDuration, "Audio Duration" },
            { TagAudioOutcue, "Audio Outcue" },
            { TagJobId, "Job Identifier" },
            { TagMasterDocumentId, "Master Document Identifier" },
            { TagShortDocumentId, "Short Document Identifier" },
            { TagUniqueDocumentId, "Unique Document Identifier" },
            { TagOwnerId, "Owner Identifier" },
            { TagObjectPreviewFileFormat, "Object Data Preview File Format" },
            { TagObjectPreviewFileFormatVersion, "Object Data Preview File Format Version" },
            { TagObjectPreviewData, "Object Data Preview Data" }
        };

        public IptcDirectory()
        {
            SetDescriptor(new IptcDescriptor(this));
        }

        public override string Name => "IPTC";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <summary>Returns any keywords contained in the IPTC data.</summary>
        /// <remarks>This value may be <c>null</c>.</remarks>
        [CanBeNull]
        public IList<string> GetKeywords()
        {
            return this.GetStringArray(TagKeywords)?.ToList();
        }

        /// <summary>
        /// Combines tags <see cref="TagDateSent" /> and <see cref="TagTimeSent"/> to obtain a single
        /// <see cref="DateTimeOffset"/> representing when the service sent this image.
        /// </summary>
        /// <returns>When the service sent this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public DateTimeOffset? GetDateSent()
        {
            return GetDate(TagDateSent, TagTimeSent);
        }

        /// <summary>
        /// Combines tags <see cref="TagReleaseDate" /> and <see cref="TagReleaseTime"/> to obtain a single
        /// <see cref="DateTimeOffset"/> representing when this image was released.
        /// </summary>
        /// <returns>When this image was released, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public DateTimeOffset? GetReleaseDate()
        {
            return GetDate(TagReleaseDate, TagReleaseTime);
        }

        /// <summary>
        /// Combines tags <see cref="TagExpirationDate" /> and <see cref="TagExpirationTime"/> to obtain a single
        /// <see cref="DateTimeOffset"/> after which this image should not be used.
        /// </summary>
        /// <returns>When this image should expire, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public DateTimeOffset? GetExpirationDate()
        {
            return GetDate(TagExpirationDate, TagExpirationTime);
        }

        /// <summary>
        /// Combines tags <see cref="TagDateCreated" /> and <see cref="TagTimeCreated"/> to obtain a single
        /// <see cref="DateTimeOffset"/> representing when this image was captured.
        /// </summary>
        /// <returns>When this image was released, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public DateTimeOffset? GetDateCreated()
        {
            return GetDate(TagDateCreated, TagTimeCreated);
        }

        /// <summary>
        /// Combines tags <see cref="TagDateCreated" /> and <see cref="TagTimeCreated"/> to obtain a single
        /// <see cref="DateTimeOffset"/> representing when the digital representation of this image was created.
        /// </summary>
        /// <returns>When the digital representation of this image was created, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public DateTimeOffset? GetDigitalDateCreated()
        {
            return GetDate(TagDigitalDateCreated, TagDigitalTimeCreated);
        }

        private static readonly string[] _formats = { "yyyyMMddHHmmsszzz", "yyyyMMddHHmmss" };

        [CanBeNull]
        private DateTimeOffset? GetDate(int dateTagType, int timeTagType)
        {
            var date = this.GetString(dateTagType);
            var time = this.GetString(timeTagType);

            if (date == null || time == null)
                return null;

            if (DateTimeOffset.TryParseExact(date + time, _formats, null, DateTimeStyles.None, out DateTimeOffset result))
                return result;

            return null;
        }
    }
}
