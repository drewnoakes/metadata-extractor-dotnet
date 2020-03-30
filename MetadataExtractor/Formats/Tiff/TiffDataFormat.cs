// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
        Double = 12,

        // From BigTIFF
        Int64U = 16,
        Int64S = 17,
        Ifd8 = 18
    }

    /// <summary>
    /// An enumeration of data formats used by the TIFF specification.
    /// </summary>
    public sealed class TiffDataFormat
    {
#pragma warning disable format
        public static readonly TiffDataFormat Int8U     = new("BYTE",      TiffDataFormatCode.Int8U,     1);
        public static readonly TiffDataFormat String    = new("STRING",    TiffDataFormatCode.String,    1);
        public static readonly TiffDataFormat Int16U    = new("USHORT",    TiffDataFormatCode.Int16U,    2);
        public static readonly TiffDataFormat Int32U    = new("ULONG",     TiffDataFormatCode.Int32U,    4);
        public static readonly TiffDataFormat RationalU = new("URATIONAL", TiffDataFormatCode.RationalU, 8);
        public static readonly TiffDataFormat Int8S     = new("SBYTE",     TiffDataFormatCode.Int8S,     1);
        public static readonly TiffDataFormat Undefined = new("UNDEFINED", TiffDataFormatCode.Undefined, 1);
        public static readonly TiffDataFormat Int16S    = new("SSHORT",    TiffDataFormatCode.Int16S,    2);
        public static readonly TiffDataFormat Int32S    = new("SLONG",     TiffDataFormatCode.Int32S,    4);
        public static readonly TiffDataFormat RationalS = new("SRATIONAL", TiffDataFormatCode.RationalS, 8);
        public static readonly TiffDataFormat Single    = new("SINGLE",    TiffDataFormatCode.Single,    4);
        public static readonly TiffDataFormat Double    = new("DOUBLE",    TiffDataFormatCode.Double,    8);

        // From BigTIFF
        public static readonly TiffDataFormat Int64U    = new("ULONG8",    TiffDataFormatCode.Int64U,    8);
        public static readonly TiffDataFormat Int64S    = new("SLONG8",    TiffDataFormatCode.Int64S,    8);
        public static readonly TiffDataFormat Ifd8      = new("IFD8",      TiffDataFormatCode.Ifd8,      8);
#pragma warning restore format

        public static TiffDataFormat? FromTiffFormatCode(TiffDataFormatCode tiffFormatCode, bool isBigTiff)
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

                // From BigTIFF
                TiffDataFormatCode.Int64U => isBigTiff ? Int64U : null,
                TiffDataFormatCode.Int64S => isBigTiff ? Int64S : null,
                TiffDataFormatCode.Ifd8 => isBigTiff ? Ifd8 : null,

                _ => null,
            };
        }

        public string Name { get; }

        public byte ComponentSizeBytes { get; }

        public TiffDataFormatCode TiffFormatCode { get; }

        private TiffDataFormat(string name, TiffDataFormatCode tiffFormatCode, byte componentSizeBytes)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TiffFormatCode = tiffFormatCode;
            ComponentSizeBytes = componentSizeBytes;
        }

        public override string ToString() => Name;
    }
}
