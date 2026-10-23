// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.MkvVideoDirectory;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvVideoDescriptor(MkvVideoDirectory directory) : MkvTrackDescriptor<MkvVideoDirectory>(directory)
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagFlagInterlaced => GetFlagInterlacedDescription(),
        TagDisplayUnit => GetDisplayUnitDescription(),
        TagRange => GetRangeDescription(),
        TagChromaSitingHorz => GetChromaSitingHorzDescription(),
        TagChromaSitingVert => GetChromaSitingVertDescription(),
        TagMatrixCoefficients => GetMatrixCoefficientsDescription(),
        TagTransferCharacteristics => GetTransferCharacteristicsDescription(),
        TagPrimaries => GetPrimariesDescription(),
        TagFrameRate => GetFrameRateDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetFlagInterlacedDescription()
        => GetIndexedDescription(TagFlagInterlaced, "Undetermined", "Interlaced", "Progressive");

    public string? GetDisplayUnitDescription()
        => GetIndexedDescription(TagDisplayUnit, "Pixels", "Centimeters", "Inches", "Display Aspect Ratio", "Unknown");

    public string? GetRangeDescription()
        => GetIndexedDescription(TagRange, "Unspecified", "Broadcast Range", "Full Range",
            "Defined by Matrix Coefficients and Transfer Characteristics");

    public string? GetChromaSitingHorzDescription()
        => GetIndexedDescription(TagChromaSitingHorz, "Unspecified", "Left", "Half");

    public string? GetChromaSitingVertDescription()
        => GetIndexedDescription(TagChromaSitingVert, "Unspecified", "Top", "Half");

    public string? GetMatrixCoefficientsDescription()
        => GetIndexedDescription(TagMatrixCoefficients, "Identity", "BT.709", "Unspecified", null,
            "FCC 73.682", "BT.470BG", "SMPTE 170M", "SMPTE 240M", "YCoCg",
            "BT.2020 Non-constant Luminance", "BT.2020 Constant Luminance", "SMPTE 2085",
            "Chromaticity-derived Non-constant Luminance", "Chromaticity-derived Constant Luminance", "ICtCp");

    public string? GetTransferCharacteristicsDescription()
        => GetIndexedDescription(TagTransferCharacteristics, 1, "BT.709", "Unspecified", null,
            "Gamma 2.2", "Gamma 2.8", "SMPTE 170M", "SMPTE 240M", "Linear",
            "Logarithmic (100:1)", "Logarithmic (316:1)", "IEC 61966-2-4", "BT.1361",
            "IEC 61966-2-1 (sRGB)", "BT.2020 10-bit", "BT.2020 12-bit",
            "SMPTE 2084 (PQ)", "SMPTE 428-1", "ARIB STD-B67 (HLG)");

    public string? GetPrimariesDescription()
        => Directory.TryGetUInt64(TagPrimaries, out var value) && value == 22
            ? "EBU Tech. 3213-E"
            : GetIndexedDescription(TagPrimaries, 1, "BT.709", "Unspecified", null,
                "BT.470M", "BT.470BG", "SMPTE 170M", "SMPTE 240M", "Film", "BT.2020",
                "SMPTE 428-1", "SMPTE RP 431-2", "SMPTE EG 432-1");

    public string? GetFrameRateDescription() => GetFormattedString(TagFrameRate, "{0} fps");
}
