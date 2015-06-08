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
using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// The Olympus makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class OlympusMakernoteDirectory : Directory
    {
        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagMakernoteVersion = unchecked((int)(0x0000));

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagCameraSettings1 = unchecked((int)(0x0001));

        /// <summary>Alternate Camera Settings Tag.</summary>
        /// <remarks>Alternate Camera Settings Tag. Used by Konica / Minolta cameras.</remarks>
        public const int TagCameraSettings2 = unchecked((int)(0x0003));

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagCompressedImageSize = unchecked((int)(0x0040));

        /// <summary>Used by Konica / Minolta cameras.</summary>
        public const int TagMinoltaThumbnailOffset1 = unchecked((int)(0x0081));

        /// <summary>Alternate Thumbnail Offset.</summary>
        /// <remarks>Alternate Thumbnail Offset. Used by Konica / Minolta cameras.</remarks>
        public const int TagMinoltaThumbnailOffset2 = unchecked((int)(0x0088));

        /// <summary>Length of thumbnail in bytes.</summary>
        /// <remarks>Length of thumbnail in bytes. Used by Konica / Minolta cameras.</remarks>
        public const int TagMinoltaThumbnailLength = unchecked((int)(0x0089));

        public const int TagThumbnailImage = unchecked((int)(0x0100));

        /// <summary>
        /// Used by Konica / Minolta cameras
        /// 0 = Natural Colour
        /// 1 = Black &amp; White
        /// 2 = Vivid colour
        /// 3 = Solarization
        /// 4 = AdobeRGB
        /// </summary>
        public const int TagColourMode = unchecked((int)(0x0101));

        /// <summary>Used by Konica / Minolta cameras.</summary>
        /// <remarks>
        /// Used by Konica / Minolta cameras.
        /// 0 = Raw
        /// 1 = Super Fine
        /// 2 = Fine
        /// 3 = Standard
        /// 4 = Extra Fine
        /// </remarks>
        public const int TagImageQuality1 = unchecked((int)(0x0102));

        /// <summary>Not 100% sure about this tag.</summary>
        /// <remarks>
        /// Not 100% sure about this tag.
        /// <p>
        /// Used by Konica / Minolta cameras.
        /// 0 = Raw
        /// 1 = Super Fine
        /// 2 = Fine
        /// 3 = Standard
        /// 4 = Extra Fine
        /// </remarks>
        public const int TagImageQuality2 = unchecked((int)(0x0103));

        public const int TagBodyFirmwareVersion = unchecked((int)(0x0104));

        /// <summary>
        /// Three values:
        /// Value 1: 0=Normal, 2=Fast, 3=Panorama
        /// Value 2: Sequence Number Value 3:
        /// 1 = Panorama Direction: Left to Right
        /// 2 = Panorama Direction: Right to Left
        /// 3 = Panorama Direction: Bottom to Top
        /// 4 = Panorama Direction: Top to Bottom
        /// </summary>
        public const int TagSpecialMode = unchecked((int)(0x0200));

        /// <summary>
        /// 1 = Standard Quality
        /// 2 = High Quality
        /// 3 = Super High Quality
        /// </summary>
        public const int TagJpegQuality = unchecked((int)(0x0201));

        /// <summary>
        /// 0 = Normal (Not Macro)
        /// 1 = Macro
        /// </summary>
        public const int TagMacroMode = unchecked((int)(0x0202));

        /// <summary>0 = Off, 1 = On</summary>
        public const int TagBwMode = unchecked((int)(0x0203));

        /// <summary>Zoom Factor (0 or 1 = normal)</summary>
        public const int TagDigiZoomRatio = unchecked((int)(0x0204));

        public const int TagFocalPlaneDiagonal = unchecked((int)(0x0205));

        public const int TagLensDistortionParameters = unchecked((int)(0x0206));

        public const int TagFirmwareVersion = unchecked((int)(0x0207));

        public const int TagPictInfo = unchecked((int)(0x0208));

        public const int TagCameraId = unchecked((int)(0x0209));

        /// <summary>
        /// Used by Epson cameras
        /// Units = pixels
        /// </summary>
        public const int TagImageWidth = unchecked((int)(0x020B));

        /// <summary>
        /// Used by Epson cameras
        /// Units = pixels
        /// </summary>
        public const int TagImageHeight = unchecked((int)(0x020C));

        /// <summary>A string.</summary>
        /// <remarks>A string. Used by Epson cameras.</remarks>
        public const int TagOriginalManufacturerModel = unchecked((int)(0x020D));

        public const int TagPreviewImage = unchecked((int)(0x0280));

        public const int TagPreCaptureFrames = unchecked((int)(0x0300));

        public const int TagWhiteBoard = unchecked((int)(0x0301));

        public const int TagOneTouchWb = unchecked((int)(0x0302));

        public const int TagWhiteBalanceBracket = unchecked((int)(0x0303));

        public const int TagWhiteBalanceBias = unchecked((int)(0x0304));

        public const int TagSceneMode = unchecked((int)(0x0403));

        public const int TagFirmware = unchecked((int)(0x0404));

        /// <summary>
        /// See the PIM specification here:
        /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html
        /// </summary>
        public const int TagPrintImageMatchingInfo = unchecked((int)(0x0E00));

        public const int TagDataDump1 = unchecked((int)(0x0F00));

        public const int TagDataDump2 = unchecked((int)(0x0F01));

        public const int TagShutterSpeedValue = unchecked((int)(0x1000));

        public const int TagIsoValue = unchecked((int)(0x1001));

        public const int TagApertureValue = unchecked((int)(0x1002));

        public const int TagBrightnessValue = unchecked((int)(0x1003));

        public const int TagFlashMode = unchecked((int)(0x1004));

        public const int TagFlashDevice = unchecked((int)(0x1005));

        public const int TagBracket = unchecked((int)(0x1006));

        public const int TagSensorTemperature = unchecked((int)(0x1007));

        public const int TagLensTemperature = unchecked((int)(0x1008));

        public const int TagLightCondition = unchecked((int)(0x1009));

        public const int TagFocusRange = unchecked((int)(0x100A));

        public const int TagFocusMode = unchecked((int)(0x100B));

        public const int TagFocusDistance = unchecked((int)(0x100C));

        public const int TagZoom = unchecked((int)(0x100D));

        public const int TagMacroFocus = unchecked((int)(0x100E));

        public const int TagSharpness = unchecked((int)(0x100F));

        public const int TagFlashChargeLevel = unchecked((int)(0x1010));

        public const int TagColourMatrix = unchecked((int)(0x1011));

        public const int TagBlackLevel = unchecked((int)(0x1012));

        public const int TagWhiteBalance = unchecked((int)(0x1015));

        public const int TagRedBias = unchecked((int)(0x1017));

        public const int TagBlueBias = unchecked((int)(0x1018));

        public const int TagColorMatrixNumber = unchecked((int)(0x1019));

        public const int TagSerialNumber = unchecked((int)(0x101A));

        public const int TagFlashBias = unchecked((int)(0x1023));

        public const int TagExternalFlashBounce = unchecked((int)(0x1026));

        public const int TagExternalFlashZoom = unchecked((int)(0x1027));

        public const int TagExternalFlashMode = unchecked((int)(0x1028));

        public const int TagContrast = unchecked((int)(0x1029));

        public const int TagSharpnessFactor = unchecked((int)(0x102A));

        public const int TagColourControl = unchecked((int)(0x102B));

        public const int TagValidBits = unchecked((int)(0x102C));

        public const int TagCoringFilter = unchecked((int)(0x102D));

        public const int TagFinalWidth = unchecked((int)(0x102E));

        public const int TagFinalHeight = unchecked((int)(0x102F));

        public const int TagCompressionRatio = unchecked((int)(0x1034));

        public const int TagThumbnail = unchecked((int)(0x1035));

        public const int TagThumbnailOffset = unchecked((int)(0x1036));

        public const int TagThumbnailLength = unchecked((int)(0x1037));

        public const int TagCcdScanMode = unchecked((int)(0x1039));

        public const int TagNoiseReduction = unchecked((int)(0x103A));

        public const int TagInfinityLensStep = unchecked((int)(0x103B));

        public const int TagNearLensStep = unchecked((int)(0x103C));

        public const int TagEquipment = unchecked((int)(0x2010));

        public const int TagCameraSettings = unchecked((int)(0x2020));

        public const int TagRawDevelopment = unchecked((int)(0x2030));

        public const int TagRawDevelopment2 = unchecked((int)(0x2031));

        public const int TagImageProcessing = unchecked((int)(0x2040));

        public const int TagFocusInfo = unchecked((int)(0x2050));

        public const int TagRawInfo = unchecked((int)(0x3000));

        public static class CameraSettings
        {
            internal const int Offset = unchecked((int)(0xF000));

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
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static OlympusMakernoteDirectory()
        {
            TagNameMap.Put(TagMakernoteVersion, "Makernote Version");
            TagNameMap.Put(TagCameraSettings1, "Camera Settings");
            TagNameMap.Put(TagCameraSettings2, "Camera Settings");
            TagNameMap.Put(TagCompressedImageSize, "Compressed Image Size");
            TagNameMap.Put(TagMinoltaThumbnailOffset1, "Thumbnail Offset");
            TagNameMap.Put(TagMinoltaThumbnailOffset2, "Thumbnail Offset");
            TagNameMap.Put(TagMinoltaThumbnailLength, "Thumbnail Length");
            TagNameMap.Put(TagThumbnailImage, "Thumbnail Image");
            TagNameMap.Put(TagColourMode, "Colour Mode");
            TagNameMap.Put(TagImageQuality1, "Image Quality");
            TagNameMap.Put(TagImageQuality2, "Image Quality");
            TagNameMap.Put(TagBodyFirmwareVersion, "Body Firmware Version");
            TagNameMap.Put(TagSpecialMode, "Special Mode");
            TagNameMap.Put(TagJpegQuality, "JPEG Quality");
            TagNameMap.Put(TagMacroMode, "Macro");
            TagNameMap.Put(TagBwMode, "BW Mode");
            TagNameMap.Put(TagDigiZoomRatio, "DigiZoom Ratio");
            TagNameMap.Put(TagFocalPlaneDiagonal, "Focal Plane Diagonal");
            TagNameMap.Put(TagLensDistortionParameters, "Lens Distortion Parameters");
            TagNameMap.Put(TagFirmwareVersion, "Firmware Version");
            TagNameMap.Put(TagPictInfo, "Pict Info");
            TagNameMap.Put(TagCameraId, "Camera Id");
            TagNameMap.Put(TagImageWidth, "Image Width");
            TagNameMap.Put(TagImageHeight, "Image Height");
            TagNameMap.Put(TagOriginalManufacturerModel, "Original Manufacturer Model");
            TagNameMap.Put(TagPreviewImage, "Preview Image");
            TagNameMap.Put(TagPreCaptureFrames, "Pre Capture Frames");
            TagNameMap.Put(TagWhiteBoard, "White Board");
            TagNameMap.Put(TagOneTouchWb, "One Touch WB");
            TagNameMap.Put(TagWhiteBalanceBracket, "White Balance Bracket");
            TagNameMap.Put(TagWhiteBalanceBias, "White Balance Bias");
            TagNameMap.Put(TagSceneMode, "Scene Mode");
            TagNameMap.Put(TagFirmware, "Firmware");
            TagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info");
            TagNameMap.Put(TagDataDump1, "Data Dump");
            TagNameMap.Put(TagDataDump2, "Data Dump 2");
            TagNameMap.Put(TagShutterSpeedValue, "Shutter Speed Value");
            TagNameMap.Put(TagIsoValue, "ISO Value");
            TagNameMap.Put(TagApertureValue, "Aperture Value");
            TagNameMap.Put(TagBrightnessValue, "Brightness Value");
            TagNameMap.Put(TagFlashMode, "Flash Mode");
            TagNameMap.Put(TagFlashDevice, "Flash Device");
            TagNameMap.Put(TagBracket, "Bracket");
            TagNameMap.Put(TagSensorTemperature, "Sensor Temperature");
            TagNameMap.Put(TagLensTemperature, "Lens Temperature");
            TagNameMap.Put(TagLightCondition, "Light Condition");
            TagNameMap.Put(TagFocusRange, "Focus Range");
            TagNameMap.Put(TagFocusMode, "Focus Mode");
            TagNameMap.Put(TagFocusDistance, "Focus Distance");
            TagNameMap.Put(TagZoom, "Zoom");
            TagNameMap.Put(TagMacroFocus, "Macro Focus");
            TagNameMap.Put(TagSharpness, "Sharpness");
            TagNameMap.Put(TagFlashChargeLevel, "Flash Charge Level");
            TagNameMap.Put(TagColourMatrix, "Colour Matrix");
            TagNameMap.Put(TagBlackLevel, "Black Level");
            TagNameMap.Put(TagWhiteBalance, "White Balance");
            TagNameMap.Put(TagRedBias, "Red Bias");
            TagNameMap.Put(TagBlueBias, "Blue Bias");
            TagNameMap.Put(TagColorMatrixNumber, "Color Matrix Number");
            TagNameMap.Put(TagSerialNumber, "Serial Number");
            TagNameMap.Put(TagFlashBias, "Flash Bias");
            TagNameMap.Put(TagExternalFlashBounce, "External Flash Bounce");
            TagNameMap.Put(TagExternalFlashZoom, "External Flash Zoom");
            TagNameMap.Put(TagExternalFlashMode, "External Flash Mode");
            TagNameMap.Put(TagContrast, "Contrast");
            TagNameMap.Put(TagSharpnessFactor, "Sharpness Factor");
            TagNameMap.Put(TagColourControl, "Colour Control");
            TagNameMap.Put(TagValidBits, "Valid Bits");
            TagNameMap.Put(TagCoringFilter, "Coring Filter");
            TagNameMap.Put(TagFinalWidth, "Final Width");
            TagNameMap.Put(TagFinalHeight, "Final Height");
            TagNameMap.Put(TagCompressionRatio, "Compression Ratio");
            TagNameMap.Put(TagThumbnail, "Thumbnail");
            TagNameMap.Put(TagThumbnailOffset, "Thumbnail Offset");
            TagNameMap.Put(TagThumbnailLength, "Thumbnail Length");
            TagNameMap.Put(TagCcdScanMode, "CCD Scan Mode");
            TagNameMap.Put(TagNoiseReduction, "Noise Reduction");
            TagNameMap.Put(TagInfinityLensStep, "Infinity Lens Step");
            TagNameMap.Put(TagNearLensStep, "Near Lens Step");
            TagNameMap.Put(TagEquipment, "Equipment");
            TagNameMap.Put(TagCameraSettings, "Camera Settings");
            TagNameMap.Put(TagRawDevelopment, "Raw Development");
            TagNameMap.Put(TagRawDevelopment2, "Raw Development 2");
            TagNameMap.Put(TagImageProcessing, "Image Processing");
            TagNameMap.Put(TagFocusInfo, "Focus Info");
            TagNameMap.Put(TagRawInfo, "Raw Info");
            TagNameMap.Put(CameraSettings.TagExposureMode, "Exposure Mode");
            TagNameMap.Put(CameraSettings.TagFlashMode, "Flash Mode");
            TagNameMap.Put(CameraSettings.TagWhiteBalance, "White Balance");
            TagNameMap.Put(CameraSettings.TagImageSize, "Image Size");
            TagNameMap.Put(CameraSettings.TagImageQuality, "Image Quality");
            TagNameMap.Put(CameraSettings.TagShootingMode, "Shooting Mode");
            TagNameMap.Put(CameraSettings.TagMeteringMode, "Metering Mode");
            TagNameMap.Put(CameraSettings.TagApexFilmSpeedValue, "Apex Film Speed Value");
            TagNameMap.Put(CameraSettings.TagApexShutterSpeedTimeValue, "Apex Shutter Speed Time Value");
            TagNameMap.Put(CameraSettings.TagApexApertureValue, "Apex Aperture Value");
            TagNameMap.Put(CameraSettings.TagMacroMode, "Macro Mode");
            TagNameMap.Put(CameraSettings.TagDigitalZoom, "Digital Zoom");
            TagNameMap.Put(CameraSettings.TagExposureCompensation, "Exposure Compensation");
            TagNameMap.Put(CameraSettings.TagBracketStep, "Bracket Step");
            TagNameMap.Put(CameraSettings.TagIntervalLength, "Interval Length");
            TagNameMap.Put(CameraSettings.TagIntervalNumber, "Interval Number");
            TagNameMap.Put(CameraSettings.TagFocalLength, "Focal Length");
            TagNameMap.Put(CameraSettings.TagFocusDistance, "Focus Distance");
            TagNameMap.Put(CameraSettings.TagFlashFired, "Flash Fired");
            TagNameMap.Put(CameraSettings.TagDate, "Date");
            TagNameMap.Put(CameraSettings.TagTime, "Time");
            TagNameMap.Put(CameraSettings.TagMaxApertureAtFocalLength, "Max Aperture at Focal Length");
            TagNameMap.Put(CameraSettings.TagFileNumberMemory, "File Number Memory");
            TagNameMap.Put(CameraSettings.TagLastFileNumber, "Last File Number");
            TagNameMap.Put(CameraSettings.TagWhiteBalanceRed, "White Balance Red");
            TagNameMap.Put(CameraSettings.TagWhiteBalanceGreen, "White Balance Green");
            TagNameMap.Put(CameraSettings.TagWhiteBalanceBlue, "White Balance Blue");
            TagNameMap.Put(CameraSettings.TagSaturation, "Saturation");
            TagNameMap.Put(CameraSettings.TagContrast, "Contrast");
            TagNameMap.Put(CameraSettings.TagSharpness, "Sharpness");
            TagNameMap.Put(CameraSettings.TagSubjectProgram, "Subject Program");
            TagNameMap.Put(CameraSettings.TagFlashCompensation, "Flash Compensation");
            TagNameMap.Put(CameraSettings.TagIsoSetting, "ISO Setting");
            TagNameMap.Put(CameraSettings.TagCameraModel, "Camera Model");
            TagNameMap.Put(CameraSettings.TagIntervalMode, "Interval Mode");
            TagNameMap.Put(CameraSettings.TagFolderName, "Folder Name");
            TagNameMap.Put(CameraSettings.TagColorMode, "Color Mode");
            TagNameMap.Put(CameraSettings.TagColorFilter, "Color Filter");
            TagNameMap.Put(CameraSettings.TagBlackAndWhiteFilter, "Black and White Filter");
            TagNameMap.Put(CameraSettings.TagInternalFlash, "Internal Flash");
            TagNameMap.Put(CameraSettings.TagApexBrightnessValue, "Apex Brightness Value");
            TagNameMap.Put(CameraSettings.TagSpotFocusPointXCoordinate, "Spot Focus Point X Coordinate");
            TagNameMap.Put(CameraSettings.TagSpotFocusPointYCoordinate, "Spot Focus Point Y Coordinate");
            TagNameMap.Put(CameraSettings.TagWideFocusZone, "Wide Focus Zone");
            TagNameMap.Put(CameraSettings.TagFocusMode, "Focus Mode");
            TagNameMap.Put(CameraSettings.TagFocusArea, "Focus Area");
            TagNameMap.Put(CameraSettings.TagDecSwitchPosition, "DEC Switch Position");
        }

        public OlympusMakernoteDirectory()
        {
            this.SetDescriptor(new OlympusMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Olympus Makernote";
        }

        public override void SetByteArray(int tagType, [NotNull] sbyte[] bytes)
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

        private void ProcessCameraSettings(sbyte[] bytes)
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
                Runtime.PrintStackTrace(e);
            }
        }

        public virtual bool IsIntervalMode()
        {
            long? value = GetLongObject(CameraSettings.TagShootingMode);
            return value != null && value == 5;
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
