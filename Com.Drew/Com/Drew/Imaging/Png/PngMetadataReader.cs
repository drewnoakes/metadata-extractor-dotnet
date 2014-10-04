using System;
using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging.Png;
using Com.Drew.Lang;
using Com.Drew.Metadata.Icc;
using Com.Drew.Metadata.Png;
using Com.Drew.Metadata.Xmp;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngMetadataReader
	{
		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(FilePath file)
		{
			InputStream inputStream = null;
			try
			{
				inputStream = new FileInputStream(file);
				return ReadMetadata(inputStream);
			}
			finally
			{
				if (inputStream != null)
				{
					inputStream.Close();
				}
			}
		}

		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata(InputStream inputStream)
		{
			// TODO keep a single static hash of these
			ICollection<PngChunkType> desiredChunkTypes = new HashSet<PngChunkType>();
			desiredChunkTypes.Add(PngChunkType.Ihdr);
			desiredChunkTypes.Add(PngChunkType.Plte);
			desiredChunkTypes.Add(PngChunkType.tRNS);
			desiredChunkTypes.Add(PngChunkType.cHRM);
			desiredChunkTypes.Add(PngChunkType.sRGB);
			desiredChunkTypes.Add(PngChunkType.gAMA);
			desiredChunkTypes.Add(PngChunkType.iCCP);
			desiredChunkTypes.Add(PngChunkType.bKGD);
			desiredChunkTypes.Add(PngChunkType.tEXt);
			desiredChunkTypes.Add(PngChunkType.iTXt);
			desiredChunkTypes.Add(PngChunkType.tIME);
			Iterable<PngChunk> chunks = new PngChunkReader().Extract(new Com.Drew.Lang.StreamReader(inputStream), desiredChunkTypes);
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			IList<KeyValuePair> textPairs = new AList<KeyValuePair>();
			foreach (PngChunk chunk in chunks)
			{
				PngChunkType chunkType = chunk.GetType();
				sbyte[] bytes = chunk.GetBytes();
				if (chunkType.Equals(PngChunkType.Ihdr))
				{
					PngHeader header = new PngHeader(bytes);
					PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
					directory.SetInt(PngDirectory.TagImageWidth, header.GetImageWidth());
					directory.SetInt(PngDirectory.TagImageHeight, header.GetImageHeight());
					directory.SetInt(PngDirectory.TagBitsPerSample, header.GetBitsPerSample());
					directory.SetInt(PngDirectory.TagColorType, header.GetColorType().GetNumericValue());
					directory.SetInt(PngDirectory.TagCompressionType, header.GetCompressionType());
					directory.SetInt(PngDirectory.TagFilterMethod, header.GetFilterMethod());
					directory.SetInt(PngDirectory.TagInterlaceMethod, header.GetInterlaceMethod());
				}
				else
				{
					if (chunkType.Equals(PngChunkType.Plte))
					{
						PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
						directory.SetInt(PngDirectory.TagPaletteSize, bytes.Length / 3);
					}
					else
					{
						if (chunkType.Equals(PngChunkType.tRNS))
						{
							PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
							directory.SetInt(PngDirectory.TagPaletteHasTransparency, 1);
						}
						else
						{
							if (chunkType.Equals(PngChunkType.sRGB))
							{
								int srgbRenderingIntent = new SequentialByteArrayReader(bytes).GetInt8();
								PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
								directory.SetInt(PngDirectory.TagSrgbRenderingIntent, srgbRenderingIntent);
							}
							else
							{
								if (chunkType.Equals(PngChunkType.cHRM))
								{
									PngChromaticities chromaticities = new PngChromaticities(bytes);
									PngChromaticitiesDirectory directory = metadata.GetOrCreateDirectory<PngChromaticitiesDirectory>();
									directory.SetInt(PngChromaticitiesDirectory.TagWhitePointX, chromaticities.GetWhitePointX());
									directory.SetInt(PngChromaticitiesDirectory.TagWhitePointX, chromaticities.GetWhitePointX());
									directory.SetInt(PngChromaticitiesDirectory.TagRedX, chromaticities.GetRedX());
									directory.SetInt(PngChromaticitiesDirectory.TagRedY, chromaticities.GetRedY());
									directory.SetInt(PngChromaticitiesDirectory.TagGreenX, chromaticities.GetGreenX());
									directory.SetInt(PngChromaticitiesDirectory.TagGreenY, chromaticities.GetGreenY());
									directory.SetInt(PngChromaticitiesDirectory.TagBlueX, chromaticities.GetBlueX());
									directory.SetInt(PngChromaticitiesDirectory.TagBlueY, chromaticities.GetBlueY());
								}
								else
								{
									if (chunkType.Equals(PngChunkType.gAMA))
									{
										int gammaInt = new SequentialByteArrayReader(bytes).GetInt32();
										PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
										directory.SetDouble(PngDirectory.TagGamma, gammaInt / 100000.0);
									}
									else
									{
										if (chunkType.Equals(PngChunkType.iCCP))
										{
											SequentialReader reader = new SequentialByteArrayReader(bytes);
											string profileName = reader.GetNullTerminatedString(79);
											PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
											directory.SetString(PngDirectory.TagProfileName, profileName);
											sbyte compressionMethod = reader.GetInt8();
											if (compressionMethod == 0)
											{
												// Only compression method allowed by the spec is zero: deflate
												// This assumes 1-byte-per-char, which it is by spec.
												int bytesLeft = bytes.Length - profileName.Length - 2;
												sbyte[] compressedProfile = reader.GetBytes(bytesLeft);
												InflaterInputStream inflateStream = new InflaterInputStream(new ByteArrayInputStream(compressedProfile));
												new IccReader().Extract(new RandomAccessStreamReader(inflateStream), metadata);
												inflateStream.Close();
											}
										}
										else
										{
											if (chunkType.Equals(PngChunkType.bKGD))
											{
												PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
												directory.SetByteArray(PngDirectory.TagBackgroundColor, bytes);
											}
											else
											{
												if (chunkType.Equals(PngChunkType.tEXt))
												{
													SequentialReader reader = new SequentialByteArrayReader(bytes);
													string keyword = reader.GetNullTerminatedString(79);
													int bytesLeft = bytes.Length - keyword.Length - 1;
													string value = reader.GetNullTerminatedString(bytesLeft);
													textPairs.Add(new KeyValuePair(keyword, value));
												}
												else
												{
													if (chunkType.Equals(PngChunkType.iTXt))
													{
														SequentialReader reader = new SequentialByteArrayReader(bytes);
														string keyword = reader.GetNullTerminatedString(79);
														sbyte compressionFlag = reader.GetInt8();
														sbyte compressionMethod = reader.GetInt8();
														string languageTag = reader.GetNullTerminatedString(bytes.Length);
														string translatedKeyword = reader.GetNullTerminatedString(bytes.Length);
														int bytesLeft = bytes.Length - keyword.Length - 1 - 1 - 1 - languageTag.Length - 1 - translatedKeyword.Length - 1;
														string text = null;
														if (compressionFlag == 0)
														{
															text = reader.GetNullTerminatedString(bytesLeft);
														}
														else
														{
															if (compressionFlag == 1)
															{
																if (compressionMethod == 0)
																{
																	text = StringUtil.FromStream(new InflaterInputStream(new ByteArrayInputStream(bytes, bytes.Length - bytesLeft, bytesLeft)));
																}
																else
																{
																	metadata.GetOrCreateDirectory<PngDirectory>().AddError("Invalid compression method value");
																}
															}
															else
															{
																metadata.GetOrCreateDirectory<PngDirectory>().AddError("Invalid compression flag value");
															}
														}
														if (text != null)
														{
															if (keyword.Equals("XML:com.adobe.xmp"))
															{
																// NOTE in testing images, the XMP has parsed successfully, but we are not extracting tags from it as necessary
																new XmpReader().Extract(text, metadata);
															}
															else
															{
																textPairs.Add(new KeyValuePair(keyword, text));
															}
														}
													}
													else
													{
														if (chunkType.Equals(PngChunkType.tIME))
														{
															SequentialByteArrayReader reader = new SequentialByteArrayReader(bytes);
															int year = reader.GetUInt16();
															int month = reader.GetUInt8() - 1;
															int day = reader.GetUInt8();
															int hour = reader.GetUInt8();
															int minute = reader.GetUInt8();
															int second = reader.GetUInt8();
															Sharpen.Calendar calendar = Sharpen.Calendar.GetInstance(Sharpen.Extensions.GetTimeZone("UTC"));
															//noinspection MagicConstant
															calendar.Set(year, month, day, hour, minute, second);
															PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
															directory.SetDate(PngDirectory.TagLastModificationTime, calendar.GetTime());
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
			if (textPairs.Count != 0)
			{
				PngDirectory directory = metadata.GetOrCreateDirectory<PngDirectory>();
				directory.SetObject(PngDirectory.TagTextualData, textPairs);
			}
			return metadata;
		}
	}
}
