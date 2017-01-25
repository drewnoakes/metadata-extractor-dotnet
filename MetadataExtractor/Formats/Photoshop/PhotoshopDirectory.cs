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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Holds the metadata found in the APPD segment of a JPEG file saved by Photoshop.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Yuri Binev</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PhotoshopDirectory : Directory
    {
        public const int TagChannelsRowsColumnsDepthMode = 0x03E8;
        public const int TagMacPrintInfo = 0x03E9;
        public const int TagXml = 0x03EA;
        public const int TagIndexedColorTable = 0x03EB;
        public const int TagResolutionInfo = 0x03ED;
        public const int TagAlphaChannels = 0x03EE;
        public const int TagDisplayInfoObsolete = 0x03EF;
        public const int TagCaption = 0x03F0;
        public const int TagBorderInformation = 0x03F1;
        public const int TagBackgroundColor = 0x03F2;
        public const int TagPrintFlags = 0x03F3;
        public const int TagGrayscaleAndMultichannelHalftoningInformation = 0x03F4;
        public const int TagColorHalftoningInformation = 0x03F5;
        public const int TagDuotoneHalftoningInformation = 0x03F6;
        public const int TagGrayscaleAndMultichannelTransferFunction = 0x03F7;
        public const int TagColorTransferFunctions = 0x03F8;
        public const int TagDuotoneTransferFunctions = 0x03F9;
        public const int TagDuotoneImageInformation = 0x03FA;
        public const int TagEffectiveBlackAndWhiteValues = 0x03FB;
        // OBSOLETE                                                                     0x03FC
        public const int TagEpsOptions = 0x03FD;
        public const int TagQuickMaskInformation = 0x03FE;
        // OBSOLETE                                                                     0x03FF
        public const int TagLayerStateInformation = 0x0400;
        // Working path (not saved)                                                     0x0401
        public const int TagLayersGroupInformation = 0x0402;
        // OBSOLETE                                                                     0x0403
        public const int TagIptc = 0x0404;
        public const int TagImageModeForRawFormatFiles = 0x0405;
        public const int TagJpegQuality = 0x0406;
        public const int TagGridAndGuidesInformation = 0x0408;
        public const int TagThumbnailOld = 0x0409;
        public const int TagCopyright = 0x040A;
        public const int TagUrl = 0x040B;
        public const int TagThumbnail = 0x040C;
        public const int TagGlobalAngle = 0x040D;
        // OBSOLETE                                                                     0x040E
        public const int TagIccProfileBytes = 0x040F;
        public const int TagWatermark = 0x0410;
        public const int TagIccUntaggedProfile = 0x0411;
        public const int TagEffectsVisible = 0x0412;
        public const int TagSpotHalftone = 0x0413;
        public const int TagSeedNumber = 0x0414;
        public const int TagUnicodeAlphaNames = 0x0415;
        public const int TagIndexedColorTableCount = 0x0416;
        public const int TagTransparencyIndex = 0x0417;
        public const int TagGlobalAltitude = 0x0419;
        public const int TagSlices = 0x041A;
        public const int TagWorkflowUrl = 0x041B;
        public const int TagJumpToXpep = 0x041C;
        public const int TagAlphaIdentifiers = 0x041D;
        public const int TagUrlList = 0x041E;
        public const int TagVersion = 0x0421;
        public const int TagExifData1 = 0x0422;
        public const int TagExifData3 = 0x0423;
        public const int TagXmpData = 0x0424;
        public const int TagCaptionDigest = 0x0425;
        public const int TagPrintScale = 0x0426;
        public const int TagPixelAspectRatio = 0x0428;
        public const int TagLayerComps = 0x0429;
        public const int TagAlternateDuotoneColors = 0x042A;
        public const int TagAlternateSpotColors = 0x042B;
        public const int TagLayerSelectionIds = 0x042D;
        public const int TagHdrToningInfo = 0x042E;
        public const int TagPrintInfo = 0x042F;
        public const int TagLayerGroupsEnabledId = 0x0430;
        public const int TagColorSamplers = 0x0431;
        public const int TagMeasurementScale = 0x0432;
        public const int TagTimelineInformation = 0x0433;
        public const int TagSheetDisclosure = 0x0434;
        public const int TagDisplayInfo = 0x0435;
        public const int TagOnionSkins = 0x0436;
        public const int TagCountInformation = 0x0438;
        public const int TagPrintInfo2 = 0x043A;
        public const int TagPrintStyle = 0x043B;
        public const int TagMacNsprintinfo = 0x043C;
        public const int TagWinDevmode = 0x043D;
        public const int TagAutoSaveFilePath = 0x043E;
        public const int TagAutoSaveFormat = 0x043F;
        public const int TagPathSelectionState = 0x0440;
        // CLIPPING PATHS                                                               0x07D0 -> 0x0BB6
        public const int TagClippingPathName = 0x0BB7;
        public const int TagOriginPathInfo = 0x0BB8;
        // PLUG IN RESOURCES                                                            0x0FA0 -> 0x1387
        public const int TagImageReadyVariablesXml = 0x1B58;
        public const int TagImageReadyDataSets = 0x1B59;
        public const int TagLightroomWorkflow = 0x1F40;
        public const int TagPrintFlagsInfo = 0x2710;

        [NotNull]
        internal static readonly Dictionary<int, string> TagNameMap = new Dictionary<int, string>
        {
            { TagChannelsRowsColumnsDepthMode, "Channels, Rows, Columns, Depth, Mode" },
            { TagMacPrintInfo, "Mac Print Info" },
            { TagXml, "XML Data" },
            { TagIndexedColorTable, "Indexed Color Table" },
            { TagResolutionInfo, "Resolution Info" },
            { TagAlphaChannels, "Alpha Channels" },
            { TagDisplayInfoObsolete, "Display Info (Obsolete)" },
            { TagCaption, "Caption" },
            { TagBorderInformation, "Border Information" },
            { TagBackgroundColor, "Background Color" },
            { TagPrintFlags, "Print Flags" },
            { TagGrayscaleAndMultichannelHalftoningInformation, "Grayscale and Multichannel Halftoning Information" },
            { TagColorHalftoningInformation, "Color Halftoning Information" },
            { TagDuotoneHalftoningInformation, "Duotone Halftoning Information" },
            { TagGrayscaleAndMultichannelTransferFunction, "Grayscale and Multichannel Transfer Function" },
            { TagColorTransferFunctions, "Color Transfer Functions" },
            { TagDuotoneTransferFunctions, "Duotone Transfer Functions" },
            { TagDuotoneImageInformation, "Duotone Image Information" },
            { TagEffectiveBlackAndWhiteValues, "Effective Black and White Values" },
            { TagEpsOptions, "EPS Options" },
            { TagQuickMaskInformation, "Quick Mask Information" },
            { TagLayerStateInformation, "Layer State Information" },
            { TagLayersGroupInformation, "Layers Group Information" },
            { TagIptc, "IPTC-NAA Record" },
            { TagImageModeForRawFormatFiles, "Image Mode for Raw Format Files" },
            { TagJpegQuality, "JPEG Quality" },
            { TagGridAndGuidesInformation, "Grid and Guides Information" },
            { TagThumbnailOld, "Photoshop 4.0 Thumbnail" },
            { TagCopyright, "Copyright Flag" },
            { TagUrl, "URL" },
            { TagThumbnail, "Thumbnail Data" },
            { TagGlobalAngle, "Global Angle" },
            { TagIccProfileBytes, "ICC Profile Bytes" },
            { TagWatermark, "Watermark" },
            { TagIccUntaggedProfile, "ICC Untagged Profile" },
            { TagEffectsVisible, "Effects Visible" },
            { TagSpotHalftone, "Spot Halftone" },
            { TagSeedNumber, "Seed Number" },
            { TagUnicodeAlphaNames, "Unicode Alpha Names" },
            { TagIndexedColorTableCount, "Indexed Color Table Count" },
            { TagTransparencyIndex, "Transparency Index" },
            { TagGlobalAltitude, "Global Altitude" },
            { TagSlices, "Slices" },
            { TagWorkflowUrl, "Workflow URL" },
            { TagJumpToXpep, "Jump To XPEP" },
            { TagAlphaIdentifiers, "Alpha Identifiers" },
            { TagUrlList, "URL List" },
            { TagVersion, "Version Info" },
            { TagExifData1, "EXIF Data 1" },
            { TagExifData3, "EXIF Data 3" },
            { TagXmpData, "XMP Data" },
            { TagCaptionDigest, "Caption Digest" },
            { TagPrintScale, "Print Scale" },
            { TagPixelAspectRatio, "Pixel Aspect Ratio" },
            { TagLayerComps, "Layer Comps" },
            { TagAlternateDuotoneColors, "Alternate Duotone Colors" },
            { TagAlternateSpotColors, "Alternate Spot Colors" },
            { TagLayerSelectionIds, "Layer Selection IDs" },
            { TagHdrToningInfo, "HDR Toning Info" },
            { TagPrintInfo, "Print Info" },
            { TagLayerGroupsEnabledId, "Layer Groups Enabled ID" },
            { TagColorSamplers, "Color Samplers" },
            { TagMeasurementScale, "Measurement Scale" },
            { TagTimelineInformation, "Timeline Information" },
            { TagSheetDisclosure, "Sheet Disclosure" },
            { TagDisplayInfo, "Display Info" },
            { TagOnionSkins, "Onion Skins" },
            { TagCountInformation, "Count information" },
            { TagPrintInfo2, "Print Info 2" },
            { TagPrintStyle, "Print Style" },
            { TagMacNsprintinfo, "Mac NSPrintInfo" },
            { TagWinDevmode, "Win DEVMODE" },
            { TagAutoSaveFilePath, "Auto Save File Path" },
            { TagAutoSaveFormat, "Auto Save Format" },
            { TagPathSelectionState, "Path Selection State" },
            { TagClippingPathName, "Clipping Path Name" },
            { TagOriginPathInfo, "Origin Path Info" },
            { TagImageReadyVariablesXml, "Image Ready Variables XML" },
            { TagImageReadyDataSets, "Image Ready Data Sets" },
            { TagLightroomWorkflow, "Lightroom Workflow" },
            { TagPrintFlagsInfo, "Print Flags Information" }
        };

        public PhotoshopDirectory()
        {
            SetDescriptor(new PhotoshopDescriptor(this));
        }

        public override string Name => "Photoshop";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return TagNameMap.TryGetValue(tagType, out tagName);
        }

        [CanBeNull]
        public byte[] GetThumbnailBytes()
        {
            var storedBytes = this.GetByteArray(TagThumbnail) ?? this.GetByteArray(TagThumbnailOld);

            if (storedBytes == null || storedBytes.Length <= 28)
                return null;

            var thumbSize = storedBytes.Length - 28;
            var thumbBytes = new byte[thumbSize];
            Array.Copy(storedBytes, 28, thumbBytes, 0, thumbSize);

            return thumbBytes;
        }
    }
}
