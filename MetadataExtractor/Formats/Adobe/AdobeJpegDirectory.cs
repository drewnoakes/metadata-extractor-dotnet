// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Contains image encoding information for DCT filters, as stored by Adobe.</summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AdobeJpegDirectory : Directory
    {
        public const int TagDctEncodeVersion = 0;

        /// <remarks>
        /// The convention for TAG_APP14_FLAGS0 and TAG_APP14_FLAGS1 is that 0 bits are benign.
        /// 1 bits in TAG_APP14_FLAGS0 pass information that is possibly useful but not essential for decoding.
        /// <para />
        /// 0x8000 bit: Encoder used Blend=1 downsampling
        /// </remarks>
        public const int TagApp14Flags0 = 1;

        /// <remarks>
        /// The convention for TAG_APP14_FLAGS0 and TAG_APP14_FLAGS1 is that 0 bits are benign.
        /// 1 bits in TAG_APP14_FLAGS1 pass information essential for decoding. DCTDecode could reject a compressed
        /// image, if there are 1 bits in TAG_APP14_FLAGS1 or color transform codes that it cannot interpret.
        /// </remarks>
        public const int TagApp14Flags1 = 2;

        public const int TagColorTransform = 3;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
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

        public override string Name => "Adobe JPEG";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
