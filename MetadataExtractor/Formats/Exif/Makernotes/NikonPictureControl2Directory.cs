// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

public sealed class NikonPictureControl2Directory : Directory
{
    // Tag values are offsets into the underlying data.
    // Data from https://exiftool.org/TagNames/Nikon.html#PictureControl

    public const int TagPictureControlVersion = 0;
    public const int TagPictureControlName = 4;
    public const int TagPictureControlBase = 24;
    public const int TagPictureControlAdjust = 48;
    public const int TagPictureControlQuickAdjust = 49;
    // skip 1
    public const int TagSharpness = 51;
    // skip 1
    public const int TagClarity = 53;
    // skip 1
    public const int TagContrast = 55;
    // skip 1
    public const int TagBrightness = 57;
    // skip 1
    public const int TagSaturation = 59;
    // skip 1
    public const int TagHue = 61;
    // skip 1
    public const int TagFilterEffect = 63;
    public const int TagToningEffect = 64;
    public const int TagToningSaturation = 65;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagPictureControlVersion, "Picture Control Version" },
        { TagPictureControlName, "Picture Control Name" },
        { TagPictureControlBase, "Picture Control Base" },
        { TagPictureControlAdjust, "Picture Control Adjust" },
        { TagPictureControlQuickAdjust, "Picture Control Quick Adjust" },
        { TagSharpness, "Sharpness" },
        { TagClarity, "Clarity" },
        { TagContrast, "Contrast" },
        { TagBrightness, "Brightness" },
        { TagSaturation, "Saturation" },
        { TagHue, "Hue" },
        { TagFilterEffect, "Filter Effect" },
        { TagToningEffect, "Toning Effect" },
        { TagToningSaturation, "Toning Saturation" },
    };

    public NikonPictureControl2Directory() : base(_tagNameMap)
    {
        SetDescriptor(new NikonPictureControl2Descriptor(this));
    }

    public override string Name => "Nikon PictureControl 2";

    internal static NikonPictureControl2Directory FromBytes(byte[] bytes)
    {
        const int ExpectedLength = 68;

        if (bytes.Length != ExpectedLength)
        {
            throw new ArgumentException($"Must have {ExpectedLength} bytes.");
        }

        SequentialByteArrayReader reader = new(bytes);

        NikonPictureControl2Directory directory = new();

        directory.Set(TagPictureControlVersion, reader.GetStringValue(4));
        directory.Set(TagPictureControlName, reader.GetStringValue(20));
        directory.Set(TagPictureControlBase, reader.GetStringValue(20));
        reader.Skip(4);
        directory.Set(TagPictureControlAdjust, reader.GetByte());
        directory.Set(TagPictureControlQuickAdjust, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagSharpness, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagClarity, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagContrast, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagBrightness, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagSaturation, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagHue, reader.GetByte());
        reader.Skip(1);
        directory.Set(TagFilterEffect, reader.GetByte());
        directory.Set(TagToningEffect, reader.GetByte());
        directory.Set(TagToningSaturation, reader.GetByte());
        reader.Skip(2);

        Debug.Assert(reader.Position == ExpectedLength);

        return directory;
    }
}
