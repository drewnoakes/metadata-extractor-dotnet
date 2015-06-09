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

using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="KyoceraMakernoteDirectory"/>.
    /// <para />
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
    /// <para />
    /// Most manufacturer's Makernote counts the "offset to data" from the first byte
    /// of TIFF header (same as the other IFD), but Kyocera (along with Fujifilm) counts
    /// it from the first byte of Makernote itself.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class KyoceraMakernoteDescriptor : TagDescriptor<KyoceraMakernoteDirectory>
    {
        public KyoceraMakernoteDescriptor([NotNull] KyoceraMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case KyoceraMakernoteDirectory.TagPrintImageMatchingInfo:
                {
                    return GetPrintImageMatchingInfoDescription();
                }

                case KyoceraMakernoteDirectory.TagProprietaryThumbnail:
                {
                    return GetProprietaryThumbnailDataDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetPrintImageMatchingInfoDescription()
        {
            return GetByteLengthDescription(KyoceraMakernoteDirectory.TagPrintImageMatchingInfo);
        }

        [CanBeNull]
        public string GetProprietaryThumbnailDataDescription()
        {
            return GetByteLengthDescription(KyoceraMakernoteDirectory.TagProprietaryThumbnail);
        }
    }
}
