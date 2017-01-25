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
    /// <summary>Describes tags specific to Fujifilm cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FujifilmMakernoteDirectory : Directory
    {
        public const int TagMakernoteVersion = 0x0000;
        public const int TagSerialNumber = 0x0010;
        public const int TagQuality = 0x1000;
        public const int TagSharpness = 0x1001;
        public const int TagWhiteBalance = 0x1002;
        public const int TagColorSaturation = 0x1003;
        public const int TagTone = 0x1004;
        public const int TagColorTemperature = 0x1005;
        public const int TagContrast = 0x1006;
        public const int TagWhiteBalanceFineTune = 0x100a;
        public const int TagNoiseReduction = 0x100b;
        public const int TagHighIsoNoiseReduction = 0x100e;
        public const int TagFlashMode = 0x1010;
        public const int TagFlashEv = 0x1011;
        public const int TagMacro = 0x1020;
        public const int TagFocusMode = 0x1021;
        public const int TagFocusPixel = 0x1023;
        public const int TagSlowSync = 0x1030;
        public const int TagPictureMode = 0x1031;
        public const int TagExrAuto = 0x1033;
        public const int TagExrMode = 0x1034;
        public const int TagAutoBracketing = 0x1100;
        public const int TagSequenceNumber = 0x1101;
        public const int TagFinePixColor = 0x1210;
        public const int TagBlurWarning = 0x1300;
        public const int TagFocusWarning = 0x1301;
        public const int TagAutoExposureWarning = 0x1302;
        public const int TagGeImageSize = 0x1304;
        public const int TagDynamicRange = 0x1400;
        public const int TagFilmMode = 0x1401;
        public const int TagDynamicRangeSetting = 0x1402;
        public const int TagDevelopmentDynamicRange = 0x1403;
        public const int TagMinFocalLength = 0x1404;
        public const int TagMaxFocalLength = 0x1405;
        public const int TagMaxApertureAtMinFocal = 0x1406;
        public const int TagMaxApertureAtMaxFocal = 0x1407;
        public const int TagAutoDynamicRange = 0x140b;
        public const int TagFacesDetected = 0x4100;
        /// <summary>Left, top, right and bottom coordinates in full-sized image for each face detected.</summary>
        public const int TagFacePositions = 0x4103;
        public const int TagFaceRecInfo = 0x4282;
        public const int TagFileSource = 0x8000;
        public const int TagOrderNumber = 0x8002;
        public const int TagFrameNumber = 0x8003;
        public const int TagParallax = 0xb211;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakernoteVersion, "Makernote Version" },
            { TagSerialNumber, "Serial Number" },
            { TagQuality, "Quality" },
            { TagSharpness, "Sharpness" },
            { TagWhiteBalance, "White Balance" },
            { TagColorSaturation, "Color Saturation" },
            { TagTone, "Tone (Contrast)" },
            { TagColorTemperature, "Color Temperature" },
            { TagContrast, "Contrast" },
            { TagWhiteBalanceFineTune, "White Balance Fine Tune" },
            { TagNoiseReduction, "Noise Reduction" },
            { TagHighIsoNoiseReduction, "High ISO Noise Reduction" },
            { TagFlashMode, "Flash Mode" },
            { TagFlashEv, "Flash Strength" },
            { TagMacro, "Macro" },
            { TagFocusMode, "Focus Mode" },
            { TagFocusPixel, "Focus Pixel" },
            { TagSlowSync, "Slow Sync" },
            { TagPictureMode, "Picture Mode" },
            { TagExrAuto, "EXR Auto" },
            { TagExrMode, "EXR Mode" },
            { TagAutoBracketing, "Auto Bracketing" },
            { TagSequenceNumber, "Sequence Number" },
            { TagFinePixColor, "FinePix Color Setting" },
            { TagBlurWarning, "Blur Warning" },
            { TagFocusWarning, "Focus Warning" },
            { TagAutoExposureWarning, "AE Warning" },
            { TagGeImageSize, "GE Image Size" },
            { TagDynamicRange, "Dynamic Range" },
            { TagFilmMode, "Film Mode" },
            { TagDynamicRangeSetting, "Dynamic Range Setting" },
            { TagDevelopmentDynamicRange, "Development Dynamic Range" },
            { TagMinFocalLength, "Minimum Focal Length" },
            { TagMaxFocalLength, "Maximum Focal Length" },
            { TagMaxApertureAtMinFocal, "Maximum Aperture at Minimum Focal Length" },
            { TagMaxApertureAtMaxFocal, "Maximum Aperture at Maximum Focal Length" },
            { TagAutoDynamicRange, "Auto Dynamic Range" },
            { TagFacesDetected, "Faces Detected" },
            { TagFacePositions, "Face Positions" },
            { TagFaceRecInfo, "Face Detection Data" },
            { TagFileSource, "File Source" },
            { TagOrderNumber, "Order Number" },
            { TagFrameNumber, "Frame Number" },
            { TagParallax, "Parallax" }
        };

        public FujifilmMakernoteDirectory()
        {
            SetDescriptor(new FujifilmMakernoteDescriptor(this));
        }

        public override string Name => "Fujifilm Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
