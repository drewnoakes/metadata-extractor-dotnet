/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Diagnostics;
using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Photoshop
{
    /// <summary>Reads metadata stored within PSD file format data.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PsdReader
    {
        public virtual void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            PsdHeaderDirectory directory = new PsdHeaderDirectory();
            metadata.AddDirectory(directory);
            // FILE HEADER SECTION
            try
            {
                int signature = reader.GetInt32();
                if (signature != unchecked((int)(0x38425053)))
                {
                    // "8BPS"
                    directory.AddError("Invalid PSD file signature");
                    return;
                }
                int version = reader.GetUInt16();
                if (version != 1 && version != 2)
                {
                    directory.AddError("Invalid PSD file version (must be 1 or 2)");
                    return;
                }
                // 6 reserved bytes are skipped here.  They should be zero.
                reader.Skip(6);
                int channelCount = reader.GetUInt16();
                directory.SetInt(PsdHeaderDirectory.TagChannelCount, channelCount);
                // even though this is probably an unsigned int, the max height in practice is 300,000
                int imageHeight = reader.GetInt32();
                directory.SetInt(PsdHeaderDirectory.TagImageHeight, imageHeight);
                // even though this is probably an unsigned int, the max width in practice is 300,000
                int imageWidth = reader.GetInt32();
                directory.SetInt(PsdHeaderDirectory.TagImageWidth, imageWidth);
                int bitsPerChannel = reader.GetUInt16();
                directory.SetInt(PsdHeaderDirectory.TagBitsPerChannel, bitsPerChannel);
                int colorMode = reader.GetUInt16();
                directory.SetInt(PsdHeaderDirectory.TagColorMode, colorMode);
            }
            catch (IOException)
            {
                directory.AddError("Unable to read PSD header");
                return;
            }
            // COLOR MODE DATA SECTION
            try
            {
                long sectionLength = reader.GetUInt32();
            /*
             * Only indexed color and duotone (see the mode field in the File header section) have color mode data.
             * For all other modes, this section is just the 4-byte length field, which is set to zero.
             *
             * Indexed color images: length is 768; color data contains the color table for the image,
             *                       in non-interleaved order.
             * Duotone images: color data contains the duotone specification (the format of which is not documented).
             *                 Other applications that read Photoshop files can treat a duotone image as a gray    image,
             *                 and just preserve the contents of the duotone information when reading and writing the
             *                 file.
             */
                reader.Skip(sectionLength);
            }
            catch (IOException)
            {
                return;
            }
            // IMAGE RESOURCES SECTION
            try
            {
                long sectionLength = reader.GetUInt32();
                Debug.Assert((sectionLength <= int.MaxValue));
                new PhotoshopReader().Extract(reader, (int)sectionLength, metadata);
            }
            catch (IOException)
            {
            }
        }
        // ignore
        // LAYER AND MASK INFORMATION SECTION (skipped)
        // IMAGE DATA SECTION (skipped)
    }
}
