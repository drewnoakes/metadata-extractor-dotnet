// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Reads metadata stored within PSD file format data.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PsdReader
    {
        public IReadOnlyList<Directory> Extract(SequentialReader reader)
        {
            var directory = new PsdHeaderDirectory();

            // FILE HEADER SECTION

            try
            {
                var signature = reader.GetInt32();
                var version = reader.GetUInt16();

                if (signature != 0x38425053)
                {
                    // "8BPS"
                    directory.AddError("Invalid PSD file signature");
                }
                else if (version != 1 && version != 2)
                {
                    directory.AddError("Invalid PSD file version (must be 1 or 2)");
                }
                else
                {
                    // 6 reserved bytes are skipped here.  They should be zero.
                    reader.Skip(6);
                    var channelCount = reader.GetUInt16();
                    directory.Set(PsdHeaderDirectory.TagChannelCount, channelCount);
                    // even though this is probably an unsigned int, the max height in practice is 300,000
                    var imageHeight = reader.GetInt32();
                    directory.Set(PsdHeaderDirectory.TagImageHeight, imageHeight);
                    // even though this is probably an unsigned int, the max width in practice is 300,000
                    var imageWidth = reader.GetInt32();
                    directory.Set(PsdHeaderDirectory.TagImageWidth, imageWidth);
                    var bitsPerChannel = reader.GetUInt16();
                    directory.Set(PsdHeaderDirectory.TagBitsPerChannel, bitsPerChannel);
                    var colorMode = reader.GetUInt16();
                    directory.Set(PsdHeaderDirectory.TagColorMode, colorMode);
                }
            }
            catch (IOException)
            {
                directory.AddError("Unable to read PSD header");
            }

            if (directory.HasError)
                return new Directory[] { directory };

            IEnumerable<Directory>? photoshopDirectories = null;

            try
            {
                // COLOR MODE DATA SECTION

                // Only indexed color and duotone (see the mode field in the File header section) have color mode data.
                // For all other modes, this section is just the 4-byte length field, which is set to zero.
                //
                // Indexed color images: length is 768; color data contains the color table for the image,
                //                       in non-interleaved order.
                // Duotone images: color data contains the duotone specification (the format of which is not documented).
                //                 Other applications that read Photoshop files can treat a duotone image as a gray    image,
                //                 and just preserve the contents of the duotone information when reading and writing the
                //                 file.
                var colorModeSectionLength = reader.GetUInt32();
                reader.Skip(colorModeSectionLength);

                // IMAGE RESOURCES SECTION

                var imageResourcesSectionLength = reader.GetUInt32();
                if (imageResourcesSectionLength > int.MaxValue)
                    throw new IOException("Invalid resource section length.");
                photoshopDirectories = new PhotoshopReader().Extract(reader, (int)imageResourcesSectionLength);
            }
            catch (IOException)
            {
                directory.AddError("Unable to read PSD image resources");
            }

            var directories = new List<Directory> { directory };

            if (photoshopDirectories != null)
                directories.AddRange(photoshopDirectories);

            // LAYER AND MASK INFORMATION SECTION (skipped)

            // IMAGE DATA SECTION (skipped)

            return directories;
        }
    }
}
