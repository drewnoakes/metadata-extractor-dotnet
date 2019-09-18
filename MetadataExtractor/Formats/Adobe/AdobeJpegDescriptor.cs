// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Provides human-readable string versions of the tags stored in an AdobeJpegDirectory.</summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AdobeJpegDescriptor : TagDescriptor<AdobeJpegDirectory>
    {
        public AdobeJpegDescriptor(AdobeJpegDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                AdobeJpegDirectory.TagColorTransform => GetColorTransformDescription(),
                AdobeJpegDirectory.TagDctEncodeVersion => GetDctEncodeVersionDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetDctEncodeVersionDescription()
        {
            if (!Directory.TryGetInt32(AdobeJpegDirectory.TagDctEncodeVersion, out int value))
                return null;

            return value == 0x64 ? "100" : value.ToString();
        }

        public string? GetColorTransformDescription()
        {
            return GetIndexedDescription(AdobeJpegDirectory.TagColorTransform,
                "Unknown (RGB or CMYK)",
                "YCbCr",
                "YCCK");
        }
    }
}
