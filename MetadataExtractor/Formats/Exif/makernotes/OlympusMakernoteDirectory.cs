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
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// The Olympus makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

        ///// <summary>Zoom Factor (0 or 1 = normal)</summary>
        public const int TagDigitalZoom = 0x0204;

        public const int TagFocalPlaneDiagonal = 0x0205;
        public const int TagLensDistortionParameters = 0x0206;
        public const int TagCameraType = 0x0207;
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
        public const int TagSerialNumber1 = 0x0404;
        public const int TagFirmware = 0x0405;

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

        public const int TagColorTemperatureBG = 0x1013;
        public const int TagColorTemperatureRG = 0x1014;
        public const int TagWbMode = 0x1015;

        public const int TagRedBalance = 0x1017;
        public const int TagBlueBalance = 0x1018;
        public const int TagColorMatrixNumber = 0x1019;
        public const int TagSerialNumber2 = 0x101A;

        public const int TagExternalFlashAE1_0 = 0x101B;
        public const int TagExternalFlashAE2_0 = 0x101C;
        public const int TagInternalFlashAE1_0 = 0x101D;
        public const int TagInternalFlashAE2_0 = 0x101E;
        public const int TagExternalFlashAE1 = 0x101F;
        public const int TagExternalFlashAE2 = 0x1020;
        public const int TagInternalFlashAE1 = 0x1021;
        public const int TagInternalFlashAE2 = 0x1022;

        public const int TagFlashBias = 0x1023;
        public const int TagInternalFlashTable = 0x1024;
        public const int TagExternalFlashGValue = 0x1025;
        public const int TagExternalFlashBounce = 0x1026;
        public const int TagExternalFlashZoom = 0x1027;
        public const int TagExternalFlashMode = 0x1028;
        public const int TagContrast = 0x1029;
        public const int TagSharpnessFactor = 0x102A;
        public const int TagColourControl = 0x102B;
        public const int TagValidBits = 0x102C;
        public const int TagCoringFilter = 0x102D;
        public const int TagOlympusImageWidth = 0x102E;
        public const int TagOlympusImageHeight = 0x102F;
        public const int TagSceneDetect = 0x1030;
        public const int TagSceneArea = 0x1031;
        public const int TagSceneDetectData = 0x1033;
        public const int TagCompressionRatio = 0x1034;
        public const int TagPreviewImageValid = 0x1035;
        public const int TagPreviewImageStart = 0x1036;
        public const int TagPreviewImageLength = 0x1037;
        public const int TagAfResult = 0x1038;
        public const int TagCcdScanMode = 0x1039;
        public const int TagNoiseReduction = 0x103A;
        public const int TagInfinityLensStep = 0x103B;
        public const int TagNearLensStep = 0x103C;
        public const int TagLightValueCenter = 0x103D;
        public const int TagLightValuePeriphery = 0x103E;
        public const int TagFieldCount = 0x103F;
        public const int TagEquipment = 0x2010;
        public const int TagCameraSettings = 0x2020;
        public const int TagRawDevelopment = 0x2030;
        public const int TagRawDevelopment2 = 0x2031;
        public const int TagImageProcessing = 0x2040;
        public const int TagFocusInfo = 0x2050;
        public const int TagRawInfo = 0x3000;

        public const int TagMainInfo = 0x4000;

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
            { TagDigitalZoom, "Digital Zoom" },
            { TagFocalPlaneDiagonal, "Focal Plane Diagonal" },
            { TagLensDistortionParameters, "Lens Distortion Parameters" },
            { TagCameraType, "Camera Type" },
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
            { TagSerialNumber1, "Serial Number" },
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
            { TagColorTemperatureBG, "Color Temperature BG" },
            { TagColorTemperatureRG, "Color Temperature RG" },
            { TagWbMode, "White Balance Mode" },
            { TagRedBalance, "Red Balance" },
            { TagBlueBalance, "Blue Balance" },
            { TagColorMatrixNumber, "Color Matrix Number" },
            { TagSerialNumber2, "Serial Number" },
            { TagExternalFlashAE1_0, "External Flash AE1 0" },
            { TagExternalFlashAE2_0, "External Flash AE2 0" },
            { TagInternalFlashAE1_0, "Internal Flash AE1 0" },
            { TagInternalFlashAE2_0, "Internal Flash AE2 0" },
            { TagExternalFlashAE1, "External Flash AE1" },
            { TagExternalFlashAE2, "External Flash AE2" },
            { TagInternalFlashAE1, "Internal Flash AE1" },
            { TagInternalFlashAE2, "Internal Flash AE2" },
            { TagFlashBias, "Flash Bias" },
            { TagInternalFlashTable, "Internal Flash Table" },
            { TagExternalFlashGValue, "External Flash G Value" },
            { TagExternalFlashBounce, "External Flash Bounce" },
            { TagExternalFlashZoom, "External Flash Zoom" },
            { TagExternalFlashMode, "External Flash Mode" },
            { TagContrast, "Contrast" },
            { TagSharpnessFactor, "Sharpness Factor" },
            { TagColourControl, "Colour Control" },
            { TagValidBits, "Valid Bits" },
            { TagCoringFilter, "Coring Filter" },
            { TagOlympusImageWidth, "Olympus Image Width" },
            { TagOlympusImageHeight, "Olympus Image Height" },
            { TagSceneDetect, "Scene Detect" },
            { TagSceneArea, "Scene Area" },
            { TagSceneDetectData, "Scene Detect Data" },
            { TagCompressionRatio, "Compression Ratio" },
            { TagPreviewImageValid, "Preview Image Valid" },
            { TagPreviewImageStart, "Preview Image Start" },
            { TagPreviewImageLength, "Preview Image Length" },
            { TagAfResult, "AF Result" },
            { TagCcdScanMode, "CCD Scan Mode" },
            { TagNoiseReduction, "Noise Reduction" },
            { TagInfinityLensStep, "Infinity Lens Step" },
            { TagNearLensStep, "Near Lens Step" },
            { TagLightValueCenter, "Light Value Center" },
            { TagLightValuePeriphery, "Light Value Periphery" },
            { TagFieldCount, "Field Count" },

            { TagEquipment, "Equipment" },
            { TagCameraSettings, "Camera Settings" },
            { TagRawDevelopment, "Raw Development" },
            { TagRawDevelopment2, "Raw Development 2" },
            { TagImageProcessing, "Image Processing" },
            { TagFocusInfo, "Focus Info" },
            { TagRawInfo, "Raw Info" },
            { TagMainInfo, "Main Info" },
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

        public override string Name => "Olympus Makernote";

        public override void Set(int tagType, object value)
        {

            if (value is byte[] bytes && (tagType == TagCameraSettings1 || tagType == TagCameraSettings2))
                ProcessCameraSettings(bytes);
            else
                base.Set(tagType, value);
        }

        private void ProcessCameraSettings([NotNull] byte[] bytes)
        {
            var reader = new SequentialByteArrayReader(bytes);
            var count = bytes.Length / 4;

            for (var i = 0; i < count; i++)
                Set(CameraSettings.Offset + i, reader.GetInt32());
        }

        public bool IsIntervalMode()
        {
            return this.TryGetInt64(CameraSettings.TagShootingMode, out long value) && value == 5;
        }

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <summary>
        /// These values are currently decoded only for Olympus models.  Models with
        /// Olympus-style maker notes from other brands such as Acer, BenQ, Hitachi, HP,
        /// Premier, Konica-Minolta, Maginon, Ricoh, Rollei, SeaLife, Sony, Supra,
        /// Vivitar are not listed.
        /// </summary>
        /// <remarks>
        /// Converted from Exiftool version 10.33 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\Olympus.pm
        /// </remarks>
        public static readonly Dictionary<string, string> OlympusCameraTypes = new Dictionary<string, string>
        {
            { "D4028", "X-2,C-50Z" },
            { "D4029", "E-20,E-20N,E-20P" },
            { "D4034", "C720UZ" },
            { "D4040", "E-1" },
            { "D4041", "E-300" },
            { "D4083", "C2Z,D520Z,C220Z" },
            { "D4106", "u20D,S400D,u400D" },
            { "D4120", "X-1" },
            { "D4122", "u10D,S300D,u300D" },
            { "D4125", "AZ-1" },
            { "D4141", "C150,D390" },
            { "D4193", "C-5000Z" },
            { "D4194", "X-3,C-60Z" },
            { "D4199", "u30D,S410D,u410D" },
            { "D4205", "X450,D535Z,C370Z" },
            { "D4210", "C160,D395" },
            { "D4211", "C725UZ" },
            { "D4213", "FerrariMODEL2003" },
            { "D4216", "u15D" },
            { "D4217", "u25D" },
            { "D4220", "u-miniD,Stylus V" },
            { "D4221", "u40D,S500,uD500" },
            { "D4231", "FerrariMODEL2004" },
            { "D4240", "X500,D590Z,C470Z" },
            { "D4244", "uD800,S800" },
            { "D4256", "u720SW,S720SW" },
            { "D4261", "X600,D630,FE5500" },
            { "D4262", "uD600,S600" },
            { "D4301", "u810/S810" }, // (yes, "/".  Olympus is not consistent in the notation)
            { "D4302", "u710,S710" },
            { "D4303", "u700,S700" },
            { "D4304", "FE100,X710" },
            { "D4305", "FE110,X705" },
            { "D4310", "FE-130,X-720" },
            { "D4311", "FE-140,X-725" },
            { "D4312", "FE150,X730" },
            { "D4313", "FE160,X735" },
            { "D4314", "u740,S740" },
            { "D4315", "u750,S750" },
            { "D4316", "u730/S730" },
            { "D4317", "FE115,X715" },
            { "D4321", "SP550UZ" },
            { "D4322", "SP510UZ" },
            { "D4324", "FE170,X760" },
            { "D4326", "FE200" },
            { "D4327", "FE190/X750" }, // (also SX876)
            { "D4328", "u760,S760" },
            { "D4330", "FE180/X745" }, // (also SX875)
            { "D4331", "u1000/S1000" },
            { "D4332", "u770SW,S770SW" },
            { "D4333", "FE240/X795" },
            { "D4334", "FE210,X775" },
            { "D4336", "FE230/X790" },
            { "D4337", "FE220,X785" },
            { "D4338", "u725SW,S725SW" },
            { "D4339", "FE250/X800" },
            { "D4341", "u780,S780" },
            { "D4343", "u790SW,S790SW" },
            { "D4344", "u1020,S1020" },
            { "D4346", "FE15,X10" },
            { "D4348", "FE280,X820,C520" },
            { "D4349", "FE300,X830" },
            { "D4350", "u820,S820" },
            { "D4351", "u1200,S1200" },
            { "D4352", "FE270,X815,C510" },
            { "D4353", "u795SW,S795SW" },
            { "D4354", "u1030SW,S1030SW" },
            { "D4355", "SP560UZ" },
            { "D4356", "u1010,S1010" },
            { "D4357", "u830,S830" },
            { "D4359", "u840,S840" },
            { "D4360", "FE350WIDE,X865" },
            { "D4361", "u850SW,S850SW" },
            { "D4362", "FE340,X855,C560" },
            { "D4363", "FE320,X835,C540" },
            { "D4364", "SP570UZ" },
            { "D4366", "FE330,X845,C550" },
            { "D4368", "FE310,X840,C530" },
            { "D4370", "u1050SW,S1050SW" },
            { "D4371", "u1060,S1060" },
            { "D4372", "FE370,X880,C575" },
            { "D4374", "SP565UZ" },
            { "D4377", "u1040,S1040" },
            { "D4378", "FE360,X875,C570" },
            { "D4379", "FE20,X15,C25" },
            { "D4380", "uT6000,ST6000" },
            { "D4381", "uT8000,ST8000" },
            { "D4382", "u9000,S9000" },
            { "D4384", "SP590UZ" },
            { "D4385", "FE3010,X895" },
            { "D4386", "FE3000,X890" },
            { "D4387", "FE35,X30" },
            { "D4388", "u550WP,S550WP" },
            { "D4390", "FE5000,X905" },
            { "D4391", "u5000" },
            { "D4392", "u7000,S7000" },
            { "D4396", "FE5010,X915" },
            { "D4397", "FE25,X20" },
            { "D4398", "FE45,X40" },
            { "D4401", "XZ-1" },
            { "D4402", "uT6010,ST6010" },
            { "D4406", "u7010,S7010 / u7020,S7020" },
            { "D4407", "FE4010,X930" },
            { "D4408", "X560WP" },
            { "D4409", "FE26,X21" },
            { "D4410", "FE4000,X920,X925" },
            { "D4411", "FE46,X41,X42" },
            { "D4412", "FE5020,X935" },
            { "D4413", "uTough-3000" },
            { "D4414", "StylusTough-6020" },
            { "D4415", "StylusTough-8010" },
            { "D4417", "u5010,S5010" },
            { "D4418", "u7040,S7040" },
            { "D4419", "u9010,S9010" },
            { "D4423", "FE4040" },
            { "D4424", "FE47,X43" },
            { "D4426", "FE4030,X950" },
            { "D4428", "FE5030,X965,X960" },
            { "D4430", "u7030,S7030" },
            { "D4432", "SP600UZ" },
            { "D4434", "SP800UZ" },
            { "D4439", "FE4020,X940" },
            { "D4442", "FE5035" },
            { "D4448", "FE4050,X970" },
            { "D4450", "FE5050,X985" },
            { "D4454", "u-7050" },
            { "D4464", "T10,X27" },
            { "D4470", "FE5040,X980" },
            { "D4472", "TG-310" },
            { "D4474", "TG-610" },
            { "D4476", "TG-810" },
            { "D4478", "VG145,VG140,D715" },
            { "D4479", "VG130,D710" },
            { "D4480", "VG120,D705" },
            { "D4482", "VR310,D720" },
            { "D4484", "VR320,D725" },
            { "D4486", "VR330,D730" },
            { "D4488", "VG110,D700" },
            { "D4490", "SP-610UZ" },
            { "D4492", "SZ-10" },
            { "D4494", "SZ-20" },
            { "D4496", "SZ-30MR" },
            { "D4498", "SP-810UZ" },
            { "D4500", "SZ-11" },
            { "D4504", "TG-615" },
            { "D4508", "TG-620" },
            { "D4510", "TG-820" },
            { "D4512", "TG-1" },
            { "D4516", "SH-21" },
            { "D4519", "SZ-14" },
            { "D4520", "SZ-31MR" },
            { "D4521", "SH-25MR" },
            { "D4523", "SP-720UZ" },
            { "D4529", "VG170" },
            { "D4531", "XZ-2" },
            { "D4535", "SP-620UZ" },
            { "D4536", "TG-320" },
            { "D4537", "VR340,D750" },
            { "D4538", "VG160,X990,D745" },
            { "D4541", "SZ-12" },
            { "D4545", "VH410" },
            { "D4546", "XZ-10" }, //IB
            { "D4547", "TG-2" },
            { "D4548", "TG-830" },
            { "D4549", "TG-630" },
            { "D4550", "SH-50" },
            { "D4553", "SZ-16,DZ-105" },
            { "D4562", "SP-820UZ" },
            { "D4566", "SZ-15" },
            { "D4572", "STYLUS1" },
            { "D4574", "TG-3" },
            { "D4575", "TG-850" },
            { "D4579", "SP-100EE" },
            { "D4580", "SH-60" },
            { "D4581", "SH-1" },
            { "D4582", "TG-835" },
            { "D4585", "SH-2 / SH-3" },
            { "D4586", "TG-4" },
            { "D4587", "TG-860" },
            { "D4591", "TG-870" },
            { "D4809", "C2500L" },
            { "D4842", "E-10" },
            { "D4856", "C-1" },
            { "D4857", "C-1Z,D-150Z" },
            { "DCHC", "D500L" },
            { "DCHT", "D600L / D620L" },
            { "K0055", "AIR-A01" },
            { "S0003", "E-330" },
            { "S0004", "E-500" },
            { "S0009", "E-400" },
            { "S0010", "E-510" },
            { "S0011", "E-3" },
            { "S0013", "E-410" },
            { "S0016", "E-420" },
            { "S0017", "E-30" },
            { "S0018", "E-520" },
            { "S0019", "E-P1" },
            { "S0023", "E-620" },
            { "S0026", "E-P2" },
            { "S0027", "E-PL1" },
            { "S0029", "E-450" },
            { "S0030", "E-600" },
            { "S0032", "E-P3" },
            { "S0033", "E-5" },
            { "S0034", "E-PL2" },
            { "S0036", "E-M5" },
            { "S0038", "E-PL3" },
            { "S0039", "E-PM1" },
            { "S0040", "E-PL1s" },
            { "S0042", "E-PL5" },
            { "S0043", "E-PM2" },
            { "S0044", "E-P5" },
            { "S0045", "E-PL6" },
            { "S0046", "E-PL7" }, //IB
            { "S0047", "E-M1" },
            { "S0051", "E-M10" },
            { "S0052", "E-M5MarkII" }, //IB
            { "S0059", "E-M10MarkII" },
            { "S0061", "PEN-F" }, //forum7005
            { "S0065", "E-PL8" },
            { "S0067", "E-M1MarkII" },
            { "SR45", "D220" },
            { "SR55", "D320L" },
            { "SR83", "D340L" },
            { "SR85", "C830L,D340R" },
            { "SR852", "C860L,D360L" },
            { "SR872", "C900Z,D400Z" },
            { "SR874", "C960Z,D460Z" },
            { "SR951", "C2000Z" },
            { "SR952", "C21" },
            { "SR953", "C21T.commu" },
            { "SR954", "C2020Z" },
            { "SR955", "C990Z,D490Z" },
            { "SR956", "C211Z" },
            { "SR959", "C990ZS,D490Z" },
            { "SR95A", "C2100UZ" },
            { "SR971", "C100,D370" },
            { "SR973", "C2,D230" },
            { "SX151", "E100RS" },
            { "SX351", "C3000Z / C3030Z" },
            { "SX354", "C3040Z" },
            { "SX355", "C2040Z" },
            { "SX357", "C700UZ" },
            { "SX358", "C200Z,D510Z" },
            { "SX374", "C3100Z,C3020Z" },
            { "SX552", "C4040Z" },
            { "SX553", "C40Z,D40Z" },
            { "SX556", "C730UZ" },
            { "SX558", "C5050Z" },
            { "SX571", "C120,D380" },
            { "SX574", "C300Z,D550Z" },
            { "SX575", "C4100Z,C4000Z" },
            { "SX751", "X200,D560Z,C350Z" },
            { "SX752", "X300,D565Z,C450Z" },
            { "SX753", "C750UZ" },
            { "SX754", "C740UZ" },
            { "SX755", "C755UZ" },
            { "SX756", "C5060WZ" },
            { "SX757", "C8080WZ" },
            { "SX758", "X350,D575Z,C360Z" },
            { "SX759", "X400,D580Z,C460Z" },
            { "SX75A", "AZ-2ZOOM" },
            { "SX75B", "D595Z,C500Z" },
            { "SX75C", "X550,D545Z,C480Z" },
            { "SX75D", "IR-300" },
            { "SX75F", "C55Z,C5500Z" },
            { "SX75G", "C170,D425" },
            { "SX75J", "C180,D435" },
            { "SX771", "C760UZ" },
            { "SX772", "C770UZ" },
            { "SX773", "C745UZ" },
            { "SX774", "X250,D560Z,C350Z" },
            { "SX775", "X100,D540Z,C310Z" },
            { "SX776", "C460ZdelSol" },
            { "SX777", "C765UZ" },
            { "SX77A", "D555Z,C315Z" },
            { "SX851", "C7070WZ" },
            { "SX852", "C70Z,C7000Z" },
            { "SX853", "SP500UZ" },
            { "SX854", "SP310" },
            { "SX855", "SP350" },
            { "SX873", "SP320" },
            { "SX875", "FE180/X745" }, // (also D4330)
            { "SX876", "FE190/X750" } // (also D4327)

            //   other brands
            //    4MP9Q3", "Camera 4MP-9Q3'
            //    4MP9T2", "BenQ DC C420 / Camera 4MP-9T2'
            //    5MP9Q3", "Camera 5MP-9Q3" },
            //    5MP9X9", "Camera 5MP-9X9" },
            //   '5MP-9T'=> 'Camera 5MP-9T3" },
            //   '5MP-9Y'=> 'Camera 5MP-9Y2" },
            //   '6MP-9U'=> 'Camera 6MP-9U9" },
            //    7MP9Q3", "Camera 7MP-9Q3" },
            //   '8MP-9U'=> 'Camera 8MP-9U4" },
            //    CE5330", "Acer CE-5330" },
            //   'CP-853'=> 'Acer CP-8531" },
            //    CS5531", "Acer CS5531" },
            //    DC500 ", "SeaLife DC500" },
            //    DC7370", "Camera 7MP-9GA" },
            //    DC7371", "Camera 7MP-9GM" },
            //    DC7371", "Hitachi HDC-751E" },
            //    DC7375", "Hitachi HDC-763E / Rollei RCP-7330X / Ricoh Caplio RR770 / Vivitar ViviCam 7330" },
            //   'DC E63'=> 'BenQ DC E63+" },
            //   'DC P86'=> 'BenQ DC P860" },
            //    DS5340", "Maginon Performic S5 / Premier 5MP-9M7" },
            //    DS5341", "BenQ E53+ / Supra TCM X50 / Maginon X50 / Premier 5MP-9P8" },
            //    DS5346", "Premier 5MP-9Q2" },
            //    E500  ", "Konica Minolta DiMAGE E500" },
            //    MAGINO", "Maginon X60" },
            //    Mz60  ", "HP Photosmart Mz60" },
            //    Q3DIGI", "Camera 5MP-9Q3" },
            //    SLIMLI", "Supra Slimline X6" },
            //    V8300s", "Vivitar V8300s" },
        };

    }
}
