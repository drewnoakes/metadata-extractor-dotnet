// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

public sealed class EbmlDirectory : Directory
{
    public const int TagEbmlVersion = ElementIds.EbmlVersion;
    public const int TagEbmlReadVersion = ElementIds.EbmlReadVersion;
    public const int TagEbmlMaxIdLength = ElementIds.EbmlMaxIdLength;
    public const int TagEbmlMaxSizeLength = ElementIds.EbmlMaxSizeLength;
    public const int TagDocType = ElementIds.DocType;
    public const int TagDocTypeVersion = ElementIds.DocTypeVersion;
    public const int TagDocTypeReadVersion = ElementIds.DocTypeReadVersion;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagEbmlVersion, "Version" },
        { TagEbmlReadVersion, "Read Version" },
        { TagEbmlMaxIdLength, "Maximum ID Length" },
        { TagEbmlMaxSizeLength, "Maximum Size Length" },
        { TagDocType, "Document Type" },
        { TagDocTypeVersion, "Document Type Version" },
        { TagDocTypeReadVersion, "Document Type Read Version" }
    };

    public EbmlDirectory() : base(_tagNameMap) => SetDescriptor(new TagDescriptor<EbmlDirectory>(this));

    public override string Name => "EBML";
}
