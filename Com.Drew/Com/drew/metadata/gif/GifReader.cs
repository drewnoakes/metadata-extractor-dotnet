using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GifReader
    {
        private const string Gif87AVersionIdentifier = "87a";

        private const string Gif89AVersionIdentifier = "89a";

        public void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            GifHeaderDirectory directory = new GifHeaderDirectory();
            metadata.AddDirectory(directory);
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
            reader.SetMotorolaByteOrder(false);
            try
            {
                string signature = reader.GetString(3);
                if (!signature.Equals("GIF"))
                {
                    directory.AddError("Invalid GIF file signature");
                    return;
                }
                string version = reader.GetString(3);
                if (!version.Equals(Gif87AVersionIdentifier) && !version.Equals(Gif89AVersionIdentifier))
                {
                    directory.AddError("Unexpected GIF version");
                    return;
                }
                directory.SetString(GifHeaderDirectory.TagGifFormatVersion, version);
                directory.SetInt(GifHeaderDirectory.TagImageWidth, reader.GetUInt16());
                directory.SetInt(GifHeaderDirectory.TagImageHeight, reader.GetUInt16());
                short flags = reader.GetUInt8();
                // First three bits = (BPP - 1)
                int colorTableSize = 1 << ((flags & 7) + 1);
                directory.SetInt(GifHeaderDirectory.TagColorTableSize, colorTableSize);
                if (version.Equals(Gif89AVersionIdentifier))
                {
                    bool isColorTableSorted = (flags & 8) != 0;
                    directory.SetBoolean(GifHeaderDirectory.TagIsColorTableSorted, isColorTableSorted);
                }
                int bitsPerPixel = ((flags & unchecked((int)(0x70))) >> 4) + 1;
                directory.SetInt(GifHeaderDirectory.TagBitsPerPixel, bitsPerPixel);
                bool hasGlobalColorTable = (flags & unchecked((int)(0xf))) != 0;
                directory.SetBoolean(GifHeaderDirectory.TagHasGlobalColorTable, hasGlobalColorTable);
                directory.SetInt(GifHeaderDirectory.TagTransparentColorIndex, reader.GetUInt8());
                int aspectRatioByte = reader.GetUInt8();
                if (aspectRatioByte != 0)
                {
                    float pixelAspectRatio = (float)((aspectRatioByte + 15d) / 64d);
                    directory.SetFloat(GifHeaderDirectory.TagPixelAspectRatio, pixelAspectRatio);
                }
            }
            catch (IOException)
            {
                directory.AddError("Unable to read BMP header");
            }
        }
    }
}
