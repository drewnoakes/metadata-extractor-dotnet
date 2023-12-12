﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496.Boxes;

internal class FileTypeBox : Box
{
    public uint MajorBrand { get; }
    public uint MinorBrand { get; }
    public IList<uint> CompatibleBrands { get; }

    public string MajorBrandString => TypeStringConverter.ToTypeString(MajorBrand);
    public string MinorBrandString => TypeStringConverter.ToTypeString(MinorBrand);

    public IEnumerable<string> CompatibleBrandStrings =>
        CompatibleBrands.Select(TypeStringConverter.ToTypeString);

    public FileTypeBox(BoxLocation loc, SequentialReader sr) : base(loc)
    {
        MajorBrand = sr.GetUInt32();
        MinorBrand = sr.GetUInt32();
        var cBrands = new List<uint>();
        CompatibleBrands = cBrands;
        while (sr.IsWithinBox(loc))
        {
            cBrands.Add(sr.GetUInt32());
        }
    }
}
