#region License
//
// Copyright 2002-2015 Drew Noakes
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
        [NotNull]
        private const string JpegSegmentPreamble = "Photoshop 3.0";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.AppD;
        }

        public
#if NET35
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
        {
            var preambleLength = JpegSegmentPreamble.Length;
            return segments
                .Where(segment => segment.Length >= preambleLength + 1 && JpegSegmentPreamble == Encoding.UTF8.GetString(segment, 0, preambleLength))
                .SelectMany(segment => Extract(new SequentialByteArrayReader(segment, preambleLength + 1), segment.Length - preambleLength - 1))
                .ToList();
        }

        public
#if NET35
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            Extract([NotNull] SequentialReader reader, int length)
        {
            var directory = new PhotoshopDirectory();

            var directories = new List<Directory> { directory };

            // Data contains a sequence of Image Resource Blocks (IRBs):
            //
            // 4 bytes - Signature "8BIM"
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
                    // 4 bytes for the signature.  Should always be "8BIM".
                    var signature = reader.GetString(4);
                    pos += 4;

                    if (signature != "8BIM")
                        throw new ImageProcessingException("Expecting 8BIM marker");

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

                    switch (tagType)
                    {
                        case PhotoshopDirectory.TagIptc:
                            directories.Add(new IptcReader().Extract(new SequentialByteArrayReader(tagBytes), tagBytes.Length));
                            break;
                        case PhotoshopDirectory.TagIccProfileBytes:
                            directories.Add(new IccReader().Extract(new ByteArrayReader(tagBytes)));
                            break;
                        case PhotoshopDirectory.TagExifData1:
                        case PhotoshopDirectory.TagExifData3:
                            directories.AddRange(new ExifReader().Extract(new ByteArrayReader(tagBytes)));
                            break;
                        case PhotoshopDirectory.TagXmpData:
                            directories.Add(new XmpReader().Extract(tagBytes));
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
