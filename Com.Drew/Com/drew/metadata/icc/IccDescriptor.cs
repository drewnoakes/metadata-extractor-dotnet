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

using System;
using System.IO;
using System.Text;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Icc
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
                {
                    return GetProfileVersionDescription();
                }

                case IccDirectory.TagProfileClass:
                {
                    return GetProfileClassDescription();
                }

                case IccDirectory.TagPlatform:
                {
                    return GetPlatformDescription();
                }

                case IccDirectory.TagRenderingIntent:
                {
                    return GetRenderingIntentDescription();
                }
            }
            if (tagType > unchecked(0x20202020) && tagType < unchecked(0x7a7a7a7a))
            {
                return GetTagDataString(tagType);
            }
            return base.GetDescription(tagType);
        }

        private const int IccTagTypeText = unchecked(0x74657874);

        private const int IccTagTypeDesc = unchecked(0x64657363);

        private const int IccTagTypeSig = unchecked(0x73696720);

        private const int IccTagTypeMeas = unchecked(0x6D656173);

        private const int IccTagTypeXyzArray = unchecked(0x58595A20);

        private const int IccTagTypeMluc = unchecked(0x6d6c7563);

        private const int IccTagTypeCurv = unchecked(0x63757276);

        [CanBeNull]
        private string GetTagDataString(int tagType)
        {
            try
            {
                byte[] bytes = Directory.GetByteArray(tagType);
                if (bytes == null)
                {
                    return Directory.GetString(tagType);
                }
                IndexedReader reader = new ByteArrayReader(bytes);
                int iccTagType = reader.GetInt32(0);
                switch (iccTagType)
                {
                    case IccTagTypeText:
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

                    case IccTagTypeDesc:
                    {
                        int stringLength = reader.GetInt32(8);
                        return Encoding.UTF8.GetString(bytes, 12, stringLength - 1);
                    }

                    case IccTagTypeSig:
                    {
                        return IccReader.GetStringFromInt32(reader.GetInt32(8));
                    }

                    case IccTagTypeMeas:
                    {
                        int observerType = reader.GetInt32(8);
                        float x = reader.GetS15Fixed16(12);
                        float y = reader.GetS15Fixed16(16);
                        float z = reader.GetS15Fixed16(20);
                        int geometryType = reader.GetInt32(24);
                        float flare = reader.GetS15Fixed16(28);
                        int illuminantType = reader.GetInt32(32);
                        string observerString;
                        switch (observerType)
                        {
                            case 0:
                            {
                                observerString = "Unknown";
                                break;
                            }

                            case 1:
                            {
                                observerString = "1931 2°";
                                break;
                            }

                            case 2:
                            {
                                observerString = "1964 10°";
                                break;
                            }

                            default:
                            {
                                observerString = string.Format("Unknown ({0})", observerType);
                                break;
                            }
                        }
                        string geometryString;
                        switch (geometryType)
                        {
                            case 0:
                            {
                                geometryString = "Unknown";
                                break;
                            }

                            case 1:
                            {
                                geometryString = "0/45 or 45/0";
                                break;
                            }

                            case 2:
                            {
                                geometryString = "0/d or d/0";
                                break;
                            }

                            default:
                            {
                                geometryString = string.Format("Unknown ({0})", observerType);
                                break;
                            }
                        }
                        string illuminantString;
                        switch (illuminantType)
                        {
                            case 0:
                            {
                                illuminantString = "unknown";
                                break;
                            }

                            case 1:
                            {
                                illuminantString = "D50";
                                break;
                            }

                            case 2:
                            {
                                illuminantString = "D65";
                                break;
                            }

                            case 3:
                            {
                                illuminantString = "D93";
                                break;
                            }

                            case 4:
                            {
                                illuminantString = "F2";
                                break;
                            }

                            case 5:
                            {
                                illuminantString = "D55";
                                break;
                            }

                            case 6:
                            {
                                illuminantString = "A";
                                break;
                            }

                            case 7:
                            {
                                illuminantString = "Equi-Power (E)";
                                break;
                            }

                            case 8:
                            {
                                illuminantString = "F8";
                                break;
                            }

                            default:
                            {
                                illuminantString = string.Format("Unknown ({0})", illuminantType);
                                break;
                            }
                        }
                        return string.Format("{0} Observer, Backing ({1}, {2}, {3}), Geometry {4}, Flare {5}%, Illuminant {6}", observerString, x, y, z, geometryString, (long)Math.Round(flare * 100), illuminantString);
                    }

                    case IccTagTypeXyzArray:
                    {
                        StringBuilder res = new StringBuilder();
                        int count = (bytes.Length - 8) / 12;
                        for (int i = 0; i < count; i++)
                        {
                            float x = reader.GetS15Fixed16(8 + i * 12);
                            float y = reader.GetS15Fixed16(8 + i * 12 + 4);
                            float z = reader.GetS15Fixed16(8 + i * 12 + 8);
                            if (i > 0)
                            {
                                res.Append(", ");
                            }
                            res.Append("(").Append(x).Append(", ").Append(y).Append(", ").Append(z).Append(")");
                        }
                        return res.ToString();
                    }

                    case IccTagTypeMluc:
                    {
                        int int1 = reader.GetInt32(8);
                        StringBuilder res = new StringBuilder();
                        res.Append(int1);
                        //int int2 = reader.getInt32(12);
                        //Console.Error.WriteLine("int1: {0}, int2: {1}", int1, int2);
                        for (int i = 0; i < int1; i++)
                        {
                            string str = IccReader.GetStringFromInt32(reader.GetInt32(16 + i * 12));
                            int len = reader.GetInt32(16 + i * 12 + 4);
                            int ofs = reader.GetInt32(16 + i * 12 + 8);
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

                    case IccTagTypeCurv:
                    {
                        int num = reader.GetInt32(8);
                        StringBuilder res = new StringBuilder();
                        for (int i = 0; i < num; i++)
                        {
                            if (i != 0)
                            {
                                res.Append(", ");
                            }
                            res.Append(FormatDoubleAsString(reader.GetUInt16(12 + i * 2) / 65535.0, 7, false));
                        }
                        //res+=String.format("%1.7g",Math.round(((float)iccReader.getInt16(b,12+i*2))/0.065535)/1E7);
                        return res.ToString();
                    }

                    default:
                    {
                        return string.Format("{0} (0x{1:X8}): {2} bytes", IccReader.GetStringFromInt32(iccTagType), iccTagType, bytes.Length);
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
        public static string FormatDoubleAsString(double value, int precision, bool zeroes)
        {
            if (precision < 1)
            {
                return string.Empty + (long)Math.Round(value);
            }
            long intPart = Math.Abs((long)value);
            long rest = (int)(long)Math.Round((Math.Abs(value) - intPart) * Math.Pow(10, precision));
            long restKept = rest;
            string res = string.Empty;
            byte cour;
            for (int i = precision; i > 0; i--)
            {
                cour = unchecked((byte)(Math.Abs(rest % 10)));
                rest /= 10;
                if (res.Length > 0 || zeroes || cour != 0 || i == 1)
                {
                    res = cour + res;
                }
            }
            intPart += rest;
            bool isNegative = ((value < 0) && (intPart != 0 || restKept != 0));
            return (isNegative ? "-" : string.Empty) + intPart + "." + res;
        }

        [CanBeNull]
        private string GetRenderingIntentDescription()
        {
            int? value = Directory.GetInteger(IccDirectory.TagRenderingIntent);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Perceptual";
                }

                case 1:
                {
                    return "Media-Relative Colorimetric";
                }

                case 2:
                {
                    return "Saturation";
                }

                case 3:
                {
                    return "ICC-Absolute Colorimetric";
                }

                default:
                {
                    return string.Format("Unknown ({0})", value);
                }
            }
        }

        [CanBeNull]
        private string GetPlatformDescription()
        {
            string str = Directory.GetString(IccDirectory.TagPlatform);
            if (str == null)
            {
                return null;
            }
            // Because Java doesn't allow switching on string values, create an integer from the first four chars
            // and switch on that instead.
            int i;
            try
            {
                i = GetInt32FromString(str);
            }
            catch (IOException)
            {
                return str;
            }
            switch (i)
            {
                case unchecked(0x4150504C):
                {
                    // "APPL"
                    return "Apple Computer, Inc.";
                }

                case unchecked(0x4D534654):
                {
                    // "MSFT"
                    return "Microsoft Corporation";
                }

                case unchecked(0x53474920):
                {
                    return "Silicon Graphics, Inc.";
                }

                case unchecked(0x53554E57):
                {
                    return "Sun Microsystems, Inc.";
                }

                case unchecked(0x54474E54):
                {
                    return "Taligent, Inc.";
                }

                default:
                {
                    return string.Format("Unknown ({0})", str);
                }
            }
        }

        [CanBeNull]
        private string GetProfileClassDescription()
        {
            string str = Directory.GetString(IccDirectory.TagProfileClass);
            if (str == null)
            {
                return null;
            }
            // Because Java doesn't allow switching on string values, create an integer from the first four chars
            // and switch on that instead.
            int i;
            try
            {
                i = GetInt32FromString(str);
            }
            catch (IOException)
            {
                return str;
            }
            switch (i)
            {
                case unchecked(0x73636E72):
                {
                    return "Input Device";
                }

                case unchecked(0x6D6E7472):
                {
                    // mntr
                    return "Display Device";
                }

                case unchecked(0x70727472):
                {
                    return "Output Device";
                }

                case unchecked(0x6C696E6B):
                {
                    return "DeviceLink";
                }

                case unchecked(0x73706163):
                {
                    return "ColorSpace Conversion";
                }

                case unchecked(0x61627374):
                {
                    return "Abstract";
                }

                case unchecked(0x6E6D636C):
                {
                    return "Named Color";
                }

                default:
                {
                    return string.Format("Unknown ({0})", str);
                }
            }
        }

        [CanBeNull]
        private string GetProfileVersionDescription()
        {
            int? value = Directory.GetInteger(IccDirectory.TagProfileVersion);
            if (value == null)
            {
                return null;
            }
            int m = ((int)value & unchecked((int)(0xFF000000))) >> 24;
            int r = ((int)value & unchecked(0x00F00000)) >> 20;
            int R = ((int)value & unchecked(0x000F0000)) >> 16;
            return string.Format("{0}.{1}.{2}", m, r, R);
        }

        /// <exception cref="System.IO.IOException"/>
        private static int GetInt32FromString([NotNull] string @string)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(@string);
            return new ByteArrayReader(bytes).GetInt32(0);
        }
    }
}
