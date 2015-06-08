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
    public class XmpDirectory : Directory
    {
        public const int TagXmpValueCount = unchecked((int)(0xFFFF));

        public const int TagMake = unchecked((int)(0x0001));

        public const int TagModel = unchecked((int)(0x0002));

        public const int TagExposureTime = unchecked((int)(0x0003));

        public const int TagShutterSpeed = unchecked((int)(0x0004));

        public const int TagFNumber = unchecked((int)(0x0005));

        public const int TagLensInfo = unchecked((int)(0x0006));

        public const int TagLens = unchecked((int)(0x0007));

        public const int TagCameraSerialNumber = unchecked((int)(0x0008));

        public const int TagFirmware = unchecked((int)(0x0009));

        public const int TagFocalLength = unchecked((int)(0x000a));

        public const int TagApertureValue = unchecked((int)(0x000b));

        public const int TagExposureProgram = unchecked((int)(0x000c));

        public const int TagDatetimeOriginal = unchecked((int)(0x000d));

        public const int TagDatetimeDigitized = unchecked((int)(0x000e));

        /// <summary>A value from 0 to 5, or -1 if the image is rejected.</summary>
        public const int TagRating = unchecked((int)(0x1001));

        /// <summary>Generally a color value Blue, Red, Green, Yellow, Purple</summary>
        public const int TagLabel = unchecked((int)(0x2000));

        /// <summary>Keywords</summary>
        public static int TagSubject = unchecked((int)(0x2001));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagSchemaMap = new Dictionary<int?, string>();

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagPropNameMap = new Dictionary<int?, string>();

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
            TagNameMap.Put(TagXmpValueCount, "XMP Value Count");
            TagNameMap.Put(TagMake, "Make");
            TagNameMap.Put(TagModel, "Model");
            TagNameMap.Put(TagExposureTime, "Exposure Time");
            TagNameMap.Put(TagShutterSpeed, "Shutter Speed Value");
            TagNameMap.Put(TagFNumber, "F-Number");
            TagNameMap.Put(TagLensInfo, "Lens Information");
            TagNameMap.Put(TagLens, "Lens");
            TagNameMap.Put(TagCameraSerialNumber, "Serial Number");
            TagNameMap.Put(TagFirmware, "Firmware");
            TagNameMap.Put(TagFocalLength, "Focal Length");
            TagNameMap.Put(TagApertureValue, "Aperture Value");
            TagNameMap.Put(TagExposureProgram, "Exposure Program");
            TagNameMap.Put(TagDatetimeOriginal, "Date/Time Original");
            TagNameMap.Put(TagDatetimeDigitized, "Date/Time Digitized");
            TagNameMap.Put(TagRating, "Rating");
            TagNameMap.Put(TagLabel, "Label");
            // this requires further research
            // _tagNameMap.put(TAG_TITLE, "Title");
            TagNameMap.Put(TagSubject, "Subject");
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
            TagPropNameMap.Put(TagMake, "tiff:Make");
            TagPropNameMap.Put(TagModel, "tiff:Model");
            TagPropNameMap.Put(TagExposureTime, "exif:ExposureTime");
            TagPropNameMap.Put(TagShutterSpeed, "exif:ShutterSpeedValue");
            TagPropNameMap.Put(TagFNumber, "exif:FNumber");
            TagPropNameMap.Put(TagLensInfo, "aux:LensInfo");
            TagPropNameMap.Put(TagLens, "aux:Lens");
            TagPropNameMap.Put(TagCameraSerialNumber, "aux:SerialNumber");
            TagPropNameMap.Put(TagFirmware, "aux:Firmware");
            TagPropNameMap.Put(TagFocalLength, "exif:FocalLength");
            TagPropNameMap.Put(TagApertureValue, "exif:ApertureValue");
            TagPropNameMap.Put(TagExposureProgram, "exif:ExposureProgram");
            TagPropNameMap.Put(TagDatetimeOriginal, "exif:DateTimeOriginal");
            TagPropNameMap.Put(TagDatetimeDigitized, "exif:DateTimeDigitized");
            TagPropNameMap.Put(TagRating, "xmp:Rating");
            TagPropNameMap.Put(TagLabel, "xmp:Label");
            // this requires further research
            // _tagPropNameMap.put(TAG_TITLE, "dc:title");
            TagPropNameMap.Put(TagSubject, "dc:subject");
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
            TagSchemaMap.Put(TagMake, Schema.ExifTiffProperties);
            TagSchemaMap.Put(TagModel, Schema.ExifTiffProperties);
            TagSchemaMap.Put(TagExposureTime, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagShutterSpeed, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagFNumber, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagLensInfo, Schema.ExifAdditionalProperties);
            TagSchemaMap.Put(TagLens, Schema.ExifAdditionalProperties);
            TagSchemaMap.Put(TagCameraSerialNumber, Schema.ExifAdditionalProperties);
            TagSchemaMap.Put(TagFirmware, Schema.ExifAdditionalProperties);
            TagSchemaMap.Put(TagFocalLength, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagApertureValue, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagExposureProgram, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagDatetimeOriginal, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagDatetimeDigitized, Schema.ExifSpecificProperties);
            TagSchemaMap.Put(TagRating, Schema.XmpProperties);
            TagSchemaMap.Put(TagLabel, Schema.XmpProperties);
            // this requires further research
            // _tagNameMap.put(TAG_TITLE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            TagSchemaMap.Put(TagSubject, Schema.DublinCoreSpecificProperties);
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
            this.SetDescriptor(new XmpDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Xmp";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        internal virtual void AddProperty([NotNull] string path, [NotNull] string value)
        {
            _propertyValueByPath.Put(path, value);
        }

        /// <summary>Gets a map of all XMP properties in this directory, not just the known ones.</summary>
        /// <remarks>
        /// Gets a map of all XMP properties in this directory, not just the known ones.
        /// <p>
        /// This is required because XMP properties are represented as strings, whereas the rest of this library
        /// uses integers for keys.
        /// </remarks>
        [NotNull]
        public virtual IDictionary<string, string> GetXmpProperties()
        {
            return Collections.UnmodifiableMap(_propertyValueByPath);
        }

        public virtual void SetXmpMeta([NotNull] IXmpMeta xmpMeta)
        {
            _xmpMeta = xmpMeta;
            try
            {
                int valueCount = 0;
                for (IIterator i = _xmpMeta.Iterator(); i.HasNext(); )
                {
                    IXmpPropertyInfo prop = (IXmpPropertyInfo)i.Next();
                    if (prop.GetPath() != null)
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
        public virtual IXmpMeta GetXmpMeta()
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
        public virtual void UpdateInt(int tagType, int value)
        {
            base.SetInt(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyInteger(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), value);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateIntArray(int tagType, int[] ints)
        {
            base.SetIntArray(tagType, ints);
            try
            {
                string schemaNs = TagSchemaMap.Get(tagType);
                string propName = TagPropNameMap.Get(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                PropertyOptions po = new PropertyOptions().SetArray(true);
                foreach (int item in ints)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item.ToString(), null);
                }
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateFloat(int tagType, float value)
        {
            base.SetFloat(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyDouble(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), value);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateFloatArray(int tagType, float[] floats)
        {
            base.SetFloatArray(tagType, floats);
            try
            {
                string schemaNs = TagSchemaMap.Get(tagType);
                string propName = TagPropNameMap.Get(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                PropertyOptions po = new PropertyOptions().SetArray(true);
                foreach (float item in floats)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item.ToString(), null);
                }
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateDouble(int tagType, double value)
        {
            base.SetDouble(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyDouble(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), value);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateDoubleArray(int tagType, double[] doubles)
        {
            base.SetDoubleArray(tagType, doubles);
            try
            {
                string schemaNs = TagSchemaMap.Get(tagType);
                string propName = TagPropNameMap.Get(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                PropertyOptions po = new PropertyOptions().SetArray(true);
                foreach (double item in doubles)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item.ToString(), null);
                }
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateString(int tagType, string value)
        {
            base.SetString(tagType, value);
            try
            {
                GetXmpMeta().SetProperty(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), value);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void DeleteProperty(int tagType)
        {
            GetXmpMeta().DeleteProperty(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType));
        }

        public virtual void UpdateStringArray(int tagType, string[] strings)
        {
            base.SetStringArray(tagType, strings);
            try
            {
                string schemaNs = TagSchemaMap.Get(tagType);
                string propName = TagPropNameMap.Get(tagType);
                GetXmpMeta().DeleteProperty(schemaNs, propName);
                PropertyOptions po = new PropertyOptions().SetArray(true);
                foreach (string item in strings)
                {
                    GetXmpMeta().AppendArrayItem(schemaNs, propName, po, item, null);
                }
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateBoolean(int tagType, bool value)
        {
            base.SetBoolean(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyBoolean(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), value);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateLong(int tagType, long value)
        {
            base.SetLong(tagType, value);
            try
            {
                GetXmpMeta().SetPropertyLong(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), value);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual void UpdateDate(int tagType, DateTime value)
        {
            base.SetDate(tagType, value);
            IXmpDateTime date = new XmpDateTime(value, TimeZoneInfo.Local);
            try
            {
                GetXmpMeta().SetPropertyDate(TagSchemaMap.Get(tagType), TagPropNameMap.Get(tagType), date);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
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
