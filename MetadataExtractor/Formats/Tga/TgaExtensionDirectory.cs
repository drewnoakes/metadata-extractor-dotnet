// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga
{
    /// <author>Dmitry Shechtman</author>
    public sealed class TgaExtensionDirectory : Directory
    {
        public const int TagAuthorName = 1;
        public const int TagComments = 2;
        public const int TagDateTime = 3;
        public const int TagJobName = 4;
        public const int TagJobTime = 5;
        public const int TagSoftwareName = 6;
        public const int TagSoftwareVersion = 7;
        public const int TagKeyColor = 8;
        public const int TagAspectRatio = 9;
        public const int TagGamma = 10;
        public const int TagColorCorrectionOffset = 11;
        public const int TagThumbnailOffset = 12;
        public const int TagScanLineOffset = 13;
        public const int TagAttributesType = 14;

        private static readonly string[] _tagNames =
        [
            "Author Name",
            "Comments",
            "Date/Time",
            "Job Name",
            "Job Time",
            "Software Name",
            "Software Version",
            "Key Color",
            "Pixel Aspect Ratio",
            "Gamma Value",
            "Color Correction Table Offset",
            "Thumbnail Offset",
            "Scan Line Table Offset",
            "Attributes Type"
        ];

        public TgaExtensionDirectory() : base(null)
        {
            SetDescriptor(new TgaExtensionDescriptor(this));
        }

        public override string Name => "TGA Extension Area";

        protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
        {
            tagName = tagType > 0 && tagType <= _tagNames.Length
                ? _tagNames[tagType - 1]
                : null;
            return tagName is not null;
        }
    }
}
