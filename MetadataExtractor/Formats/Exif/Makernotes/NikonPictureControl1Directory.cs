// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

public sealed class NikonPictureControl1Directory : Directory
{
    // Tag values are offsets into the underlying data.
    // Data from https://exiftool.org/TagNames/Nikon.html#PictureControl

    public const int TagPictureControlVersion = 0;
    public const int TagPictureControlName = 4;
    public const int TagPictureControlBase = 24;
    // skip 4
    public const int TagPictureControlAdjust = 48;
    public const int TagPictureControlQuickAdjust = 49;
    public const int TagSharpness = 50;
    public const int TagContrast = 51;
    public const int TagBrightness = 52;
    public const int TagSaturation = 53;
    public const int TagHueAdjustment = 54;
    public const int TagFilterEffect = 55;
    public const int TagToningEffect = 56;
    public const int TagToningSaturation = 57;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagPictureControlVersion, "Picture Control Version" },
        { TagPictureControlName, "Picture Control Name" },
        { TagPictureControlBase, "Picture Control Base" },
        { TagPictureControlAdjust, "Picture Control Adjust" },
        { TagPictureControlQuickAdjust, "Picture Control Quick Adjust" },
        { TagSharpness, "Sharpness" },
        { TagContrast, "Contrast" },
        { TagBrightness, "Brightness" },
        { TagSaturation, "Saturation" },
        { TagHueAdjustment, "Hue Adjustment" },
        { TagFilterEffect, "Filter Effect" },
        { TagToningEffect, "Toning Effect" },
        { TagToningSaturation, "Toning Saturation" },
    };

    public NikonPictureControl1Directory() : base(_tagNameMap)
    {
        SetDescriptor(new NikonPictureControl1Descriptor(this));
    }

    public override string Name => "Nikon PictureControl 1";

    internal static NikonPictureControl1Directory FromBytes(byte[] bytes)
    {
        const int ExpectedLength = 58;

        if (bytes.Length != ExpectedLength)
        {
            throw new ArgumentException($"Must have {ExpectedLength} bytes.");
        }

        SequentialByteArrayReader reader = new(bytes);

        NikonPictureControl1Directory directory = new();

        directory.Set(TagPictureControlVersion, reader.GetNullTerminatedStringValue(4));
        directory.Set(TagPictureControlName, reader.GetNullTerminatedStringValue(20));
        directory.Set(TagPictureControlBase, reader.GetNullTerminatedStringValue(20));
        reader.Skip(4);
        directory.Set(TagPictureControlAdjust, reader.GetByte());
        directory.Set(TagPictureControlQuickAdjust, reader.GetByte());
        directory.Set(TagSharpness, reader.GetByte());
        directory.Set(TagContrast, reader.GetByte());
        directory.Set(TagBrightness, reader.GetByte());
        directory.Set(TagSaturation, reader.GetByte());
        directory.Set(TagHueAdjustment, reader.GetByte());
        directory.Set(TagFilterEffect, reader.GetByte());
        directory.Set(TagToningEffect, reader.GetByte());
        directory.Set(TagToningSaturation, reader.GetByte());

        Debug.Assert(reader.Position == ExpectedLength);

        return directory;
    }
}
