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
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>Describes tags specific to Canon cameras.</summary>
    /// <remarks>
    /// Describes tags specific to Canon cameras.
    /// Thanks to Bill Richards for his contribution to this makernote directory.
    /// Many tag definitions explained here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/canon_mn.html
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class CanonMakernoteDirectory : Directory
    {
        private const int TagCameraSettingsArray = unchecked((int)(0x0001));

        private const int TagFocalLengthArray = unchecked((int)(0x0002));

        private const int TagShotInfoArray = unchecked((int)(0x0004));

        private const int TagPanoramaArray = unchecked((int)(0x0005));

        public const int TagCanonImageType = unchecked((int)(0x0006));

        public const int TagCanonFirmwareVersion = unchecked((int)(0x0007));

        public const int TagCanonImageNumber = unchecked((int)(0x0008));

        public const int TagCanonOwnerName = unchecked((int)(0x0009));

        public const int TagCanonSerialNumber = unchecked((int)(0x000C));

        public const int TagCameraInfoArray = unchecked((int)(0x000D));

        public const int TagCanonFileLength = unchecked((int)(0x000E));

        public const int TagCanonCustomFunctionsArray = unchecked((int)(0x000F));

        public const int TagModelId = unchecked((int)(0x0010));

        public const int TagMovieInfoArray = unchecked((int)(0x0011));

        private const int TagAfInfoArray = unchecked((int)(0x0012));

        public const int TagThumbnailImageValidArea = unchecked((int)(0x0013));

        public const int TagSerialNumberFormat = unchecked((int)(0x0015));

        public const int TagSuperMacro = unchecked((int)(0x001A));

        public const int TagDateStampMode = unchecked((int)(0x001C));

        public const int TagMyColors = unchecked((int)(0x001D));

        public const int TagFirmwareRevision = unchecked((int)(0x001E));

        public const int TagCategories = unchecked((int)(0x0023));

        public const int TagFaceDetectArray1 = unchecked((int)(0x0024));

        public const int TagFaceDetectArray2 = unchecked((int)(0x0025));

        public const int TagAfInfoArray2 = unchecked((int)(0x0026));

        public const int TagImageUniqueId = unchecked((int)(0x0028));

        public const int TagRawDataOffset = unchecked((int)(0x0081));

        public const int TagOriginalDecisionDataOffset = unchecked((int)(0x0083));

        public const int TagCustomFunctions1dArray = unchecked((int)(0x0090));

        public const int TagPersonalFunctionsArray = unchecked((int)(0x0091));

        public const int TagPersonalFunctionValuesArray = unchecked((int)(0x0092));

        public const int TagFileInfoArray = unchecked((int)(0x0093));

        public const int TagAfPointsInFocus1d = unchecked((int)(0x0094));

        public const int TagLensModel = unchecked((int)(0x0095));

        public const int TagSerialInfoArray = unchecked((int)(0x0096));

        public const int TagDustRemovalData = unchecked((int)(0x0097));

        public const int TagCropInfo = unchecked((int)(0x0098));

        public const int TagCustomFunctionsArray2 = unchecked((int)(0x0099));

        public const int TagAspectInfoArray = unchecked((int)(0x009A));

        public const int TagProcessingInfoArray = unchecked((int)(0x00A0));

        public const int TagToneCurveTable = unchecked((int)(0x00A1));

        public const int TagSharpnessTable = unchecked((int)(0x00A2));

        public const int TagSharpnessFreqTable = unchecked((int)(0x00A3));

        public const int TagWhiteBalanceTable = unchecked((int)(0x00A4));

        public const int TagColorBalanceArray = unchecked((int)(0x00A9));

        public const int TagMeasuredColorArray = unchecked((int)(0x00AA));

        public const int TagColorTemperature = unchecked((int)(0x00AE));

        public const int TagCanonFlagsArray = unchecked((int)(0x00B0));

        public const int TagModifiedInfoArray = unchecked((int)(0x00B1));

        public const int TagToneCurveMatching = unchecked((int)(0x00B2));

        public const int TagWhiteBalanceMatching = unchecked((int)(0x00B3));

        public const int TagColorSpace = unchecked((int)(0x00B4));

        public const int TagPreviewImageInfoArray = unchecked((int)(0x00B6));

        public const int TagVrdOffset = unchecked((int)(0x00D0));

        public const int TagSensorInfoArray = unchecked((int)(0x00E0));

        public const int TagColorDataArray2 = unchecked((int)(0x4001));

        public const int TagCrwParam = unchecked((int)(0x4002));

        public const int TagColorInfoArray2 = unchecked((int)(0x4003));

        public const int TagBlackLevel = unchecked((int)(0x4008));

        public const int TagCustomPictureStyleFileName = unchecked((int)(0x4010));

        public const int TagColorInfoArray = unchecked((int)(0x4013));

        public const int TagVignettingCorrectionArray1 = unchecked((int)(0x4015));

        public const int TagVignettingCorrectionArray2 = unchecked((int)(0x4016));

        public const int TagLightingOptimizerArray = unchecked((int)(0x4018));

        public const int TagLensInfoArray = unchecked((int)(0x4019));

        public const int TagAmbianceInfoArray = unchecked((int)(0x4020));

        public const int TagFilterInfoArray = unchecked((int)(0x4024));

        public static class CameraSettings
        {
            internal const int Offset = unchecked((int)(0xC100));

            /// <summary>
            /// 1 = Macro
            /// 2 = Normal
            /// </summary>
            public const int TagMacroMode = Offset + unchecked((int)(0x01));

            public const int TagSelfTimerDelay = Offset + unchecked((int)(0x02));

            /// <summary>
            /// 2 = Normal
            /// 3 = Fine
            /// 5 = Superfine
            /// </summary>
            public const int TagQuality = Offset + unchecked((int)(0x03));

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
            public const int TagFlashMode = Offset + unchecked((int)(0x04));

            /// <summary>
            /// 0 = Single Frame or Timer Mode
            /// 1 = Continuous
            /// </summary>
            public const int TagContinuousDriveMode = Offset + unchecked((int)(0x05));

            public const int TagUnknown2 = Offset + unchecked((int)(0x06));

            /// <summary>
            /// 0 = One-Shot
            /// 1 = AI Servo
            /// 2 = AI Focus
            /// 3 = Manual Focus
            /// 4 = Single
            /// 5 = Continuous
            /// 6 = Manual Focus
            /// </summary>
            public const int TagFocusMode1 = Offset + unchecked((int)(0x07));

            public const int TagUnknown3 = Offset + unchecked((int)(0x08));

            public const int TagUnknown4 = Offset + unchecked((int)(0x09));

            /// <summary>
            /// 0 = Large
            /// 1 = Medium
            /// 2 = Small
            /// </summary>
            public const int TagImageSize = Offset + unchecked((int)(0x0A));

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
            public const int TagEasyShootingMode = Offset + unchecked((int)(0x0B));

            /// <summary>
            /// 0 = No Digital Zoom
            /// 1 = 2x
            /// 2 = 4x
            /// </summary>
            public const int TagDigitalZoom = Offset + unchecked((int)(0x0C));

            /// <summary>
            /// 0 = Normal
            /// 1 = High
            /// 65535 = Low
            /// </summary>
            public const int TagContrast = Offset + unchecked((int)(0x0D));

            /// <summary>
            /// 0 = Normal
            /// 1 = High
            /// 65535 = Low
            /// </summary>
            public const int TagSaturation = Offset + unchecked((int)(0x0E));

            /// <summary>
            /// 0 = Normal
            /// 1 = High
            /// 65535 = Low
            /// </summary>
            public const int TagSharpness = Offset + unchecked((int)(0x0F));

            /// <summary>
            /// 0 = Check ISOSpeedRatings EXIF tag for ISO Speed
            /// 15 = Auto ISO
            /// 16 = ISO 50
            /// 17 = ISO 100
            /// 18 = ISO 200
            /// 19 = ISO 400
            /// </summary>
            public const int TagIso = Offset + unchecked((int)(0x10));

            /// <summary>
            /// 3 = Evaluative
            /// 4 = Partial
            /// 5 = Centre Weighted
            /// </summary>
            public const int TagMeteringMode = Offset + unchecked((int)(0x11));

            /// <summary>
            /// 0 = Manual
            /// 1 = Auto
            /// 3 = Close-up (Macro)
            /// 8 = Locked (Pan Mode)
            /// </summary>
            public const int TagFocusType = Offset + unchecked((int)(0x12));

            /// <summary>
            /// 12288 = None (Manual Focus)
            /// 12289 = Auto Selected
            /// 12290 = Right
            /// 12291 = Centre
            /// 12292 = Left
            /// </summary>
            public const int TagAfPointSelected = Offset + unchecked((int)(0x13));

            /// <summary>
            /// 0 = Easy Shooting (See Easy Shooting Mode)
            /// 1 = Program
            /// 2 = Tv-Priority
            /// 3 = Av-Priority
            /// 4 = Manual
            /// 5 = A-DEP
            /// </summary>
            public const int TagExposureMode = Offset + unchecked((int)(0x14));

            public const int TagUnknown7 = Offset + unchecked((int)(0x15));

            public const int TagUnknown8 = Offset + unchecked((int)(0x16));

            public const int TagLongFocalLength = Offset + unchecked((int)(0x17));

            public const int TagShortFocalLength = Offset + unchecked((int)(0x18));

            public const int TagFocalUnitsPerMm = Offset + unchecked((int)(0x19));

            public const int TagUnknown9 = Offset + unchecked((int)(0x1A));

            public const int TagUnknown10 = Offset + unchecked((int)(0x1B));

            /// <summary>
            /// 0 = Flash Did Not Fire
            /// 1 = Flash Fired
            /// </summary>
            public const int TagFlashActivity = Offset + unchecked((int)(0x1C));

            public const int TagFlashDetails = Offset + unchecked((int)(0x1D));

            public const int TagUnknown12 = Offset + unchecked((int)(0x1E));

            public const int TagUnknown13 = Offset + unchecked((int)(0x1F));

            /// <summary>
            /// 0 = Focus Mode: Single
            /// 1 = Focus Mode: Continuous
            /// </summary>
            public const int TagFocusMode2 = Offset + unchecked((int)(0x20));
            // These TAG_*_ARRAY Exif tags map to arrays of int16 values which are split out into separate 'fake' tags.
            // When an attempt is made to set one of these on the directory, it is split and the corresponding offset added to the tagType.
            // So the resulting tag is the offset + the index into the array.
            //    private static final int TAG_FLASH_INFO                     = 0x0003;
            // depends upon model, so leave for now
            // depends upon model, so leave for now
            // not currently decoded as not sure we see it in still images
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // depends upon camera model, not currently decoded
            // depends upon camera model, not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // not currently decoded
            // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        }

        public static class FocalLength
        {
            internal const int Offset = unchecked((int)(0xC200));

            /// <summary>
            /// 0 = Auto
            /// 1 = Sunny
            /// 2 = Cloudy
            /// 3 = Tungsten
            /// 4 = Florescent
            /// 5 = Flash
            /// 6 = Custom
            /// </summary>
            public const int TagWhiteBalance = Offset + unchecked((int)(0x07));

            public const int TagSequenceNumber = Offset + unchecked((int)(0x09));

            public const int TagAfPointUsed = Offset + unchecked((int)(0x0E));

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
            public const int TagFlashBias = Offset + unchecked((int)(0x0F));

            public const int TagAutoExposureBracketing = Offset + unchecked((int)(0x10));

            public const int TagAebBracketValue = Offset + unchecked((int)(0x11));

            public const int TagSubjectDistance = Offset + unchecked((int)(0x13));
            // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        }

        public static class ShotInfo
        {
            internal const int Offset = unchecked((int)(0xC400));

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
            // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        }

        public static class Panorama
        {
            internal const int Offset = unchecked((int)(0xC500));

            public const int TagPanoramaFrameNumber = Offset + 2;

            public const int TagPanoramaDirection = Offset + 5;
            // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        }

        public static class AFInfo
        {
            internal const int Offset = unchecked((int)(0xD200));

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
            // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
            // not sure why there are two of these
        }

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static CanonMakernoteDirectory()
        {
            //    /**
            //     * Long Exposure Noise Reduction
            //     * 0 = Off
            //     * 1 = On
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_LONG_EXPOSURE_NOISE_REDUCTION = 0xC301;
            //
            //    /**
            //     * Shutter/Auto Exposure-lock buttons
            //     * 0 = AF/AE lock
            //     * 1 = AE lock/AF
            //     * 2 = AF/AF lock
            //     * 3 = AE+release/AE+AF
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_SHUTTER_AUTO_EXPOSURE_LOCK_BUTTONS = 0xC302;
            //
            //    /**
            //     * Mirror lockup
            //     * 0 = Disable
            //     * 1 = Enable
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_MIRROR_LOCKUP = 0xC303;
            //
            //    /**
            //     * Tv/Av and exposure level
            //     * 0 = 1/2 stop
            //     * 1 = 1/3 stop
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_TV_AV_AND_EXPOSURE_LEVEL = 0xC304;
            //
            //    /**
            //     * AF-assist light
            //     * 0 = On (Auto)
            //     * 1 = Off
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_AF_ASSIST_LIGHT = 0xC305;
            //
            //    /**
            //     * Shutter speed in Av mode
            //     * 0 = Automatic
            //     * 1 = 1/200 (fixed)
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_SHUTTER_SPEED_IN_AV_MODE = 0xC306;
            //
            //    /**
            //     * Auto-Exposure Bracketing sequence/auto cancellation
            //     * 0 = 0,-,+ / Enabled
            //     * 1 = 0,-,+ / Disabled
            //     * 2 = -,0,+ / Enabled
            //     * 3 = -,0,+ / Disabled
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_BRACKETING = 0xC307;
            //
            //    /**
            //     * Shutter Curtain Sync
            //     * 0 = 1st Curtain Sync
            //     * 1 = 2nd Curtain Sync
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_SHUTTER_CURTAIN_SYNC = 0xC308;
            //
            //    /**
            //     * Lens Auto-Focus stop button Function Switch
            //     * 0 = AF stop
            //     * 1 = Operate AF
            //     * 2 = Lock AE and start timer
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_AF_STOP = 0xC309;
            //
            //    /**
            //     * Auto reduction of fill flash
            //     * 0 = Enable
            //     * 1 = Disable
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_FILL_FLASH_REDUCTION = 0xC30A;
            //
            //    /**
            //     * Menu button return position
            //     * 0 = Top
            //     * 1 = Previous (volatile)
            //     * 2 = Previous
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_MENU_BUTTON_RETURN = 0xC30B;
            //
            //    /**
            //     * SET button function when shooting
            //     * 0 = Not Assigned
            //     * 1 = Change Quality
            //     * 2 = Change ISO Speed
            //     * 3 = Select Parameters
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_SET_BUTTON_FUNCTION = 0xC30C;
            //
            //    /**
            //     * Sensor cleaning
            //     * 0 = Disable
            //     * 1 = Enable
            //     */
            //    public static final int TAG_CANON_CUSTOM_FUNCTION_SENSOR_CLEANING = 0xC30D;
            _tagNameMap.Put(TagCanonFirmwareVersion, "Firmware Version");
            _tagNameMap.Put(TagCanonImageNumber, "Image Number");
            _tagNameMap.Put(TagCanonImageType, "Image Type");
            _tagNameMap.Put(TagCanonOwnerName, "Owner Name");
            _tagNameMap.Put(TagCanonSerialNumber, "Camera Serial Number");
            _tagNameMap.Put(TagCameraInfoArray, "Camera Info Array");
            _tagNameMap.Put(TagCanonFileLength, "File Length");
            _tagNameMap.Put(TagCanonCustomFunctionsArray, "Custom Functions");
            _tagNameMap.Put(TagModelId, "Canon Model ID");
            _tagNameMap.Put(TagMovieInfoArray, "Movie Info Array");
            _tagNameMap.Put(CameraSettings.TagAfPointSelected, "AF Point Selected");
            _tagNameMap.Put(CameraSettings.TagContinuousDriveMode, "Continuous Drive Mode");
            _tagNameMap.Put(CameraSettings.TagContrast, "Contrast");
            _tagNameMap.Put(CameraSettings.TagEasyShootingMode, "Easy Shooting Mode");
            _tagNameMap.Put(CameraSettings.TagExposureMode, "Exposure Mode");
            _tagNameMap.Put(CameraSettings.TagFlashDetails, "Flash Details");
            _tagNameMap.Put(CameraSettings.TagFlashMode, "Flash Mode");
            _tagNameMap.Put(CameraSettings.TagFocalUnitsPerMm, "Focal Units per mm");
            _tagNameMap.Put(CameraSettings.TagFocusMode1, "Focus Mode");
            _tagNameMap.Put(CameraSettings.TagFocusMode2, "Focus Mode");
            _tagNameMap.Put(CameraSettings.TagImageSize, "Image Size");
            _tagNameMap.Put(CameraSettings.TagIso, "Iso");
            _tagNameMap.Put(CameraSettings.TagLongFocalLength, "Long Focal Length");
            _tagNameMap.Put(CameraSettings.TagMacroMode, "Macro Mode");
            _tagNameMap.Put(CameraSettings.TagMeteringMode, "Metering Mode");
            _tagNameMap.Put(CameraSettings.TagSaturation, "Saturation");
            _tagNameMap.Put(CameraSettings.TagSelfTimerDelay, "Self Timer Delay");
            _tagNameMap.Put(CameraSettings.TagSharpness, "Sharpness");
            _tagNameMap.Put(CameraSettings.TagShortFocalLength, "Short Focal Length");
            _tagNameMap.Put(CameraSettings.TagQuality, "Quality");
            _tagNameMap.Put(CameraSettings.TagUnknown2, "Unknown Camera Setting 2");
            _tagNameMap.Put(CameraSettings.TagUnknown3, "Unknown Camera Setting 3");
            _tagNameMap.Put(CameraSettings.TagUnknown4, "Unknown Camera Setting 4");
            _tagNameMap.Put(CameraSettings.TagDigitalZoom, "Digital Zoom");
            _tagNameMap.Put(CameraSettings.TagFocusType, "Focus Type");
            _tagNameMap.Put(CameraSettings.TagUnknown7, "Unknown Camera Setting 7");
            _tagNameMap.Put(CameraSettings.TagUnknown8, "Unknown Camera Setting 8");
            _tagNameMap.Put(CameraSettings.TagUnknown9, "Unknown Camera Setting 9");
            _tagNameMap.Put(CameraSettings.TagUnknown10, "Unknown Camera Setting 10");
            _tagNameMap.Put(CameraSettings.TagFlashActivity, "Flash Activity");
            _tagNameMap.Put(CameraSettings.TagUnknown12, "Unknown Camera Setting 12");
            _tagNameMap.Put(CameraSettings.TagUnknown13, "Unknown Camera Setting 13");
            _tagNameMap.Put(FocalLength.TagWhiteBalance, "White Balance");
            _tagNameMap.Put(FocalLength.TagSequenceNumber, "Sequence Number");
            _tagNameMap.Put(FocalLength.TagAfPointUsed, "AF Point Used");
            _tagNameMap.Put(FocalLength.TagFlashBias, "Flash Bias");
            _tagNameMap.Put(FocalLength.TagAutoExposureBracketing, "Auto Exposure Bracketing");
            _tagNameMap.Put(FocalLength.TagAebBracketValue, "AEB Bracket Value");
            _tagNameMap.Put(FocalLength.TagSubjectDistance, "Subject Distance");
            _tagNameMap.Put(ShotInfo.TagAutoIso, "Auto ISO");
            _tagNameMap.Put(ShotInfo.TagBaseIso, "Base ISO");
            _tagNameMap.Put(ShotInfo.TagMeasuredEv, "Measured EV");
            _tagNameMap.Put(ShotInfo.TagTargetAperture, "Target Aperture");
            _tagNameMap.Put(ShotInfo.TagTargetExposureTime, "Target Exposure Time");
            _tagNameMap.Put(ShotInfo.TagExposureCompensation, "Exposure Compensation");
            _tagNameMap.Put(ShotInfo.TagWhiteBalance, "White Balance");
            _tagNameMap.Put(ShotInfo.TagSlowShutter, "Slow Shutter");
            _tagNameMap.Put(ShotInfo.TagSequenceNumber, "Sequence Number");
            _tagNameMap.Put(ShotInfo.TagOpticalZoomCode, "Optical Zoom Code");
            _tagNameMap.Put(ShotInfo.TagCameraTemperature, "Camera Temperature");
            _tagNameMap.Put(ShotInfo.TagFlashGuideNumber, "Flash Guide Number");
            _tagNameMap.Put(ShotInfo.TagAfPointsInFocus, "AF Points in Focus");
            _tagNameMap.Put(ShotInfo.TagFlashExposureBracketing, "Flash Exposure Compensation");
            _tagNameMap.Put(ShotInfo.TagAutoExposureBracketing, "Auto Exposure Bracketing");
            _tagNameMap.Put(ShotInfo.TagAebBracketValue, "AEB Bracket Value");
            _tagNameMap.Put(ShotInfo.TagControlMode, "Control Mode");
            _tagNameMap.Put(ShotInfo.TagFocusDistanceUpper, "Focus Distance Upper");
            _tagNameMap.Put(ShotInfo.TagFocusDistanceLower, "Focus Distance Lower");
            _tagNameMap.Put(ShotInfo.TagFNumber, "F Number");
            _tagNameMap.Put(ShotInfo.TagExposureTime, "Exposure Time");
            _tagNameMap.Put(ShotInfo.TagMeasuredEv2, "Measured EV 2");
            _tagNameMap.Put(ShotInfo.TagBulbDuration, "Bulb Duration");
            _tagNameMap.Put(ShotInfo.TagCameraType, "Camera Type");
            _tagNameMap.Put(ShotInfo.TagAutoRotate, "Auto Rotate");
            _tagNameMap.Put(ShotInfo.TagNdFilter, "ND Filter");
            _tagNameMap.Put(ShotInfo.TagSelfTimer2, "Self Timer 2");
            _tagNameMap.Put(ShotInfo.TagFlashOutput, "Flash Output");
            _tagNameMap.Put(Panorama.TagPanoramaFrameNumber, "Panorama Frame Number");
            _tagNameMap.Put(Panorama.TagPanoramaDirection, "Panorama Direction");
            _tagNameMap.Put(AFInfo.TagNumAfPoints, "AF Point Count");
            _tagNameMap.Put(AFInfo.TagValidAfPoints, "Valid AF Point Count");
            _tagNameMap.Put(AFInfo.TagImageWidth, "Image Width");
            _tagNameMap.Put(AFInfo.TagImageHeight, "Image Height");
            _tagNameMap.Put(AFInfo.TagAfImageWidth, "AF Image Width");
            _tagNameMap.Put(AFInfo.TagAfImageHeight, "AF Image Height");
            _tagNameMap.Put(AFInfo.TagAfAreaWidth, "AF Area Width");
            _tagNameMap.Put(AFInfo.TagAfAreaHeight, "AF Area Height");
            _tagNameMap.Put(AFInfo.TagAfAreaXPositions, "AF Area X Positions");
            _tagNameMap.Put(AFInfo.TagAfAreaYPositions, "AF Area Y Positions");
            _tagNameMap.Put(AFInfo.TagAfPointsInFocus, "AF Points in Focus Count");
            _tagNameMap.Put(AFInfo.TagPrimaryAfPoint1, "Primary AF Point 1");
            _tagNameMap.Put(AFInfo.TagPrimaryAfPoint2, "Primary AF Point 2");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_LONG_EXPOSURE_NOISE_REDUCTION, "Long Exposure Noise Reduction");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_SHUTTER_AUTO_EXPOSURE_LOCK_BUTTONS, "Shutter/Auto Exposure-lock Buttons");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_MIRROR_LOCKUP, "Mirror Lockup");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_TV_AV_AND_EXPOSURE_LEVEL, "Tv/Av And Exposure Level");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_AF_ASSIST_LIGHT, "AF-Assist Light");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_SHUTTER_SPEED_IN_AV_MODE, "Shutter Speed in Av Mode");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_BRACKETING, "Auto-Exposure Bracketing Sequence/Auto Cancellation");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_SHUTTER_CURTAIN_SYNC, "Shutter Curtain Sync");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_AF_STOP, "Lens Auto-Focus Stop Button Function Switch");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_FILL_FLASH_REDUCTION, "Auto Reduction of Fill Flash");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_MENU_BUTTON_RETURN, "Menu Button Return Position");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_SET_BUTTON_FUNCTION, "SET Button Function When Shooting");
            //        _tagNameMap.put(TAG_CANON_CUSTOM_FUNCTION_SENSOR_CLEANING, "Sensor Cleaning");
            _tagNameMap.Put(TagThumbnailImageValidArea, "Thumbnail Image Valid Area");
            _tagNameMap.Put(TagSerialNumberFormat, "Serial Number Format");
            _tagNameMap.Put(TagSuperMacro, "Super Macro");
            _tagNameMap.Put(TagDateStampMode, "Date Stamp Mode");
            _tagNameMap.Put(TagMyColors, "My Colors");
            _tagNameMap.Put(TagFirmwareRevision, "Firmware Revision");
            _tagNameMap.Put(TagCategories, "Categories");
            _tagNameMap.Put(TagFaceDetectArray1, "Face Detect Array 1");
            _tagNameMap.Put(TagFaceDetectArray2, "Face Detect Array 2");
            _tagNameMap.Put(TagAfInfoArray2, "AF Info Array 2");
            _tagNameMap.Put(TagImageUniqueId, "Image Unique ID");
            _tagNameMap.Put(TagRawDataOffset, "Raw Data Offset");
            _tagNameMap.Put(TagOriginalDecisionDataOffset, "Original Decision Data Offset");
            _tagNameMap.Put(TagCustomFunctions1dArray, "Custom Functions (1D) Array");
            _tagNameMap.Put(TagPersonalFunctionsArray, "Personal Functions Array");
            _tagNameMap.Put(TagPersonalFunctionValuesArray, "Personal Function Values Array");
            _tagNameMap.Put(TagFileInfoArray, "File Info Array");
            _tagNameMap.Put(TagAfPointsInFocus1d, "AF Points in Focus (1D)");
            _tagNameMap.Put(TagLensModel, "Lens Model");
            _tagNameMap.Put(TagSerialInfoArray, "Serial Info Array");
            _tagNameMap.Put(TagDustRemovalData, "Dust Removal Data");
            _tagNameMap.Put(TagCropInfo, "Crop Info");
            _tagNameMap.Put(TagCustomFunctionsArray2, "Custom Functions Array 2");
            _tagNameMap.Put(TagAspectInfoArray, "Aspect Information Array");
            _tagNameMap.Put(TagProcessingInfoArray, "Processing Information Array");
            _tagNameMap.Put(TagToneCurveTable, "Tone Curve Table");
            _tagNameMap.Put(TagSharpnessTable, "Sharpness Table");
            _tagNameMap.Put(TagSharpnessFreqTable, "Sharpness Frequency Table");
            _tagNameMap.Put(TagWhiteBalanceTable, "White Balance Table");
            _tagNameMap.Put(TagColorBalanceArray, "Color Balance Array");
            _tagNameMap.Put(TagMeasuredColorArray, "Measured Color Array");
            _tagNameMap.Put(TagColorTemperature, "Color Temperature");
            _tagNameMap.Put(TagCanonFlagsArray, "Canon Flags Array");
            _tagNameMap.Put(TagModifiedInfoArray, "Modified Information Array");
            _tagNameMap.Put(TagToneCurveMatching, "Tone Curve Matching");
            _tagNameMap.Put(TagWhiteBalanceMatching, "White Balance Matching");
            _tagNameMap.Put(TagColorSpace, "Color Space");
            _tagNameMap.Put(TagPreviewImageInfoArray, "Preview Image Info Array");
            _tagNameMap.Put(TagVrdOffset, "VRD Offset");
            _tagNameMap.Put(TagSensorInfoArray, "Sensor Information Array");
            _tagNameMap.Put(TagColorDataArray2, "Color Data Array 1");
            _tagNameMap.Put(TagCrwParam, "CRW Parameters");
            _tagNameMap.Put(TagColorInfoArray2, "Color Data Array 2");
            _tagNameMap.Put(TagBlackLevel, "Black Level");
            _tagNameMap.Put(TagCustomPictureStyleFileName, "Custom Picture Style File Name");
            _tagNameMap.Put(TagColorInfoArray, "Color Info Array");
            _tagNameMap.Put(TagVignettingCorrectionArray1, "Vignetting Correction Array 1");
            _tagNameMap.Put(TagVignettingCorrectionArray2, "Vignetting Correction Array 2");
            _tagNameMap.Put(TagLightingOptimizerArray, "Lighting Optimizer Array");
            _tagNameMap.Put(TagLensInfoArray, "Lens Info Array");
            _tagNameMap.Put(TagAmbianceInfoArray, "Ambiance Info Array");
            _tagNameMap.Put(TagFilterInfoArray, "Filter Info Array");
        }

        public CanonMakernoteDirectory()
        {
            this.SetDescriptor(new CanonMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Canon Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }

        public override void SetObjectArray(int tagType, [NotNull] object array)
        {
            switch (tagType)
            {
                case TagCameraSettingsArray:
                {
                    // TODO is there some way to drop out 'null' or 'zero' values that are present in the array to reduce the noise?
                    // Certain Canon tags contain arrays of values that we split into 'fake' tags as each
                    // index in the array has its own meaning and decoding.
                    // Pick those tags out here and throw away the original array.
                    // Otherwise just add as usual.
                    int[] ints = (int[])array;
                    for (int i = 0; i < ints.Length; i++)
                    {
                        SetInt(CameraSettings.Offset + i, ints[i]);
                    }
                    break;
                }

                case TagFocalLengthArray:
                {
                    int[] ints = (int[])array;
                    for (int i = 0; i < ints.Length; i++)
                    {
                        SetInt(FocalLength.Offset + i, ints[i]);
                    }
                    break;
                }

                case TagShotInfoArray:
                {
                    int[] ints = (int[])array;
                    for (int i = 0; i < ints.Length; i++)
                    {
                        SetInt(ShotInfo.Offset + i, ints[i]);
                    }
                    break;
                }

                case TagPanoramaArray:
                {
                    int[] ints = (int[])array;
                    for (int i = 0; i < ints.Length; i++)
                    {
                        SetInt(Panorama.Offset + i, ints[i]);
                    }
                    break;
                }

                case TagAfInfoArray:
                {
                    // TODO the interpretation of the custom functions tag depends upon the camera model
                    //            case TAG_CANON_CUSTOM_FUNCTIONS_ARRAY:
                    //                int subTagTypeBase = 0xC300;
                    //                // we intentionally skip the first array member
                    //                for (int i = 1; i < ints.length; i++)
                    //                    setInt(subTagTypeBase + i + 1, ints[i] & 0x0F);
                    //                break;
                    int[] ints = (int[])array;
                    for (int i = 0; i < ints.Length; i++)
                    {
                        SetInt(AFInfo.Offset + i, ints[i]);
                    }
                    break;
                }

                default:
                {
                    // no special handling...
                    base.SetObjectArray(tagType, array);
                    break;
                }
            }
        }
    }
}
