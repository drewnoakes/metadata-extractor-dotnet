#region License
//
// Copyright 2002-2016 Drew Noakes
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Util;
using MetadataExtractor.Formats.Jpeg;
using XmpCore;

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Extracts XMP data from a JPEG header segment.</summary>
    /// <remarks>
    /// The extraction is done with Adobe's XmpCore-Library (XMP-Toolkit)
    /// Copyright (c) 1999 - 2007, Adobe Systems Incorporated All rights reserved.
    /// </remarks>
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpReader : IJpegSegmentMetadataReader
    {
        private enum FormatType
        {
            String = 1,
            Rational = 2,
            Int = 3,
            Double = 4,
            StringArray = 5
        }

//        private const string SchemaXmpProperties = "http://ns.adobe.com/xap/1.0/";
//        private const string SchemaExifSpecificProperties = "http://ns.adobe.com/exif/1.0/";
//        private const string SchemaExifAdditionalProperties = "http://ns.adobe.com/exif/1.0/aux/";
//        private const string SchemaExifTiffProperties = "http://ns.adobe.com/tiff/1.0/";
//        private const string SchemaDublinCoreSpecificProperties = "http://purl.org/dc/elements/1.1/";

        public const string JpegSegmentPreamble = "http://ns.adobe.com/xap/1.0/\0";
        public const string JpegSegmentPreambleExtension = "http://ns.adobe.com/xmp/extension/\0";

        private static byte[] JpegSegmentPreambleBytes { get; } = Encoding.UTF8.GetBytes(JpegSegmentPreamble);
        private static byte[] JpegSegmentPreambleExtensionBytes { get; } = Encoding.UTF8.GetBytes(JpegSegmentPreambleExtension);

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.App1 };

        public

