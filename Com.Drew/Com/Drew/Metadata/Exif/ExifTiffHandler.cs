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
using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Exif.Makernotes;
using Com.Drew.Metadata.Tiff;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>
	/// Implementation of
	/// <see cref="Com.Drew.Imaging.Tiff.TiffHandler"/>
	/// used for handling TIFF tags according to the Exif
	/// standard.
	/// <p/>
	/// Includes support for camera manufacturer makernotes.
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifTiffHandler : DirectoryTiffHandler
	{
		private bool _storeThumbnailBytes;

		public ExifTiffHandler(Com.Drew.Metadata.Metadata metadata, bool storeThumbnailBytes)
			: base(metadata, typeof(ExifIFD0Directory))
		{
			_storeThumbnailBytes = storeThumbnailBytes;
		}

		/// <exception cref="Com.Drew.Imaging.Tiff.TiffProcessingException"/>
		public override void SetTiffMarker(int marker)
		{
			int standardTiffMarker = unchecked((int)(0x002A));
			int olympusRawTiffMarker = unchecked((int)(0x4F52));
			// for ORF files
			int panasonicRawTiffMarker = unchecked((int)(0x0055));
			// for RW2 files
			if (marker != standardTiffMarker && marker != olympusRawTiffMarker && marker != panasonicRawTiffMarker)
			{
				throw new TiffProcessingException("Unexpected TIFF marker: 0x" + Sharpen.Extensions.ToHexString(marker));
			}
		}

		public override bool IsTagIfdPointer(int tagType)
		{
			if (tagType == ExifIFD0Directory.TagExifSubIfdOffset && _currentDirectory is ExifIFD0Directory)
			{
                PushDirectory<ExifSubIFDDirectory>();
				return true;
			}
			else
			{
				if (tagType == ExifIFD0Directory.TagGpsInfoOffset && _currentDirectory is ExifIFD0Directory)
				{
                    PushDirectory<GpsDirectory>();
					return true;
				}
				else
				{
					if (tagType == ExifSubIFDDirectory.TagInteropOffset && _currentDirectory is ExifSubIFDDirectory)
					{
                        PushDirectory<ExifInteropDirectory>();
						return true;
					}
				}
			}
			return false;
		}

		public override bool HasFollowerIfd()
		{
			// In Exif, the only known 'follower' IFD is the thumbnail one, however this may not be the case.
			if (_currentDirectory is ExifIFD0Directory)
			{
                PushDirectory<ExifThumbnailDirectory>();
				return true;
			}
			// This should not happen, as Exif doesn't use follower IFDs apart from that above.
			// NOTE have seen the CanonMakernoteDirectory IFD have a follower pointer, but it points to invalid data.
			return false;
		}

		/// <exception cref="System.IO.IOException"/>
		public override bool CustomProcessTag(int makernoteOffset, ICollection<int> processedIfdOffsets, int tiffHeaderOffset, RandomAccessReader reader, int tagId, int byteCount)
		{
			// In Exif, we only want custom processing for the Makernote tag
			if (tagId == ExifSubIFDDirectory.TagMakernote && _currentDirectory is ExifSubIFDDirectory)
			{
				return ProcessMakernote(makernoteOffset, processedIfdOffsets, tiffHeaderOffset, reader, byteCount);
			}
			return false;
		}

		public override void Completed(RandomAccessReader reader, int tiffHeaderOffset)
		{
			if (_storeThumbnailBytes)
			{
				// after the extraction process, if we have the correct tags, we may be able to store thumbnail information
				ExifThumbnailDirectory thumbnailDirectory = _metadata.GetDirectory<ExifThumbnailDirectory>();
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
							thumbnailDirectory.AddError("Invalid thumbnail data specification: " + ex.Message);
						}
					}
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private bool ProcessMakernote(int makernoteOffset, ICollection<int> processedIfdOffsets, int tiffHeaderOffset, RandomAccessReader reader, int byteCount)
		{
			// Determine the camera model and makernote format.
			Com.Drew.Metadata.Directory ifd0Directory = _metadata.GetDirectory<ExifIFD0Directory>();
			if (ifd0Directory == null)
			{
				return false;
			}
			string cameraMake = ifd0Directory.GetString(ExifIFD0Directory.TagMake);
			string firstTwoChars = reader.GetString(makernoteOffset, 2);
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
                PushDirectory<OlympusMakernoteDirectory>();
				TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
			}
			else
			{
				if (cameraMake != null && cameraMake.ToUpper().StartsWith("MINOLTA"))
				{
					// Cases seen with the model starting with MINOLTA in capitals seem to have a valid Olympus makernote
					// area that commences immediately.
                    PushDirectory<OlympusMakernoteDirectory>();
					TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
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
                                    PushDirectory<NikonType1MakernoteDirectory>();
									TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
									break;
								}

								case 2:
								{
                                    PushDirectory<NikonType2MakernoteDirectory>();
									TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 18, makernoteOffset + 10);
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
                            PushDirectory<NikonType2MakernoteDirectory>();
							TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
						}
					}
					else
					{
						if ("SONY CAM".Equals(firstEightChars) || "SONY DSC".Equals(firstEightChars))
						{
                            PushDirectory<SonyType1MakernoteDirectory>();
							TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 12, tiffHeaderOffset);
						}
						else
						{
							if ("SEMC MS\u0000\u0000\u0000\u0000\u0000".Equals(firstTwelveChars))
							{
								// force MM for this directory
								reader.SetMotorolaByteOrder(true);
								// skip 12 byte header + 2 for "MM" + 6
                                PushDirectory<SonyType6MakernoteDirectory>();
								TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 20, tiffHeaderOffset);
							}
							else
							{
								if ("SIGMA\u0000\u0000\u0000".Equals(firstEightChars) || "FOVEON\u0000\u0000".Equals(firstEightChars))
								{
                                    PushDirectory<SigmaMakernoteDirectory>();
									TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 10, tiffHeaderOffset);
								}
								else
								{
									if ("KDK".Equals(firstThreeChars))
									{
										reader.SetMotorolaByteOrder(firstSevenChars.Equals("KDK INFO"));
										ProcessKodakMakernote(_metadata.GetOrCreateDirectory<KodakMakernoteDirectory>(), makernoteOffset, reader);
									}
									else
									{
										if (Sharpen.Runtime.EqualsIgnoreCase("Canon", cameraMake))
										{
                                            PushDirectory<CanonMakernoteDirectory>();
											TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
										}
										else
										{
											if (cameraMake != null && cameraMake.ToUpper().StartsWith("CASIO"))
											{
												if ("QVC\u0000\u0000\u0000".Equals(firstSixChars))
												{
                                                    PushDirectory<CasioType2MakernoteDirectory>();
													TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 6, tiffHeaderOffset);
												}
												else
												{
                                                    PushDirectory<CasioType1MakernoteDirectory>();
													TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
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
                                                    PushDirectory<FujifilmMakernoteDirectory>();
													TiffReader.ProcessIfd(this, reader, processedIfdOffsets, ifdStart, makernoteOffset);
												}
												else
												{
													if ("KYOCERA".Equals(firstSevenChars))
													{
														// http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
                                                        PushDirectory<KyoceraMakernoteDirectory>();
														TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 22, tiffHeaderOffset);
													}
													else
													{
														if ("LEICA".Equals(firstFiveChars))
														{
															reader.SetMotorolaByteOrder(false);
															if ("Leica Camera AG".Equals(cameraMake))
															{
                                                                PushDirectory<LeicaMakernoteDirectory>();
																TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
															}
															else
															{
																if ("LEICA".Equals(cameraMake))
																{
																	// Some Leica cameras use Panasonic makernote tags
                                                                    PushDirectory<PanasonicMakernoteDirectory>();
																	TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
																}
																else
																{
																	return false;
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
                                                                PushDirectory<PanasonicMakernoteDirectory>();
																TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 12, tiffHeaderOffset);
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
                                                                    PushDirectory<CasioType2MakernoteDirectory>();
																	TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 6, makernoteOffset);
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
                                                                        PushDirectory<PentaxMakernoteDirectory>();
																		TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, makernoteOffset);
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
                                                                            PushDirectory<SanyoMakernoteDirectory>();
																			TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, makernoteOffset);
																		}
																		else
																		{
																			if (cameraMake != null && cameraMake.ToLower().StartsWith("ricoh"))
																			{
																				if (firstTwoChars.Equals("Rv") || firstThreeChars.Equals("Rev"))
																				{
																					// This is a textual format, where the makernote bytes look like:
																					//   Rv0103;Rg1C;Bg18;Ll0;Ld0;Aj0000;Bn0473800;Fp2E00:пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
																					//   Rv0103;Rg1C;Bg18;Ll0;Ld0;Aj0000;Bn0473800;Fp2D05:пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
																					//   Rv0207;Sf6C84;Rg76;Bg60;Gg42;Ll0;Ld0;Aj0004;Bn0B02900;Fp10B8;Md6700;Ln116900086D27;Sv263:0000000000000000000000пїЅпїЅ
																					// This format is currently unsupported
																					return false;
																				}
																				else
																				{
																					if (Sharpen.Runtime.EqualsIgnoreCase(firstFiveChars, "Ricoh"))
																					{
																						// Always in Motorola byte order
																						reader.SetMotorolaByteOrder(true);
																						PushDirectory<RicohMakernoteDirectory>();
																						TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, makernoteOffset);
																					}
																				}
																			}
																			else
																			{
																				// The makernote is not comprehended by this library.
																				// If you are reading this and believe a particular camera's image should be processed, get in touch.
																				return false;
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
			}
			reader.SetMotorolaByteOrder(byteOrderBefore);
			return true;
		}

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
	}
}
