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
using Com.Drew.Imaging;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Pcx
{
    /// <summary>Reads PCX image file metadata.</summary>
    /// <remarks>
    /// Reads PCX image file metadata.
    /// <ul>
    /// <li>https://courses.engr.illinois.edu/ece390/books/labmanual/graphics-pcx.html</li>
    /// <li>http://www.fileformat.info/format/pcx/egff.htm</li>
    /// <li>http://fileformats.archiveteam.org/wiki/PCX</li>
    /// </ul>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PcxReader
    {
        public virtual void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            reader.SetMotorolaByteOrder(false);
            PcxDirectory directory = new PcxDirectory();
            metadata.AddDirectory(directory);
            try
            {
                sbyte identifier = reader.GetInt8();
                if (identifier != unchecked((int)(0x0A)))
                {
                    throw new ImageProcessingException("Invalid PCX identifier byte");
                }
                directory.SetInt(PcxDirectory.TagVersion, reader.GetInt8());
                sbyte encoding = reader.GetInt8();
                if (encoding != unchecked((int)(0x01)))
                {
                    throw new ImageProcessingException("Invalid PCX encoding byte");
                }
                directory.SetInt(PcxDirectory.TagBitsPerPixel, reader.GetUInt8());
                directory.SetInt(PcxDirectory.TagXmin, reader.GetUInt16());
                directory.SetInt(PcxDirectory.TagYmin, reader.GetUInt16());
                directory.SetInt(PcxDirectory.TagXmax, reader.GetUInt16());
                directory.SetInt(PcxDirectory.TagYmax, reader.GetUInt16());
                directory.SetInt(PcxDirectory.TagHorizontalDpi, reader.GetUInt16());
                directory.SetInt(PcxDirectory.TagVerticalDpi, reader.GetUInt16());
                directory.SetByteArray(PcxDirectory.TagPalette, reader.GetBytes(48));
                reader.Skip(1);
                directory.SetInt(PcxDirectory.TagColorPlanes, reader.GetUInt8());
                directory.SetInt(PcxDirectory.TagBytesPerLine, reader.GetUInt16());
                int paletteType = reader.GetUInt16();
                if (paletteType != 0)
                {
                    directory.SetInt(PcxDirectory.TagPaletteType, paletteType);
                }
                int hScrSize = reader.GetUInt16();
                if (hScrSize != 0)
                {
                    directory.SetInt(PcxDirectory.TagHscrSize, hScrSize);
                }
                int vScrSize = reader.GetUInt16();
                if (vScrSize != 0)
                {
                    directory.SetInt(PcxDirectory.TagVscrSize, vScrSize);
                }
            }
            catch (Exception ex)
            {
                directory.AddError("Exception reading PCX file metadata: " + ex.Message);
            }
        }
    }
}
