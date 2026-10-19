// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Tiff;

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeTiffHandler<T> : ExifTiffHandler
    where T : Directory, new()
{
    public QuickTimeTiffHandler(List<Directory> directories)
        : base(directories, exifStartOffset: 0)
    {
    }

    public override TiffStandard ProcessTiffMarker(ushort marker)
    {
        const ushort StandardTiffMarker = 0x002A;
        const ushort BigTiffMarker = 0x002B;

        var standard = marker switch
        {
            StandardTiffMarker => TiffStandard.Tiff,
            BigTiffMarker => TiffStandard.BigTiff,
            _ => throw new TiffProcessingException($"Unexpected TIFF marker: 0x{marker:X}")
        };

        PushDirectory(new T());

        return standard;
    }
}
