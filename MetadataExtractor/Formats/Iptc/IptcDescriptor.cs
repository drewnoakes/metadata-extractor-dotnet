// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using static MetadataExtractor.Formats.Iptc.IptcDirectory;

namespace MetadataExtractor.Formats.Iptc
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="IptcDirectory"/>.
    /// <para />
    /// As the IPTC directory already stores values as strings, this class simply returns the tag's value.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Akihiko Kusanagi https://github.com/nagix</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class IptcDescriptor : TagDescriptor<IptcDirectory>
    {
        public IptcDescriptor(IptcDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                TagDateCreated => GetDateCreatedDescription(),
                TagDigitalDateCreated => GetDigitalDateCreatedDescription(),
                TagDateSent => GetDateSentDescription(),
                TagExpirationDate => GetExpirationDateDescription(),
                TagExpirationTime => GetExpirationTimeDescription(),
                TagFileFormat => GetFileFormatDescription(),
                TagKeywords => GetKeywordsDescription(),
                TagReferenceDate => GetReferenceDateDescription(),
                TagReleaseDate => GetReleaseDateDescription(),
                TagReleaseTime => GetReleaseTimeDescription(),
                TagTimeCreated => GetTimeCreatedDescription(),
                TagDigitalTimeCreated => GetDigitalTimeCreatedDescription(),
                TagTimeSent => GetTimeSentDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        private string? GetDateDescription(int tagType)
        {
            var s = Directory.GetString(tagType);
            if (s == null)
                return null;
            if (s.Length == 8)
                return s.Substring(0, 4) + ':' + s.Substring(4, 2) + ':' + s.Substring(6);
            return s;
        }

        private string? GetTimeDescription(int tagType)
        {
            var s = Directory.GetString(tagType);
            if (s == null)
                return null;
            if (s.Length == 6 || s.Length == 11)
                return s.Substring(0, 2) + ':' + s.Substring(2, 2) + ':' + s.Substring(4);
            return s;
        }

        public string? GetFileFormatDescription()
        {
            return GetIndexedDescription(TagFileFormat,
                "No ObjectData",
                "IPTC-NAA Digital Newsphoto Parameter Record",
                "IPTC7901 Recommended Message Format",
                "Tagged Image File Format (Adobe/Aldus Image data)",
                "Illustrator (Adobe Graphics data)",
                "AppleSingle (Apple Computer Inc)",
                "NAA 89-3 (ANPA 1312)",
                "MacBinary II",
                "IPTC Unstructured Character Oriented File Format (UCOFF)",
                "United Press International ANPA 1312 variant",
                "United Press International Down-Load Message",
                "JPEG File Interchange (JFIF)",
                "Photo-CD Image-Pac (Eastman Kodak)",
                "Bit Mapped Graphics File [.BMP] (Microsoft)",
                "Digital Audio File [.WAV] (Microsoft & Creative Labs)",
                "Audio plus Moving Video [.AVI] (Microsoft)",
                "PC DOS/Windows Executable Files [.COM][.EXE]",
                "Compressed Binary File [.ZIP] (PKWare Inc)",
                "Audio Interchange File Format AIFF (Apple Computer Inc)",
                "RIFF Wave (Microsoft Corporation)",
                "Freehand (Macromedia/Aldus)",
                "Hypertext Markup Language [.HTML] (The Internet Society)",
                "MPEG 2 Audio Layer 2 (Musicom), ISO/IEC",
                "MPEG 2 Audio Layer 3, ISO/IEC",
                "Portable Document File [.PDF] Adobe",
                "News Industry Text Format (NITF)",
                "Tape Archive [.TAR]",
                "Tidningarnas Telegrambyra NITF version (TTNITF DTD)",
                "Ritzaus Bureau NITF version (RBNITF DTD)",
                "Corel Draw [.CDR]");
        }

        public string? GetByLineDescription()
        {
            return Directory.GetString(TagByLine);
        }

        public string? GetByLineTitleDescription()
        {
            return Directory.GetString(TagByLineTitle);
        }

        public string? GetCaptionDescription()
        {
            return Directory.GetString(TagCaption);
        }

        public string? GetCategoryDescription()
        {
            return Directory.GetString(TagCategory);
        }

        public string? GetCityDescription()
        {
            return Directory.GetString(TagCity);
        }

        public string? GetCopyrightNoticeDescription()
        {
            return Directory.GetString(TagCopyrightNotice);
        }

        public string? GetCountryOrPrimaryLocationDescription()
        {
            return Directory.GetString(TagCountryOrPrimaryLocationName);
        }

        public string? GetCreditDescription()
        {
            return Directory.GetString(TagCredit);
        }

        public string? GetDateCreatedDescription()
        {
            return GetDateDescription(TagDateCreated);
        }

        public string? GetDigitalDateCreatedDescription()
        {
            return GetDateDescription(TagDigitalDateCreated);
        }

        public string? GetDateSentDescription()
        {
            return GetDateDescription(TagDateSent);
        }

        public string? GetExpirationDateDescription()
        {
            return GetDateDescription(TagExpirationDate);
        }

        public string? GetExpirationTimeDescription()
        {
            return GetTimeDescription(TagExpirationTime);
        }

        public string? GetHeadlineDescription()
        {
            return Directory.GetString(TagHeadline);
        }

        public string? GetKeywordsDescription()
        {
            var keywords = Directory.GetStringArray(TagKeywords);
            return keywords == null ? null : string.Join(";", keywords);
        }

        public string? GetObjectNameDescription()
        {
            return Directory.GetString(TagObjectName);
        }

        public string? GetOriginalTransmissionReferenceDescription()
        {
            return Directory.GetString(TagOriginalTransmissionReference);
        }

        public string? GetOriginatingProgramDescription()
        {
            return Directory.GetString(TagOriginatingProgram);
        }

        public string? GetProvinceOrStateDescription()
        {
            return Directory.GetString(TagProvinceOrState);
        }

        public string? GetRecordVersionDescription()
        {
            return Directory.GetString(TagApplicationRecordVersion);
        }

        public string? GetReferenceDateDescription()
        {
            return GetDateDescription(TagReferenceDate);
        }

        public string? GetReleaseDateDescription()
        {
            return GetDateDescription(TagReleaseDate);
        }

        public string? GetReleaseTimeDescription()
        {
            return GetTimeDescription(TagReleaseTime);
        }

        public string? GetTimeSentDescription()
        {
            return GetTimeDescription(TagTimeSent);
        }

        public string? GetSourceDescription()
        {
            return Directory.GetString(TagSource);
        }

        public string? GetSpecialInstructionsDescription()
        {
            return Directory.GetString(TagSpecialInstructions);
        }

        public string? GetSupplementalCategoriesDescription()
        {
            return Directory.GetString(TagSupplementalCategories);
        }

        public string? GetTimeCreatedDescription()
        {
            return GetTimeDescription(TagTimeCreated);
        }

        public string? GetDigitalTimeCreatedDescription()
        {
            return GetTimeDescription(TagDigitalTimeCreated);
        }

        public string? GetUrgencyDescription()
        {
            return Directory.GetString(TagUrgency);
        }

        public string? GetWriterDescription()
        {
            return Directory.GetString(TagCaptionWriter);
        }
    }
}
