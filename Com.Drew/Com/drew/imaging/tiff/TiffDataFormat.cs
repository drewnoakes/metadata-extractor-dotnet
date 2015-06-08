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

using JetBrains.Annotations;

namespace Com.Drew.Imaging.Tiff
{
    /// <summary>An enumeration of data formats used by the TIFF specification.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class TiffDataFormat
    {
        public const int CodeInt8U = 1;

        public const int CodeString = 2;

        public const int CodeInt16U = 3;

        public const int CodeInt32U = 4;

        public const int CodeRationalU = 5;

        public const int CodeInt8S = 6;

        public const int CodeUndefined = 7;

        public const int CodeInt16S = 8;

        public const int CodeInt32S = 9;

        public const int CodeRationalS = 10;

        public const int CodeSingle = 11;

        public const int CodeDouble = 12;

        [NotNull]
        public static readonly TiffDataFormat Int8U = new TiffDataFormat("BYTE", CodeInt8U, 1);

        [NotNull]
        public static readonly TiffDataFormat String = new TiffDataFormat("STRING", CodeString, 1);

        [NotNull]
        public static readonly TiffDataFormat Int16U = new TiffDataFormat("USHORT", CodeInt16U, 2);

        [NotNull]
        public static readonly TiffDataFormat Int32U = new TiffDataFormat("ULONG", CodeInt32U, 4);

        [NotNull]
        public static readonly TiffDataFormat RationalU = new TiffDataFormat("URATIONAL", CodeRationalU, 8);

        [NotNull]
        public static readonly TiffDataFormat Int8S = new TiffDataFormat("SBYTE", CodeInt8S, 1);

        [NotNull]
        public static readonly TiffDataFormat Undefined = new TiffDataFormat("UNDEFINED", CodeUndefined, 1);

        [NotNull]
        public static readonly TiffDataFormat Int16S = new TiffDataFormat("SSHORT", CodeInt16S, 2);

        [NotNull]
        public static readonly TiffDataFormat Int32S = new TiffDataFormat("SLONG", CodeInt32S, 4);

        [NotNull]
        public static readonly TiffDataFormat RationalS = new TiffDataFormat("SRATIONAL", CodeRationalS, 8);

        [NotNull]
        public static readonly TiffDataFormat Single = new TiffDataFormat("SINGLE", CodeSingle, 4);

        [NotNull]
        public static readonly TiffDataFormat Double = new TiffDataFormat("DOUBLE", CodeDouble, 8);

        [NotNull]
        private readonly string _name;

        private readonly int _tiffFormatCode;

        private readonly int _componentSizeBytes;

        [CanBeNull]
        public static TiffDataFormat FromTiffFormatCode(int tiffFormatCode)
        {
            switch (tiffFormatCode)
            {
                case 1:
                {
                    return Int8U;
                }

                case 2:
                {
                    return String;
                }

                case 3:
                {
                    return Int16U;
                }

                case 4:
                {
                    return Int32U;
                }

                case 5:
                {
                    return RationalU;
                }

                case 6:
                {
                    return Int8S;
                }

                case 7:
                {
                    return Undefined;
                }

                case 8:
                {
                    return Int16S;
                }

                case 9:
                {
                    return Int32S;
                }

                case 10:
                {
                    return RationalS;
                }

                case 11:
                {
                    return Single;
                }

                case 12:
                {
                    return Double;
                }
            }
            return null;
        }

        private TiffDataFormat([NotNull] string name, int tiffFormatCode, int componentSizeBytes)
        {
            _name = name;
            _tiffFormatCode = tiffFormatCode;
            _componentSizeBytes = componentSizeBytes;
        }

        public int GetComponentSizeBytes()
        {
            return _componentSizeBytes;
        }

        public int GetTiffFormatCode()
        {
            return _tiffFormatCode;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
