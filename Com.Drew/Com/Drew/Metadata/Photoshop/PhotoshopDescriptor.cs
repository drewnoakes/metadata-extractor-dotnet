/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
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
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Photoshop;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
	/// <author>Yuri Binev, Drew Noakes http://drewnoakes.com</author>
	public class PhotoshopDescriptor : TagDescriptor<PhotoshopDirectory>
	{
		public PhotoshopDescriptor(PhotoshopDirectory directory)
			: base(directory)
		{
		}

		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case PhotoshopDirectory.TagThumbnail:
				case PhotoshopDirectory.TagThumbnailOld:
				{
					return GetThumbnailDescription(tagType);
				}

				case PhotoshopDirectory.TagUrl:
				case PhotoshopDirectory.TagXml:
				{
					return GetSimpleString(tagType);
				}

				case PhotoshopDirectory.TagIptc:
				{
					return GetBinaryDataString(tagType);
				}

				case PhotoshopDirectory.TagSlices:
				{
					return GetSlicesDescription();
				}

				case PhotoshopDirectory.TagVersion:
				{
					return GetVersionDescription();
				}

				case PhotoshopDirectory.TagCopyright:
				{
					return GetBooleanString(tagType);
				}

				case PhotoshopDirectory.TagResolutionInfo:
				{
					return GetResolutionInfoDescription();
				}

				case PhotoshopDirectory.TagGlobalAngle:
				case PhotoshopDirectory.TagGlobalAltitude:
				case PhotoshopDirectory.TagUrlList:
				case PhotoshopDirectory.TagSeedNumber:
				{
					return Get32BitNumberString(tagType);
				}

				case PhotoshopDirectory.TagJpegQuality:
				{
					return GetJpegQualityString();
				}

				case PhotoshopDirectory.TagPrintScale:
				{
					return GetPrintScaleDescription();
				}

				case PhotoshopDirectory.TagPixelAspectRatio:
				{
					return GetPixelAspectRatioString();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetJpegQualityString()
		{
			try
			{
				sbyte[] b = _directory.GetByteArray(PhotoshopDirectory.TagJpegQuality);
				RandomAccessReader reader = new ByteArrayReader(b);
				int q = reader.GetUInt16(0);
				// & 0xFFFF;
				int f = reader.GetUInt16(2);
				// & 0xFFFF;
				int s = reader.GetUInt16(4);
				int q1;
				if (q <= unchecked((int)(0xFFFF)) && q >= unchecked((int)(0xFFFD)))
				{
					q1 = q - unchecked((int)(0xFFFC));
				}
				else
				{
					if (q <= 8)
					{
						q1 = q + 4;
					}
					else
					{
						q1 = q;
					}
				}
				string quality;
				switch (q)
				{
					case unchecked((int)(0xFFFD)):
					case unchecked((int)(0xFFFE)):
					case unchecked((int)(0xFFFF)):
					case 0:
					{
						quality = "Low";
						break;
					}

					case 1:
					case 2:
					case 3:
					{
						quality = "Medium";
						break;
					}

					case 4:
					case 5:
					{
						quality = "High";
						break;
					}

					case 6:
					case 7:
					case 8:
					{
						quality = "Maximum";
						break;
					}

					default:
					{
						quality = "Unknown";
						break;
					}
				}
				string format;
				switch (f)
				{
					case unchecked((int)(0x0000)):
					{
						format = "Standard";
						break;
					}

					case unchecked((int)(0x0001)):
					{
						format = "Optimised";
						break;
					}

					case unchecked((int)(0x0101)):
					{
						format = "Progressive ";
						break;
					}

					default:
					{
						format = Sharpen.Extensions.StringFormat("Unknown 0x%04X", f);
						break;
					}
				}
				string scans = s >= 1 && s <= 3 ? Sharpen.Extensions.StringFormat("%d", s + 2) : Sharpen.Extensions.StringFormat("Unknown 0x%04X", s);
				return Sharpen.Extensions.StringFormat("%d (%s), %s format, %s scans", q1, quality, format, scans);
			}
			catch (IOException)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetPixelAspectRatioString()
		{
			try
			{
				sbyte[] bytes = _directory.GetByteArray(PhotoshopDirectory.TagPixelAspectRatio);
				if (bytes == null)
				{
					return null;
				}
				RandomAccessReader reader = new ByteArrayReader(bytes);
				double d = reader.GetDouble64(4);
				return d.ToString();
			}
			catch (Exception)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetPrintScaleDescription()
		{
			try
			{
				sbyte[] bytes = _directory.GetByteArray(PhotoshopDirectory.TagPrintScale);
				if (bytes == null)
				{
					return null;
				}
				RandomAccessReader reader = new ByteArrayReader(bytes);
				int style = reader.GetInt32(0);
				float locX = reader.GetFloat32(2);
				float locY = reader.GetFloat32(6);
				float scale = reader.GetFloat32(10);
				switch (style)
				{
					case 0:
					{
						return "Centered, Scale " + scale;
					}

					case 1:
					{
						return "Size to fit";
					}

					case 2:
					{
						return Sharpen.Extensions.StringFormat("User defined, X:%s Y:%s, Scale:%s", locX, locY, scale);
					}

					default:
					{
						return Sharpen.Extensions.StringFormat("Unknown %04X, X:%s Y:%s, Scale:%s", style, locX, locY, scale);
					}
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetResolutionInfoDescription()
		{
			try
			{
				sbyte[] bytes = _directory.GetByteArray(PhotoshopDirectory.TagResolutionInfo);
				if (bytes == null)
				{
					return null;
				}
				RandomAccessReader reader = new ByteArrayReader(bytes);
				float resX = reader.GetS15Fixed16(0);
				float resY = reader.GetS15Fixed16(8);
				// is this the correct offset? it's only reading 4 bytes each time
				return resX + "x" + resY + " DPI";
			}
			catch (Exception)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetVersionDescription()
		{
			try
			{
				sbyte[] bytes = _directory.GetByteArray(PhotoshopDirectory.TagVersion);
				if (bytes == null)
				{
					return null;
				}
				RandomAccessReader reader = new ByteArrayReader(bytes);
				int pos = 0;
				int ver = reader.GetInt32(0);
				pos += 4;
				pos++;
				int readerLength = reader.GetInt32(5);
				pos += 4;
				string readerStr = reader.GetString(9, readerLength * 2, "UTF-16");
				pos += readerLength * 2;
				int writerLength = reader.GetInt32(pos);
				pos += 4;
				string writerStr = reader.GetString(pos, writerLength * 2, "UTF-16");
				pos += writerLength * 2;
				int fileVersion = reader.GetInt32(pos);
				return Sharpen.Extensions.StringFormat("%d (%s, %s) %d", ver, readerStr, writerStr, fileVersion);
			}
			catch (IOException)
			{
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetSlicesDescription()
		{
			try
			{
				sbyte[] bytes = _directory.GetByteArray(PhotoshopDirectory.TagSlices);
				if (bytes == null)
				{
					return null;
				}
				RandomAccessReader reader = new ByteArrayReader(bytes);
				int nameLength = reader.GetInt32(20);
				string name = reader.GetString(24, nameLength * 2, "UTF-16");
				int pos = 24 + nameLength * 2;
				int sliceCount = reader.GetInt32(pos);
				//pos += 4;
				return Sharpen.Extensions.StringFormat("%s (%d,%d,%d,%d) %d Slices", name, reader.GetInt32(4), reader.GetInt32(8), reader.GetInt32(12), reader.GetInt32(16), sliceCount);
			}
			catch (IOException)
			{
				/*for (int i=0;i<sliceCount;i++){
                pos+=16;
                int slNameLen=getInt32(b,pos);
                pos+=4;
                String slName=new String(b, pos, slNameLen*2,"UTF-16");
                res+=slName;
            }*/
				return null;
			}
		}

		[CanBeNull]
		public virtual string GetThumbnailDescription(int tagType)
		{
			try
			{
				sbyte[] v = _directory.GetByteArray(tagType);
				if (v == null)
				{
					return null;
				}
				RandomAccessReader reader = new ByteArrayReader(v);
				//int pos = 0;
				int format = reader.GetInt32(0);
				//pos += 4;
				int width = reader.GetInt32(4);
				//pos += 4;
				int height = reader.GetInt32(8);
				//pos += 4;
				//pos += 4; //skip WidthBytes
				int totalSize = reader.GetInt32(16);
				//pos += 4;
				int compSize = reader.GetInt32(20);
				//pos += 4;
				int bpp = reader.GetInt32(24);
				//pos+=2;
				//pos+=2; //skip Number of planes
				//int thumbSize=v.length-pos;
				return Sharpen.Extensions.StringFormat("%s, %dx%d, Decomp %d bytes, %d bpp, %d bytes", format == 1 ? "JpegRGB" : "RawRGB", width, height, totalSize, bpp, compSize);
			}
			catch (IOException)
			{
				return null;
			}
		}

		[CanBeNull]
		private string GetBooleanString(int tag)
		{
			sbyte[] bytes = _directory.GetByteArray(tag);
			if (bytes == null)
			{
				return null;
			}
			return bytes[0] == 0 ? "No" : "Yes";
		}

		[CanBeNull]
		private string Get32BitNumberString(int tag)
		{
			sbyte[] bytes = _directory.GetByteArray(tag);
			if (bytes == null)
			{
				return null;
			}
			RandomAccessReader reader = new ByteArrayReader(bytes);
			try
			{
				return Sharpen.Extensions.StringFormat("%d", reader.GetInt32(0));
			}
			catch (IOException)
			{
				return null;
			}
		}

		[CanBeNull]
		private string GetSimpleString(int tagType)
		{
			sbyte[] bytes = _directory.GetByteArray(tagType);
			if (bytes == null)
			{
				return null;
			}
			return Sharpen.Runtime.GetStringForBytes(bytes);
		}

		[CanBeNull]
		private string GetBinaryDataString(int tagType)
		{
			sbyte[] bytes = _directory.GetByteArray(tagType);
			if (bytes == null)
			{
				return null;
			}
			return Sharpen.Extensions.StringFormat("%d bytes binary data", bytes.Length);
		}
	}
}
