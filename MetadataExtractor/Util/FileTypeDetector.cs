// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Mpeg;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Riff;
using MetadataExtractor.Formats.Tga;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace MetadataExtractor.Util;

/// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
public static class FileTypeDetector
{
    // https://en.wikipedia.org/wiki/List_of_file_signatures
    private static readonly ByteTrie<FileType> _root = new(defaultValue: FileType.Unknown)
    {
        { FileType.Jpeg, new byte[] { 0xff, 0xd8 } },
        { FileType.Tiff, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00 } },
        { FileType.Tiff, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2a } },
        { FileType.Tiff, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2b, 0x00 } }, // BigTIFF
        { FileType.Tiff, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2b } }, // BigTIFF
        { FileType.Psd, Encoding.UTF8.GetBytes("8BPS") },
        { FileType.Png, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 } },
        { FileType.Bmp, Encoding.UTF8.GetBytes("BM") }, // Standard Bitmap Windows and OS/2
        { FileType.Bmp, Encoding.UTF8.GetBytes("BA") }, // OS/2 Bitmap Array
        { FileType.Bmp, Encoding.UTF8.GetBytes("CI") }, // OS/2 Color Icon
        { FileType.Bmp, Encoding.UTF8.GetBytes("CP") }, // OS/2 Color Pointer
        { FileType.Bmp, Encoding.UTF8.GetBytes("IC") }, // OS/2 Icon
        { FileType.Bmp, Encoding.UTF8.GetBytes("PT") }, // OS/2 Pointer
        { FileType.Gif, Encoding.UTF8.GetBytes("GIF87a") },
        { FileType.Gif, Encoding.UTF8.GetBytes("GIF89a") },
        { FileType.Ico, new byte[] { 0x00, 0x00, 0x01, 0x00 } },
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P1") }, // ASCII B
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P2") }, // ASCII greysca
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P3") }, // ASCII R
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P4") }, // RAW B
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P5") }, // RAW greysca
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P6") }, // RAW R
        { FileType.Netpbm, Encoding.UTF8.GetBytes("P7") }, // P
        { FileType.Pcx, new byte[] { 0x0A, 0x00, 0x01 } },
        // multiple PCX versions, explicitly list
        { FileType.Pcx, new byte[] { 0x0A, 0x02, 0x01 } },
        { FileType.Pcx, new byte[] { 0x0A, 0x03, 0x01 } },
        { FileType.Pcx, new byte[] { 0x0A, 0x05, 0x01 } },
        { FileType.Eps, Encoding.UTF8.GetBytes("%!PS") },
        { FileType.Eps, new byte[] { 0xC5, 0xD0, 0xD3, 0xC6 } },
        { FileType.Arw, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00, 0x08, 0x00 } },
        { FileType.Crw, Encoding.UTF8.GetBytes("II"), new byte[] { 0x1a, 0x00, 0x00, 0x00 }, Encoding.UTF8.GetBytes("HEAPCCDR") },
        { FileType.Cr2, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x43, 0x52 } },
        // NOTE this doesn't work for NEF as it incorrectly flags many other TIFF files as being NEF
//            { FileType.Nef, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2a, 0x00, 0x00, 0x00, 0x08, 0x00 } },
        { FileType.Orf, Encoding.UTF8.GetBytes("IIRO"), new byte[] { 0x08, 0x00 } },
        { FileType.Orf, Encoding.UTF8.GetBytes("MMOR"), new byte[] { 0x00, 0x00 } },
        { FileType.Orf, Encoding.UTF8.GetBytes("IIRS"), new byte[] { 0x08, 0x00 } },
        { FileType.Raf, Encoding.UTF8.GetBytes("FUJIFILMCCD-RAW") },
        { FileType.Rw2, Encoding.UTF8.GetBytes("II"), new byte[] { 0x55, 0x00 } },
    };

    private static readonly IEnumerable<ITypeChecker> _fixedCheckers = new ITypeChecker[]
    {
        new QuickTimeTypeChecker(),
        new RiffTypeChecker(),
        new TgaTypeChecker(),
        new MpegAudioTypeChecker()
    };

    private static readonly int _bytesNeeded = Math.Max(
            _root.MaxDepth,
            _fixedCheckers.Max(checker => checker.ByteCount));

    /// <summary>Examines the file's first bytes and estimates the file's type.</summary>
    /// <exception cref="ArgumentException">Stream does not support seeking.</exception>
    /// <exception cref="IOException">An IO error occurred, or the input stream ended unexpectedly.</exception>
    public static FileType DetectFileType(Stream stream)
    {
        if (!stream.CanSeek)
            throw new ArgumentException("Must support seek", nameof(stream));

        var bytes = new byte[_bytesNeeded];
        var bytesRead = stream.Read(bytes, 0, bytes.Length);

        if (bytesRead == 0)
            return FileType.Unknown;

        stream.Seek(-bytesRead, SeekOrigin.Current);

        var fileType = _root.Find(bytes);

        if (fileType == FileType.Unknown)
        {
            foreach (var fixedChecker in _fixedCheckers)
            {
                if (bytesRead >= fixedChecker.ByteCount)
                {
                    fileType = fixedChecker.CheckType(bytes);

                    if (fileType != FileType.Unknown)
                        return fileType;
                }
            }
        }

        return fileType;
    }
}
