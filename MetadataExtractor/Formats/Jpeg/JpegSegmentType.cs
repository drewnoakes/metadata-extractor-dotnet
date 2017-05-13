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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>An enumeration of the known segment types found in JPEG files.</summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>http://www.ozhiker.com/electronics/pjmt/jpeg_info/app_segments.html</item>
    ///   <item>http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/JPEG.html</item>
    ///   <item>http://lad.dsc.ufcg.edu.br/multimidia/jpegmarker.pdf</item>
    ///   <item>http://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_JPEG_files</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum JpegSegmentType : byte
    {
        /// <summary>For temporary use in arithmetic coding.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Tem = 0x01,

        /// <summary>Start Of Image segment. Begins the compressed JPEG data stream.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Soi = 0xD8,

        /// <summary>Define Quantization Table.</summary>
        /// <remarks>Specifies one or more quantization tables.</remarks>
        Dqt = 0xDB,

        /// <summary>Start-of-Frame, non-differential Huffman coding frame, baseline DCT.</summary>
        /// <remarks>
        /// Indicates that this is a baseline DCT-based JPEG, and specifies the width,
        /// height, number of components, and component subsampling (e.g., 4:2:0).
        /// </remarks>
        Sof0 = 0xC0,

        /// <summary>Start-of-Frame, non-differential Huffman coding frame, extended sequential DCT.</summary>
        Sof1 = 0xC1,

        /// <summary>Start-of-Frame, non-differential Huffman coding frame, progressive DCT.</summary>
        /// <remarks>
        /// Indicates that this is a progressive DCT-based JPEG, and specifies the width,
        /// height, number of components, and component subsampling (e.g., 4:2:0).
        /// </remarks>
        Sof2 = 0xC2,

        /// <summary>Start-of-Frame, non-differential Huffman coding frame, lossless sequential.</summary>
        Sof3 = 0xC3,

        /// <summary>Define Huffman Table(s).</summary>
        /// <remarks>Specifies one or more Huffman tables.</remarks>
        Dht = 0xC4,

        /// <summary>Start-of-Frame, differential Huffman coding frame, differential sequential DCT.</summary>
        Sof5 = 0xC5,

        /// <summary>Start-of-Frame, differential Huffman coding frame, differential progressive DCT.</summary>
        Sof6 = 0xC6,

        /// <summary>Start-of-Frame, differential Huffman coding frame, differential lossless.</summary>
        Sof7 = 0xC7,

        /// <summary>Start-of-Frame, non-differential arithmetic coding frame, extended sequential DCT.</summary>
        Sof9 = 0xC9,

        /// <summary>Start-of-Frame, non-differential arithmetic coding frame, progressive DCT.</summary>
        Sof10 = 0xCA,

        /// <summary>Start-of-Frame, non-differential arithmetic coding frame, lossless sequential.</summary>
        Sof11 = 0xCB,

        /// <summary>Define Arithmetic Coding table(s).</summary>
        Dac = 0xCC,

        /// <summary>Start-of-Frame, differential arithmetic coding frame, differential sequential DCT.</summary>
        Sof13 = 0xCD,

        /// <summary>Start-of-Frame, differential arithmetic coding frame, differential progressive DCT.</summary>
        Sof14 = 0xCE,

        /// <summary>Start-of-Frame, differential arithmetic coding frame, differential lossless.</summary>
        Sof15 = 0xCF,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst0 = 0xD0,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst1 = 0xD1,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst2 = 0xD2,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst3 = 0xD3,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst4 = 0xD4,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst5 = 0xD5,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst6 = 0xD6,

        /// <summary>Restart.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Rst7 = 0xD7,

        /// <summary>End-of-Image. Terminates the JPEG compressed data stream that started at <see cref="Soi"/>.</summary>
        /// <remarks>No length or parameter sequence follows this marker.</remarks>
        Eoi = 0xD9,

        /// <summary>Start-of-Scan.</summary>
        /// <remarks>
        /// Begins a top-to-bottom scan of the image.
        /// In baseline DCT JPEG images, there is generally a single scan.
        /// Progressive DCT JPEG images usually contain multiple scans.
        /// This marker specifies which slice of data it will contain, and is
        /// immediately followed by entropy-coded data.
        /// </remarks>
        Sos = 0xDA,

        /// <summary>Define Number of Lines.</summary>
        Dnl = 0xDC,

        /// <summary>Define Restart Interval.</summary>
        /// <remarks>
        /// Specifies the interval between RSTn markers, in macroblocks.
        /// This marker is followed by two bytes indicating the fixed size so
        /// it can be treated like any other variable size segment.
        /// </remarks>
        Dri = 0xDD,

        /// <summary>Define Hierarchical Progression.</summary>
        Dhp = 0xDE,

        /// <summary>Expand reference components.</summary>
        Exp = 0xDF,

        /// <summary>Application specific, type 0. Commonly contains JFIF, JFXX.</summary>
        App0 = 0xE0,

        /// <summary>Application specific, type 1. Commonly contains Exif. XMP data is also kept in here, though usually in a second instance.</summary>
        App1 = 0xE1,

        /// <summary>Application specific, type 2. Commonly contains ICC.</summary>
        App2 = 0xE2,

        /// <summary>Application specific, type 3.</summary>
        App3 = 0xE3,

        /// <summary>Application specific, type 4.</summary>
        App4 = 0xE4,

        /// <summary>Application specific, type 5.</summary>
        App5 = 0xE5,

        /// <summary>Application specific, type 6.</summary>
        App6 = 0xE6,

        /// <summary>Application specific, type 7.</summary>
        App7 = 0xE7,

        /// <summary>Application specific, type 8.</summary>
        App8 = 0xE8,

        /// <summary>Application specific, type 9.</summary>
        App9 = 0xE9,

        /// <summary>Application specific, type A. Can contain Unicode comments, though <see cref="Com"/> is more commonly used for comments.</summary>
        AppA = 0xEA,

        /// <summary>Application specific, type B.</summary>
        AppB = 0xEB,

        /// <summary>Application specific, type C.</summary>
        AppC = 0xEC,

        /// <summary>Application specific, type D. Commonly contains IPTC, Photoshop data.</summary>
        AppD = 0xED,

        /// <summary>Application specific, type E. Commonly contains Adobe data.</summary>
        AppE = 0xEE,

        /// <summary>Application specific, type F.</summary>
        AppF = 0xEF,

        /// <summary>JPEG comment (text).</summary>
        Com = 0xFE
    }

    /// <summary>
    /// Extension methods for <see cref="JpegSegmentType"/> enum.
    /// </summary>
    public static class JpegSegmentTypeExtensions
    {
        /// <summary>Gets whether this JPEG segment type might contain metadata.</summary>
        /// <remarks>Used to exclude large image-data-only segment from certain types of processing.</remarks>
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

        /// <summary>Gets JPEG segment types that might contain metadata.</summary>
#if NET35
        public static IEnumerable<JpegSegmentType> CanContainMetadataTypes { get; }
#else
        public static IReadOnlyList<JpegSegmentType> CanContainMetadataTypes { get; }
#endif
            = Enum.GetValues(typeof(JpegSegmentType)).Cast<JpegSegmentType>().Where(type => type.CanContainMetadata()).ToList();

        /// <summary>Gets whether this JPEG segment type's marker is followed by a length indicator.</summary>
        public static bool ContainsPayload(this JpegSegmentType type)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case JpegSegmentType.Soi:
                case JpegSegmentType.Eoi:
                case JpegSegmentType.Rst0:
                case JpegSegmentType.Rst1:
                case JpegSegmentType.Rst2:
                case JpegSegmentType.Rst3:
                case JpegSegmentType.Rst4:
                case JpegSegmentType.Rst5:
                case JpegSegmentType.Rst6:
                case JpegSegmentType.Rst7:
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>Gets whether this JPEG segment is intended to hold application specific data.</summary>
        public static bool IsApplicationSpecific(this JpegSegmentType type) => type >= JpegSegmentType.App0 && type <= JpegSegmentType.AppF;
    }
}
