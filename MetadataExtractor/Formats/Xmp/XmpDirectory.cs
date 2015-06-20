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

using System.Collections.Generic;
using System.Linq;
using XmpCore;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Xmp
{
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpDirectory : Directory
    {
        public const int TagXmpValueCount = 0xFFFF;
        public const int TagMake = 0x0001;
        public const int TagModel = 0x0002;
        public const int TagExposureTime = 0x0003;
        public const int TagShutterSpeed = 0x0004;
        public const int TagFNumber = 0x0005;
        public const int TagLensInfo = 0x0006;
        public const int TagLens = 0x0007;
        public const int TagCameraSerialNumber = 0x0008;
        public const int TagFirmware = 0x0009;
        public const int TagFocalLength = 0x000a;
        public const int TagApertureValue = 0x000b;
        public const int TagExposureProgram = 0x000c;
        public const int TagDateTimeOriginal = 0x000d;
        public const int TagDateTimeDigitized = 0x000e;

        /// <summary>A value from 0 to 5, or -1 if the image is rejected.</summary>
        public const int TagRating = 0x1001;

        /// <summary>Generally a color value Blue, Red, Green, Yellow, Purple</summary>
        public const int TagLabel = 0x2000;

        /// <summary>Keywords</summary>
        public static int TagSubject = 0x2001;

        [NotNull]
        private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        [NotNull]
        internal static readonly Dictionary<int?, string> TagSchemaMap = new Dictionary<int?, string>();

        [NotNull]
        internal static readonly Dictionary<int?, string> TagPropNameMap = new Dictionary<int?, string>();

        [NotNull]
        private readonly Dictionary<string, string> _propertyValueByPath = new Dictionary<string, string>();

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
            TagNameMap[TagDateTimeOriginal] = "Date/Time Original";
            TagNameMap[TagDateTimeDigitized] = "Date/Time Digitized";
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
            TagPropNameMap[TagDateTimeOriginal] = "exif:DateTimeOriginal";
            TagPropNameMap[TagDateTimeDigitized] = "exif:DateTimeDigitized";
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
            TagSchemaMap[TagDateTimeOriginal] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagDateTimeDigitized] = Schema.ExifSpecificProperties;
            TagSchemaMap[TagRating] = Schema.XmpProperties;
            TagSchemaMap[TagLabel] = Schema.XmpProperties;
            // this requires further research
            // _tagNameMap.put(TAG_TITLE, Schema.DUBLIN_CORE_SPECIFIC_PROPERTIES);
            TagSchemaMap[TagSubject] = Schema.DublinCoreSpecificProperties;
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
        }

        [CanBeNull]
        private IXmpMeta _xmpMeta;

        public XmpDirectory()
        {
            SetDescriptor(new XmpDescriptor(this));
        }

        public override string Name
        {
            get { return "Xmp"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        internal void AddProperty([NotNull] string path, [NotNull] string value)
        {
            _propertyValueByPath[path] = value;
        }

        /// <summary>Gets a map of all XMP properties in this directory, not just the known ones.</summary>
        /// <remarks>
        /// This is required because XMP properties are represented as strings, whereas the rest of this library
        /// uses integers for keys.
        /// </remarks>
        [NotNull]
        public IDictionary<string, string> GetXmpProperties()
        {
            return new Dictionary<string, string>(_propertyValueByPath);
        }

        public void SetXmpMeta([NotNull] IXmpMeta xmpMeta)
        {
            _xmpMeta = xmpMeta;

            try
            {
                Set(TagXmpValueCount, _xmpMeta.Properties.Count(prop => prop.Path != null));
            }
            catch (XmpException)
            {
            }
        }

        /// <summary>Gets the <see cref="IXmpMeta"/> object within this directory.</summary>
        /// <remarks>This object provides a rich API for working with XMP data.</remarks>
        [CanBeNull]
        public IXmpMeta XmpMeta
        {
            get { return _xmpMeta; }
        }
    }
}
