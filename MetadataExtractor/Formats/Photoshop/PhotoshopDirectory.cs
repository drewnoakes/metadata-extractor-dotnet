#region License
//
// Copyright 2002-2015 Drew Noakes
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
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Holds the metadata found in the APPD segment of a JPEG file saved by Photoshop.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Yuri Binev</author>
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

        public const int TagEpsOptions = 0x03FD;

        public const int TagQuickMaskInformation = 0x03FE;

        public const int TagLayerStateInformation = 0x0400;

        public const int TagLayersGroupInformation = 0x0402;

        public const int TagIptc = 0x0404;

        public const int TagImageModeForRawFormatFiles = 0x0405;

        public const int TagJpegQuality = 0x0406;

        public const int TagGridAndGuidesInformation = 0x0408;

        public const int TagThumbnailOld = 0x0409;

        public const int TagCopyright = 0x040A;

        public const int TagUrl = 0x040B;

        public const int TagThumbnail = 0x040C;

        public const int TagGlobalAngle = 0x040D;

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

        public const int TagClippingPathName = 0x0BB7;

        public const int TagOriginPathInfo = 0x0BB8;

        public const int TagImageReadyVariablesXml = 0x1B58;

        public const int TagImageReadyDataSets = 0x1B59;

        public const int TagLightroomWorkflow = 0x1F40;

        public const int TagPrintFlagsInfo = 0x2710;

        [NotNull]
        internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PhotoshopDirectory()
        {
            // OBSOLETE                                                                     0x03FC
            // OBSOLETE                                                                     0x03FF
            // Working path (not saved)                                                     0x0401
            // OBSOLETE                                                                     0x0403
            // OBSOLETE                                                                     0x040E
            // CLIPPING PATHS                                                               0x07D0 -> 0x0BB6
            // PLUG IN RESOURCES                                                            0x0FA0 -> 0x1387
            TagNameMap[TagChannelsRowsColumnsDepthMode] = "Channels, Rows, Columns, Depth, Mode";
            TagNameMap[TagMacPrintInfo] = "Mac Print Info";
            TagNameMap[TagXml] = "XML Data";
            TagNameMap[TagIndexedColorTable] = "Indexed Color Table";
            TagNameMap[TagResolutionInfo] = "Resolution Info";
            TagNameMap[TagAlphaChannels] = "Alpha Channels";
            TagNameMap[TagDisplayInfoObsolete] = "Display Info (Obsolete)";
            TagNameMap[TagCaption] = "Caption";
            TagNameMap[TagBorderInformation] = "Border Information";
            TagNameMap[TagBackgroundColor] = "Background Color";
            TagNameMap[TagPrintFlags] = "Print Flags";
            TagNameMap[TagGrayscaleAndMultichannelHalftoningInformation] = "Grayscale and Multichannel Halftoning Information";
            TagNameMap[TagColorHalftoningInformation] = "Color Halftoning Information";
            TagNameMap[TagDuotoneHalftoningInformation] = "Duotone Halftoning Information";
            TagNameMap[TagGrayscaleAndMultichannelTransferFunction] = "Grayscale and Multichannel Transfer Function";
            TagNameMap[TagColorTransferFunctions] = "Color Transfer Functions";
            TagNameMap[TagDuotoneTransferFunctions] = "Duotone Transfer Functions";
            TagNameMap[TagDuotoneImageInformation] = "Duotone Image Information";
            TagNameMap[TagEffectiveBlackAndWhiteValues] = "Effective Black and White Values";
            TagNameMap[TagEpsOptions] = "EPS Options";
            TagNameMap[TagQuickMaskInformation] = "Quick Mask Information";
            TagNameMap[TagLayerStateInformation] = "Layer State Information";
            TagNameMap[TagLayersGroupInformation] = "Layers Group Information";
            TagNameMap[TagIptc] = "IPTC-NAA Record";
            TagNameMap[TagImageModeForRawFormatFiles] = "Image Mode for Raw Format Files";
            TagNameMap[TagJpegQuality] = "JPEG Quality";
            TagNameMap[TagGridAndGuidesInformation] = "Grid and Guides Information";
            TagNameMap[TagThumbnailOld] = "Photoshop 4.0 Thumbnail";
            TagNameMap[TagCopyright] = "Copyright Flag";
            TagNameMap[TagUrl] = "URL";
            TagNameMap[TagThumbnail] = "Thumbnail Data";
            TagNameMap[TagGlobalAngle] = "Global Angle";
            TagNameMap[TagIccProfileBytes] = "ICC Profile Bytes";
            TagNameMap[TagWatermark] = "Watermark";
            TagNameMap[TagIccUntaggedProfile] = "ICC Untagged Profile";
            TagNameMap[TagEffectsVisible] = "Effects Visible";
            TagNameMap[TagSpotHalftone] = "Spot Halftone";
            TagNameMap[TagSeedNumber] = "Seed Number";
            TagNameMap[TagUnicodeAlphaNames] = "Unicode Alpha Names";
            TagNameMap[TagIndexedColorTableCount] = "Indexed Color Table Count";
            TagNameMap[TagTransparencyIndex] = "Transparency Index";
            TagNameMap[TagGlobalAltitude] = "Global Altitude";
            TagNameMap[TagSlices] = "Slices";
            TagNameMap[TagWorkflowUrl] = "Workflow URL";
            TagNameMap[TagJumpToXpep] = "Jump To XPEP";
            TagNameMap[TagAlphaIdentifiers] = "Alpha Identifiers";
            TagNameMap[TagUrlList] = "URL List";
            TagNameMap[TagVersion] = "Version Info";
            TagNameMap[TagExifData1] = "EXIF Data 1";
            TagNameMap[TagExifData3] = "EXIF Data 3";
            TagNameMap[TagXmpData] = "XMP Data";
            TagNameMap[TagCaptionDigest] = "Caption Digest";
            TagNameMap[TagPrintScale] = "Print Scale";
            TagNameMap[TagPixelAspectRatio] = "Pixel Aspect Ratio";
            TagNameMap[TagLayerComps] = "Layer Comps";
            TagNameMap[TagAlternateDuotoneColors] = "Alternate Duotone Colors";
            TagNameMap[TagAlternateSpotColors] = "Alternate Spot Colors";
            TagNameMap[TagLayerSelectionIds] = "Layer Selection IDs";
            TagNameMap[TagHdrToningInfo] = "HDR Toning Info";
            TagNameMap[TagPrintInfo] = "Print Info";
            TagNameMap[TagLayerGroupsEnabledId] = "Layer Groups Enabled ID";
            TagNameMap[TagColorSamplers] = "Color Samplers";
            TagNameMap[TagMeasurementScale] = "Measurement Scale";
            TagNameMap[TagTimelineInformation] = "Timeline Information";
            TagNameMap[TagSheetDisclosure] = "Sheet Disclosure";
            TagNameMap[TagDisplayInfo] = "Display Info";
            TagNameMap[TagOnionSkins] = "Onion Skins";
            TagNameMap[TagCountInformation] = "Count information";
            TagNameMap[TagPrintInfo2] = "Print Info 2";
            TagNameMap[TagPrintStyle] = "Print Style";
            TagNameMap[TagMacNsprintinfo] = "Mac NSPrintInfo";
            TagNameMap[TagWinDevmode] = "Win DEVMODE";
            TagNameMap[TagAutoSaveFilePath] = "Auto Save File Path";
            TagNameMap[TagAutoSaveFormat] = "Auto Save Format";
            TagNameMap[TagPathSelectionState] = "Path Selection State";
            TagNameMap[TagClippingPathName] = "Clipping Path Name";
            TagNameMap[TagOriginPathInfo] = "Origin Path Info";
            TagNameMap[TagImageReadyVariablesXml] = "Image Ready Variables XML";
            TagNameMap[TagImageReadyDataSets] = "Image Ready Data Sets";
            TagNameMap[TagLightroomWorkflow] = "Lightroom Workflow";
            TagNameMap[TagPrintFlagsInfo] = "Print Flags Information";
        }

        public PhotoshopDirectory()
        {
            SetDescriptor(new PhotoshopDescriptor(this));
        }

        public override string Name
        {
            get { return "Photoshop"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
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
