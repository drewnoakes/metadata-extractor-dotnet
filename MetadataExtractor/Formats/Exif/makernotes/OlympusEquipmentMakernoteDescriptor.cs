#region License
//
// Copyright 2002-2017 Drew Noakes
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
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusEquipmentMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions and the Extender and Lens types lists converted from Exiftool version 10.10 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusEquipmentMakernoteDescriptor : TagDescriptor<OlympusEquipmentMakernoteDirectory>
    {
        public OlympusEquipmentMakernoteDescriptor([NotNull] OlympusEquipmentMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusEquipmentMakernoteDirectory.TagEquipmentVersion:
                    return GetEquipmentVersionDescription();
                case OlympusEquipmentMakernoteDirectory.TagCameraType2:
                    return GetCameraType2Description();
                case OlympusEquipmentMakernoteDirectory.TagFocalPlaneDiagonal:
                    return GetFocalPlaneDiagonalDescription();
                case OlympusEquipmentMakernoteDirectory.TagBodyFirmwareVersion:
                    return GetBodyFirmwareVersionDescription();
                case OlympusEquipmentMakernoteDirectory.TagLensType:
                    return GetLensTypeDescription();
                case OlympusEquipmentMakernoteDirectory.TagLensFirmwareVersion:
                    return GetLensFirmwareVersionDescription();
                case OlympusEquipmentMakernoteDirectory.TagMaxApertureAtMinFocal:
                    return GetMaxApertureAtMinFocalDescription();
                case OlympusEquipmentMakernoteDirectory.TagMaxApertureAtMaxFocal:
                    return GetMaxApertureAtMaxFocalDescription();
                case OlympusEquipmentMakernoteDirectory.TagMaxAperture:
                    return GetMaxApertureDescription();
                case OlympusEquipmentMakernoteDirectory.TagLensProperties:
                    return GetLensPropertiesDescription();
                case OlympusEquipmentMakernoteDirectory.TagExtender:
                    return GetExtenderDescription();
                case OlympusEquipmentMakernoteDirectory.TagFlashType:
                    return GetFlashTypeDescription();
                case OlympusEquipmentMakernoteDirectory.TagFlashModel:
                    return GetFlashModelDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetEquipmentVersionDescription()
        {
            return GetVersionBytesDescription(OlympusEquipmentMakernoteDirectory.TagEquipmentVersion, 4);
        }

        [CanBeNull]
        public string GetCameraType2Description()
        {
            var cameratype = Directory.GetString(OlympusEquipmentMakernoteDirectory.TagCameraType2);
            if (cameratype == null)
                return null;

            if (OlympusMakernoteDirectory.OlympusCameraTypes.ContainsKey(cameratype))
                return OlympusMakernoteDirectory.OlympusCameraTypes[cameratype];

            return cameratype;
        }

        [CanBeNull]
        public string GetFocalPlaneDiagonalDescription()
        {
            return Directory.GetString(OlympusEquipmentMakernoteDirectory.TagFocalPlaneDiagonal) + " mm";
        }

        [CanBeNull]
        public string GetBodyFirmwareVersionDescription()
        {
            if (!Directory.TryGetInt32(OlympusEquipmentMakernoteDirectory.TagBodyFirmwareVersion, out int value))
                return null;

            var hex = ((uint)value).ToString("X4");
            return hex.Substring(0, hex.Length - 3) + "." + hex.Substring(hex.Length - 3);
        }

        [CanBeNull]
        public string GetLensTypeDescription()
        {
            var str = Directory.GetString(OlympusEquipmentMakernoteDirectory.TagLensType);

            if (str == null)
                return null;

            // The string contains six numbers:
            //
            // - Make
            // - Unknown
            // - Model
            // - Sub-model
            // - Unknown
            // - Unknown
            //
            // Only the Make, Model and Sub-model are used to identify the lens type
            var values = str.Split(' ');

            if (values.Length < 6)
                return null;


            return int.TryParse(values[0], out int num1) &&
                   int.TryParse(values[2], out int num2) &&
                   int.TryParse(values[3], out int num3) &&
                   _olympusLensTypes.TryGetValue($"{num1:X} {num2:X2} {num3:X2}", out string lensType)
                       ? lensType
                       : null;
        }

        [CanBeNull]
        public string GetLensFirmwareVersionDescription()
        {
            if (!Directory.TryGetInt32(OlympusEquipmentMakernoteDirectory.TagLensFirmwareVersion, out int value))
                return null;

            var hexstring = ((uint)value).ToString("X4");
            return hexstring.Insert(hexstring.Length - 3, ".");
        }

        [CanBeNull]
        public string GetMaxApertureAtMinFocalDescription()
        {
            if (!Directory.TryGetInt32(OlympusEquipmentMakernoteDirectory.TagMaxApertureAtMinFocal, out int value))
                return null;

            return CalcMaxAperture((ushort)value).ToString("0.#");
        }

        [CanBeNull]
        public string GetMaxApertureAtMaxFocalDescription()
        {
            if (!Directory.TryGetInt32(OlympusEquipmentMakernoteDirectory.TagMaxApertureAtMaxFocal, out int value))
                return null;

            return CalcMaxAperture((ushort)value).ToString("0.#");
        }

        [CanBeNull]
        public string GetMaxApertureDescription()
        {
            if (!Directory.TryGetInt32(OlympusEquipmentMakernoteDirectory.TagMaxAperture, out int value))
                return null;

            return CalcMaxAperture((ushort)value).ToString("0.#");
        }

        private static double CalcMaxAperture(ushort value)
        {
            return Math.Pow(Math.Sqrt(2.00), value / 256.0);
        }

        [CanBeNull]
        public string GetLensPropertiesDescription()
        {
            if (!Directory.TryGetInt32(OlympusEquipmentMakernoteDirectory.TagLensProperties, out int value))
                return null;

            return $"0x{value:X4}";
        }

        [CanBeNull]
        public string GetExtenderDescription()
        {
            var str = Directory.GetString(OlympusEquipmentMakernoteDirectory.TagExtender);

            if (str == null)
                return null;

            // The string contains six numbers:
            //
            // - Make
            // - Unknown
            // - Model
            // - Sub-model
            // - Unknown
            // - Unknown
            //
            // Only the Make and Model are used to identify the extender
            var values = str.Split(' ');

            if (values.Length < 6)
                return null;


            return int.TryParse(values[0], out int num1) &&
                   int.TryParse(values[2], out int num2) &&
                   _olympusExtenderTypes.TryGetValue($"{num1:X} {num2:X2}", out string extenderType)
                       ? extenderType
                       : null;
        }

        [CanBeNull]
        public string GetFlashTypeDescription()
        {
            return GetIndexedDescription(OlympusEquipmentMakernoteDirectory.TagFlashType,
                "None", null, "Simple E-System", "E-System");
        }

        [CanBeNull]
        public string GetFlashModelDescription()
        {
            return GetIndexedDescription(OlympusEquipmentMakernoteDirectory.TagFlashModel,
                "None", "FL-20", "FL-50", "RF-11", "TF-22", "FL-36", "FL-50R", "FL-36R");
        }

        private static readonly Dictionary<string, string> _olympusLensTypes = new Dictionary<string, string>
        {
            { "0 00 00", "None" },
            // Olympus lenses (also Kenko Tokina)
            { "0 01 00", "Olympus Zuiko Digital ED 50mm F2.0 Macro" },
            { "0 01 01", "Olympus Zuiko Digital 40-150mm F3.5-4.5" }, //8
            { "0 01 10", "Olympus M.Zuiko Digital ED 14-42mm F3.5-5.6" }, //PH (E-P1 pre-production)
            { "0 02 00", "Olympus Zuiko Digital ED 150mm F2.0" },
            { "0 02 10", "Olympus M.Zuiko Digital 17mm F2.8 Pancake" }, //PH (E-P1 pre-production)
            { "0 03 00", "Olympus Zuiko Digital ED 300mm F2.8" },
            { "0 03 10", "Olympus M.Zuiko Digital ED 14-150mm F4.0-5.6 [II]" }, //11 (The second version of this lens seems to have the same lens ID number as the first version #20)
            { "0 04 10", "Olympus M.Zuiko Digital ED 9-18mm F4.0-5.6" }, //11
            { "0 05 00", "Olympus Zuiko Digital 14-54mm F2.8-3.5" },
            { "0 05 01", "Olympus Zuiko Digital Pro ED 90-250mm F2.8" }, //9
            { "0 05 10", "Olympus M.Zuiko Digital ED 14-42mm F3.5-5.6 L" }, //11 (E-PL1)
            { "0 06 00", "Olympus Zuiko Digital ED 50-200mm F2.8-3.5" },
            { "0 06 01", "Olympus Zuiko Digital ED 8mm F3.5 Fisheye" }, //9
            { "0 06 10", "Olympus M.Zuiko Digital ED 40-150mm F4.0-5.6" }, //PH
            { "0 07 00", "Olympus Zuiko Digital 11-22mm F2.8-3.5" },
            { "0 07 01", "Olympus Zuiko Digital 18-180mm F3.5-6.3" }, //6
            { "0 07 10", "Olympus M.Zuiko Digital ED 12mm F2.0" }, //PH
            { "0 08 01", "Olympus Zuiko Digital 70-300mm F4.0-5.6" }, //7 (seen as release 1 - PH)
            { "0 08 10", "Olympus M.Zuiko Digital ED 75-300mm F4.8-6.7" }, //PH
            { "0 09 10", "Olympus M.Zuiko Digital 14-42mm F3.5-5.6 II" }, //PH (E-PL2)
            { "0 10 01", "Kenko Tokina Reflex 300mm F6.3 MF Macro" }, //20
            { "0 10 10", "Olympus M.Zuiko Digital ED 12-50mm F3.5-6.3 EZ" }, //PH
            { "0 11 10", "Olympus M.Zuiko Digital 45mm F1.8" }, //17
            { "0 12 10", "Olympus M.Zuiko Digital ED 60mm F2.8 Macro" }, //20
            { "0 13 10", "Olympus M.Zuiko Digital 14-42mm F3.5-5.6 II R" }, //PH/20
            { "0 14 10", "Olympus M.Zuiko Digital ED 40-150mm F4.0-5.6 R" }, //19
          // '0 14 10.1", "Olympus M.Zuiko Digital ED 14-150mm F4.0-5.6 II" }, //11 (questionable & unconfirmed -- all samples I can find are '0 3 10' - PH)
            { "0 15 00", "Olympus Zuiko Digital ED 7-14mm F4.0" },
            { "0 15 10", "Olympus M.Zuiko Digital ED 75mm F1.8" }, //PH
            { "0 16 10", "Olympus M.Zuiko Digital 17mm F1.8" }, //20
            { "0 17 00", "Olympus Zuiko Digital Pro ED 35-100mm F2.0" }, //7
            { "0 18 00", "Olympus Zuiko Digital 14-45mm F3.5-5.6" },
            { "0 18 10", "Olympus M.Zuiko Digital ED 75-300mm F4.8-6.7 II" }, //20
            { "0 19 10", "Olympus M.Zuiko Digital ED 12-40mm F2.8 Pro" }, //PH
            { "0 20 00", "Olympus Zuiko Digital 35mm F3.5 Macro" }, //9
            { "0 20 10", "Olympus M.Zuiko Digital ED 40-150mm F2.8 Pro" }, //20
            { "0 21 10", "Olympus M.Zuiko Digital ED 14-42mm F3.5-5.6 EZ" }, //20
            { "0 22 00", "Olympus Zuiko Digital 17.5-45mm F3.5-5.6" }, //9
            { "0 22 10", "Olympus M.Zuiko Digital 25mm F1.8" }, //20
            { "0 23 00", "Olympus Zuiko Digital ED 14-42mm F3.5-5.6" }, //PH
            { "0 23 10", "Olympus M.Zuiko Digital ED 7-14mm F2.8 Pro" }, //20
            { "0 24 00", "Olympus Zuiko Digital ED 40-150mm F4.0-5.6" }, //PH
            { "0 24 10", "Olympus M.Zuiko Digital ED 300mm F4.0 IS Pro" }, //20
            { "0 25 10", "Olympus M.Zuiko Digital ED 8mm F1.8 Fisheye Pro" }, //20
            { "0 30 00", "Olympus Zuiko Digital ED 50-200mm F2.8-3.5 SWD" }, //7
            { "0 31 00", "Olympus Zuiko Digital ED 12-60mm F2.8-4.0 SWD" }, //7
            { "0 32 00", "Olympus Zuiko Digital ED 14-35mm F2.0 SWD" }, //PH
            { "0 33 00", "Olympus Zuiko Digital 25mm F2.8" }, //PH
            { "0 34 00", "Olympus Zuiko Digital ED 9-18mm F4.0-5.6" }, //7
            { "0 35 00", "Olympus Zuiko Digital 14-54mm F2.8-3.5 II" }, //PH
            // Sigma lenses
            { "1 01 00", "Sigma 18-50mm F3.5-5.6 DC" }, //8
            { "1 01 10", "Sigma 30mm F2.8 EX DN" }, //20
            { "1 02 00", "Sigma 55-200mm F4.0-5.6 DC" },
            { "1 02 10", "Sigma 19mm F2.8 EX DN" }, //20
            { "1 03 00", "Sigma 18-125mm F3.5-5.6 DC" },
            { "1 03 10", "Sigma 30mm F2.8 DN | A" }, //20
            { "1 04 00", "Sigma 18-125mm F3.5-5.6 DC" }, //7
            { "1 04 10", "Sigma 19mm F2.8 DN | A" }, //20
            { "1 05 00", "Sigma 30mm F1.4 EX DC HSM" }, //10
            { "1 05 10", "Sigma 60mm F2.8 DN | A" }, //20
            { "1 06 00", "Sigma APO 50-500mm F4.0-6.3 EX DG HSM" }, //6
            { "1 07 00", "Sigma Macro 105mm F2.8 EX DG" }, //PH
            { "1 08 00", "Sigma APO Macro 150mm F2.8 EX DG HSM" }, //PH
            { "1 09 00", "Sigma 18-50mm F2.8 EX DC Macro" }, //20
            { "1 10 00", "Sigma 24mm F1.8 EX DG Aspherical Macro" }, //PH
            { "1 11 00", "Sigma APO 135-400mm F4.5-5.6 DG" }, //11
            { "1 12 00", "Sigma APO 300-800mm F5.6 EX DG HSM" }, //11
            { "1 13 00", "Sigma 30mm F1.4 EX DC HSM" }, //11
            { "1 14 00", "Sigma APO 50-500mm F4.0-6.3 EX DG HSM" }, //11
            { "1 15 00", "Sigma 10-20mm F4.0-5.6 EX DC HSM" }, //11
            { "1 16 00", "Sigma APO 70-200mm F2.8 II EX DG Macro HSM" }, //11
            { "1 17 00", "Sigma 50mm F1.4 EX DG HSM" }, //11
            // Panasonic/Leica lenses
            { "2 01 00", "Leica D Vario Elmarit 14-50mm F2.8-3.5 Asph." }, //11
            { "2 01 10", "Lumix G Vario 14-45mm F3.5-5.6 Asph. Mega OIS" }, //16
            { "2 02 00", "Leica D Summilux 25mm F1.4 Asph." }, //11
            { "2 02 10", "Lumix G Vario 45-200mm F4.0-5.6 Mega OIS" }, //16
            { "2 03 00", "Leica D Vario Elmar 14-50mm F3.8-5.6 Asph. Mega OIS" }, //11
            { "2 03 01", "Leica D Vario Elmar 14-50mm F3.8-5.6 Asph." }, //14 (L10 kit)
            { "2 03 10", "Lumix G Vario HD 14-140mm F4.0-5.8 Asph. Mega OIS" }, //16
            { "2 04 00", "Leica D Vario Elmar 14-150mm F3.5-5.6" }, //13
            { "2 04 10", "Lumix G Vario 7-14mm F4.0 Asph." }, //PH (E-P1 pre-production)
            { "2 05 10", "Lumix G 20mm F1.7 Asph." }, //16
            { "2 06 10", "Leica DG Macro-Elmarit 45mm F2.8 Asph. Mega OIS" }, //PH
            { "2 07 10", "Lumix G Vario 14-42mm F3.5-5.6 Asph. Mega OIS" }, //20
            { "2 08 10", "Lumix G Fisheye 8mm F3.5" }, //PH
            { "2 09 10", "Lumix G Vario 100-300mm F4.0-5.6 Mega OIS" }, //11
            { "2 10 10", "Lumix G 14mm F2.5 Asph." }, //17
            { "2 11 10", "Lumix G 12.5mm F12 3D" }, //20 (H-FT012)
            { "2 12 10", "Leica DG Summilux 25mm F1.4 Asph." }, //20
            { "2 13 10", "Lumix G X Vario PZ 45-175mm F4.0-5.6 Asph. Power OIS" }, //20
            { "2 14 10", "Lumix G X Vario PZ 14-42mm F3.5-5.6 Asph. Power OIS" }, //20
            { "2 15 10", "Lumix G X Vario 12-35mm F2.8 Asph. Power OIS" }, //PH
            { "2 16 10", "Lumix G Vario 45-150mm F4.0-5.6 Asph. Mega OIS" }, //20
            { "2 17 10", "Lumix G X Vario 35-100mm F2.8 Power OIS" }, //PH
            { "2 18 10", "Lumix G Vario 14-42mm F3.5-5.6 II Asph. Mega OIS" }, //20
            { "2 19 10", "Lumix G Vario 14-140mm F3.5-5.6 Asph. Power OIS" }, //20
            { "2 20 10", "Lumix G Vario 12-32mm F3.5-5.6 Asph. Mega OIS" }, //20
            { "2 21 10", "Leica DG Nocticron 42.5mm F1.2 Asph. Power OIS" }, //20
            { "2 22 10", "Leica DG Summilux 15mm F1.7 Asph." }, //20
          // '2 23 10", "Lumix G Vario 35-100mm F4.0-5.6 Asph. Mega OIS" }, //20 (guess)
            { "2 24 10", "Lumix G Macro 30mm F2.8 Asph. Mega OIS" }, //20
            { "2 25 10", "Lumix G 42.5mm F1.7 Asph. Power OIS" }, //20
            { "3 01 00", "Leica D Vario Elmarit 14-50mm F2.8-3.5 Asph." }, //11
            { "3 02 00", "Leica D Summilux 25mm F1.4 Asph." }, //11
            // Tamron lenses
            { "5 01 10", "Tamron 14-150mm F3.5-5.8 Di III" } //20 (model C001)
        };

        private static readonly Dictionary<string, string> _olympusExtenderTypes = new Dictionary<string, string>
        {
            { "0 00", "None" },
            { "0 04", "Olympus Zuiko Digital EC-14 1.4x Teleconverter" },
            { "0 08", "Olympus EX-25 Extension Tube" },
            { "0 10", "Olympus Zuiko Digital EC-20 2.0x Teleconverter" }
        };
    }
}
