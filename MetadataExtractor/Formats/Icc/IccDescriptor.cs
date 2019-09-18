// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Icc
{
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class IccDescriptor : TagDescriptor<IccDirectory>
    {
        public IccDescriptor(IccDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case IccDirectory.TagProfileVersion:
                    return GetProfileVersionDescription();
                case IccDirectory.TagProfileClass:
                    return GetProfileClassDescription();
                case IccDirectory.TagPlatform:
                    return GetPlatformDescription();
                case IccDirectory.TagRenderingIntent:
                    return GetRenderingIntentDescription();
                case IccDirectory.TagProfileDateTime:
                    return GetProfileDateTimeDescription();
            }

            if (tagType > 0x20202020 && tagType < 0x7a7a7a7a)
                return GetTagDataString(tagType);

            return base.GetDescription(tagType);
        }

        private enum IccTagType
        {
            Text = 0x74657874,
            Desc = 0x64657363,
            Sig = 0x73696720,
            Meas = 0x6D656173,
            XyzArray = 0x58595A20,
            Mluc = 0x6d6c7563,
            Curv = 0x63757276
        }

        private string? GetTagDataString(int tagType)
        {
            try
            {
                var bytes = Directory.GetByteArray(tagType);
                if (bytes == null)
                    return Directory.GetString(tagType);

                var reader = new ByteArrayReader(bytes);

                var iccTagType = (IccTagType)reader.GetInt32(0);

                switch (iccTagType)
                {
                    case IccTagType.Text:
                    {
#if !NETSTANDARD1_3
                        try
                        {
                            return Encoding.ASCII.GetString(bytes, 8, bytes.Length - 8 - 1);
                        }
                        catch
#endif
                        {
                            return Encoding.UTF8.GetString(bytes, 8, bytes.Length - 8 - 1);
                        }
                    }

                    case IccTagType.Desc:
                    {
                        var stringLength = reader.GetInt32(8);
                        return Encoding.UTF8.GetString(bytes, 12, stringLength - 1);
                    }

                    case IccTagType.Sig:
                    {
                        return IccReader.GetStringFromUInt32(reader.GetUInt32(8));
                    }

                    case IccTagType.Meas:
                    {
                        var observerType = reader.GetInt32(8);
                        var x = reader.GetS15Fixed16(12);
                        var y = reader.GetS15Fixed16(16);
                        var z = reader.GetS15Fixed16(20);
                        var geometryType = reader.GetInt32(24);
                        var flare = reader.GetS15Fixed16(28);
                        var illuminantType = reader.GetInt32(32);
                            var observerString = observerType switch
                            {
                                0 => "Unknown",
                                1 => "1931 2\u00b0",
                                2 => "1964 10\u00b0",
                                _ => $"Unknown ({observerType})",
                            };
                            var geometryString = geometryType switch
                            {
                                0 => "Unknown",
                                1 => "0/45 or 45/0",
                                2 => "0/d or d/0",
                                _ => $"Unknown ({observerType})",
                            };
                            var illuminantString = illuminantType switch
                            {
                                0 => "unknown",
                                1 => "D50",
                                2 => "D65",
                                3 => "D93",
                                4 => "F2",
                                5 => "D55",
                                6 => "A",
                                7 => "Equi-Power (E)",
                                8 => "F8",
                                _ => $"Unknown ({illuminantType})",
                            };
                            return $"{observerString} Observer, Backing ({x:0.###}, {y:0.###}, {z:0.###}), Geometry {geometryString}, Flare {(long)Math.Round(flare*100)}%, Illuminant {illuminantString}";
                    }

                    case IccTagType.XyzArray:
                    {
                        var res = new StringBuilder();
                        var count = (bytes.Length - 8) / 12;

                        for (var i = 0; i < count; i++)
                        {
                            var x = reader.GetS15Fixed16(8 + i * 12);
                            var y = reader.GetS15Fixed16(8 + i * 12 + 4);
                            var z = reader.GetS15Fixed16(8 + i * 12 + 8);
                            if (i > 0)
                                res.Append(", ");
                            res.AppendFormat("({0:0.####}, {1:0.####}, {2:0.####})", x, y, z);
                        }

                        return res.ToString();
                    }

                    case IccTagType.Mluc:
                    {
                        var int1 = reader.GetInt32(8);
                        var res = new StringBuilder();
                        res.Append(int1);
                        for (var i = 0; i < int1; i++)
                        {
                            var str = IccReader.GetStringFromUInt32(reader.GetUInt32(16 + i * 12));
                            var len = reader.GetInt32(16 + i * 12 + 4);
                            var ofs = reader.GetInt32(16 + i * 12 + 8);
                            string name;
                            try
                            {
                                name = Encoding.BigEndianUnicode.GetString(bytes, ofs, len);
                            }
                            catch
                            {
                                name = Encoding.UTF8.GetString(bytes, ofs, len);
                            }
                            res.Append(" ").Append(str).Append("(").Append(name).Append(")");
                        }
                        return res.ToString();
                    }

                    case IccTagType.Curv:
                    {
                        var num = reader.GetInt32(8);
                        var res = new StringBuilder();
                        for (var i = 0; i < num; i++)
                        {
                            if (i != 0)
                                res.Append(", ");
                            res.Append(FormatDoubleAsString(reader.GetUInt16(12 + i * 2) / 65535.0, 7, false));
                        }
                        return res.ToString();
                    }

                    default:
                    {
                        return $"{IccReader.GetStringFromUInt32(unchecked((uint)iccTagType))} (0x{(int)iccTagType:X8}): {bytes.Length} bytes";
                    }
                }
            }
            catch (IOException)
            {
                // TODO decode these values during IccReader.extract so we can report any errors at that time
                // It is convention to return null if a description cannot be formulated.
                // If an error is to be reported, it should be done during the extraction process.
                return null;
            }
        }

        private static string FormatDoubleAsString(double value, int precision, bool zeroes)
        {
            if (precision < 1)
                return string.Empty + (long)Math.Round(value);

            var intPart = Math.Abs((long)value);
            long rest = (int)(long)Math.Round((Math.Abs(value) - intPart) * Math.Pow(10, precision));
            var restKept = rest;
            var res = string.Empty;
            for (var i = precision; i > 0; i--)
            {
                var cour = unchecked((byte)Math.Abs(rest % 10));
                rest /= 10;
                if (res.Length > 0 || zeroes || cour != 0 || i == 1)
                    res = cour + res;
            }

            intPart += rest;
            var isNegative = value < 0 && (intPart != 0 || restKept != 0);
            return (isNegative ? "-" : string.Empty) + intPart + "." + res;
        }

        private string? GetRenderingIntentDescription()
        {
            return GetIndexedDescription(IccDirectory.TagRenderingIntent,
                "Perceptual",
                "Media-Relative Colorimetric",
                "Saturation",
                "ICC-Absolute Colorimetric");
        }

        private string? GetPlatformDescription()
        {
            var str = Directory.GetString(IccDirectory.TagPlatform);
            if (str == null)
                return null;

            return str switch
            {
                "APPL" => "Apple Computer, Inc.",
                "MSFT" => "Microsoft Corporation",
                "SGI " => "Silicon Graphics, Inc.",
                "SUNW" => "Sun Microsystems, Inc.",
                "TGNT" => "Taligent, Inc.",
                _ => $"Unknown ({str})",
            };
        }

        private string? GetProfileClassDescription()
        {
            var str = Directory.GetString(IccDirectory.TagProfileClass);

            if (str == null)
                return null;

            return str switch
            {
                "scnr" => "Input Device",
                "mntr" => "Display Device",
                "prtr" => "Output Device",
                "link" => "DeviceLink",
                "spac" => "ColorSpace Conversion",
                "abst" => "Abstract",
                "nmcl" => "Named Color",
                _ => $"Unknown ({str})",
            };
        }

        private string? GetProfileVersionDescription()
        {
            if (!Directory.TryGetInt32(IccDirectory.TagProfileVersion, out int value))
                return null;

            var m = (byte)(value >> 24);
            var r = (byte)((value >> 20) & 0x0F);
            var R = (byte)((value >> 16) & 0x0F);
            return $"{m}.{r}.{R}";
        }

        private string? GetProfileDateTimeDescription()
        {
            if (!Directory.TryGetDateTime(IccDirectory.TagProfileDateTime, out DateTime value))
                return null;

            return value.ToString("yyyy:MM:dd HH:mm:ss");
        }
    }
}
