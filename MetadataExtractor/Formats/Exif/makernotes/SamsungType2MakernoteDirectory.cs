// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to certain 'newer' Samsung cameras.</summary>
    /// <remarks>
    /// Tag reference from: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Samsung.html
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SamsungType2MakernoteDirectory : Directory
    {
        // This list is incomplete
        public const int TagMakerNoteVersion = 0x001;
        public const int TagDeviceType = 0x0002;
        public const int TagSamsungModelId = 0x0003;

        public const int TagCameraTemperature = 0x0043;

        public const int TagFaceDetect = 0x0100;
        public const int TagFaceRecognition = 0x0120;
        public const int TagFaceName = 0x0123;

        // following tags found only in SRW images
        public const int TagFirmwareName = 0xa001;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakerNoteVersion, "Maker Note Version" },
            { TagDeviceType, "Device Type" },
            { TagSamsungModelId, "Model Id" },

            { TagCameraTemperature, "Camera Temperature" },

            { TagFaceDetect, "Face Detect" },
            { TagFaceRecognition, "Face Recognition" },
            { TagFaceName, "Face Name" },
            { TagFirmwareName, "Firmware Name" }
        };

        public SamsungType2MakernoteDirectory()
        {
            SetDescriptor(new SamsungType2MakernoteDescriptor(this));
        }

        public override string Name => "Samsung Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
