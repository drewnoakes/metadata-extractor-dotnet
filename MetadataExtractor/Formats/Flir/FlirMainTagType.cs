// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Flir
{
    internal enum FlirMainTagType : ushort
    {
        // General

        Unused = 0,
        Pixels = 1,
        GainMap = 2,
        OffsMap = 3,
        DeadMap = 4,
        GainDeadMap = 5,
        CoarseMap = 6,
        ImageMap = 7,

        // FLIR matrix tags

        BasicData = 0x20,
        Measure,
        ColorPal
    }
}
