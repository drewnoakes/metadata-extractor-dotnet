using JetBrains.Annotations;

namespace MetadataExtractor
{
    public static class Schema
    {
        /// <summary>XMP tag namespace.</summary>
        /// <remarks>
        /// XMP tag namespace. TODO the older "xap", "xapBJ", "xapMM" or "xapRights" namespace prefixes should be translated to the newer "xmp", "xmpBJ",
        /// "xmpMM" and "xmpRights" prefixes for use in family 1 group names
        /// </remarks>
        [NotNull]
        public const string XmpProperties = "http://ns.adobe.com/xap/1.0/";

        [NotNull]
        public const string ExifSpecificProperties = "http://ns.adobe.com/exif/1.0/";

        [NotNull]
        public const string ExifAdditionalProperties = "http://ns.adobe.com/exif/1.0/aux/";

        [NotNull]
        public const string ExifTiffProperties = "http://ns.adobe.com/tiff/1.0/";

        [NotNull]
        public const string DublinCoreSpecificProperties = "http://purl.org/dc/elements/1.1/";
    }
}
