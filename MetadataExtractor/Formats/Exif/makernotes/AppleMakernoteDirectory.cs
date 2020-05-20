// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Apple cameras.</summary>
    /// <remarks>Using information from http://owl.phy.queensu.ca/~phil/exiftool/TagNames/Apple.html</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AppleMakernoteDirectory : Directory
    {
#pragma warning disable format
        public const int TagRunTime            = 0x0003;
        /// <summary>
        /// XYZ coordinates of the acceleration vector in units of g.
        /// As viewed from the front of the phone,
        /// positive X is toward the left side,
        /// positive Y is toward the bottom,
        /// positive Z points into the face of the phone
        /// </summary>
        public const int TagAccelerationVector = 0x0008;
        public const int TagHdrImageType       = 0x000a;
        /// <summary>
        /// Unique ID for all images in a burst.
        /// </summary>
        public const int TagBurstUuid          = 0x000b;
        public const int TagContentIdentifier  = 0x0011;
        public const int TagImageUniqueId      = 0x0015;
        public const int TagLivePhotoId        = 0x0017;
#pragma warning restore format

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagRunTime, "Run Time" },
            { TagAccelerationVector, "Acceleration Vector" },
            { TagHdrImageType, "HDR Image Type" },
            { TagBurstUuid, "Burst UUID" },
            { TagContentIdentifier, "Content Identifier" },
            { TagImageUniqueId, "Image Unique ID" },
            { TagLivePhotoId, "Live Photo ID" }
        };

        public AppleMakernoteDirectory()
        {
            SetDescriptor(new AppleMakernoteDescriptor(this));
        }

        public override string Name => "Apple Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
