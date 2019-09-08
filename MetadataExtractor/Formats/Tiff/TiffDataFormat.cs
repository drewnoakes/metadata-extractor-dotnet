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

namespace MetadataExtractor.Formats.Tiff
{
    public enum TiffDataFormatCode : ushort
    {
        Int8U = 1,
        String = 2,
        Int16U = 3,
        Int32U = 4,
        RationalU = 5,
        Int8S = 6,
        Undefined = 7,
        Int16S = 8,
        Int32S = 9,
        RationalS = 10,
        Single = 11,
        Double = 12
    }

    /// <summary>An enumeration of data formats used by the TIFF specification.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class TiffDataFormat
    {
        public static readonly TiffDataFormat Int8U     = new TiffDataFormat("BYTE",      TiffDataFormatCode.Int8U,     1);
        public static readonly TiffDataFormat String    = new TiffDataFormat("STRING",    TiffDataFormatCode.String,    1);
        public static readonly TiffDataFormat Int16U    = new TiffDataFormat("USHORT",    TiffDataFormatCode.Int16U,    2);
        public static readonly TiffDataFormat Int32U    = new TiffDataFormat("ULONG",     TiffDataFormatCode.Int32U,    4);
        public static readonly TiffDataFormat RationalU = new TiffDataFormat("URATIONAL", TiffDataFormatCode.RationalU, 8);
        public static readonly TiffDataFormat Int8S     = new TiffDataFormat("SBYTE",     TiffDataFormatCode.Int8S,     1);
        public static readonly TiffDataFormat Undefined = new TiffDataFormat("UNDEFINED", TiffDataFormatCode.Undefined, 1);
        public static readonly TiffDataFormat Int16S    = new TiffDataFormat("SSHORT",    TiffDataFormatCode.Int16S,    2);
        public static readonly TiffDataFormat Int32S    = new TiffDataFormat("SLONG",     TiffDataFormatCode.Int32S,    4);
        public static readonly TiffDataFormat RationalS = new TiffDataFormat("SRATIONAL", TiffDataFormatCode.RationalS, 8);
        public static readonly TiffDataFormat Single    = new TiffDataFormat("SINGLE",    TiffDataFormatCode.Single,    4);
        public static readonly TiffDataFormat Double    = new TiffDataFormat("DOUBLE",    TiffDataFormatCode.Double,    8);

        public static TiffDataFormat? FromTiffFormatCode(TiffDataFormatCode tiffFormatCode)
        {
            return tiffFormatCode switch
            {
                TiffDataFormatCode.Int8U => Int8U,
                TiffDataFormatCode.String => String,
                TiffDataFormatCode.Int16U => Int16U,
                TiffDataFormatCode.Int32U => Int32U,
                TiffDataFormatCode.RationalU => RationalU,
                TiffDataFormatCode.Int8S => Int8S,
                TiffDataFormatCode.Undefined => Undefined,
                TiffDataFormatCode.Int16S => Int16S,
                TiffDataFormatCode.Int32S => Int32S,
                TiffDataFormatCode.RationalS => RationalS,
                TiffDataFormatCode.Single => Single,
                TiffDataFormatCode.Double => Double,

                _ => null,
            };
        }

        public string Name { get; }
        public int ComponentSizeBytes { get; }
        public TiffDataFormatCode TiffFormatCode { get; }

        private TiffDataFormat(string name, TiffDataFormatCode tiffFormatCode, int componentSizeBytes)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TiffFormatCode = tiffFormatCode;
            ComponentSizeBytes = componentSizeBytes;
        }

        public override string ToString() => Name;
    }
}
