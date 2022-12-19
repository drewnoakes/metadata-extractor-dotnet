// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Reads TGA image file header.</summary>
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaHeaderReader : TgaDirectoryReader<TgaHeaderDirectory>
    {
        private readonly struct ColormapInfo
        {
            public byte Type { get; }
            public int Origin { get; }
            public int Length { get; }
            public int Depth { get; }

            public ColormapInfo(byte type, int origin, int length, int depth)
            {
                Type = type;
                Origin = origin;
                Length = length;
                Depth = depth;
            }
        }

        public const int HeaderSize = 18;

        public static bool TryExtract(byte[] buffer, out TgaHeaderDirectory? directory)
        {
            var reader = new ByteArrayReader(buffer, isMotorolaByteOrder: false);
            directory = new TgaHeaderDirectory();
            try
            {
                return PopulateHeader(reader, directory, out _, out _);
            }
            catch
            {
                directory = null;
                return false;
            }
        }

        protected override void Populate(Stream stream, int offset, TgaHeaderDirectory directory)
        {
            var reader = new IndexedSeekingReader(stream, isMotorolaByteOrder: false);

            PopulateHeader(reader, directory, out var idLength, out var colormapInfo);

            if (idLength > 0)
            {
                var id = reader.GetBytes(HeaderSize, idLength);
                directory.Set(TgaHeaderDirectory.TagId, id);
            }

            if (colormapInfo.Type > 0)
            {
                var colormapLength = colormapInfo.Length * (Math.Max(colormapInfo.Depth / 3, 8) / 8);
                var colormap = reader.GetBytes(HeaderSize + idLength, colormapLength);
                directory.Set(TgaHeaderDirectory.TagColormap, colormap);
            }
        }

        private static bool PopulateHeader(IndexedReader reader, TgaHeaderDirectory directory, out int idLength, out ColormapInfo colormap)
        {
            idLength = reader.GetByte(0);
            directory.Set(TgaHeaderDirectory.TagIdLength, idLength);
            SetDataType();
            colormap = SetColormapInfo();
            SetGeometry();
            SetDepth();
            SetFlags();
            return true;

            byte SetDataType()
            {
                var dataType = reader.GetByte(2);
                return dataType switch
                {
                    1 => SetDataTypeMapped(),
                    9 => SetDataTypeMapped(),
                    2 => SetDataTypeTrueColor(),
                    10 => SetDataTypeTrueColor(),
                    3 => SetDataTypeGrayscale(),
                    11 => SetDataTypeGrayscale(),
                    _ => throw new ImageProcessingException("Invalid TGA data type"),
                };

                byte SetDataTypeMapped()
                {
                    var colormapType = reader.GetByte(1);
                    if (colormapType == 0)
                        throw new ImageProcessingException("Invalid TGA data type / color map type combo");
                    directory.Set(TgaHeaderDirectory.TagDataType, dataType);
                    return dataType;
                }

                byte SetDataTypeTrueColor()
                {
                    var colormapType = reader.GetByte(1);
                    if (colormapType != 0)
                        throw new ImageProcessingException("Invalid TGA data type / color map type combo");
                    directory.Set(TgaHeaderDirectory.TagDataType, dataType);
                    return dataType;
                }

                byte SetDataTypeGrayscale()
                {
                    var colormapType = reader.GetByte(1);
                    if (colormapType != 0)
                        throw new ImageProcessingException("Invalid TGA data type / color map type combo");
                    directory.Set(TgaHeaderDirectory.TagDataType, dataType);
                    return dataType;
                }
            }

            ColormapInfo SetColormapInfo()
            {
                var colormapType = reader.GetByte(1);
                return colormapType switch
                {
                    0 => SetColormapNotIncluded(),
                    1 => SetColormapIncluded(),
                    _ => throw new ImageProcessingException("Invalid TGA color map type"),
                };

                ColormapInfo SetColormapNotIncluded()
                {
                    var colormap = new ColormapInfo(type: 0, default, default, default);

                    directory.Set(TgaHeaderDirectory.TagColormapType, colormap.Type);

                    return colormap;
                }

                ColormapInfo SetColormapIncluded()
                {
                    var colormap = new ColormapInfo(
                        type: 1,
                        origin: reader.GetInt16(3),
                        length: reader.GetInt16(5),
                        depth: reader.GetByte(7));

                    directory.Set(TgaHeaderDirectory.TagColormapType, colormap.Type);

                    if (colormap.Origin < 0)
                        throw new ImageProcessingException("Invalid TGA color map origin");
                    directory.Set(TgaHeaderDirectory.TagColormapOrigin, colormap.Origin);

                    if (colormap.Length < 0)
                        throw new ImageProcessingException("Invalid TGA color map length");
                    directory.Set(TgaHeaderDirectory.TagColormapLength, colormap.Length);

                    if (colormap.Depth != 15 && colormap.Depth != 16 && colormap.Depth != 24 && colormap.Depth != 32)
                        throw new ImageProcessingException("Invalid TGA color map depth");
                    directory.Set(TgaHeaderDirectory.TagColormapDepth, colormap.Depth);

                    return colormap;
                }
            }

            void SetGeometry()
            {
                directory.Set(TgaHeaderDirectory.TagXOrigin, reader.GetUInt16(8));
                directory.Set(TgaHeaderDirectory.TagYOrigin, reader.GetUInt16(10));

                var width = reader.GetInt16(12);
                if (width <= 0)
                    throw new ImageProcessingException("Invalid TGA image width");
                directory.Set(TgaHeaderDirectory.TagImageWidth, width);

                var height = reader.GetInt16(14);
                if (height <= 0)
                    throw new ImageProcessingException("Invalid TGA image height");
                directory.Set(TgaHeaderDirectory.TagImageHeight, height);
            }

            byte SetDepth()
            {
                var depth = reader.GetByte(16);
                return depth switch
                {
                    8 => SetDepth8(),
                    16 => SetDepth16(),
                    24 => SetDepth24(),
                    32 => SetDepth32(),
                    _ => throw new ImageProcessingException("Invalid TGA pixel depth"),
                };

                byte SetDepth8()
                {
                    var dataType = reader.GetByte(2);
                    if (dataType != 1 && dataType != 3 && dataType != 9 && dataType != 11)
                        throw new ImageProcessingException("Invalid TGA data type / depth combo");
                    directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
                    return depth;
                }

                byte SetDepth16()
                {
                    directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
                    return depth;
                }

                byte SetDepth24()
                {
                    var dataType = reader.GetByte(2);
                    if (dataType != 2 && dataType != 10)
                        throw new ImageProcessingException("Invalid TGA data type / depth combo");
                    directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
                    return depth;
                }

                byte SetDepth32()
                {
                    var dataType = reader.GetByte(2);
                    if (dataType != 2 && dataType != 10)
                        throw new ImageProcessingException("Invalid TGA data type / depth combo");
                    directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
                    return depth;
                }
            }

            byte SetFlags()
            {
                var flags = reader.GetByte(17);
                if ((flags & 0xC0) != 0)
                    throw new ImageProcessingException("Invalid TGA image flags");

                var abpp = flags & 0x0F;
                if (abpp > 0)
                {
                    if (abpp != 6 && abpp != 8)
                        throw new ImageProcessingException("Invalid TGA attribute bits per pixel");
                    directory.Set(TgaHeaderDirectory.TagAttributeBitsPerPixel, abpp);
                }

                var horizontalOrder = (flags & 0x10) != 0;
                directory.Set(TgaHeaderDirectory.TagHorizontalOrder, horizontalOrder);

                var verticalOrder = (flags & 0x20) != 0;
                directory.Set(TgaHeaderDirectory.TagVerticalOrder, verticalOrder);

                return flags;
            }
        }
    }
}
