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
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
    /// <summary>Holds the basic metadata found in the header of a Photoshop PSD file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PsdHeaderDirectory : Directory
    {
        /// <summary>The number of channels in the image, including any alpha channels.</summary>
        /// <remarks>The number of channels in the image, including any alpha channels. Supported range is 1 to 56.</remarks>
        public const int TagChannelCount = 1;

        /// <summary>The height of the image in pixels.</summary>
        public const int TagImageHeight = 2;

        /// <summary>The width of the image in pixels.</summary>
        public const int TagImageWidth = 3;

        /// <summary>The number of bits per channel.</summary>
        /// <remarks>The number of bits per channel. Supported values are 1, 8, 16 and 32.</remarks>
        public const int TagBitsPerChannel = 4;

        /// <summary>The color mode of the file.</summary>
        /// <remarks>
        /// The color mode of the file. Supported values are:
        /// Bitmap = 0; Grayscale = 1; Indexed = 2; RGB = 3; CMYK = 4; Multichannel = 7; Duotone = 8; Lab = 9.
        /// </remarks>
        public const int TagColorMode = 5;

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PsdHeaderDirectory()
        {
            TagNameMap.Put(TagChannelCount, "Channel Count");
            TagNameMap.Put(TagImageHeight, "Image Height");
            TagNameMap.Put(TagImageWidth, "Image Width");
            TagNameMap.Put(TagBitsPerChannel, "Bits Per Channel");
            TagNameMap.Put(TagColorMode, "Color Mode");
        }

        public PsdHeaderDirectory()
        {
            this.SetDescriptor(new PsdHeaderDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "PSD Header";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
