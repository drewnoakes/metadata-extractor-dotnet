// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Tiff;
using System.Collections.Generic;
using System.Linq;

namespace MetadataExtractor.Formats.QuickTime
{
    public sealed class QuickTimeTiffHandler<T, TParent> : ExifTiffHandler
        where T : Directory, new()
        where TParent : Directory
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
            var directory = new T
            {
                Parent = Directories.OfType<TParent>().FirstOrDefault()
            };
            PushDirectory(directory);
        }
    }
}
