// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public sealed class BmpReader
    {
        public DirectoryList Extract(SequentialReader reader)
        {
            var directories = new List<Directory>();
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

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

        private static void ReadFileHeader(SequentialReader reader, bool allowArray, List<Directory> directories)
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

            Directory? directory = null;
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
                    case (int)BmpHeaderDirectory.BitmapType.OS2BitmapArray:
                        if (!allowArray)
                        {
                            //directories.Add(new ErrorDirectory("Invalid bitmap file - nested arrays not allowed"));
                            AddError("Invalid bitmap file - nested arrays not allowed", directories);
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
                        if (reader.Position > nextHeaderOffset)
                        {
                            AddError("Invalid next header offset", directories);
                            return;
                        }
                        reader.Skip(nextHeaderOffset - reader.Position);
                        ReadFileHeader(reader, true, directories);
                        break;
                    case (int)BmpHeaderDirectory.BitmapType.Bitmap:
                    case (int)BmpHeaderDirectory.BitmapType.OS2Icon:
                    case (int)BmpHeaderDirectory.BitmapType.OS2ColorIcon:
                    case (int)BmpHeaderDirectory.BitmapType.OS2ColorPointer:
                    case (int)BmpHeaderDirectory.BitmapType.OS2Pointer:
                        directory = new BmpHeaderDirectory();
                        directories.Add(directory);
                        directory.Set(BmpHeaderDirectory.TagBitmapType, magicNumber);
                        // skip past the rest of the file header
                        reader.Skip(4 + 2 + 2 + 4);
                        ReadBitmapHeader(reader, (BmpHeaderDirectory)directory, directories);
                        break;
                    default:
                        directories.Add(new ErrorDirectory("Invalid BMP magic number 0x" + magicNumber.ToString("X4")));
                        return;
                }
            }
            catch (IOException)
            {
                if (directory == null)
                    AddError("Unable to read BMP file header", directories);
                else
                    directory.AddError("Unable to read BMP file header");
            }

            return;
        }

        private static void ReadBitmapHeader(SequentialReader reader, BmpHeaderDirectory directory, List<Directory> directories)
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
             *    WORD   NumPlanes        - Number of bit planes (color depth)
             *    WORD   BitsPerPixel     - Number of bits per pixel per plane
             *
             * OS22XBITMAPHEADER (16/64 bytes):
             *
             *    DWORD  Size             - Size of this structure in bytes
             *    DWORD  Width            - Bitmap width in pixels
             *    DWORD  Height           - Bitmap height in pixel
             *    WORD   NumPlanes        - Number of bit planes (color depth)
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
                long headerOffset = reader.Position;
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

                if (headerSize == 12 && bitmapType == (int)BmpHeaderDirectory.BitmapType.Bitmap)
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
                    if (csType == (long)BmpHeaderDirectory.ColorSpaceType.ProfileEmbedded || csType == (long)BmpHeaderDirectory.ColorSpaceType.ProfileLinked)
                    {
                        long profileOffset = reader.GetUInt32();
                        int profileSize = reader.GetInt32();
                        if (reader.Position > headerOffset + profileOffset)
                        {
                            directory.AddError("Invalid profile data offset 0x" + (headerOffset + profileOffset).ToString("X8"));
                            return;
                        }
                        reader.Skip(headerOffset + profileOffset - reader.Position);
                        if (csType == (long)BmpHeaderDirectory.ColorSpaceType.ProfileLinked)
                        {
                            directory.Set(BmpHeaderDirectory.TagLinkedProfile, reader.GetNullTerminatedString(profileSize, Encoding.GetEncoding(1252)));
                        }
                        else
                        {
                            var iccReader = new ByteArrayReader(reader.GetBytes(profileSize));
                            var iccDirectory = new IccReader().Extract(iccReader);
                            iccDirectory.Parent = directory;
                            directories.Add(iccDirectory);
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

        private static void AddError(string errorMessage, List<Directory> directories)
        {
            ErrorDirectory directory = directories.OfType<ErrorDirectory>().FirstOrDefault();
            if (directory == null)
                directories.Add(new ErrorDirectory(errorMessage));
            else
                directory.AddError(errorMessage);
        }
    }
}
