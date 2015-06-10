using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class BmpReader
    {
        public void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            var directory = new BmpHeaderDirectory();
            metadata.AddDirectory(directory);
            // FILE HEADER
            //
            // 2 - magic number (0x42 0x4D = "BM")
            // 4 - size of BMP file in bytes
            // 2 - reserved
            // 2 - reserved
            // 4 - the offset of the pixel array
            //
            // BITMAP INFORMATION HEADER
            //
            // The first four bytes of the header give the size, which is a discriminator of the actual header format.
            // See this for more information http://en.wikipedia.org/wiki/BMP_file_format
            //
            // BITMAPINFOHEADER (size = 40)
            //
            // 4 - size of header
            // 4 - pixel width (signed)
            // 4 - pixel height (signed)
            // 2 - number of colour planes (must be set to 1)
            // 2 - number of bits per pixel
            // 4 - compression being used (needs decoding)
            // 4 - pixel data length (not total file size, just pixel array)
            // 4 - horizontal resolution, pixels/meter (signed)
            // 4 - vertical resolution, pixels/meter (signed)
            // 4 - number of colours in the palette (0 means no palette)
            // 4 - number of important colours (generally ignored)
            //
            // BITMAPCOREHEADER (size = 12)
            //
            // 4 - size of header
            // 2 - pixel width
            // 2 - pixel height
            // 2 - number of colour planes (must be set to 1)
            // 2 - number of bits per pixel
            //
            // COMPRESSION VALUES
            //
            // 0 = None
            // 1 = RLE 8-bit/pixel
            // 2 = RLE 4-bit/pixel
            // 3 = Bit field (or Huffman 1D if BITMAPCOREHEADER2 (size 64))
            // 4 = JPEG (or RLE-24 if BITMAPCOREHEADER2 (size 64))
            // 5 = PNG
            // 6 = Bit field
            reader.IsMotorolaByteOrder = false;
            try
            {
                var magicNumber = reader.GetUInt16();
                if (magicNumber != unchecked(0x4D42))
                {
                    directory.AddError("Invalid BMP magic number");
                    return;
                }
                // skip past the rest of the file header
                reader.Skip(4 + 2 + 2 + 4);
                var headerSize = reader.GetInt32();
                directory.SetInt(BmpHeaderDirectory.TagHeaderSize, headerSize);
                // We expect the header size to be either 40 (BITMAPINFOHEADER) or 12 (BITMAPCOREHEADER)
                if (headerSize == 40)
                {
                    // BITMAPINFOHEADER
                    directory.SetInt(BmpHeaderDirectory.TagImageWidth, reader.GetInt32());
                    directory.SetInt(BmpHeaderDirectory.TagImageHeight, reader.GetInt32());
                    directory.SetInt(BmpHeaderDirectory.TagColourPlanes, reader.GetInt16());
                    directory.SetInt(BmpHeaderDirectory.TagBitsPerPixel, reader.GetInt16());
                    directory.SetInt(BmpHeaderDirectory.TagCompression, reader.GetInt32());
                    // skip the pixel data length
                    reader.Skip(4);
                    directory.SetInt(BmpHeaderDirectory.TagXPixelsPerMeter, reader.GetInt32());
                    directory.SetInt(BmpHeaderDirectory.TagYPixelsPerMeter, reader.GetInt32());
                    directory.SetInt(BmpHeaderDirectory.TagPaletteColourCount, reader.GetInt32());
                    directory.SetInt(BmpHeaderDirectory.TagImportantColourCount, reader.GetInt32());
                }
                else
                {
                    if (headerSize == 12)
                    {
                        directory.SetInt(BmpHeaderDirectory.TagImageWidth, reader.GetInt16());
                        directory.SetInt(BmpHeaderDirectory.TagImageHeight, reader.GetInt16());
                        directory.SetInt(BmpHeaderDirectory.TagColourPlanes, reader.GetInt16());
                        directory.SetInt(BmpHeaderDirectory.TagBitsPerPixel, reader.GetInt16());
                    }
                    else
                    {
                        directory.AddError("Unexpected DIB header size: " + headerSize);
                    }
                }
            }
            catch (IOException)
            {
                directory.AddError("Unable to read BMP header");
            }
        }
    }
}
