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

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
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
        private const int FmtString = 1;
        private const int FmtRational = 2;
        private const int FmtInt = 3;
        private const int FmtDouble = 4;
        private const int FmtStringArray = 5;

        /// <summary>XMP tag namespace.</summary>
        /// <remarks>
        /// XMP tag namespace.
        /// TODO the older "xap", "xapBJ", "xapMM" or "xapRights" namespace prefixes should be translated to the newer "xmp", "xmpBJ", "xmpMM" and "xmpRights" prefixes for use in family 1 group names
        /// </remarks>
        [NotNull]
        private const string SchemaXmpProperties = "http://ns.adobe.com/xap/1.0/";

        [NotNull]
        private const string SchemaExifSpecificProperties = "http://ns.adobe.com/exif/1.0/";

        [NotNull]
        private const string SchemaExifAdditionalProperties = "http://ns.adobe.com/exif/1.0/aux/";

        [NotNull]
        private const string SchemaExifTiffProperties = "http://ns.adobe.com/tiff/1.0/";

//      [NotNull]
//      private const string SchemaDublinCoreSpecificProperties = "http://purl.org/dc/elements/1.1/";

        [NotNull]
        public const string XmpJpegPreamble = "http://ns.adobe.com/xap/1.0/\x0";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.App1;
        }

        public IReadOnlyList<Directory> ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
        {
            var directories = new List<Directory>();

            foreach (var segmentBytes in segments)
            {
                // XMP in a JPEG file has an identifying preamble which is not valid XML
                var preambleLength = XmpJpegPreamble.Length;
                if (segmentBytes.Length >= preambleLength && XmpJpegPreamble.Equals(Encoding.UTF8.GetString(segmentBytes, 0, preambleLength), StringComparison.OrdinalIgnoreCase))
                {
                    var xmlBytes = new byte[segmentBytes.Length - preambleLength];
                    Array.Copy(segmentBytes, preambleLength, xmlBytes, 0, xmlBytes.Length);
                    directories.Add(Extract(xmlBytes));
                }
            }

            return directories;
        }

        /// <summary>
        /// Performs the XMP data extraction.
        /// <para />
        /// The extraction is done with Adobe's XMPCore library.
        /// </summary>
        public XmpDirectory Extract([NotNull] byte[] xmpBytes)
        {
            var directory = new XmpDirectory();
            try
            {
                var xmpMeta = XmpMetaFactory.ParseFromBuffer(xmpBytes);
                ProcessXmpTags(directory, xmpMeta);
            }
            catch (XmpException e)
            {
                directory.AddError("Error processing XMP data: " + e.Message);
            }
            return directory;
        }

        /// <summary>
        /// Performs the XMP data extraction.
        /// <para />
        /// The extraction is done with Adobe's XMPCore library.
        /// </summary>
        public XmpDirectory Extract([NotNull] string xmpString) => Extract(Encoding.UTF8.GetBytes(xmpString));

        /// <exception cref="XmpException"/>
        private static void ProcessXmpTags(XmpDirectory directory, IXmpMeta xmpMeta)
        {
            // store the XMPMeta object on the directory in case others wish to use it
            directory.SetXmpMeta(xmpMeta);
            // read all the tags and send them to the directory
            // I've added some popular tags, feel free to add more tags
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagLensInfo, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagLens, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagCameraSerialNumber, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagFirmware, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagMake, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagModel, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagExposureTime, FmtString);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagExposureProgram, FmtInt);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagApertureValue, FmtRational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagFNumber, FmtRational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagFocalLength, FmtRational);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagShutterSpeed, FmtRational);
            ProcessXmpDateTag(xmpMeta, directory, XmpDirectory.TagDateTimeOriginal);
            ProcessXmpDateTag(xmpMeta, directory, XmpDirectory.TagDateTimeDigitized);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagRating, FmtDouble);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagLabel, FmtString);
            // this requires further research
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:title", XmpDirectory.TAG_TITLE, FMT_STRING);
            ProcessXmpTag(xmpMeta, directory, XmpDirectory.TagSubject, FmtStringArray);
            // processXmpDateTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:date", XmpDirectory.TAG_DATE);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:type", XmpDirectory.TAG_TYPE, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:description", XmpDirectory.TAG_DESCRIPTION, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:relation", XmpDirectory.TAG_RELATION, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:coverage", XmpDirectory.TAG_COVERAGE, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:creator", XmpDirectory.TAG_CREATOR, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:publisher", XmpDirectory.TAG_PUBLISHER, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:contributor", XmpDirectory.TAG_CONTRIBUTOR, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:rights", XmpDirectory.TAG_RIGHTS, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:format", XmpDirectory.TAG_FORMAT, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:identifier", XmpDirectory.TAG_IDENTIFIER, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:language", XmpDirectory.TAG_LANGUAGE, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:audience", XmpDirectory.TAG_AUDIENCE, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:provenance", XmpDirectory.TAG_PROVENANCE, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:rightsHolder", XmpDirectory.TAG_RIGHTS_HOLDER, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:instructionalMethod", XmpDirectory.TAG_INSTRUCTIONAL_METHOD,
            // FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:accrualMethod", XmpDirectory.TAG_ACCRUAL_METHOD, FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:accrualPeriodicity", XmpDirectory.TAG_ACCRUAL_PERIODICITY,
            // FMT_STRING);
            // processXmpTag(xmpMeta, directory, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES, "dc:accrualPolicy", XmpDirectory.TAG_ACCRUAL_POLICY, FMT_STRING);
            foreach (var prop in xmpMeta.Properties)
            {
                var path = prop.Path;
                var value = prop.Value;
                if (path != null && value != null)
                    directory.AddProperty(path, value);
            }
        }

        /// <summary>Reads an property value with given namespace URI and property name.</summary>
        /// <remarks>Reads an property value with given namespace URI and property name. Add property value to directory if exists</remarks>
        /// <exception cref="XmpException"/>
        private static void ProcessXmpTag([NotNull] IXmpMeta meta, [NotNull] XmpDirectory directory, int tagType, int formatCode)
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
                case FmtRational:
                {
                    var rationalParts = property.Split(new[]{'/'}, 2);
                    if (rationalParts.Length == 2)
                    {
                        try
                        {
                            var rational = new Rational((long)float.Parse(rationalParts[0]), (long)float.Parse(rationalParts[1]));
                            directory.Set(tagType, rational);
                        }
                        catch (FormatException)
                        {
                            directory.AddError($"Unable to parse XMP property {propName} as a Rational.");
                        }
                    }
                    else
                    {
                        directory.AddError("Error in rational format for tag " + tagType);
                    }
                    break;
                }

                case FmtInt:
                {
                    try
                    {
                        directory.Set(tagType, int.Parse(property));
                    }
                    catch (FormatException)
                    {
                        directory.AddError($"Unable to parse XMP property {propName} as an int.");
                    }
                    break;
                }

                case FmtDouble:
                {
                    try
                    {
                        directory.Set(tagType, double.Parse(property));
                    }
                    catch (FormatException)
                    {
                        directory.AddError($"Unable to parse XMP property {propName} as an double.");
                    }
                    break;
                }

                case FmtString:
                {
                    directory.Set(tagType, property);
                    break;
                }

                case FmtStringArray:
                {
                    //XMP iterators are 1-based
                    var count = meta.CountArrayItems(schemaNs, propName);
                    var array = new string[count];
                    for (var i = 1; i <= count; ++i)
                    {
                        array[i - 1] = meta.GetArrayItem(schemaNs, propName, i).Value;
                    }
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

        /// <exception cref="XmpException"/>
        private static void ProcessXmpDateTag([NotNull] IXmpMeta meta, [NotNull] XmpDirectory directory, int tagType)
        {
            string schemaNs;
            string propName;
            if (!XmpDirectory.TagSchemaMap.TryGetValue(tagType, out schemaNs) || !XmpDirectory.TagPropNameMap.TryGetValue(tagType, out propName))
                return;

            var cal = meta.GetPropertyCalendar(schemaNs, propName);

            if (cal != null)
                directory.Set(tagType, cal.GetTime());
        }
    }
}
