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
    public class IccDirectory : Com.Drew.Metadata.Directory
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

        public const int TagTagA2b0 = unchecked((int)(0x41324230));

        public const int TagTagA2b1 = unchecked((int)(0x41324231));

        public const int TagTagA2b2 = unchecked((int)(0x41324232));

        public const int TAG_TAG_bXYZ = unchecked((int)(0x6258595A));

        public const int TAG_TAG_bTRC = unchecked((int)(0x62545243));

        public const int TagTagB2a0 = unchecked((int)(0x42324130));

        public const int TagTagB2a1 = unchecked((int)(0x42324131));

        public const int TagTagB2a2 = unchecked((int)(0x42324132));

        public const int TAG_TAG_calt = unchecked((int)(0x63616C74));

        public const int TAG_TAG_targ = unchecked((int)(0x74617267));

        public const int TAG_TAG_chad = unchecked((int)(0x63686164));

        public const int TAG_TAG_chrm = unchecked((int)(0x6368726D));

        public const int TAG_TAG_cprt = unchecked((int)(0x63707274));

        public const int TAG_TAG_crdi = unchecked((int)(0x63726469));

        public const int TAG_TAG_dmnd = unchecked((int)(0x646D6E64));

        public const int TAG_TAG_dmdd = unchecked((int)(0x646D6464));

        public const int TAG_TAG_devs = unchecked((int)(0x64657673));

        public const int TAG_TAG_gamt = unchecked((int)(0x67616D74));

        public const int TAG_TAG_kTRC = unchecked((int)(0x6B545243));

        public const int TAG_TAG_gXYZ = unchecked((int)(0x6758595A));

        public const int TAG_TAG_gTRC = unchecked((int)(0x67545243));

        public const int TAG_TAG_lumi = unchecked((int)(0x6C756D69));

        public const int TAG_TAG_meas = unchecked((int)(0x6D656173));

        public const int TAG_TAG_bkpt = unchecked((int)(0x626B7074));

        public const int TAG_TAG_wtpt = unchecked((int)(0x77747074));

        public const int TAG_TAG_ncol = unchecked((int)(0x6E636F6C));

        public const int TAG_TAG_ncl2 = unchecked((int)(0x6E636C32));

        public const int TAG_TAG_resp = unchecked((int)(0x72657370));

        public const int TAG_TAG_pre0 = unchecked((int)(0x70726530));

        public const int TAG_TAG_pre1 = unchecked((int)(0x70726531));

        public const int TAG_TAG_pre2 = unchecked((int)(0x70726532));

        public const int TAG_TAG_desc = unchecked((int)(0x64657363));

        public const int TAG_TAG_pseq = unchecked((int)(0x70736571));

        public const int TAG_TAG_psd0 = unchecked((int)(0x70736430));

        public const int TAG_TAG_psd1 = unchecked((int)(0x70736431));

        public const int TAG_TAG_psd2 = unchecked((int)(0x70736432));

        public const int TAG_TAG_psd3 = unchecked((int)(0x70736433));

        public const int TAG_TAG_ps2s = unchecked((int)(0x70733273));

        public const int TAG_TAG_ps2i = unchecked((int)(0x70733269));

        public const int TAG_TAG_rXYZ = unchecked((int)(0x7258595A));

        public const int TAG_TAG_rTRC = unchecked((int)(0x72545243));

        public const int TAG_TAG_scrd = unchecked((int)(0x73637264));

        public const int TAG_TAG_scrn = unchecked((int)(0x7363726E));

        public const int TAG_TAG_tech = unchecked((int)(0x74656368));

        public const int TAG_TAG_bfd = unchecked((int)(0x62666420));

        public const int TAG_TAG_vued = unchecked((int)(0x76756564));

        public const int TAG_TAG_view = unchecked((int)(0x76696577));

        public const int TagAppleMultiLanguageProfileName = unchecked((int)(0x6473636d));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static IccDirectory()
        {
            // These (smaller valued) tags have an integer value that's equal to their offset within the ICC data buffer.
            // These tag values
            _tagNameMap.Put(TagProfileByteCount, "Profile Size");
            _tagNameMap.Put(TagCmmType, "CMM Type");
            _tagNameMap.Put(TagProfileVersion, "Version");
            _tagNameMap.Put(TagProfileClass, "Class");
            _tagNameMap.Put(TagColorSpace, "Color space");
            _tagNameMap.Put(TagProfileConnectionSpace, "Profile Connection Space");
            _tagNameMap.Put(TagProfileDatetime, "Profile Date/Time");
            _tagNameMap.Put(TagSignature, "Signature");
            _tagNameMap.Put(TagPlatform, "Primary Platform");
            _tagNameMap.Put(TagCmmFlags, "CMM Flags");
            _tagNameMap.Put(TagDeviceMake, "Device manufacturer");
            _tagNameMap.Put(TagDeviceModel, "Device model");
            _tagNameMap.Put(TagDeviceAttr, "Device attributes");
            _tagNameMap.Put(TagRenderingIntent, "Rendering Intent");
            _tagNameMap.Put(TagXyzValues, "XYZ values");
            _tagNameMap.Put(TagProfileCreator, "Profile Creator");
            _tagNameMap.Put(TagTagCount, "Tag Count");
            _tagNameMap.Put(TagTagA2b0, "AToB 0");
            _tagNameMap.Put(TagTagA2b1, "AToB 1");
            _tagNameMap.Put(TagTagA2b2, "AToB 2");
            _tagNameMap.Put(TAG_TAG_bXYZ, "Blue Colorant");
            _tagNameMap.Put(TAG_TAG_bTRC, "Blue TRC");
            _tagNameMap.Put(TagTagB2a0, "BToA 0");
            _tagNameMap.Put(TagTagB2a1, "BToA 1");
            _tagNameMap.Put(TagTagB2a2, "BToA 2");
            _tagNameMap.Put(TAG_TAG_calt, "Calibration Date/Time");
            _tagNameMap.Put(TAG_TAG_targ, "Char Target");
            _tagNameMap.Put(TAG_TAG_chad, "Chromatic Adaptation");
            _tagNameMap.Put(TAG_TAG_chrm, "Chromaticity");
            _tagNameMap.Put(TAG_TAG_cprt, "Copyright");
            _tagNameMap.Put(TAG_TAG_crdi, "CrdInfo");
            _tagNameMap.Put(TAG_TAG_dmnd, "Device Mfg Description");
            _tagNameMap.Put(TAG_TAG_dmdd, "Device Model Description");
            _tagNameMap.Put(TAG_TAG_devs, "Device Settings");
            _tagNameMap.Put(TAG_TAG_gamt, "Gamut");
            _tagNameMap.Put(TAG_TAG_kTRC, "Gray TRC");
            _tagNameMap.Put(TAG_TAG_gXYZ, "Green Colorant");
            _tagNameMap.Put(TAG_TAG_gTRC, "Green TRC");
            _tagNameMap.Put(TAG_TAG_lumi, "Luminance");
            _tagNameMap.Put(TAG_TAG_meas, "Measurement");
            _tagNameMap.Put(TAG_TAG_bkpt, "Media Black Point");
            _tagNameMap.Put(TAG_TAG_wtpt, "Media White Point");
            _tagNameMap.Put(TAG_TAG_ncol, "Named Color");
            _tagNameMap.Put(TAG_TAG_ncl2, "Named Color 2");
            _tagNameMap.Put(TAG_TAG_resp, "Output Response");
            _tagNameMap.Put(TAG_TAG_pre0, "Preview 0");
            _tagNameMap.Put(TAG_TAG_pre1, "Preview 1");
            _tagNameMap.Put(TAG_TAG_pre2, "Preview 2");
            _tagNameMap.Put(TAG_TAG_desc, "Profile Description");
            _tagNameMap.Put(TAG_TAG_pseq, "Profile Sequence Description");
            _tagNameMap.Put(TAG_TAG_psd0, "Ps2 CRD 0");
            _tagNameMap.Put(TAG_TAG_psd1, "Ps2 CRD 1");
            _tagNameMap.Put(TAG_TAG_psd2, "Ps2 CRD 2");
            _tagNameMap.Put(TAG_TAG_psd3, "Ps2 CRD 3");
            _tagNameMap.Put(TAG_TAG_ps2s, "Ps2 CSA");
            _tagNameMap.Put(TAG_TAG_ps2i, "Ps2 Rendering Intent");
            _tagNameMap.Put(TAG_TAG_rXYZ, "Red Colorant");
            _tagNameMap.Put(TAG_TAG_rTRC, "Red TRC");
            _tagNameMap.Put(TAG_TAG_scrd, "Screening Desc");
            _tagNameMap.Put(TAG_TAG_scrn, "Screening");
            _tagNameMap.Put(TAG_TAG_tech, "Technology");
            _tagNameMap.Put(TAG_TAG_bfd, "Ucrbg");
            _tagNameMap.Put(TAG_TAG_vued, "Viewing Conditions Description");
            _tagNameMap.Put(TAG_TAG_view, "Viewing Conditions");
            _tagNameMap.Put(TagAppleMultiLanguageProfileName, "Apple Multi-language Profile Name");
        }

        public IccDirectory()
        {
            this.SetDescriptor(new IccDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "ICC Profile";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
