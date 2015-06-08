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

namespace Com.Drew.Metadata.Icc
{
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class IccDirectory : Directory
    {
        public const int TagProfileByteCount = 0;

        public const int TagCmmType = 4;

        public const int TagProfileVersion = 8;

        public const int TagProfileClass = 12;

        public const int TagColorSpace = 16;

        public const int TagProfileConnectionSpace = 20;

        public const int TagProfileDatetime = 24;

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

        public const int TagTagA2B0 = unchecked(0x41324230);

        public const int TagTagA2B1 = unchecked(0x41324231);

        public const int TagTagA2B2 = unchecked(0x41324232);

        public const int TagTag_BXyz = unchecked(0x6258595A);

        public const int TagTag_BTrc = unchecked(0x62545243);

        public const int TagTagB2A0 = unchecked(0x42324130);

        public const int TagTagB2A1 = unchecked(0x42324131);

        public const int TagTagB2A2 = unchecked(0x42324132);

        public const int TagTagCalt = unchecked(0x63616C74);

        public const int TagTagTarg = unchecked(0x74617267);

        public const int TagTagChad = unchecked(0x63686164);

        public const int TagTagChrm = unchecked(0x6368726D);

        public const int TagTagCprt = unchecked(0x63707274);

        public const int TagTagCrdi = unchecked(0x63726469);

        public const int TagTagDmnd = unchecked(0x646D6E64);

        public const int TagTagDmdd = unchecked(0x646D6464);

        public const int TagTagDevs = unchecked(0x64657673);

        public const int TagTagGamt = unchecked(0x67616D74);

        public const int TagTag_KTrc = unchecked(0x6B545243);

        public const int TagTag_GXyz = unchecked(0x6758595A);

        public const int TagTag_GTrc = unchecked(0x67545243);

        public const int TagTagLumi = unchecked(0x6C756D69);

        public const int TagTagMeas = unchecked(0x6D656173);

        public const int TagTagBkpt = unchecked(0x626B7074);

        public const int TagTagWtpt = unchecked(0x77747074);

        public const int TagTagNcol = unchecked(0x6E636F6C);

        public const int TagTagNcl2 = unchecked(0x6E636C32);

        public const int TagTagResp = unchecked(0x72657370);

        public const int TagTagPre0 = unchecked(0x70726530);

        public const int TagTagPre1 = unchecked(0x70726531);

        public const int TagTagPre2 = unchecked(0x70726532);

        public const int TagTagDesc = unchecked(0x64657363);

        public const int TagTagPseq = unchecked(0x70736571);

        public const int TagTagPsd0 = unchecked(0x70736430);

        public const int TagTagPsd1 = unchecked(0x70736431);

        public const int TagTagPsd2 = unchecked(0x70736432);

        public const int TagTagPsd3 = unchecked(0x70736433);

        public const int TagTagPs2S = unchecked(0x70733273);

        public const int TagTagPs2I = unchecked(0x70733269);

        public const int TagTag_RXyz = unchecked(0x7258595A);

        public const int TagTag_RTrc = unchecked(0x72545243);

        public const int TagTagScrd = unchecked(0x73637264);

        public const int TagTagScrn = unchecked(0x7363726E);

        public const int TagTagTech = unchecked(0x74656368);

        public const int TagTagBfd = unchecked(0x62666420);

        public const int TagTagVued = unchecked(0x76756564);

        public const int TagTagView = unchecked(0x76696577);

        public const int TagAppleMultiLanguageProfileName = unchecked(0x6473636d);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static IccDirectory()
        {
            // These (smaller valued) tags have an integer value that's equal to their offset within the ICC data buffer.
            // These tag values
            TagNameMap.Put(TagProfileByteCount, "Profile Size");
            TagNameMap.Put(TagCmmType, "CMM Type");
            TagNameMap.Put(TagProfileVersion, "Version");
            TagNameMap.Put(TagProfileClass, "Class");
            TagNameMap.Put(TagColorSpace, "Color space");
            TagNameMap.Put(TagProfileConnectionSpace, "Profile Connection Space");
            TagNameMap.Put(TagProfileDatetime, "Profile Date/Time");
            TagNameMap.Put(TagSignature, "Signature");
            TagNameMap.Put(TagPlatform, "Primary Platform");
            TagNameMap.Put(TagCmmFlags, "CMM Flags");
            TagNameMap.Put(TagDeviceMake, "Device manufacturer");
            TagNameMap.Put(TagDeviceModel, "Device model");
            TagNameMap.Put(TagDeviceAttr, "Device attributes");
            TagNameMap.Put(TagRenderingIntent, "Rendering Intent");
            TagNameMap.Put(TagXyzValues, "XYZ values");
            TagNameMap.Put(TagProfileCreator, "Profile Creator");
            TagNameMap.Put(TagTagCount, "Tag Count");
            TagNameMap.Put(TagTagA2B0, "AToB 0");
            TagNameMap.Put(TagTagA2B1, "AToB 1");
            TagNameMap.Put(TagTagA2B2, "AToB 2");
            TagNameMap.Put(TagTag_BXyz, "Blue Colorant");
            TagNameMap.Put(TagTag_BTrc, "Blue TRC");
            TagNameMap.Put(TagTagB2A0, "BToA 0");
            TagNameMap.Put(TagTagB2A1, "BToA 1");
            TagNameMap.Put(TagTagB2A2, "BToA 2");
            TagNameMap.Put(TagTagCalt, "Calibration Date/Time");
            TagNameMap.Put(TagTagTarg, "Char Target");
            TagNameMap.Put(TagTagChad, "Chromatic Adaptation");
            TagNameMap.Put(TagTagChrm, "Chromaticity");
            TagNameMap.Put(TagTagCprt, "Copyright");
            TagNameMap.Put(TagTagCrdi, "CrdInfo");
            TagNameMap.Put(TagTagDmnd, "Device Mfg Description");
            TagNameMap.Put(TagTagDmdd, "Device Model Description");
            TagNameMap.Put(TagTagDevs, "Device Settings");
            TagNameMap.Put(TagTagGamt, "Gamut");
            TagNameMap.Put(TagTag_KTrc, "Gray TRC");
            TagNameMap.Put(TagTag_GXyz, "Green Colorant");
            TagNameMap.Put(TagTag_GTrc, "Green TRC");
            TagNameMap.Put(TagTagLumi, "Luminance");
            TagNameMap.Put(TagTagMeas, "Measurement");
            TagNameMap.Put(TagTagBkpt, "Media Black Point");
            TagNameMap.Put(TagTagWtpt, "Media White Point");
            TagNameMap.Put(TagTagNcol, "Named Color");
            TagNameMap.Put(TagTagNcl2, "Named Color 2");
            TagNameMap.Put(TagTagResp, "Output Response");
            TagNameMap.Put(TagTagPre0, "Preview 0");
            TagNameMap.Put(TagTagPre1, "Preview 1");
            TagNameMap.Put(TagTagPre2, "Preview 2");
            TagNameMap.Put(TagTagDesc, "Profile Description");
            TagNameMap.Put(TagTagPseq, "Profile Sequence Description");
            TagNameMap.Put(TagTagPsd0, "Ps2 CRD 0");
            TagNameMap.Put(TagTagPsd1, "Ps2 CRD 1");
            TagNameMap.Put(TagTagPsd2, "Ps2 CRD 2");
            TagNameMap.Put(TagTagPsd3, "Ps2 CRD 3");
            TagNameMap.Put(TagTagPs2S, "Ps2 CSA");
            TagNameMap.Put(TagTagPs2I, "Ps2 Rendering Intent");
            TagNameMap.Put(TagTag_RXyz, "Red Colorant");
            TagNameMap.Put(TagTag_RTrc, "Red TRC");
            TagNameMap.Put(TagTagScrd, "Screening Desc");
            TagNameMap.Put(TagTagScrn, "Screening");
            TagNameMap.Put(TagTagTech, "Technology");
            TagNameMap.Put(TagTagBfd, "Ucrbg");
            TagNameMap.Put(TagTagVued, "Viewing Conditions Description");
            TagNameMap.Put(TagTagView, "Viewing Conditions");
            TagNameMap.Put(TagAppleMultiLanguageProfileName, "Apple Multi-language Profile Name");
        }

        public IccDirectory()
        {
            SetDescriptor(new IccDescriptor(this));
        }

        public override string GetName()
        {
            return "ICC Profile";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
