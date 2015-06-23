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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
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
                {
                    return GetColorTypeDescription();
                }

                case PngDirectory.TagCompressionType:
                {
                    return GetCompressionTypeDescription();
                }

                case PngDirectory.TagFilterMethod:
                {
                    return GetFilterMethodDescription();
                }

                case PngDirectory.TagInterlaceMethod:
                {
                    return GetInterlaceMethodDescription();
                }

                case PngDirectory.TagPaletteHasTransparency:
                {
                    return GetPaletteHasTransparencyDescription();
                }

                case PngDirectory.TagSrgbRenderingIntent:
                {
                    return GetIsSrgbColorSpaceDescription();
                }

                case PngDirectory.TagTextualData:
                {
                    return GetTextualDataDescription();
                }

                case PngDirectory.TagBackgroundColor:
                {
                    return GetBackgroundColorDescription();
                }

                case PngDirectory.TagUnitSpecifier:
                {
                    return GetUnitSpecifierDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetColorTypeDescription()
        {
            int value;
            if (!Directory.TryGetInt32(PngDirectory.TagColorType, out value))
                return null;
            var colorType = PngColorType.FromNumericValue(value);
            if (colorType == null)
            {
                return null;
            }
            return colorType.Description;
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
        public string GetTextualDataDescription()
        {
            var pairs = Directory.GetObject(PngDirectory.TagTextualData) as IList<KeyValuePair>;

            return pairs == null
                ? null
                : string.Join("\n", pairs.Select(kv => string.Format("{0}: {1}", kv.Key, kv.Value)));
        }

        [CanBeNull]
        public string GetBackgroundColorDescription()
        {
            var bytes = Directory.GetByteArray(PngDirectory.TagBackgroundColor);
            int colorType;
            if (bytes == null || !Directory.TryGetInt32(PngDirectory.TagColorType, out colorType))
                return null;

            var reader = new SequentialByteArrayReader(bytes);
            try
            {
                switch (colorType)
                {
                    case 0:
                    case 4:
                    {
                        // TODO do we need to normalise these based upon the bit depth?
                        return string.Format("Greyscale Level {0}", reader.GetUInt16());
                    }

                    case 2:
                    case 6:
                    {
                        return string.Format("R {0}, G {1}, B {2}", reader.GetUInt16(), reader.GetUInt16(), reader.GetUInt16());
                    }

                    case 3:
                    {
                        return string.Format("Palette Index {0}", reader.GetByte());
                    }
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
