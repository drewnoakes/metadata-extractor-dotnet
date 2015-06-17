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

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Contains image encoding information for DCT filters, as stored by Adobe.</summary>
    public class AdobeJpegDirectory : Directory
    {
        public const int TagDctEncodeVersion = 0;

        /// <summary>The convention for TAG_APP14_FLAGS0 and TAG_APP14_FLAGS1 is that 0 bits are benign.</summary>
        /// <remarks>
        /// The convention for TAG_APP14_FLAGS0 and TAG_APP14_FLAGS1 is that 0 bits are benign.
        /// 1 bits in TAG_APP14_FLAGS0 pass information that is possibly useful but not essential for decoding.
        /// <para />
        /// 0x8000 bit: Encoder used Blend=1 downsampling
        /// </remarks>
        public const int TagApp14Flags0 = 1;

        /// <summary>The convention for TAG_APP14_FLAGS0 and TAG_APP14_FLAGS1 is that 0 bits are benign.</summary>
        /// <remarks>
        /// The convention for TAG_APP14_FLAGS0 and TAG_APP14_FLAGS1 is that 0 bits are benign.
        /// 1 bits in TAG_APP14_FLAGS1 pass information essential for decoding. DCTDecode could reject a compressed
        /// image, if there are 1 bits in TAG_APP14_FLAGS1 or color transform codes that it cannot interpret.
        /// </remarks>
        public const int TagApp14Flags1 = 2;

        public const int TagColorTransform = 3;

        private static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>
        {
            { TagDctEncodeVersion, "DCT Encode Version" },
            { TagApp14Flags0, "Flags 0" },
            { TagApp14Flags1, "Flags 1" },
            { TagColorTransform, "Color Transform" }
        };

        public AdobeJpegDirectory()
        {
            SetDescriptor(new AdobeJpegDescriptor(this));
        }

        public override string Name
        {
            get { return "Adobe JPEG"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
