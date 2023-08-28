// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

public sealed class AppleRunTimeMakernoteDescriptor : TagDescriptor<AppleRunTimeMakernoteDirectory>
{
    public AppleRunTimeMakernoteDescriptor(AppleRunTimeMakernoteDirectory directory) : base(directory)
    {
    }

    public override string? GetDescription(int tagType)
    {
        return tagType switch
        {
            AppleRunTimeMakernoteDirectory.TagFlags => GetFlagsDescription(),
            AppleRunTimeMakernoteDirectory.TagValue => GetValueDescription(),
            _ => base.GetDescription(tagType),
        };
    }

    public string? GetFlagsDescription()
    {
        // flags bitmask details
        // 0000 0001 = Valid
        // 0000 0010 = Rounded
        // 0000 0100 = Positive Infinity
        // 0000 1000 = Negative Infinity
        // 0001 0000 = Indefinite

        if (Directory.TryGetInt32(AppleRunTimeMakernoteDirectory.TagFlags, out var value))
        {
            StringBuilder sb = new();

            if ((value & 0x1) != 0)
                sb.Append("Valid");
            else
                sb.Append("Invalid");

            if ((value & 0x2) != 0)
                sb.Append(", rounded");

            if ((value & 0x4) != 0)
                sb.Append(", positive infinity");

            if ((value & 0x8) != 0)
                sb.Append(", negative infinity");

            if ((value & 0x10) != 0)
                sb.Append(", indefinite");

            return sb.ToString();
        }

        return base.GetDescription(AppleRunTimeMakernoteDirectory.TagFlags);
    }

    public string? GetValueDescription()
    {
        if (Directory.TryGetInt64(AppleRunTimeMakernoteDirectory.TagValue, out var value) &&
            Directory.TryGetInt64(AppleRunTimeMakernoteDirectory.TagScale, out var scale))
        {
            return $"{value / scale} seconds";
        }

        return base.GetDescription(AppleRunTimeMakernoteDirectory.TagValue);
    }
}
