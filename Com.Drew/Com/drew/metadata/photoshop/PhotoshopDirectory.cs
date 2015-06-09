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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
    /// <summary>Holds the metadata found in the APPD segment of a JPEG file saved by Photoshop.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Yuri Binev</author>
    public sealed class PhotoshopDirectory : Directory
    {
        public const int TagChannelsRowsColumnsDepthMode = unchecked(0x03E8);

        public const int TagMacPrintInfo = unchecked(0x03E9);

        public const int TagXml = unchecked(0x03EA);

        public const int TagIndexedColorTable = unchecked(0x03EB);

        public const int TagResolutionInfo = unchecked(0x03ED);

        public const int TagAlphaChannels = unchecked(0x03EE);

        public const int TagDisplayInfoObsolete = unchecked(0x03EF);

        public const int TagCaption = unchecked(0x03F0);

        public const int TagBorderInformation = unchecked(0x03F1);

        public const int TagBackgroundColor = unchecked(0x03F2);

        public const int TagPrintFlags = unchecked(0x03F3);

        public const int TagGrayscaleAndMultichannelHalftoningInformation = unchecked(0x03F4);

        public const int TagColorHalftoningInformation = unchecked(0x03F5);

        public const int TagDuotoneHalftoningInformation = unchecked(0x03F6);

        public const int TagGrayscaleAndMultichannelTransferFunction = unchecked(0x03F7);

        public const int TagColorTransferFunctions = unchecked(0x03F8);

        public const int TagDuotoneTransferFunctions = unchecked(0x03F9);

        public const int TagDuotoneImageInformation = unchecked(0x03FA);

        public const int TagEffectiveBlackAndWhiteValues = unchecked(0x03FB);

        public const int TagEpsOptions = unchecked(0x03FD);

        public const int TagQuickMaskInformation = unchecked(0x03FE);

        public const int TagLayerStateInformation = unchecked(0x0400);

        public const int TagLayersGroupInformation = unchecked(0x0402);

        public const int TagIptc = unchecked(0x0404);

        public const int TagImageModeForRawFormatFiles = unchecked(0x0405);

        public const int TagJpegQuality = unchecked(0x0406);

        public const int TagGridAndGuidesInformation = unchecked(0x0408);

        public const int TagThumbnailOld = unchecked(0x0409);

        public const int TagCopyright = unchecked(0x040A);

        public const int TagUrl = unchecked(0x040B);

        public const int TagThumbnail = unchecked(0x040C);

        public const int TagGlobalAngle = unchecked(0x040D);

        public const int TagIccProfileBytes = unchecked(0x040F);

        public const int TagWatermark = unchecked(0x0410);

        public const int TagIccUntaggedProfile = unchecked(0x0411);

        public const int TagEffectsVisible = unchecked(0x0412);

        public const int TagSpotHalftone = unchecked(0x0413);

        public const int TagSeedNumber = unchecked(0x0414);

        public const int TagUnicodeAlphaNames = unchecked(0x0415);

        public const int TagIndexedColorTableCount = unchecked(0x0416);

        public const int TagTransparencyIndex = unchecked(0x0417);

        public const int TagGlobalAltitude = unchecked(0x0419);

        public const int TagSlices = unchecked(0x041A);

        public const int TagWorkflowUrl = unchecked(0x041B);

        public const int TagJumpToXpep = unchecked(0x041C);

        public const int TagAlphaIdentifiers = unchecked(0x041D);

        public const int TagUrlList = unchecked(0x041E);

        public const int TagVersion = unchecked(0x0421);

        public const int TagExifData1 = unchecked(0x0422);

        public const int TagExifData3 = unchecked(0x0423);

        public const int TagXmpData = unchecked(0x0424);

        public const int TagCaptionDigest = unchecked(0x0425);

        public const int TagPrintScale = unchecked(0x0426);

        public const int TagPixelAspectRatio = unchecked(0x0428);

        public const int TagLayerComps = unchecked(0x0429);

        public const int TagAlternateDuotoneColors = unchecked(0x042A);

        public const int TagAlternateSpotColors = unchecked(0x042B);

        public const int TagLayerSelectionIds = unchecked(0x042D);

        public const int TagHdrToningInfo = unchecked(0x042E);

        public const int TagPrintInfo = unchecked(0x042F);

        public const int TagLayerGroupsEnabledId = unchecked(0x0430);

        public const int TagColorSamplers = unchecked(0x0431);

        public const int TagMeasurementScale = unchecked(0x0432);

        public const int TagTimelineInformation = unchecked(0x0433);

        public const int TagSheetDisclosure = unchecked(0x0434);

        public const int TagDisplayInfo = unchecked(0x0435);

        public const int TagOnionSkins = unchecked(0x0436);

        public const int TagCountInformation = unchecked(0x0438);

        public const int TagPrintInfo2 = unchecked(0x043A);

        public const int TagPrintStyle = unchecked(0x043B);

        public const int TagMacNsprintinfo = unchecked(0x043C);

        public const int TagWinDevmode = unchecked(0x043D);

        public const int TagAutoSaveFilePath = unchecked(0x043E);

        public const int TagAutoSaveFormat = unchecked(0x043F);

        public const int TagPathSelectionState = unchecked(0x0440);

        public const int TagClippingPathName = unchecked(0x0BB7);

        public const int TagOriginPathInfo = unchecked(0x0BB8);

        public const int TagImageReadyVariablesXml = unchecked(0x1B58);

        public const int TagImageReadyDataSets = unchecked(0x1B59);

        public const int TagLightroomWorkflow = unchecked(0x1F40);

        public const int TagPrintFlagsInfo = unchecked(0x2710);

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
            TagNameMap.Put(TagChannelsRowsColumnsDepthMode, "Channels, Rows, Columns, Depth, Mode");
            TagNameMap.Put(TagMacPrintInfo, "Mac Print Info");
            TagNameMap.Put(TagXml, "XML Data");
            TagNameMap.Put(TagIndexedColorTable, "Indexed Color Table");
            TagNameMap.Put(TagResolutionInfo, "Resolution Info");
            TagNameMap.Put(TagAlphaChannels, "Alpha Channels");
            TagNameMap.Put(TagDisplayInfoObsolete, "Display Info (Obsolete)");
            TagNameMap.Put(TagCaption, "Caption");
            TagNameMap.Put(TagBorderInformation, "Border Information");
            TagNameMap.Put(TagBackgroundColor, "Background Color");
            TagNameMap.Put(TagPrintFlags, "Print Flags");
            TagNameMap.Put(TagGrayscaleAndMultichannelHalftoningInformation, "Grayscale and Multichannel Halftoning Information");
            TagNameMap.Put(TagColorHalftoningInformation, "Color Halftoning Information");
            TagNameMap.Put(TagDuotoneHalftoningInformation, "Duotone Halftoning Information");
            TagNameMap.Put(TagGrayscaleAndMultichannelTransferFunction, "Grayscale and Multichannel Transfer Function");
            TagNameMap.Put(TagColorTransferFunctions, "Color Transfer Functions");
            TagNameMap.Put(TagDuotoneTransferFunctions, "Duotone Transfer Functions");
            TagNameMap.Put(TagDuotoneImageInformation, "Duotone Image Information");
            TagNameMap.Put(TagEffectiveBlackAndWhiteValues, "Effective Black and White Values");
            TagNameMap.Put(TagEpsOptions, "EPS Options");
            TagNameMap.Put(TagQuickMaskInformation, "Quick Mask Information");
            TagNameMap.Put(TagLayerStateInformation, "Layer State Information");
            TagNameMap.Put(TagLayersGroupInformation, "Layers Group Information");
            TagNameMap.Put(TagIptc, "IPTC-NAA Record");
            TagNameMap.Put(TagImageModeForRawFormatFiles, "Image Mode for Raw Format Files");
            TagNameMap.Put(TagJpegQuality, "JPEG Quality");
            TagNameMap.Put(TagGridAndGuidesInformation, "Grid and Guides Information");
            TagNameMap.Put(TagThumbnailOld, "Photoshop 4.0 Thumbnail");
            TagNameMap.Put(TagCopyright, "Copyright Flag");
            TagNameMap.Put(TagUrl, "URL");
            TagNameMap.Put(TagThumbnail, "Thumbnail Data");
            TagNameMap.Put(TagGlobalAngle, "Global Angle");
            TagNameMap.Put(TagIccProfileBytes, "ICC Profile Bytes");
            TagNameMap.Put(TagWatermark, "Watermark");
            TagNameMap.Put(TagIccUntaggedProfile, "ICC Untagged Profile");
            TagNameMap.Put(TagEffectsVisible, "Effects Visible");
            TagNameMap.Put(TagSpotHalftone, "Spot Halftone");
            TagNameMap.Put(TagSeedNumber, "Seed Number");
            TagNameMap.Put(TagUnicodeAlphaNames, "Unicode Alpha Names");
            TagNameMap.Put(TagIndexedColorTableCount, "Indexed Color Table Count");
            TagNameMap.Put(TagTransparencyIndex, "Transparency Index");
            TagNameMap.Put(TagGlobalAltitude, "Global Altitude");
            TagNameMap.Put(TagSlices, "Slices");
            TagNameMap.Put(TagWorkflowUrl, "Workflow URL");
            TagNameMap.Put(TagJumpToXpep, "Jump To XPEP");
            TagNameMap.Put(TagAlphaIdentifiers, "Alpha Identifiers");
            TagNameMap.Put(TagUrlList, "URL List");
            TagNameMap.Put(TagVersion, "Version Info");
            TagNameMap.Put(TagExifData1, "EXIF Data 1");
            TagNameMap.Put(TagExifData3, "EXIF Data 3");
            TagNameMap.Put(TagXmpData, "XMP Data");
            TagNameMap.Put(TagCaptionDigest, "Caption Digest");
            TagNameMap.Put(TagPrintScale, "Print Scale");
            TagNameMap.Put(TagPixelAspectRatio, "Pixel Aspect Ratio");
            TagNameMap.Put(TagLayerComps, "Layer Comps");
            TagNameMap.Put(TagAlternateDuotoneColors, "Alternate Duotone Colors");
            TagNameMap.Put(TagAlternateSpotColors, "Alternate Spot Colors");
            TagNameMap.Put(TagLayerSelectionIds, "Layer Selection IDs");
            TagNameMap.Put(TagHdrToningInfo, "HDR Toning Info");
            TagNameMap.Put(TagPrintInfo, "Print Info");
            TagNameMap.Put(TagLayerGroupsEnabledId, "Layer Groups Enabled ID");
            TagNameMap.Put(TagColorSamplers, "Color Samplers");
            TagNameMap.Put(TagMeasurementScale, "Measurement Scale");
            TagNameMap.Put(TagTimelineInformation, "Timeline Information");
            TagNameMap.Put(TagSheetDisclosure, "Sheet Disclosure");
            TagNameMap.Put(TagDisplayInfo, "Display Info");
            TagNameMap.Put(TagOnionSkins, "Onion Skins");
            TagNameMap.Put(TagCountInformation, "Count information");
            TagNameMap.Put(TagPrintInfo2, "Print Info 2");
            TagNameMap.Put(TagPrintStyle, "Print Style");
            TagNameMap.Put(TagMacNsprintinfo, "Mac NSPrintInfo");
            TagNameMap.Put(TagWinDevmode, "Win DEVMODE");
            TagNameMap.Put(TagAutoSaveFilePath, "Auto Save File Path");
            TagNameMap.Put(TagAutoSaveFormat, "Auto Save Format");
            TagNameMap.Put(TagPathSelectionState, "Path Selection State");
            TagNameMap.Put(TagClippingPathName, "Clipping Path Name");
            TagNameMap.Put(TagOriginPathInfo, "Origin Path Info");
            TagNameMap.Put(TagImageReadyVariablesXml, "Image Ready Variables XML");
            TagNameMap.Put(TagImageReadyDataSets, "Image Ready Data Sets");
            TagNameMap.Put(TagLightroomWorkflow, "Lightroom Workflow");
            TagNameMap.Put(TagPrintFlagsInfo, "Print Flags Information");
        }

        public PhotoshopDirectory()
        {
            SetDescriptor(new PhotoshopDescriptor(this));
        }

        public override string GetName()
        {
            return "Photoshop";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        [CanBeNull]
        public byte[] GetThumbnailBytes()
        {
            byte[] storedBytes = GetByteArray(TagThumbnail);
            if (storedBytes == null)
            {
                storedBytes = GetByteArray(TagThumbnailOld);
            }
            if (storedBytes == null)
            {
                return null;
            }
            int thumbSize = storedBytes.Length - 28;
            byte[] thumbBytes = new byte[thumbSize];
            Array.Copy(storedBytes, 28, thumbBytes, 0, thumbSize);
            return thumbBytes;
        }
    }
}
