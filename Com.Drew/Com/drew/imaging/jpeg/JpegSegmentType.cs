/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */
using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
    /// <summary>An enumeration of the known segment types found in JPEG files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [System.Serializable]
    public sealed class JpegSegmentType
    {
        /// <summary>APP0 JPEG segment identifier -- JFIF data (also JFXX apparently).</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App0 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE0), true);

        /// <summary>APP1 JPEG segment identifier -- where Exif data is kept.</summary>
        /// <remarks>APP1 JPEG segment identifier -- where Exif data is kept.  XMP data is also kept in here, though usually in a second instance.</remarks>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App1 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE1), true);

        /// <summary>APP2 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App2 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE2), true);

        /// <summary>APP3 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App3 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE3), true);

        /// <summary>APP4 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App4 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE4), true);

        /// <summary>APP5 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App5 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE5), true);

        /// <summary>APP6 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App6 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE6), true);

        /// <summary>APP7 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App7 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE7), true);

        /// <summary>APP8 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App8 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE8), true);

        /// <summary>APP9 JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType App9 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xE9), true);

        /// <summary>APPA (App10) JPEG segment identifier -- can hold Unicode comments.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Appa = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xEA), true);

        /// <summary>APPB (App11) JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Appb = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xEB), true);

        /// <summary>APPC (App12) JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Appc = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xEC), true);

        /// <summary>APPD (App13) JPEG segment identifier -- IPTC data in here.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Appd = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xED), true);

        /// <summary>APPE (App14) JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Appe = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xEE), true);

        /// <summary>APPF (App15) JPEG segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Appf = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xEF), true);

        /// <summary>Start Of Image segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Soi = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xD8), false);

        /// <summary>Define Quantization Table segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Dqt = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xDB), false);

        /// <summary>Define Huffman Table segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Dht = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC4), false);

        /// <summary>Start-of-Frame (0) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof0 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC0), true);

        /// <summary>Start-of-Frame (1) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof1 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC1), true);

        /// <summary>Start-of-Frame (2) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof2 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC2), true);

        /// <summary>Start-of-Frame (3) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof3 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC3), true);

        /// <summary>Start-of-Frame (5) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof5 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC5), true);

        /// <summary>Start-of-Frame (6) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof6 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC6), true);

        /// <summary>Start-of-Frame (7) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof7 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC7), true);

        /// <summary>Start-of-Frame (8) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof8 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC8), true);

        /// <summary>Start-of-Frame (9) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof9 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xC9), true);

        /// <summary>Start-of-Frame (10) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof10 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xCA), true);

        /// <summary>Start-of-Frame (11) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof11 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xCB), true);

        /// <summary>Start-of-Frame (13) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof13 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xCD), true);

        /// <summary>Start-of-Frame (14) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof14 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xCE), true);

        /// <summary>Start-of-Frame (15) segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Sof15 = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xCF), true);

        /// <summary>JPEG comment segment identifier.</summary>
        public static readonly Com.Drew.Imaging.Jpeg.JpegSegmentType Com = new Com.Drew.Imaging.Jpeg.JpegSegmentType(unchecked((sbyte)0xFE), true);

        public static readonly ICollection<Com.Drew.Imaging.Jpeg.JpegSegmentType> canContainMetadataTypes;

        static JpegSegmentType()
        {
            //    /** Start-of-Frame (4) segment identifier. */
            //    SOF4((byte)0xC4, true),
            //    /** Start-of-Frame (12) segment identifier. */
            //    SOF12((byte)0xCC, true),
            IList<Com.Drew.Imaging.Jpeg.JpegSegmentType> segmentTypes = new AList<Com.Drew.Imaging.Jpeg.JpegSegmentType>();
            foreach (Com.Drew.Imaging.Jpeg.JpegSegmentType segmentType in typeof(Com.Drew.Imaging.Jpeg.JpegSegmentType).GetEnumConstants<JpegSegmentType>())
            {
                if (segmentType.canContainMetadata)
                {
                    segmentTypes.Add(segmentType);
                }
            }
            canContainMetadataTypes = segmentTypes;
        }

        public readonly sbyte byteValue;

        public readonly bool canContainMetadata;

        internal JpegSegmentType(sbyte byteValue, bool canContainMetadata)
        {
            this.byteValue = byteValue;
            this.canContainMetadata = canContainMetadata;
        }

        [CanBeNull]
        public static Com.Drew.Imaging.Jpeg.JpegSegmentType FromByte(sbyte segmentTypeByte)
        {
            foreach (Com.Drew.Imaging.Jpeg.JpegSegmentType segmentType in typeof(Com.Drew.Imaging.Jpeg.JpegSegmentType).GetEnumConstants<JpegSegmentType>())
            {
                if (segmentType.byteValue == segmentTypeByte)
                {
                    return segmentType;
                }
            }
            return null;
        }

        public static JpegSegmentType ValueOf(string segmentName)
        {
            return Extensions.GetEnumConstantByName<JpegSegmentType>(segmentName);
        }
    }
}
