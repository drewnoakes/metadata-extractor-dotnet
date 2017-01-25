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

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Canon cameras.</summary>
    /// <remarks>
    /// Describes tags specific to Canon cameras.
    /// Thanks to Bill Richards for his contribution to this makernote directory.
    /// Many tag definitions explained here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/canon_mn.html
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class CanonMakernoteDirectory : Directory
    {
        // These TAG_*_ARRAY Exif tags map to arrays of int16 values which are split out into separate 'fake' tags.
        // When an attempt is made to set one of these on the directory, it is split and the corresponding offset added to the tagType.
        // So the resulting tag is the offset + the index into the array.
        private const int TagCameraSettingsArray        = 0x0001;
        private const int TagFocalLengthArray           = 0x0002;
//      private const int TagFlashInfo                  = 0x0003;
        private const int TagShotInfoArray              = 0x0004;
        private const int TagPanoramaArray              = 0x0005;

        public const int TagCanonFirmwareVersion        = 0x0007;
        public const int TagCanonImageNumber            = 0x0008;
        public const int TagCanonOwnerName              = 0x0009;
        public const int TagCanonSerialNumber           = 0x000C;
        public const int TagCameraInfoArray             = 0x000D;
        public const int TagCanonFileLength             = 0x000E;
        public const int TagsArray                      = 0x000F;
        public const int TagModelId                     = 0x0010;
        public const int TagMovieInfoArray              = 0x0011;
        private const int TagAfInfoArray                = 0x0012;
        public const int TagThumbnailImageValidArea     = 0x0013;
        public const int TagSerialNumberFormat          = 0x0015;
        public const int TagSuperMacro                  = 0x001A;
        public const int TagDateStampMode               = 0x001C;
        public const int TagMyColors                    = 0x001D;
        public const int TagFirmwareRevision            = 0x001E;
        public const int TagCategories                  = 0x0023;
        public const int TagFaceDetectArray1            = 0x0024;
        public const int TagFaceDetectArray2            = 0x0025;
        public const int TagAfInfoArray2                = 0x0026;
        public const int TagImageUniqueId               = 0x0028;
        public const int TagRawDataOffset               = 0x0081;
        public const int TagOriginalDecisionDataOffset  = 0x0083;
        public const int TagCustomFunctions1DArray      = 0x0090;
        public const int TagPersonalFunctionsArray      = 0x0091;
        public const int TagPersonalFunctionValuesArray = 0x0092;
        public const int TagFileInfoArray               = 0x0093;
        public const int TagAfPointsInFocus1D           = 0x0094;
        public const int TagLensModel                   = 0x0095;
        public const int TagSerialInfoArray             = 0x0096;
        public const int TagDustRemovalData             = 0x0097;
        public const int TagCropInfo                    = 0x0098;
        public const int TagCustomFunctionsArray2       = 0x0099;
        public const int TagAspectInfoArray             = 0x009A;
        public const int TagProcessingInfoArray         = 0x00A0;
        public const int TagToneCurveTable              = 0x00A1;
        public const int TagSharpnessTable              = 0x00A2;
        public const int TagSharpnessFreqTable          = 0x00A3;
        public const int TagWhiteBalanceTable           = 0x00A4;
        public const int TagColorBalanceArray           = 0x00A9;
        public const int TagMeasuredColorArray          = 0x00AA;
        public const int TagColorTemperature            = 0x00AE;
        public const int TagCanonFlagsArray             = 0x00B0;
        public const int TagModifiedInfoArray           = 0x00B1;
        public const int TagToneCurveMatching           = 0x00B2;
        public const int TagWhiteBalanceMatching        = 0x00B3;
        public const int TagColorSpace                  = 0x00B4;
        public const int TagPreviewImageInfoArray       = 0x00B6;
        public const int TagVrdOffset                   = 0x00D0;
        public const int TagSensorInfoArray             = 0x00E0;
        public const int TagColorDataArray2             = 0x4001;
        public const int TagCrwParam                    = 0x4002;
        public const int TagColorInfoArray2             = 0x4003;
        public const int TagBlackLevel                  = 0x4008;
        public const int TagCustomPictureStyleFileName  = 0x4010;
        public const int TagColorInfoArray              = 0x4013;
        public const int TagVignettingCorrectionArray1  = 0x4015;
        public const int TagVignettingCorrectionArray2  = 0x4016;
        public const int TagLightingOptimizerArray      = 0x4018;
        public const int TagLensInfoArray               = 0x4019;
        public const int TagAmbianceInfoArray           = 0x4020;
        public const int TagCanonImageType              = 0x0006;
        public const int TagFilterInfoArray             = 0x4024;

        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        public static class CameraSettings
        {
            internal const int Offset = 0xC100;

            /// <summary>
            /// 1 = Macro
            /// 2 = Normal
            /// </summary>
            public const int TagMacroMode = Offset + 0x01;

            public const int TagSelfTimerDelay = Offset + 0x02;

            /// <summary>
            /// 2 = Normal
            /// 3 = Fine
            /// 5 = Superfine
            /// </summary>
            public const int TagQuality = Offset + 0x03;

            /// <summary>
            /// 0 = Flash Not Fired
            /// 1 = Auto
            /// 2 = On
            /// 3 = Red Eye Reduction
            /// 4 = Slow Synchro
            /// 5 = Auto + Red Eye Reduction
            /// 6 = On + Red Eye Reduction
            /// 16 = External Flash
            /// </summary>
            public const int TagFlashMode = Offset + 0x04;

            /// <summary>
            /// 0 = Single Frame or Timer Mode
            /// 1 = Continuous
            /// </summary>
            public const int TagContinuousDriveMode = Offset + 0x05;

            public const int TagUnknown2 = Offset + 0x06;

            /// <summary>
            /// 0 = One-Shot
            /// 1 = AI Servo
            /// 2 = AI Focus
            /// 3 = Manual Focus
            /// 4 = Single
            /// 5 = Continuous
            /// 6 = Manual Focus
            /// </summary>
            public const int TagFocusMode1 = Offset + 0x07;

            public const int TagUnknown3 = Offset + 0x08;
            public const int TagRecordMode = Offset + 0x09;

            /// <summary>
            /// 0 = Large
            /// 1 = Medium
            /// 2 = Small
            /// </summary>
            public const int TagImageSize = Offset + 0x0A;

            /// <summary>
            /// 0 = Full Auto
            /// 1 = Manual
            /// 2 = Landscape
            /// 3 = Fast Shutter
            /// 4 = Slow Shutter
            /// 5 = Night
            /// 6 = Black &amp; White
            /// 7 = Sepia
            /// 8 = Portrait
            /// 9 = Sports
            /// 10 = Macro / Close-Up
            /// 11 = Pan Focus
            /// </summary>
            public const int TagEasyShootingMode = Offset + 0x0B;

            /// <summary>
            /// 0 = No Digital Zoom
            /// 1 = 2x
            /// 2 = 4x
            /// </summary>
            public const int TagDigitalZoom = Offset + 0x0C;

            /// <summary>
            /// 0 = Normal
            /// 1 = High
            /// 65535 = Low
            /// </summary>
            public const int TagContrast = Offset + 0x0D;

            /// <summary>
            /// 0 = Normal
            /// 1 = High
            /// 65535 = Low
            /// </summary>
            public const int TagSaturation = Offset + 0x0E;

            /// <summary>
            /// 0 = Normal
            /// 1 = High
            /// 65535 = Low
            /// </summary>
            public const int TagSharpness = Offset + 0x0F;

            /// <summary>
            /// 0 = Check ISOSpeedRatings EXIF tag for ISO Speed
            /// 15 = Auto ISO
            /// 16 = ISO 50
            /// 17 = ISO 100
            /// 18 = ISO 200
            /// 19 = ISO 400
            /// </summary>
            public const int TagIso = Offset + 0x10;

            /// <summary>
            /// 3 = Evaluative
            /// 4 = Partial
            /// 5 = Centre Weighted
            /// </summary>
            public const int TagMeteringMode = Offset + 0x11;

            /// <summary>
            /// 0 = Manual
            /// 1 = Auto
            /// 3 = Close-up (Macro)
            /// 8 = Locked (Pan Mode)
            /// </summary>
            public const int TagFocusType = Offset + 0x12;

            /// <summary>
            /// 12288 = None (Manual Focus)
            /// 12289 = Auto Selected
            /// 12290 = Right
            /// 12291 = Centre
            /// 12292 = Left
            /// </summary>
            public const int TagAfPointSelected = Offset + 0x13;

            /// <summary>
            /// 0 = Easy Shooting (See Easy Shooting Mode)
            /// 1 = Program
            /// 2 = Tv-Priority
            /// 3 = Av-Priority
            /// 4 = Manual
            /// 5 = A-DEP
            /// </summary>
            public const int TagExposureMode = Offset + 0x14;

            public const int TagUnknown7 = Offset + 0x15;
            public const int TagLensType = Offset + 0x16;
            public const int TagLongFocalLength = Offset + 0x17;
            public const int TagShortFocalLength = Offset + 0x18;
            public const int TagFocalUnitsPerMm = Offset + 0x19;
            public const int TagMaxAperture = Offset + 0x1A;
            public const int TagMinAperture = Offset + 0x1B;

            /// <summary>
            /// 0 = Flash Did Not Fire
            /// 1 = Flash Fired
            /// </summary>
            public const int TagFlashActivity = Offset + 0x1C;

            public const int TagFlashDetails = Offset + 0x1D;
            public const int TagFocusContinuous = Offset + 0x1E;
            public const int TagAESetting = Offset + 0x1F;

            /// <summary>
            /// 0 = Focus Mode: Single
            /// 1 = Focus Mode: Continuous
            /// </summary>
            public const int TagFocusMode2 = Offset + 0x20;

            public const int TagDisplayAperture = Offset + 0x21;
            public const int TagZoomSourceWidth = Offset + 0x22;
            public const int TagZoomTargetWidth = Offset + 0x23;

            public const int TagSpotMeteringMode = Offset + 0x25;
            public const int TagPhotoEffect = Offset + 0x26;
            public const int TagManualFlashOutput = Offset + 0x27;

            public const int TagColorTone = Offset + 0x29;
            public const int TagSRawQuality = Offset + 0x2D;
        }

        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        public static class FocalLength
        {
            internal const int Offset = 0xC200;

            /// <summary>
            /// 0 = Auto
            /// 1 = Sunny
            /// 2 = Cloudy
            /// 3 = Tungsten
            /// 4 = Florescent
            /// 5 = Flash
            /// 6 = Custom
            /// </summary>
            public const int TagWhiteBalance = Offset + 0x07;

            public const int TagSequenceNumber = Offset + 0x09;
            public const int TagAfPointUsed = Offset + 0x0E;

            /// <summary>The value of this tag may be translated into a flash bias value, in EV.</summary>
            /// <remarks>
            /// The value of this tag may be translated into a flash bias value, in EV.
            /// 0xffc0 = -2 EV
            /// 0xffcc = -1.67 EV
            /// 0xffd0 = -1.5 EV
            /// 0xffd4 = -1.33 EV
            /// 0xffe0 = -1 EV
            /// 0xffec = -0.67 EV
            /// 0xfff0 = -0.5 EV
            /// 0xfff4 = -0.33 EV
            /// 0x0000 = 0 EV
            /// 0x000c = 0.33 EV
            /// 0x0010 = 0.5 EV
            /// 0x0014 = 0.67 EV
            /// 0x0020 = 1 EV
            /// 0x002c = 1.33 EV
            /// 0x0030 = 1.5 EV
            /// 0x0034 = 1.67 EV
            /// 0x0040 = 2 EV
            /// </remarks>
            public const int TagFlashBias = Offset + 0x0F;

            public const int TagAutoExposureBracketing = Offset + 0x10;
            public const int TagAebBracketValue = Offset + 0x11;
            public const int TagSubjectDistance = Offset + 0x13;
        }

//        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
//        public static class CustomFunction
//        {
//            /**
//             * Long Exposure Noise Reduction
//             * 0 = Off
//             * 1 = On
//             */
//            public const int TagLongExposureNoiseReduction = 0xC301;
//
//            /**
//             * Shutter/Auto Exposure-lock buttons
//             * 0 = AF/AE lock
//             * 1 = AE lock/AF
//             * 2 = AF/AF lock
//             * 3 = AE+release/AE+AF
//             */
//            public const int TagShutterAutoExposureLockButtons = 0xC302;
//
//            /**
//             * Mirror lockup
//             * 0 = Disable
//             * 1 = Enable
//             */
//            public const int TagMirrorLockup = 0xC303;
//
//            /**
//             * Tv/Av and exposure level
//             * 0 = 1/2 stop
//             * 1 = 1/3 stop
//             */
//            public const int TagTvAvAndExposureLevel = 0xC304;
//
//            /**
//             * AF-assist light
//             * 0 = On (Auto)
//             * 1 = Off
//             */
//            public const int TagAfAssistLight = 0xC305;
//
//            /**
//             * Shutter speed in Av mode
//             * 0 = Automatic
//             * 1 = 1/200 (fixed)
//             */
//            public const int TagShutterSpeedInAvMode = 0xC306;
//
//            /**
//             * Auto-Exposure Bracketing sequence/auto cancellation
//             * 0 = 0,-,+ / Enabled
//             * 1 = 0,-,+ / Disabled
//             * 2 = -,0,+ / Enabled
//             * 3 = -,0,+ / Disabled
//             */
//            public const int TagBracketing = 0xC307;
//
//            /**
//             * Shutter Curtain Sync
//             * 0 = 1st Curtain Sync
//             * 1 = 2nd Curtain Sync
//             */
//            public const int TagShutterCurtainSync = 0xC308;
//
//            /**
//             * Lens Auto-Focus stop button Function Switch
//             * 0 = AF stop
//             * 1 = Operate AF
//             * 2 = Lock AE and start timer
//             */
//            public const int TagAfStop = 0xC309;
//
//            /**
//             * Auto reduction of fill flash
//             * 0 = Enable
//             * 1 = Disable
//             */
//            public const int TagFillFlashReduction = 0xC30A;
//
//            /**
//             * Menu button return position
//             * 0 = Top
//             * 1 = Previous (volatile)
//             * 2 = Previous
//             */
//            public const int TagMenuButtonReturn = 0xC30B;
//
//            /**
//             * SET button function when shooting
//             * 0 = Not Assigned
//             * 1 = Change Quality
//             * 2 = Change ISO Speed
//             * 3 = Select Parameters
//             */
//            public const int TagSetButtonFunction = 0xC30C;
//
//            /**
//             * Sensor cleaning
//             * 0 = Disable
//             * 1 = Enable
//             */
//            public const int TagSensorCleaning = 0xC30D;
//        }

        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        public static class ShotInfo
        {
            internal const int Offset = 0xC400;

            public const int TagAutoIso = Offset + 1;
            public const int TagBaseIso = Offset + 2;
            public const int TagMeasuredEv = Offset + 3;
            public const int TagTargetAperture = Offset + 4;
            public const int TagTargetExposureTime = Offset + 5;
            public const int TagExposureCompensation = Offset + 6;
            public const int TagWhiteBalance = Offset + 7;
            public const int TagSlowShutter = Offset + 8;
            public const int TagSequenceNumber = Offset + 9;
            public const int TagOpticalZoomCode = Offset + 10;
            public const int TagCameraTemperature = Offset + 12;
            public const int TagFlashGuideNumber = Offset + 13;
            public const int TagAfPointsInFocus = Offset + 14;
            public const int TagFlashExposureBracketing = Offset + 15;
            public const int TagAutoExposureBracketing = Offset + 16;
            public const int TagAebBracketValue = Offset + 17;
            public const int TagControlMode = Offset + 18;
            public const int TagFocusDistanceUpper = Offset + 19;
            public const int TagFocusDistanceLower = Offset + 20;
            public const int TagFNumber = Offset + 21;
            public const int TagExposureTime = Offset + 22;
            public const int TagMeasuredEv2 = Offset + 23;
            public const int TagBulbDuration = Offset + 24;
            public const int TagCameraType = Offset + 26;
            public const int TagAutoRotate = Offset + 27;
            public const int TagNdFilter = Offset + 28;
            public const int TagSelfTimer2 = Offset + 29;
            public const int TagFlashOutput = Offset + 33;
        }

        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        public static class Panorama
        {
            internal const int Offset = 0xC500;

            public const int TagPanoramaFrameNumber = Offset + 2;
            public const int TagPanoramaDirection = Offset + 5;
        }

        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        public static class AfInfo
        {
            internal const int Offset = 0xD200;

            public const int TagNumAfPoints = Offset;
            public const int TagValidAfPoints = Offset + 1;
            public const int TagImageWidth = Offset + 2;
            public const int TagImageHeight = Offset + 3;
            public const int TagAfImageWidth = Offset + 4;
            public const int TagAfImageHeight = Offset + 5;
            public const int TagAfAreaWidth = Offset + 6;
            public const int TagAfAreaHeight = Offset + 7;
            public const int TagAfAreaXPositions = Offset + 8;
            public const int TagAfAreaYPositions = Offset + 9;
            public const int TagAfPointsInFocus = Offset + 10;
            public const int TagPrimaryAfPoint1 = Offset + 11;
            public const int TagPrimaryAfPoint2 = Offset + 12;
        }

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagCanonFirmwareVersion, "Firmware Version" },
            { TagCanonImageNumber, "Image Number" },
            { TagCanonImageType, "Image Type" },
            { TagCanonOwnerName, "Owner Name" },
            { TagCanonSerialNumber, "Camera Serial Number" },
            { TagCameraInfoArray, "Camera Info Array" },
            { TagCanonFileLength, "File Length" },
            { TagsArray, "Custom Functions" },
            { TagModelId, "Canon Model ID" },
            { TagMovieInfoArray, "Movie Info Array" },
            { CameraSettings.TagAfPointSelected, "AF Point Selected" },
            { CameraSettings.TagContinuousDriveMode, "Continuous Drive Mode" },
            { CameraSettings.TagContrast, "Contrast" },
            { CameraSettings.TagEasyShootingMode, "Easy Shooting Mode" },
            { CameraSettings.TagExposureMode, "Exposure Mode" },
            { CameraSettings.TagFlashDetails, "Flash Details" },
            { CameraSettings.TagFlashMode, "Flash Mode" },
            { CameraSettings.TagFocalUnitsPerMm, "Focal Units per mm" },
            { CameraSettings.TagFocusMode1, "Focus Mode" },
            { CameraSettings.TagFocusMode2, "Focus Mode" },
            { CameraSettings.TagImageSize, "Image Size" },
            { CameraSettings.TagIso, "Iso" },
            { CameraSettings.TagLongFocalLength, "Long Focal Length" },
            { CameraSettings.TagMacroMode, "Macro Mode" },
            { CameraSettings.TagMeteringMode, "Metering Mode" },
            { CameraSettings.TagSaturation, "Saturation" },
            { CameraSettings.TagSelfTimerDelay, "Self Timer Delay" },
            { CameraSettings.TagSharpness, "Sharpness" },
            { CameraSettings.TagShortFocalLength, "Short Focal Length" },
            { CameraSettings.TagQuality, "Quality" },
            { CameraSettings.TagUnknown2, "Unknown Camera Setting 2" },
            { CameraSettings.TagUnknown3, "Unknown Camera Setting 3" },
            { CameraSettings.TagRecordMode, "Record Mode" },
            { CameraSettings.TagDigitalZoom, "Digital Zoom" },
            { CameraSettings.TagFocusType, "Focus Type" },
            { CameraSettings.TagUnknown7, "Unknown Camera Setting 7" },
            { CameraSettings.TagLensType, "Lens Type" },
            { CameraSettings.TagMaxAperture, "Max Aperture" },
            { CameraSettings.TagMinAperture, "Min Aperture" },
            { CameraSettings.TagFlashActivity, "Flash Activity" },
            { CameraSettings.TagFocusContinuous, "Focus Continuous" },
            { CameraSettings.TagAESetting, "AE Setting" },
            { CameraSettings.TagDisplayAperture, "Display Aperture" },
            { CameraSettings.TagZoomSourceWidth, "Zoom Source Width" },
            { CameraSettings.TagZoomTargetWidth, "Zoom Target Width" },
            { CameraSettings.TagSpotMeteringMode, "Spot Metering Mode" },
            { CameraSettings.TagPhotoEffect, "Photo Effect" },
            { CameraSettings.TagManualFlashOutput, "Manual Flash Output" },
            { CameraSettings.TagColorTone, "Color Tone" },
            { CameraSettings.TagSRawQuality, "SRAW Quality" },

            { FocalLength.TagWhiteBalance, "White Balance" },
            { FocalLength.TagSequenceNumber, "Sequence Number" },
            { FocalLength.TagAfPointUsed, "AF Point Used" },
            { FocalLength.TagFlashBias, "Flash Bias" },
            { FocalLength.TagAutoExposureBracketing, "Auto Exposure Bracketing" },
            { FocalLength.TagAebBracketValue, "AEB Bracket Value" },
            { FocalLength.TagSubjectDistance, "Subject Distance" },
