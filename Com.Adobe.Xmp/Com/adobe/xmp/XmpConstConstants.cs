// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp
{
    public static class XmpConstConstants
    {
        /// <summary>The XML namespace for XML.</summary>
        public const string NsXml = "http://www.w3.org/XML/1998/namespace";

        /// <summary>The XML namespace for RDF.</summary>
        public const string NsRdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        /// <summary>The XML namespace for the Dublin Core schema.</summary>
        public const string NsDc = "http://purl.org/dc/elements/1.1/";

        /// <summary>The XML namespace for the IPTC Core schema.</summary>
        public const string NsIptccore = "http://iptc.org/std/Iptc4xmpCore/1.0/xmlns/";

        /// <summary>The XML namespace for the IPTC Extension schema.</summary>
        public const string NsIptcext = "http://iptc.org/std/Iptc4xmpExt/2008-02-29/";

        /// <summary>The XML namespace for the DICOM medical schema.</summary>
        public const string NsDicom = "http://ns.adobe.com/DICOM/";

        /// <summary>The XML namespace for the PLUS (Picture Licensing Universal System, http://www.useplus.org)</summary>
        public const string NsPlus = "http://ns.useplus.org/ldf/xmp/1.0/";

        /// <summary>The XML namespace Adobe XMP Metadata.</summary>
        public const string NsX = "adobe:ns:meta/";

        public const string NsIx = "http://ns.adobe.com/iX/1.0/";

        /// <summary>The XML namespace for the XMP "basic" schema.</summary>
        public const string NsXmp = "http://ns.adobe.com/xap/1.0/";

        /// <summary>The XML namespace for the XMP copyright schema.</summary>
        public const string NsXmpRights = "http://ns.adobe.com/xap/1.0/rights/";

        /// <summary>The XML namespace for the XMP digital asset management schema.</summary>
        public const string NsXmpMm = "http://ns.adobe.com/xap/1.0/mm/";

        /// <summary>The XML namespace for the job management schema.</summary>
        public const string NsXmpBj = "http://ns.adobe.com/xap/1.0/bj/";

        /// <summary>The XML namespace for the job management schema.</summary>
        public const string NsXmpNote = "http://ns.adobe.com/xmp/note/";

        /// <summary>The XML namespace for the PDF schema.</summary>
        public const string NsPdf = "http://ns.adobe.com/pdf/1.3/";

        /// <summary>The XML namespace for the PDF schema.</summary>
        public const string NsPdfx = "http://ns.adobe.com/pdfx/1.3/";

        public const string NsPdfxId = "http://www.npes.org/pdfx/ns/id/";

        public const string NsPdfaSchema = "http://www.aiim.org/pdfa/ns/schema#";

        public const string NsPdfaProperty = "http://www.aiim.org/pdfa/ns/property#";

        public const string NsPdfaType = "http://www.aiim.org/pdfa/ns/type#";

        public const string NsPdfaField = "http://www.aiim.org/pdfa/ns/field#";

        public const string NsPdfaId = "http://www.aiim.org/pdfa/ns/id/";

        public const string NsPdfaExtension = "http://www.aiim.org/pdfa/ns/extension/";

        /// <summary>The XML namespace for the Photoshop custom schema.</summary>
        public const string NsPhotoshop = "http://ns.adobe.com/photoshop/1.0/";

        /// <summary>The XML namespace for the Photoshop Album schema.</summary>
        public const string NsPsalbum = "http://ns.adobe.com/album/1.0/";

        /// <summary>The XML namespace for Adobe's EXIF schema.</summary>
        public const string NsExif = "http://ns.adobe.com/exif/1.0/";

        /// <summary>NS for the CIPA XMP for Exif document v1.1</summary>
        public const string NsExifx = "http://cipa.jp/exif/1.0/";

        public const string NsExifAux = "http://ns.adobe.com/exif/1.0/aux/";

        /// <summary>The XML namespace for Adobe's TIFF schema.</summary>
        public const string NsTiff = "http://ns.adobe.com/tiff/1.0/";

        public const string NsPng = "http://ns.adobe.com/png/1.0/";

        public const string NsJpeg = "http://ns.adobe.com/jpeg/1.0/";

        public const string NsJp2K = "http://ns.adobe.com/jp2k/1.0/";

        public const string NsCameraraw = "http://ns.adobe.com/camera-raw-settings/1.0/";

        public const string NsAdobestockphoto = "http://ns.adobe.com/StockPhoto/1.0/";

        public const string NsCreatorAtom = "http://ns.adobe.com/creatorAtom/1.0/";

        public const string NsAsf = "http://ns.adobe.com/asf/1.0/";

        public const string NsWav = "http://ns.adobe.com/xmp/wav/1.0/";

        /// <summary>BExt Schema</summary>
        public const string NsBwf = "http://ns.adobe.com/bwf/bext/1.0/";

        /// <summary>RIFF Info Schema</summary>
        public const string NsRiffinfo = "http://ns.adobe.com/riff/info/";

        public const string NsScript = "http://ns.adobe.com/xmp/1.0/Script/";

        /// <summary>Transform XMP</summary>
        public const string NsTxmp = "http://ns.adobe.com/TransformXMP/";

        /// <summary>Adobe Flash SWF</summary>
        public const string NsSwf = "http://ns.adobe.com/swf/1.0/";

        public const string NsDm = "http://ns.adobe.com/xmp/1.0/DynamicMedia/";

        public const string NsTransient = "http://ns.adobe.com/xmp/transient/1.0/";

        /// <summary>legacy Dublin Core NS, will be converted to NS_DC</summary>
        public const string NsDcDeprecated = "http://purl.org/dc/1.1/";

        /// <summary>The XML namespace for qualifiers of the xmp:Identifier property.</summary>
        public const string TypeIdentifierqual = "http://ns.adobe.com/xmp/Identifier/qual/1.0/";

        /// <summary>The XML namespace for fields of the Dimensions type.</summary>
        public const string TypeDimensions = "http://ns.adobe.com/xap/1.0/sType/Dimensions#";

        public const string TypeText = "http://ns.adobe.com/xap/1.0/t/";

        public const string TypePagedfile = "http://ns.adobe.com/xap/1.0/t/pg/";

        public const string TypeGraphics = "http://ns.adobe.com/xap/1.0/g/";

        /// <summary>The XML namespace for fields of a graphical image.</summary>
        /// <remarks>The XML namespace for fields of a graphical image. Used for the Thumbnail type.</remarks>
        public const string TypeImage = "http://ns.adobe.com/xap/1.0/g/img/";

        public const string TypeFont = "http://ns.adobe.com/xap/1.0/sType/Font#";

        /// <summary>The XML namespace for fields of the ResourceEvent type.</summary>
        public const string TypeResourceevent = "http://ns.adobe.com/xap/1.0/sType/ResourceEvent#";

        /// <summary>The XML namespace for fields of the ResourceRef type.</summary>
        public const string TypeResourceref = "http://ns.adobe.com/xap/1.0/sType/ResourceRef#";

        /// <summary>The XML namespace for fields of the Version type.</summary>
        public const string TypeStVersion = "http://ns.adobe.com/xap/1.0/sType/Version#";

        /// <summary>The XML namespace for fields of the JobRef type.</summary>
        public const string TypeStJob = "http://ns.adobe.com/xap/1.0/sType/Job#";

        public const string TypeManifestitem = "http://ns.adobe.com/xap/1.0/sType/ManifestItem#";

        /// <summary>The canonical true string value for Booleans in serialized XMP.</summary>
        /// <remarks>
        /// The canonical true string value for Booleans in serialized XMP. Code that converts from the
        /// string to a bool should be case insensitive, and even allow "1".
        /// </remarks>
        public const string TrueString = "True";

        /// <summary>The canonical false string value for Booleans in serialized XMP.</summary>
        /// <remarks>
        /// The canonical false string value for Booleans in serialized XMP. Code that converts from the
        /// string to a bool should be case insensitive, and even allow "0".
        /// </remarks>
        public const string FalseString = "False";

        /// <summary>Index that has the meaning to be always the last item in an array.</summary>
        public const int ArrayLastItem = -1;

        /// <summary>Node name of an array item.</summary>
        public const string ArrayItemName = "[]";

        /// <summary>The x-default string for localized properties</summary>
        public const string XDefault = "x-default";

        /// <summary>xml:lang qualfifier</summary>
        public const string XmlLang = "xml:lang";

        /// <summary>rdf:type qualfifier</summary>
        public const string RdfType = "rdf:type";

        /// <summary>Processing Instruction (PI) for xmp packet</summary>
        public const string XmpPi = "xpacket";

        /// <summary>XMP meta tag version new</summary>
        public const string TagXmpmeta = "xmpmeta";

        /// <summary>XMP meta tag version old</summary>
        public const string TagXapmeta = "xapmeta";
    }
}
