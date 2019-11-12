// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using System;
using System.IO;

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Reads TGA image file header.</summary>
    /// <author>Dmitry Shechtman</author>
    sealed class TgaHeaderReader : TgaDirectoryReader<TgaHeaderDirectory, IndexedReader>
    {
        struct ColormapInfo
        {
            public byte type;
            public int origin;
            public int length;
            public int depth;
        }

        public const int HeaderSize = 18;

        public static readonly TgaHeaderReader Instance = new TgaHeaderReader();

        private TgaHeaderReader()
        {
        }

        public bool TryExtract(byte[] buffer, out TgaHeaderDirectory directory)
        {
            var reader = new ByteArrayReader(buffer, isMotorolaByteOrder: false);
            directory = new TgaHeaderDirectory();
            try
            {
                return PopulateHeader(reader, directory, out var idLength, out var colormap);
            }
            catch
            {
                return false;
            }
        }

        protected override IndexedReader CreateReader(Stream stream)
        {
            return new IndexedSeekingReader(stream, isMotorolaByteOrder: false);
        }

        protected override void Populate(Stream stream, int offset, IndexedReader reader, TgaHeaderDirectory directory)
        {
            PopulateHeader(reader, directory, out var idLength, out var colormapInfo);
            if (idLength > 0)
            {
                var id = reader.GetBytes(HeaderSize, idLength);
                directory.Set(TgaHeaderDirectory.TagId, id);
            }
            if (colormapInfo.type > 0)
            {
                var colormapLength = colormapInfo.length * (Math.Max(colormapInfo.depth / 3, 8) / 8);
                var colormap = reader.GetBytes(HeaderSize + idLength, colormapLength);
                directory.Set(TgaHeaderDirectory.TagColormap, colormap);
            }
        }

        private static bool PopulateHeader(IndexedReader reader, TgaHeaderDirectory directory, out int idLength, out ColormapInfo colormap)
        {
            idLength = reader.GetByte(0);
            directory.Set(TgaHeaderDirectory.TagIdLength, idLength);
            SetDataType(reader, directory);
            colormap = SetColormapInfo(reader, directory);
            SetGeometry(reader, directory);
            SetDepth(reader, directory);
            SetFlags(reader, directory);
            return true;
        }

        private static byte SetDataType(IndexedReader reader, TgaHeaderDirectory directory)
        {
            var dataType = reader.GetByte(2);
            switch (dataType)
            {
                case 1:
                case 9:
                    return SetDataTypeMapped(reader, directory, dataType);
                case 2:
                case 10:
                    return SetDataTypeTrueColor(reader, directory, dataType);
                case 3:
                case 11:
                    return SetDataTypeGrayscale(reader, directory, dataType);
                default:
                    throw new ImageProcessingException("Invalid TGA data type");
            }
        }

        private static ColormapInfo SetColormapInfo(IndexedReader reader, TgaHeaderDirectory directory)
        {
            var colormapType = reader.GetByte(1);
            switch (colormapType)
            {
                case 0:
                    return SetColormapNotIncluded(directory);
                case 1:
                    return SetColormapIncluded(reader, directory);
                default:
                    throw new ImageProcessingException("Invalid TGA color map type");
            }
        }

        private static void SetGeometry(IndexedReader reader, TgaHeaderDirectory directory)
        {
            var xOrigin = reader.GetUInt16(8);
            if (xOrigin < 0)
                throw new ImageProcessingException("Invalid TGA X-origin");
            directory.Set(TgaHeaderDirectory.TagXOrigin, xOrigin);

            var yOrigin = reader.GetUInt16(10);
            if (yOrigin < 0)
                throw new ImageProcessingException("Invalid TGA Y-origin");
            directory.Set(TgaHeaderDirectory.TagYOrigin, yOrigin);

            var width = reader.GetInt16(12);
            if (width <= 0)
                throw new ImageProcessingException("Invalid TGA image width");
            directory.Set(TgaHeaderDirectory.TagImageWidth, width);

            var height = reader.GetInt16(14);
            if (height <= 0)
                throw new ImageProcessingException("Invalid TGA image height");
            directory.Set(TgaHeaderDirectory.TagImageHeight, height);
        }

        private static byte SetDepth(IndexedReader reader, TgaHeaderDirectory directory)
        {
            var depth = reader.GetByte(16);
            switch (depth)
            {
                case 8:
                    return SetDepth8(reader, directory, depth);
                case 16:
                    return SetDepth16(reader, directory, depth);
                case 24:
                    return SetDepth24(reader, directory, depth);
                case 32:
                    return SetDepth32(reader, directory, depth);
                default:
                    throw new ImageProcessingException("Invalid TGA pixel depth");
            }
        }

        private static byte SetFlags(IndexedReader reader, TgaHeaderDirectory directory)
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

        private static byte SetDataTypeMapped(IndexedReader reader, TgaHeaderDirectory directory, byte dataType)
        {
            var colormapType = reader.GetByte(1);
            if (colormapType == 0)
                throw new ImageProcessingException("Invalid TGA data type / color map type combo");
            directory.Set(TgaHeaderDirectory.TagDataType, dataType);
            return dataType;
        }

        private static byte SetDataTypeTrueColor(IndexedReader reader, TgaHeaderDirectory directory, byte dataType)
        {
            var colormapType = reader.GetByte(1);
            if (colormapType != 0)
                throw new ImageProcessingException("Invalid TGA data type / color map type combo");
            directory.Set(TgaHeaderDirectory.TagDataType, dataType);
            return dataType;
        }

        private static byte SetDataTypeGrayscale(IndexedReader reader, TgaHeaderDirectory directory, byte dataType)
        {
            var colormapType = reader.GetByte(1);
            if (colormapType != 0)
                throw new ImageProcessingException("Invalid TGA data type / color map type combo");
            directory.Set(TgaHeaderDirectory.TagDataType, dataType);
            return dataType;
        }

        private static ColormapInfo SetColormapNotIncluded(TgaHeaderDirectory directory)
        {
            var colormap = new ColormapInfo
            {
                type = 0
            };

            directory.Set(TgaHeaderDirectory.TagColormapType, colormap.type);

            return colormap;
        }

        private static ColormapInfo SetColormapIncluded(IndexedReader reader, TgaHeaderDirectory directory)
        {
            var colormap = new ColormapInfo
            {
                type = 1,
                origin = reader.GetInt16(3),
                length = reader.GetInt16(5),
                depth = reader.GetByte(7)
            };

            directory.Set(TgaHeaderDirectory.TagColormapType, colormap.type);

            if (colormap.origin < 0)
                throw new ImageProcessingException("Invalid TGA color map origin");
            directory.Set(TgaHeaderDirectory.TagColormapOrigin, colormap.origin);

            if (colormap.length < 0)
                throw new ImageProcessingException("Invalid TGA color map length");
            directory.Set(TgaHeaderDirectory.TagColormapLength, colormap.length);

            if (colormap.depth != 15 && colormap.depth != 16 && colormap.depth != 24 && colormap.depth != 32)
                throw new ImageProcessingException("Invalid TGA color map depth");
            directory.Set(TgaHeaderDirectory.TagColormapDepth, colormap.depth);

            return colormap;
        }

        private static byte SetDepth8(IndexedReader reader, TgaHeaderDirectory directory, byte depth)
        {
            var dataType = reader.GetByte(2);
            if (dataType != 1 && dataType != 3 && dataType != 9 && dataType != 11)
                throw new ImageProcessingException("Invalid TGA data type / depth combo");
            directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
            return depth;
        }

        private static byte SetDepth16(IndexedReader reader, TgaHeaderDirectory directory, byte depth)
        {
            directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
            return depth;
        }

        private static byte SetDepth24(IndexedReader reader, TgaHeaderDirectory directory, byte depth)
        {
            var dataType = reader.GetByte(2);
            if (dataType != 2 && dataType != 10)
                throw new ImageProcessingException("Invalid TGA data type / depth combo");
            directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
            return depth;
        }

        private static byte SetDepth32(IndexedReader reader, TgaHeaderDirectory directory, byte depth)
        {
            var dataType = reader.GetByte(2);
            if (dataType != 2 && dataType != 10)
                throw new ImageProcessingException("Invalid TGA data type / depth combo");
            directory.Set(TgaHeaderDirectory.TagImageDepth, depth);
            return depth;
        }
    }
}
