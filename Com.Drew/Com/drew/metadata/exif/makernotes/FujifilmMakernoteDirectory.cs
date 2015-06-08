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
    /// <summary>Describes tags specific to Fujifilm cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class FujifilmMakernoteDirectory : Directory
    {
        public const int TagMakernoteVersion = unchecked((int)(0x0000));

        public const int TagSerialNumber = unchecked((int)(0x0010));

        public const int TagQuality = unchecked((int)(0x1000));

        public const int TagSharpness = unchecked((int)(0x1001));

        public const int TagWhiteBalance = unchecked((int)(0x1002));

        public const int TagColorSaturation = unchecked((int)(0x1003));

        public const int TagTone = unchecked((int)(0x1004));

        public const int TagColorTemperature = unchecked((int)(0x1005));

        public const int TagContrast = unchecked((int)(0x1006));

        public const int TagWhiteBalanceFineTune = unchecked((int)(0x100a));

        public const int TagNoiseReduction = unchecked((int)(0x100b));

        public const int TagHighIsoNoiseReduction = unchecked((int)(0x100e));

        public const int TagFlashMode = unchecked((int)(0x1010));

        public const int TagFlashEv = unchecked((int)(0x1011));

        public const int TagMacro = unchecked((int)(0x1020));

        public const int TagFocusMode = unchecked((int)(0x1021));

        public const int TagFocusPixel = unchecked((int)(0x1023));

        public const int TagSlowSync = unchecked((int)(0x1030));

        public const int TagPictureMode = unchecked((int)(0x1031));

        public const int TagExrAuto = unchecked((int)(0x1033));

        public const int TagExrMode = unchecked((int)(0x1034));

        public const int TagAutoBracketing = unchecked((int)(0x1100));

        public const int TagSequenceNumber = unchecked((int)(0x1101));

        public const int TagFinePixColor = unchecked((int)(0x1210));

        public const int TagBlurWarning = unchecked((int)(0x1300));

        public const int TagFocusWarning = unchecked((int)(0x1301));

        public const int TagAutoExposureWarning = unchecked((int)(0x1302));

        public const int TagGeImageSize = unchecked((int)(0x1304));

        public const int TagDynamicRange = unchecked((int)(0x1400));

        public const int TagFilmMode = unchecked((int)(0x1401));

        public const int TagDynamicRangeSetting = unchecked((int)(0x1402));

        public const int TagDevelopmentDynamicRange = unchecked((int)(0x1403));

        public const int TagMinFocalLength = unchecked((int)(0x1404));

        public const int TagMaxFocalLength = unchecked((int)(0x1405));

        public const int TagMaxApertureAtMinFocal = unchecked((int)(0x1406));

        public const int TagMaxApertureAtMaxFocal = unchecked((int)(0x1407));

        public const int TagAutoDynamicRange = unchecked((int)(0x140b));

        public const int TagFacesDetected = unchecked((int)(0x4100));

        /// <summary>Left, top, right and bottom coordinates in full-sized image for each face detected.</summary>
        public const int TagFacePositions = unchecked((int)(0x4103));

        public const int TagFaceRecInfo = unchecked((int)(0x4282));

        public const int TagFileSource = unchecked((int)(0x8000));

        public const int TagOrderNumber = unchecked((int)(0x8002));

        public const int TagFrameNumber = unchecked((int)(0x8003));

        public const int TagParallax = unchecked((int)(0xb211));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static FujifilmMakernoteDirectory()
        {
            _tagNameMap.Put(TagMakernoteVersion, "Makernote Version");
            _tagNameMap.Put(TagSerialNumber, "Serial Number");
            _tagNameMap.Put(TagQuality, "Quality");
            _tagNameMap.Put(TagSharpness, "Sharpness");
            _tagNameMap.Put(TagWhiteBalance, "White Balance");
            _tagNameMap.Put(TagColorSaturation, "Color Saturation");
            _tagNameMap.Put(TagTone, "Tone (Contrast)");
            _tagNameMap.Put(TagColorTemperature, "Color Temperature");
            _tagNameMap.Put(TagContrast, "Contrast");
            _tagNameMap.Put(TagWhiteBalanceFineTune, "White Balance Fine Tune");
            _tagNameMap.Put(TagNoiseReduction, "Noise Reduction");
            _tagNameMap.Put(TagHighIsoNoiseReduction, "High ISO Noise Reduction");
            _tagNameMap.Put(TagFlashMode, "Flash Mode");
            _tagNameMap.Put(TagFlashEv, "Flash Strength");
            _tagNameMap.Put(TagMacro, "Macro");
            _tagNameMap.Put(TagFocusMode, "Focus Mode");
            _tagNameMap.Put(TagFocusPixel, "Focus Pixel");
            _tagNameMap.Put(TagSlowSync, "Slow Sync");
            _tagNameMap.Put(TagPictureMode, "Picture Mode");
            _tagNameMap.Put(TagExrAuto, "EXR Auto");
            _tagNameMap.Put(TagExrMode, "EXR Mode");
            _tagNameMap.Put(TagAutoBracketing, "Auto Bracketing");
            _tagNameMap.Put(TagSequenceNumber, "Sequence Number");
            _tagNameMap.Put(TagFinePixColor, "FinePix Color Setting");
            _tagNameMap.Put(TagBlurWarning, "Blur Warning");
            _tagNameMap.Put(TagFocusWarning, "Focus Warning");
            _tagNameMap.Put(TagAutoExposureWarning, "AE Warning");
            _tagNameMap.Put(TagGeImageSize, "GE Image Size");
            _tagNameMap.Put(TagDynamicRange, "Dynamic Range");
            _tagNameMap.Put(TagFilmMode, "Film Mode");
            _tagNameMap.Put(TagDynamicRangeSetting, "Dynamic Range Setting");
            _tagNameMap.Put(TagDevelopmentDynamicRange, "Development Dynamic Range");
            _tagNameMap.Put(TagMinFocalLength, "Minimum Focal Length");
            _tagNameMap.Put(TagMaxFocalLength, "Maximum Focal Length");
            _tagNameMap.Put(TagMaxApertureAtMinFocal, "Maximum Aperture at Minimum Focal Length");
            _tagNameMap.Put(TagMaxApertureAtMaxFocal, "Maximum Aperture at Maximum Focal Length");
            _tagNameMap.Put(TagAutoDynamicRange, "Auto Dynamic Range");
            _tagNameMap.Put(TagFacesDetected, "Faces Detected");
            _tagNameMap.Put(TagFacePositions, "Face Positions");
            _tagNameMap.Put(TagFaceRecInfo, "Face Detection Data");
            _tagNameMap.Put(TagFileSource, "File Source");
            _tagNameMap.Put(TagOrderNumber, "Order Number");
            _tagNameMap.Put(TagFrameNumber, "Frame Number");
            _tagNameMap.Put(TagParallax, "Parallax");
        }

        public FujifilmMakernoteDirectory()
        {
            this.SetDescriptor(new FujifilmMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Fujifilm Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
