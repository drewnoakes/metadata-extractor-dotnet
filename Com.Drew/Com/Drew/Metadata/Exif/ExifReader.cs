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
using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Decodes Exif binary data, populating a
	/// <see cref="Com.Drew.Metadata.Metadata"/>
	/// object with tag values in
	/// <see cref="ExifSubIFDDirectory"/>
	/// ,
	/// <see cref="ExifThumbnailDirectory"/>
	/// ,
	/// <see cref="ExifInteropDirectory"/>
	/// ,
	/// <see cref="GpsDirectory"/>
	/// and one of the many camera makernote directories.
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifReader : JpegSegmentMetadataReader
	{
		/// <summary>The number of bytes used per format descriptor.</summary>
		[NotNull]
		private static readonly int[] BytesPerFormat = new int[] { 0, 1, 1, 2, 4, 8, 1, 1, 2, 4, 8, 4, 8 };

		/// <summary>The number of formats known.</summary>
		private const int MaxFormatCode = 12;

		/// <summary>An 8-bit unsigned integer.</summary>
		private const int FmtByte = 1;

		/// <summary>A fixed-length character string.</summary>
		private const int FmtString = 2;

		/// <summary>An unsigned 16-bit integer.</summary>
		private const int FmtUshort = 3;

		/// <summary>An unsigned 32-bit integer.</summary>
		private const int FmtUlong = 4;

		private const int FmtUrational = 5;

		/// <summary>An 8-bit signed integer.</summary>
		private const int FmtSbyte = 6;

		private const int FmtUndefined = 7;

		/// <summary>A signed 16-bit integer.</summary>
		private const int FmtSshort = 8;

		/// <summary>A signed 32-bit integer.</summary>
		private const int FmtSlong = 9;

		private const int FmtSrational = 10;

		/// <summary>A 32-bit floating point number.</summary>
		private const int FmtSingle = 11;

		/// <summary>A 64-bit floating point number.</summary>
		private const int FmtDouble = 12;

		/// <summary>The offset at which the TIFF data actually starts.</summary>
		/// <remarks>
		/// The offset at which the TIFF data actually starts. This may be necessary when, for example, processing
		/// JPEG Exif data from APP0 which has a 6-byte preamble before starting the TIFF data.
		/// </remarks>
		private const string JpegExifSegmentPreamble = "Exif\x0\x0";

		private bool _storeThumbnailBytes = true;

		// Format types
		// TODO use an enum for these?
		public virtual bool IsStoreThumbnailBytes()
		{
			return _storeThumbnailBytes;
		}

		public virtual void SetStoreThumbnailBytes(bool storeThumbnailBytes)
		{
			_storeThumbnailBytes = storeThumbnailBytes;
		}

		[NotNull]
		public virtual Iterable<JpegSegmentType> GetSegmentTypes()
		{
			return Arrays.AsList(JpegSegmentType.App1).AsIterable();
		}

		public virtual bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType)
		{
			return segmentBytes.Length >= JpegExifSegmentPreamble.Length && Sharpen.Runtime.EqualsIgnoreCase(Sharpen.Runtime.GetStringForBytes(segmentBytes, 0, JpegExifSegmentPreamble.Length), JpegExifSegmentPreamble);
		}

		public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
		{
			if (segmentBytes == null)
			{
				throw new ArgumentNullException("segmentBytes cannot be null");
			}
			if (metadata == null)
			{
				throw new ArgumentNullException("metadata cannot be null");
			}
			if (segmentType == null)
			{
				throw new ArgumentNullException("segmentType cannot be null");
			}
			try
			{
				ByteArrayReader reader = new ByteArrayReader(segmentBytes);
				//
				// Check for the header preamble
				//
				try
				{
					if (!reader.GetString(0, JpegExifSegmentPreamble.Length).Equals(JpegExifSegmentPreamble))
					{
						// TODO what do to with this error state?
						System.Console.Error.Println("Invalid JPEG Exif segment preamble");
						return;
					}
				}
				catch (IOException e)
				{
					// TODO what do to with this error state?
					Sharpen.Runtime.PrintStackTrace(e, System.Console.Error);
					return;
				}
				//
				// Read the TIFF-formatted Exif data
				//
				new TiffReader().ProcessTiff(reader, new ExifTiffHandler(metadata, _storeThumbnailBytes), JpegExifSegmentPreamble.Length);
			}
			catch (TiffProcessingException e)
			{
				// TODO what do to with this error state?
				Sharpen.Runtime.PrintStackTrace(e, System.Console.Error);
			}
			catch (IOException e)
			{
				// TODO what do to with this error state?
				Sharpen.Runtime.PrintStackTrace(e, System.Console.Error);
			}
		}

		/// <summary>
		/// Performs the Exif data extraction on a TIFF/RAW, adding found values to the specified
		/// instance of
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// .
		/// </summary>
		/// <param name="reader">
		/// The
		/// <see cref="Com.Drew.Lang.RandomAccessReader"/>
		/// from which TIFF data should be read.
		/// </param>
		/// <param name="metadata">The Metadata object into which extracted values should be merged.</param>
		[Obsolete]
		public virtual void ExtractTiff(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata)
		{
			ExifIFD0Directory directory = metadata.GetOrCreateDirectory<ExifIFD0Directory>();
			try
			{
				ExtractTiff(reader, metadata, directory, 0);
			}
			catch (IOException e)
			{
				directory.AddError("IO problem: " + e.Message);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		[Obsolete]
		private static void ExtractTiff(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata, Com.Drew.Metadata.Directory firstDirectory, int tiffHeaderOffset)
		{
			// this should be either "MM" or "II"
			string byteOrderIdentifier = reader.GetString(tiffHeaderOffset, 2);
			if ("MM".Equals(byteOrderIdentifier))
			{
				reader.SetMotorolaByteOrder(true);
			}
			else
			{
				if ("II".Equals(byteOrderIdentifier))
				{
					reader.SetMotorolaByteOrder(false);
				}
				else
				{
					firstDirectory.AddError("Unclear distinction between Motorola/Intel byte ordering: " + byteOrderIdentifier);
					return;
				}
			}
			// Check the next two values for correctness.
			int tiffMarker = reader.GetUInt16(2 + tiffHeaderOffset);
			int standardTiffMarker = unchecked((int)(0x002A));
			int olympusRawTiffMarker = unchecked((int)(0x4F52));
			// for ORF files
			int panasonicRawTiffMarker = unchecked((int)(0x0055));
			// for RW2 files
			if (tiffMarker != standardTiffMarker && tiffMarker != olympusRawTiffMarker && tiffMarker != panasonicRawTiffMarker)
			{
				firstDirectory.AddError("Unexpected TIFF marker after byte order identifier: 0x" + Sharpen.Extensions.ToHexString(tiffMarker));
				return;
			}
			int firstIfdOffset = reader.GetInt32(4 + tiffHeaderOffset) + tiffHeaderOffset;
			// David Ekholm sent a digital camera image that has this problem
			// TODO getLength should be avoided as it causes RandomAccessStreamReader to read to the end of the stream
			if (firstIfdOffset >= reader.GetLength() - 1)
			{
				firstDirectory.AddError("First Exif directory offset is beyond end of Exif data segment");
				// First directory normally starts 14 bytes in -- try it here and catch another error in the worst case
				firstIfdOffset = 14;
			}
			ICollection<int> processedIfdOffsets = new HashSet<int>();
			ProcessIFD(firstDirectory, processedIfdOffsets, firstIfdOffset, tiffHeaderOffset, metadata, reader);
			// after the extraction process, if we have the correct tags, we may be able to store thumbnail information
			ExifThumbnailDirectory thumbnailDirectory = metadata.GetDirectory<ExifThumbnailDirectory>();
			if (thumbnailDirectory != null && thumbnailDirectory.ContainsTag(ExifThumbnailDirectory.TagThumbnailCompression))
			{
				int? offset = thumbnailDirectory.GetInteger(ExifThumbnailDirectory.TagThumbnailOffset);
				int? length = thumbnailDirectory.GetInteger(ExifThumbnailDirectory.TagThumbnailLength);
				if (offset != null && length != null)
				{
					try
					{
						sbyte[] thumbnailData = reader.GetBytes(tiffHeaderOffset + offset.Value, length.Value);
						thumbnailDirectory.SetThumbnailData(thumbnailData);
					}
					catch (IOException ex)
					{
						firstDirectory.AddError("Invalid thumbnail data specification: " + ex.Message);
					}
				}
			}
		}

		/// <summary>
		/// Processes a TIFF IFD, storing tag values in the specified
		/// <see cref="Com.Drew.Metadata.Directory"/>
		/// .
		/// <p/>
		/// IFD Header:
		/// <ul>
		/// <li><b>2 bytes</b> number of tags</li>
		/// </ul>
		/// Tag structure:
		/// <ul>
		/// <li><b>2 bytes</b> tag type</li>
		/// <li><b>2 bytes</b> format code (values 1 to 12, inclusive)</li>
		/// <li><b>4 bytes</b> component count</li>
		/// <li><b>4 bytes</b> inline value, or offset pointer if too large to fit in four bytes</li>
		/// </ul>
		/// </summary>
		/// <param name="directory">
		/// the
		/// <see cref="Com.Drew.Metadata.Directory"/>
		/// to write extracted values into
		/// </param>
		/// <param name="processedIfdOffsets">the set of visited IFD offsets, to avoid revisiting the same IFD in an endless loop</param>
		/// <param name="ifdOffset">the offset within <code>reader</code> at which the IFD data starts</param>
		/// <param name="tiffHeaderOffset">the offset within <code>reader</code> at which the TIFF header starts</param>
		/// <exception cref="System.IO.IOException"/>
		[Obsolete]
		private static void ProcessIFD(Com.Drew.Metadata.Directory directory, ICollection<int> processedIfdOffsets, int ifdOffset, int tiffHeaderOffset, Com.Drew.Metadata.Metadata metadata, RandomAccessReader reader)
		{
			// check for directories we've already visited to avoid stack overflows when recursive/cyclic directory structures exist
			if (processedIfdOffsets.Contains(Sharpen.Extensions.ValueOf(ifdOffset)))
			{
				return;
			}
			// remember that we've visited this directory so that we don't visit it again later
			processedIfdOffsets.Add(ifdOffset);
			if (ifdOffset >= reader.GetLength() || ifdOffset < 0)
			{
				directory.AddError("Ignored IFD marked to start outside data segment");
				return;
			}
			// First two bytes in the IFD are the number of tags in this directory
			int dirTagCount = reader.GetUInt16(ifdOffset);
			int dirLength = (2 + (12 * dirTagCount) + 4);
			if (dirLength + ifdOffset > reader.GetLength())
			{
				directory.AddError("Illegally sized IFD");
				return;
			}
			// Handle each tag in this directory
			for (int tagNumber = 0; tagNumber < dirTagCount; tagNumber++)
			{
				int tagOffset = CalculateTagOffset(ifdOffset, tagNumber);
				// 2 bytes for the tag type
				int tagType = reader.GetUInt16(tagOffset);
				// 2 bytes for the format code
				int formatCode = reader.GetUInt16(tagOffset + 2);
				if (formatCode < 1 || formatCode > MaxFormatCode)
				{
					// This error suggests that we are processing at an incorrect index and will generate
					// rubbish until we go out of bounds (which may be a while).  Exit now.
					directory.AddError("Invalid TIFF tag format code: " + formatCode);
					return;
				}
				// 4 bytes dictate the number of components in this tag's data
				int componentCount = reader.GetInt32(tagOffset + 4);
				if (componentCount < 0)
				{
					directory.AddError("Negative TIFF tag component count");
					continue;
				}
				// each component may have more than one byte... calculate the total number of bytes
				int byteCount = componentCount * BytesPerFormat[formatCode];
				int tagValueOffset;
				if (byteCount > 4)
				{
					// If it's bigger than 4 bytes, the dir entry contains an offset.
					// dirEntryOffset must be passed, as some makernote implementations (e.g. Fujifilm) incorrectly use an
					// offset relative to the start of the makernote itself, not the TIFF segment.
					int offsetVal = reader.GetInt32(tagOffset + 8);
					if (offsetVal + byteCount > reader.GetLength())
					{
						// Bogus pointer offset and / or byteCount value
						directory.AddError("Illegal TIFF tag pointer offset");
						continue;
					}
					tagValueOffset = tiffHeaderOffset + offsetVal;
				}
				else
				{
					// 4 bytes or less and value is in the dir entry itself
					tagValueOffset = tagOffset + 8;
				}
				if (tagValueOffset < 0 || tagValueOffset > reader.GetLength())
				{
					directory.AddError("Illegal TIFF tag pointer offset");
					continue;
				}
				// Check that this tag isn't going to allocate outside the bounds of the data array.
				// This addresses an uncommon OutOfMemoryError.
				if (byteCount < 0 || tagValueOffset + byteCount > reader.GetLength())
				{
					directory.AddError("Illegal number of bytes for TIFF tag data: " + byteCount);
					continue;
				}
				//
				// Special handling for certain known tags that point to or contain other chunks of data to be processed
				//
				if (tagType == ExifIFD0Directory.TagExifSubIfdOffset && directory is ExifIFD0Directory)
				{
					if (byteCount != 4)
					{
						directory.AddError("Exif SubIFD Offset tag should have a component count of four (bytes) for the offset.");
					}
					else
					{
						int subDirOffset = tiffHeaderOffset + reader.GetInt32(tagValueOffset);
						ProcessIFD(metadata.GetOrCreateDirectory<ExifSubIFDDirectory>(), processedIfdOffsets, subDirOffset, tiffHeaderOffset, metadata, reader);
					}
				}
				else
				{
					if (tagType == ExifSubIFDDirectory.TagInteropOffset && directory is ExifSubIFDDirectory)
					{
						if (byteCount != 4)
						{
							directory.AddError("Exif Interop Offset tag should have a component count of four (bytes) for the offset.");
						}
						else
						{
							int subDirOffset = tiffHeaderOffset + reader.GetInt32(tagValueOffset);
							ProcessIFD(metadata.GetOrCreateDirectory<ExifInteropDirectory>(), processedIfdOffsets, subDirOffset, tiffHeaderOffset, metadata, reader);
						}
					}
					else
					{
						if (tagType == ExifIFD0Directory.TagGpsInfoOffset && directory is ExifIFD0Directory)
						{
							if (byteCount != 4)
							{
								directory.AddError("Exif GPS Info Offset tag should have a component count of four (bytes) for the offset.");
							}
							else
							{
								int subDirOffset = tiffHeaderOffset + reader.GetInt32(tagValueOffset);
								ProcessIFD(metadata.GetOrCreateDirectory<GpsDirectory>(), processedIfdOffsets, subDirOffset, tiffHeaderOffset, metadata, reader);
							}
						}
						else
						{
							if (tagType == ExifSubIFDDirectory.TagMakernote && directory is ExifSubIFDDirectory)
							{
								// The makernote tag contains the encoded makernote data directly.
								// Pass the offset to this tag's value. Manufacturer/Model-specific logic will be used to
								// determine the correct offset for further processing.
								ProcessMakernote(tagValueOffset, processedIfdOffsets, tiffHeaderOffset, metadata, reader);
							}
							else
							{
								ProcessTag(directory, tagType, tagValueOffset, componentCount, formatCode, reader);
							}
						}
					}
				}
			}
			// at the end of each IFD is an optional link to the next IFD
			int finalTagOffset = CalculateTagOffset(ifdOffset, dirTagCount);
			int nextDirectoryOffset = reader.GetInt32(finalTagOffset);
			if (nextDirectoryOffset != 0)
			{
				nextDirectoryOffset += tiffHeaderOffset;
				if (nextDirectoryOffset >= reader.GetLength())
				{
					// Last 4 bytes of IFD reference another IFD with an address that is out of bounds
					// Note this could have been caused by jhead 1.3 cropping too much
					return;
				}
				else
				{
					if (nextDirectoryOffset < ifdOffset)
					{
						// Last 4 bytes of IFD reference another IFD with an address that is before the start of this directory
						return;
					}
				}
				// TODO in Exif, the only known 'follower' IFD is the thumbnail one, however this may not be the case
				ExifThumbnailDirectory nextDirectory = metadata.GetOrCreateDirectory<ExifThumbnailDirectory>();
				ProcessIFD(nextDirectory, processedIfdOffsets, nextDirectoryOffset, tiffHeaderOffset, metadata, reader);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		[Obsolete]
		private static void ProcessMakernote(int makernoteOffset, ICollection<int> processedIfdOffsets, int tiffHeaderOffset, Com.Drew.Metadata.Metadata metadata, RandomAccessReader reader)
		{
			// Determine the camera model and makernote format
			Com.Drew.Metadata.Directory ifd0Directory = metadata.GetDirectory<ExifIFD0Directory>();
			if (ifd0Directory == null)
			{
				return;
			}
			string cameraMake = ifd0Directory.GetString(ExifIFD0Directory.TagMake);
			string firstThreeChars = reader.GetString(makernoteOffset, 3);
			string firstFourChars = reader.GetString(makernoteOffset, 4);
			string firstFiveChars = reader.GetString(makernoteOffset, 5);
			string firstSixChars = reader.GetString(makernoteOffset, 6);
			string firstSevenChars = reader.GetString(makernoteOffset, 7);
			string firstEightChars = reader.GetString(makernoteOffset, 8);
			string firstTwelveChars = reader.GetString(makernoteOffset, 12);
			bool byteOrderBefore = reader.IsMotorolaByteOrder();
			if ("OLYMP".Equals(firstFiveChars) || "EPSON".Equals(firstFiveChars) || "AGFA".Equals(firstFourChars))
			{
				// Olympus Makernote
				// Epson and Agfa use Olympus makernote standard: http://www.ozhiker.com/electronics/pjmt/jpeg_info/
				ProcessIFD(metadata.GetOrCreateDirectory<OlympusMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset, metadata, reader);
			}
			else
			{
				if (cameraMake != null && Sharpen.Extensions.Trim(cameraMake).ToUpper().StartsWith("NIKON"))
				{
					if ("Nikon".Equals(firstFiveChars))
					{
						switch (reader.GetUInt8(makernoteOffset + 6))
						{
							case 1:
							{
								/* There are two scenarios here:
                 * Type 1:                  **
                 * :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
                 * :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
                 * Type 3:                  **
                 * :0000: 4E 69 6B 6F 6E 00 02 00-00 00 4D 4D 00 2A 00 00 Nikon....MM.*...
                 * :0010: 00 08 00 1E 00 01 00 07-00 00 00 04 30 32 30 30 ............0200
                 */
								ProcessIFD(metadata.GetOrCreateDirectory<NikonType1MakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset, metadata, reader);
								break;
							}

							case 2:
							{
								ProcessIFD(metadata.GetOrCreateDirectory<NikonType2MakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 18, makernoteOffset + 10, metadata, reader);
								break;
							}

							default:
							{
								ifd0Directory.AddError("Unsupported Nikon makernote data ignored.");
								break;
							}
						}
					}
					else
					{
						// The IFD begins with the first Makernote byte (no ASCII name).  This occurs with CoolPix 775, E990 and D1 models.
						ProcessIFD(metadata.GetOrCreateDirectory<NikonType2MakernoteDirectory>(), processedIfdOffsets, makernoteOffset, tiffHeaderOffset, metadata, reader);
					}
				}
				else
				{
					if ("SONY CAM".Equals(firstEightChars) || "SONY DSC".Equals(firstEightChars))
					{
						ProcessIFD(metadata.GetOrCreateDirectory<SonyType1MakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 12, tiffHeaderOffset, metadata, reader);
					}
					else
					{
						if ("SEMC MS\u0000\u0000\u0000\u0000\u0000".Equals(firstTwelveChars))
						{
							// force MM for this directory
							reader.SetMotorolaByteOrder(true);
							// skip 12 byte header + 2 for "MM" + 6
							ProcessIFD(metadata.GetOrCreateDirectory<SonyType6MakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 20, tiffHeaderOffset, metadata, reader);
						}
						else
						{
							if ("SIGMA\u0000\u0000\u0000".Equals(firstEightChars) || "FOVEON\u0000\u0000".Equals(firstEightChars))
							{
								ProcessIFD(metadata.GetOrCreateDirectory<SigmaMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 10, tiffHeaderOffset, metadata, reader);
							}
							else
							{
								if ("KDK".Equals(firstThreeChars))
								{
									reader.SetMotorolaByteOrder(firstSevenChars.Equals("KDK INFO"));
									ProcessKodakMakernote(metadata.GetOrCreateDirectory<KodakMakernoteDirectory>(), makernoteOffset, reader);
								}
								else
								{
									if (Sharpen.Runtime.EqualsIgnoreCase("Canon", cameraMake))
									{
										ProcessIFD(metadata.GetOrCreateDirectory<CanonMakernoteDirectory>(), processedIfdOffsets, makernoteOffset, tiffHeaderOffset, metadata, reader);
									}
									else
									{
										if (cameraMake != null && cameraMake.ToUpper().StartsWith("CASIO"))
										{
											if ("QVC\u0000\u0000\u0000".Equals(firstSixChars))
											{
												ProcessIFD(metadata.GetOrCreateDirectory<CasioType2MakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 6, tiffHeaderOffset, metadata, reader);
											}
											else
											{
												ProcessIFD(metadata.GetOrCreateDirectory<CasioType1MakernoteDirectory>(), processedIfdOffsets, makernoteOffset, tiffHeaderOffset, metadata, reader);
											}
										}
										else
										{
											if ("FUJIFILM".Equals(firstEightChars) || Sharpen.Runtime.EqualsIgnoreCase("Fujifilm", cameraMake))
											{
												// Note that this also applies to certain Leica cameras, such as the Digilux-4.3
												reader.SetMotorolaByteOrder(false);
												// the 4 bytes after "FUJIFILM" in the makernote point to the start of the makernote
												// IFD, though the offset is relative to the start of the makernote, not the TIFF
												// header (like everywhere else)
												int ifdStart = makernoteOffset + reader.GetInt32(makernoteOffset + 8);
												ProcessIFD(metadata.GetOrCreateDirectory<FujifilmMakernoteDirectory>(), processedIfdOffsets, ifdStart, makernoteOffset, metadata, reader);
											}
											else
											{
												if (cameraMake != null && cameraMake.ToUpper().StartsWith("MINOLTA"))
												{
													// Cases seen with the model starting with MINOLTA in capitals seem to have a valid Olympus makernote
													// area that commences immediately.
													ProcessIFD(metadata.GetOrCreateDirectory<OlympusMakernoteDirectory>(), processedIfdOffsets, makernoteOffset, tiffHeaderOffset, metadata, reader);
												}
												else
												{
													if ("KYOCERA".Equals(firstSevenChars))
													{
														// http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
														ProcessIFD(metadata.GetOrCreateDirectory<KyoceraMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 22, tiffHeaderOffset, metadata, reader);
													}
													else
													{
														if ("LEICA".Equals(firstFiveChars))
														{
															reader.SetMotorolaByteOrder(false);
															if ("Leica Camera AG".Equals(cameraMake))
															{
																ProcessIFD(metadata.GetOrCreateDirectory<LeicaMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset, metadata, reader);
															}
															else
															{
																if ("LEICA".Equals(cameraMake))
																{
																	// Some Leica cameras use Panasonic makernote tags
																	ProcessIFD(metadata.GetOrCreateDirectory<PanasonicMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset, metadata, reader);
																}
															}
														}
														else
														{
															if ("Panasonic\u0000\u0000\u0000".Equals(reader.GetString(makernoteOffset, 12)))
															{
																// NON-Standard TIFF IFD Data using Panasonic Tags. There is no Next-IFD pointer after the IFD
																// Offsets are relative to the start of the TIFF header at the beginning of the EXIF segment
																// more information here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html
																ProcessIFD(metadata.GetOrCreateDirectory<PanasonicMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 12, tiffHeaderOffset, metadata, reader);
															}
															else
															{
																if ("AOC\u0000".Equals(firstFourChars))
																{
																	// NON-Standard TIFF IFD Data using Casio Type 2 Tags
																	// IFD has no Next-IFD pointer at end of IFD, and
																	// Offsets are relative to the start of the current IFD tag, not the TIFF header
																	// Observed for:
																	// - Pentax ist D
																	ProcessIFD(metadata.GetOrCreateDirectory<CasioType2MakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 6, makernoteOffset, metadata, reader);
																}
																else
																{
																	if (cameraMake != null && (cameraMake.ToUpper().StartsWith("PENTAX") || cameraMake.ToUpper().StartsWith("ASAHI")))
																	{
																		// NON-Standard TIFF IFD Data using Pentax Tags
																		// IFD has no Next-IFD pointer at end of IFD, and
																		// Offsets are relative to the start of the current IFD tag, not the TIFF header
																		// Observed for:
																		// - PENTAX Optio 330
																		// - PENTAX Optio 430
																		ProcessIFD(metadata.GetOrCreateDirectory<PentaxMakernoteDirectory>(), processedIfdOffsets, makernoteOffset, makernoteOffset, metadata, reader);
																	}
																	else
																	{
																		//        } else if ("KC".equals(firstTwoChars) || "MINOL".equals(firstFiveChars) || "MLY".equals(firstThreeChars) || "+M+M+M+M".equals(firstEightChars)) {
																		//            // This Konica data is not understood.  Header identified in accordance with information at this site:
																		//            // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html
																		//            // TODO add support for minolta/konica cameras
																		//            exifDirectory.addError("Unsupported Konica/Minolta data ignored.");
																		if ("SANYO\x0\x1\x0".Equals(firstEightChars))
																		{
																			ProcessIFD(metadata.GetOrCreateDirectory<SanyoMakernoteDirectory>(), processedIfdOffsets, makernoteOffset + 8, makernoteOffset, metadata, reader);
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			// The makernote is not comprehended by this library.
			// If you are reading this and believe a particular camera's image should be processed, get in touch.
			reader.SetMotorolaByteOrder(byteOrderBefore);
		}

		[Obsolete]
		private static void ProcessKodakMakernote(KodakMakernoteDirectory directory, int tagValueOffset, RandomAccessReader reader)
		{
			// Kodak's makernote is not in IFD format. It has values at fixed offsets.
			int dataOffset = tagValueOffset + 8;
			try
			{
				directory.SetString(KodakMakernoteDirectory.TagKodakModel, reader.GetString(dataOffset, 8));
				directory.SetInt(KodakMakernoteDirectory.TagQuality, reader.GetUInt8(dataOffset + 9));
				directory.SetInt(KodakMakernoteDirectory.TagBurstMode, reader.GetUInt8(dataOffset + 10));
				directory.SetInt(KodakMakernoteDirectory.TagImageWidth, reader.GetUInt16(dataOffset + 12));
				directory.SetInt(KodakMakernoteDirectory.TagImageHeight, reader.GetUInt16(dataOffset + 14));
				directory.SetInt(KodakMakernoteDirectory.TagYearCreated, reader.GetUInt16(dataOffset + 16));
				directory.SetByteArray(KodakMakernoteDirectory.TagMonthDayCreated, reader.GetBytes(dataOffset + 18, 2));
				directory.SetByteArray(KodakMakernoteDirectory.TagTimeCreated, reader.GetBytes(dataOffset + 20, 4));
				directory.SetInt(KodakMakernoteDirectory.TagBurstMode2, reader.GetUInt16(dataOffset + 24));
				directory.SetInt(KodakMakernoteDirectory.TagShutterMode, reader.GetUInt8(dataOffset + 27));
				directory.SetInt(KodakMakernoteDirectory.TagMeteringMode, reader.GetUInt8(dataOffset + 28));
				directory.SetInt(KodakMakernoteDirectory.TagSequenceNumber, reader.GetUInt8(dataOffset + 29));
				directory.SetInt(KodakMakernoteDirectory.TagFNumber, reader.GetUInt16(dataOffset + 30));
				directory.SetLong(KodakMakernoteDirectory.TagExposureTime, reader.GetUInt32(dataOffset + 32));
				directory.SetInt(KodakMakernoteDirectory.TagExposureCompensation, reader.GetInt16(dataOffset + 36));
				directory.SetInt(KodakMakernoteDirectory.TagFocusMode, reader.GetUInt8(dataOffset + 56));
				directory.SetInt(KodakMakernoteDirectory.TagWhiteBalance, reader.GetUInt8(dataOffset + 64));
				directory.SetInt(KodakMakernoteDirectory.TagFlashMode, reader.GetUInt8(dataOffset + 92));
				directory.SetInt(KodakMakernoteDirectory.TagFlashFired, reader.GetUInt8(dataOffset + 93));
				directory.SetInt(KodakMakernoteDirectory.TagIsoSetting, reader.GetUInt16(dataOffset + 94));
				directory.SetInt(KodakMakernoteDirectory.TagIso, reader.GetUInt16(dataOffset + 96));
				directory.SetInt(KodakMakernoteDirectory.TagTotalZoom, reader.GetUInt16(dataOffset + 98));
				directory.SetInt(KodakMakernoteDirectory.TagDateTimeStamp, reader.GetUInt16(dataOffset + 100));
				directory.SetInt(KodakMakernoteDirectory.TagColorMode, reader.GetUInt16(dataOffset + 102));
				directory.SetInt(KodakMakernoteDirectory.TagDigitalZoom, reader.GetUInt16(dataOffset + 104));
				directory.SetInt(KodakMakernoteDirectory.TagSharpness, reader.GetInt8(dataOffset + 107));
			}
			catch (IOException ex)
			{
				directory.AddError("Error processing Kodak makernote data: " + ex.Message);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		[Obsolete]
		private static void ProcessTag(Com.Drew.Metadata.Directory directory, int tagType, int tagValueOffset, int componentCount, int formatCode, RandomAccessReader reader)
		{
			switch (formatCode)
			{
				case FmtUndefined:
				{
					// Directory simply stores raw values
					// The display side uses a Descriptor class per directory to turn the raw values into 'pretty' descriptions
					// this includes exif user comments
					directory.SetByteArray(tagType, reader.GetBytes(tagValueOffset, componentCount));
					break;
				}

				case FmtString:
				{
					string @string = reader.GetNullTerminatedString(tagValueOffset, componentCount);
					directory.SetString(tagType, @string);
					break;
				}

				case FmtSrational:
				{
					if (componentCount == 1)
					{
						directory.SetRational(tagType, new Rational(reader.GetInt32(tagValueOffset), reader.GetInt32(tagValueOffset + 4)));
					}
					else
					{
						if (componentCount > 1)
						{
							Rational[] rationals = new Rational[componentCount];
							for (int i = 0; i < componentCount; i++)
							{
								rationals[i] = new Rational(reader.GetInt32(tagValueOffset + (8 * i)), reader.GetInt32(tagValueOffset + 4 + (8 * i)));
							}
							directory.SetRationalArray(tagType, rationals);
						}
					}
					break;
				}

				case FmtUrational:
				{
					if (componentCount == 1)
					{
						directory.SetRational(tagType, new Rational(reader.GetUInt32(tagValueOffset), reader.GetUInt32(tagValueOffset + 4)));
					}
					else
					{
						if (componentCount > 1)
						{
							Rational[] rationals = new Rational[componentCount];
							for (int i = 0; i < componentCount; i++)
							{
								rationals[i] = new Rational(reader.GetUInt32(tagValueOffset + (8 * i)), reader.GetUInt32(tagValueOffset + 4 + (8 * i)));
							}
							directory.SetRationalArray(tagType, rationals);
						}
					}
					break;
				}

				case FmtSingle:
				{
					if (componentCount == 1)
					{
						directory.SetFloat(tagType, reader.GetFloat32(tagValueOffset));
					}
					else
					{
						float[] floats = new float[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							floats[i] = reader.GetFloat32(tagValueOffset + (i * 4));
						}
						directory.SetFloatArray(tagType, floats);
					}
					break;
				}

				case FmtDouble:
				{
					if (componentCount == 1)
					{
						directory.SetDouble(tagType, reader.GetDouble64(tagValueOffset));
					}
					else
					{
						double[] doubles = new double[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							doubles[i] = reader.GetDouble64(tagValueOffset + (i * 4));
						}
						directory.SetDoubleArray(tagType, doubles);
					}
					break;
				}

				case FmtSbyte:
				{
					//
					// Note that all integral types are stored as int32 internally (the largest supported by TIFF)
					//
					if (componentCount == 1)
					{
						directory.SetInt(tagType, reader.GetInt8(tagValueOffset));
					}
					else
					{
						int[] bytes = new int[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							bytes[i] = reader.GetInt8(tagValueOffset + i);
						}
						directory.SetIntArray(tagType, bytes);
					}
					break;
				}

				case FmtByte:
				{
					if (componentCount == 1)
					{
						directory.SetInt(tagType, reader.GetUInt8(tagValueOffset));
					}
					else
					{
						int[] bytes = new int[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							bytes[i] = reader.GetUInt8(tagValueOffset + i);
						}
						directory.SetIntArray(tagType, bytes);
					}
					break;
				}

				case FmtUshort:
				{
					if (componentCount == 1)
					{
						int i = reader.GetUInt16(tagValueOffset);
						directory.SetInt(tagType, i);
					}
					else
					{
						int[] ints = new int[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							ints[i] = reader.GetUInt16(tagValueOffset + (i * 2));
						}
						directory.SetIntArray(tagType, ints);
					}
					break;
				}

				case FmtSshort:
				{
					if (componentCount == 1)
					{
						int i = reader.GetInt16(tagValueOffset);
						directory.SetInt(tagType, i);
					}
					else
					{
						int[] ints = new int[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							ints[i] = reader.GetInt16(tagValueOffset + (i * 2));
						}
						directory.SetIntArray(tagType, ints);
					}
					break;
				}

				case FmtSlong:
				case FmtUlong:
				{
					// NOTE 'long' in this case means 32 bit, not 64
					if (componentCount == 1)
					{
						int i = reader.GetInt32(tagValueOffset);
						directory.SetInt(tagType, i);
					}
					else
					{
						int[] ints = new int[componentCount];
						for (int i = 0; i < componentCount; i++)
						{
							ints[i] = reader.GetInt32(tagValueOffset + (i * 4));
						}
						directory.SetIntArray(tagType, ints);
					}
					break;
				}

				default:
				{
					directory.AddError("Unknown format code " + formatCode + " for tag " + tagType);
					break;
				}
			}
		}

		/// <summary>Determine the offset at which a given InteropArray entry begins within the specified IFD.</summary>
		/// <param name="ifdStartOffset">the offset at which the IFD starts</param>
		/// <param name="entryNumber">the zero-based entry number</param>
		[Obsolete]
		private static int CalculateTagOffset(int ifdStartOffset, int entryNumber)
		{
			// add 2 bytes for the tag count
			// each entry is 12 bytes, so we skip 12 * the number seen so far
			return ifdStartOffset + 2 + (12 * entryNumber);
		}
	}
}
