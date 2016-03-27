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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using XmpCore;

namespace MetadataExtractor.Formats.Xmp
{
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class XmpDirectory : Directory
    {
        public const int TagXmpValueCount = 0xFFFF;

        // These are some Tags, belonging to xmp-data-tags
        // The numeration is more like enums. The real xmp-tags are strings,
        // so we do some kind of mapping here...
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

        // dublin core properties
        // this requires further research
//        public const int TagTitle = 0x100;
        /// <summary>Keywords</summary>
        public const int TagSubject = 0x2001;
//        public const int TagDate = 0x1002;
//        public const int TagType = 0x1003;
//        public const int TagDescription = 0x1004;
//        public const int TagRelation = 0x1005;
//        public const int TagCoverage = 0x1006;
//        public const int TagCreator = 0x1007;
//        public const int TagPublisher = 0x1008;
//        public const int TagContributor = 0x1009;
//        public const int TagRights = 0x100A;
//        public const int TagFormat = 0x100B;
//        public const int TagIdentifier = 0x100C;
//        public const int TagLanguage = 0x100D;
//        public const int TagAudience = 0x100E;
//        public const int TagProvenance = 0x100F;
//        public const int TagRightsHolder = 0x1010;
//        public const int TagInstructionalMethod = 0x1011;
//        public const int TagAccrualMethod = 0x1012;
//        public const int TagAccrualPeriodicity = 0x1013;
//        public const int TagAccrualPolicy = 0x1014;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagXmpValueCount, "XMP Value Count" },
            { TagMake, "Make" },
            { TagModel, "Model" },
            { TagExposureTime, "Exposure Time" },
            { TagShutterSpeed, "Shutter Speed Value" },
            { TagFNumber, "F-Number" },
            { TagLensInfo, "Lens Information" },
            { TagLens, "Lens" },
            { TagCameraSerialNumber, "Serial Number" },
            { TagFirmware, "Firmware" },
            { TagFocalLength, "Focal Length" },
            { TagApertureValue, "Aperture Value" },
            { TagExposureProgram, "Exposure Program" },
            { TagDateTimeOriginal, "Date/Time Original" },
            { TagDateTimeDigitized, "Date/Time Digitized" },
            { TagRating, "Rating" },
            { TagLabel, "Label" },
//            { TagTitle, "Title" },
            { TagSubject, "Subject" }
//            { TagDate, "Date" },
//            { TagType, "Type" },
//            { TagDescription, "Description" },
//            { TagRelation, "Relation" },
//            { TagCoverage, "Coverage" },
//            { TagCreator, "Creator" },
//            { TagPublisher, "Publisher" },
//            { TagContributor, "Contributor" },
//            { TagRights, "Rights" },
//            { TagFormat, "Format" },
//            { TagIdentifier, "Identifier" },
//            { TagLanguage, "Language" },
//            { TagAudience, "Audience" },
//            { TagProvenance, "Provenance" },
//            { TagRightsHolder, "Rights Holder" },
//            { TagInstructionalMethod, "Instructional Method" },
//            { TagAccrualMethod, "Accrual Method" },
//            { TagAccrualPeriodicity, "Accrual Periodicity" },
//            { TagAccrualPolicy, "Accrual Policy" }
        };

