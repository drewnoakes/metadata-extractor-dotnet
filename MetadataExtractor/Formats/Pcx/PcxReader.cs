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

using System;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Pcx
{
    /// <summary>Reads PCX image file metadata.</summary>
    /// <remarks>
    /// Reads PCX image file metadata.
    /// <list type="bullet">
    /// <item>https://courses.engr.illinois.edu/ece390/books/labmanual/graphics-pcx.html</item>
    /// <item>http://www.fileformat.info/format/pcx/egff.htm</item>
    /// <item>http://fileformats.archiveteam.org/wiki/PCX</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PcxReader
    {
        public void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            reader.IsMotorolaByteOrder = false;
            var directory = new PcxDirectory();
            metadata.AddDirectory(directory);
            try
            {
                var identifier = reader.GetInt8();
                if (identifier != 0x0A)
                {
                    throw new ImageProcessingException("Invalid PCX identifier byte");
                }
                directory.Set(PcxDirectory.TagVersion, reader.GetInt8());
                var encoding = reader.GetInt8();
                if (encoding != 0x01)
                {
                    throw new ImageProcessingException("Invalid PCX encoding byte");
                }
                directory.Set(PcxDirectory.TagBitsPerPixel, reader.GetUInt8());
                directory.Set(PcxDirectory.TagXmin, reader.GetUInt16());
                directory.Set(PcxDirectory.TagYmin, reader.GetUInt16());
                directory.Set(PcxDirectory.TagXmax, reader.GetUInt16());
                directory.Set(PcxDirectory.TagYmax, reader.GetUInt16());
                directory.Set(PcxDirectory.TagHorizontalDpi, reader.GetUInt16());
                directory.Set(PcxDirectory.TagVerticalDpi, reader.GetUInt16());
                directory.Set(PcxDirectory.TagPalette, reader.GetBytes(48));
                reader.Skip(1);
                directory.Set(PcxDirectory.TagColorPlanes, reader.GetUInt8());
                directory.Set(PcxDirectory.TagBytesPerLine, reader.GetUInt16());
                var paletteType = reader.GetUInt16();
                if (paletteType != 0)
                {
                    directory.Set(PcxDirectory.TagPaletteType, paletteType);
                }
                var hScrSize = reader.GetUInt16();
                if (hScrSize != 0)
                {
                    directory.Set(PcxDirectory.TagHscrSize, hScrSize);
                }
                var vScrSize = reader.GetUInt16();
                if (vScrSize != 0)
                {
                    directory.Set(PcxDirectory.TagVscrSize, vScrSize);
                }
            }
            catch (Exception ex)
            {
                directory.AddError("Exception reading PCX file metadata: " + ex.Message);
            }
        }
    }
}
