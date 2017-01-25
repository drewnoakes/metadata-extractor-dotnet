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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Holds the basic metadata found in the header of a Photoshop PSD file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PsdHeaderDirectory : Directory
    {
        /// <summary>The number of channels in the image, including any alpha channels.</summary>
        /// <remarks>Supported range is 1 to 56.</remarks>
        public const int TagChannelCount = 1;

        /// <summary>The height of the image in pixels.</summary>
        public const int TagImageHeight = 2;

        /// <summary>The width of the image in pixels.</summary>
        public const int TagImageWidth = 3;

        /// <summary>The number of bits per channel.</summary>
        /// <remarks>Supported values are 1, 8, 16 and 32.</remarks>
        public const int TagBitsPerChannel = 4;

        /// <summary>The color mode of the file.</summary>
        /// <remarks>
        /// Supported values are:
        /// Bitmap = 0; Grayscale = 1; Indexed = 2; RGB = 3; CMYK = 4; Multichannel = 7; Duotone = 8; Lab = 9.
        /// </remarks>
        public const int TagColorMode = 5;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagChannelCount, "Channel Count" },
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagBitsPerChannel, "Bits Per Channel" },
            { TagColorMode, "Color Mode" }
        };

        public PsdHeaderDirectory()
        {
            SetDescriptor(new PsdHeaderDescriptor(this));
        }

        public override string Name => "PSD Header";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
