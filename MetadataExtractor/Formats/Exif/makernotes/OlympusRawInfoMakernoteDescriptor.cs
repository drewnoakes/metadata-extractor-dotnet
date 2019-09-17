// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        public OlympusRawInfoMakernoteDescriptor(OlympusRawInfoMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                OlympusRawInfoMakernoteDirectory.TagRawInfoVersion => GetVersionBytesDescription(OlympusRawInfoMakernoteDirectory.TagRawInfoVersion, 4),
                OlympusRawInfoMakernoteDirectory.TagColorMatrix2 => GetColorMatrix2Description(),
                OlympusRawInfoMakernoteDirectory.TagYCbCrCoefficients => GetYCbCrCoefficientsDescription(),
                OlympusRawInfoMakernoteDirectory.TagLightSource => GetOlympusLightSourceDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetColorMatrix2Description()
        {
            if (!(Directory.GetObject(OlympusRawInfoMakernoteDirectory.TagColorMatrix2) is short[] values))
                return null;

            return string.Join(" ", values.Select(b => b.ToString()).ToArray());
        }

        public string? GetYCbCrCoefficientsDescription()
        {
            if (!(Directory.GetObject(OlympusRawInfoMakernoteDirectory.TagYCbCrCoefficients) is ushort[] values))
                return null;

            var ret = new Rational[values.Length / 2];
            for(var i = 0; i < values.Length / 2; i++)
            {
                ret[i] = new Rational(values[2*i], values[2*i + 1]);
            }

            return string.Join(" ", ret.Select(r => r.ToDecimal().ToString()).ToArray());
        }

        public string? GetOlympusLightSourceDescription()
        {
            if (!Directory.TryGetUInt16(OlympusRawInfoMakernoteDirectory.TagLightSource, out ushort value))
                return null;

            return value switch
            {
                0 => "Unknown",
                16 => "Shade",
                17 => "Cloudy",
                18 => "Fine Weather",
                20 => "Tungsten (Incandescent)",
                22 => "Evening Sunlight",
                33 => "Daylight Fluorescent",
                34 => "Day White Fluorescent",
                35 => "Cool White Fluorescent",
                36 => "White Fluorescent",
                256 => "One Touch White Balance",
                512 => "Custom 1-4",
                _ => "Unknown (" + value + ")",
            };
        }
    }
}
