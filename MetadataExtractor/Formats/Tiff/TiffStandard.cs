// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tiff
{
    public enum TiffStandard
    {
        /// <summary>
        /// Regular TIFF.
        /// </summary>
        Tiff,

        /// <summary>
        /// The "BigTIFF" standard, which supports greater than 4GB files, more entries
        /// in IFDs, and larger tag value arrays.
        /// </summary>
        BigTiff
    }
}
