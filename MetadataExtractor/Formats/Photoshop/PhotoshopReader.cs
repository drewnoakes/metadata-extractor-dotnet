// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Reads metadata created by Photoshop and stored in the APPD segment of JPEG files.</summary>
    /// <remarks>
    /// Reads metadata created by Photoshop and stored in the APPD segment of JPEG files.
    /// Note that IPTC data may be stored within this segment, in which case this reader will
    /// create both a <see cref="PhotoshopDirectory"/> and a <see cref="IptcDirectory"/>.
    /// </remarks>
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PhotoshopReader : JpegSegmentWithPreambleMetadataReader
    {
        public static ReadOnlySpan<byte> JpegSegmentPreamble => "Photoshop 3.0"u8;

        protected override ReadOnlySpan<byte> PreambleBytes => JpegSegmentPreamble;

        public override IReadOnlyCollection<JpegSegmentType> SegmentTypes { get; } = [JpegSegmentType.AppD];

        protected override IEnumerable<Directory> Extract(byte[] segmentBytes, int preambleLength)
        {
            if (segmentBytes.Length >= preambleLength + 1)
            {
                return Extract(
                    reader: new SequentialByteArrayReader(segmentBytes, preambleLength + 1),
                    length: segmentBytes.Length - preambleLength - 1);
            }

            return [];
        }

        public IReadOnlyList<Directory> Extract(SequentialReader reader, int length)
        {
            var directory = new PhotoshopDirectory();

            var directories = new List<Directory> { directory };

            // Data contains a sequence of Image Resource Blocks (IRBs):
            //
            // 4 bytes - Signature; mostly "8BIM" but "PHUT", "AgHg" and "DCSR" are also found
            // 2 bytes - Resource identifier
            // String  - Pascal string, padded to make length even
            // 4 bytes - Size of resource data which follows
            // Data    - The resource data, padded to make size even
            //
            // http://www.adobe.com/devnet-apps/photoshop/fileformatashtml/#50577409_pgfId-1037504
            var pos = 0;
            int clippingPathCount = 0;
            while (pos < length)
            {
                try
                {
                    // 4 bytes for the signature ("8BIM", "PHUT", etc.)
                    var signature = reader.GetString(4, Encoding.UTF8);
                    pos += 4;

                    // 2 bytes for the resource identifier (tag type).
                    var tagType = reader.GetUInt16();
                    pos += 2;

                    // A variable number of bytes holding a pascal string (two leading bytes for length).
                    var descriptionLength = reader.GetByte();
                    pos += 1;

                    // Some basic bounds checking
                    if (descriptionLength + pos > length)
                        throw new ImageProcessingException("Invalid string length");

                    // Get name (important for paths)
                    var description = new StringBuilder();
                    // Loop through each byte and append to string
                    while (descriptionLength > 0)
                    {
                        description.Append((char)reader.GetByte());
                        pos++;
                        descriptionLength--;
                    }

                    // The number of bytes is padded with a trailing zero, if needed, to make the size even.
                    if (pos % 2 != 0)
                    {
                        reader.Skip(1);
                        pos++;
                    }

                    // 4 bytes for the size of the resource data that follows.
                    var byteCount = reader.GetInt32();
                    pos += 4;

                    // The resource data.
                    var tagBytes = reader.GetBytes(byteCount);
                    pos += byteCount;

                    // The number of bytes is padded with a trailing zero, if needed, to make the size even.
                    if (pos % 2 != 0)
                    {
                        reader.Skip(1);
                        pos++;
                    }

                    // Skip any unsupported IRBs
                    if (signature != "8BIM")
                        continue;

                    switch (tagType)
                    {
                        case PhotoshopDirectory.TagIptc:
                            var iptcDirectory = new IptcReader().Extract(new SequentialByteArrayReader(tagBytes), tagBytes.Length);
                            iptcDirectory.Parent = directory;
                            directories.Add(iptcDirectory);
                            break;
                        case PhotoshopDirectory.TagIccProfileBytes:
                            var iccDirectory = new IccReader().Extract(new ByteArrayReader(tagBytes));
                            iccDirectory.Parent = directory;
                            directories.Add(iccDirectory);
                            break;
                        case PhotoshopDirectory.TagExifData1:
                        case PhotoshopDirectory.TagExifData3:
                            var exifDirectories = new ExifReader().Extract(new ByteArrayReader(tagBytes), exifStartOffset: 0);
                            foreach (var exifDirectory in exifDirectories.Where(d => d.Parent is null))
                                exifDirectory.Parent = directory;
                            directories.AddRange(exifDirectories);
                            break;
                        case PhotoshopDirectory.TagXmpData:
                            var xmpDirectory = new XmpReader().Extract(tagBytes);
                            xmpDirectory.Parent = directory;
                            directories.Add(xmpDirectory);
                            break;
                        default:
                            if (tagType is >= PhotoshopDirectory.TagClippingPathBlockStart and <= PhotoshopDirectory.TagClippingPathBlockEnd)
                            {
                                clippingPathCount++;
                                Array.Resize(ref tagBytes, tagBytes.Length + description.Length + 1);
                                // Append description(name) to end of byte array with 1 byte before the description representing the length
                                for (int i = tagBytes.Length - description.Length - 1; i < tagBytes.Length; i++)
                                {
                                    if (i % (tagBytes.Length - description.Length - 1 + description.Length) == 0)
                                        tagBytes[i] = (byte)description.Length;
                                    else
                                        tagBytes[i] = (byte)description[i - (tagBytes.Length - description.Length - 1)];
                                }
                                PhotoshopDirectory.TagNameMap[PhotoshopDirectory.TagClippingPathBlockStart + clippingPathCount - 1] = "Path Info " + clippingPathCount;
                                directory.Set(PhotoshopDirectory.TagClippingPathBlockStart + clippingPathCount - 1, tagBytes);
                            }
                            else
                                directory.Set(tagType, tagBytes);
                            break;
                    }

                    if (tagType is >= 0x0fa0 and <= 0x1387)
                        PhotoshopDirectory.TagNameMap[tagType] = $"Plug-in {tagType - 0x0fa0 + 1} Data";
                }
                catch (Exception ex)
                {
                    directory.AddError(ex.Message);
                    break;
                }
            }

            return directories;
        }
    }
}
