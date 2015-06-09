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

using System;
using System.Collections.Generic;
using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// The Olympus makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class OlympusMakernoteDirectory : Directory
    {
        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagMakernoteVersion = unchecked(0x0000);

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagCameraSettings1 = unchecked(0x0001);

        /// <summary>Alternate Camera Settings Tag.</summary>
        /// <remarks>Alternate Camera Settings Tag. Used by Konica / Minolta cameras.</remarks>
        public const int TagCameraSettings2 = unchecked(0x0003);

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagCompressedImageSize = unchecked(0x0040);

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagMinoltaThumbnailOffset1 = unchecked(0x0081);

        /// <summary>Alternate Thumbnail Offset.</summary>
        /// <remarks>Alternate Thumbnail Offset. Used by Konica / Minolta cameras.</remarks>
        public const int TagMinoltaThumbnailOffset2 = unchecked(0x0088);

        /// <summary>Length of thumbnail in bytes.</summary>
        /// <remarks>Length of thumbnail in bytes. Used by Konica / Minolta cameras.</remarks>
        public const int TagMinoltaThumbnailLength = unchecked(0x0089);

        public const int TagThumbnailImage = unchecked(0x0100);

        /// <summary>
        /// Used by Konica / Minolta cameras
        /// 0 = Natural Colour
        /// 1 = Black &amp; White
        /// 2 = Vivid colour
        /// 3 = Solarization
        /// 4 = AdobeRGB
        /// </summary>
        public const int TagColourMode = unchecked(0x0101);

        /// <summary>Used by Konica / Minolta cameras.</summary>
        /// <remarks>
        /// Used by Konica / Minolta cameras.
        /// 0 = Raw
        /// 1 = Super Fine
        /// 2 = Fine
        /// 3 = Standard
        /// 4 = Extra Fine
        /// </remarks>
        public const int TagImageQuality1 = unchecked(0x0102);

        /// <summary>Not 100% sure about this tag.</summary>
        /// <remarks>
        /// Not 100% sure about this tag.
        /// <para>
        /// Used by Konica / Minolta cameras.
        /// 0 = Raw
        /// 1 = Super Fine
        /// 2 = Fine
        /// 3 = Standard
        /// 4 = Extra Fine
        /// </remarks>
        public const int TagImageQuality2 = unchecked(0x0103);

        public const int TagBodyFirmwareVersion = unchecked(0x0104);

        /// <summary>
        /// Three values:
        /// Value 1: 0=Normal, 2=Fast, 3=Panorama
        /// Value 2: Sequence Number Value 3:
        /// 1 = Panorama Direction: Left to Right
        /// 2 = Panorama Direction: Right to Left
        /// 3 = Panorama Direction: Bottom to Top
        /// 4 = Panorama Direction: Top to Bottom
        /// </summary>
        public const int TagSpecialMode = unchecked(0x0200);

        /// <summary>
        /// 1 = Standard Quality
        /// 2 = High Quality
        /// 3 = Super High Quality
        /// </summary>
        public const int TagJpegQuality = unchecked(0x0201);

        /// <summary>
        /// 0 = Normal (Not Macro)
        /// 1 = Macro
        /// </summary>
        public const int TagMacroMode = unchecked(0x0202);

        /// <summary>0 = Off, 1 = On</summary>
        public const int TagBwMode = unchecked(0x0203);

        /// <summary>Zoom Factor (0 or 1 = normal)</summary>
        public const int TagDigiZoomRatio = unchecked(0x0204);

        public const int TagFocalPlaneDiagonal = unchecked(0x0205);

        public const int TagLensDistortionParameters = unchecked(0x0206);

        public const int TagFirmwareVersion = unchecked(0x0207);

        public const int TagPictInfo = unchecked(0x0208);

        public const int TagCameraId = unchecked(0x0209);

        /// <summary>
        /// Used by Epson cameras
        /// Units = pixels
        /// </summary>
        public const int TagImageWidth = unchecked(0x020B);

        /// <summary>
        /// Used by Epson cameras
        /// Units = pixels
        /// </summary>
        public const int TagImageHeight = unchecked(0x020C);

        /// <summary>A string.</summary>
        /// <remarks>A string. Used by Epson cameras.</remarks>
        public const int TagOriginalManufacturerModel = unchecked(0x020D);

        public const int TagPreviewImage = unchecked(0x0280);

        public const int TagPreCaptureFrames = unchecked(0x0300);

        public const int TagWhiteBoard = unchecked(0x0301);

        public const int TagOneTouchWb = unchecked(0x0302);

        public const int TagWhiteBalanceBracket = unchecked(0x0303);

        public const int TagWhiteBalanceBias = unchecked(0x0304);

        public const int TagSceneMode = unchecked(0x0403);

        public const int TagFirmware = unchecked(0x0404);

        /// <summary>
        /// See the PIM specification here:
        /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html
        /// </summary>
        public const int TagPrintImageMatchingInfo = unchecked(0x0E00);

        public const int TagDataDump1 = unchecked(0x0F00);

        public const int TagDataDump2 = unchecked(0x0F01);

        public const int TagShutterSpeedValue = unchecked(0x1000);

        public const int TagIsoValue = unchecked(0x1001);

        public const int TagApertureValue = unchecked(0x1002);

        public const int TagBrightnessValue = unchecked(0x1003);

        public const int TagFlashMode = unchecked(0x1004);

        public const int TagFlashDevice = unchecked(0x1005);

        public const int TagBracket = unchecked(0x1006);

        public const int TagSensorTemperature = unchecked(0x1007);

        public const int TagLensTemperature = unchecked(0x1008);

        public const int TagLightCondition = unchecked(0x1009);

        public const int TagFocusRange = unchecked(0x100A);

        public const int TagFocusMode = unchecked(0x100B);

        public const int TagFocusDistance = unchecked(0x100C);

        public const int TagZoom = unchecked(0x100D);

        public const int TagMacroFocus = unchecked(0x100E);

        public const int TagSharpness = unchecked(0x100F);

        public const int TagFlashChargeLevel = unchecked(0x1010);

        public const int TagColourMatrix = unchecked(0x1011);

        public const int TagBlackLevel = unchecked(0x1012);

        public const int TagWhiteBalance = unchecked(0x1015);

        public const int TagRedBias = unchecked(0x1017);

        public const int TagBlueBias = unchecked(0x1018);

        public const int TagColorMatrixNumber = unchecked(0x1019);

        public const int TagSerialNumber = unchecked(0x101A);

        public const int TagFlashBias = unchecked(0x1023);

        public const int TagExternalFlashBounce = unchecked(0x1026);

        public const int TagExternalFlashZoom = unchecked(0x1027);

        public const int TagExternalFlashMode = unchecked(0x1028);

        public const int TagContrast = unchecked(0x1029);

        public const int TagSharpnessFactor = unchecked(0x102A);

        public const int TagColourControl = unchecked(0x102B);

        public const int TagValidBits = unchecked(0x102C);

        public const int TagCoringFilter = unchecked(0x102D);

        public const int TagFinalWidth = unchecked(0x102E);

        public const int TagFinalHeight = unchecked(0x102F);

        public const int TagCompressionRatio = unchecked(0x1034);

        public const int TagThumbnail = unchecked(0x1035);

        public const int TagThumbnailOffset = unchecked(0x1036);

        public const int TagThumbnailLength = unchecked(0x1037);

        public const int TagCcdScanMode = unchecked(0x1039);

        public const int TagNoiseReduction = unchecked(0x103A);

        public const int TagInfinityLensStep = unchecked(0x103B);

        public const int TagNearLensStep = unchecked(0x103C);

        public const int TagEquipment = unchecked(0x2010);

        public const int TagCameraSettings = unchecked(0x2020);

        public const int TagRawDevelopment = unchecked(0x2030);

        public const int TagRawDevelopment2 = unchecked(0x2031);

        public const int TagImageProcessing = unchecked(0x2040);

        public const int TagFocusInfo = unchecked(0x2050);

        public const int TagRawInfo = unchecked(0x3000);

        public static class CameraSettings
        {
            internal const int Offset = unchecked(0xF000);

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

            public const int TagIntervalLength = Offset + 17;

            public const int TagIntervalNumber = Offset + 18;

            public const int TagFocalLength = Offset + 19;

            public const int TagFocusDistance = Offset + 20;

            public const int TagFlashFired = Offset + 21;

            public const int TagDate = Offset + 22;

            public const int TagTime = Offset + 23;

            public const int TagMaxApertureAtFocalLength = Offset + 24;

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
            //    public static final int TAG_ = 0x1013;
            //    public static final int TAG_ = 0x1014;
            //    public static final int TAG_ = 0x1016;
            //    public static final int TAG_ = 0x101B;
            //    public static final int TAG_ = 0x101C;
            //    public static final int TAG_ = 0x101D;
            //    public static final int TAG_ = 0x101E;
            //    public static final int TAG_ = 0x101F;
            //    public static final int TAG_ = 0x1020;
            //    public static final int TAG_ = 0x1021;
            //    public static final int TAG_ = 0x1022;
            //    public static final int TAG_ = 0x1024;
            //    public static final int TAG_ = 0x1025;
            //    public static final int TAG_ = 0x1030;
            //    public static final int TAG_ = 0x1031;
            //    public static final int TAG_ = 0x1032;
            //    public static final int TAG_ = 0x1033;
            //    public static final int TAG_ = 0x1038;
            // These 'sub'-tag values have been created for consistency -- they don't exist within the Makernote IFD
            // 16 missing
            // 25, 26 missing
        }

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static OlympusMakernoteDirectory()
        {
            TagNameMap[TagMakernoteVersion] = "Makernote Version";
            TagNameMap[TagCameraSettings1] = "Camera Settings";
            TagNameMap[TagCameraSettings2] = "Camera Settings";
            TagNameMap[TagCompressedImageSize] = "Compressed Image Size";
            TagNameMap[TagMinoltaThumbnailOffset1] = "Thumbnail Offset";
            TagNameMap[TagMinoltaThumbnailOffset2] = "Thumbnail Offset";
            TagNameMap[TagMinoltaThumbnailLength] = "Thumbnail Length";
            TagNameMap[TagThumbnailImage] = "Thumbnail Image";
            TagNameMap[TagColourMode] = "Colour Mode";
            TagNameMap[TagImageQuality1] = "Image Quality";
            TagNameMap[TagImageQuality2] = "Image Quality";
            TagNameMap[TagBodyFirmwareVersion] = "Body Firmware Version";
            TagNameMap[TagSpecialMode] = "Special Mode";
            TagNameMap[TagJpegQuality] = "JPEG Quality";
            TagNameMap[TagMacroMode] = "Macro";
            TagNameMap[TagBwMode] = "BW Mode";
            TagNameMap[TagDigiZoomRatio] = "DigiZoom Ratio";
            TagNameMap[TagFocalPlaneDiagonal] = "Focal Plane Diagonal";
            TagNameMap[TagLensDistortionParameters] = "Lens Distortion Parameters";
            TagNameMap[TagFirmwareVersion] = "Firmware Version";
            TagNameMap[TagPictInfo] = "Pict Info";
            TagNameMap[TagCameraId] = "Camera Id";
            TagNameMap[TagImageWidth] = "Image Width";
            TagNameMap[TagImageHeight] = "Image Height";
            TagNameMap[TagOriginalManufacturerModel] = "Original Manufacturer Model";
            TagNameMap[TagPreviewImage] = "Preview Image";
            TagNameMap[TagPreCaptureFrames] = "Pre Capture Frames";
            TagNameMap[TagWhiteBoard] = "White Board";
            TagNameMap[TagOneTouchWb] = "One Touch WB";
            TagNameMap[TagWhiteBalanceBracket] = "White Balance Bracket";
            TagNameMap[TagWhiteBalanceBias] = "White Balance Bias";
            TagNameMap[TagSceneMode] = "Scene Mode";
            TagNameMap[TagFirmware] = "Firmware";
            TagNameMap[TagPrintImageMatchingInfo] = "Print Image Matching (PIM) Info";
            TagNameMap[TagDataDump1] = "Data Dump";
            TagNameMap[TagDataDump2] = "Data Dump 2";
            TagNameMap[TagShutterSpeedValue] = "Shutter Speed Value";
            TagNameMap[TagIsoValue] = "ISO Value";
            TagNameMap[TagApertureValue] = "Aperture Value";
            TagNameMap[TagBrightnessValue] = "Brightness Value";
            TagNameMap[TagFlashMode] = "Flash Mode";
            TagNameMap[TagFlashDevice] = "Flash Device";
            TagNameMap[TagBracket] = "Bracket";
            TagNameMap[TagSensorTemperature] = "Sensor Temperature";
            TagNameMap[TagLensTemperature] = "Lens Temperature";
            TagNameMap[TagLightCondition] = "Light Condition";
            TagNameMap[TagFocusRange] = "Focus Range";
            TagNameMap[TagFocusMode] = "Focus Mode";
            TagNameMap[TagFocusDistance] = "Focus Distance";
            TagNameMap[TagZoom] = "Zoom";
            TagNameMap[TagMacroFocus] = "Macro Focus";
            TagNameMap[TagSharpness] = "Sharpness";
            TagNameMap[TagFlashChargeLevel] = "Flash Charge Level";
            TagNameMap[TagColourMatrix] = "Colour Matrix";
            TagNameMap[TagBlackLevel] = "Black Level";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagRedBias] = "Red Bias";
            TagNameMap[TagBlueBias] = "Blue Bias";
            TagNameMap[TagColorMatrixNumber] = "Color Matrix Number";
            TagNameMap[TagSerialNumber] = "Serial Number";
            TagNameMap[TagFlashBias] = "Flash Bias";
            TagNameMap[TagExternalFlashBounce] = "External Flash Bounce";
            TagNameMap[TagExternalFlashZoom] = "External Flash Zoom";
            TagNameMap[TagExternalFlashMode] = "External Flash Mode";
            TagNameMap[TagContrast] = "Contrast";
            TagNameMap[TagSharpnessFactor] = "Sharpness Factor";
            TagNameMap[TagColourControl] = "Colour Control";
            TagNameMap[TagValidBits] = "Valid Bits";
            TagNameMap[TagCoringFilter] = "Coring Filter";
            TagNameMap[TagFinalWidth] = "Final Width";
            TagNameMap[TagFinalHeight] = "Final Height";
            TagNameMap[TagCompressionRatio] = "Compression Ratio";
            TagNameMap[TagThumbnail] = "Thumbnail";
            TagNameMap[TagThumbnailOffset] = "Thumbnail Offset";
            TagNameMap[TagThumbnailLength] = "Thumbnail Length";
            TagNameMap[TagCcdScanMode] = "CCD Scan Mode";
            TagNameMap[TagNoiseReduction] = "Noise Reduction";
            TagNameMap[TagInfinityLensStep] = "Infinity Lens Step";
            TagNameMap[TagNearLensStep] = "Near Lens Step";
            TagNameMap[TagEquipment] = "Equipment";
            TagNameMap[TagCameraSettings] = "Camera Settings";
            TagNameMap[TagRawDevelopment] = "Raw Development";
            TagNameMap[TagRawDevelopment2] = "Raw Development 2";
            TagNameMap[TagImageProcessing] = "Image Processing";
            TagNameMap[TagFocusInfo] = "Focus Info";
            TagNameMap[TagRawInfo] = "Raw Info";
            TagNameMap[CameraSettings.TagExposureMode] = "Exposure Mode";
            TagNameMap[CameraSettings.TagFlashMode] = "Flash Mode";
            TagNameMap[CameraSettings.TagWhiteBalance] = "White Balance";
            TagNameMap[CameraSettings.TagImageSize] = "Image Size";
            TagNameMap[CameraSettings.TagImageQuality] = "Image Quality";
            TagNameMap[CameraSettings.TagShootingMode] = "Shooting Mode";
            TagNameMap[CameraSettings.TagMeteringMode] = "Metering Mode";
            TagNameMap[CameraSettings.TagApexFilmSpeedValue] = "Apex Film Speed Value";
            TagNameMap[CameraSettings.TagApexShutterSpeedTimeValue] = "Apex Shutter Speed Time Value";
            TagNameMap[CameraSettings.TagApexApertureValue] = "Apex Aperture Value";
            TagNameMap[CameraSettings.TagMacroMode] = "Macro Mode";
            TagNameMap[CameraSettings.TagDigitalZoom] = "Digital Zoom";
            TagNameMap[CameraSettings.TagExposureCompensation] = "Exposure Compensation";
            TagNameMap[CameraSettings.TagBracketStep] = "Bracket Step";
            TagNameMap[CameraSettings.TagIntervalLength] = "Interval Length";
            TagNameMap[CameraSettings.TagIntervalNumber] = "Interval Number";
            TagNameMap[CameraSettings.TagFocalLength] = "Focal Length";
            TagNameMap[CameraSettings.TagFocusDistance] = "Focus Distance";
            TagNameMap[CameraSettings.TagFlashFired] = "Flash Fired";
            TagNameMap[CameraSettings.TagDate] = "Date";
            TagNameMap[CameraSettings.TagTime] = "Time";
            TagNameMap[CameraSettings.TagMaxApertureAtFocalLength] = "Max Aperture at Focal Length";
            TagNameMap[CameraSettings.TagFileNumberMemory] = "File Number Memory";
            TagNameMap[CameraSettings.TagLastFileNumber] = "Last File Number";
            TagNameMap[CameraSettings.TagWhiteBalanceRed] = "White Balance Red";
            TagNameMap[CameraSettings.TagWhiteBalanceGreen] = "White Balance Green";
            TagNameMap[CameraSettings.TagWhiteBalanceBlue] = "White Balance Blue";
            TagNameMap[CameraSettings.TagSaturation] = "Saturation";
            TagNameMap[CameraSettings.TagContrast] = "Contrast";
            TagNameMap[CameraSettings.TagSharpness] = "Sharpness";
            TagNameMap[CameraSettings.TagSubjectProgram] = "Subject Program";
            TagNameMap[CameraSettings.TagFlashCompensation] = "Flash Compensation";
            TagNameMap[CameraSettings.TagIsoSetting] = "ISO Setting";
            TagNameMap[CameraSettings.TagCameraModel] = "Camera Model";
            TagNameMap[CameraSettings.TagIntervalMode] = "Interval Mode";
            TagNameMap[CameraSettings.TagFolderName] = "Folder Name";
            TagNameMap[CameraSettings.TagColorMode] = "Color Mode";
            TagNameMap[CameraSettings.TagColorFilter] = "Color Filter";
            TagNameMap[CameraSettings.TagBlackAndWhiteFilter] = "Black and White Filter";
            TagNameMap[CameraSettings.TagInternalFlash] = "Internal Flash";
            TagNameMap[CameraSettings.TagApexBrightnessValue] = "Apex Brightness Value";
            TagNameMap[CameraSettings.TagSpotFocusPointXCoordinate] = "Spot Focus Point X Coordinate";
            TagNameMap[CameraSettings.TagSpotFocusPointYCoordinate] = "Spot Focus Point Y Coordinate";
            TagNameMap[CameraSettings.TagWideFocusZone] = "Wide Focus Zone";
            TagNameMap[CameraSettings.TagFocusMode] = "Focus Mode";
            TagNameMap[CameraSettings.TagFocusArea] = "Focus Area";
            TagNameMap[CameraSettings.TagDecSwitchPosition] = "DEC Switch Position";
        }

        public OlympusMakernoteDirectory()
        {
            SetDescriptor(new OlympusMakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Olympus Makernote";
        }

        public override void SetByteArray(int tagType, byte[] bytes)
        {
            if (tagType == TagCameraSettings1 || tagType == TagCameraSettings2)
            {
                ProcessCameraSettings(bytes);
            }
            else
            {
                base.SetByteArray(tagType, bytes);
            }
        }

        private void ProcessCameraSettings(byte[] bytes)
        {
            SequentialByteArrayReader reader = new SequentialByteArrayReader(bytes);
            reader.SetMotorolaByteOrder(true);
            int count = bytes.Length / 4;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    int value = reader.GetInt32();
                    SetInt(CameraSettings.Offset + i, value);
                }
            }
            catch (IOException e)
            {
                // Should never happen, given that we check the length of the bytes beforehand.
                Console.WriteLine (e);
            }
        }

        public bool IsIntervalMode()
        {
            long? value = GetLongObject(CameraSettings.TagShootingMode);
            return value != null && value == 5;
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
