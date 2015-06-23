#region License
//
// Copyright 2002-2015 Drew Noakes
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

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Iptc
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="IptcDirectory"/>.
    /// <para />
    /// As the IPTC directory already stores values as strings, this class simply returns the tag's value.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcDescriptor : TagDescriptor<IptcDirectory>
    {
        public IptcDescriptor([NotNull] IptcDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case IptcDirectory.TagFileFormat:
                {
                    return GetFileFormatDescription();
                }

                case IptcDirectory.TagKeywords:
                {
                    return GetKeywordsDescription();
                }

                case IptcDirectory.TagTimeCreated:
                {
                    return GetTimeCreatedDescription();
                }

                case IptcDirectory.TagDigitalTimeCreated:
                {
                    return GetDigitalTimeCreatedDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetFileFormatDescription()
        {
            int value;
            if (!Directory.TryGetInt32(IptcDirectory.TagFileFormat, out value))
                return null;
            switch (value)
            {
                case 0:
                {
                    return "No ObjectData";
                }

                case 1:
                {
                    return "IPTC-NAA Digital Newsphoto Parameter Record";
                }

                case 2:
                {
                    return "IPTC7901 Recommended Message Format";
                }

                case 3:
                {
                    return "Tagged Image File Format (Adobe/Aldus Image data)";
                }

                case 4:
                {
                    return "Illustrator (Adobe Graphics data)";
                }

                case 5:
                {
                    return "AppleSingle (Apple Computer Inc)";
                }

                case 6:
                {
                    return "NAA 89-3 (ANPA 1312)";
                }

                case 7:
                {
                    return "MacBinary II";
                }

                case 8:
                {
                    return "IPTC Unstructured Character Oriented File Format (UCOFF)";
                }

                case 9:
                {
                    return "United Press International ANPA 1312 variant";
                }

                case 10:
                {
                    return "United Press International Down-Load Message";
                }

                case 11:
                {
                    return "JPEG File Interchange (JFIF)";
                }

                case 12:
                {
                    return "Photo-CD Image-Pac (Eastman Kodak)";
                }

                case 13:
                {
                    return "Bit Mapped Graphics File [.BMP] (Microsoft)";
                }

                case 14:
                {
                    return "Digital Audio File [.WAV] (Microsoft & Creative Labs)";
                }

                case 15:
                {
                    return "Audio plus Moving Video [.AVI] (Microsoft)";
                }

                case 16:
                {
                    return "PC DOS/Windows Executable Files [.COM][.EXE]";
                }

                case 17:
                {
                    return "Compressed Binary File [.ZIP] (PKWare Inc)";
                }

                case 18:
                {
                    return "Audio Interchange File Format AIFF (Apple Computer Inc)";
                }

                case 19:
                {
                    return "RIFF Wave (Microsoft Corporation)";
                }

                case 20:
                {
                    return "Freehand (Macromedia/Aldus)";
                }

                case 21:
                {
                    return "Hypertext Markup Language [.HTML] (The Internet Society)";
                }

                case 22:
                {
                    return "MPEG 2 Audio Layer 2 (Musicom), ISO/IEC";
                }

                case 23:
                {
                    return "MPEG 2 Audio Layer 3, ISO/IEC";
                }

                case 24:
                {
                    return "Portable Document File [.PDF] Adobe";
                }

                case 25:
                {
                    return "News Industry Text Format (NITF)";
                }

                case 26:
                {
                    return "Tape Archive [.TAR]";
                }

                case 27:
                {
                    return "Tidningarnas Telegrambyra NITF version (TTNITF DTD)";
                }

                case 28:
                {
                    return "Ritzaus Bureau NITF version (RBNITF DTD)";
                }

                case 29:
                {
                    return "Corel Draw [.CDR]";
                }
            }
            return string.Format("Unknown ({0})", value);
        }

        [CanBeNull]
        public string GetByLineDescription()
        {
            return Directory.GetString(IptcDirectory.TagByLine);
        }

        [CanBeNull]
        public string GetByLineTitleDescription()
        {
            return Directory.GetString(IptcDirectory.TagByLineTitle);
        }

        [CanBeNull]
        public string GetCaptionDescription()
        {
            return Directory.GetString(IptcDirectory.TagCaption);
        }

        [CanBeNull]
        public string GetCategoryDescription()
        {
            return Directory.GetString(IptcDirectory.TagCategory);
        }

        [CanBeNull]
        public string GetCityDescription()
        {
            return Directory.GetString(IptcDirectory.TagCity);
        }

        [CanBeNull]
        public string GetCopyrightNoticeDescription()
        {
            return Directory.GetString(IptcDirectory.TagCopyrightNotice);
        }

        [CanBeNull]
        public string GetCountryOrPrimaryLocationDescription()
        {
            return Directory.GetString(IptcDirectory.TagCountryOrPrimaryLocationName);
        }

        [CanBeNull]
        public string GetCreditDescription()
        {
            return Directory.GetString(IptcDirectory.TagCredit);
        }

        [CanBeNull]
        public string GetDateCreatedDescription()
        {
            return Directory.GetString(IptcDirectory.TagDateCreated);
        }

        [CanBeNull]
        public string GetHeadlineDescription()
        {
            return Directory.GetString(IptcDirectory.TagHeadline);
        }

        [CanBeNull]
        public string GetKeywordsDescription()
        {
            var keywords = Directory.GetStringArray(IptcDirectory.TagKeywords);
            if (keywords == null)
            {
                return null;
            }
            return string.Join(";", keywords);
        }

        [CanBeNull]
        public string GetObjectNameDescription()
        {
            return Directory.GetString(IptcDirectory.TagObjectName);
        }

        [CanBeNull]
        public string GetOriginalTransmissionReferenceDescription()
        {
            return Directory.GetString(IptcDirectory.TagOriginalTransmissionReference);
        }

        [CanBeNull]
        public string GetOriginatingProgramDescription()
        {
            return Directory.GetString(IptcDirectory.TagOriginatingProgram);
        }

        [CanBeNull]
        public string GetProvinceOrStateDescription()
        {
            return Directory.GetString(IptcDirectory.TagProvinceOrState);
        }

        [CanBeNull]
        public string GetRecordVersionDescription()
        {
            return Directory.GetString(IptcDirectory.TagApplicationRecordVersion);
        }

        [CanBeNull]
        public string GetReleaseDateDescription()
        {
            return Directory.GetString(IptcDirectory.TagReleaseDate);
        }

        [CanBeNull]
        public string GetReleaseTimeDescription()
        {
            return Directory.GetString(IptcDirectory.TagReleaseTime);
        }

        [CanBeNull]
        public string GetSourceDescription()
        {
            return Directory.GetString(IptcDirectory.TagSource);
        }

        [CanBeNull]
        public string GetSpecialInstructionsDescription()
        {
            return Directory.GetString(IptcDirectory.TagSpecialInstructions);
        }

        [CanBeNull]
        public string GetSupplementalCategoriesDescription()
        {
            return Directory.GetString(IptcDirectory.TagSupplementalCategories);
        }

        [CanBeNull]
        public string GetTimeCreatedDescription()
        {
            var s = Directory.GetString(IptcDirectory.TagTimeCreated);
            if (s == null)
            {
                return null;
            }
            if (s.Length == 6 || s.Length == 11)
            {
                return s.Substring (0, 2 - 0) + ':' + s.Substring (2, 4 - 2) + ':' + s.Substring (4);
            }
            return s;
        }

        [CanBeNull]
        public string GetDigitalTimeCreatedDescription()
        {
            var s = Directory.GetString(IptcDirectory.TagDigitalTimeCreated);
            if (s == null)
            {
                return null;
            }
            if (s.Length == 6 || s.Length == 11)
            {
                return s.Substring (0, 2 - 0) + ':' + s.Substring (2, 4 - 2) + ':' + s.Substring (4);
            }
            return s;
        }

        [CanBeNull]
        public string GetUrgencyDescription()
        {
            return Directory.GetString(IptcDirectory.TagUrgency);
        }

        [CanBeNull]
        public string GetWriterDescription()
        {
            return Directory.GetString(IptcDirectory.TagCaptionWriter);
        }
    }
}
