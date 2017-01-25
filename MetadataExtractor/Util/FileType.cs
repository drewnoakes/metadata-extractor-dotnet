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

namespace MetadataExtractor.Util
{
    /// <summary>Enumeration of supported image file formats.</summary>
    public enum FileType
    {
        /// <summary>File type is not known.</summary>
        Unknown,

        /// <summary>Joint Photographic Experts Group (JPEG).</summary>
        Jpeg,

        /// <summary>Tagged Image File Format (TIFF).</summary>
        Tiff,

        /// <summary>Photoshop Document.</summary>
        Psd,

        /// <summary>Portable Network Graphic (PNG).</summary>
        Png,

        /// <summary>Bitmap (BMP).</summary>
        Bmp,

        /// <summary>Graphics Interchange Format (GIF).</summary>
        Gif,

        /// <summary>Windows Icon.</summary>
        Ico,

        /// <summary>PiCture eXchange.</summary>
        Pcx,

        /// <summary>Resource Interchange File Format.</summary>
        Riff,

        /// <summary>Sony camera raw.</summary>
        Arw,

        /// <summary>Canon camera raw (version 1).</summary>
        Crw,

        /// <summary>Canon camera raw (version 2).</summary>
        Cr2,

        /// <summary>Nikon camera raw.</summary>
        Nef,

        /// <summary>Olympus camera raw.</summary>
        Orf,

        /// <summary>Fujifilm camera raw.</summary>
        Raf,

        /// <summary>Panasonic camera raw.</summary>
        Rw2,

        /// <summary>QuickTime (mov) format video.</summary>
        QuickTime,

        /// <summary>Netpbm family of image formats.</summary>
        Netpbm
    }
}
