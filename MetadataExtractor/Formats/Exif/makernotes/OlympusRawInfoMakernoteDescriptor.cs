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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusRawInfoMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions converted from Exiftool version 10.33 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusRawInfoMakernoteDescriptor : TagDescriptor<OlympusRawInfoMakernoteDirectory>
    {
        public OlympusRawInfoMakernoteDescriptor([NotNull] OlympusRawInfoMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusRawInfoMakernoteDirectory.TagRawInfoVersion:
                    return GetVersionBytesDescription(OlympusRawInfoMakernoteDirectory.TagRawInfoVersion, 4);
                case OlympusRawInfoMakernoteDirectory.TagColorMatrix2:
                    return GetColorMatrix2Description();
                case OlympusRawInfoMakernoteDirectory.TagYCbCrCoefficients:
                    return GetYCbCrCoefficientsDescription();
                case OlympusRawInfoMakernoteDirectory.TagLightSource:
                    return GetOlympusLightSourceDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetColorMatrix2Description()
        {
            var values = Directory.GetObject(OlympusRawInfoMakernoteDirectory.TagColorMatrix2) as short[];
            if (values == null)
                return null;

            return string.Join(" ", values.Select(b => b.ToString()).ToArray());
        }

        [CanBeNull]
        public string GetYCbCrCoefficientsDescription()
        {
            var values = Directory.GetObject(OlympusRawInfoMakernoteDirectory.TagYCbCrCoefficients) as ushort[];
            if (values == null)
                return null;

            var ret = new Rational[values.Length / 2];
            for(var i = 0; i < values.Length / 2; i++)
            {
                ret[i] = new Rational(values[2*i], values[2*i + 1]);
            }

            return string.Join(" ", ret.Select(r => r.ToDecimal().ToString()).ToArray());
        }

        [CanBeNull]
        public string GetOlympusLightSourceDescription()
        {
            if (!Directory.TryGetUInt16(OlympusRawInfoMakernoteDirectory.TagLightSource, out ushort value))
                return null;

            switch (value)
            {
                case 0:
                    return "Unknown";
                case 16:
                    return "Shade";
                case 17:
                    return "Cloudy";
                case 18:
                    return "Fine Weather";
                case 20:
                    return "Tungsten (Incandescent)";
                case 22:
                    return "Evening Sunlight";
                case 33:
                    return "Daylight Fluorescent";
                case 34:
                    return "Day White Fluorescent";
                case 35:
                    return "Cool White Fluorescent";
                case 36:
                    return "White Fluorescent";
                case 256:
                    return "One Touch White Balance";
                case 512:
                    return "Custom 1-4";
                default:
                    return "Unknown (" + value + ")";
            }
        }
    }
}
