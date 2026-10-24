// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Dng.DngDirectory;

namespace MetadataExtractor.Formats.Dng;

public sealed class DngDescriptor(DngDirectory directory) : TagDescriptor<DngDirectory>(directory)
{
    public override string? GetDescription(int tagType) => tagType switch
    {
        TagDngVersion => GetDngVersionDescription(),
        TagDngBackwardVersion => GetDngBackwardVersionDescription(),
        TagMakerNoteSafety => GetMakerNoteSafetyDescription(),
        TagColorimetricReference => GetColorimetricReferenceDescription(),
        TagProfileEmbedPolicy => GetProfileEmbedPolicyDescription(),
        TagPreviewColorSpace => GetPreviewColorSpaceDescription(),
        TagProfileHueSatMapEncoding => GetProfileHueSatMapEncodingDescription(),
        TagProfileLookTableEncoding => GetProfileLookTableEncodingDescription(),
        TagDefaultBlackRender => GetDefaultBlackRenderDescription(),
        _ => base.GetDescription(tagType)
    };

    public string? GetDngVersionDescription() => GetDngVersionDescription(TagDngVersion);

    public string? GetDngBackwardVersionDescription() => GetDngVersionDescription(TagDngBackwardVersion);

    public string? GetMakerNoteSafetyDescription() => GetIndexedDescription(TagMakerNoteSafety, "Unsafe", "Safe");

    public string? GetColorimetricReferenceDescription() => GetIndexedDescription(TagColorimetricReference, "Scene-referred", "Output-referred");

    public string? GetProfileEmbedPolicyDescription() => GetIndexedDescription(
        TagProfileEmbedPolicy, "Allow copying", "Embed if used", "Never embed", "No restrictions");

    public string? GetPreviewColorSpaceDescription() => GetIndexedDescription(
        TagPreviewColorSpace, "Unknown", "Gray Gamma 2.2", "sRGB", "Adobe RGB", "ProPhoto RGB");

    public string? GetProfileHueSatMapEncodingDescription() => GetIndexedDescription(TagProfileHueSatMapEncoding, "Linear", "sRGB");

    public string? GetProfileLookTableEncodingDescription() => GetIndexedDescription(TagProfileLookTableEncoding, "Linear", "sRGB");

    public string? GetDefaultBlackRenderDescription() => GetIndexedDescription(TagDefaultBlackRender, "Auto", "None");

    private string? GetDngVersionDescription(int tagType)
    {
        var bytes = Directory.GetByteArray(tagType);
        return bytes is { Length: 4 } ? $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}" : null;
    }
}
