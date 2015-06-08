/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Drew.Imaging.Png;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PngDescriptor : TagDescriptor<PngDirectory>
    {
        public PngDescriptor([NotNull] PngDirectory directory)
            : base(directory)
        {
        }

        [CanBeNull]
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
        public virtual string GetColorTypeDescription()
        {
            int? value = _directory.GetInteger(PngDirectory.TagColorType);
            if (value == null)
            {
                return null;
            }
            PngColorType colorType = PngColorType.FromNumericValue((int)value);
            if (colorType == null)
            {
                return null;
            }
            return colorType.GetDescription();
        }

        [CanBeNull]
        public virtual string GetCompressionTypeDescription()
        {
            return GetIndexedDescription(PngDirectory.TagCompressionType, "Deflate");
        }

        [CanBeNull]
        public virtual string GetFilterMethodDescription()
        {
            return GetIndexedDescription(PngDirectory.TagFilterMethod, "Adaptive");
        }

        [CanBeNull]
        public virtual string GetInterlaceMethodDescription()
        {
            return GetIndexedDescription(PngDirectory.TagInterlaceMethod, "No Interlace", "Adam7 Interlace");
        }

        [CanBeNull]
        public virtual string GetPaletteHasTransparencyDescription()
        {
            return GetIndexedDescription(PngDirectory.TagPaletteHasTransparency, null, "Yes");
        }

        [CanBeNull]
        public virtual string GetIsSrgbColorSpaceDescription()
        {
            return GetIndexedDescription(PngDirectory.TagSrgbRenderingIntent, "Perceptual", "Relative Colorimetric", "Saturation", "Absolute Colorimetric");
        }

        [CanBeNull]
        public virtual string GetUnitSpecifierDescription()
        {
            return GetIndexedDescription(PngDirectory.TagUnitSpecifier, "Unspecified", "Metres");
        }

        [CanBeNull]
        public virtual string GetTextualDataDescription()
        {
            object @object = _directory.GetObject(PngDirectory.TagTextualData);
            if (@object == null)
            {
                return null;
            }
            IList<KeyValuePair> keyValues = (IList<KeyValuePair>)@object;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair keyValue in keyValues)
            {
                if (sb.Length != 0)
                {
                    sb.Append('\n');
                }
                sb.Append(Extensions.StringFormat("%s: %s", keyValue.GetKey(), keyValue.GetValue()));
            }
            return Extensions.ConvertToString(sb);
        }

        [CanBeNull]
        public virtual string GetBackgroundColorDescription()
        {
            sbyte[] bytes = _directory.GetByteArray(PngDirectory.TagBackgroundColor);
            int? colorType = _directory.GetInteger(PngDirectory.TagColorType);
            if (bytes == null || colorType == null)
            {
                return null;
            }
            SequentialReader reader = new SequentialByteArrayReader(bytes);
            try
            {
                switch (colorType)
                {
                    case 0:
                    case 4:
                    {
                        // TODO do we need to normalise these based upon the bit depth?
                        return Extensions.StringFormat("Greyscale Level %d", reader.GetUInt16());
                    }

                    case 2:
                    case 6:
                    {
                        return Extensions.StringFormat("R %d, G %d, B %d", reader.GetUInt16(), reader.GetUInt16(), reader.GetUInt16());
                    }

                    case 3:
                    {
                        return Extensions.StringFormat("Palette Index %d", reader.GetUInt8());
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
