// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Exif.Makernotes.SamsungType2MakernoteDirectory;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SamsungType2MakernoteDirectory"/>.
    /// <para />
    /// Tag reference from: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Samsung.html
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SamsungType2MakernoteDescriptor : TagDescriptor<SamsungType2MakernoteDirectory>
    {
        public SamsungType2MakernoteDescriptor(SamsungType2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                TagMakerNoteVersion => GetMakernoteVersionDescription(),
                TagDeviceType => GetDeviceTypeDescription(),
                TagSamsungModelId => GetSamsungModelIdDescription(),

                TagRawDataByteOrder => GetRawDataByteOrderDescription(),
                TagWhiteBalanceSetup => GetWhiteBalanceSetupDescription(),

                TagCameraTemperature => GetCameraTemperatureDescription(),

                TagRawDataCfaPattern => GetRawDataCfaPatternDescription(),

                TagFaceDetect => GetFaceDetectDescription(),
                TagFaceRecognition => GetFaceRecognitionDescription(),

                TagLensType => GetLensTypeDescription(),

                TagColorSpace => GetColorSpaceDescription(),
                TagSmartRange => GetSmartRangeDescription(),

                _ => base.GetDescription(tagType),
            };
        }

        public string? GetMakernoteVersionDescription()
        {
            return GetVersionBytesDescription(TagMakerNoteVersion, 2);
        }

        public string? GetDeviceTypeDescription()
        {
            if (!Directory.TryGetUInt32(TagDeviceType, out uint value))
                return null;

            return value switch
            {
                0x1000 => "Compact Digital Camera",
                0x2000 => "High-end NX Camera",
                0x3000 => "HXM Video Camera",
                0x12000 => "Cell Phone",
                0x300000 => "SMX Video Camera",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetSamsungModelIdDescription()
        {
            if (!Directory.TryGetUInt32(TagSamsungModelId, out uint value))
                return null;

            return value switch
            {
                0x100101c => "NX10",
                /*case 0x1001226:
                        return "HMX-S10BP";*/
                0x1001226 => "HMX-S15BP",
                0x1001233 => "HMX-Q10",
                /*case 0x1001234:
                        return "HMX-H300";*/
                0x1001234 => "HMX-H304",
                0x100130c => "NX100",
                0x1001327 => "NX11",
                0x170104e => "ES70, ES71 / VLUU ES70, ES71 / SL600",
                0x1701052 => "ES73 / VLUU ES73 / SL605",
                0x1701300 => "ES28 / VLUU ES28",
                0x1701303 => "ES74,ES75,ES78 / VLUU ES75,ES78",
                0x2001046 => "PL150 / VLUU PL150 / TL210 / PL151",
                0x2001311 => "PL120,PL121 / VLUU PL120,PL121",
                0x2001315 => "PL170,PL171 / VLUUPL170,PL171",
                0x200131e => "PL210, PL211 / VLUU PL210, PL211",
                0x2701317 => "PL20,PL21 / VLUU PL20,PL21",
                0x2a0001b => "WP10 / VLUU WP10 / AQ100",
                0x3000000 => "Various Models (0x3000000)",
                0x3a00018 => "Various Models (0x3a00018)",
                0x400101f => "ST1000 / ST1100 / VLUU ST1000 / CL65",
                0x4001022 => "ST550 / VLUU ST550 / TL225",
                0x4001025 => "Various Models (0x4001025)",
                0x400103e => "VLUU ST5500, ST5500, CL80",
                0x4001041 => "VLUU ST5000, ST5000, TL240",
                0x4001043 => "ST70 / VLUU ST70 / ST71",
                0x400130a => "Various Models (0x400130a)",
                0x400130e => "ST90,ST91 / VLUU ST90,ST91",
                0x4001313 => "VLUU ST95, ST95",
                0x4a00015 => "VLUU ST60",
                0x4a0135b => "ST30, ST65 / VLUU ST65 / ST67",
                0x5000000 => "Various Models (0x5000000)",
                0x5001038 => "Various Models (0x5001038)",
                0x500103a => "WB650 / VLUU WB650 / WB660",
                0x500103c => "WB600 / VLUU WB600 / WB610",
                0x500133e => "WB150 / WB150F / WB152 / WB152F / WB151",
                0x5a0000f => "WB5000 / HZ25W",
                0x6001036 => "EX1",
                0x700131c => "VLUU SH100, SH100",
                0x27127002 => "SMX - C20N",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetRawDataByteOrderDescription()
        {
            return GetIndexedDescription(TagRawDataByteOrder, "Little-endian (Intel)", "Big-endian (Motorola)");
        }

        public string? GetWhiteBalanceSetupDescription()
        {
            return GetIndexedDescription(TagWhiteBalanceSetup, "Auto", "Manual");
        }

        public string? GetCameraTemperatureDescription()
        {
            return GetFormattedInt(TagCameraTemperature, "{0} C");
        }


        public string? GetRawDataCfaPatternDescription()
        {
            if (!Directory.TryGetInt32(TagRawDataCfaPattern, out int value))
                return null;

            return value switch
            {
                0 => "Unchanged",
                1 => "Swap",
                65535 => "Roll",
                _ => $"Unknown ({value})"
            };
        }

        public string? GetFaceDetectDescription()
        {
            return GetIndexedDescription(TagFaceDetect, "Off", "On");
        }

        public string? GetFaceRecognitionDescription()
        {
            return GetIndexedDescription(TagFaceRecognition, "Off", "On");
        }

        public string? GetLensTypeDescription()
        {
            return GetIndexedDescription(TagLensType,
                "Built-in or Manual Lens",
                "Samsung NX 30mm F2 Pancake",
                "Samsung NX 18-55mm F3.5-5.6 OIS",
                "Samsung NX 50-200mm F4-5.6 ED OIS",
                "Samsung NX 20-50mm F3.5-5.6 ED",
                "Samsung NX 20mm F2.8 Pancake",
                "Samsung NX 18-200mm F3.5-6.3 ED OIS",
                "Samsung NX 60mm F2.8 Macro ED OIS SSA",
                "Samsung NX 16mm F2.4 Pancake",
                "Samsung NX 85mm F1.4 ED SSA",
                "Samsung NX 45mm F1.8",
                "Samsung NX 45mm F1.8 2D/3D",
                "Samsung NX 12-24mm F4-5.6 ED",
                "Samsung NX 16-50mm F2-2.8 S ED OIS",
                "Samsung NX 10mm F3.5 Fisheye",
                "Samsung NX 16-50mm F3.5-5.6 Power Zoom ED OIS",
                null,
                null,
                null,
                null,
                "Samsung NX 50-150mm F2.8 S ED OIS",
                "Samsung NX 300mm F2.8 ED OIS");
        }

        public string? GetColorSpaceDescription()
        {
            return GetIndexedDescription(TagColorSpace, "sRGB", "Adobe RGB");
        }

        public string? GetSmartRangeDescription()
        {
            return GetIndexedDescription(TagSmartRange, "Off", "On");
        }
    }
}
