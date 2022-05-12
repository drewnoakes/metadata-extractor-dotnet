// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Pentax tags.
    /// </summary>
    /// <remarks>
    /// https://exiftool.org/TagNames/Pentax.html
    /// </remarks>
    public class PentaxType2MakernoteDirectory : Directory
    {
#pragma warning disable format
        public const int TagPentaxVersion              = 0x0000;
        public const int TagPentaxModelType            = 0x0001;
        public const int TagPreviewImageSize           = 0x0002;
        public const int TagPreviewImageLength         = 0x0003;
        public const int TagPreviewImageStart          = 0x0004;
        public const int TagPentaxModelId              = 0x0005;
        public const int TagDate                       = 0x0006;
        public const int TagTime                       = 0x0007;
        public const int TagQuality                    = 0x0008;
        public const int TagPentaxImageSize            = 0x0009;
        public const int TagPictureMode                = 0x000B;
        public const int TagFlashMode                  = 0x000C;
        public const int TagFocusMode                  = 0x000D;
        public const int TagAFPointSelected            = 0x000E;
        public const int TagAFPointsInFocus            = 0x000F;
        public const int TagFocusPosition              = 0x0010;
        public const int TagExposureTime               = 0x0012;
        public const int TagFNumber                    = 0x0013;
        public const int TagIso                        = 0x0014;
        public const int TagLightReading               = 0x0015;
        public const int TagExposureCompensation       = 0x0016;
        public const int TagMeteringMode               = 0x0017;
        public const int TagAutoBracketing             = 0x0018;
        public const int TagWhiteBalance               = 0x0019;
        public const int TagWhiteBalanceMode           = 0x001A;
        public const int TagBlueBalance                = 0x001B;
        public const int TagRedBalance                 = 0x001C;
        public const int TagFocalLength                = 0x001D;
        public const int TagDigitalZoom                = 0x001E;
        public const int TagSaturation                 = 0x001F;
        public const int TagContrast                   = 0x0020;
        public const int TagSharpness                  = 0x0021;
        public const int TagWorldTimeLocation          = 0x0022;
        public const int TagHometownCity               = 0x0023;
        public const int TagDestinationCity            = 0x0024;
        public const int TagHometownDST                = 0x0025;
        public const int TagDestinationDST             = 0x0026;
        public const int TagDSPFirmwareVersion         = 0x0027;
        public const int TagCPUFirmwareVersion         = 0x0028;
        public const int TagFrameNumber                = 0x0029;
        public const int TagEffectiveLV                = 0x002D;
        public const int TagImageEditing               = 0x0032;
        public const int TagPictureMode2               = 0x0033;
        public const int TagDriveMode                  = 0x0034;
        public const int TagSensorSize                 = 0x0035;
        public const int TagColorSpace                 = 0x0037;
        public const int TagImageAreaOffset            = 0x0038;
        public const int TagRawImageSize               = 0x0039;
        public const int TagAFPointsInFocus2           = 0x003C;
        public const int TagDataScaling                = 0x003D;
        public const int TagPreviewImageBorders        = 0x003E;
        public const int TagLensRec                    = 0x003F;
        public const int TagSensitivityAdjust          = 0x0040;
        public const int TagImageEditCount             = 0x0041;
        public const int TagCameraTemperature          = 0x0047;
        public const int TagAELock                     = 0x0048;
        public const int TagNoiseReduction             = 0x0049;
        public const int TagFlashExposureComp          = 0x004D;
        public const int TagImageTone                  = 0x004F;
        public const int TagColorTemperature           = 0x0050;
        public const int TagColorTempDaylight          = 0x0053;
        public const int TagColorTempShade             = 0x0054;
        public const int TagColorTempCloudy            = 0x0055;
        public const int TagColorTempTungsten          = 0x0056;
        public const int TagColorTempFluorescentD      = 0x0057;
        public const int TagColorTempFluorescentN      = 0x0058;
        public const int TagColorTempFluorescentW      = 0x0059;
        public const int TagColorTempFlash             = 0x005A;
        public const int TagShakeReductionInfo         = 0x005C;
        public const int TagShutterCount               = 0x005D;
        public const int TagFaceInfo                   = 0x0060;
        public const int TagRawDevelopmentProcess      = 0x0062;
        public const int TagHue                        = 0x0067;
        public const int TagAWBInfo                    = 0x0068;
        public const int TagDynamicRangeExpansion      = 0x0069;
        public const int TagTimeInfo                   = 0x006B;
        public const int TagHighLowKeyAdj              = 0x006C;
        public const int TagContrastHighlight          = 0x006D;
        public const int TagContrastShadow             = 0x006E;
        public const int TagContrastHighlightShadowAdj = 0x006F;
        public const int TagFineSharpness              = 0x0070;
        public const int TagHighISONoiseReduction      = 0x0071;
        public const int TagAFAdjustment               = 0x0072;
        public const int TagMonochromeFilterEffect     = 0x0073;
        public const int TagMonochromeToning           = 0x0074;
        public const int TagFaceDetect                 = 0x0076;
        public const int TagFaceDetectFrameSize        = 0x0077;
        public const int TagShadowCorrection           = 0x0079;
        public const int TagISOAutoParameters          = 0x007A;
        public const int TagCrossProcess               = 0x007B;
        public const int TagLensCorr                   = 0x007D;
        public const int TagWhiteLevel                 = 0x007E;
        public const int TagBleachBypassToning         = 0x007F;
        public const int TagAspectRatio                = 0x0080;
        public const int TagBlurControl                = 0x0082;
        public const int TagHDR                        = 0x0085;
        public const int TagShutterType                = 0x0087;
        public const int TagNeutralDensityFilter       = 0x0088;
        public const int TagISO                        = 0x008B;
        public const int TagIntervalShooting           = 0x0092;
        public const int TagSkinToneCorrection         = 0x0095;
        public const int TagClarityControl             = 0x0096;
        public const int TagBlackPoint                 = 0x0200;
        public const int TagWhitePoint                 = 0x0201;
        public const int TagColorMatrixA               = 0x0203;
        public const int TagColorMatrixB               = 0x0204;
        public const int TagCameraSettings             = 0x0205;
        public const int TagAEInfo                     = 0x0206;
        public const int TagLensInfo                   = 0x0207;
        public const int TagFlashInfo                  = 0x0208;
        public const int TagAEMeteringSegments         = 0x0209;
        public const int TagFlashMeteringSegments      = 0x020A;
        public const int TagSlaveFlashMeteringSegments = 0x020B;
        public const int TagWB_RGGBLevelsDaylight      = 0x020D;
        public const int TagWB_RGGBLevelsShade         = 0x020E;
        public const int TagWB_RGGBLevelsCloudy        = 0x020F;
        public const int TagWB_RGGBLevelsTungsten      = 0x0210;
        public const int TagWB_RGGBLevelsFluorescentD  = 0x0211;
        public const int TagWB_RGGBLevelsFluorescentN  = 0x0212;
        public const int TagWB_RGGBLevelsFluorescentW  = 0x0213;
        public const int TagWB_RGGBLevelsFlash         = 0x0214;
        public const int TagCameraInfo                 = 0x0215;
        public const int TagBatteryInfo                = 0x0216;
        public const int TagSaturationInfo             = 0x021B;
        public const int TagColorMatrixA2              = 0x021C;
        public const int TagColorMatrixB2              = 0x021D;
        public const int TagAFInfo                     = 0x021F;
        public const int TagHuffmanTable               = 0x0220;
        public const int TagKelvinWB                   = 0x0221;
        public const int TagColorInfo                  = 0x0222;
        public const int TagEVStepInfo                 = 0x0224;
        public const int TagShotInfo                   = 0x0226;
        public const int TagFacePos                    = 0x0227;
        public const int TagFaceSize                   = 0x0228;
        public const int TagSerialNumber               = 0x0229;
        public const int TagFilterInfo                 = 0x022A;
        public const int TagLevelInfo                  = 0x022B;
        public const int TagWBLevels                   = 0x022D;
        public const int TagArtist                     = 0x022E;
        public const int TagCopyright                  = 0x022F;
        public const int TagFirmwareVersion            = 0x0230;
        public const int TagContrastDetectAFArea       = 0x0231;
        public const int TagCrossProcessParams         = 0x0235;
        public const int TagLensInfoQ                  = 0x0239;
        public const int TagModel                      = 0x023F;
        public const int TagPixelShiftInfo             = 0x0243;
        public const int TagAFPointInfo                = 0x0245;
        public const int TagDataDump                   = 0x02FE;
        public const int TagTempInfo                   = 0x03FF;
        public const int TagToneCurve                  = 0x0402;
        public const int TagToneCurves                 = 0x0403;
        public const int TagUnknownBlock               = 0x0405;
        public const int TagPrintIM                    = 0x0E00;
#pragma warning restore format

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagPentaxVersion, "PentaxVersion" },
            { TagPentaxModelType, "PentaxModelType" },
            { TagPreviewImageSize, "PreviewImageSize" },
            { TagPreviewImageLength, "PreviewImageLength" },
            { TagPreviewImageStart, "PreviewImageStart" },
            { TagPentaxModelId, "PentaxModelId" },
            { TagDate, "Date" },
            { TagTime, "Time" },
            { TagQuality, "Quality" },
            { TagPentaxImageSize, "PentaxImageSize" },
            { TagPictureMode, "PictureMode" },
            { TagFlashMode, "FlashMode" },
            { TagFocusMode, "FocusMode" },
            { TagAFPointSelected, "AFPointSelected" },
            { TagAFPointsInFocus, "AFPointsInFocus" },
            { TagFocusPosition, "FocusPosition" },
            { TagExposureTime, "ExposureTime" },
            { TagFNumber, "FNumber" },
            { TagIso, "Iso" },
            { TagLightReading, "LightReading" },
            { TagExposureCompensation, "ExposureCompensation" },
            { TagMeteringMode, "MeteringMode" },
            { TagAutoBracketing, "AutoBracketing" },
            { TagWhiteBalance, "WhiteBalance" },
            { TagWhiteBalanceMode, "WhiteBalanceMode" },
            { TagBlueBalance, "BlueBalance" },
            { TagRedBalance, "RedBalance" },
            { TagFocalLength, "FocalLength" },
            { TagDigitalZoom, "DigitalZoom" },
            { TagContrast, "Contrast" },
            { TagSaturation, "Saturation" },
            { TagSharpness, "Sharpness" },
            { TagWorldTimeLocation, "WorldTimeLocation" },
            { TagHometownCity, "HometownCity" },
            { TagDestinationCity, "DestinationCity" },
            { TagHometownDST, "HometownDST" },
            { TagDestinationDST, "DestinationDST" },
            { TagDSPFirmwareVersion, "DSPFirmwareVersion" },
            { TagCPUFirmwareVersion, "CPUFirmwareVersion" },
            { TagFrameNumber, "FrameNumber" },
            { TagEffectiveLV, "EffectiveLV" },
            { TagImageEditing, "ImageEditing" },
            { TagPictureMode2, "PictureMode2" },
            { TagDriveMode, "DriveMode" },
            { TagSensorSize, "SensorSize" },
            { TagColorSpace, "ColorSpace" },
            { TagImageAreaOffset, "ImageAreaOffset" },
            { TagRawImageSize, "RawImageSize" },
            { TagAFPointsInFocus2, "AFPointsInFocus2" },
            { TagDataScaling, "DataScaling" },
            { TagPreviewImageBorders, "PreviewImageBorders" },
            { TagLensRec, "LensRec" },
            { TagSensitivityAdjust, "SensitivityAdjust" },
            { TagImageEditCount, "ImageEditCount" },
            { TagCameraTemperature, "CameraTemperature" },
            { TagAELock, "AELock" },
            { TagNoiseReduction, "NoiseReduction" },
            { TagFlashExposureComp, "FlashExposureComp" },
            { TagImageTone, "ImageTone" },
            { TagColorTemperature, "ColorTemperature" },
            { TagColorTempDaylight, "ColorTempDaylight" },
            { TagColorTempShade, "ColorTempShade" },
            { TagColorTempCloudy, "ColorTempCloudy" },
            { TagColorTempTungsten, "ColorTempTungsten" },
            { TagColorTempFluorescentD, "ColorTempFluorescentD" },
            { TagColorTempFluorescentN, "ColorTempFluorescentN" },
            { TagColorTempFluorescentW, "ColorTempFluorescentW" },
            { TagColorTempFlash, "ColorTempFlash" },
            { TagShakeReductionInfo, "ShakeReductionInfo" },
            { TagShutterCount, "ShutterCount" },
            { TagFaceInfo, "FaceInfo" },
            { TagRawDevelopmentProcess, "RawDevelopmentProcess" },
            { TagHue, "Hue" },
            { TagAWBInfo, "AWBInfo" },
            { TagDynamicRangeExpansion, "DynamicRangeExpansion" },
            { TagTimeInfo, "TimeInfo" },
            { TagHighLowKeyAdj, "HighLowKeyAdj" },
            { TagContrastHighlight, "ContrastHighlight" },
            { TagContrastShadow, "ContrastShadow" },
            { TagContrastHighlightShadowAdj, "ContrastHighlightShadowAdj" },
            { TagFineSharpness, "FineSharpness" },
            { TagHighISONoiseReduction, "HighISONoiseReduction" },
            { TagAFAdjustment, "AFAdjustment" },
            { TagMonochromeFilterEffect, "MonochromeFilterEffect" },
            { TagMonochromeToning, "MonochromeToning" },
            { TagFaceDetect, "FaceDetect" },
            { TagFaceDetectFrameSize, "FaceDetectFrameSize" },
            { TagShadowCorrection, "ShadowCorrection" },
            { TagISOAutoParameters, "ISOAutoParameters" },
            { TagCrossProcess, "CrossProcess" },
            { TagLensCorr, "LensCorr" },
            { TagWhiteLevel, "WhiteLevel" },
            { TagBleachBypassToning, "BleachBypassToning" },
            { TagAspectRatio, "AspectRatio" },
            { TagBlurControl, "BlurControl" },
            { TagHDR, "HDR" },
            { TagShutterType, "ShutterType" },
            { TagNeutralDensityFilter, "NeutralDensityFilter" },
            { TagISO, "ISO" },
            { TagIntervalShooting, "IntervalShooting" },
            { TagSkinToneCorrection, "SkinToneCorrection" },
            { TagClarityControl, "ClarityControl" },
            { TagBlackPoint, "BlackPoint" },
            { TagWhitePoint, "WhitePoint" },
            { TagColorMatrixA, "ColorMatrixA" },
            { TagColorMatrixB, "ColorMatrixB" },
            { TagCameraSettings, "CameraSettings" },
            { TagAEInfo, "AEInfo" },
            { TagLensInfo, "LensInfo" },
            { TagFlashInfo, "FlashInfo" },
            { TagAEMeteringSegments, "AEMeteringSegments" },
            { TagFlashMeteringSegments, "FlashMeteringSegments" },
            { TagSlaveFlashMeteringSegments, "SlaveFlashMeteringSegments" },
            { TagWB_RGGBLevelsDaylight, "WB_RGGBLevelsDaylight" },
            { TagWB_RGGBLevelsShade, "WB_RGGBLevelsShade" },
            { TagWB_RGGBLevelsCloudy, "WB_RGGBLevelsCloudy" },
            { TagWB_RGGBLevelsTungsten, "WB_RGGBLevelsTungsten" },
            { TagWB_RGGBLevelsFluorescentD, "WB_RGGBLevelsFluorescentD" },
            { TagWB_RGGBLevelsFluorescentN, "WB_RGGBLevelsFluorescentN" },
            { TagWB_RGGBLevelsFluorescentW, "WB_RGGBLevelsFluorescentW" },
            { TagWB_RGGBLevelsFlash, "WB_RGGBLevelsFlash" },
            { TagCameraInfo, "CameraInfo" },
            { TagBatteryInfo, "BatteryInfo" },
            { TagSaturationInfo, "SaturationInfo" },
            { TagColorMatrixA2, "ColorMatrixA2" },
            { TagColorMatrixB2, "ColorMatrixB2" },
            { TagAFInfo, "AFInfo" },
            { TagHuffmanTable, "HuffmanTable" },
            { TagKelvinWB, "KelvinWB" },
            { TagColorInfo, "ColorInfo" },
            { TagEVStepInfo, "EVStepInfo" },
            { TagShotInfo, "ShotInfo" },
            { TagFacePos, "FacePos" },
            { TagFaceSize, "FaceSize" },
            { TagSerialNumber, "SerialNumber" },
            { TagFilterInfo, "FilterInfo" },
            { TagLevelInfo, "LevelInfo" },
            { TagWBLevels, "WBLevels" },
            { TagArtist, "Artist" },
            { TagCopyright, "Copyright" },
            { TagFirmwareVersion, "FirmwareVersion" },
            { TagContrastDetectAFArea, "ContrastDetectAFArea" },
            { TagCrossProcessParams, "CrossProcessParams" },
            { TagLensInfoQ, "LensInfoQ" },
            { TagModel, "Model" },
            { TagPixelShiftInfo, "PixelShiftInfo" },
            { TagAFPointInfo, "AFPointInfo" },
            { TagDataDump, "DataDump" },
            { TagTempInfo, "TempInfo" },
            { TagToneCurve, "ToneCurve" },
            { TagToneCurves, "ToneCurves" },
            { TagUnknownBlock, "UnknownBlock" },
            { TagPrintIM, "PrintIM" }
        };

        public PentaxType2MakernoteDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new PentaxType2MakernoteDescriptor(this));
        }

        public override string Name => "Pentax Makernote";
    }
}
