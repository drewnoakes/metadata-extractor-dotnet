// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PngDescriptor : TagDescriptor<PngDirectory>
    {
        public PngDescriptor(PngDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PngDirectory.TagColorType => GetColorTypeDescription(),
                PngDirectory.TagCompressionType => GetCompressionTypeDescription(),
                PngDirectory.TagFilterMethod => GetFilterMethodDescription(),
                PngDirectory.TagInterlaceMethod => GetInterlaceMethodDescription(),
                PngDirectory.TagPaletteHasTransparency => GetPaletteHasTransparencyDescription(),
                PngDirectory.TagSrgbRenderingIntent => GetIsSrgbColorSpaceDescription(),
                PngDirectory.TagTextualData => GetTextualDataDescription(),
                PngDirectory.TagBackgroundColor => GetBackgroundColorDescription(),
                PngDirectory.TagUnitSpecifier => GetUnitSpecifierDescription(),
                PngDirectory.TagLastModificationTime => GetLastModificationTimeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetColorTypeDescription()
        {
            if (!Directory.TryGetInt32(PngDirectory.TagColorType, out int value))
                return null;
            return PngColorType.FromNumericValue(value).Description;
        }

        public string? GetCompressionTypeDescription()
        {
            return GetIndexedDescription(PngDirectory.TagCompressionType, "Deflate");
        }

        public string? GetFilterMethodDescription()
        {
            return GetIndexedDescription(PngDirectory.TagFilterMethod, "Adaptive");
        }

        public string? GetInterlaceMethodDescription()
        {
            return GetIndexedDescription(PngDirectory.TagInterlaceMethod, "No Interlace", "Adam7 Interlace");
        }

        public string? GetPaletteHasTransparencyDescription()
        {
            return GetIndexedDescription(PngDirectory.TagPaletteHasTransparency, null, "Yes");
        }

        public string? GetIsSrgbColorSpaceDescription()
        {
            return GetIndexedDescription(PngDirectory.TagSrgbRenderingIntent, "Perceptual", "Relative Colorimetric", "Saturation", "Absolute Colorimetric");
        }

        public string? GetUnitSpecifierDescription()
        {
            return GetIndexedDescription(PngDirectory.TagUnitSpecifier, "Unspecified", "Metres");
        }

        public string? GetLastModificationTimeDescription()
        {
            if (!Directory.TryGetDateTime(PngDirectory.TagLastModificationTime, out DateTime value))
                return null;

            return value.ToString("yyyy:MM:dd HH:mm:ss");
        }

        public string? GetTextualDataDescription()
        {
            return !(Directory.GetObject(PngDirectory.TagTextualData) is IList<KeyValuePair> pairs)
                ? null
                : string.Join(
                    "\n",
                    pairs.Select(kv => $"{kv.Key}: {kv.Value}")
#if NET35
                    .ToArray()
#endif
                    );
        }

        public string? GetBackgroundColorDescription()
        {
            var bytes = Directory.GetByteArray(PngDirectory.TagBackgroundColor);
            if (bytes == null)
                return null;

            var reader = new SequentialByteArrayReader(bytes);
            try
            {
                // TODO do we need to normalise these based upon the bit depth?
                switch (bytes.Length)
                {
                    case 1:
                        return $"Palette Index {reader.GetByte()}";
                    case 2:
                        return $"Greyscale Level {reader.GetUInt16()}";
                    case 6:
                        return $"R {reader.GetUInt16()}, G {reader.GetUInt16()}, B {reader.GetUInt16()}";
                }
            }
            catch (IOException)
            {
                return null;
            }

            return null;
        }
    }
}
