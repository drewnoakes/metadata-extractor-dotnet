// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

        public const int TagOrientationInfo = 0x0011;

        public const int TagSmartAlbumColor = 0x0020;
        public const int TagPictureWizard = 0x0021;

        public const int TagLocalLocationName = 0x0030;

        public const int TagPreviewIfd = 0x0035;

        public const int TagRawDataByteOrder = 0x0040;
        public const int TagWhiteBalanceSetup = 0x0041;

        public const int TagCameraTemperature = 0x0043;

        public const int TagRawDataCfaPattern = 0x0050;

        public const int TagFaceDetect = 0x0100;
        public const int TagFaceRecognition = 0x0120;
        public const int TagFaceName = 0x0123;

        // following tags found only in SRW images
        public const int TagFirmwareName = 0xa001;
        public const int TagSerialNumber = 0xa002;
        public const int TagLensType = 0xa003;
        public const int TagLensFirmware = 0xa004;
        public const int TagInternalLensSerialNumber = 0xa005;

        public const int TagSensorAreas = 0xa010;
        public const int TagColorSpace = 0xa011;
        public const int TagSmartRange = 0xa012;
        public const int TagExposureCompensation = 0xa013;
        public const int TagIso = 0xa014;

        public const int TagExposureTime = 0xa018;
        public const int TagFNumber = 0xa019;

        public const int TagFocalLengthIn35MMFormat = 0xa01a;

        public const int TagEncryptionKey = 0xa020;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagMakerNoteVersion, "Maker Note Version" },
            { TagDeviceType, "Device Type" },
            { TagSamsungModelId, "Model Id" },

            { TagOrientationInfo, "Orientation Info" },

            { TagSmartAlbumColor, "Smart Album Color" },
            { TagPictureWizard, "Picture Wizard" },

            { TagLocalLocationName, "Local Location Name" },

            { TagPreviewIfd, "Preview IFD" },

            { TagRawDataByteOrder, "Raw Data Byte Order" },
            { TagWhiteBalanceSetup, "White Balance Setup" },

            { TagCameraTemperature, "Camera Temperature" },

            { TagRawDataCfaPattern, "Raw Data CFA Pattern" },

            { TagFaceDetect, "Face Detect" },
            { TagFaceRecognition, "Face Recognition" },
            { TagFaceName, "Face Name" },

            { TagFirmwareName, "Firmware Name" },
            { TagSerialNumber, "Serial Number" },
            { TagLensType, "Lens Type" },
            { TagLensFirmware, "Lens Firmware" },
            { TagInternalLensSerialNumber, "Internal Lens Serial Number" },

            { TagSensorAreas, "Sensor Areas" },
            { TagColorSpace, "Color Space" },
            { TagSmartRange, "Smart Range" },
            { TagExposureCompensation, "Exposure Compensation" },
            { TagIso, "ISO" },

            { TagExposureTime, "Exposure Time" },
            { TagFNumber, "F-Number" },

            { TagFocalLengthIn35MMFormat, "Focal Length in 35mm Format" },

            { TagEncryptionKey, "Encryption Key" }
        };

        public SamsungType2MakernoteDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new SamsungType2MakernoteDescriptor(this));
        }

        public override string Name => "Samsung Makernote";
    }
}
