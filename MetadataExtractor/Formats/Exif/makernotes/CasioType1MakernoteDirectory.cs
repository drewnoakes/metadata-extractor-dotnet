// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

/// <summary>Describes tags specific to Casio (type 1) cameras.</summary>
/// <remarks>
/// Describes tags specific to Casio (type 1) cameras.
/// A standard TIFF IFD directory but always uses Motorola (Big-Endian) Byte Alignment.
/// Makernote data begins immediately (no header).
/// </remarks>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class CasioType1MakernoteDirectory : Directory
{
    public const int TagRecordingMode = 0x0001;
    public const int TagQuality = 0x0002;
    public const int TagFocusingMode = 0x0003;
    public const int TagFlashMode = 0x0004;
    public const int TagFlashIntensity = 0x0005;
    public const int TagObjectDistance = 0x0006;
    public const int TagWhiteBalance = 0x0007;
    public const int TagUnknown1 = 0x0008;
    public const int TagUnknown2 = 0x0009;
    public const int TagDigitalZoom = 0x000A;
    public const int TagSharpness = 0x000B;
    public const int TagContrast = 0x000C;
    public const int TagSaturation = 0x000D;
    public const int TagUnknown3 = 0x000E;
    public const int TagUnknown4 = 0x000F;
    public const int TagUnknown5 = 0x0010;
    public const int TagUnknown6 = 0x0011;
    public const int TagUnknown7 = 0x0012;
    public const int TagUnknown8 = 0x0013;
    public const int TagCcdSensitivity = 0x0014;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagCcdSensitivity, "CCD Sensitivity" },
        { TagContrast, "Contrast" },
        { TagDigitalZoom, "Digital Zoom" },
        { TagFlashIntensity, "Flash Intensity" },
        { TagFlashMode, "Flash Mode" },
        { TagFocusingMode, "Focusing Mode" },
        { TagObjectDistance, "Object Distance" },
        { TagQuality, "Quality" },
        { TagRecordingMode, "Recording Mode" },
        { TagSaturation, "Saturation" },
        { TagSharpness, "Sharpness" },
        { TagUnknown1, "Makernote Unknown 1" },
        { TagUnknown2, "Makernote Unknown 2" },
        { TagUnknown3, "Makernote Unknown 3" },
        { TagUnknown4, "Makernote Unknown 4" },
        { TagUnknown5, "Makernote Unknown 5" },
        { TagUnknown6, "Makernote Unknown 6" },
        { TagUnknown7, "Makernote Unknown 7" },
        { TagUnknown8, "Makernote Unknown 8" },
        { TagWhiteBalance, "White Balance" }
    };

    public CasioType1MakernoteDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new CasioType1MakernoteDescriptor(this));
    }

    public override string Name => "Casio Makernote";
}
