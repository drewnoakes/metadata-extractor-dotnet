// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mpeg;

public sealed class Mp3Descriptor : TagDescriptor<Mp3Directory>
{
    public Mp3Descriptor(Mp3Directory directory)
        : base(directory)
    {
    }
}