        internal static readonly Dictionary<int, string> TagSchemaMap = new Dictionary<int, string>
        {
            { TagMake, Schema.ExifTiffProperties },
            { TagModel, Schema.ExifTiffProperties },
            { TagExposureTime, Schema.ExifSpecificProperties },
            { TagShutterSpeed, Schema.ExifSpecificProperties },
            { TagFNumber, Schema.ExifSpecificProperties },
            { TagLensInfo, Schema.ExifAdditionalProperties },
            { TagLens, Schema.ExifAdditionalProperties },
            { TagCameraSerialNumber, Schema.ExifAdditionalProperties },
            { TagFirmware, Schema.ExifAdditionalProperties },
            { TagFocalLength, Schema.ExifSpecificProperties },
            { TagApertureValue, Schema.ExifSpecificProperties },
            { TagExposureProgram, Schema.ExifSpecificProperties },
            { TagDateTimeOriginal, Schema.ExifSpecificProperties },
            { TagDateTimeDigitized, Schema.ExifSpecificProperties },
            { TagRating, Schema.XmpProperties },
            { TagLabel, Schema.XmpProperties },
//            { TagTitle, Schema.DublinCoreSpecificProperties },
            { TagSubject, Schema.DublinCoreSpecificProperties },
//            { TagDate, Schema.DublinCoreSpecificProperties },
//            { TagType, Schema.DublinCoreSpecificProperties },
//            { TagDescription, Schema.DublinCoreSpecificProperties },
//            { TagRelation, Schema.DublinCoreSpecificProperties },
//            { TagCoverage, Schema.DublinCoreSpecificProperties },
//            { TagCreator, Schema.DublinCoreSpecificProperties },
//            { TagPublisher, Schema.DublinCoreSpecificProperties },
//            { TagContributor, Schema.DublinCoreSpecificProperties },
//            { TagRights, Schema.DublinCoreSpecificProperties },
//            { TagFormat, Schema.DublinCoreSpecificProperties },
//            { TagIdentifier, Schema.DublinCoreSpecificProperties },
//            { TagLanguage, Schema.DublinCoreSpecificProperties },
//            { TagAudience, Schema.DublinCoreSpecificProperties },
//            { TagProvenance, Schema.DublinCoreSpecificProperties },
//            { TagRightsHolder, Schema.DublinCoreSpecificProperties },
//            { TagInstructionalMethod, Schema.DublinCoreSpecificProperties },
//            { TagAccrualMethod, Schema.DublinCoreSpecificProperties },
//            { TagAccrualPeriodicity, Schema.DublinCoreSpecificProperties },
//            { TagAccrualPolicy, Schema.DublinCoreSpecificProperties }
        };

        internal static readonly Dictionary<int, string> TagPropNameMap = new Dictionary<int, string>
        {
            { TagMake, "tiff:Make" },
            { TagModel, "tiff:Model" },
            { TagExposureTime, "exif:ExposureTime" },
            { TagShutterSpeed, "exif:ShutterSpeedValue" },
            { TagFNumber, "exif:FNumber" },
            { TagLensInfo, "aux:LensInfo" },
            { TagLens, "aux:Lens" },
            { TagCameraSerialNumber, "aux:SerialNumber" },
            { TagFirmware, "aux:Firmware" },
            { TagFocalLength, "exif:FocalLength" },
            { TagApertureValue, "exif:ApertureValue" },
            { TagExposureProgram, "exif:ExposureProgram" },
            { TagDateTimeOriginal, "exif:DateTimeOriginal" },
            { TagDateTimeDigitized, "exif:DateTimeDigitized" },
            { TagRating, "xmp:Rating" },
            { TagLabel, "xmp:Label" },
//            { TagTitle, "dc:title" },
            { TagSubject, "dc:subject" },
//            { TagDate, "dc:date" },
//            { TagType, "dc:type" },
//            { TagDescription, "dc:description" },
//            { TagRelation, "dc:relation" },
//            { TagCoverage, "dc:coverage" },
//            { TagCreator, "dc:creator" },
//            { TagPublisher, "dc:publisher" },
//            { TagContributor, "dc:contributor" },
//            { TagRights, "dc:rights" },
//            { TagFormat, "dc:format" },
//            { TagIdentifier, "dc:identifier" },
//            { TagLanguage, "dc:language" },
//            { TagAudience, "dc:audience" },
//            { TagProvenance, "dc:provenance" },
//            { TagRightsHolder, "dc:rightsHolder" },
//            { TagInstructionalMethod, "dc:instructionalMethod" },
//            { TagAccrualMethod, "dc:accrualMethod" },
//            { TagAccrualPeriodicity, "dc:accrualPeriodicity" },
//            { TagAccrualPolicy, "dc:accrualPolicy" }
        };

        [NotNull]
        private readonly Dictionary<string, string> _propertyValueByPath = new Dictionary<string, string>();

        [CanBeNull]
        private IXmpMeta _xmpMeta;

        public XmpDirectory()
        {
            SetDescriptor(new XmpDescriptor(this));
        }

        public override string Name => "XMP";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
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
        public IDictionary<string, string> GetXmpProperties() => new Dictionary<string, string>(_propertyValueByPath);

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
        public IXmpMeta XmpMeta => _xmpMeta;
    }
}
