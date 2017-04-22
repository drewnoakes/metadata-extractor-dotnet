#region License
//
// Copyright 2002-2017 Drew Noakes
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PngDescriptor : TagDescriptor<PngDirectory>
    {
        public PngDescriptor([NotNull] PngDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PngDirectory.TagColorType:
                    return GetColorTypeDescription();
                case PngDirectory.TagCompressionType:
                    return GetCompressionTypeDescription();
                case PngDirectory.TagFilterMethod:
                    return GetFilterMethodDescription();
                case PngDirectory.TagInterlaceMethod:
                    return GetInterlaceMethodDescription();
                case PngDirectory.TagPaletteHasTransparency:
                    return GetPaletteHasTransparencyDescription();
                case PngDirectory.TagSrgbRenderingIntent:
                    return GetIsSrgbColorSpaceDescription();
                case PngDirectory.TagTextualData:
                    return GetTextualDataDescription();
                case PngDirectory.TagBackgroundColor:
                    return GetBackgroundColorDescription();
                case PngDirectory.TagUnitSpecifier:
                    return GetUnitSpecifierDescription();
                case PngDirectory.TagLastModificationTime:
                    return GetLastModificationTimeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetColorTypeDescription()
        {
            if (!Directory.TryGetInt32(PngDirectory.TagColorType, out int value))
                return null;
            return PngColorType.FromNumericValue(value).Description;
        }

        [CanBeNull]
        public string GetCompressionTypeDescription()
        {
            return GetIndexedDescription(PngDirectory.TagCompressionType, "Deflate");
        }

        [CanBeNull]
        public string GetFilterMethodDescription()
        {
            return GetIndexedDescription(PngDirectory.TagFilterMethod, "Adaptive");
        }

        [CanBeNull]
        public string GetInterlaceMethodDescription()
        {
            return GetIndexedDescription(PngDirectory.TagInterlaceMethod, "No Interlace", "Adam7 Interlace");
        }

        [CanBeNull]
        public string GetPaletteHasTransparencyDescription()
        {
            return GetIndexedDescription(PngDirectory.TagPaletteHasTransparency, null, "Yes");
        }

        [CanBeNull]
        public string GetIsSrgbColorSpaceDescription()
        {
            return GetIndexedDescription(PngDirectory.TagSrgbRenderingIntent, "Perceptual", "Relative Colorimetric", "Saturation", "Absolute Colorimetric");
        }

        [CanBeNull]
        public string GetUnitSpecifierDescription()
        {
            return GetIndexedDescription(PngDirectory.TagUnitSpecifier, "Unspecified", "Metres");
        }

        [CanBeNull]
        public string GetLastModificationTimeDescription()
        {
            if (!Directory.TryGetDateTime(PngDirectory.TagLastModificationTime, out DateTime value))
                return null;

            return value.ToString("yyyy:MM:dd HH:mm:ss");
        }

        [CanBeNull]
        public string GetTextualDataDescription()
        {
            var pairs = Directory.GetObject(PngDirectory.TagTextualData) as IList<KeyValuePair>;

            return pairs == null
                ? null
                : string.Join(
                    "\n",
                    pairs.Select(kv => $"{kv.Key}: {kv.Value}")
#if NET35
                    .ToArray()
#endif
                    );
        }

        [CanBeNull]
        public string GetBackgroundColorDescription()
        {
            var bytes = Directory.GetByteArray(PngDirectory.TagBackgroundColor);
            if (bytes == null || !Directory.TryGetInt32(PngDirectory.TagColorType, out int colorType))
                return null;

            var reader = new SequentialByteArrayReader(bytes);
            try
            {
                switch (colorType)
                {
                    case 0:
                    case 4:
                        // TODO do we need to normalise these based upon the bit depth?
                        return $"Greyscale Level {reader.GetUInt16()}";
                    case 2:
                    case 6:
                        return $"R {reader.GetUInt16()}, G {reader.GetUInt16()}, B {reader.GetUInt16()}";
                    case 3:
                        return $"Palette Index {reader.GetByte()}";
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
