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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PhotoshopDescriptor : TagDescriptor<PhotoshopDirectory>
    {
        public PhotoshopDescriptor([NotNull] PhotoshopDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
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
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetJpegQualityString()
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

                var q1 = q <= 0xFFFF && q >= 0xFFFD
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

                string format;
                switch (f)
                {
                    case 0x0000:
                        format = "Standard";
                        break;
                    case 0x0001:
                        format = "Optimised";
                        break;
                    case 0x0101:
                        format = "Progressive";
                        break;
                    default:
                        format = $"Unknown (0x{f:X4})";
                        break;
                }

                var scans = s >= 1 && s <= 3
                    ? (s + 2).ToString()
                    : $"Unknown (0x{s:X4})";

                return $"{q1} ({quality}), {format} format, {scans} scans";
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        public string GetPixelAspectRatioString()
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

        [CanBeNull]
        public string GetPrintScaleDescription()
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

                switch (style)
                {
                    case 0:
                        return $"Centered, Scale {scale:0.0##}";
                    case 1:
                        return "Size to fit";
                    case 2:
                        return $"User defined, X:{locX} Y:{locY}, Scale:{scale:0.0##}";
                    default:
                        return $"Unknown {style:X4}, X:{locX} Y:{locY}, Scale:{scale:0.0##}";
                }
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        public string GetResolutionInfoDescription()
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

        [CanBeNull]
        public string GetVersionDescription()
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

        [CanBeNull]
        public string GetSlicesDescription()
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

        [CanBeNull]
        public string GetThumbnailDescription(int tagType)
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

        [CanBeNull]
        private string GetBooleanString(int tag)
        {
            var bytes = Directory.GetByteArray(tag);

            if (bytes == null || bytes.Length == 0)
                return null;

            return bytes[0] == 0 ? "No" : "Yes";
        }

        [CanBeNull]
        private string Get32BitNumberString(int tag)
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

        [CanBeNull]
        private string GetSimpleString(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);

            return bytes == null
                ? null
                : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        [CanBeNull]
        private string GetBinaryDataString(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);

            return bytes == null
                ? null
                : $"{bytes.Length} bytes binary data";
        }
    }
}
