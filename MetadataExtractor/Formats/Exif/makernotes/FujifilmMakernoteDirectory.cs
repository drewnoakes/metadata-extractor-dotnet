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

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Fujifilm cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
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

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static FujifilmMakernoteDirectory()
        {
            TagNameMap[TagMakernoteVersion] = "Makernote Version";
            TagNameMap[TagSerialNumber] = "Serial Number";
            TagNameMap[TagQuality] = "Quality";
            TagNameMap[TagSharpness] = "Sharpness";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagColorSaturation] = "Color Saturation";
            TagNameMap[TagTone] = "Tone (Contrast)";
            TagNameMap[TagColorTemperature] = "Color Temperature";
            TagNameMap[TagContrast] = "Contrast";
            TagNameMap[TagWhiteBalanceFineTune] = "White Balance Fine Tune";
            TagNameMap[TagNoiseReduction] = "Noise Reduction";
            TagNameMap[TagHighIsoNoiseReduction] = "High ISO Noise Reduction";
            TagNameMap[TagFlashMode] = "Flash Mode";
            TagNameMap[TagFlashEv] = "Flash Strength";
            TagNameMap[TagMacro] = "Macro";
            TagNameMap[TagFocusMode] = "Focus Mode";
            TagNameMap[TagFocusPixel] = "Focus Pixel";
            TagNameMap[TagSlowSync] = "Slow Sync";
            TagNameMap[TagPictureMode] = "Picture Mode";
            TagNameMap[TagExrAuto] = "EXR Auto";
            TagNameMap[TagExrMode] = "EXR Mode";
            TagNameMap[TagAutoBracketing] = "Auto Bracketing";
            TagNameMap[TagSequenceNumber] = "Sequence Number";
            TagNameMap[TagFinePixColor] = "FinePix Color Setting";
            TagNameMap[TagBlurWarning] = "Blur Warning";
            TagNameMap[TagFocusWarning] = "Focus Warning";
            TagNameMap[TagAutoExposureWarning] = "AE Warning";
            TagNameMap[TagGeImageSize] = "GE Image Size";
            TagNameMap[TagDynamicRange] = "Dynamic Range";
            TagNameMap[TagFilmMode] = "Film Mode";
            TagNameMap[TagDynamicRangeSetting] = "Dynamic Range Setting";
            TagNameMap[TagDevelopmentDynamicRange] = "Development Dynamic Range";
            TagNameMap[TagMinFocalLength] = "Minimum Focal Length";
            TagNameMap[TagMaxFocalLength] = "Maximum Focal Length";
            TagNameMap[TagMaxApertureAtMinFocal] = "Maximum Aperture at Minimum Focal Length";
            TagNameMap[TagMaxApertureAtMaxFocal] = "Maximum Aperture at Maximum Focal Length";
            TagNameMap[TagAutoDynamicRange] = "Auto Dynamic Range";
            TagNameMap[TagFacesDetected] = "Faces Detected";
            TagNameMap[TagFacePositions] = "Face Positions";
            TagNameMap[TagFaceRecInfo] = "Face Detection Data";
            TagNameMap[TagFileSource] = "File Source";
            TagNameMap[TagOrderNumber] = "Order Number";
            TagNameMap[TagFrameNumber] = "Frame Number";
            TagNameMap[TagParallax] = "Parallax";
        }

        public FujifilmMakernoteDirectory()
        {
            SetDescriptor(new FujifilmMakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Fujifilm Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
