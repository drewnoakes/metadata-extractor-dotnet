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
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Properties;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Xmp
{
    /// <summary>Extracts XMP data from a JPEG header segment.</summary>
    /// <remarks>
    /// Extracts XMP data from a JPEG header segment.
    /// <p>
    /// The extraction is done with Adobe's XmpCore-Library (XMP-Toolkit)
    /// Copyright (c) 1999 - 2007, Adobe Systems Incorporated All rights reserved.
    /// </remarks>
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class XmpReader : JpegSegmentMetadataReader
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

        [NotNull]
        public const string XmpJpegPreamble = "http://ns.adobe.com/xap/1.0/\x0";

        //    @NotNull
        //    private static final String SCHEMA_DUBLIN_CORE_SPECIFIC_PROPERTIES = "http://purl.org/dc/elements/1.1/";
        [NotNull]
        public virtual Iterable<JpegSegmentType> GetSegmentTypes()
        {
            return Arrays.AsList(JpegSegmentType.App1).AsIterable();
        }

        /// <summary>Version specifically for dealing with XMP found in JPEG segments.</summary>
        /// <remarks>
        /// Version specifically for dealing with XMP found in JPEG segments. This form of XMP has a peculiar preamble, which
        /// must be removed before parsing the XML.
        /// </remarks>
        /// <param name="segments">The byte array from which the metadata should be extracted.</param>
        /// <param name="metadata">
        /// The
        /// <see cref="Com.Drew.Metadata.Metadata"/>
        /// object into which extracted values should be merged.
        /// </param>
        /// <param name="segmentType">
        /// The
        /// <see cref="Com.Drew.Imaging.Jpeg.JpegSegmentType"/>
        /// being read.
        /// </param>
        public virtual void ReadJpegSegments([NotNull] Iterable<sbyte[]> segments, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] JpegSegmentType segmentType)
        {
            foreach (sbyte[] segmentBytes in segments)
            {
                // XMP in a JPEG file has an identifying preamble which is not valid XML
                int preambleLength = XmpJpegPreamble.Length;
                if (segmentBytes.Length < preambleLength || !Sharpen.Runtime.EqualsIgnoreCase(XmpJpegPreamble, Sharpen.Runtime.GetStringForBytes(segmentBytes, 0, preambleLength)))
                {
                    continue;
                }
                sbyte[] xmlBytes = new sbyte[segmentBytes.Length - preambleLength];
                System.Array.Copy(segmentBytes, preambleLength, xmlBytes, 0, xmlBytes.Length);
                Extract(xmlBytes, metadata);
            }
        }

        /// <summary>
        /// Performs the XMP data extraction, adding found values to the specified instance of
        /// <see cref="Com.Drew.Metadata.Metadata"/>
        /// .
        /// <p>
        /// The extraction is done with Adobe's XMPCore library.
        /// </summary>
        public virtual void Extract([NotNull] sbyte[] xmpBytes, [NotNull] Com.Drew.Metadata.Metadata metadata)
        {
            XmpDirectory directory = new XmpDirectory();
            try
            {
                XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(xmpBytes);
                ProcessXmpTags(directory, xmpMeta);
            }
            catch (XMPException e)
            {
                directory.AddError("Error processing XMP data: " + e.Message);
            }
            if (!directory.IsEmpty())
            {
                metadata.AddDirectory(directory);
            }
        }

        /// <summary>
        /// Performs the XMP data extraction, adding found values to the specified instance of
        /// <see cref="Com.Drew.Metadata.Metadata"/>
        /// .
        /// <p>
        /// The extraction is done with Adobe's XMPCore library.
        /// </summary>
        public virtual void Extract([NotNull] string xmpString, [NotNull] Com.Drew.Metadata.Metadata metadata)
        {
            XmpDirectory directory = new XmpDirectory();
            try
            {
                XMPMeta xmpMeta = XMPMetaFactory.ParseFromString(xmpString);
                ProcessXmpTags(directory, xmpMeta);
            }
            catch (XMPException e)
            {
                directory.AddError("Error processing XMP data: " + e.Message);
            }
            if (!directory.IsEmpty())
            {
                metadata.AddDirectory(directory);
            }
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        private static void ProcessXmpTags(XmpDirectory directory, XMPMeta xmpMeta)
        {
            // store the XMPMeta object on the directory in case others wish to use it
            directory.SetXMPMeta(xmpMeta);
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
            ProcessXmpDateTag(xmpMeta, directory, XmpDirectory.TagDatetimeOriginal);
            ProcessXmpDateTag(xmpMeta, directory, XmpDirectory.TagDatetimeDigitized);
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
            for (XMPIterator iterator = xmpMeta.Iterator(); iterator.HasNext(); )
            {
                XMPPropertyInfo propInfo = (XMPPropertyInfo)iterator.Next();
                string path = propInfo.GetPath();
                string value = propInfo.GetValue();
                if (path != null && value != null)
                {
                    directory.AddProperty(path, value);
                }
            }
        }

        /// <summary>Reads an property value with given namespace URI and property name.</summary>
        /// <remarks>Reads an property value with given namespace URI and property name. Add property value to directory if exists</remarks>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        private static void ProcessXmpTag([NotNull] XMPMeta meta, [NotNull] XmpDirectory directory, int tagType, int formatCode)
        {
            string schemaNS = XmpDirectory._tagSchemaMap.Get(tagType);
            string propName = XmpDirectory._tagPropNameMap.Get(tagType);
            string property = meta.GetPropertyString(schemaNS, propName);
            if (property == null)
            {
                return;
            }
            switch (formatCode)
            {
                case FmtRational:
                {
                    string[] rationalParts = property.Split("/", 2);
                    if (rationalParts.Length == 2)
                    {
                        try
                        {
                            Rational rational = new Rational((long)float.Parse(rationalParts[0]), (long)float.Parse(rationalParts[1]));
                            directory.SetRational(tagType, rational);
                        }
                        catch (FormatException)
                        {
                            directory.AddError(Sharpen.Extensions.StringFormat("Unable to parse XMP property %s as a Rational.", propName));
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
                        directory.SetInt(tagType, (int)Sharpen.Extensions.ValueOf(property));
                    }
                    catch (FormatException)
                    {
                        directory.AddError(Sharpen.Extensions.StringFormat("Unable to parse XMP property %s as an int.", propName));
                    }
                    break;
                }

                case FmtDouble:
                {
                    try
                    {
                        directory.SetDouble(tagType, (double)double.Parse(property));
                    }
                    catch (FormatException)
                    {
                        directory.AddError(Sharpen.Extensions.StringFormat("Unable to parse XMP property %s as an double.", propName));
                    }
                    break;
                }

                case FmtString:
                {
                    directory.SetString(tagType, property);
                    break;
                }

                case FmtStringArray:
                {
                    //XMP iterators are 1-based
                    int count = meta.CountArrayItems(schemaNS, propName);
                    string[] array = new string[count];
                    for (int i = 1; i <= count; ++i)
                    {
                        array[i - 1] = meta.GetArrayItem(schemaNS, propName, i).GetValue();
                    }
                    directory.SetStringArray(tagType, array);
                    break;
                }

                default:
                {
                    directory.AddError(Sharpen.Extensions.StringFormat("Unknown format code %d for tag %d", formatCode, tagType));
                    break;
                }
            }
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        private static void ProcessXmpDateTag([NotNull] XMPMeta meta, [NotNull] XmpDirectory directory, int tagType)
        {
            string schemaNS = XmpDirectory._tagSchemaMap.Get(tagType);
            string propName = XmpDirectory._tagPropNameMap.Get(tagType);
            Sharpen.Calendar cal = meta.GetPropertyCalendar(schemaNS, propName);
            if (cal != null)
            {
                directory.SetDate(tagType, cal.GetTime());
            }
        }
    }
}
