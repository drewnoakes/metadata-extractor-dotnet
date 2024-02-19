// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class PhotoshopTiffHandler(List<Directory> directories)
        : ExifTiffHandler(directories, exifStartOffset: 0)
    {
        // Photoshop-specific Tiff Tags
        // http://www.adobe.com/devnet-apps/photoshop/fileformatashtml/#50577413_pgfId-1039502
        //private const int TagPageMakerExtension = 0x014A;
        //private const int TagJpegTables = 0X01B5;
        private const int TagXmp = 0x02BC;
        //private const int TagFileInformation = 0x83BB;
        private const int TagPhotoshopImageResources = 0x8649;
        //private const int TagExifIfdPointer = 0x8769;
        private const int TagIccProfiles = 0x8773;

        public override bool CustomProcessTag(in TiffReaderContext context, int tagId, int valueOffset, int byteCount)
        {
            switch (tagId)
            {
                case TagXmp:
                    Directories.Add(new XmpReader().Extract(context.Reader.GetBytes(valueOffset, byteCount)));
                    return true;
                case TagPhotoshopImageResources:
                    Directories.AddRange(new PhotoshopReader().Extract(new SequentialByteArrayReader(context.Reader.GetBytes(valueOffset, byteCount)), byteCount));
                    return true;
                case TagIccProfiles:
                {
                    using var bufferScope = new ScopedBuffer(byteCount);
                    context.Reader.GetBytes(valueOffset, bufferScope.Span);
                    Directories.Add(new IccReader().Extract(bufferScope.Span));
                    return true;
                }
            }

            return base.CustomProcessTag(context, tagId, valueOffset, byteCount);
        }
    }
}
