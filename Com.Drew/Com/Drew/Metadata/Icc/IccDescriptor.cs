/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System;
using System.IO;
using System.Text;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Icc;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Icc
{
	/// <author>Yuri Binev, Drew Noakes http://drewnoakes.com</author>
	public class IccDescriptor : TagDescriptor<IccDirectory>
	{
		public IccDescriptor(IccDirectory directory)
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
			if (tagType > unchecked((int)(0x20202020)) && tagType < unchecked((int)(0x7a7a7a7a)))
			{
				return GetTagDataString(tagType);
			}
			return base.GetDescription(tagType);
		}

		private const int IccTagTypeText = unchecked((int)(0x74657874));

		private const int IccTagTypeDesc = unchecked((int)(0x64657363));

		private const int IccTagTypeSig = unchecked((int)(0x73696720));

		private const int IccTagTypeMeas = unchecked((int)(0x6D656173));

		private const int IccTagTypeXyzArray = unchecked((int)(0x58595A20));

		private const int IccTagTypeMluc = unchecked((int)(0x6d6c7563));

		private const int IccTagTypeCurv = unchecked((int)(0x63757276));

		[CanBeNull]
		private string GetTagDataString(int tagType)
		{
			try
			{
				sbyte[] bytes = _directory.GetByteArray(tagType);
				RandomAccessReader reader = new ByteArrayReader(bytes);
				int iccTagType = reader.GetInt32(0);
				switch (iccTagType)
				{
					case IccTagTypeText:
					{
						try
						{
							return Sharpen.Runtime.GetStringForBytes(bytes, 8, bytes.Length - 8 - 1, "ASCII");
						}
						catch (UnsupportedEncodingException)
						{
							return Sharpen.Runtime.GetStringForBytes(bytes, 8, bytes.Length - 8 - 1);
						}
						goto case IccTagTypeDesc;
					}

					case IccTagTypeDesc:
					{
						int stringLength = reader.GetInt32(8);
						return Sharpen.Runtime.GetStringForBytes(bytes, 12, stringLength - 1);
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
								observerString = "1931 2В°";
								break;
							}

							case 2:
							{
								observerString = "1964 10В°";
								break;
							}

							default:
							{
								observerString = Sharpen.Extensions.StringFormat("Unknown %d", observerType);
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
								geometryString = Sharpen.Extensions.StringFormat("Unknown %d", observerType);
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
								illuminantString = Sharpen.Extensions.StringFormat("Unknown %d", illuminantType);
								break;
							}
						}
						return Sharpen.Extensions.StringFormat("%s Observer, Backing (%s, %s, %s), Geometry %s, Flare %d%%, Illuminant %s", observerString, x, y, z, geometryString, Math.Round(flare * 100), illuminantString);
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
						//System.err.format("int1: %d, int2: %d\n", int1, int2);
						for (int i = 0; i < int1; i++)
						{
							string str = IccReader.GetStringFromInt32(reader.GetInt32(16 + i * 12));
							int len = reader.GetInt32(16 + i * 12 + 4);
							int ofs = reader.GetInt32(16 + i * 12 + 8);
							string name;
							try
							{
								name = Sharpen.Runtime.GetStringForBytes(bytes, ofs, len, "UTF-16BE");
							}
							catch (UnsupportedEncodingException)
							{
								name = Sharpen.Runtime.GetStringForBytes(bytes, ofs, len);
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
							res.Append(FormatDoubleAsString(((float)reader.GetUInt16(12 + i * 2)) / 65535.0, 7, false));
						}
						//res+=String.format("%1.7g",Math.round(((float)iccReader.getInt16(b,12+i*2))/0.065535)/1E7);
						return res.ToString();
					}

					default:
					{
						return Sharpen.Extensions.StringFormat("%s(0x%08X): %d bytes", IccReader.GetStringFromInt32(iccTagType), iccTagType, bytes.Length);
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
				return string.Empty + Math.Round(value);
			}
			long intPart = Math.Abs((long)value);
			long rest = (int)Math.Round((Math.Abs(value) - intPart) * Math.Pow(10, precision));
			long restKept = rest;
			string res = string.Empty;
			sbyte cour;
			for (int i = precision; i > 0; i--)
			{
				cour = unchecked((sbyte)(Math.Abs(rest % 10)));
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
			int? value = _directory.GetInteger(IccDirectory.TagRenderingIntent);
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
					return Sharpen.Extensions.StringFormat("Unknown (%d)", value);
				}
			}
		}

		[CanBeNull]
		private string GetPlatformDescription()
		{
			string str = _directory.GetString(IccDirectory.TagPlatform);
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
				case unchecked((int)(0x4150504C)):
				{
					// "APPL"
					return "Apple Computer, Inc.";
				}

				case unchecked((int)(0x4D534654)):
				{
					// "MSFT"
					return "Microsoft Corporation";
				}

				case unchecked((int)(0x53474920)):
				{
					return "Silicon Graphics, Inc.";
				}

				case unchecked((int)(0x53554E57)):
				{
					return "Sun Microsystems, Inc.";
				}

				case unchecked((int)(0x54474E54)):
				{
					return "Taligent, Inc.";
				}

				default:
				{
					return Sharpen.Extensions.StringFormat("Unknown (%s)", str);
				}
			}
		}

		[CanBeNull]
		private string GetProfileClassDescription()
		{
			string str = _directory.GetString(IccDirectory.TagProfileClass);
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
				case unchecked((int)(0x73636E72)):
				{
					return "Input Device";
				}

				case unchecked((int)(0x6D6E7472)):
				{
					// mntr
					return "Display Device";
				}

				case unchecked((int)(0x70727472)):
				{
					return "Output Device";
				}

				case unchecked((int)(0x6C696E6B)):
				{
					return "DeviceLink";
				}

				case unchecked((int)(0x73706163)):
				{
					return "ColorSpace Conversion";
				}

				case unchecked((int)(0x61627374)):
				{
					return "Abstract";
				}

				case unchecked((int)(0x6E6D636C)):
				{
					return "Named Color";
				}

				default:
				{
					return Sharpen.Extensions.StringFormat("Unknown (%s)", str);
				}
			}
		}

		[CanBeNull]
		private string GetProfileVersionDescription()
		{
			int? value = _directory.GetInteger(IccDirectory.TagProfileVersion);
			if (value == null)
			{
				return null;
			}
			int m = (value.Value & unchecked((int)(0xFF000000))) >> 24;
			int r = (value.Value & unchecked((int)(0x00F00000))) >> 20;
			int R = (value.Value & unchecked((int)(0x000F0000))) >> 16;
			return Sharpen.Extensions.StringFormat("%d.%d.%d", m, r, R);
		}

		/// <exception cref="System.IO.IOException"/>
		private static int GetInt32FromString(string @string)
		{
			sbyte[] bytes = Sharpen.Runtime.GetBytesForString(@string);
			return new ByteArrayReader(bytes).GetInt32(0);
		}
	}
}
