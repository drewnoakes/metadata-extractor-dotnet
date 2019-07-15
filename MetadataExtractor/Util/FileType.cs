#region License
//
// Copyright 2002-2019 Drew Noakes
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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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
        Wav = 10, // ("WAV", "Waveform Audio File Format", "audio/vnd.wave", "wav", "wave"),
        
        /// <summary>Audio Video Interleaved.</summary>
        Avi = 11, //("AVI", "Audio Video Interleaved", "video/vnd.avi", "avi"),
        
        /// <summary>WebP.</summary>
        WebP = 12, //("WebP", "WebP", "image/webp", "webp"),

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
        Netpbm = 21
    }

    public static class FileTypeExtensions
    {
        private static readonly string[] _shortNames =
        {
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
            "Netpbm"
        };
        
        private static readonly string[] _longNames =
        {
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
            "Netpbm"
        };

        [ItemCanBeNull] private static readonly string[] _mimeTypes =
        {
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
            null, // Sony RAW
            null,
            null,
            null,
            null,
            null,
            null,
            "video/quicktime",
            "image/x-portable-graymap"
        };

        [ItemCanBeNull] private static readonly string[][] _extensions =
        {
            null,
            new[] { "jpg", "jpeg", "jpe" },
            new[] { "tiff", "tif" },
            new[] { "psd" },
            new[] { "png" },
            new[] { "bmp" },
            new[] { "gif" },
            new[] { "ico" },
            new[] { "pcx" },
            null, // RIFF
            new[] { "wav", "wave" },
            new[] { "avi" },
            new[] { "webp" },
            new[] { "arw" },
            new[] { "crw" },
            new[] { "cr2" },
            new[] { "nef" },
            new[] { "orf" },
            new[] { "raf" },
            new[] { "rw2" },
            new[] { "mov" },
            new[] { "pbm", "ppm" }
        };
        
        [NotNull]
        public static string GetName(this FileType fileType)
        {
            var i = (int)fileType;
            if (i < 0 || i >= _shortNames.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _shortNames[i];
        }
        
        [NotNull]
        public static string GetLongName(this FileType fileType)
        {
            var i = (int)fileType;
            if (i < 0 || i >= _longNames.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _longNames[i];
        }
        
        [CanBeNull]
        public static string GetMimeType(this FileType fileType)
        {
            var i = (int)fileType;
            if (i < 0 || i >= _mimeTypes.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _mimeTypes[i];
        }
        
        [CanBeNull]
        public static string GetCommonExtension(this FileType fileType)
        {
            var i = (int)fileType;
            if (i < 0 || i >= _extensions.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _extensions[i]?.FirstOrDefault();
        }
        
        [CanBeNull]
        public static IEnumerable<string> GetAllExtensions(this FileType fileType)
        {
            var i = (int)fileType;
            if (i < 0 || i >= _mimeTypes.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _extensions[i];
        }
    }
}
