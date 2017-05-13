#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

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
    public sealed class PhotoshopReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "Photoshop 3.0";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.AppD };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            var preambleLength = JpegSegmentPreamble.Length;
            return segments
                .Where(segment => segment.Bytes.Length >= preambleLength + 1 && JpegSegmentPreamble == Encoding.UTF8.GetString(segment.Bytes, 0, preambleLength))
                .SelectMany(segment => Extract(new SequentialByteArrayReader(segment.Bytes, preambleLength + 1), segment.Bytes.Length - preambleLength - 1))
                .ToList();
        }

        [NotNull]
        public DirectoryList Extract([NotNull] SequentialReader reader, int length)
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

                    // We don't use the string value here
                    reader.Skip(descriptionLength);
                    pos += descriptionLength;

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
                            var exifDirectories = new ExifReader().Extract(new ByteArrayReader(tagBytes));
                            foreach (var exifDirectory in exifDirectories.Where(d => d.Parent == null))
                                exifDirectory.Parent = directory;
                            directories.AddRange(exifDirectories);
                            break;
                        case PhotoshopDirectory.TagXmpData:
                            var xmpDirectory = new XmpReader().Extract(tagBytes);
                            xmpDirectory.Parent = directory;
                            directories.Add(xmpDirectory);
                            break;
                        default:
                            directory.Set(tagType, tagBytes);
                            break;
                    }

                    if (tagType >= 0x0fa0 && tagType <= 0x1387)
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
