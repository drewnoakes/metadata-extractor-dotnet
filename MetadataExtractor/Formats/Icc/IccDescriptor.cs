#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.IO;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Icc
{
    /// <author>Yuri Binev</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class IccDescriptor : TagDescriptor<IccDirectory>
    {
        public IccDescriptor([NotNull] IccDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
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
            }

            if (tagType > 0x20202020 && tagType < 0x7a7a7a7a)
                return GetTagDataString(tagType);

            return base.GetDescription(tagType);
        }

        private enum IccTagType : int
        {
            Text = 0x74657874,
            Desc = 0x64657363,
            Sig = 0x73696720,
            Meas = 0x6D656173,
            XyzArray = 0x58595A20,
            Mluc = 0x6d6c7563,
            Curv = 0x63757276
        }

        [CanBeNull]
        private string GetTagDataString(int tagType)
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
                        try
                        {
                            return Encoding.ASCII.GetString(bytes, 8, bytes.Length - 8 - 1);
                        }
                        catch
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

                        string observerString;
                        switch (observerType)
                        {
                            case 0:
                                observerString = "Unknown";
                                break;
                            case 1:
                                observerString = "1931 2°";
                                break;
                            case 2:
                                observerString = "1964 10°";
                                break;
                            default:
                                observerString = string.Format("Unknown ({0})", observerType);
                                break;
                        }

                        string geometryString;
                        switch (geometryType)
                        {
                            case 0:
                                geometryString = "Unknown";
                                break;
                            case 1:
                                geometryString = "0/45 or 45/0";
                                break;
                            case 2:
                                geometryString = "0/d or d/0";
                                break;
                            default:
                                geometryString = string.Format("Unknown ({0})", observerType);
                                break;
                        }

                        string illuminantString;
                        switch (illuminantType)
                        {
                            case 0:
                                illuminantString = "unknown";
                                break;
                            case 1:
                                illuminantString = "D50";
                                break;
                            case 2:
                                illuminantString = "D65";
                                break;
                            case 3:
                                illuminantString = "D93";
                                break;
                            case 4:
                                illuminantString = "F2";
                                break;
                            case 5:
                                illuminantString = "D55";
                                break;
                            case 6:
                                illuminantString = "A";
                                break;
                            case 7:
                                illuminantString = "Equi-Power (E)";
                                break;
                            case 8:
                                illuminantString = "F8";
                                break;
                            default:
                                illuminantString = string.Format("Unknown ({0})", illuminantType);
                                break;
                        }

                        return string.Format("{0} Observer, Backing ({1}, {2}, {3}), Geometry {4}, Flare {5}%, Illuminant {6}",
                            observerString, x, y, z, geometryString, (long)Math.Round(flare * 100), illuminantString);
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
                            res.Append("(").Append(x).Append(", ").Append(y).Append(", ").Append(z).Append(")");
                        }

                        return res.ToString();
                    }

                    case IccTagType.Mluc:
                    {
                        var int1 = reader.GetInt32(8);
                        var res = new StringBuilder();
                        res.Append(int1);
                        //int int2 = reader.getInt32(12);
                        //Console.Error.WriteLine("int1: {0}, int2: {1}", int1, int2);
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
                        //System.err.format("% 3d: %s, len: %d, ofs: %d, \"%s\"\n", i, str, len,ofs,name);
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
                        //res+=String.format("%1.7g",Math.round(((float)iccReader.getInt16(b,12+i*2))/0.065535)/1E7);
                        return res.ToString();
                    }

                    default:
                    {
                        return string.Format("{0} (0x{1:X8}): {2} bytes",
                            IccReader.GetStringFromUInt32(unchecked((uint)iccTagType)),
                            iccTagType,
                            bytes.Length);
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

        [NotNull]
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
                var cour = unchecked((byte)(Math.Abs(rest % 10)));
                rest /= 10;
                if (res.Length > 0 || zeroes || cour != 0 || i == 1)
                    res = cour + res;
            }

            intPart += rest;
            var isNegative = ((value < 0) && (intPart != 0 || restKept != 0));
            return (isNegative ? "-" : string.Empty) + intPart + "." + res;
        }

        [CanBeNull]
        private string GetRenderingIntentDescription()
        {
            var value = Directory.GetInt32Nullable(IccDirectory.TagRenderingIntent);
            if (value == null)
                return null;

            switch (value)
            {
                case 0:
                    return "Perceptual";
                case 1:
                    return "Media-Relative Colorimetric";
                case 2:
                    return "Saturation";
                case 3:
                    return "ICC-Absolute Colorimetric";
                default:
                    return string.Format("Unknown ({0})", value);
            }
        }

        [CanBeNull]
        private string GetPlatformDescription()
        {
            var str = Directory.GetString(IccDirectory.TagPlatform);
            if (str == null)
                return null;

            switch (str)
            {
                case "APPL":
                    return "Apple Computer, Inc.";
                case "MSFT":
                    return "Microsoft Corporation";
                case "SGI ":
                    return "Silicon Graphics, Inc.";
                case "SUNW":
                    return "Sun Microsystems, Inc.";
                case "TGNT":
                    return "Taligent, Inc.";
                default:
                    return string.Format("Unknown ({0})", str);
            }
        }

        [CanBeNull]
        private string GetProfileClassDescription()
        {
            var str = Directory.GetString(IccDirectory.TagProfileClass);

            if (str == null)
                return null;

            switch (str)
            {
                case "scnr":
                    return "Input Device";
                case "mntr":
                    return "Display Device";
                case "prtr":
                    return "Output Device";
                case "link":
                    return "DeviceLink";
                case "spac":
                    return "ColorSpace Conversion";
                case "abst":
                    return "Abstract";
                case "nmcl":
                    return "Named Color";
                default:
                    return string.Format("Unknown ({0})", str);
            }
        }

        [CanBeNull]
        private string GetProfileVersionDescription()
        {
            var value = Directory.GetInt32Nullable(IccDirectory.TagProfileVersion);

            if (value == null)
                return null;

            var m = (byte)(value >> 24);
            var r = (byte)((value >> 20) & 0x0F);
            var R = (byte)((value >> 16) & 0x0F);
            return string.Format("{0}.{1}.{2}", m, r, R);
        }
    }
}
