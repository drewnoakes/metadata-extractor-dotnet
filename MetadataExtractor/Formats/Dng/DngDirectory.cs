// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Dng;

/// <summary>Adobe Digital Negative tags, through DNG specification version 1.4.</summary>
/// <remarks>
/// Each directory contains the DNG tags of one TIFF image IFD. Its parent is the original
/// TIFF directory, which retains all raw tag values for compatibility.
/// Tag definitions follow the <see href="https://helpx.adobe.com/camera-raw/digital-negative.html">Adobe DNG specification</see>.
/// </remarks>
/// <author>twest820 https://github.com/twest820</author>
public sealed class DngDirectory : Directory
{
    public const int TagDngVersion = 0xC612;
    public const int TagDngBackwardVersion = 0xC613;
    public const int TagUniqueCameraModel = 0xC614;
    public const int TagLocalizedCameraModel = 0xC615;
    public const int TagCfaPlaneColor = 0xC616;
    public const int TagCfaLayout = 0xC617;
    public const int TagLinearizationTable = 0xC618;
    public const int TagBlackLevelRepeatDim = 0xC619;
    public const int TagBlackLevel = 0xC61A;
    public const int TagBlackLevelDeltaH = 0xC61B;
    public const int TagBlackLevelDeltaV = 0xC61C;
    public const int TagWhiteLevel = 0xC61D;
    public const int TagDefaultScale = 0xC61E;
    public const int TagDefaultCropOrigin = 0xC61F;
    public const int TagDefaultCropSize = 0xC620;
    public const int TagColorMatrix1 = 0xC621;
    public const int TagColorMatrix2 = 0xC622;
    public const int TagCameraCalibration1 = 0xC623;
    public const int TagCameraCalibration2 = 0xC624;
    public const int TagReductionMatrix1 = 0xC625;
    public const int TagReductionMatrix2 = 0xC626;
    public const int TagAnalogBalance = 0xC627;
    public const int TagAsShotNeutral = 0xC628;
    public const int TagAsShotWhiteXY = 0xC629;
    public const int TagBaselineExposure = 0xC62A;
    public const int TagBaselineNoise = 0xC62B;
    public const int TagBaselineSharpness = 0xC62C;
    public const int TagBayerGreenSplit = 0xC62D;
    public const int TagLinearResponseLimit = 0xC62E;
    public const int TagCameraSerialNumber = 0xC62F;
    public const int TagLensInfo = 0xC630;
    public const int TagChromaBlurRadius = 0xC631;
    public const int TagAntiAliasStrength = 0xC632;
    public const int TagShadowScale = 0xC633;
    public const int TagDngPrivateData = 0xC634;
    public const int TagMakerNoteSafety = 0xC635;
    public const int TagCalibrationIlluminant1 = 0xC65A;
    public const int TagCalibrationIlluminant2 = 0xC65B;
    public const int TagBestQualityScale = 0xC65C;
    public const int TagRawDataUniqueId = 0xC65D;
    public const int TagOriginalRawFileName = 0xC68B;
    public const int TagOriginalRawFileData = 0xC68C;
    public const int TagActiveArea = 0xC68D;
    public const int TagMaskedAreas = 0xC68E;
    public const int TagAsShotIccProfile = 0xC68F;
    public const int TagAsShotPreProfileMatrix = 0xC690;
    public const int TagCurrentIccProfile = 0xC691;
    public const int TagCurrentPreProfileMatrix = 0xC692;
    public const int TagColorimetricReference = 0xC6BF;
    public const int TagCameraCalibrationSignature = 0xC6F3;
    public const int TagProfileCalibrationSignature = 0xC6F4;
    public const int TagExtraCameraProfiles = 0xC6F5;
    public const int TagAsShotProfileName = 0xC6F6;
    public const int TagNoiseReductionApplied = 0xC6F7;
    public const int TagProfileName = 0xC6F8;
    public const int TagProfileHueSatMapDims = 0xC6F9;
    public const int TagProfileHueSatMapData1 = 0xC6FA;
    public const int TagProfileHueSatMapData2 = 0xC6FB;
    public const int TagProfileToneCurve = 0xC6FC;
    public const int TagProfileEmbedPolicy = 0xC6FD;
    public const int TagProfileCopyright = 0xC6FE;
    public const int TagForwardMatrix1 = 0xC714;
    public const int TagForwardMatrix2 = 0xC715;
    public const int TagPreviewApplicationName = 0xC716;
    public const int TagPreviewApplicationVersion = 0xC717;
    public const int TagPreviewSettingsName = 0xC718;
    public const int TagPreviewSettingsDigest = 0xC719;
    public const int TagPreviewColorSpace = 0xC71A;
    public const int TagPreviewDateTime = 0xC71B;
    public const int TagRawImageDigest = 0xC71C;
    public const int TagOriginalRawFileDigest = 0xC71D;
    public const int TagSubTileBlockSize = 0xC71E;
    public const int TagRowInterleaveFactor = 0xC71F;
    public const int TagProfileLookTableDims = 0xC725;
    public const int TagProfileLookTableData = 0xC726;
    public const int TagOpcodeList1 = 0xC740;
    public const int TagOpcodeList2 = 0xC741;
    public const int TagOpcodeList3 = 0xC74E;
    public const int TagNoiseProfile = 0xC761;
    public const int TagOriginalDefaultFinalSize = 0xC791;
    public const int TagOriginalBestQualityFinalSize = 0xC792;
    public const int TagOriginalDefaultCropSize = 0xC793;
    public const int TagProfileHueSatMapEncoding = 0xC7A3;
    public const int TagProfileLookTableEncoding = 0xC7A4;
    public const int TagBaselineExposureOffset = 0xC7A5;
    public const int TagDefaultBlackRender = 0xC7A6;
    public const int TagNewRawImageDigest = 0xC7A7;
    public const int TagRawToPreviewGain = 0xC7A8;
    public const int TagDefaultUserCrop = 0xC7B5;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagDngVersion, "DNG Version" },
        { TagDngBackwardVersion, "DNG Backward Version" },
        { TagUniqueCameraModel, "Unique Camera Model" },
        { TagLocalizedCameraModel, "Localized Camera Model" },
        { TagCfaPlaneColor, "CFA Plane Color" },
        { TagCfaLayout, "CFA Layout" },
        { TagLinearizationTable, "Linearization Table" },
        { TagBlackLevelRepeatDim, "Black Level Repeat Dimensions" },
        { TagBlackLevel, "Black Level" },
        { TagBlackLevelDeltaH, "Black Level Delta H" },
        { TagBlackLevelDeltaV, "Black Level Delta V" },
        { TagWhiteLevel, "White Level" },
        { TagDefaultScale, "Default Scale" },
        { TagDefaultCropOrigin, "Default Crop Origin" },
        { TagDefaultCropSize, "Default Crop Size" },
        { TagColorMatrix1, "Color Matrix 1" },
        { TagColorMatrix2, "Color Matrix 2" },
        { TagCameraCalibration1, "Camera Calibration 1" },
        { TagCameraCalibration2, "Camera Calibration 2" },
        { TagReductionMatrix1, "Reduction Matrix 1" },
        { TagReductionMatrix2, "Reduction Matrix 2" },
        { TagAnalogBalance, "Analog Balance" },
        { TagAsShotNeutral, "As Shot Neutral" },
        { TagAsShotWhiteXY, "As Shot White XY" },
        { TagBaselineExposure, "Baseline Exposure" },
        { TagBaselineNoise, "Baseline Noise" },
        { TagBaselineSharpness, "Baseline Sharpness" },
        { TagBayerGreenSplit, "Bayer Green Split" },
        { TagLinearResponseLimit, "Linear Response Limit" },
        { TagCameraSerialNumber, "Camera Serial Number" },
        { TagLensInfo, "Lens Info" },
        { TagChromaBlurRadius, "Chroma Blur Radius" },
        { TagAntiAliasStrength, "Anti Alias Strength" },
        { TagShadowScale, "Shadow Scale" },
        { TagDngPrivateData, "DNG Private Data" },
        { TagMakerNoteSafety, "Maker Note Safety" },
        { TagCalibrationIlluminant1, "Calibration Illuminant 1" },
        { TagCalibrationIlluminant2, "Calibration Illuminant 2" },
        { TagBestQualityScale, "Best Quality Scale" },
        { TagRawDataUniqueId, "Raw Data Unique ID" },
        { TagOriginalRawFileName, "Original Raw File Name" },
        { TagOriginalRawFileData, "Original Raw File Data" },
        { TagActiveArea, "Active Area" },
        { TagMaskedAreas, "Masked Areas" },
        { TagAsShotIccProfile, "As Shot ICC Profile" },
        { TagAsShotPreProfileMatrix, "As Shot Pre-profile Matrix" },
        { TagCurrentIccProfile, "Current ICC Profile" },
        { TagCurrentPreProfileMatrix, "Current Pre-profile Matrix" },
        { TagColorimetricReference, "Colorimetric Reference" },
        { TagCameraCalibrationSignature, "Camera Calibration Signature" },
        { TagProfileCalibrationSignature, "Profile Calibration Signature" },
        { TagExtraCameraProfiles, "Extra Camera Profiles" },
        { TagAsShotProfileName, "As Shot Profile Name" },
        { TagNoiseReductionApplied, "Noise Reduction Applied" },
        { TagProfileName, "Profile Name" },
        { TagProfileHueSatMapDims, "Profile Hue/Saturation Map Dimensions" },
        { TagProfileHueSatMapData1, "Profile Hue/Saturation Map Data 1" },
        { TagProfileHueSatMapData2, "Profile Hue/Saturation Map Data 2" },
        { TagProfileToneCurve, "Profile Tone Curve" },
        { TagProfileEmbedPolicy, "Profile Embed Policy" },
        { TagProfileCopyright, "Profile Copyright" },
        { TagForwardMatrix1, "Forward Matrix 1" },
        { TagForwardMatrix2, "Forward Matrix 2" },
        { TagPreviewApplicationName, "Preview Application Name" },
        { TagPreviewApplicationVersion, "Preview Application Version" },
        { TagPreviewSettingsName, "Preview Settings Name" },
        { TagPreviewSettingsDigest, "Preview Settings Digest" },
        { TagPreviewColorSpace, "Preview Color Space" },
        { TagPreviewDateTime, "Preview Date/Time" },
        { TagRawImageDigest, "Raw Image Digest" },
        { TagOriginalRawFileDigest, "Original Raw File Digest" },
        { TagSubTileBlockSize, "Sub-tile Block Size" },
        { TagRowInterleaveFactor, "Row Interleave Factor" },
        { TagProfileLookTableDims, "Profile Look Table Dimensions" },
        { TagProfileLookTableData, "Profile Look Table Data" },
        { TagOpcodeList1, "Opcode List 1" },
        { TagOpcodeList2, "Opcode List 2" },
        { TagOpcodeList3, "Opcode List 3" },
        { TagNoiseProfile, "Noise Profile" },
        { TagOriginalDefaultFinalSize, "Original Default Final Size" },
        { TagOriginalBestQualityFinalSize, "Original Best Quality Final Size" },
        { TagOriginalDefaultCropSize, "Original Default Crop Size" },
        { TagProfileHueSatMapEncoding, "Profile Hue/Saturation Map Encoding" },
        { TagProfileLookTableEncoding, "Profile Look Table Encoding" },
        { TagBaselineExposureOffset, "Baseline Exposure Offset" },
        { TagDefaultBlackRender, "Default Black Render" },
        { TagNewRawImageDigest, "New Raw Image Digest" },
        { TagRawToPreviewGain, "Raw to Preview Gain" },
        { TagDefaultUserCrop, "Default User Crop" }
    };

    public DngDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new DngDescriptor(this));
    }

    public override string Name => "DNG";
}
