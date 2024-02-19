// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using MetadataExtractor.Formats.Mpeg;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Riff;
using MetadataExtractor.Formats.Tga;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace MetadataExtractor.Util
{
    /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
    public static class FileTypeDetector
    {
        // https://en.wikipedia.org/wiki/List_of_file_signatures
        private static readonly ByteTrie<FileType> _root = new(defaultValue: FileType.Unknown)
        {
            { FileType.Jpeg, [0xff, 0xd8] },
            { FileType.Tiff, "II"u8.ToArray(), [0x2a, 0x00] },
            { FileType.Tiff, "MM"u8.ToArray(), [0x00, 0x2a] },
            { FileType.Tiff, "II"u8.ToArray(), [0x2b, 0x00] }, // BigTIFF
            { FileType.Tiff, "MM"u8.ToArray(), [0x00, 0x2b] }, // BigTIFF
            { FileType.Psd, "8BPS"u8 },
            { FileType.Png, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52] },
            { FileType.Bmp, "BM"u8 }, // Standard Bitmap Windows and OS/2
            { FileType.Bmp, "BA"u8 }, // OS/2 Bitmap Array
            { FileType.Bmp, "CI"u8 }, // OS/2 Color Icon
            { FileType.Bmp, "CP"u8 }, // OS/2 Color Pointer
            { FileType.Bmp, "IC"u8 }, // OS/2 Icon
            { FileType.Bmp, "PT"u8 }, // OS/2 Pointer
            { FileType.Gif, "GIF87a"u8 },
            { FileType.Gif, "GIF89a"u8 },
            { FileType.Ico, [0x00, 0x00, 0x01, 0x00] },
            { FileType.Netpbm, "P1"u8 }, // ASCII B
            { FileType.Netpbm, "P2"u8 }, // ASCII greysca
            { FileType.Netpbm, "P3"u8 }, // ASCII R
            { FileType.Netpbm, "P4"u8 }, // RAW B
            { FileType.Netpbm, "P5"u8 }, // RAW greysca
            { FileType.Netpbm, "P6"u8 }, // RAW R
            { FileType.Netpbm, "P7"u8 }, // P
            { FileType.Pcx, [0x0A, 0x00, 0x01] },
            // multiple PCX versions, explicitly list
            { FileType.Pcx, [0x0A, 0x02, 0x01] },
            { FileType.Pcx, [0x0A, 0x03, 0x01] },
            { FileType.Pcx, [0x0A, 0x05, 0x01] },
            { FileType.Eps, "%!PS"u8 },
            { FileType.Eps, [0xC5, 0xD0, 0xD3, 0xC6] },
            // NOTE several file types match this, which we handle in TryDisambiguate: DNG, GPR (GoPro), KDC (Kodak), 3FR (Hasselblad)
            //{ FileType.Arw, "II"u8.ToArray(), [0x2a, 0x00, 0x08, 0x00] },
            { FileType.Crw, "II"u8.ToArray(), [0x1a, 0x00, 0x00, 0x00], "HEAPCCDR"u8.ToArray() },
            { FileType.Cr2, "II"u8.ToArray(), [0x2a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x43, 0x52] },
            // NOTE this doesn't work for NEF as it incorrectly flags many other TIFF files as being NEF
//            { FileType.Nef, "MM"u8.ToArray(), [0x00, 0x2a, 0x00, 0x00, 0x00, 0x08, 0x00] },
            { FileType.Orf, "IIRO"u8.ToArray(), [0x08, 0x00] },
            { FileType.Orf, "MMOR"u8.ToArray(), [0x00, 0x00] },
            { FileType.Orf, "IIRS"u8.ToArray(), [0x08, 0x00] },
            { FileType.Raf, "FUJIFILMCCD-RAW"u8 },
            { FileType.Rw2, "II"u8.ToArray(), [0x55, 0x00] },
        };

        private static FileType TryDisambiguate(FileType detectedFileType, string fileName)
        {
            if (detectedFileType == FileType.Tiff)
            {
                var extension = GetExtension();

                if (MemoryExtensions.Equals(extension, ".arw".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.Arw;
                }
                else if (MemoryExtensions.Equals(extension, ".dng".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.Dng;
                }
                else if (MemoryExtensions.Equals(extension, ".gpr".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.GoPro;
                }
                else if (MemoryExtensions.Equals(extension, ".kdc".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.Kdc;
                }
                else if (MemoryExtensions.Equals(extension, ".nef".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.Nef;
                }
                else if (MemoryExtensions.Equals(extension, ".3fr".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.ThreeFR;
                }
                else if (MemoryExtensions.Equals(extension, ".pef".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.Pef;
                }
                else if (MemoryExtensions.Equals(extension, ".srw".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return FileType.Srw;
                }
            }

            return detectedFileType;

            ReadOnlySpan<char> GetExtension()
            {
                int index = fileName.LastIndexOf('.');

                if (index == -1)
                {
                    return [];
                }

                return fileName.AsSpan(index);
            }
        }

        private static readonly IReadOnlyList<ITypeChecker> _checkers =
        [
            new QuickTimeTypeChecker(),
            new RiffTypeChecker(),
            new TgaTypeChecker(),
            new MpegAudioTypeChecker()
        ];

        private static readonly int _bytesNeeded = Math.Max(
            _root.MaxDepth,
            _checkers.Max(checker => checker.ByteCount));

        /// <summary>Examines the file's first bytes and estimates the file's type.</summary>
        /// <exception cref="ArgumentException">Stream does not support seeking.</exception>
        /// <exception cref="IOException">An IO error occurred, or the input stream ended unexpectedly.</exception>
        public static FileType DetectFileType(Stream stream, string? fileName = null)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Must support seek", nameof(stream));

            var bytes = ArrayPool<byte>.Shared.Rent(_bytesNeeded);

            try
            {
                var bytesRead = stream.Read(bytes, 0, _bytesNeeded);

                if (bytesRead == 0)
                {
                    // Stream was empty
                    return FileType.Unknown;
                }

                // Rewind the stream to where we started. We are only peeking at the data.
                stream.Seek(-bytesRead, SeekOrigin.Current);

                // Use the prefix trie to match bytes.
                var fileType = _root.Find(bytes);

                if (fileType != FileType.Unknown)
                {
                    // Found
                    return fileName is null ? fileType : TryDisambiguate(fileType, fileName);
                }

                foreach (var checker in _checkers)
                {
                    if (bytesRead >= checker.ByteCount)
                    {
                        fileType = checker.CheckType(bytes);

                        if (fileType != FileType.Unknown)
                        {
                            // Found
                            return fileType;
                        }
                    }
                }

                return FileType.Unknown;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
    }
}
