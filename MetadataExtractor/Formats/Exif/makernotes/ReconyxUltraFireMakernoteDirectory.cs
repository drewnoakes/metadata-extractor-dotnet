// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

/// <summary>Describes tags specific to Reconyx UltraFire cameras.</summary>
/// <author>Todd West http://cascadescarnivoreproject.blogspot.com</author>
/// <remarks>Reconyx uses a fixed makernote block.  Tag values are the byte index of the tag within the makernote.</remarks>
public class ReconyxUltraFireMakernoteDirectory : Directory
{
    /// <summary>
    /// Version number used for identifying makernotes from Reconyx UltraFire cameras.
    /// </summary>
    public static readonly uint MakernoteId = 0x00010000;

    /// <summary>
    /// Version number used for identifying the public portion of makernotes from Reconyx UltraFire cameras.
    /// </summary>
    public static readonly uint MakernotePublicId = 0x07f10001;

    public const int TagLabel = 0;
    public const int TagMakernoteId = 10;
    public const int TagMakernoteSize = 14;
    public const int TagMakernotePublicId = 18;
    public const int TagMakernotePublicSize = 22;
    public const int TagCameraVersion = 24;
    public const int TagUibVersion = 31;
    public const int TagBtlVersion = 38;
    public const int TagPexVersion = 45;
    public const int TagEventType = 52;
    public const int TagSequence = 53;
    public const int TagEventNumber = 55;
    public const int TagDateTimeOriginal = 59;
    public const int TagDayOfWeek = 66;
    public const int TagMoonPhase = 67;
    public const int TagAmbientTemperatureFahrenheit = 68;
    public const int TagAmbientTemperature = 70;
    public const int TagFlash = 72;
    public const int TagBatteryVoltage = 73;
    public const int TagSerialNumber = 75;
    public const int TagUserLabel = 90;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
         { TagLabel, "Makernote Label" },
         { TagMakernoteId, "Makernote ID" },
         { TagMakernoteSize, "Makernote Size" },
         { TagMakernotePublicId, "Makernote Public ID" },
         { TagMakernotePublicSize, "Makernote Public Size" },
         { TagCameraVersion, "Camera Version" },
         { TagUibVersion, "Uib Version" },
         { TagBtlVersion, "Btl Version" },
         { TagPexVersion, "Pex Version" },
         { TagEventType, "Event Type" },
         { TagSequence, "Sequence" },
         { TagEventNumber, "Event Number" },
         { TagDateTimeOriginal, "Date/Time Original" },
         { TagDayOfWeek, "Day of Week" },
         { TagMoonPhase, "Moon Phase" },
         { TagAmbientTemperatureFahrenheit, "Ambient Temperature Fahrenheit" },
         { TagAmbientTemperature, "Ambient Temperature" },
         { TagFlash, "Flash" },
         { TagBatteryVoltage, "Battery Voltage" },
         { TagSerialNumber, "Serial Number" },
         { TagUserLabel, "User Label" }
    };

    public ReconyxUltraFireMakernoteDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new ReconyxUltraFireMakernoteDescriptor(this));
    }

    public override string Name => "Reconyx UltraFire Makernote";
}
