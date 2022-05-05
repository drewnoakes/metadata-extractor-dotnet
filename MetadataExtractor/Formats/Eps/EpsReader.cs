// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Text;

using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Eps
{
    /// <summary>Reads file passed in through SequentialReader and parses encountered data:</summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Basic EPS Comments</item>
    ///   <item>EXIF</item>
    ///   <item>Photoshop</item>
    ///   <item>IPTC</item>
    ///   <item>ICC Profile</item>
    ///   <item>XMP</item>
    /// </list>
    /// EPS comments are retrieved from EPS directory.  Photoshop, ICC Profile, and XMP processing
    /// is passed to their respective reader.
    /// <para />
    /// EPS Constraints (Source: https://www-cdf.fnal.gov/offline/PostScript/5001.PDF pg.18):
    /// <list type = "bullet" >
    ///   <item>Max line length is 255 characters</item>
    ///   <item>Lines end with a CR(0xD) or LF(0xA) character (or both, in practice)</item>
    ///   <item>':' separates keywords (considered part of the keyword)</item>
    ///   <item>Whitespace is either a space(0x20) or tab(0x9)</item>
    ///   <item>If there is more than one header, the 1st is truth</item>
    /// </list>
    /// </remarks>
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class EpsReader
    {
        private int _previousTag;

        public IReadOnlyList<Directory> Extract(Stream inputStream)
        {
            IndexedReader reader = new IndexedSeekingReader(inputStream);
            var directory = new EpsDirectory();
            var epsDirectories = new List<Directory>() { directory };

            // 0xC5D0D3C6 signifies an EPS Header block which contains 32-bytes of basic information
            // 0x25215053 (%!PS) signifies an EPS File and leads straight into the PostScript

            int startingPosition = (int)inputStream.Position;

            switch (reader.GetInt32(0))
            {
                case unchecked((int)0xC5D0D3C6):
                    reader = reader.WithByteOrder(isMotorolaByteOrder: false);
                    int postScriptOffset = reader.GetInt32(4);
                    int postScriptLength = reader.GetInt32(8);
                    int wmfOffset = reader.GetInt32(12);
                    int wmfSize = reader.GetInt32(16);
                    int tifOffset = reader.GetInt32(20);
                    int tifSize = reader.GetInt32(24);
                    //int checkSum = reader.getInt32(28);

                    // Get Tiff/WMF preview data if applicable
                    if (tifSize != 0)
                    {
                        directory.Set(EpsDirectory.TagTiffPreviewSize, tifSize);
                        directory.Set(EpsDirectory.TagTiffPreviewOffset, tifOffset);
                        // Get Tiff metadata
                        try
                        {
                            ByteArrayReader byteArrayReader = new(reader.GetBytes(tifOffset, tifSize));
                            TiffReader.ProcessTiff(byteArrayReader, new PhotoshopTiffHandler(epsDirectories));
                        }
                        catch (TiffProcessingException ex)
                        {
                            directory.AddError("Unable to process TIFF data: " + ex.Message);
                        }
                    }
                    else if (wmfSize != 0)
                    {
                        directory.Set(EpsDirectory.TagWmfPreviewSize, wmfSize);
                        directory.Set(EpsDirectory.TagWmfPreviewOffset, wmfOffset);
                    }

                    // TODO avoid allocating byte array here -- read directly from InputStream
                    Extract(directory, epsDirectories, new SequentialByteArrayReader(reader.GetBytes(postScriptOffset, postScriptLength)));
                    break;
                case 0x25215053:
                    inputStream.Position = startingPosition;
                    Extract(directory, epsDirectories, new SequentialStreamReader(inputStream));
                    break;
                default:
                    directory.AddError("File type not supported.");
                    break;
            }

            return epsDirectories;
        }

        /// <summary>
        /// Main method that parses all comments and then distributes data extraction among other methods that parse the
        /// rest of file and store encountered data in metadata(if there exists an entry in EpsDirectory
        /// for the found data).  Reads until a begin data/binary comment is found or reader's estimated
        /// available data has run out (or AI09 End Private Data).  Will extract data from normal EPS comments, Photoshop, ICC, and XMP.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directories">list to add directory to and extracted data</param>
        /// <param name="reader"></param>
        private void Extract(EpsDirectory directory, List<Directory> directories, SequentialReader reader)
        {
            var line = new StringBuilder();

            while (true)
            {
                line.Length = 0;

                // Read the next line, excluding any trailing newline character
                // Note that for Windows-style line endings ("\r\n") the outer loop will be run a second time with an empty
                // string, which is fine.
                while (true)
                {
                    char c = (char)reader.GetByte();
                    if (c == '\r' || c == '\n')
                        break;
                    line.Append(c);
                }

                // Stop when we hit a line that is not a comment
                if (line.Length != 0 && line[0] != '%')
                    break;

                string name;

                // ':' signifies there is an associated keyword (should be put in directory)
                // otherwise, the name could be a marker
                int colonIndex = line.IndexOf(':');
                if (colonIndex != -1)
                {
                    name = line.ToString(0, colonIndex).Trim();
                    var value = line.ToString(colonIndex + 1, line.Length - (colonIndex + 1)).Trim();
                    AddToDirectory(directory, name, value);
                }
                else
                    name = line.ToString().Trim();

                // Some comments will both have a value and signify a new block to follow
                if (name.Equals("%BeginPhotoshop"))
                    ExtractPhotoshopData(directories, reader);
                else if (name.Equals("%%BeginICCProfile"))
                    ExtractIccData(directories, reader);
                else if (name.Equals("%begin_xml_packet"))
                    ExtractXmpData(directories, reader);
            }
        }

        /// <summary>
        /// Default case that adds comment with keyword to directory
        /// </summary>
        /// <param name="directory">EpsDirectory to add extracted data to</param>
        /// <param name="name">String that holds name of current comment</param>
        /// <param name="value">String that holds value of current comment</param>
        private void AddToDirectory(EpsDirectory directory, string name, string value)
        {
            if (!EpsDirectory.TagIntegerMap.ContainsKey(name))
                return;

            var tag = EpsDirectory.TagIntegerMap[name];

            switch (tag)
            {
                case EpsDirectory.TagImageData:
                    ExtractImageData(directory, value);
                    break;
                case EpsDirectory.TagContinueLine:
                    directory.Set(_previousTag, directory.GetString(_previousTag) + " " + value);
                    break;
                default:
                    if (EpsDirectory.TagNameMap.ContainsKey(tag) && !directory.ContainsTag(tag))
                    {
                        directory.Set(tag, value);
                        _previousTag = tag;
                    }
                    else
                    {
                        // Set previous tag to an Integer that doesn't exist in EpsDirectory
                        _previousTag = 0;
                    }
                    break;
            }
            _previousTag = tag;
        }

        /// <summary>
        /// Parses '%ImageData' comment which holds several values including width in px,
        /// height in px and color type.
        /// </summary>
        private static void ExtractImageData(EpsDirectory directory, string imageData)
        {
            // %ImageData: 1000 1000 8 3 1 1000 7 "beginimage"
            directory.Set(EpsDirectory.TagImageData, imageData.Trim());

            var imageDataParts = imageData.Split(' ');

            int width = int.Parse(imageDataParts[0]);
            int height = int.Parse(imageDataParts[1]);
            int colorType = int.Parse(imageDataParts[3]);

            // Only add values that are not already present
            if (!directory.ContainsTag(EpsDirectory.TagImageWidth))
                directory.Set(EpsDirectory.TagImageWidth, width);
            if (!directory.ContainsTag(EpsDirectory.TagImageHeight))
                directory.Set(EpsDirectory.TagImageHeight, height);
            if (!directory.ContainsTag(EpsDirectory.TagColorType))
                directory.Set(EpsDirectory.TagColorType, colorType);

            if (!directory.ContainsTag(EpsDirectory.TagRamSize))
            {
                int bytesPerPixel = colorType switch
                {
                    1 => 1, // grayscale
                    2 => 3, // Lab
                    3 => 3, // RGB
                    4 => 3, // CMYK
                    _ => 0
                };

                if (bytesPerPixel != 0)
                    directory.Set(EpsDirectory.TagRamSize, bytesPerPixel * width * height);
            }
        }

        /// <summary>
        /// Decodes a commented hex section, and uses <see cref="PhotoshopReader"/> to decode the resulting data.
        /// </summary>
        private static void ExtractPhotoshopData(List<Directory> directories, SequentialReader reader)
        {
            var buffer = DecodeHexCommentBlock(reader);

            if (buffer != null)
                directories.AddRange(new PhotoshopReader().Extract(new SequentialByteArrayReader(buffer), buffer.Length));
        }

        /// <summary>
        /// Decodes a commented hex section, and uses <see cref="IccReader"/> to decode the resulting data.
        /// </summary>
        private static void ExtractIccData(List<Directory> directories, SequentialReader reader)
        {
            var buffer = DecodeHexCommentBlock(reader);

            if (buffer != null)
                directories.Add(new IccReader().Extract(new ByteArrayReader(buffer)));
        }

        /// <summary>
        /// Extracts an XMP xpacket, and uses <see cref="XmpReader"/> to decode the resulting data.
        /// </summary>
        private static void ExtractXmpData(List<Directory> directories, SequentialReader reader)
        {
            byte[] xmp = ReadUntil(reader, Encoding.UTF8.GetBytes("<?xpacket end=\"w\"?>"));
            directories.Add(new XmpReader().Extract(xmp));
        }

        /// <summary>
        /// Reads all bytes until the given sentinel is observed.
        /// The sentinel will be included in the returned bytes.
        /// </summary>
        private static byte[] ReadUntil(SequentialReader reader, byte[] sentinel)
        {
            var bytes = new MemoryStream();

            int length = sentinel.Length;
            int depth = 0;

            while (depth != length)
            {
                byte b = reader.GetByte();
                if (b == sentinel[depth])
                    depth++;
                else
                    depth = 0;
                bytes.WriteByte(b);
            }

            return bytes.ToArray();
        }

        /**
         * EPS files can contain hexadecimal-encoded ASCII blocks, each prefixed with <c>"% "</c>.
         * This method reads such a block and returns a byte[] of the decoded contents.
         * Reading stops at the first invalid line, which is discarded (it's a terminator anyway).
         * <p/>
         * For example:
         * <pre><code>
         * %BeginPhotoshop: 9564
         * % 3842494D040400000000005D1C015A00031B25471C0200000200041C02780004
         * % 6E756C6C1C027A00046E756C6C1C025000046E756C6C1C023700083230313630
         * % 3331311C023C000B3131343335362B303030301C023E00083230313630333131
         * % 48000000010000003842494D03FD0000000000080101000000000000
         * %EndPhotoshop
         * </code></pre>
         * When calling this method, the reader must be positioned at the start of the first line containing
         * hex data, not at the introductory line.
         *
         * @return The decoded bytes, or <code>null</code> if decoding failed.
         */
        /// <remarks>
        /// EPS files can contain hexadecimal-encoded ASCII blocks, each prefixed with "% ".
        /// This method reads such a block and returns a byte[] of the decoded contents.
        /// Reading stops at the first invalid line, which is discarded(it's a terminator anyway).
        /// <para />
        /// For example:
        /// <para />
        /// %BeginPhotoshop: 9564
        /// % 3842494D040400000000005D1C015A00031B25471C0200000200041C02780004
        /// % 6E756C6C1C027A00046E756C6C1C025000046E756C6C1C023700083230313630
        /// % 3331311C023C000B3131343335362B303030301C023E00083230313630333131
        /// % 48000000010000003842494D03FD0000000000080101000000000000
        /// %EndPhotoshop
        /// <para />
        /// When calling this method, the reader must be positioned at the start of the first line containing
        /// hex data, not at the introductory line.
        /// </remarks>
        /// <returns>The decoded bytes, or null if decoding failed.</returns>
        private static byte[]? DecodeHexCommentBlock(SequentialReader reader)
        {
            var bytes = new MemoryStream();

            // Use a state machine to efficiently parse data in a single traversal

            const int AwaitingPercent = 0;
            const int AwaitingSpace = 1;
            const int AwaitingHex1 = 2;
            const int AwaitingHex2 = 3;

            int state = AwaitingPercent;

            int carry = 0;
            bool done = false;

            byte b = 0;
            while (!done)
            {
                b = reader.GetByte();

                switch (state)
                {
                    case AwaitingPercent:
                    {
                        switch (b)
                        {
                            case (byte)'\r':
                            case (byte)'\n':
                            case (byte)' ':
                                // skip newline chars and spaces
                                break;
                            case (byte)'%':
                                state = AwaitingSpace;
                                break;
                            default:
                                return null;
                        }
                        break;
                    }
                    case AwaitingSpace:
                    {
                        switch (b)
                        {
                            case (byte)' ':
                                state = AwaitingHex1;
                                break;
                            default:
                                done = true;
                                break;
                        }
                        break;
                    }
                    case AwaitingHex1:
                    {
                        int i = TryHexToInt(b);
                        if (i != -1)
                        {
                            carry = i * 16;
                            state = AwaitingHex2;
                        }
                        else if (b == '\r' || b == '\n')
                        {
                            state = AwaitingPercent;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    }
                    case AwaitingHex2:
                    {
                        int i = TryHexToInt(b);
                        if (i == -1)
                            return null;
                        bytes.WriteByte((byte)(carry + i));
                        state = AwaitingHex1;
                        break;
                    }
                }
            }

            // skip through the remainder of the last line
            while (b != '\n')
                b = reader.GetByte();

            return bytes.ToArray();
        }

        /// <summary>
        /// Treats a byte as an ASCII character, and returns its numerical value in hexadecimal.
        /// If conversion is not possible, returns -1.
        /// </summary>
        private static int TryHexToInt(byte b)
        {
            if (b >= '0' && b <= '9')
                return b - '0';
            if (b >= 'A' && b <= 'F')
                return b - 'A' + 10;
            if (b >= 'a' && b <= 'f')
                return b - 'a' + 10;
            return -1;
        }
    }
}
