#region License
//
// Copyright 2002-2019 Drew Noakes
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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="KyoceraMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
    /// <para />
    /// Most manufacturer's Makernote counts the "offset to data" from the first byte
    /// of TIFF header (same as the other IFD), but Kyocera (along with Fujifilm) counts
    /// it from the first byte of Makernote itself.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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
                case KyoceraMakernoteDirectory.TagProprietaryThumbnail:
                    return GetProprietaryThumbnailDataDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetProprietaryThumbnailDataDescription()
        {
            return GetByteLengthDescription(KyoceraMakernoteDirectory.TagProprietaryThumbnail);
        }
    }
}
