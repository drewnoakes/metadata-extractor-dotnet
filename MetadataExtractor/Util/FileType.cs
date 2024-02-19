// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util
{
    /// <summary>Enumeration of supported image file formats.</summary>
    public enum FileType
    {
        /// <summary>File type is not known.</summary>
        Unknown = 0,

        /// <summary>Joint Photographic Experts Group (JPEG).</summary>
        Jpeg = 1,

        /// <summary>Tagged Image File Format (TIFF).</summary>
        Tiff = 2,

        /// <summary>Photoshop Document.</summary>
        Psd = 3,

        /// <summary>Portable Network Graphic (PNG).</summary>
        Png = 4,

        /// <summary>Bitmap (BMP).</summary>
        Bmp = 5,

        /// <summary>Graphics Interchange Format (GIF).</summary>
        Gif = 6,

        /// <summary>Windows Icon.</summary>
        Ico = 7,

        /// <summary>PiCture eXchange.</summary>
        Pcx = 8,

        /// <summary>Resource Interchange File Format.</summary>
        Riff = 9,

        /// <summary>Waveform Audio File Format.</summary>
        Wav = 10,

        /// <summary>Audio Video Interleaved.</summary>
        Avi = 11,

        /// <summary>WebP.</summary>
        WebP = 12,

        /// <summary>Sony camera raw.</summary>
        Arw = 13,

        /// <summary>Canon camera raw (version 1).</summary>
        Crw = 14,

        /// <summary>Canon camera raw (version 2).</summary>
        Cr2 = 15,

        /// <summary>Nikon camera raw.</summary>
        Nef = 16,

        /// <summary>Olympus camera raw.</summary>
        Orf = 17,

        /// <summary>Fujifilm camera raw.</summary>
        Raf = 18,

        /// <summary>Panasonic camera raw.</summary>
        Rw2 = 19,

        /// <summary>QuickTime (mov) format video.</summary>
        QuickTime = 20,

        /// <summary>Netpbm family of image formats.</summary>
        Netpbm = 21,

        /// <summary>Canon camera raw (version 3).</summary>
        /// <remarks>Shared by CR3 (image) and CRM (video).</remarks>
        Crx = 22,

        /// <summary>Encapsulated PostScript.</summary>
        Eps = 23,

        /// <summary>Truevision graphics (Targa).</summary>
        Tga = 24,

        /// <summary>MPEG-1 / MPEG-2 Audio Layer III.</summary>
        Mp3 = 25,

        /// <summary>High Efficiency Image File Format.</summary>
        Heif = 26,

        /// <summary>MPEG-4 Part 14.</summary>
        Mp4 = 27,

        /// <summary>AV1 Image File Format.</summary>
        Avif = 28,

        /// <summary>DNG Image File Format.</summary>
        Dng = 29,

        /// <summary>GPR (GoPro) Image File Format.</summary>
        GoPro = 30,

        /// <summary>KDC (Kodak) Image File Format.</summary>
        Kdc = 31,

        /// <summary>3FR (Hasselblad) Image File Format.</summary>
        ThreeFR = 32,

        /// <summary>PEF (Pentax) Image File Format.</summary>
        Pef = 33,

        /// <summary>SRW (Samsung) Image File Format.</summary>
        Srw = 34,
    }

    public static class FileTypeExtensions
    {
        private static readonly string[] _shortNames =
        [
            "Unknown",
            "JPEG",
            "TIFF",
            "PSD",
            "PNG",
            "BMP",
            "GIF",
            "ICO",
            "PCX",
            "RIFF",
            "WAV",
            "AVI",
            "WebP",
            "ARW",
            "CRW",
            "CR2",
            "NEF",
            "ORF",
            "RAF",
            "RW2",
            "QuickTime",
            "Netpbm",
            "CRX",
            "EPS",
            "TGA",
            "MP3",
            "HEIF",
            "MP4",
            "AVIF",
            "DNG",
            "GPR",
            "KDC",
            "3FR",
            "PEF",
            "SRW",
        ];

        private static readonly string[] _longNames =
        [
            "Unknown",
            "Joint Photographic Experts Group",
            "Tagged Image File Format",
            "Photoshop Document",
            "Portable Network Graphics",
            "Device Independent Bitmap",
            "Graphics Interchange Format",
            "Windows Icon",
            "PiCture eXchange",
            "Resource Interchange File Format",
            "Waveform Audio File Format",
            "Audio Video Interleaved",
            "WebP",
            "Sony Camera Raw",
            "Canon Camera Raw",
            "Canon Camera Raw",
            "Nikon Camera Raw",
            "Olympus Camera Raw",
            "FujiFilm Camera Raw",
            "Panasonic Camera Raw",
            "QuickTime",
            "Netpbm",
            "Canon Camera Raw",
            "Encapsulated PostScript",
            "Truevision Graphics",
            "MPEG Audio Layer III",
            "High Efficiency Image File Format",
            "MPEG-4 Part 14",
            "AV1 Image File Format",
            "Digital Negative",
            "GoPro Raw",
            "Kodak Raw",
            "Hasselblad Raw",
            "Pentax Raw",
            "Samsung Raw",
        ];

        private static readonly string?[] _mimeTypes =
        [
            null,
            "image/jpeg",
            "image/tiff",
            "image/vnd.adobe.photoshop",
            "image/png",
            "image/bmp",
            "image/gif",
            "image/x-icon",
            "image/x-pcx",
            null, // RIFF
            "audio/vnd.wave",
            "video/vnd.avi",
            "image/webp",
            "image/x-sony-arw", // https://stackoverflow.com/a/47612661/24874
            "image/x-canon-crw",
            "image/x-canon-cr2",
            "image/x-nikon-nef",
            "image/x-olympus-orf",
            "image/x-fuji-raf",
            null,
            "video/quicktime",
            "image/x-portable-graymap",
            null,
            "application/postscript",
            "image/x-targa",
            "audio/mpeg",
            "image/heic",
            "video/mp4",
            "image/avif",
            "image/x-adobe-dng",
            "image/x-gopro-gpr",
            "image/x-kodak-kdc",
            "image/x-hasselblad-3fr",
            "image/x-pentax-pef",
            "image/x-samsung-srw",
        ];

        private static readonly string[]?[] _extensions =
        [
            null,
            ["jpg", "jpeg", "jpe"],
            ["tiff", "tif"],
            ["psd"],
            ["png"],
            ["bmp"],
            ["gif"],
            ["ico"],
            ["pcx"],
            null, // RIFF
            ["wav", "wave"],
            ["avi"],
            ["webp"],
            ["arw"],
            ["crw"],
            ["cr2"],
            ["nef"],
            ["orf"],
            ["raf"],
            ["rw2"],
            ["mov"],
            ["pbm", "ppm"],
            ["cr3", "crm"],
            ["eps", "epsf", "epsi"],
            ["tga", "icb", "vda", "vst"],
            ["mp3"],
            ["heic", "heif", "avci"],
            ["mp4", "m4a", "m4p", "m4b", "m4r", "m4v"],
            ["avif"],
            ["dng"],
            ["gpr"],
            ["kdc"],
            ["3fr"],
            ["pef"],
            ["srw"],
        ];

        public static string GetName(this FileType fileType)
        {
            var i = (uint)fileType;
            if (i >= _shortNames.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _shortNames[i];
        }

        public static string GetLongName(this FileType fileType)
        {
            var i = (uint)fileType;
            if (i >= _longNames.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _longNames[i];
        }

        public static string? GetMimeType(this FileType fileType)
        {
            var i = (uint)fileType;
            if (i >= _mimeTypes.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _mimeTypes[i];
        }

        public static string? GetCommonExtension(this FileType fileType)
        {
            var i = (uint)fileType;
            if (i >= _extensions.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _extensions[i]?.FirstOrDefault();
        }

        public static IEnumerable<string>? GetAllExtensions(this FileType fileType)
        {
            var i = (uint)fileType;
            if (i >= _extensions.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _extensions[i];
        }
    }
}
