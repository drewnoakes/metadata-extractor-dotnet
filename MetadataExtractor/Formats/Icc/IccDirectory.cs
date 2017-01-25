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

namespace MetadataExtractor.Formats.Icc
{
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class IccDirectory : Directory
    {
        // The smaller valued tags have an integer value that's equal to their offset within the ICC data buffer.
        public const int TagProfileByteCount = 0;
        public const int TagCmmType = 4;
        public const int TagProfileVersion = 8;
        public const int TagProfileClass = 12;
        public const int TagColorSpace = 16;
        public const int TagProfileConnectionSpace = 20;
        public const int TagProfileDateTime = 24;
        public const int TagSignature = 36;
        public const int TagPlatform = 40;
        public const int TagCmmFlags = 44;
        public const int TagDeviceMake = 48;
        public const int TagDeviceModel = 52;
        public const int TagDeviceAttr = 56;
        public const int TagRenderingIntent = 64;
        public const int TagXyzValues = 68;
        public const int TagProfileCreator = 80;
        public const int TagTagCount = 128;
        public const int TagTagA2B0 = 0x41324230;
        public const int TagTagA2B1 = 0x41324231;
        public const int TagTagA2B2 = 0x41324232;
        public const int TagTag_BXyz = 0x6258595A;
        public const int TagTag_BTrc = 0x62545243;
        public const int TagTagB2A0 = 0x42324130;
        public const int TagTagB2A1 = 0x42324131;
        public const int TagTagB2A2 = 0x42324132;
        public const int TagTagCalt = 0x63616C74;
        public const int TagTagTarg = 0x74617267;
        public const int TagTagChad = 0x63686164;
        public const int TagTagChrm = 0x6368726D;
        public const int TagTagCprt = 0x63707274;
        public const int TagTagCrdi = 0x63726469;
        public const int TagTagDmnd = 0x646D6E64;
        public const int TagTagDmdd = 0x646D6464;
        public const int TagTagDevs = 0x64657673;
        public const int TagTagGamt = 0x67616D74;
        public const int TagTag_KTrc = 0x6B545243;
        public const int TagTag_GXyz = 0x6758595A;
        public const int TagTag_GTrc = 0x67545243;
        public const int TagTagLumi = 0x6C756D69;
        public const int TagTagMeas = 0x6D656173;
        public const int TagTagBkpt = 0x626B7074;
        public const int TagTagWtpt = 0x77747074;
        public const int TagTagNcol = 0x6E636F6C;
        public const int TagTagNcl2 = 0x6E636C32;
        public const int TagTagResp = 0x72657370;
        public const int TagTagPre0 = 0x70726530;
        public const int TagTagPre1 = 0x70726531;
        public const int TagTagPre2 = 0x70726532;
        public const int TagTagDesc = 0x64657363;
        public const int TagTagPseq = 0x70736571;
        public const int TagTagPsd0 = 0x70736430;
        public const int TagTagPsd1 = 0x70736431;
        public const int TagTagPsd2 = 0x70736432;
        public const int TagTagPsd3 = 0x70736433;
        public const int TagTagPs2S = 0x70733273;
        public const int TagTagPs2I = 0x70733269;
        public const int TagTag_RXyz = 0x7258595A;
        public const int TagTag_RTrc = 0x72545243;
        public const int TagTagScrd = 0x73637264;
        public const int TagTagScrn = 0x7363726E;
        public const int TagTagTech = 0x74656368;
        public const int TagTagBfd = 0x62666420;
        public const int TagTagVued = 0x76756564;
        public const int TagTagView = 0x76696577;
        public const int TagAppleMultiLanguageProfileName = 0x6473636d;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagProfileByteCount, "Profile Size" },
            { TagCmmType, "CMM Type" },
            { TagProfileVersion, "Version" },
            { TagProfileClass, "Class" },
            { TagColorSpace, "Color space" },
            { TagProfileConnectionSpace, "Profile Connection Space" },
            { TagProfileDateTime, "Profile Date/Time" },
            { TagSignature, "Signature" },
            { TagPlatform, "Primary Platform" },
            { TagCmmFlags, "CMM Flags" },
            { TagDeviceMake, "Device manufacturer" },
            { TagDeviceModel, "Device model" },
            { TagDeviceAttr, "Device attributes" },
            { TagRenderingIntent, "Rendering Intent" },
            { TagXyzValues, "XYZ values" },
            { TagProfileCreator, "Profile Creator" },
            { TagTagCount, "Tag Count" },
            { TagTagA2B0, "AToB 0" },
            { TagTagA2B1, "AToB 1" },
            { TagTagA2B2, "AToB 2" },
            { TagTag_BXyz, "Blue Colorant" },
            { TagTag_BTrc, "Blue TRC" },
            { TagTagB2A0, "BToA 0" },
            { TagTagB2A1, "BToA 1" },
            { TagTagB2A2, "BToA 2" },
            { TagTagCalt, "Calibration Date/Time" },
            { TagTagTarg, "Char Target" },
            { TagTagChad, "Chromatic Adaptation" },
            { TagTagChrm, "Chromaticity" },
            { TagTagCprt, "Copyright" },
            { TagTagCrdi, "CrdInfo" },
            { TagTagDmnd, "Device Mfg Description" },
            { TagTagDmdd, "Device Model Description" },
            { TagTagDevs, "Device Settings" },
            { TagTagGamt, "Gamut" },
            { TagTag_KTrc, "Gray TRC" },
            { TagTag_GXyz, "Green Colorant" },
            { TagTag_GTrc, "Green TRC" },
            { TagTagLumi, "Luminance" },
            { TagTagMeas, "Measurement" },
            { TagTagBkpt, "Media Black Point" },
            { TagTagWtpt, "Media White Point" },
            { TagTagNcol, "Named Color" },
            { TagTagNcl2, "Named Color 2" },
            { TagTagResp, "Output Response" },
            { TagTagPre0, "Preview 0" },
            { TagTagPre1, "Preview 1" },
            { TagTagPre2, "Preview 2" },
            { TagTagDesc, "Profile Description" },
            { TagTagPseq, "Profile Sequence Description" },
            { TagTagPsd0, "Ps2 CRD 0" },
            { TagTagPsd1, "Ps2 CRD 1" },
            { TagTagPsd2, "Ps2 CRD 2" },
            { TagTagPsd3, "Ps2 CRD 3" },
            { TagTagPs2S, "Ps2 CSA" },
            { TagTagPs2I, "Ps2 Rendering Intent" },
            { TagTag_RXyz, "Red Colorant" },
            { TagTag_RTrc, "Red TRC" },
            { TagTagScrd, "Screening Desc" },
            { TagTagScrn, "Screening" },
            { TagTagTech, "Technology" },
            { TagTagBfd, "Ucrbg" },
            { TagTagVued, "Viewing Conditions Description" },
            { TagTagView, "Viewing Conditions" },
            { TagAppleMultiLanguageProfileName, "Apple Multi-language Profile Name" }
        };

        public IccDirectory()
        {
            SetDescriptor(new IccDescriptor(this));
        }

        public override string Name => "ICC Profile";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
