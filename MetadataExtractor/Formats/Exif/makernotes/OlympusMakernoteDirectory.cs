#region License
//
// Copyright 2002-2015 Drew Noakes
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

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// The Olympus makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class OlympusMakernoteDirectory : Directory
    {
        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagMakernoteVersion = 0x0000;

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagCameraSettings1 = 0x0001;

        /// <summary>Alternate Camera Settings Tag.</summary>
        /// <remarks>Alternate Camera Settings Tag. Used by Konica / Minolta cameras.</remarks>
        public const int TagCameraSettings2 = 0x0003;

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagCompressedImageSize = 0x0040;

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagMinoltaThumbnailOffset1 = 0x0081;

        /// <summary>Alternate Thumbnail Offset.</summary>
        /// <remarks>Alternate Thumbnail Offset. Used by Konica / Minolta cameras.</remarks>
        public const int TagMinoltaThumbnailOffset2 = 0x0088;

        /// <summary>Length of thumbnail in bytes.</summary>
        /// <remarks>Length of thumbnail in bytes. Used by Konica / Minolta cameras.</remarks>
        public const int TagMinoltaThumbnailLength = 0x0089;

        public const int TagThumbnailImage = 0x0100;

        /// <summary>
        /// Used by Konica / Minolta cameras
        /// 0 = Natural Colour
        /// 1 = Black &amp; White
        /// 2 = Vivid colour
        /// 3 = Solarization
        /// 4 = AdobeRGB
        /// </summary>
        public const int TagColourMode = 0x0101;

        /// <summary>Used by Konica / Minolta cameras.</summary>
        /// <remarks>
        /// Used by Konica / Minolta cameras.
        /// 0 = Raw
        /// 1 = Super Fine
        /// 2 = Fine
        /// 3 = Standard
        /// 4 = Extra Fine
        /// </remarks>
        public const int TagImageQuality1 = 0x0102;

        /// <summary>Not 100% sure about this tag.</summary>
        /// <remarks>
        /// Not 100% sure about this tag.
        /// <para />
        /// Used by Konica / Minolta cameras.
        /// 0 = Raw
        /// 1 = Super Fine
        /// 2 = Fine
        /// 3 = Standard
        /// 4 = Extra Fine
        /// </remarks>
        public const int TagImageQuality2 = 0x0103;

        public const int TagBodyFirmwareVersion = 0x0104;

        /// <summary>
        /// Three values:
        /// Value 1: 0=Normal, 2=Fast, 3=Panorama
        /// Value 2: Sequence Number Value 3:
        /// 1 = Panorama Direction: Left to Right
        /// 2 = Panorama Direction: Right to Left
        /// 3 = Panorama Direction: Bottom to Top
        /// 4 = Panorama Direction: Top to Bottom
        /// </summary>
        public const int TagSpecialMode = 0x0200;

        /// <summary>
        /// 1 = Standard Quality
        /// 2 = High Quality
        /// 3 = Super High Quality
        /// </summary>
        public const int TagJpegQuality = 0x0201;

        /// <summary>
        /// 0 = Normal (Not Macro)
        /// 1 = Macro
        /// </summary>
        public const int TagMacroMode = 0x0202;

        /// <summary>0 = Off, 1 = On</summary>
        public const int TagBwMode = 0x0203;

        /// <summary>Zoom Factor (0 or 1 = normal)</summary>
        public const int TagDigiZoomRatio = 0x0204;

        public const int TagFocalPlaneDiagonal = 0x0205;
        public const int TagLensDistortionParameters = 0x0206;
        public const int TagFirmwareVersion = 0x0207;
        public const int TagPictInfo = 0x0208;
        public const int TagCameraId = 0x0209;

        /// <summary>
        /// Used by Epson cameras
        /// Units = pixels
        /// </summary>
        public const int TagImageWidth = 0x020B;

        /// <summary>
        /// Used by Epson cameras
        /// Units = pixels
        /// </summary>
        public const int TagImageHeight = 0x020C;

        /// <summary>A string.</summary>
        /// <remarks>A string. Used by Epson cameras.</remarks>
        public const int TagOriginalManufacturerModel = 0x020D;

        public const int TagPreviewImage = 0x0280;
        public const int TagPreCaptureFrames = 0x0300;
        public const int TagWhiteBoard = 0x0301;
        public const int TagOneTouchWb = 0x0302;
        public const int TagWhiteBalanceBracket = 0x0303;
        public const int TagWhiteBalanceBias = 0x0304;
        public const int TagSceneMode = 0x0403;
        public const int TagFirmware = 0x0404;

        /// <summary>
        /// See the PIM specification here:
        /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html
        /// </summary>
        public const int TagPrintImageMatchingInfo = 0x0E00;

        public const int TagDataDump1 = 0x0F00;
        public const int TagDataDump2 = 0x0F01;
        public const int TagShutterSpeedValue = 0x1000;
        public const int TagIsoValue = 0x1001;
        public const int TagApertureValue = 0x1002;
        public const int TagBrightnessValue = 0x1003;
        public const int TagFlashMode = 0x1004;
        public const int TagFlashDevice = 0x1005;
        public const int TagBracket = 0x1006;
        public const int TagSensorTemperature = 0x1007;
        public const int TagLensTemperature = 0x1008;
        public const int TagLightCondition = 0x1009;
        public const int TagFocusRange = 0x100A;
        public const int TagFocusMode = 0x100B;
        public const int TagFocusDistance = 0x100C;
        public const int TagZoom = 0x100D;
        public const int TagMacroFocus = 0x100E;
        public const int TagSharpness = 0x100F;
        public const int TagFlashChargeLevel = 0x1010;
        public const int TagColourMatrix = 0x1011;
        public const int TagBlackLevel = 0x1012;
        public const int TagWhiteBalance = 0x1015;
        public const int TagRedBias = 0x1017;
        public const int TagBlueBias = 0x1018;
        public const int TagColorMatrixNumber = 0x1019;
        public const int TagSerialNumber = 0x101A;
        public const int TagFlashBias = 0x1023;
        public const int TagExternalFlashBounce = 0x1026;
        public const int TagExternalFlashZoom = 0x1027;
        public const int TagExternalFlashMode = 0x1028;
        public const int TagContrast = 0x1029;
        public const int TagSharpnessFactor = 0x102A;
        public const int TagColourControl = 0x102B;
        public const int TagValidBits = 0x102C;
        public const int TagCoringFilter = 0x102D;
        public const int TagFinalWidth = 0x102E;
        public const int TagFinalHeight = 0x102F;
        public const int TagCompressionRatio = 0x1034;
        public const int TagThumbnail = 0x1035;
        public const int TagThumbnailOffset = 0x1036;
        public const int TagThumbnailLength = 0x1037;
        public const int TagCcdScanMode = 0x1039;
        public const int TagNoiseReduction = 0x103A;
        public const int TagInfinityLensStep = 0x103B;
        public const int TagNearLensStep = 0x103C;
        public const int TagEquipment = 0x2010;
        public const int TagCameraSettings = 0x2020;
        public const int TagRawDevelopment = 0x2030;
        public const int TagRawDevelopment2 = 0x2031;
        public const int TagImageProcessing = 0x2040;
        public const int TagFocusInfo = 0x2050;
        public const int TagRawInfo = 0x3000;

        // These 'sub'-tag values have been created for consistency -- they don't exist within the Makernote IFD
        public static class CameraSettings
        {
            internal const int Offset = 0xF000;

            public const int TagExposureMode = Offset + 2;
            public const int TagFlashMode = Offset + 3;
            public const int TagWhiteBalance = Offset + 4;
            public const int TagImageSize = Offset + 5;
            public const int TagImageQuality = Offset + 6;
            public const int TagShootingMode = Offset + 7;
            public const int TagMeteringMode = Offset + 8;
            public const int TagApexFilmSpeedValue = Offset + 9;
            public const int TagApexShutterSpeedTimeValue = Offset + 10;
            public const int TagApexApertureValue = Offset + 11;
            public const int TagMacroMode = Offset + 12;
            public const int TagDigitalZoom = Offset + 13;
            public const int TagExposureCompensation = Offset + 14;
            public const int TagBracketStep = Offset + 15;
            // 16 missing
            public const int TagIntervalLength = Offset + 17;
            public const int TagIntervalNumber = Offset + 18;
            public const int TagFocalLength = Offset + 19;
            public const int TagFocusDistance = Offset + 20;
            public const int TagFlashFired = Offset + 21;
            public const int TagDate = Offset + 22;
            public const int TagTime = Offset + 23;
            public const int TagMaxApertureAtFocalLength = Offset + 24;
            // 25, 26 missing
            public const int TagFileNumberMemory = Offset + 27;
            public const int TagLastFileNumber = Offset + 28;
            public const int TagWhiteBalanceRed = Offset + 29;
            public const int TagWhiteBalanceGreen = Offset + 30;
            public const int TagWhiteBalanceBlue = Offset + 31;
            public const int TagSaturation = Offset + 32;
            public const int TagContrast = Offset + 33;
            public const int TagSharpness = Offset + 34;
            public const int TagSubjectProgram = Offset + 35;
            public const int TagFlashCompensation = Offset + 36;
            public const int TagIsoSetting = Offset + 37;
            public const int TagCameraModel = Offset + 38;
            public const int TagIntervalMode = Offset + 39;
            public const int TagFolderName = Offset + 40;
            public const int TagColorMode = Offset + 41;
            public const int TagColorFilter = Offset + 42;
            public const int TagBlackAndWhiteFilter = Offset + 43;
            public const int TagInternalFlash = Offset + 44;
            public const int TagApexBrightnessValue = Offset + 45;
            public const int TagSpotFocusPointXCoordinate = Offset + 46;
            public const int TagSpotFocusPointYCoordinate = Offset + 47;
            public const int TagWideFocusZone = Offset + 48;
            public const int TagFocusMode = Offset + 49;
            public const int TagFocusArea = Offset + 50;
            public const int TagDecSwitchPosition = Offset + 51;
        }

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakernoteVersion, "Makernote Version" },
            { TagCameraSettings1, "Camera Settings" },
            { TagCameraSettings2, "Camera Settings" },
            { TagCompressedImageSize, "Compressed Image Size" },
            { TagMinoltaThumbnailOffset1, "Thumbnail Offset" },
            { TagMinoltaThumbnailOffset2, "Thumbnail Offset" },
            { TagMinoltaThumbnailLength, "Thumbnail Length" },
            { TagThumbnailImage, "Thumbnail Image" },
            { TagColourMode, "Colour Mode" },
            { TagImageQuality1, "Image Quality" },
            { TagImageQuality2, "Image Quality" },
            { TagBodyFirmwareVersion, "Body Firmware Version" },
            { TagSpecialMode, "Special Mode" },
            { TagJpegQuality, "JPEG Quality" },
            { TagMacroMode, "Macro" },
            { TagBwMode, "BW Mode" },
            { TagDigiZoomRatio, "DigiZoom Ratio" },
            { TagFocalPlaneDiagonal, "Focal Plane Diagonal" },
            { TagLensDistortionParameters, "Lens Distortion Parameters" },
            { TagFirmwareVersion, "Firmware Version" },
            { TagPictInfo, "Pict Info" },
            { TagCameraId, "Camera Id" },
            { TagImageWidth, "Image Width" },
            { TagImageHeight, "Image Height" },
            { TagOriginalManufacturerModel, "Original Manufacturer Model" },
            { TagPreviewImage, "Preview Image" },
            { TagPreCaptureFrames, "Pre Capture Frames" },
            { TagWhiteBoard, "White Board" },
            { TagOneTouchWb, "One Touch WB" },
            { TagWhiteBalanceBracket, "White Balance Bracket" },
            { TagWhiteBalanceBias, "White Balance Bias" },
            { TagSceneMode, "Scene Mode" },
            { TagFirmware, "Firmware" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagDataDump1, "Data Dump" },
            { TagDataDump2, "Data Dump 2" },
            { TagShutterSpeedValue, "Shutter Speed Value" },
            { TagIsoValue, "ISO Value" },
            { TagApertureValue, "Aperture Value" },
            { TagBrightnessValue, "Brightness Value" },
            { TagFlashMode, "Flash Mode" },
            { TagFlashDevice, "Flash Device" },
            { TagBracket, "Bracket" },
            { TagSensorTemperature, "Sensor Temperature" },
            { TagLensTemperature, "Lens Temperature" },
            { TagLightCondition, "Light Condition" },
            { TagFocusRange, "Focus Range" },
            { TagFocusMode, "Focus Mode" },
            { TagFocusDistance, "Focus Distance" },
            { TagZoom, "Zoom" },
            { TagMacroFocus, "Macro Focus" },
            { TagSharpness, "Sharpness" },
            { TagFlashChargeLevel, "Flash Charge Level" },
            { TagColourMatrix, "Colour Matrix" },
            { TagBlackLevel, "Black Level" },
            { TagWhiteBalance, "White Balance" },
            { TagRedBias, "Red Bias" },
            { TagBlueBias, "Blue Bias" },
            { TagColorMatrixNumber, "Color Matrix Number" },
            { TagSerialNumber, "Serial Number" },
            { TagFlashBias, "Flash Bias" },
            { TagExternalFlashBounce, "External Flash Bounce" },
            { TagExternalFlashZoom, "External Flash Zoom" },
            { TagExternalFlashMode, "External Flash Mode" },
            { TagContrast, "Contrast" },
            { TagSharpnessFactor, "Sharpness Factor" },
            { TagColourControl, "Colour Control" },
            { TagValidBits, "Valid Bits" },
            { TagCoringFilter, "Coring Filter" },
            { TagFinalWidth, "Final Width" },
            { TagFinalHeight, "Final Height" },
            { TagCompressionRatio, "Compression Ratio" },
            { TagThumbnail, "Thumbnail" },
            { TagThumbnailOffset, "Thumbnail Offset" },
            { TagThumbnailLength, "Thumbnail Length" },
            { TagCcdScanMode, "CCD Scan Mode" },
            { TagNoiseReduction, "Noise Reduction" },
            { TagInfinityLensStep, "Infinity Lens Step" },
            { TagNearLensStep, "Near Lens Step" },
            { TagEquipment, "Equipment" },
            { TagCameraSettings, "Camera Settings" },
            { TagRawDevelopment, "Raw Development" },
            { TagRawDevelopment2, "Raw Development 2" },
            { TagImageProcessing, "Image Processing" },
            { TagFocusInfo, "Focus Info" },
            { TagRawInfo, "Raw Info" },
            { CameraSettings.TagExposureMode, "Exposure Mode" },
            { CameraSettings.TagFlashMode, "Flash Mode" },
            { CameraSettings.TagWhiteBalance, "White Balance" },
            { CameraSettings.TagImageSize, "Image Size" },
            { CameraSettings.TagImageQuality, "Image Quality" },
            { CameraSettings.TagShootingMode, "Shooting Mode" },
            { CameraSettings.TagMeteringMode, "Metering Mode" },
            { CameraSettings.TagApexFilmSpeedValue, "Apex Film Speed Value" },
            { CameraSettings.TagApexShutterSpeedTimeValue, "Apex Shutter Speed Time Value" },
            { CameraSettings.TagApexApertureValue, "Apex Aperture Value" },
            { CameraSettings.TagMacroMode, "Macro Mode" },
            { CameraSettings.TagDigitalZoom, "Digital Zoom" },
            { CameraSettings.TagExposureCompensation, "Exposure Compensation" },
            { CameraSettings.TagBracketStep, "Bracket Step" },
            { CameraSettings.TagIntervalLength, "Interval Length" },
            { CameraSettings.TagIntervalNumber, "Interval Number" },
            { CameraSettings.TagFocalLength, "Focal Length" },
            { CameraSettings.TagFocusDistance, "Focus Distance" },
            { CameraSettings.TagFlashFired, "Flash Fired" },
            { CameraSettings.TagDate, "Date" },
            { CameraSettings.TagTime, "Time" },
            { CameraSettings.TagMaxApertureAtFocalLength, "Max Aperture at Focal Length" },
            { CameraSettings.TagFileNumberMemory, "File Number Memory" },
            { CameraSettings.TagLastFileNumber, "Last File Number" },
            { CameraSettings.TagWhiteBalanceRed, "White Balance Red" },
            { CameraSettings.TagWhiteBalanceGreen, "White Balance Green" },
            { CameraSettings.TagWhiteBalanceBlue, "White Balance Blue" },
            { CameraSettings.TagSaturation, "Saturation" },
            { CameraSettings.TagContrast, "Contrast" },
            { CameraSettings.TagSharpness, "Sharpness" },
            { CameraSettings.TagSubjectProgram, "Subject Program" },
            { CameraSettings.TagFlashCompensation, "Flash Compensation" },
            { CameraSettings.TagIsoSetting, "ISO Setting" },
            { CameraSettings.TagCameraModel, "Camera Model" },
            { CameraSettings.TagIntervalMode, "Interval Mode" },
            { CameraSettings.TagFolderName, "Folder Name" },
            { CameraSettings.TagColorMode, "Color Mode" },
            { CameraSettings.TagColorFilter, "Color Filter" },
            { CameraSettings.TagBlackAndWhiteFilter, "Black and White Filter" },
            { CameraSettings.TagInternalFlash, "Internal Flash" },
            { CameraSettings.TagApexBrightnessValue, "Apex Brightness Value" },
            { CameraSettings.TagSpotFocusPointXCoordinate, "Spot Focus Point X Coordinate" },
            { CameraSettings.TagSpotFocusPointYCoordinate, "Spot Focus Point Y Coordinate" },
            { CameraSettings.TagWideFocusZone, "Wide Focus Zone" },
            { CameraSettings.TagFocusMode, "Focus Mode" },
            { CameraSettings.TagFocusArea, "Focus Area" },
            { CameraSettings.TagDecSwitchPosition, "DEC Switch Position" }
        };

        public OlympusMakernoteDirectory()
        {
            SetDescriptor(new OlympusMakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Olympus Makernote"; }
        }

        public override void Set(int tagType, object value)
        {
            var bytes = value as byte[];

            if (bytes != null && (tagType == TagCameraSettings1 || tagType == TagCameraSettings2))
                ProcessCameraSettings(bytes);
            else
                base.Set(tagType, value);
        }

        private void ProcessCameraSettings([NotNull] byte[] bytes)
        {
            var reader = new SequentialByteArrayReader(bytes) { IsMotorolaByteOrder = true };
            var count = bytes.Length / 4;

            try
            {
                for (var i = 0; i < count; i++)
                    Set(CameraSettings.Offset + i, reader.GetInt32());
            }
            catch (IOException e)
            {
                // Should never happen, given that we check the length of the bytes beforehand.
                Console.WriteLine (e);
            }
        }

        public bool IsIntervalMode()
        {
            var value = this.GetInt64Nullable(CameraSettings.TagShootingMode);
            return value != null && value == 5;
        }

        protected override IReadOnlyDictionary<int, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
