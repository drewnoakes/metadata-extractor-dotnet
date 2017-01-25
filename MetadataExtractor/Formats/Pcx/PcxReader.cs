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
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Pcx
{
    /// <summary>Reads PCX image file metadata.</summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>https://courses.engr.illinois.edu/ece390/books/labmanual/graphics-pcx.html</item>
    /// <item>http://www.fileformat.info/format/pcx/egff.htm</item>
    /// <item>http://fileformats.archiveteam.org/wiki/PCX</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PcxReader
    {
        [NotNull]
        public PcxDirectory Extract([NotNull] SequentialReader reader)
        {
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            var directory = new PcxDirectory();

            try
            {
                var identifier = reader.GetSByte();

                if (identifier != 0x0A)
                    throw new ImageProcessingException("Invalid PCX identifier byte");

                directory.Set(PcxDirectory.TagVersion, reader.GetSByte());

                var encoding = reader.GetSByte();
                if (encoding != 0x01)
                    throw new ImageProcessingException("Invalid PCX encoding byte");

                directory.Set(PcxDirectory.TagBitsPerPixel, reader.GetByte());
                directory.Set(PcxDirectory.TagXMin, reader.GetUInt16());
                directory.Set(PcxDirectory.TagYMin, reader.GetUInt16());
                directory.Set(PcxDirectory.TagXMax, reader.GetUInt16());
                directory.Set(PcxDirectory.TagYMax, reader.GetUInt16());
                directory.Set(PcxDirectory.TagHorizontalDpi, reader.GetUInt16());
                directory.Set(PcxDirectory.TagVerticalDpi, reader.GetUInt16());
                directory.Set(PcxDirectory.TagPalette, reader.GetBytes(48));
                reader.Skip(1);
                directory.Set(PcxDirectory.TagColorPlanes, reader.GetByte());
                directory.Set(PcxDirectory.TagBytesPerLine, reader.GetUInt16());

                var paletteType = reader.GetUInt16();
                if (paletteType != 0)
                    directory.Set(PcxDirectory.TagPaletteType, paletteType);

                var hScrSize = reader.GetUInt16();
                if (hScrSize != 0)
                    directory.Set(PcxDirectory.TagHScrSize, hScrSize);

                var vScrSize = reader.GetUInt16();
                if (vScrSize != 0)
                    directory.Set(PcxDirectory.TagVScrSize, vScrSize);
            }
            catch (Exception ex)
            {
                directory.AddError("Exception reading PCX file metadata: " + ex.Message);
            }

            return directory;
        }
    }
}
