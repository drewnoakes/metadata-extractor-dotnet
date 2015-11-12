#region License
//
// Copyright 2002-2015 Drew Noakes
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

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>An enumeration of the known segment types found in JPEG files.</summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>http://www.ozhiker.com/electronics/pjmt/jpeg_info/app_segments.html</item>
    ///   <item>http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/JPEG.html</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public enum JpegSegmentType : byte
    {
        /// <summary>APP0 JPEG segment. Commonly contains JFIF, JFXX.</summary>
        App0 = 0xE0,

        /// <summary>APP1 JPEG segment. Commonly contains Exif. XMP data is also kept in here, though usually in a second instance.</summary>
        App1 = 0xE1,

        /// <summary>APP2 JPEG segment. Commonly contains ICC.</summary>
        App2 = 0xE2,

        /// <summary>APP3 JPEG segment.</summary>
        App3 = 0xE3,

        /// <summary>APP4 JPEG segment.</summary>
        App4 = 0xE4,

        /// <summary>APP5 JPEG segment.</summary>
        App5 = 0xE5,

        /// <summary>APP6 JPEG segment.</summary>
        App6 = 0xE6,

        /// <summary>APP7 JPEG segment.</summary>
        App7 = 0xE7,

        /// <summary>APP8 JPEG segment.</summary>
        App8 = 0xE8,

        /// <summary>APP9 JPEG segment.</summary>
        App9 = 0xE9,

        /// <summary>APPA (App10) JPEG segment. Can contain Unicode comments, though <see cref="Com"/> is more commonly used for comments.</summary>
        AppA = 0xEA,

        /// <summary>APPB (App11) JPEG segment.</summary>
        AppB = 0xEB,

        /// <summary>APPC (App12) JPEG segment.</summary>
        AppC = 0xEC,

        /// <summary>APPD (App13) JPEG segment. Commonly contains IPTC, Photoshop data.</summary>
        AppD = 0xED,

        /// <summary>APPE (App14) JPEG segment. Commonly contains Adobe data.</summary>
        AppE = 0xEE,

        /// <summary>APPF (App15) JPEG segment.</summary>
        AppF = 0xEF,

        /// <summary>Start Of Image segment.</summary>
        Soi = 0xD8,

        /// <summary>Define Quantization Table segment.</summary>
        Dqt = 0xDB,

        /// <summary>Define Huffman Table segment.</summary>
        Dht = 0xC4,

        /// <summary>Start-of-Frame (0) segment.</summary>
        Sof0 = 0xC0,

        /// <summary>Start-of-Frame (1) segment.</summary>
        Sof1 = 0xC1,

        /// <summary>Start-of-Frame (2) segment.</summary>
        Sof2 = 0xC2,

        /// <summary>Start-of-Frame (3) segment.</summary>
        Sof3 = 0xC3,

//        /// <summary>Start-of-Frame (4) segment.</summary>
//        Sof12 = 0xC4,

        /// <summary>Start-of-Frame (5) segment.</summary>
        Sof5 = 0xC5,

        /// <summary>Start-of-Frame (6) segment.</summary>
        Sof6 = 0xC6,

        /// <summary>Start-of-Frame (7) segment.</summary>
        Sof7 = 0xC7,

        /// <summary>Start-of-Frame (8) segment.</summary>
        Sof8 = 0xC8,

        /// <summary>Start-of-Frame (9) segment.</summary>
        Sof9 = 0xC9,

        /// <summary>Start-of-Frame (10) segment.</summary>
        Sof10 = 0xCA,

        /// <summary>Start-of-Frame (11) segment.</summary>
        Sof11 = 0xCB,

//        /// <summary>Start-of-Frame (12) segment.</summary>
//        Sof12 = 0xCC,

        /// <summary>Start-of-Frame (13) segment.</summary>
        Sof13 = 0xCD,

        /// <summary>Start-of-Frame (14) segment.</summary>
        Sof14 = 0xCE,

        /// <summary>Start-of-Frame (15) segment.</summary>
        Sof15 = 0xCF,

        /// <summary>JPEG comment segment.</summary>
        Com = 0xFE,

        /// <summary>Start-of-Scan segment.</summary>
        Sos = 0xDA,

        /// <summary>End-of-Image segment.</summary>
        Eoi = 0xD9
    }

    public static class JpegSegmentTypeExtensions
    {
        public static bool CanContainMetadata(this JpegSegmentType type)
        {
            switch (type)
            {
                case JpegSegmentType.Soi:
                case JpegSegmentType.Dqt:
                case JpegSegmentType.Dht:
                    return false;
                default:
                    return true;
            }
        }

#if NET35 || PORTABLE
        public static IEnumerable<JpegSegmentType> CanContainMetadataTypes { get; }
#else
        public static IReadOnlyList<JpegSegmentType> CanContainMetadataTypes { get; }
#endif
            = Enum.GetValues(typeof(JpegSegmentType)).Cast<JpegSegmentType>().Where(type => type.CanContainMetadata()).ToList();
    }
}
