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
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

using MetadataExtractor.IO;
using MetadataExtractor.Formats.Icc;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class BmpReader
    {
        [NotNull]
        public DirectoryList Extract([NotNull] ReaderInfo reader)
        {
            reader.IsMotorolaByteOrder = false;

            var directories = new List<Directory>();

            // BITMAP INFORMATION HEADER
            //
            // The first four bytes of the header give the size, which is a discriminator of the actual header format.
            // See this for more information http://en.wikipedia.org/wiki/BMP_file_format
            try
            {
                ReadFileHeader(reader, true, directories);
            }
            catch (IOException)
            {
                directories.Add(new ErrorDirectory("IOException processing BMP data"));
            }

            return directories;
        }
        
        private static void ReadFileHeader([NotNull] ReaderInfo reader, bool allowArray, List<Directory> directories)
        {
            /*
             * There are two possible headers a file can start with. If the magic
             * number is OS/2 Bitmap Array (0x4142) the OS/2 Bitmap Array Header
             * will follow. In all other cases the file header will follow, which
             * is identical for Windows and OS/2.
             *
             * File header:
             *
             *    WORD   FileType;      - File type identifier
             *    DWORD  FileSize;      - Size of the file in bytes
             *    WORD   XHotSpot;      - X coordinate of hotspot for pointers
             *    WORD   YHotSpot;      - Y coordinate of hotspot for pointers
             *    DWORD  BitmapOffset;  - Starting position of image data in bytes
             *
             * OS/2 Bitmap Array Header:
             *
             *     WORD  Type;          - Header type, always 4142h ("BA")
             *     DWORD Size;          - Size of this header
             *     DWORD OffsetToNext;  - Offset to next OS2BMPARRAYFILEHEADER
             *     WORD  ScreenWidth;   - Width of the image display in pixels
             *     WORD  ScreenHeight;  - Height of the image display in pixels
             *
             */

            Directory directory = null;
            int magicNumber;
            try
            {
                magicNumber = reader.GetUInt16();
            }
            catch (IOException e)
            {
                directories.Add(new ErrorDirectory("Couldn't determine bitmap type: " + e.ToString()));
                return;
            }

            try
            {
                switch (magicNumber)
                {
                    case (int)BitmapType.OS2BitmapArray:
                        if (!allowArray)
                        {
                            //directories.Add(new ErrorDirectory("Invalid bitmap file - nested arrays not allowed"));
                            addError("Invalid bitmap file - nested arrays not allowed", directories);
                            return;
                        }
                        reader.Skip(4); // Size
                        long nextHeaderOffset = reader.GetUInt32();
                        reader.Skip(2 + 2); // Screen Resolution
                        ReadFileHeader(reader, false, directories);
                        if (nextHeaderOffset == 0)
                        {
                            return; // No more bitmaps
                        }
                        if (reader.LocalPosition > nextHeaderOffset)
                        {
                            addError("Invalid next header offset", directories);
                            return;
                        }
                        reader.Skip(nextHeaderOffset - reader.LocalPosition);
                        ReadFileHeader(reader, true, directories);
                        break;
                    case (int)BitmapType.Bitmap:
                    case (int)BitmapType.OS2Icon:
                    case (int)BitmapType.OS2ColorIcon:
                    case (int)BitmapType.OS2ColorPointer:
                    case (int)BitmapType.OS2Pointer:
                        directory = new BmpHeaderDirectory();
                        directories.Add(directory);
                        directory.Set(BmpHeaderDirectory.TagBitmapType, magicNumber);
                        // skip past the rest of the file header
                        reader.Skip(4 + 2 + 2 + 4);
                        ReadBitmapHeader(reader.Clone(), (BmpHeaderDirectory)directory);
                        break;
                    default:
                        directories.Add(new ErrorDirectory("Invalid BMP magic number 0x" + magicNumber.ToString("X4")));
                        return;
                }
            }
            catch (IOException)
            {
                if (directory == null)
                    addError("Unable to read BMP file header", directories);
                else
                    directory.AddError("Unable to read BMP file header");
            }

            return;
        }

        private static void ReadBitmapHeader([NotNull] ReaderInfo reader, [NotNull] BmpHeaderDirectory directory)
        {
            /*
             * BITMAPCOREHEADER (12 bytes):
             *
             *    DWORD Size              - Size of this header in bytes
             *    SHORT Width             - Image width in pixels
             *    SHORT Height            - Image height in pixels
             *    WORD  Planes            - Number of color planes
             *    WORD  BitsPerPixel      - Number of bits per pixel
             *
             * OS21XBITMAPHEADER (12 bytes):
             *
             *    DWORD  Size             - Size of this structure in bytes
             *    WORD   Width            - Bitmap width in pixels
             *    WORD   Height           - Bitmap height in pixel
             *      WORD   NumPlanes        - Number of bit planes (color depth)
             *    WORD   BitsPerPixel     - Number of bits per pixel per plane
             *
             * OS22XBITMAPHEADER (16/64 bytes):
             *
             *    DWORD  Size             - Size of this structure in bytes
             *    DWORD  Width            - Bitmap width in pixels
             *    DWORD  Height           - Bitmap height in pixel
             *      WORD   NumPlanes        - Number of bit planes (color depth)
             *    WORD   BitsPerPixel     - Number of bits per pixel per plane
             *
             *    - Short version ends here -
             *
             *    DWORD  Compression      - Bitmap compression scheme
             *    DWORD  ImageDataSize    - Size of bitmap data in bytes
             *    DWORD  XResolution      - X resolution of display device
             *    DWORD  YResolution      - Y resolution of display device
             *    DWORD  ColorsUsed       - Number of color table indices used
             *    DWORD  ColorsImportant  - Number of important color indices
             *    WORD   Units            - Type of units used to measure resolution
             *    WORD   Reserved         - Pad structure to 4-byte boundary
             *    WORD   Recording        - Recording algorithm
             *    WORD   Rendering        - Halftoning algorithm used
             *    DWORD  Size1            - Reserved for halftoning algorithm use
             *    DWORD  Size2            - Reserved for halftoning algorithm use
             *    DWORD  ColorEncoding    - Color model used in bitmap
             *    DWORD  Identifier       - Reserved for application use
             *
             * BITMAPINFOHEADER (40 bytes), BITMAPV2INFOHEADER (52 bytes), BITMAPV3INFOHEADER (56 bytes),
             * BITMAPV4HEADER (108 bytes) and BITMAPV5HEADER (124 bytes):
             *
             *    DWORD Size              - Size of this header in bytes
             *    LONG  Width             - Image width in pixels
             *    LONG  Height            - Image height in pixels
             *    WORD  Planes            - Number of color planes
             *    WORD  BitsPerPixel      - Number of bits per pixel
             *    DWORD Compression       - Compression methods used
             *    DWORD SizeOfBitmap      - Size of bitmap in bytes
             *    LONG  HorzResolution    - Horizontal resolution in pixels per meter
             *    LONG  VertResolution    - Vertical resolution in pixels per meter
             *    DWORD ColorsUsed        - Number of colors in the image
             *    DWORD ColorsImportant   - Minimum number of important colors
             *
             *    - BITMAPINFOHEADER ends here -
             *
             *    DWORD RedMask           - Mask identifying bits of red component
             *    DWORD GreenMask         - Mask identifying bits of green component
             *    DWORD BlueMask          - Mask identifying bits of blue component
             *
             *    - BITMAPV2INFOHEADER ends here -
             *
             *    DWORD AlphaMask         - Mask identifying bits of alpha component
             *
             *    - BITMAPV3INFOHEADER ends here -
             *
             *    DWORD CSType            - Color space type
             *    LONG  RedX              - X coordinate of red endpoint
             *    LONG  RedY              - Y coordinate of red endpoint
             *    LONG  RedZ              - Z coordinate of red endpoint
             *    LONG  GreenX            - X coordinate of green endpoint
             *    LONG  GreenY            - Y coordinate of green endpoint
             *    LONG  GreenZ            - Z coordinate of green endpoint
             *    LONG  BlueX             - X coordinate of blue endpoint
             *    LONG  BlueY             - Y coordinate of blue endpoint
             *    LONG  BlueZ             - Z coordinate of blue endpoint
             *    DWORD GammaRed          - Gamma red coordinate scale value
             *    DWORD GammaGreen        - Gamma green coordinate scale value
             *    DWORD GammaBlue         - Gamma blue coordinate scale value
             *
             *    - BITMAPV4HEADER ends here -
             *
             *    DWORD Intent            - Rendering intent for bitmap
             *    DWORD ProfileData       - Offset of the profile data relative to BITMAPV5HEADER
             *    DWORD ProfileSize       - Size, in bytes, of embedded profile data
             *    DWORD Reserved          - Shall be zero
             *
             */

            try
            {
                int bitmapType = directory.GetInt32(BmpHeaderDirectory.TagBitmapType);
                long headerOffset = reader.LocalPosition;
                int headerSize = reader.GetInt32();

                directory.Set(BmpHeaderDirectory.TagHeaderSize, headerSize);

                /*
                 * Known header type sizes:
                 *
                 *  12 - BITMAPCOREHEADER or OS21XBITMAPHEADER
                 *  16 - OS22XBITMAPHEADER (short)
                 *  40 - BITMAPINFOHEADER
                 *  52 - BITMAPV2INFOHEADER
                 *  56 - BITMAPV3INFOHEADER
                 *  64 - OS22XBITMAPHEADER (full)
                 * 108 - BITMAPV4HEADER
                 * 124 - BITMAPV5HEADER
                 *
                 */

                if (headerSize == 12 && bitmapType == (int)BitmapType.Bitmap)
                { //BITMAPCOREHEADER
                  /*
                   * There's no way to tell BITMAPCOREHEADER and OS21XBITMAPHEADER
                   * apart for the "standard" bitmap type. The difference is only
                   * that BITMAPCOREHEADER has signed width and height while
                   * in OS21XBITMAPHEADER they are unsigned. Since BITMAPCOREHEADER,
                   * the Windows version, is most common, read them as signed.
                   */
                    directory.Set(BmpHeaderDirectory.TagImageWidth, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagImageHeight, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagColourPlanes, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagBitsPerPixel, reader.GetUInt16());
                }
                else if (headerSize == 12)
                { // OS21XBITMAPHEADER
                    directory.Set(BmpHeaderDirectory.TagImageWidth, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagImageHeight, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagColourPlanes, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagBitsPerPixel, reader.GetUInt16());
                }
                else if (headerSize == 16 || headerSize == 64)
                { // OS22XBITMAPHEADER
                    directory.Set(BmpHeaderDirectory.TagImageWidth, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagImageHeight, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagColourPlanes, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagBitsPerPixel, reader.GetUInt16());
                    if (headerSize > 16)
                    {
                        directory.Set(BmpHeaderDirectory.TagCompression, reader.GetInt32());
                        reader.Skip(4); // skip the pixel data length
                        directory.Set(BmpHeaderDirectory.TagXPixelsPerMeter, reader.GetInt32());
                        directory.Set(BmpHeaderDirectory.TagYPixelsPerMeter, reader.GetInt32());
                        directory.Set(BmpHeaderDirectory.TagPaletteColourCount, reader.GetInt32());
                        directory.Set(BmpHeaderDirectory.TagImportantColourCount, reader.GetInt32());
                        reader.Skip(
                            2 + // Skip Units, can only be 0 (pixels per meter)
                            2 + // Skip padding
                            2   // Skip Recording, can only be 0 (left to right, bottom to top)
                        );
                        directory.Set(BmpHeaderDirectory.TagRendering, reader.GetUInt16());
                        reader.Skip(4 + 4); // Skip Size1 and Size2
                        directory.Set(BmpHeaderDirectory.TagColorEncoding, reader.GetInt32());
                        reader.Skip(4); // Skip Identifier
                    }
                }
                else if (
                  headerSize == 40 || headerSize == 52 || headerSize == 56 ||
                  headerSize == 108 || headerSize == 124)
                { // BITMAPINFOHEADER V1-5
                    directory.Set(BmpHeaderDirectory.TagImageWidth, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagImageHeight, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagColourPlanes, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagBitsPerPixel, reader.GetUInt16());
                    directory.Set(BmpHeaderDirectory.TagCompression, reader.GetInt32());
                    // skip the pixel data length
                    reader.Skip(4);
                    directory.Set(BmpHeaderDirectory.TagXPixelsPerMeter, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagYPixelsPerMeter, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagPaletteColourCount, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagImportantColourCount, reader.GetInt32());
                    if (headerSize == 40)
                    { // BITMAPINFOHEADER end
                        return;
                    }
                    directory.Set(BmpHeaderDirectory.TagRedMask, reader.GetUInt32());
                    directory.Set(BmpHeaderDirectory.TagGreenMask, reader.GetUInt32());
                    directory.Set(BmpHeaderDirectory.TagBlueMask, reader.GetUInt32());
                    if (headerSize == 52)
                    { // BITMAPV2INFOHEADER end
                        return;
                    }
                    directory.Set(BmpHeaderDirectory.TagAlphaMask, reader.GetUInt32());
                    if (headerSize == 56)
                    { // BITMAPV3INFOHEADER end
                        return;
                    }
                    long csType = reader.GetUInt32();
                    directory.Set(BmpHeaderDirectory.TagColorSpaceType, csType);
                    reader.Skip(36); // Skip color endpoint coordinates
                    directory.Set(BmpHeaderDirectory.TagGammaRed, reader.GetUInt32());
                    directory.Set(BmpHeaderDirectory.TagGammaGreen, reader.GetUInt32());
                    directory.Set(BmpHeaderDirectory.TagGammaBlue, reader.GetUInt32());
                    if (headerSize == 108)
                    { // BITMAPV4HEADER end
                        return;
                    }
                    directory.Set(BmpHeaderDirectory.TagIntent, reader.GetInt32());
                    if (csType == (long)ColorSpaceType.ProfileEmbedded || csType == (long)ColorSpaceType.ProfileLinked)
                    {
                        long profileOffset = reader.GetUInt32();
                        int profileSize = reader.GetInt32();
                        if (reader.LocalPosition > headerOffset + profileOffset)
                        {
                            directory.AddError("Invalid profile data offset 0x" + (headerOffset + profileOffset).ToString("X8"));
                            return;
                        }
                        reader.Skip(headerOffset + profileOffset - reader.LocalPosition);
                        if (csType == (long)ColorSpaceType.ProfileLinked)
                        {
                            directory.Set(BmpHeaderDirectory.TagLinkedProfile, reader.GetNullTerminatedString(profileSize, Encoding.GetEncoding(1252)));
                        }
                        else
                        {
                            ReaderInfo iccReader = reader.Clone(profileSize);
                            new IccReader().Extract(iccReader);
                        }
                    }
                    else
                    {
                        reader.Skip(
                            4 + // Skip ProfileData offset
                            4 + // Skip ProfileSize
                            4   // Skip Reserved
                        );
                    }
                }
                else
                {
                    directory.AddError("Unexpected DIB header size: " + headerSize);
                }
            }
            catch (IOException)
            {
                directory.AddError("Unable to read BMP header");
            }
            catch (MetadataException)
            {
                directory.AddError("Internal error");
            }
        }

        [NotNull]
        public BmpHeaderDirectory ExtractOld([NotNull] ReaderInfo reader)
        {
            var directory = new BmpHeaderDirectory();

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

                if (magicNumber != 0x4D42)
                {
                    directory.AddError("Invalid BMP magic number");
                    return directory;
                }

                // skip past the rest of the file header
                reader.Skip(4 + 2 + 2 + 4);

                var headerSize = reader.GetInt32();
                directory.Set(BmpHeaderDirectory.TagHeaderSize, headerSize);

                // We expect the header size to be either 40 (BITMAPINFOHEADER) or 12 (BITMAPCOREHEADER)
                if (headerSize == 40)
                {
                    // BITMAPINFOHEADER
                    directory.Set(BmpHeaderDirectory.TagImageWidth, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagImageHeight, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagColourPlanes, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagBitsPerPixel, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagCompression, reader.GetInt32());
                    // skip the pixel data length
                    reader.Skip(4);
                    directory.Set(BmpHeaderDirectory.TagXPixelsPerMeter, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagYPixelsPerMeter, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagPaletteColourCount, reader.GetInt32());
                    directory.Set(BmpHeaderDirectory.TagImportantColourCount, reader.GetInt32());
                }
                else if (headerSize == 12)
                {
                    directory.Set(BmpHeaderDirectory.TagImageWidth, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagImageHeight, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagColourPlanes, reader.GetInt16());
                    directory.Set(BmpHeaderDirectory.TagBitsPerPixel, reader.GetInt16());
                }
                else
                {
                    directory.AddError("Unexpected DIB header size: " + headerSize);
                }
            }
            catch (IOException)
            {
                directory.AddError("Unable to read BMP header");
            }

            return directory;
        }

        private static void addError([NotNull] string errorMessage, [NotNull] List<Directory> directories)
        {
            ErrorDirectory directory = directories.OfType<ErrorDirectory>().FirstOrDefault();
            if (directory == null)
                directories.Add(new ErrorDirectory(errorMessage));
            else
                directory.AddError(errorMessage);
        }
    }

}
