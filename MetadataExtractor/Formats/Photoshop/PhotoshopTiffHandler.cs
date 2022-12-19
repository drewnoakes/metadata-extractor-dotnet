// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class PhotoshopTiffHandler : ExifTiffHandler
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
        //private const int TagExifGps = 0x8825;
        //private const int TagTImageSourceData = 0x935C;
        //private const int TagTAnnotations = 0xC44F;

        public PhotoshopTiffHandler(List<Directory> directories)
            : base(directories)
        {
        }

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
                    Directories.Add(new IccReader().Extract(new ByteArrayReader(context.Reader.GetBytes(valueOffset, byteCount))));
                    return true;
            }

            return base.CustomProcessTag(context, tagId, valueOffset, byteCount);
        }
    }
}