#if NET35
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Ensure collection materialised (avoiding multiple lazy enumeration)
            segments = segments.ToList();

            var directories = segments
                .Where(IsXmpSegment)
                .Select(segment => Extract(segment.Bytes, JpegSegmentPreambleBytes.Length, segment.Bytes.Length - JpegSegmentPreambleBytes.Length))
                .Cast<Directory>()
                .ToList();

            var extensionGroups = segments.Where(IsExtendedXmpSegment).GroupBy(GetExtendedDataGuid);

            foreach (var extensionGroup in extensionGroups)
            {
                var buffer = new MemoryStream();
                foreach (var segment in extensionGroup)
                {
                    var N = JpegSegmentPreambleExtensionBytes.Length + 32 + 4 + 4;
                    buffer.Write(segment.Bytes, N, segment.Bytes.Length - N);
                }

                buffer.Position = 0;
                var directory = new XmpDirectory();
                var xmpMeta = XmpMetaFactory.Parse(buffer);
                ProcessXmpTags(directory, xmpMeta);
                directories.Add(directory);
            }

            return directories;
        }

        private static string GetExtendedDataGuid(JpegSegment segment) => Encoding.UTF8.GetString(segment.Bytes, JpegSegmentPreambleExtensionBytes.Length, 32);

        private static bool IsXmpSegment(JpegSegment segment) => segment.Bytes.StartsWith(JpegSegmentPreambleBytes);
        private static bool IsExtendedXmpSegment(JpegSegment segment) => segment.Bytes.StartsWith(JpegSegmentPreambleExtensionBytes);

        [NotNull]
        public XmpDirectory Extract([NotNull] byte[] xmpBytes) => Extract(xmpBytes, 0, xmpBytes.Length);

        [NotNull]
        public XmpDirectory Extract([NotNull] byte[] xmpBytes, int offset, int length)
        {
            var directory = new XmpDirectory();
            try
            {
                var xmpMeta = XmpMetaFactory.ParseFromBuffer(xmpBytes, offset, length);
                ProcessXmpTags(directory, xmpMeta);
            }
            catch (XmpException e)
            {
                directory.AddError("Error processing XMP data: " + e.Message);
            }
            return directory;
        }

        private static void ProcessXmpTags([NotNull] XmpDirectory directory, [NotNull] IXmpMeta xmpMeta)
        {
            // store the XMPMeta object on the directory in case others wish to use it
            directory.SetXmpMeta(xmpMeta);

            // extract selected XMP tags
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagLensInfo,           FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagLens,               FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagCameraSerialNumber, FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagFirmware,           FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagMake,               FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagModel,              FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagExposureTime,       FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagExposureProgram,    FormatType.Int);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagApertureValue,      FormatType.Rational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagFNumber,            FormatType.Rational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagFocalLength,        FormatType.Rational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagShutterSpeed,       FormatType.Rational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagDateTimeOriginal,   FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagDateTimeDigitized,  FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagBaseUrl,            FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagCreateDate,         FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagCreatorTool,        FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagIdentifier,         FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagMetadataDate,       FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagModifyDate,         FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagNickname,           FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagRating,             FormatType.Double);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagLabel,              FormatType.String);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagSubject,            FormatType.StringArray);

            // this requires further research
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:title", XmpDirectory.TAG_TITLE, FMT_STRING);
            // ProcessXmpDateTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:date", XmpDirectory.TAG_DATE);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:type", XmpDirectory.TAG_TYPE, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:description", XmpDirectory.TAG_DESCRIPTION, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:relation", XmpDirectory.TAG_RELATION, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:coverage", XmpDirectory.TAG_COVERAGE, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:creator", XmpDirectory.TAG_CREATOR, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:publisher", XmpDirectory.TAG_PUBLISHER, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:contributor", XmpDirectory.TAG_CONTRIBUTOR, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:rights", XmpDirectory.TAG_RIGHTS, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:format", XmpDirectory.TAG_FORMAT, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:identifier", XmpDirectory.TAG_IDENTIFIER, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:language", XmpDirectory.TAG_LANGUAGE, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:audience", XmpDirectory.TAG_AUDIENCE, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:provenance", XmpDirectory.TAG_PROVENANCE, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:rightsHolder", XmpDirectory.TAG_RIGHTS_HOLDER, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:instructionalMethod", XmpDirectory.TAG_INSTRUCTIONAL_METHOD, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:accrualMethod", XmpDirectory.TAG_ACCRUAL_METHOD, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:accrualPeriodicity", XmpDirectory.TAG_ACCRUAL_PERIODICITY, FMT_STRING);
            // ProcessXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:accrualPolicy", XmpDirectory.TAG_ACCRUAL_POLICY, FMT_STRING);
        }

        private static void ProcessXmpTag([NotNull] IXmpMeta meta, [NotNull] XmpDirectory directory, int tagType, FormatType formatCode)
        {
            string schemaNs;
            string propName;
            if (!XmpDirectory.TagSchemaMap.TryGetValue(tagType, out schemaNs) || !XmpDirectory.TagPropNameMap.TryGetValue(tagType, out propName))
                return;

            var property = meta.GetPropertyString(schemaNs, propName);
            if (property == null)
                return;

            switch (formatCode)
            {
                case FormatType.Rational:
                {
                    // TODO introduce Rational.TryParse
                    var rationalParts = property.Split('/').Take(2).ToArray();
                    if (rationalParts.Length == 2)
                    {
                        // TODO should this really be parsed as float?
                        float numerator;
                        float denominator;
                        if (float.TryParse(rationalParts[0], out numerator) && float.TryParse(rationalParts[1], out denominator))
                            directory.Set(tagType, new Rational((long)numerator, (long)denominator));
                        else
                            directory.AddError($"Unable to parse XMP property {propName} as a Rational.");
                    }
                    else
                    {
                        directory.AddError($"Error in rational format for tag {tagType}");
                    }
                    break;
                }
                case FormatType.Int:
                {
                    int value;
                    if (int.TryParse(property, out value))
                        directory.Set(tagType, value);
                    else
                        directory.AddError($"Unable to parse XMP property {propName} as an int.");
                    break;
                }
                case FormatType.Double:
                {
                    double value;
                    if (double.TryParse(property, out value))
                        directory.Set(tagType, value);
                    else
                        directory.AddError($"Unable to parse XMP property {propName} as a double.");
                    break;
                }
                case FormatType.String:
                {
                    directory.Set(tagType, property);
                    break;
                }
                case FormatType.StringArray:
                {
                    // XMP iterators are 1-based
                    var count = meta.CountArrayItems(schemaNs, propName);
                    var array = new string[count];
                    for (var i = 1; i <= count; i++)
                        array[i - 1] = meta.GetArrayItem(schemaNs, propName, i).Value;
                    directory.Set(tagType, array);
                    break;
                }
                default:
                {
                    directory.AddError($"Unknown format code {formatCode} for tag {tagType}");
                    break;
                }
            }
        }
    }
}