//                { CustomFunction.TagLongExposureNoiseReduction, "Long Exposure Noise Reduction" },
//                { CustomFunction.TagShutterAutoExposureLockButtons, "Shutter/Auto Exposure-lock Buttons" },
//                { CustomFunction.TagMirrorLockup, "Mirror Lockup" },
//                { CustomFunction.TagTvAvAndExposureLevel, "Tv/Av And Exposure Level" },
//                { CustomFunction.TagAfAssistLight, "AF-Assist Light" },
//                { CustomFunction.TagShutterSpeedInAvMode, "Shutter Speed in Av Mode" },
//                { CustomFunction.TagBracketing, "Auto-Exposure Bracketing Sequence/Auto Cancellation" },
//                { CustomFunction.TagShutterCurtainSync, "Shutter Curtain Sync" },
//                { CustomFunction.TagAfStop, "Lens Auto-Focus Stop Button Function Switch" },
//                { CustomFunction.TagFillFlashReduction, "Auto Reduction of Fill Flash" },
//                { CustomFunction.TagMenuButtonReturn, "Menu Button Return Position" },
//                { CustomFunction.TagSetButtonFunction, "SET Button Function When Shooting" },
//                { CustomFunction.TagSensorCleaning, "Sensor Cleaning" },
            { ShotInfo.TagAutoIso, "Auto ISO" },
            { ShotInfo.TagBaseIso, "Base ISO" },
            { ShotInfo.TagMeasuredEv, "Measured EV" },
            { ShotInfo.TagTargetAperture, "Target Aperture" },
            { ShotInfo.TagTargetExposureTime, "Target Exposure Time" },
            { ShotInfo.TagExposureCompensation, "Exposure Compensation" },
            { ShotInfo.TagWhiteBalance, "White Balance" },
            { ShotInfo.TagSlowShutter, "Slow Shutter" },
            { ShotInfo.TagSequenceNumber, "Sequence Number" },
            { ShotInfo.TagOpticalZoomCode, "Optical Zoom Code" },
            { ShotInfo.TagCameraTemperature, "Camera Temperature" },
            { ShotInfo.TagFlashGuideNumber, "Flash Guide Number" },
            { ShotInfo.TagAfPointsInFocus, "AF Points in Focus" },
            { ShotInfo.TagFlashExposureBracketing, "Flash Exposure Compensation" },
            { ShotInfo.TagAutoExposureBracketing, "Auto Exposure Bracketing" },
            { ShotInfo.TagAebBracketValue, "AEB Bracket Value" },
            { ShotInfo.TagControlMode, "Control Mode" },
            { ShotInfo.TagFocusDistanceUpper, "Focus Distance Upper" },
            { ShotInfo.TagFocusDistanceLower, "Focus Distance Lower" },
            { ShotInfo.TagFNumber, "F Number" },
            { ShotInfo.TagExposureTime, "Exposure Time" },
            { ShotInfo.TagMeasuredEv2, "Measured EV 2" },
            { ShotInfo.TagBulbDuration, "Bulb Duration" },
            { ShotInfo.TagCameraType, "Camera Type" },
            { ShotInfo.TagAutoRotate, "Auto Rotate" },
            { ShotInfo.TagNdFilter, "ND Filter" },
            { ShotInfo.TagSelfTimer2, "Self Timer 2" },
            { ShotInfo.TagFlashOutput, "Flash Output" },
            { Panorama.TagPanoramaFrameNumber, "Panorama Frame Number" },
            { Panorama.TagPanoramaDirection, "Panorama Direction" },
            { AfInfo.TagNumAfPoints, "AF Point Count" },
            { AfInfo.TagValidAfPoints, "Valid AF Point Count" },
            { AfInfo.TagImageWidth, "Image Width" },
            { AfInfo.TagImageHeight, "Image Height" },
            { AfInfo.TagAfImageWidth, "AF Image Width" },
            { AfInfo.TagAfImageHeight, "AF Image Height" },
            { AfInfo.TagAfAreaWidth, "AF Area Width" },
            { AfInfo.TagAfAreaHeight, "AF Area Height" },
            { AfInfo.TagAfAreaXPositions, "AF Area X Positions" },
            { AfInfo.TagAfAreaYPositions, "AF Area Y Positions" },
            { AfInfo.TagAfPointsInFocus, "AF Points in Focus" },
            { AfInfo.TagPrimaryAfPoint1, "Primary AF Point 1" },
            { AfInfo.TagPrimaryAfPoint2, "Primary AF Point 2" },
            { TagThumbnailImageValidArea, "Thumbnail Image Valid Area" },
            { TagSerialNumberFormat, "Serial Number Format" },
            { TagSuperMacro, "Super Macro" },
            { TagDateStampMode, "Date Stamp Mode" },
            { TagMyColors, "My Colors" },
            { TagFirmwareRevision, "Firmware Revision" },
            { TagCategories, "Categories" },
            { TagFaceDetectArray1, "Face Detect Array 1" },
            { TagFaceDetectArray2, "Face Detect Array 2" },
            { TagAfInfoArray2, "AF Info Array 2" },
            { TagImageUniqueId, "Image Unique ID" },
            { TagRawDataOffset, "Raw Data Offset" },
            { TagOriginalDecisionDataOffset, "Original Decision Data Offset" },
            { TagCustomFunctions1DArray, "Custom Functions (1D) Array" },
            { TagPersonalFunctionsArray, "Personal Functions Array" },
            { TagPersonalFunctionValuesArray, "Personal Function Values Array" },
            { TagFileInfoArray, "File Info Array" },
            { TagAfPointsInFocus1D, "AF Points in Focus (1D)" },
            { TagLensModel, "Lens Model" },
            { TagSerialInfoArray, "Serial Info Array" },
            { TagDustRemovalData, "Dust Removal Data" },
            { TagCropInfo, "Crop Info" },
            { TagCustomFunctionsArray2, "Custom Functions Array 2" },
            { TagAspectInfoArray, "Aspect Information Array" },
            { TagProcessingInfoArray, "Processing Information Array" },
            { TagToneCurveTable, "Tone Curve Table" },
            { TagSharpnessTable, "Sharpness Table" },
            { TagSharpnessFreqTable, "Sharpness Frequency Table" },
            { TagWhiteBalanceTable, "White Balance Table" },
            { TagColorBalanceArray, "Color Balance Array" },
            { TagMeasuredColorArray, "Measured Color Array" },
            { TagColorTemperature, "Color Temperature" },
            { TagCanonFlagsArray, "Canon Flags Array" },
            { TagModifiedInfoArray, "Modified Information Array" },
            { TagToneCurveMatching, "Tone Curve Matching" },
            { TagWhiteBalanceMatching, "White Balance Matching" },
            { TagColorSpace, "Color Space" },
            { TagPreviewImageInfoArray, "Preview Image Info Array" },
            { TagVrdOffset, "VRD Offset" },
            { TagSensorInfoArray, "Sensor Information Array" },
            { TagColorDataArray2, "Color Data Array 1" },
            { TagCrwParam, "CRW Parameters" },
            { TagColorInfoArray2, "Color Data Array 2" },
            { TagBlackLevel, "Black Level" },
            { TagCustomPictureStyleFileName, "Custom Picture Style File Name" },
            { TagColorInfoArray, "Color Info Array" },
            { TagVignettingCorrectionArray1, "Vignetting Correction Array 1" },
            { TagVignettingCorrectionArray2, "Vignetting Correction Array 2" },
            { TagLightingOptimizerArray, "Lighting Optimizer Array" },
            { TagLensInfoArray, "Lens Info Array" },
            { TagAmbianceInfoArray, "Ambiance Info Array" },
            { TagFilterInfoArray, "Filter Info Array" }
        };

        public CanonMakernoteDirectory()
        {
            SetDescriptor(new CanonMakernoteDescriptor(this));
        }

        public override string Name => "Canon Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public override void Set(int tagType, object value)
        {
            var array = value as ushort[];

            if (array == null)
            {
                base.Set(tagType, value);
                return;
            }

            switch (tagType)
            {
                case TagCameraSettingsArray:
                {
                    // TODO is there some way to drop out 'null' or 'zero' values that are present in the array to reduce the noise?
                    // Certain Canon tags contain arrays of values that we split into 'fake' tags as each
                    // index in the array has its own meaning and decoding.
                    // Pick those tags out here and throw away the original array.
                    // Otherwise just add as usual.
                    for (var i = 0; i < array.Length; i++)
                        Set(CameraSettings.Offset + i, array[i]);
                    break;
                }

                case TagFocalLengthArray:
                {
                    for (var i = 0; i < array.Length; i++)
                        Set(FocalLength.Offset + i, array[i]);
                    break;
                }

                case TagShotInfoArray:
                {
                    for (var i = 0; i < array.Length; i++)
                        Set(ShotInfo.Offset + i, array[i]);
                    break;
                }

                case TagPanoramaArray:
                {
                    for (var i = 0; i < array.Length; i++)
                        Set(Panorama.Offset + i, array[i]);
                    break;
                }

                // Notes from Exiftool 10.10 by Phil Harvey, lib\Image\Exiftool\Canon.pm:
                // Auto-focus information used by many older Canon models. The values in this
                // record are sequential, and some have variable sizes based on the value of
                // numafpoints (which may be 1,5,7,9,15,45 or 53). The AFArea coordinates are
                // given in a system where the image has dimensions given by AFImageWidth and
                // AFImageHeight, and 0,0 is the image center. The direction of the Y axis
                // depends on the camera model, with positive Y upwards for EOS models, but
                // apparently downwards for PowerShot models.
                //
                // AFInfo is another array with 'fake' tags. The first int of the array contains
                // the number of AF points. Iterate through the array one byte at a time, generally
                // assuming one byte corresponds to one tag UNLESS certain tag numbers are encountered.
                // For these, read specific subsequent bytes from the array based on the tag type. The
                // number of bytes read can vary.
                //
                case TagAfInfoArray:
                {
                    int afPointCount = array[0];
                    for (int idx = 0, tagnumber = AfInfo.Offset;
                         idx < array.Length;
                         idx++, tagnumber++)
                    {
                        switch (tagnumber)
                        {
                            // These two tags store 'afPointCount' bytes of data in the array
                            case AfInfo.TagAfAreaXPositions:
                            case AfInfo.TagAfAreaYPositions:
                            {
                                // There could be incorrect data in the array, so boundary check
                                if (array.Length - 1 >= idx + afPointCount)
                                {
                                    var areaPositions = new short[afPointCount];
                                    for (var j = 0; j < areaPositions.Length; j++)
                                        areaPositions[j] = (short)array[idx + j];

                                    Set(tagnumber, areaPositions);
                                }
                                // assume these bytes are processed and skip
                                idx += afPointCount - 1;
                                break;
                            }
                            case AfInfo.TagAfPointsInFocus:
                            {
                                var pointsInFocus = new short[(afPointCount + 15)/16];

                                // There could be incorrect data in the array, so boundary check
                                if (array.Length - 1 >= idx + pointsInFocus.Length)
                                {
                                    for (var j = 0; j < pointsInFocus.Length; j++)
                                        pointsInFocus[j] = (short)array[idx + j];

                                    Set(tagnumber, pointsInFocus);
                                }
                                // assume these bytes are processed and skip
                                idx += pointsInFocus.Length - 1;
                                break;
                            }
                            default:
                            {
                                Set(tagnumber, array[idx]);
                                break;
                            }
                        }
                    }
                    break;
                }

                // TODO the interpretation of the custom functions tag depends upon the camera model
//                case TAG_CANON_CUSTOM_FUNCTIONS_ARRAY:
//                {
//                    int subTagTypeBase = 0xC300;
//                    // we intentionally skip the first array member
//                    for (int i = 1; i < ints.length; i++)
//                        setInt(subTagTypeBase + i + 1, ints[i] & 0x0F);
//                    break;
//                }

                default:
                {
                    // no special handling...
                    base.Set(tagType, value);
                    break;
                }
            }
        }
    }
}
