// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvVideoDirectory : MkvTrackDirectory
{
    public const int TagPixelHeight = ElementIds.PixelHeight;
    public const int TagPixelWidth = ElementIds.PixelWidth;
    public const int TagFlagInterlaced = ElementIds.FlagInterlaced;
    public const int TagChromaSitingHorz = ElementIds.ChromaSitingHorz;
    public const int TagChromaSitingVert = ElementIds.ChromaSitingVert;
    public const int TagTransferCharacteristics = ElementIds.TransferCharacteristics;
    public const int TagMatrixCoefficients = ElementIds.MatrixCoefficients;
    public const int TagPrimaries = ElementIds.Primaries;
    public const int TagRange = ElementIds.ColourRange;
    public const int TagDisplayWidth = ElementIds.DisplayWidth;
    public const int TagDisplayHeight = ElementIds.DisplayHeight;
    public const int TagDisplayUnit = ElementIds.DisplayUnit;

    /// <summary>Nominal frame rate derived from DefaultDuration, not a measured average.</summary>
    public const int TagFrameRate = 1;

    private static readonly Dictionary<int, string> _tagNameMap = new(CreateTagNameMap())
    {
        { TagPixelHeight, "Pixel Height" },
        { TagPixelWidth, "Pixel Width" },
        { TagFlagInterlaced, "Interlaced" },
        { TagChromaSitingHorz, "Horizontal Chroma Siting" },
        { TagChromaSitingVert, "Vertical Chroma Siting" },
        { TagTransferCharacteristics, "Colour Transfer Characteristics" },
        { TagMatrixCoefficients, "Colour Matrix Coefficients" },
        { TagPrimaries, "Colour Primaries" },
        { TagRange, "Colour Range" },
        { TagDisplayWidth, "Display Width" },
        { TagDisplayHeight, "Display Height" },
        { TagDisplayUnit, "Display Unit" },
        { TagFrameRate, "Nominal Frame Rate" }
    };

    public MkvVideoDirectory() : base(_tagNameMap) => SetDescriptor(new MkvVideoDescriptor(this));

    public override string Name => "Matroska Video";
}
