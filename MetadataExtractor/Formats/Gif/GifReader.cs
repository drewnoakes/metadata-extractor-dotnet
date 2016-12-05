#region License
//
// Copyright 2002-2016 Drew Noakes
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
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class GifReader
    {
        private const string Gif87AVersionIdentifier = "87a";

        private const string Gif89AVersionIdentifier = "89a";

        [NotNull]
        public
#if NET35
        IList<Directory>
#else
        IReadOnlyList<Directory>
#endif
            Extract([NotNull] SequentialReader reader)
        {
            var directory = new GifHeaderDirectory();
            var directories = new List<Directory> { directory };

            // FILE HEADER
            //
            // 3 - signature: "GIF"
            // 3 - version: either "87a" or "89a"
            //
            // LOGICAL SCREEN DESCRIPTOR
            //
            // 2 - pixel width
            // 2 - pixel height
            // 1 - screen and color map information flags (0 is MSB)
            //       0    Global color table flag
            //       1-3  Color resolution
            //       4    Color table sort flag (89a only)
            //       5-7  Size of the global color table

            // 1 - background color index
            // 1 - pixel aspect ratio

            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            try
            {
                var signature = reader.GetString(3, Encoding.UTF8);
                if (signature != "GIF")
                {
                    directory.AddError("Invalid GIF file signature");
                    return directories;
                }
                var version = reader.GetString(3, Encoding.UTF8);
                if (version != Gif87AVersionIdentifier && version != Gif89AVersionIdentifier)
                {
                    directory.AddError("Unexpected GIF version");
                    return directories;
                }
                directory.Set(GifHeaderDirectory.TagGifFormatVersion, version);
                directory.Set(GifHeaderDirectory.TagImageWidth, reader.GetUInt16());
                directory.Set(GifHeaderDirectory.TagImageHeight, reader.GetUInt16());

                var flags = reader.GetByte();

                var hasGlobalColorTable = (flags & 0xf) != 0;
                directory.Set(GifHeaderDirectory.Screen.TagHasGlobalColorTable, hasGlobalColorTable);
                var colorResolutionDepth = ((flags & 0x70) >> 4) + 1;
                directory.Set(GifHeaderDirectory.Screen.TagColorResolutionDepth, colorResolutionDepth);
                var bitsPerPixel = (flags & 0x07) + 1;
                directory.Set(GifHeaderDirectory.Screen.TagBitsPerPixel, bitsPerPixel);
                if (version == Gif89AVersionIdentifier)
                {
                    var isColorTableSorted = (flags & 8) != 0;
                    directory.Set(GifHeaderDirectory.Screen.TagIsColorTableSorted, isColorTableSorted);
                }

                var colorTableSize = 2 << (flags & 0x07);
                directory.Set(GifHeaderDirectory.Screen.TagColorTableSize, colorTableSize);
                var colorTableLength = 3 * (2 << (flags & 0x07));
                directory.Set(GifHeaderDirectory.Screen.TagColorTableLength, colorTableLength);
                directory.Set(GifHeaderDirectory.Screen.TagBackgroundColorIndex, reader.GetByte());
                int aspectRatioByte = reader.GetByte();
                if (aspectRatioByte != 0)
                {
                    var pixelAspectRatio = (float)((aspectRatioByte + 15d) / 64d);
                    directory.Set(GifHeaderDirectory.Screen.TagPixelAspectRatio, pixelAspectRatio);
                }

                // don't (currently) process the color table so skip over those bytes
                if (colorTableLength > 0)
                    reader.Skip(colorTableLength);

                // Process any descriptors or extensions
                ProcessBlocks(reader, directory, directories);
            }
            catch (IOException)
            {
                directory.AddError("Unable to read GIF header");
            }

            return directories;
        }

        /// <summary>Processes GIF extension blocks. Attempts to read as much as possible when invalid blocks are encountered.</summary>
        /// <remarks>
        /// Some parts converted from Exiftool version 10.33 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\GIF.pm
        ///
        /// More details on processing GIF blocks here:
        /// <list type="bullet">
        ///   <item><a href="http://giflib.sourceforge.net/whatsinagif/bits_and_bytes.html">http://giflib.sourceforge.net/whatsinagif/bits_and_bytes.html</a></item>
        ///   <item><a href="http://giflib.sourceforge.net/whatsinagif/animation_and_transparency.html">http://giflib.sourceforge.net/whatsinagif/animation_and_transparency.html</a></item>
        ///   <item><a href="http://wwwimages.adobe.com/content/dam/Adobe/en/devnet/xmp/pdfs/XMPSpecificationPart3.pdf">http://wwwimages.adobe.com/content/dam/Adobe/en/devnet/xmp/pdfs/XMPSpecificationPart3.pdf</a></item>
        /// </list>
        /// </remarks>
        private void ProcessBlocks(SequentialReader reader, GifHeaderDirectory header, List<Directory> directories)
        {
            byte length = 0x0;
            byte nextByte = 0x0;

            int framecount = 0;
            uint delayTotal = 0;

            // The trailer block (0x3B) indicates when you've reached the end of the file.
            // However, when files aren't formatted correctly, reaching (and knowing if it is) the trailer is unreliable.
            // A forever-while loop is used instead to be more forgiving with bad data.
            while (true)
            {
                try { nextByte = reader.GetByte(); }
                catch (IOException) { break; }

                if (nextByte == 0x2c)   // Image Descriptor
                {
                    framecount++;

                    try
                    {
                        reader.GetBytes(8); // left, top, width, height
                        nextByte = reader.GetByte();
                        //if ((length & 0x80) == 0x80)    // does color table exist?
                        if ((nextByte & 0xf) != 0)    // does color table exist?
                        {
                            // skip the color table
                            reader.Skip(3 * (2 << (nextByte & 0x07)));
                        }

                        // skip "LZW Minimum Code Size" byte
                        reader.GetByte();

                        // skip image blocks
                        while (true)
                        {
                            try
                            {
                                nextByte = reader.GetByte();
                                if (nextByte == 0)
                                    break;
                                reader.Skip(nextByte);  // nextByte is a 'length'
                            }
                            catch (IOException)
                            {
                                // probably a bad block and will usually lead to the end of the file
                                header.AddError("GIF format exception - skipping invalid Image Descriptor block");
                                break;  // break out of this while loop, but not the main reader loop
                            }
                        }
                    }
                    catch (IOException)
                    {
                        // almost certainly a bad block that lead to the end of the file. Stop processing...
                        header.AddError("GIF format exception - invalid Image Descriptor block");
                        break;
                    }
                    // continue with next field
                }
                else if (nextByte == 0x21)   // extension introducer
                {
                    byte[] extBlockTypeSize;

                    // get extension block type/size
                    try
                    {
                        extBlockTypeSize = reader.GetBytes(2);
                    }
                    catch(IOException)
                    {
                        header.AddError("GIF format exception - unable to read extension block type/size");
                        break;
                    }

                    var extmarker = extBlockTypeSize[0];
                    var extsize = extBlockTypeSize[1];

                    if (extmarker == 0xfe) // comment extension
                    {
                        var chunks = new List<byte[]>();

                        // extsize is length of first sub-block
                        length = extsize;
                        try
                        {
                            // keep reading and appending until a 0 byte is reached
                            while (length > 0)
                            {
                                chunks.Add(reader.GetBytes(length));

                                nextByte = reader.GetByte();
                                length = nextByte; // reader.GetByte();
                            }
                        }
                        catch (IOException)
                        {
                            header.AddError("GIF format exception - invalid comment block");
                            break;
                        }

                        if (chunks.Count > 0)
                        {
                            var onechunk = chunks.SelectMany(a => a).ToArray();
                            header.Set(GifHeaderDirectory.TagComment, new StringValue(onechunk, Encoding.ASCII));
                        }

                        if (length > 0) // was a read error if length isn't zero
                            break;
                    }
                    else if (extmarker == 0xff && extsize == 0x0b)  // application extension
                    {
                        var buffer = reader.GetString(extsize, Encoding.UTF8);
                        if (buffer.Equals("XMP DataXMP"))
                        {
                            var chunks = new List<byte[]>();
                            byte lastLength = 0;
                            try
                            {
                                while (true)
                                {
                                    nextByte = reader.GetByte();
                                    if (nextByte == 0)
                                        break;

                                    length = nextByte;      // get next block size

                                    var chunk = new byte[length + 1];
                                    chunk[0] = length;
                                    Array.Copy(reader.GetBytes(length), 0, chunk, 1, length);
                                    chunks.Add(chunk);

                                    lastLength = length;
                                }
                            }
                            catch (IOException)
                            {
                                header.AddError("GIF format exception - invalid XMP block");
                                break;
                            }

                            if (chunks.Count > 0)
                            {
                                var onechunk = chunks.SelectMany(a => a).ToArray();
                                if (lastLength > 0)
                                    Array.Resize(ref onechunk, onechunk.Length - 0xFF - 2);

                                var xmpDirectory = new XmpReader().Extract(onechunk);
                                xmpDirectory.Parent = header;
                                directories.Add(xmpDirectory);
                            }

                            // *** Keeping this just in case; if the above byte version is comprehensive, this can be removed ***
                            // string search version
                            /*
                            var sb = new StringBuilder();
                            var length = reader.GetByte();
                            while(length > 0)
                            {
                                sb.Append(((char)length).ToString() + reader.GetString(length, Encoding.UTF8));
                                length = reader.GetByte();
                            }

                            if (sb.Length > 0)
                            {
                                string xmp = sb.ToString();

                                // get length of XMP without landing zone data
                                // (note that LZ data may not be exactly the same as what we use)
                                var findlist = new string[] { "<?xpacket end='w'?>", "<?xpacket end=\"w\"?>",
                                                                "<?xpacket end='r'?>", "<?xpacket end=\"r\"?>" };
                                var endIdx = -1;
                                foreach (var find in findlist)
                                {
                                    endIdx = xmp.IndexOf(find, StringComparison.OrdinalIgnoreCase);
                                    if (endIdx != -1)
                                    {
                                        endIdx += find.Length;
                                        break;
                                    }
                                }

                                if (endIdx != -1)
                                    xmp = xmp.Substring(0, endIdx);

                                var xmpDirectory = new XmpReader().Extract(Encoding.UTF8.GetBytes(xmp));
                                xmpDirectory.Parent = directory;
                                directories.Add(xmpDirectory);
                            }
                            */
                        }
                        else if (buffer.Equals("ICCRGBG1012"))   // ICC_Profile extension
                        {
                            var chunks = new List<byte[]>();
                            try
                            {
                                while (true)
                                {
                                    nextByte = reader.GetByte();
                                    if (nextByte == 0)
                                        break;

                                    length = nextByte;
                                    chunks.Add(reader.GetBytes(length));
                                }
                            }
                            catch (IOException)
                            {
                                header.AddError("GIF format exception - invalid ICC Profile block");
                                break;
                            }

                            if (chunks.Count > 0)
                            {
                                var onechunk = chunks.SelectMany(a => a).ToArray();

                                var iccDirectory = new IccReader().Extract(new ByteArrayReader(onechunk));
                                iccDirectory.Parent = header;
                                directories.Add(iccDirectory);
                            }
                        }
                        else if (buffer.Equals("NETSCAPE2.0"))   // animated GIF extension
                        {
                            var animreader = new ByteArrayReader(reader.GetBytes(5), 0, reader.IsMotorolaByteOrder);
                            header.Set(GifHeaderDirectory.Animate.TagAnimationIterations, animreader.GetUInt16(2));
                        }
                    }
                    else if (extmarker == 0xF9 && extsize == 4) // graphic control extension
                    {
                        try
                        {
                            var buff = reader.GetBytes(extsize);

                            var delay = new ByteArrayReader(buff).WithByteOrder(reader.IsMotorolaByteOrder).GetUInt16(1);
                            delayTotal += delay;
                            // skip 0x0 block terminator
                            reader.GetByte();
                        }
                        catch(IOException)
                        {
                            header.AddError("GIF format exception - invalid graphic control block");
                            break;
                        }
                    }
                    else if (extmarker == 0x01 && extsize == 12)    // plain text extension
                    {
                        // It seems this extension is deprecated. If somebody finds an image with this in it, could implement here.
                        // Just skip the entire block for now.
                        try
                        {
                            // skip 'extsize' bytes
                            reader.Skip(12);
                            // keep reading and skipping until a 0 byte is reached
                            while (true)
                            {
                                nextByte = reader.GetByte();
                                if (nextByte == 0)
                                    break;

                                length = nextByte;
                                reader.Skip(length);
                            }
                        }
                        catch (IOException)
                        {
                            header.AddError("GIF format exception - invalid Plain Text block");
                            break;
                        }
                    }
                }
                else
                    break;  // only need to handle image descriptors and extensions
            }

            if (framecount > 1)
                header.Set(GifHeaderDirectory.TagFrameCount, framecount);
            if (delayTotal > 0)
                header.Set(GifHeaderDirectory.TagDuration, delayTotal);
        }

    }
}
