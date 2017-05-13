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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Reads metadata stored within PSD file format data.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PsdReader
    {
        [NotNull]
        public DirectoryList Extract([NotNull] SequentialReader reader)
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

            IEnumerable<Directory> photoshopDirectories = null;

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
                Debug.Assert(imageResourcesSectionLength <= int.MaxValue);
                photoshopDirectories = new PhotoshopReader().Extract(reader, (int)imageResourcesSectionLength);
            }
            catch (IOException)
            {
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
