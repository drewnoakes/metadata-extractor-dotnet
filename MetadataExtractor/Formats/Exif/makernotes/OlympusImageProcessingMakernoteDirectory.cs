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
    /// The Olympus image processing makernote is used by many manufacturers (Epson, Konica, Minolta and Agfa...), and as such contains some tags
    /// that appear specific to those manufacturers.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusImageProcessingMakernoteDirectory : Directory
    {
        public const int TagImageProcessingVersion = 0x0000;
        public const int TagWbRbLevels = 0x0100;
        // 0x0101 - in-camera AutoWB unless it is all 0's or all 256's (ref IB)
        public const int TagWbRbLevels3000K = 0x0102;
        public const int TagWbRbLevels3300K = 0x0103;
        public const int TagWbRbLevels3600K = 0x0104;
        public const int TagWbRbLevels3900K = 0x0105;
        public const int TagWbRbLevels4000K = 0x0106;
        public const int TagWbRbLevels4300K = 0x0107;
        public const int TagWbRbLevels4500K = 0x0108;
        public const int TagWbRbLevels4800K = 0x0109;
        public const int TagWbRbLevels5300K = 0x010a;
        public const int TagWbRbLevels6000K = 0x010b;
        public const int TagWbRbLevels6600K = 0x010c;
        public const int TagWbRbLevels7500K = 0x010d;
        public const int TagWbRbLevelsCwB1 = 0x010e;
        public const int TagWbRbLevelsCwB2 = 0x010f;
        public const int TagWbRbLevelsCwB3 = 0x0110;
        public const int TagWbRbLevelsCwB4 = 0x0111;
        public const int TagWbGLevel3000K = 0x0113;
        public const int TagWbGLevel3300K = 0x0114;
        public const int TagWbGLevel3600K = 0x0115;
        public const int TagWbGLevel3900K = 0x0116;
        public const int TagWbGLevel4000K = 0x0117;
        public const int TagWbGLevel4300K = 0x0118;
        public const int TagWbGLevel4500K = 0x0119;
        public const int TagWbGLevel4800K = 0x011a;
        public const int TagWbGLevel5300K = 0x011b;
        public const int TagWbGLevel6000K = 0x011c;
        public const int TagWbGLevel6600K = 0x011d;
        public const int TagWbGLevel7500K = 0x011e;
        public const int TagWbGLevel = 0x011f;
        // 0x0121 = WB preset for flash (about 6000K) (ref IB)
        // 0x0125 = WB preset for underwater (ref IB)

        public const int TagColorMatrix = 0x0200;
        // color matrices (ref 11):
        // 0x0201-0x020d are sRGB color matrices
        // 0x020e-0x021a are Adobe RGB color matrices
        // 0x021b-0x0227 are ProPhoto RGB color matrices
        // 0x0228 and 0x0229 are ColorMatrix for E-330
        // 0x0250-0x0252 are sRGB color matrices
        // 0x0253-0x0255 are Adobe RGB color matrices
        // 0x0256-0x0258 are ProPhoto RGB color matrices

        public const int TagEnhancer = 0x0300;
        public const int TagEnhancerValues = 0x0301;
        public const int TagCoringFilter = 0x0310;
        public const int TagCoringValues = 0x0311;
        public const int TagBlackLevel2 = 0x0600;
        public const int TagGainBase = 0x0610;
        public const int TagValidBits = 0x0611;
        public const int TagCropLeft = 0x0612;
        public const int TagCropTop = 0x0613;
        public const int TagCropWidth = 0x0614;
        public const int TagCropHeight = 0x0615;
        public const int TagUnknownBlock1 = 0x0635;
        public const int TagUnknownBlock2 = 0x0636;

        // 0x0800 LensDistortionParams, float[9] (ref 11)
        // 0x0801 LensShadingParams, int16u[16] (ref 11)
        public const int TagSensorCalibration = 0x0805;

        public const int TagNoiseReduction2 = 0x1010;
        public const int TagDistortionCorrection2 = 0x1011;
        public const int TagShadingCompensation2 = 0x1012;
        public const int TagMultipleExposureMode = 0x101c;
        public const int TagUnknownBlock3 = 0x1103;
        public const int TagUnknownBlock4 = 0x1104;
        public const int TagAspectRatio = 0x1112;
        public const int TagAspectFrame = 0x1113;
        public const int TagFacesDetected = 0x1200;
        public const int TagFaceDetectArea = 0x1201;
        public const int TagMaxFaces = 0x1202;
        public const int TagFaceDetectFrameSize = 0x1203;
        public const int TagFaceDetectFrameCrop = 0x1207;
        public const int TagCameraTemperature = 0x1306;

        public const int TagKeystoneCompensation = 0x1900;
        public const int TagKeystoneDirection = 0x1901;
        // 0x1905 - focal length (PH, E-M1)
        public const int TagKeystoneValue = 0x1906;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagImageProcessingVersion, "Image Processing Version" },
            { TagWbRbLevels, "WB RB Levels" },
            { TagWbRbLevels3000K, "WB RB Levels 3000K" },
            { TagWbRbLevels3300K, "WB RB Levels 3300K" },
            { TagWbRbLevels3600K, "WB RB Levels 3600K" },
            { TagWbRbLevels3900K, "WB RB Levels 3900K" },
            { TagWbRbLevels4000K, "WB RB Levels 4000K" },
            { TagWbRbLevels4300K, "WB RB Levels 4300K" },
            { TagWbRbLevels4500K, "WB RB Levels 4500K" },
            { TagWbRbLevels4800K, "WB RB Levels 4800K" },
            { TagWbRbLevels5300K, "WB RB Levels 5300K" },
            { TagWbRbLevels6000K, "WB RB Levels 6000K" },
            { TagWbRbLevels6600K, "WB RB Levels 6600K" },
            { TagWbRbLevels7500K, "WB RB Levels 7500K" },
            { TagWbRbLevelsCwB1, "WB RB Levels CWB1" },
            { TagWbRbLevelsCwB2, "WB RB Levels CWB2" },
            { TagWbRbLevelsCwB3, "WB RB Levels CWB3" },
            { TagWbRbLevelsCwB4, "WB RB Levels CWB4" },
            { TagWbGLevel3000K, "WB G Level 3000K" },
            { TagWbGLevel3300K, "WB G Level 3300K" },
            { TagWbGLevel3600K, "WB G Level 3600K" },
            { TagWbGLevel3900K, "WB G Level 3900K" },
            { TagWbGLevel4000K, "WB G Level 4000K" },
            { TagWbGLevel4300K, "WB G Level 4300K" },
            { TagWbGLevel4500K, "WB G Level 4500K" },
            { TagWbGLevel4800K, "WB G Level 4800K" },
            { TagWbGLevel5300K, "WB G Level 5300K" },
            { TagWbGLevel6000K, "WB G Level 6000K" },
            { TagWbGLevel6600K, "WB G Level 6600K" },
            { TagWbGLevel7500K, "WB G Level 7500K" },
            { TagWbGLevel, "WB G Level" },

            { TagColorMatrix, "Color Matrix" },

            { TagEnhancer, "Enhancer" },
            { TagEnhancerValues, "Enhancer Values" },
            { TagCoringFilter, "Coring Filter" },
            { TagCoringValues, "Coring Values" },
            { TagBlackLevel2, "Black Level 2" },
            { TagGainBase, "Gain Base" },
            { TagValidBits, "Valid Bits" },
            { TagCropLeft, "Crop Left" },
            { TagCropTop, "Crop Top" },
            { TagCropWidth, "Crop Width" },
            { TagCropHeight, "Crop Height" },
            { TagUnknownBlock1, "Unknown Block 1" },
            { TagUnknownBlock2, "Unknown Block 2" },

            { TagSensorCalibration, "Sensor Calibration" },

            { TagNoiseReduction2, "Noise Reduction 2" },
            { TagDistortionCorrection2, "Distortion Correction 2" },
            { TagShadingCompensation2, "Shading Compensation 2" },
            { TagMultipleExposureMode, "Multiple Exposure Mode" },
            { TagUnknownBlock3, "Unknown Block 3" },
            { TagUnknownBlock4, "Unknown Block 4" },
            { TagAspectRatio, "Aspect Ratio" },
            { TagAspectFrame, "Aspect Frame" },
            { TagFacesDetected, "Faces Detected" },
            { TagFaceDetectArea, "Face Detect Area" },
            { TagMaxFaces, "Max Faces" },
            { TagFaceDetectFrameSize, "Face Detect Frame Size" },
            { TagFaceDetectFrameCrop, "Face Detect Frame Crop" },
            { TagCameraTemperature , "Camera Temperature" },
            { TagKeystoneCompensation, "Keystone Compensation" },
            { TagKeystoneDirection, "Keystone Direction" },
            { TagKeystoneValue, "Keystone Value" }
        };

        public OlympusImageProcessingMakernoteDirectory()
        {
            SetDescriptor(new OlympusImageProcessingMakernoteDescriptor(this));
        }

        public override string Name => "Olympus Image Processing";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
