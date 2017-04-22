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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PsdHeaderDescriptor : TagDescriptor<PsdHeaderDirectory>
    {
        public PsdHeaderDescriptor([NotNull] PsdHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PsdHeaderDirectory.TagChannelCount:
                    return GetChannelCountDescription();
                case PsdHeaderDirectory.TagBitsPerChannel:
                    return GetBitsPerChannelDescription();
                case PsdHeaderDirectory.TagColorMode:
                    return GetColorModeDescription();
                case PsdHeaderDirectory.TagImageHeight:
                    return GetImageHeightDescription();
                case PsdHeaderDirectory.TagImageWidth:
                    return GetImageWidthDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetChannelCountDescription()
        {
            // Supported range is 1 to 56.
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagChannelCount, out int value))
                return null;
            return value + " channel" + (value == 1 ? string.Empty : "s");
        }

        [CanBeNull]
        public string GetBitsPerChannelDescription()
        {
            // Supported values are 1, 8, 16 and 32.
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagBitsPerChannel, out int value))
                return null;
            return value + " bit" + (value == 1 ? string.Empty : "s") + " per channel";
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            return GetIndexedDescription(PsdHeaderDirectory.TagColorMode,
                "Bitmap",
                "Grayscale",
                "Indexed",
                "RGB",
                "CMYK",
                null,
                null,
                "Multichannel",
                "Duotone",
                "Lab");
        }

        [CanBeNull]
        public string GetImageHeightDescription()
        {
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagImageHeight, out int value))
                return null;
            return value + " pixel" + (value == 1 ? string.Empty : "s");
        }

        [CanBeNull]
        public string GetImageWidthDescription()
        {
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagImageWidth, out int value))
                return null;
            return value + " pixel" + (value == 1 ? string.Empty : "s");
        }
    }
}
