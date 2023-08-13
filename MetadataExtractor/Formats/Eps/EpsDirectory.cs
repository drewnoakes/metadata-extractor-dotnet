// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Eps
{
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class EpsDirectory : Directory
    {
        // Sources: https://www-cdf.fnal.gov/offline/PostScript/5001.PDF
        //          http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/PostScript.html

        public const int TagDscVersion = 1;
        public const int TagAuthor = 2;
        public const int TagBoundingBox = 3;
        public const int TagCopyright = 4;
        public const int TagCreationDate = 5;
        public const int TagCreator = 6;
        public const int TagFor = 7;
        public const int TagImageData = 8;
        public const int TagKeywords = 9;
        public const int TagModifyDate = 10;
        public const int TagPages = 11;
        public const int TagRouting = 12;
        public const int TagSubject = 13;
        public const int TagTitle = 14;
        public const int TagVersion = 15;
        public const int TagDocumentData = 16;
        public const int TagEmulation = 17;
        public const int TagExtensions = 18;
        public const int TagLanguageLevel = 19;
        public const int TagOrientation = 20;
        public const int TagPageOrder = 21;
        public const int TagOperatorIntervention = 22;
        public const int TagOperatorMessage = 23;
        public const int TagProofMode = 24;
        public const int TagRequirements = 25;
        public const int TagVmLocation = 26;
        public const int TagVmUsage = 27;
        public const int TagImageWidth = 28;
        public const int TagImageHeight = 29;
        public const int TagColorType = 30;
        public const int TagRamSize = 31;
        public const int TagTiffPreviewSize = 32;
        public const int TagTiffPreviewOffset = 33;
        public const int TagWmfPreviewSize = 34;
        public const int TagWmfPreviewOffset = 35;
        public const int TagContinueLine = 36;

        // Section Markers
        // public const int TagBeginIcc = 37;
        // public const int TagBeginPhotoshop = 38;
        // public const int TagBeginXmlPacket = 39;
        // public const int TagBeginBinary = 40;
        // public const int TagBeginData = 41;
        // public const int TagAI9EndPrivateData = 42;

        internal static readonly Dictionary<int, string> TagNameMap = new()
        {
            { TagContinueLine, "Line Continuation" },
            { TagBoundingBox, "Bounding Box" },
            { TagCopyright, "Copyright" },
            { TagDocumentData, "Document Data" },
            { TagEmulation, "Emulation" },
            { TagExtensions, "Extensions" },
            { TagLanguageLevel, "Language Level" },
            { TagOrientation, "Orientation" },
            { TagPageOrder, "Page Order" },
            { TagVersion, "Version" },
            { TagImageData, "Image Data" },
            { TagImageWidth, "Image Width" },
            { TagImageHeight, "Image Height" },
            { TagColorType, "Color Type" },
            { TagRamSize, "Ram Size" },
            { TagCreator, "Creator" },
            { TagCreationDate, "Creation Date" },
            { TagFor, "For" },
            { TagRequirements, "Requirements" },
            { TagRouting, "Routing" },
            { TagTitle, "Title" },
            { TagDscVersion, "DSC Version" },
            { TagPages, "Pages" },
            { TagOperatorIntervention, "Operator Intervention" },
            { TagOperatorMessage, "Operator Message" },
            { TagProofMode, "Proof Mode" },
            { TagVmLocation, "VM Location" },
            { TagVmUsage, "VM Usage" },
            { TagAuthor, "Author" },
            { TagKeywords, "Keywords" },
            { TagModifyDate, "Modify Date" },
            { TagSubject, "Subject" },
            { TagTiffPreviewSize, "TIFF Preview Size" },
            { TagTiffPreviewOffset, "TIFF Preview Offset" },
            { TagWmfPreviewSize, "WMF Preview Size" },
            { TagWmfPreviewOffset, "WMF Preview Offset" }
        };

        internal static readonly Dictionary<string, int> TagIntegerMap = new()
        {
            { "%!PS-Adobe-", TagDscVersion },
            { "%%Author", TagAuthor },
            { "%%BoundingBox", TagBoundingBox },
            { "%%Copyright", TagCopyright },
            { "%%CreationDate", TagCreationDate },
            { "%%Creator", TagCreator },
            { "%%For", TagFor },
            { "%ImageData", TagImageData },
            { "%%Keywords", TagKeywords },
            { "%%ModDate", TagModifyDate },
            { "%%Pages", TagPages },
            { "%%Routing", TagRouting },
            { "%%Subject", TagSubject },
            { "%%Title", TagTitle },
            { "%%Version", TagVersion },
            { "%%DocumentData", TagDocumentData },
            { "%%Emulation", TagEmulation },
            { "%%Extensions", TagExtensions },
            { "%%LanguageLevel", TagLanguageLevel },
            { "%%Orientation", TagOrientation },
            { "%%PageOrder", TagPageOrder },
            { "%%OperatorIntervention", TagOperatorIntervention },
            { "%%OperatorMessage", TagOperatorMessage },
            { "%%ProofMode", TagProofMode },
            { "%%Requirements", TagRequirements },
            { "%%VMlocation", TagVmLocation },
            { "%%VMusage", TagVmUsage },
            { "Image Width", TagImageWidth },
            { "Image Height", TagImageHeight },
            { "Color Type", TagColorType },
            { "Ram Size", TagRamSize },
            { "TIFFPreview", TagTiffPreviewSize },
            { "TIFFPreviewOffset", TagTiffPreviewOffset },
            { "WMFPreview", TagWmfPreviewSize },
            { "WMFPreviewOffset", TagWmfPreviewOffset },
            { "%%+", TagContinueLine }
        };

        public EpsDirectory() : base(TagNameMap)
        {
            SetDescriptor(new EpsDescriptor(this));
        }

        public override string Name => "EPS";
    }
}
