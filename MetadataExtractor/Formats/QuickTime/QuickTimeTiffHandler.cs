// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Tiff;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.QuickTime
{
    public sealed class QuickTimeTiffHandler<T> : ExifTiffHandler
        where T : Directory, new()
    {
        public QuickTimeTiffHandler([NotNull] List<Directory> directories)
            : base(directories)
        {
        }

        public override void SetTiffMarker(int marker)
        {
            const int standardTiffMarker = 0x002A;
            if (marker != standardTiffMarker)
            {
                throw new TiffProcessingException($"Unexpected TIFF marker: 0x{marker:X}");
            }
            PushDirectory(new T());
        }
    }
}
