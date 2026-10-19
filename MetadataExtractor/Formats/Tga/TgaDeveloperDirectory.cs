// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga;

/// <author>Dmitry Shechtman</author>
public sealed class TgaDeveloperDirectory : Directory
{
    public TgaDeveloperDirectory() : base(null)
    {
        SetDescriptor(new TagDescriptor<TgaDeveloperDirectory>(this));
    }

    public override string Name => "TGA Developer Area";
}
