#region License
//
// Copyright 2002-2016 Drew Noakes
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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif
{
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawWbInfoDescriptor : TagDescriptor<PanasonicRawWbInfoDirectory>
    {
        public PanasonicRawWbInfoDescriptor([NotNull] PanasonicRawWbInfoDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PanasonicRawWbInfoDirectory.TagWbType1:
                case PanasonicRawWbInfoDirectory.TagWbType2:
                case PanasonicRawWbInfoDirectory.TagWbType3:
                case PanasonicRawWbInfoDirectory.TagWbType4:
                case PanasonicRawWbInfoDirectory.TagWbType5:
                case PanasonicRawWbInfoDirectory.TagWbType6:
                case PanasonicRawWbInfoDirectory.TagWbType7:
                    return GetWbTypeDescription(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetWbTypeDescription(int tagType)
        {
            ushort wbtype;
            if (!Directory.TryGetUInt16(tagType, out wbtype))
                return null;

            return GetLightSourceDescription(wbtype);
        }

        // EXIF LightSource
        public string GetLightSourceDescription(ushort wbtype)
        {
            switch(wbtype)
            {
                case 0:
                    return "Unknown";
                case 1:
                    return "Daylight";
                case 2:
                    return "Fluorescent";
                case 3:
                    return "Tungsten (Incandescent)";
                case 4:
                    return "Flash";
                case 9:
                    return "Fine Weather";
                case 10:
                    return "Cloudy";
                case 11:
                    return "Shade";
                case 12:
                    return "Daylight Fluorescent";    // (D 5700 - 7100K)
                case 13:
                    return "Day White Fluorescent";   // (N 4600 - 5500K)
                case 14:
                    return "Cool White Fluorescent";  // (W 3800 - 4500K)
                case 15:
                    return "White Fluorescent";       // (WW 3250 - 3800K)
                case 16:
                    return "Warm White Fluorescent";  // (L 2600 - 3250K)
                case 17:
                    return "Standard Light A";
                case 18:
                    return "Standard Light B";
                case 19:
                    return "Standard Light C";
                case 20:
                    return "D55";
                case 21:
                    return "D65";
                case 22:
                    return "D75";
                case 23:
                    return "D50";
                case 24:
                    return "ISO Studio Tungsten";
                case 255:
                    return "Other";
            }

            return base.GetDescription(wbtype);
        }

    }
}
