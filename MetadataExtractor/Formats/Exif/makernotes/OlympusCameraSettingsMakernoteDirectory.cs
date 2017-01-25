#region License
//
// Copyright 2002-2017 Drew Noakes
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
    /// <summary>
    /// The Olympus camera settings makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusCameraSettingsMakernoteDirectory : Directory
    {
        public const int TagCameraSettingsVersion = 0x0000;
        public const int TagPreviewImageValid = 0x0100;
        public const int TagPreviewImageStart = 0x0101;
        public const int TagPreviewImageLength = 0x0102;

        public const int TagExposureMode = 0x0200;
        public const int TagAeLock = 0x0201;
        public const int TagMeteringMode = 0x0202;
        public const int TagExposureShift = 0x0203;
        public const int TagNdFilter = 0x0204;

        public const int TagMacroMode = 0x0300;
        public const int TagFocusMode = 0x0301;
        public const int TagFocusProcess = 0x0302;
        public const int TagAfSearch = 0x0303;
        public const int TagAfAreas = 0x0304;
        public const int TagAfPointSelected = 0x0305;
        public const int TagAfFineTune = 0x0306;
        public const int TagAfFineTuneAdj = 0x0307;

        public const int TagFlashMode = 0x400;
        public const int TagFlashExposureComp = 0x401;
        public const int TagFlashRemoteControl = 0x403;
        public const int TagFlashControlMode = 0x404;
        public const int TagFlashIntensity = 0x405;
        public const int TagManualFlashStrength = 0x406;

        public const int TagWhiteBalance2 = 0x500;
        public const int TagWhiteBalanceTemperature = 0x501;
        public const int TagWhiteBalanceBracket = 0x502;
        public const int TagCustomSaturation = 0x503;
        public const int TagModifiedSaturation = 0x504;
        public const int TagContrastSetting = 0x505;
        public const int TagSharpnessSetting = 0x506;
        public const int TagColorSpace = 0x507;
        public const int TagSceneMode = 0x509;
        public const int TagNoiseReduction = 0x50a;
        public const int TagDistortionCorrection = 0x50b;
        public const int TagShadingCompensation = 0x50c;
        public const int TagCompressionFactor = 0x50d;
        public const int TagGradation = 0x50f;
        public const int TagPictureMode = 0x520;
        public const int TagPictureModeSaturation = 0x521;
        public const int TagPictureModeHue = 0x522;
        public const int TagPictureModeContrast = 0x523;
        public const int TagPictureModeSharpness = 0x524;
        public const int TagPictureModeBWFilter = 0x525;
        public const int TagPictureModeTone = 0x526;
        public const int TagNoiseFilter = 0x527;
        public const int TagArtFilter = 0x529;
        public const int TagMagicFilter = 0x52c;
        public const int TagPictureModeEffect = 0x52d;
        public const int TagToneLevel = 0x52e;
        public const int TagArtFilterEffect = 0x52f;
        public const int TagColorCreatorEffect = 0x532;

        public const int TagDriveMode = 0x600;
        public const int TagPanoramaMode = 0x601;
        public const int TagImageQuality2 = 0x603;
        public const int TagImageStabilization = 0x604;

        public const int TagStackedImage = 0x804;

        public const int TagManometerPressure = 0x900;
        public const int TagManometerReading = 0x901;
        public const int TagExtendedWBDetect = 0x902;
        public const int TagRollAngle = 0x903;
        public const int TagPitchAngle = 0x904;
        public const int TagDateTimeUtc = 0x908;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagCameraSettingsVersion, "Camera Settings Version" },
            { TagPreviewImageValid, "Preview Image Valid" },
            { TagPreviewImageStart, "Preview Image Start" },
            { TagPreviewImageLength, "Preview Image Length" },

            { TagExposureMode, "Exposure Mode" },
            { TagAeLock, "AE Lock" },
            { TagMeteringMode, "Metering Mode" },
            { TagExposureShift, "Exposure Shift" },
            { TagNdFilter, "ND Filter" },

            { TagMacroMode, "Macro Mode" },
            { TagFocusMode, "Focus Mode" },
            { TagFocusProcess, "Focus Process" },
            { TagAfSearch, "AF Search" },
            { TagAfAreas, "AF Areas" },
            { TagAfPointSelected, "AF Point Selected" },
            { TagAfFineTune, "AF Fine Tune" },
            { TagAfFineTuneAdj, "AF Fine Tune Adj" },

            { TagFlashMode, "Flash Mode" },
            { TagFlashExposureComp, "Flash Exposure Comp" },
            { TagFlashRemoteControl, "Flash Remote Control" },
            { TagFlashControlMode, "Flash Control Mode" },
            { TagFlashIntensity, "Flash Intensity" },
            { TagManualFlashStrength, "Manual Flash Strength" },

            { TagWhiteBalance2, "White Balance 2" },
            { TagWhiteBalanceTemperature, "White Balance Temperature" },
            { TagWhiteBalanceBracket, "White Balance Bracket" },
            { TagCustomSaturation, "Custom Saturation" },
            { TagModifiedSaturation, "Modified Saturation" },
            { TagContrastSetting, "Contrast Setting" },
            { TagSharpnessSetting, "Sharpness Setting" },
            { TagColorSpace, "Color Space" },
            { TagSceneMode, "Scene Mode" },
            { TagNoiseReduction, "Noise Reduction" },
            { TagDistortionCorrection, "Distortion Correction" },
            { TagShadingCompensation, "Shading Compensation" },
            { TagCompressionFactor, "Compression Factor" },
            { TagGradation, "Gradation" },
            { TagPictureMode, "Picture Mode" },
            { TagPictureModeSaturation, "Picture Mode Saturation" },
            { TagPictureModeHue, "Picture Mode Hue" },
            { TagPictureModeContrast, "Picture Mode Contrast" },
            { TagPictureModeSharpness, "Picture Mode Sharpness" },
            { TagPictureModeBWFilter, "Picture Mode BW Filter" },
            { TagPictureModeTone, "Picture Mode Tone" },
            { TagNoiseFilter, "Noise Filter" },
            { TagArtFilter, "Art Filter" },
            { TagMagicFilter, "Magic Filter" },
            { TagPictureModeEffect, "Picture Mode Effect" },
            { TagToneLevel, "Tone Level" },
            { TagArtFilterEffect, "Art Filter Effect" },
            { TagColorCreatorEffect, "Color Creator Effect" },

            { TagDriveMode, "Drive Mode" },
            { TagPanoramaMode, "Panorama Mode" },
            { TagImageQuality2, "Image Quality 2" },
            { TagImageStabilization, "Image Stabilization" },

            { TagStackedImage, "Stacked Image" },

            { TagManometerPressure, "Manometer Pressure" },
            { TagManometerReading, "Manometer Reading" },
            { TagExtendedWBDetect, "Extended WB Detect" },
            { TagRollAngle, "Roll Angle" },
            { TagPitchAngle, "Pitch Angle" },
            { TagDateTimeUtc, "Date Time UTC" }
        };

        public OlympusCameraSettingsMakernoteDirectory()
        {
            SetDescriptor(new OlympusCameraSettingsMakernoteDescriptor(this));
        }

        public override string Name => "Olympus Camera Settings";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
