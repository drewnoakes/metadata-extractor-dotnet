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
using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging;
using Com.Drew.Imaging.Bmp;
using Com.Drew.Imaging.Gif;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Imaging.Png;
using Com.Drew.Imaging.Psd;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging
{
	/// <summary>
	/// Obtains
	/// <see cref="Com.Drew.Metadata.Metadata"/>
	/// from all supported file formats.
	/// <p/>
	/// This class a lightweight wrapper around specific file type processors:
	/// <ul>
	/// <li>
	/// <see cref="Com.Drew.Imaging.Jpeg.JpegMetadataReader"/>
	/// for JPEG files</li>
	/// <li>
	/// <see cref="Com.Drew.Imaging.Tiff.TiffMetadataReader"/>
	/// for TIFF and (most) RAW files</li>
	/// <li>
	/// <see cref="Com.Drew.Imaging.Psd.PsdMetadataReader"/>
	/// for Photoshop files</li>
	/// <li>
	/// <see cref="Com.Drew.Imaging.Png.PngMetadataReader"/>
	/// for BMP files</li>
	/// <li>
	/// <see cref="Com.Drew.Imaging.Bmp.BmpMetadataReader"/>
	/// for BMP files</li>
	/// <li>
	/// <see cref="Com.Drew.Imaging.Gif.GifMetadataReader"/>
	/// for GIF files</li>
	/// </ul>
	/// If you know the file type you're working with, you may use one of the above processors directly.
	/// For most scenarios it is simpler, more convenient and more robust to use this class.
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ImageMetadataReader
	{
		private const int JpegFileMagicNumber = unchecked((int)(0xFFD8));

		private const int MotorolaTiffMagicNumber = unchecked((int)(0x4D4D));

		private const int IntelTiffMagicNumber = unchecked((int)(0x4949));

		private const int PsdMagicNumber = unchecked((int)(0x3842));

		private const int PngMagicNumber = unchecked((int)(0x8950));

		private const int BmpMagicNumber = unchecked((int)(0x424D));

		private const int GifMagicNumber = unchecked((int)(0x4749));

		// "MM"
		// "II"
		// "8B" // TODO the full magic number is 8BPS
		// "?P" // TODO the full magic number is six bytes long
		// "BM" // TODO technically there are other very rare magic numbers for OS/2 BMP files...
		// "GI" // TODO the full magic number is GIF or possibly GIF89a/GIF87a
		/// <summary>
		/// Reads metadata from an
		/// <see cref="InputStream"/>
		/// .
		/// <p/>
		/// The file type is determined by inspecting the leading bytes of the stream, and parsing of the file
		/// is delegated to one of:
		/// <ul>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Jpeg.JpegMetadataReader"/>
		/// for JPEG files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Tiff.TiffMetadataReader"/>
		/// for TIFF and (most) RAW files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Psd.PsdMetadataReader"/>
		/// for Photoshop files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Png.PngMetadataReader"/>
		/// for PNG files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Bmp.BmpMetadataReader"/>
		/// for BMP files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Gif.GifMetadataReader"/>
		/// for GIF files</li>
		/// </ul>
		/// </summary>
		/// <param name="inputStream">
		/// a stream from which the file data may be read.  The stream must be positioned at the
		/// beginning of the file's data.
		/// </param>
		/// <returns>
		/// a populated
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// object containing directories of tags with values and any processing errors.
		/// </returns>
		/// <exception cref="ImageProcessingException">if the file type is unknown, or for general processing errors.</exception>
		/// <exception cref="Com.Drew.Imaging.ImageProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(InputStream inputStream)
		{
			InputStream bufferedInputStream = inputStream is BufferedInputStream ? inputStream : new BufferedInputStream(inputStream);
			int magicNumber = PeekMagicNumber(bufferedInputStream);
			if (magicNumber == -1)
			{
				throw new ImageProcessingException("Could not determine file's magic number.");
			}
			// This covers all JPEG files
			if ((magicNumber & JpegFileMagicNumber) == JpegFileMagicNumber)
			{
				return JpegMetadataReader.ReadMetadata(bufferedInputStream);
			}
			// This covers all TIFF and camera RAW files
			if (magicNumber == IntelTiffMagicNumber || magicNumber == MotorolaTiffMagicNumber)
			{
				return TiffMetadataReader.ReadMetadata(bufferedInputStream);
			}
			// This covers PSD files
			// TODO we should really check all 4 bytes of the PSD magic number
			if (magicNumber == PsdMagicNumber)
			{
				return PsdMetadataReader.ReadMetadata(bufferedInputStream);
			}
			// This covers BMP files
			if (magicNumber == PngMagicNumber)
			{
				return PngMetadataReader.ReadMetadata(bufferedInputStream);
			}
			// This covers BMP files
			if (magicNumber == BmpMagicNumber)
			{
				return BmpMetadataReader.ReadMetadata(bufferedInputStream);
			}
			// This covers GIF files
			if (magicNumber == GifMagicNumber)
			{
				return GifMetadataReader.ReadMetadata(bufferedInputStream);
			}
			throw new ImageProcessingException("File format is not supported");
		}

		/// <summary>
		/// Reads
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// from a
		/// <see cref="Sharpen.FilePath"/>
		/// object.
		/// <p/>
		/// The file type is determined by inspecting the leading bytes of the stream, and parsing of the file
		/// is delegated to one of:
		/// <ul>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Jpeg.JpegMetadataReader"/>
		/// for JPEG files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Tiff.TiffMetadataReader"/>
		/// for TIFF and (most) RAW files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Psd.PsdMetadataReader"/>
		/// for Photoshop files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Png.PngMetadataReader"/>
		/// for PNG files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Bmp.BmpMetadataReader"/>
		/// for BMP files</li>
		/// <li>
		/// <see cref="Com.Drew.Imaging.Gif.GifMetadataReader"/>
		/// for GIF files</li>
		/// </ul>
		/// </summary>
		/// <param name="file">a file from which the image data may be read.</param>
		/// <returns>
		/// a populated
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// object containing directories of tags with values and any processing errors.
		/// </returns>
		/// <exception cref="ImageProcessingException">for general processing errors.</exception>
		/// <exception cref="Com.Drew.Imaging.ImageProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(FilePath file)
		{
			InputStream inputStream = new FileInputStream(file);
			try
			{
				return ReadMetadata(inputStream);
			}
			finally
			{
				inputStream.Close();
			}
		}

		/// <summary>Reads the first two bytes from <code>inputStream</code>, then rewinds.</summary>
		/// <exception cref="System.IO.IOException"/>
		private static int PeekMagicNumber(InputStream inputStream)
		{
			inputStream.Mark(2);
			int byte1 = inputStream.Read();
			int byte2 = inputStream.Read();
			inputStream.Reset();
			if (byte1 == -1 || byte2 == -1)
			{
				return -1;
			}
			return byte1 << 8 | byte2;
		}

		/// <exception cref="System.Exception"/>
		private ImageMetadataReader()
		{
			throw new Exception("Not intended for instantiation");
		}

		/// <summary>An application entry point.</summary>
		/// <remarks>
		/// An application entry point.  Takes the name of one or more files as arguments and prints the contents of all
		/// metadata directories to <code>System.out</code>.
		/// <p/>
		/// If <code>-thumb</code> is passed, then any thumbnail data will be written to a file with name of the
		/// input file having <code>.thumb.jpg</code> appended.
		/// <p/>
		/// If <code>-wiki</code> is passed, then output will be in a format suitable for Google Code's wiki.
		/// <p/>
		/// If <code>-hex</code> is passed, then the ID of each tag will be displayed in hexadecimal.
		/// </remarks>
		/// <param name="args">the command line arguments</param>
		/// <exception cref="Com.Drew.Metadata.MetadataException"/>
		/// <exception cref="System.IO.IOException"/>
		public static void Main(string[] args)
		{
			ICollection<string> argList = new AList<string>(Arrays.AsList(args));
			bool thumbRequested = argList.Remove("-thumb");
			bool wikiFormat = argList.Remove("-wiki");
			bool showHex = argList.Remove("-hex");
			if (argList.Count < 1)
			{
				string version = typeof(Com.Drew.Imaging.ImageMetadataReader).Assembly.GetImplementationVersion();
				System.Console.Out.Println("metadata-extractor version " + version);
				System.Console.Out.Println();
				System.Console.Out.Println(Sharpen.Extensions.StringFormat("Usage: java -jar metadata-extractor-%s.jar <filename> [<filename>] [-thumb] [-wiki] [-hex]", version == null ? "a.b.c" : version));
				System.Environment.Exit(1);
			}
			foreach (string filePath in argList)
			{
				long startTime = Runtime.NanoTime();
				FilePath file = new FilePath(filePath);
				if (!wikiFormat && argList.Count > 1)
				{
					System.Console.Out.Printf("\n***** PROCESSING: %s\n%n", filePath);
				}
				Com.Drew.Metadata.Metadata metadata = null;
				try
				{
					metadata = Com.Drew.Imaging.ImageMetadataReader.ReadMetadata(file);
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e, System.Console.Error);
					System.Environment.Exit(1);
				}
				long took = Runtime.NanoTime() - startTime;
				if (!wikiFormat)
				{
					System.Console.Out.Printf("Processed %.3f MB file in %.2f ms%n%n", file.Length() / (1024d * 1024), took / 1000000d);
				}
				if (wikiFormat)
				{
					string fileName = file.GetName();
					string urlName = StringUtil.UrlEncode(fileName);
					ExifIFD0Directory exifIFD0Directory = metadata.GetDirectory<ExifIFD0Directory>();
					string make = exifIFD0Directory == null ? string.Empty : StringUtil.EscapeForWiki(exifIFD0Directory.GetString(ExifIFD0Directory.TagMake));
					string model = exifIFD0Directory == null ? string.Empty : StringUtil.EscapeForWiki(exifIFD0Directory.GetString(ExifIFD0Directory.TagModel));
					System.Console.Out.Println();
					System.Console.Out.Println("-----");
					System.Console.Out.Println();
					System.Console.Out.Printf("= %s - %s =%n", make, model);
					System.Console.Out.Println();
					System.Console.Out.Printf("<a href=\"http://sample-images.metadata-extractor.googlecode.com/git/%s\">%n", urlName);
					System.Console.Out.Printf("<img src=\"http://sample-images.metadata-extractor.googlecode.com/git/%s\" width=\"300\"/><br/>%n", urlName);
					System.Console.Out.Println(StringUtil.EscapeForWiki(fileName));
					System.Console.Out.Println("</a>");
					System.Console.Out.Println();
					System.Console.Out.Println("|| *Directory* || *Tag Id* || *Tag Name* || *Extracted Value* ||");
				}
				// iterate over the metadata and print to System.out
				foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
				{
					string directoryName = directory.GetName();
					foreach (Tag tag in directory.GetTags())
					{
						string tagName = tag.GetTagName();
						string description = tag.GetDescription();
						// truncate the description if it's too long
						if (description != null && description.Length > 1024)
						{
							description = Sharpen.Runtime.Substring(description, 0, 1024) + "...";
						}
						if (wikiFormat)
						{
							System.Console.Out.Printf("||%s||0x%s||%s||%s||%n", StringUtil.EscapeForWiki(directoryName), Sharpen.Extensions.ToHexString(tag.GetTagType()), StringUtil.EscapeForWiki(tagName), StringUtil.EscapeForWiki(description));
						}
						else
						{
							if (showHex)
							{
								System.Console.Out.Printf("[%s - %s] %s = %s%n", directoryName, tag.GetTagTypeHex(), tagName, description);
							}
							else
							{
								System.Console.Out.Printf("[%s] %s = %s%n", directoryName, tagName, description);
							}
						}
					}
					// print out any errors
					foreach (string error in directory.GetErrors())
					{
						System.Console.Error.Println("ERROR: " + error);
					}
				}
				if (args.Length > 1 && thumbRequested)
				{
					ExifThumbnailDirectory directory_1 = metadata.GetDirectory<ExifThumbnailDirectory>();
					if (directory_1 != null && directory_1.HasThumbnailData())
					{
						System.Console.Out.Println("Writing thumbnail...");
						directory_1.WriteThumbnail(Sharpen.Extensions.Trim(args[0]) + ".thumb.jpg");
					}
					else
					{
						System.Console.Out.Println("No thumbnail data exists in this image");
					}
				}
			}
		}
	}
}
