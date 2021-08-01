// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PhotoshopDescriptor : TagDescriptor<PhotoshopDirectory>
    {
        public PhotoshopDescriptor(PhotoshopDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PhotoshopDirectory.TagThumbnail:
                case PhotoshopDirectory.TagThumbnailOld:
                    return GetThumbnailDescription(tagType);
                case PhotoshopDirectory.TagUrl:
                case PhotoshopDirectory.TagXml:
                    return GetSimpleString(tagType);
                case PhotoshopDirectory.TagIptc:
                    return GetBinaryDataString(tagType);
                case PhotoshopDirectory.TagSlices:
                    return GetSlicesDescription();
                case PhotoshopDirectory.TagVersion:
                    return GetVersionDescription();
                case PhotoshopDirectory.TagCopyright:
                    return GetBooleanString(tagType);
                case PhotoshopDirectory.TagResolutionInfo:
                    return GetResolutionInfoDescription();
                case PhotoshopDirectory.TagGlobalAngle:
                case PhotoshopDirectory.TagGlobalAltitude:
                case PhotoshopDirectory.TagUrlList:
                case PhotoshopDirectory.TagSeedNumber:
                    return Get32BitNumberString(tagType);
                case PhotoshopDirectory.TagJpegQuality:
                    return GetJpegQualityString();
                case PhotoshopDirectory.TagPrintScale:
                    return GetPrintScaleDescription();
                case PhotoshopDirectory.TagPixelAspectRatio:
                    return GetPixelAspectRatioString();
                case PhotoshopDirectory.TagClippingPathName:
                    return GetClippingPathNameString(tagType);
                default:
                    if (tagType is >= PhotoshopDirectory.TagClippingPathBlockStart and <= PhotoshopDirectory.TagClippingPathBlockEnd)
                        return GetPathString(tagType);
                    return base.GetDescription(tagType);
            }
        }

        public string? GetJpegQualityString()
        {
            try
            {
                var b = Directory.GetByteArray(PhotoshopDirectory.TagJpegQuality);

                if (b == null)
                    return Directory.GetString(PhotoshopDirectory.TagJpegQuality);

                var reader = new ByteArrayReader(b);

                int q = reader.GetUInt16(0);
                int f = reader.GetUInt16(2);
                int s = reader.GetUInt16(4);

                var q1 = q is >= 0xFFFD and <= 0xFFFF
                    ? q - 0xFFFC
                    : q <= 8
                        ? q + 4
                        : q;

                string quality;
                switch (q)
                {
                    case 0xFFFD:
                    case 0xFFFE:
                    case 0xFFFF:
                    case 0:
                        quality = "Low";
                        break;
                    case 1:
                    case 2:
                    case 3:
                        quality = "Medium";
                        break;
                    case 4:
                    case 5:
                        quality = "High";
                        break;
                    case 6:
                    case 7:
                    case 8:
                        quality = "Maximum";
                        break;
                    default:
                        quality = "Unknown";
                        break;
                }
                var format = f switch
                {
                    0x0000 => "Standard",
                    0x0001 => "Optimised",
                    0x0101 => "Progressive",
                    _ => $"Unknown (0x{f:X4})",
                };
                var scans = s is >= 1 and <= 3
                    ? (s + 2).ToString()
                    : $"Unknown (0x{s:X4})";

                return $"{q1} ({quality}), {format} format, {scans} scans";
            }
            catch
            {
                return null;
            }
        }

        public string? GetPixelAspectRatioString()
        {
            try
            {
                var bytes = Directory.GetByteArray(PhotoshopDirectory.TagPixelAspectRatio);

                if (bytes == null)
                    return null;

                var reader = new ByteArrayReader(bytes);
                var d = reader.GetDouble64(4);
                return d.ToString("0.0##");
            }
            catch
            {
                return null;
            }
        }

        public string? GetPrintScaleDescription()
        {
            try
            {
                var bytes = Directory.GetByteArray(PhotoshopDirectory.TagPrintScale);

                if (bytes == null)
                    return null;

                var reader = new ByteArrayReader(bytes);
                var style = reader.GetInt32(0);
                var locX = reader.GetFloat32(2);
                var locY = reader.GetFloat32(6);
                var scale = reader.GetFloat32(10);

                return style switch
                {
                    0 => $"Centered, Scale {scale:0.0##}",
                    1 => "Size to fit",
                    2 => $"User defined, X:{locX} Y:{locY}, Scale:{scale:0.0##}",
                    _ => $"Unknown {style:X4}, X:{locX} Y:{locY}, Scale:{scale:0.0##}",
                };
            }
            catch
            {
                return null;
            }
        }

        public string? GetResolutionInfoDescription()
        {
            try
            {
                var bytes = Directory.GetByteArray(PhotoshopDirectory.TagResolutionInfo);

                if (bytes == null)
                    return null;

                var reader = new ByteArrayReader(bytes);

                var resX = reader.GetS15Fixed16(0);
                var resY = reader.GetS15Fixed16(8);

                // is this the correct offset? it's only reading 4 bytes each time
                return $"{resX:0.##}x{resY:0.##} DPI";
            }
            catch
            {
                return null;
            }
        }

        public string? GetVersionDescription()
        {
            try
            {
                var bytes = Directory.GetByteArray(PhotoshopDirectory.TagVersion);

                if (bytes == null)
                    return null;

                var reader = new ByteArrayReader(bytes);

                var pos = 0;
                var ver = reader.GetInt32(0);
                pos += 4;
                pos++;
                var readerLength = reader.GetInt32(5);
                pos += 4;
                var readerStr = reader.GetString(9, readerLength * 2, Encoding.BigEndianUnicode);
                pos += readerLength * 2;
                var writerLength = reader.GetInt32(pos);
                pos += 4;
                var writerStr = reader.GetString(pos, writerLength * 2, Encoding.BigEndianUnicode);
                pos += writerLength * 2;
                var fileVersion = reader.GetInt32(pos);

                return $"{ver} ({readerStr}, {writerStr}) {fileVersion}";
            }
            catch
            {
                return null;
            }
        }

        public string? GetSlicesDescription()
        {
            try
            {
                var bytes = Directory.GetByteArray(PhotoshopDirectory.TagSlices);

                if (bytes == null)
                    return null;

                var reader = new ByteArrayReader(bytes);

                var nameLength = reader.GetInt32(20);
                var name = reader.GetString(24, nameLength * 2, Encoding.BigEndianUnicode);
                var pos = 24 + nameLength * 2;
                var sliceCount = reader.GetInt32(pos);
                return $"{name} ({reader.GetInt32(4)},{reader.GetInt32(8)},{reader.GetInt32(12)},{reader.GetInt32(16)}) {sliceCount} Slices";
            }
            catch
            {
                return null;
            }
        }

        public string? GetThumbnailDescription(int tagType)
        {
            try
            {
                var v = Directory.GetByteArray(tagType);

                if (v == null)
                    return null;

                var reader = new ByteArrayReader(v);
                var format = reader.GetInt32(0);
                var width = reader.GetInt32(4);
                var height = reader.GetInt32(8);
                // skip WidthBytes
                var totalSize = reader.GetInt32(16);
                var compSize = reader.GetInt32(20);
                var bpp = reader.GetInt32(24);
                // skip Number of planes

                return $"{(format == 1 ? "JpegRGB" : "RawRGB")}, {width}x{height}, Decomp {totalSize} bytes, {bpp} bpp, {compSize} bytes";
            }
            catch
            {
                return null;
            }
        }

        private string? GetBooleanString(int tag)
        {
            var bytes = Directory.GetByteArray(tag);

            if (bytes == null || bytes.Length == 0)
                return null;

            return bytes[0] == 0 ? "No" : "Yes";
        }

        private string? Get32BitNumberString(int tag)
        {
            var bytes = Directory.GetByteArray(tag);

            if (bytes == null)
                return null;

            var reader = new ByteArrayReader(bytes);

            try
            {
                return $"{reader.GetInt32(0)}";
            }
            catch
            {
                return null;
            }
        }

        private string? GetSimpleString(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);

            return bytes == null
                ? null
                : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        private string? GetBinaryDataString(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);

            return bytes == null
                ? null
                : $"{bytes.Length} bytes binary data";
        }

        private string? GetClippingPathNameString(int tagType)
        {
            try
            {
                var bytes = Directory.GetByteArray(tagType);
                if (bytes == null)
                    return null;
                var reader = new ByteArrayReader(bytes);
                int length = reader.GetByte(0);
                return Encoding.UTF8.GetString(reader.GetBytes(1, length));
            }
            catch
            {
                return null;
            }
        }

        public string? GetPathString(int tagType)
        {
            try
            {
                var bytes = Directory.GetByteArray(tagType);
                if (bytes == null)
                    return null;
                var reader = new ByteArrayReader(bytes);
                int length = (int)(reader.Length - reader.GetByte((int)reader.Length - 1) - 1) / 26;

                string? fillRecord = null;

                // Possible subpaths
                var cSubpath = new Subpath();
                var oSubpath = new Subpath();

                var paths = new List<Subpath>();

                // Loop through each path resource block segment (26-bytes)
                for (int i = 0; i < length; i++)
                {
                    // Spacer takes into account which block is currently being worked on while accessing byte array
                    int recordSpacer = 26 * i;
                    int selector = reader.GetInt16(recordSpacer);

                    /*
                     * Subpath resource blocks come in 26-byte segments with 9 possible selectors - some selectors
                     * are formatted different from others
                     *
                     *      0 = Closed subpath length record
                     *      1 = Closed subpath Bezier knot, linked
                     *      2 = Closed subpath Bezier knot, unlinked
                     *      3 = Open subpath length record
                     *      4 = Open subpath Bezier knot, linked
                     *      5 = Open subpath Bezier knot, unlinked
                     *      6 = Subpath fill rule record
                     *      7 = Clipboard record
                     *      8 = Initial fill rule record
                     *
                     * Source: http://www.adobe.com/devnet-apps/photoshop/fileformatashtml/
                     */
                    switch (selector)
                    {
                        case 0:
                            // Insert previous Paths if there are any
                            if (cSubpath.KnotCount != 0)
                            {
                                paths.Add(cSubpath);
                            }

                            // Make path size accordingly
                            cSubpath = new Subpath("Closed Subpath");
                            break;
                        case 1:
                        case 2:
                        {
                            Knot knot;
                            if (selector == 1)
                                knot = new Knot("Linked");
                            else
                                knot = new Knot("Unlinked");
                            // Insert each point into cSubpath - points are 32-bit signed, fixed point numbers and have 8-bits before the point
                            for (int j = 0; j < 6; j++)
                            {
                                knot[j] = reader.GetByte((j * 4) + 2 + recordSpacer) + (reader.GetInt24((j * 4) + 3 + recordSpacer) / Math.Pow(2.0, 24.0));
                            }
                            cSubpath.Add(knot);
                            break;
                        }
                        case 3:
                            // Insert previous Paths if there are any
                            if (oSubpath.KnotCount != 0)
                            {
                                paths.Add(oSubpath);
                            }

                            // Make path size accordingly
                            oSubpath = new Subpath("Open Subpath");
                            break;
                        case 4:
                        case 5:
                        {
                            Knot knot;
                            if (selector == 4)
                                knot = new Knot("Linked");
                            else
                                knot = new Knot("Unlinked");
                            // Insert each point into oSubpath - points are 32-bit signed, fixed point numbers and have 8-bits before the point
                            for (int j = 0; j < 6; j++)
                            {
                                knot[j] = reader.GetByte((j * 4) + 2 + recordSpacer) + (reader.GetInt24((j * 4) + 3 + recordSpacer) / Math.Pow(2.0, 24.0));
                            }
                            oSubpath.Add(knot);
                            break;
                        }
                        case 6:
                            break;
                        case 7:
                            // TODO: Clipboard record
                            //                        for (int j = 0; j < 24; j++) {
                            //                           clipboardRecord[j] = bytes[j + 2 + recordSpacer];
                            //                        }
                            break;
                        case 8:
                            if (reader.GetInt16(2 + recordSpacer) == 1)
                                fillRecord = "with all pixels";
                            else
                                fillRecord = "without all pixels";
                            break;
                    }
                }

                // Add any more paths that were not added already
                if (cSubpath.KnotCount != 0)
                    paths.Add(cSubpath);
                if (oSubpath.KnotCount != 0)
                    paths.Add(oSubpath);

                // Extract name (previously appended to end of byte array)
                int nameLength = reader.GetByte((int)reader.Length - 1);
                var name = reader.GetString((int)reader.Length - nameLength - 1, nameLength, Encoding.ASCII);

                // Build description
                var str = new StringBuilder();

                str.Append($"\"{name}\" having ");
                if (fillRecord != null)
                    str.Append($"initial fill rule \"{fillRecord}\" and ");

                str.Append(paths.Count).Append(paths.Count == 1 ? " subpath:" : " subpaths:");

                foreach (Subpath path in paths)
                {
                    str.Append($"\n- {path.Type} with { paths.Count}").Append(paths.Count == 1 ? " knot:" : " knots:");

                    foreach (Knot knot in path.Knots)
                    {
                        str.Append($"\n  - {knot.Type}");
                        str.Append($" ({knot[0]},{knot[1]})");
                        str.Append($" ({knot[2]},{knot[3]})");
                        str.Append($" ({knot[4]},{knot[5]})");
                    }
                }

                return str.ToString();
            }
            catch
            {
                return null;
            }
        }


    }
}
