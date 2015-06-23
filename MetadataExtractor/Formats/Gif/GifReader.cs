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

using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GifReader
    {
        private const string Gif87AVersionIdentifier = "87a";

        private const string Gif89AVersionIdentifier = "89a";

        public GifHeaderDirectory Extract([NotNull] SequentialReader reader)
        {
            var directory = new GifHeaderDirectory();

            // FILE HEADER
            //
            // 3 - signature: "GIF"
            // 3 - version: either "87a" or "89a"
            //
            // LOGICAL SCREEN DESCRIPTOR
            //
            // 2 - pixel width
            // 2 - pixel height
            // 1 - screen and color map information flags (0 is LSB)
            //       0-2  Size of the global color table
            //       3    Color table sort flag (89a only)
            //       4-6  Color resolution
            //       7    Global color table flag
            // 1 - background color index
            // 1 - pixel aspect ratio

            reader.IsMotorolaByteOrder = false;

            try
            {
                var signature = reader.GetString(3);
                if (signature != "GIF")
                {
                    directory.AddError("Invalid GIF file signature");
                    return directory;
                }
                var version = reader.GetString(3);
                if (version != Gif87AVersionIdentifier && version != Gif89AVersionIdentifier)
                {
                    directory.AddError("Unexpected GIF version");
                    return directory;
                }
                directory.Set(GifHeaderDirectory.TagGifFormatVersion, version);
                directory.Set(GifHeaderDirectory.TagImageWidth, reader.GetUInt16());
                directory.Set(GifHeaderDirectory.TagImageHeight, reader.GetUInt16());
                var flags = reader.GetByte();
                // First three bits = (BPP - 1)
                var colorTableSize = 1 << ((flags & 7) + 1);
                directory.Set(GifHeaderDirectory.TagColorTableSize, colorTableSize);
                if (version == Gif89AVersionIdentifier)
                {
                    var isColorTableSorted = (flags & 8) != 0;
                    directory.Set(GifHeaderDirectory.TagIsColorTableSorted, isColorTableSorted);
                }
                var bitsPerPixel = ((flags & 0x70) >> 4) + 1;
                directory.Set(GifHeaderDirectory.TagBitsPerPixel, bitsPerPixel);
                var hasGlobalColorTable = (flags & 0xf) != 0;
                directory.Set(GifHeaderDirectory.TagHasGlobalColorTable, hasGlobalColorTable);
                directory.Set(GifHeaderDirectory.TagTransparentColorIndex, reader.GetByte());
                int aspectRatioByte = reader.GetByte();
                if (aspectRatioByte != 0)
                {
                    var pixelAspectRatio = (float)((aspectRatioByte + 15d) / 64d);
                    directory.Set(GifHeaderDirectory.TagPixelAspectRatio, pixelAspectRatio);
                }
            }
            catch (IOException)
            {
                directory.AddError("Unable to read BMP header");
            }

            return directory;
        }
    }
}
