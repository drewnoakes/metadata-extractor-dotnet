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
using System.Collections.Generic;
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Impl;
using Com.Adobe.Xmp.Options;
using Com.Adobe.Xmp.Properties;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Xmp
{
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpDirectory : Directory
    {
        public const int TagXmpValueCount = unchecked(0xFFFF);

        public const int TagMake = unchecked(0x0001);

        public const int TagModel = unchecked(0x0002);

        public const int TagExposureTime = unchecked(0x0003);

        public const int TagShutterSpeed = unchecked(0x0004);

        public const int TagFNumber = unchecked(0x0005);

        public const int TagLensInfo = unchecked(0x0006);

        public const int TagLens = unchecked(0x0007);

        public const int TagCameraSerialNumber = unchecked(0x0008);

        public const int TagFirmware = unchecked(0x0009);

        public const int TagFocalLength = unchecked(0x000a);

        public const int TagApertureValue = unchecked(0x000b);

        public const int TagExposureProgram = unchecked(0x000c);

        public const int TagDatetimeOriginal = unchecked(0x000d);

        public const int TagDatetimeDigitized = unchecked(0x000e);

        /// <summary>A value from 0 to 5, or -1 if the image is rejected.</summary>
        public const int TagRating = unchecked(0x1001);

        /// <summary>Generally a color value Blue, Red, Green, Yellow, Purple</summary>
        public const int TagLabel = unchecked(0x2000);

        /// <summary>Keywords</summary>
        public static int TagSubject = unchecked(0x2001);

        [NotNull]
        private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        [NotNull]
        internal static readonly Dictionary<int?, string> TagSchemaMap = new Dictionary<int?, string>();

        [NotNull]
        internal static readonly Dictionary<int?, string> TagPropNameMap = new Dictionary<int?, string>();

        [NotNull]
        private readonly IDictionary<string, string> _propertyValueByPath = new Dictionary<string, string>();

        static XmpDirectory()
        {
            // These are some Tags, belonging to xmp-data-tags
            // The numeration is more like enums. The real xmp-tags are strings,
            // so we do some kind of mapping here...
            // dublin core properties
            // this requires further research
            // public static int TAG_TITLE = 0x100;
            // public static int TAG_DATE = 0x1002;
            // public static int TAG_TYPE = 0x1003;
            // public static int TAG_DESCRIPTION = 0x1004;
            // public static int TAG_RELATION = 0x1005;
            // public static int TAG_COVERAGE = 0x1006;
            // public static int TAG_CREATOR = 0x1007;
            // public static int TAG_PUBLISHER = 0x1008;
            // public static int TAG_CONTRIBUTOR = 0x1009;
            // public static int TAG_RIGHTS = 0x100A;
            // public static int TAG_FORMAT = 0x100B;
            // public static int TAG_IDENTIFIER = 0x100C;
            // public static int TAG_LANGUAGE = 0x100D;
            // public static int TAG_AUDIENCE = 0x100E;
            // public static int TAG_PROVENANCE = 0x100F;
            // public static int TAG_RIGHTS_HOLDER = 0x1010;
            // public static int TAG_INSTRUCTIONAL_METHOD = 0x1011;
            // public static int TAG_ACCRUAL_METHOD = 0x1012;
            // public static int TAG_ACCRUAL_PERIODICITY = 0x1013;
            // public static int TAG_ACCRUAL_POLICY = 0x1014;
            TagNameMap[TagXmpValueCount] = "XMP Value Count";
            TagNameMap[TagMake] = "Make";
            TagNameMap[TagModel] = "Model";
            TagNameMap[TagExposureTime] = "Exposure Time";
            TagNameMap[TagShutterSpeed] = "Shutter Speed Value";
            TagNameMap[TagFNumber] = "F-Number";
            TagNameMap[TagLensInfo] = "Lens Information";
            TagNameMap[TagLens] = "Lens";
            TagNameMap[TagCameraSerialNumber] = "Serial Number";
            TagNameMap[TagFirmware] = "Firmware";
            TagNameMap[TagFocalLength] = "Focal Length";
            TagNameMap[TagApertureValue] = "Aperture Value";
            TagNameMap[TagExposureProgram] = "Exposure Program";
            TagNameMap[TagDatetimeOriginal] = "Date/Time Original";
            TagNameMap[TagDatetimeDigitized] = "Date/Time Digitized";
            TagNameMap[TagRating] = "Rating";
            TagNameMap[TagLabel] = "Label";
            // this requires further research
            // _tagNameMap.put(TAG_TITLE, "Title");
            TagNameMap[TagSubject] = "Subject";
            // _tagNameMap.put(TAG_DATE, "Date");
            // _tagNameMap.put(TAG_TYPE, "Type");
            // _tagNameMap.put(TAG_DESCRIPTION, "Description");
            // _tagNameMap.put(TAG_RELATION, "Relation");
            // _tagNameMap.put(TAG_COVERAGE, "Coverage");
            // _tagNameMap.put(TAG_CREATOR, "Creator");
            // _tagNameMap.put(TAG_PUBLISHER, "Publisher");
            // _tagNameMap.put(TAG_CONTRIBUTOR, "Contributor");
            // _tagNameMap.put(TAG_RIGHTS, "Rights");
            // _tagNameMap.put(TAG_FORMAT, "Format");
            // _tagNameMap.put(TAG_IDENTIFIER, "Identifier");
            // _tagNameMap.put(TAG_LANGUAGE, "Language");
            // _tagNameMap.put(TAG_AUDIENCE, "Audience");
            // _tagNameMap.put(TAG_PROVENANCE, "Provenance");
            // _tagNameMap.put(TAG_RIGHTS_HOLDER, "Rights Holder");
            // _tagNameMap.put(TAG_INSTRUCTIONAL_METHOD, "Instructional Method");
            // _tagNameMap.put(TAG_ACCRUAL_METHOD, "Accrual Method");
            // _tagNameMap.put(TAG_ACCRUAL_PERIODICITY, "Accrual Periodicity");
            // _tagNameMap.put(TAG_ACCRUAL_POLICY, "Accrual Policy");
            TagPropNameMap[TagMake] = "tiff:Make";
            TagPropNameMap[TagModel] = "tiff:Model";
            TagPropNameMap[TagExposureTime] = "exif:ExposureTime";
            TagPropNameMap[TagShutterSpeed] = "exif:ShutterSpeedValue";
            TagPropNameMap[TagFNumber] = "exif:FNumber";
            TagPropNameMap[TagLensInfo] = "aux:LensInfo";
            TagPropNameMap[TagLens] = "aux:Lens";
            TagPropNameMap[TagCameraSerialNumber] = "aux:SerialNumber";
            TagPropNameMap[TagFirmware] = "aux:Firmware";
            TagPropNameMap[TagFocalLength] = "exif:FocalLength";
            TagPropNameMap[TagApertureValue] = "exif:ApertureValue";
            TagPropNameMap[TagExposureProgram] = "exif:ExposureProgram";
            TagPropNameMap[TagDatetimeOriginal] = "exif:DateTimeOriginal";
            TagPropNameMap[TagDatetimeDigitized] = "exif:DateTimeDigitized";
            TagPropNameMap[TagRating] = "xmp:Rating";
            TagPropNameMap[TagLabel] = "xmp:Label";
            // this requires further research
            // _tagPropNameMap.put(TAG_TITLE, "dc:title");
            TagPropNameMap[TagSubject] = "dc:subject";
            // _tagPropNameMap.put(TAG_DATE, "dc:date");
            // _tagPropNameMap.put(TAG_TYPE, "dc:type");
            // _tagPropNameMap.put(TAG_DESCRIPTION, "dc:description");
            // _tagPropNameMap.put(TAG_RELATION, "dc:relation");
            // _tagPropNameMap.put(TAG_COVERAGE, "dc:coverage");
            // _tagPropNameMap.put(TAG_CREATOR, "dc:creator");
            // _tagPropNameMap.put(TAG_PUBLISHER, "dc:publisher");
            // _tagPropNameMap.put(TAG_CONTRIBUTOR, "dc:contributor");
            // _tagPropNameMap.put(TAG_RIGHTS, "dc:rights");
            // _tagPropNameMap.put(TAG_FORMAT, "dc:format");
            // _tagPropNameMap.put(TAG_IDENTIFIER, "dc:identifier");
            // _tagPropNameMap.put(TAG_LANGUAGE, "dc:language");
            // _tagPropNameMap.put(TAG_AUDIENCE, "dc:audience");
            // _tagPropNameMap.put(TAG_PROVENANCE, "dc:provenance");
            // _tagPropNameMap.put(TAG_RIGHTS_HOLDER, "dc:rightsHolder");
            // _tagPropNameMap.put(TAG_INSTRUCTIONAL_METHOD, "dc:instructionalMethod");
            // _tagPropNameMap.put(TAG_ACCRUAL_METHOD, "dc:accrualMethod");
            // _tagPropNameMap.put(TAG_ACCRUAL_PERIODICITY, "dc:accrualPeriodicity");
            // _tagPropNameMap.put(TAG_ACCRUAL_POLICY, "dc:accrualPolicy");
            TagSchemaMap[TagMake] = Schema.ExifTiffProperties;
            TagSchemaMap[TagModel] = Schema.ExifTiffProperties;
            TagSchemaMap[TagExposureTime] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagShutterSpeed] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagFNumber] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagLensInfo] = Schema.ExifAdditionalProperties;
            TagSchemaMap[TagLens] = Schema.ExifAdditionalProperties;
            TagSchemaMap[TagCameraSerialNumber] = Schema.ExifAdditionalProperties;
            TagSchemaMap[TagFirmware] = Schema.ExifAdditionalProperties;
            TagSchemaMap[TagFocalLength] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagApertureValue] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagExposureProgram] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagDatetimeOriginal] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagDatetimeDigitized] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagRating] = Schema.XmpProperties;
            TagSchemaMap[TagLabel] = Schema.XmpProperties;
            // this requires further research
            // _tagNameMap.put(TAG_TITLE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            TagSchemaMap[TagSubject] = Schema.DublinCoreSpecificProperties;
        }

        [CanBeNull]
        private IXmpMeta _xmpMeta;

        public XmpDirectory()
        {
            // _tagNameMap.put(TAG_DATE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_TYPE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_DESCRIPTION, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_RELATION, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_COVERAGE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_CREATOR, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_PUBLISHER, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_CONTRIBUTOR, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_RIGHTS, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_FORMAT, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_IDENTIFIER, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_LANGUAGE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_AUDIENCE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_PROVENANCE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_RIGHTS_HOLDER, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_INSTRUCTIONAL_METHOD, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_ACCRUAL_METHOD, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_ACCRUAL_PERIODICITY, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            // _tagNameMap.put(TAG_ACCRUAL_POLICY, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            SetDescriptor(new XmpDescriptor(this));
        }

        public override string GetName()
        {
            return "Xmp";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        internal void AddProperty([NotNull] string path, [NotNull] string value)
        {
            _propertyValueByPath[path] = value;
        }

        /// <summary>Gets a map of all XMP properties in this directory, not just the known ones.</summary>
        /// <remarks>
        /// Gets a map of all XMP properties in this directory, not just the known ones.
        /// <para />
        /// This is required because XMP properties are represented as strings, whereas the rest of this library
        /// uses integers for keys.
        /// </remarks>
        [NotNull]
        public IDictionary<string, string> GetXmpProperties()
        {
            return Collections.UnmodifiableMap(_propertyValueByPath);
        }

        public void SetXmpMeta([NotNull] IXmpMeta xmpMeta)
        {
            _xmpMeta = xmpMeta;
            try
            {
                var valueCount = 0;
                for (IIterator i = _xmpMeta.Iterator(); i.HasNext(); )
                {
                    var prop = (IXmpPropertyInfo)i.Next();
                    if (prop.Path != null)
                    {
                        //System.out.printf("%s = %s\n", prop.getPath(), prop.getValue());
                        valueCount++;
                    }
                }
                SetInt(TagXmpValueCount, valueCount);
            }
            catch (XmpException)
            {
            }
        }

        /// <summary>Gets the XMPMeta object used to populate this directory.</summary>
        /// <remarks>
        /// Gets the XMPMeta object used to populate this directory. It can be used for more XMP-oriented operations. If one does not exist it will be
        /// created.
        /// </remarks>
        [CanBeNull]
        public IXmpMeta GetXmpMeta()
        {
            if (_xmpMeta == null)
            {
                _xmpMeta = new XmpMeta();
            }
            return _xmpMeta;
        }

        // TODO: Might consider returning a boolean in the super to allow for exception handling. Failing to set is sufficient for now.
        // TODO: update[Type] avoids rewriting the whole _xmpMeta on processXmpTags(),
        // but with sets exposed this is still less than ideal...
        // At the very least document this carefully!
        public void UpdateInt(int tagType, int value)
        {
            SetInt(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyInteger(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), value);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateIntArray(int tagType, int[] ints)
        {
            SetIntArray(tagType, ints);
            try
            {
                var schemaNs = TagSchemaMap.GetOrNull(tagType);
                var propName = TagPropNameMap.GetOrNull(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                var po = new PropertyOptions();
                po.IsArray = true;
                foreach (var item in ints)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item.ToString(), null);
                }
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateFloat(int tagType, float value)
        {
            SetFloat(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyDouble(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), value);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateFloatArray(int tagType, float[] floats)
        {
            SetFloatArray(tagType, floats);
            try
            {
                var schemaNs = TagSchemaMap.GetOrNull(tagType);
                var propName = TagPropNameMap.GetOrNull(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                var po = new PropertyOptions();
                po.IsArray = true;
                foreach (var item in floats)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item.ToString(), null);
                }
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateDouble(int tagType, double value)
        {
            SetDouble(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyDouble(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), value);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateDoubleArray(int tagType, double[] doubles)
        {
            SetDoubleArray(tagType, doubles);
            try
            {
                var schemaNs = TagSchemaMap.GetOrNull(tagType);
                var propName = TagPropNameMap.GetOrNull(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                var po = new PropertyOptions();
                po.IsArray = true;
                foreach (var item in doubles)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item.ToString(), null);
                }
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateString(int tagType, string value)
        {
            SetString(tagType, value);
            try
            {
                GetXmpMeta().SetProperty(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), value);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void DeleteProperty(int tagType)
        {
            GetXmpMeta().DeleteProperty(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType));
        }

        public void UpdateStringArray(int tagType, string[] strings)
        {
            SetStringArray(tagType, strings);
            try
            {
                var schemaNs = TagSchemaMap.GetOrNull(tagType);
                var propName = TagPropNameMap.GetOrNull(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                var po = new PropertyOptions();
                po.IsArray = true;
                foreach (var item in strings)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item, null);
                }
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateBoolean(int tagType, bool value)
        {
            SetBoolean(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyBoolean(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), value);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateLong(int tagType, long value)
        {
            SetLong(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyLong(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), value);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }

        public void UpdateDate(int tagType, DateTime value)
        {
            SetDate(tagType, value);
            IXmpDateTime date = new XmpDateTime(value, TimeZoneInfo.Local);
            try
            {
                GetXmpMeta().SetPropertyDate(TagSchemaMap.GetOrNull(tagType), TagPropNameMap.GetOrNull(tagType), date);
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
            }
        }
        // TODO: Ignoring rationals for now, not sure their relevance to XMP (rational/floating storage)
        // @Override
        // public void setRational(int tagType, Rational rational)
        // {
        // super.setRational(tagType, rational);
        // }
        //
        // @Override
        // public void setRationalArray(int tagType, Rational[] rationals)
        // {
        // // TODO Auto-generated method stub
        // super.setRationalArray(tagType, rationals);
        // }
        // TODO: Not sure the intention of the byte array, probably store like the other arrays.
        // @Override
        // public void setByteArray(int tagType, byte[] bytes)
        // {
        // // TODO Auto-generated method stub
        // super.setByteArray(tagType, bytes);
        // }
    }
}
