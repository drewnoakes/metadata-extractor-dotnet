#region License
//
// Copyright 2002-2016 Drew Noakes
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Implementation of <see cref="ITiffHandler"/> used for handling TIFF tags according to the Exif standard.
    /// <para />
    /// Includes support for camera manufacturer makernotes.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifTiffHandler : DirectoryTiffHandler
    {
        private readonly bool _storeThumbnailBytes;

        public ExifTiffHandler([NotNull] List<Directory> directories, bool storeThumbnailBytes)
            : base(directories, typeof(ExifIfd0Directory))
        {
            _storeThumbnailBytes = storeThumbnailBytes;
        }

        /// <exception cref="TiffProcessingException"/>
        public override void SetTiffMarker(int marker)
        {
            const int standardTiffMarker     = 0x002A;
            const int olympusRawTiffMarker   = 0x4F52; // for ORF files
            const int olympusRawTiffMarker2  = 0x5352; // for ORF files
            const int panasonicRawTiffMarker = 0x0055; // for RW2 files

            if (marker != standardTiffMarker && marker != olympusRawTiffMarker && marker != olympusRawTiffMarker2 && marker != panasonicRawTiffMarker)
                throw new TiffProcessingException($"Unexpected TIFF marker: 0x{marker:X}");
        }

        public override bool TryEnterSubIfd(int tagId)
        {
            if (tagId == ExifDirectoryBase.TagSubIfdOffset)
            {
                PushDirectory(typeof(ExifSubIfdDirectory));
                return true;
            }

            if (CurrentDirectory is ExifIfd0Directory)
            {
                if (tagId == ExifIfd0Directory.TagExifSubIfdOffset)
                {
                    PushDirectory(typeof(ExifSubIfdDirectory));
                    return true;
                }
                if (tagId == ExifIfd0Directory.TagGpsInfoOffset)
                {
                    PushDirectory(typeof(GpsDirectory));
                    return true;
                }
            }

            if (CurrentDirectory is ExifSubIfdDirectory)
            {
                if (tagId == ExifSubIfdDirectory.TagInteropOffset)
                {
                    PushDirectory(typeof(ExifInteropDirectory));
                    return true;
                }
            }

            if (CurrentDirectory is OlympusMakernoteDirectory)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (tagId)
                {
                    case OlympusMakernoteDirectory.TagEquipment:
                        PushDirectory(typeof(OlympusEquipmentMakernoteDirectory));
                        return true;
                    case OlympusMakernoteDirectory.TagCameraSettings:
                        PushDirectory(typeof(OlympusCameraSettingsMakernoteDirectory));
                        return true;
                    case OlympusMakernoteDirectory.TagRawDevelopment:
                        PushDirectory(typeof(OlympusRawDevelopmentMakernoteDirectory));
                        return true;
                    case OlympusMakernoteDirectory.TagRawDevelopment2:
                        PushDirectory(typeof(OlympusRawDevelopment2MakernoteDirectory));
                        return true;
                    case OlympusMakernoteDirectory.TagImageProcessing:
                        PushDirectory(typeof(OlympusImageProcessingMakernoteDirectory));
                        return true;
                    case OlympusMakernoteDirectory.TagFocusInfo:
                        PushDirectory(typeof(OlympusFocusInfoMakernoteDirectory));
                        return true;
                    case OlympusMakernoteDirectory.TagMainInfo:
                        PushDirectory(typeof(OlympusMakernoteDirectory));
                        return true;
                }
            }

            return false;
        }

        public override bool HasFollowerIfd()
        {
            // In Exif, the only known 'follower' IFD is the thumbnail one, however this may not be the case.
            if (CurrentDirectory is ExifIfd0Directory)
            {
                PushDirectory(typeof(ExifThumbnailDirectory));
                return true;
            }
            // The Canon EOS 7D (CR2) has three chained/following thumbnail IFDs
            if (CurrentDirectory is ExifThumbnailDirectory)
            {
                return true;
            }
            // This should not happen, as Exif doesn't use follower IFDs apart from that above.
            // NOTE have seen the CanonMakernoteDirectory IFD have a follower pointer, but it points to invalid data.
            return false;
        }

        /// <exception cref="System.IO.IOException"/>
        public override bool CustomProcessTag(int tagOffset, ICollection<int> processedIfdOffsets, int tiffHeaderOffset, IndexedReader reader, int tagId, int byteCount)
        {
            // Custom processing for the Makernote tag
            if (tagId == ExifDirectoryBase.TagMakernote && CurrentDirectory is ExifSubIfdDirectory)
            {
                return ProcessMakernote(tagOffset, processedIfdOffsets, tiffHeaderOffset, reader);
            }

            // Custom processing for embedded IPTC data
            if (tagId == ExifDirectoryBase.TagIptcNaa && CurrentDirectory is ExifIfd0Directory)
            {
                // NOTE Adobe sets type 4 for IPTC instead of 7
                if (reader.GetSByte(tagOffset) == 0x1c)
                {
                    var iptcBytes = reader.GetBytes(tagOffset, byteCount);
                    var iptcDirectory = new IptcReader().Extract(new SequentialByteArrayReader(iptcBytes), iptcBytes.Length);
                    iptcDirectory.Parent = CurrentDirectory;
                    Directories.Add(iptcDirectory);
                    return true;
                }
                return false;
            }

            // Custom processing for embedded XMP data
            if (tagId == ExifDirectoryBase.TagApplicationNotes && CurrentDirectory is ExifIfd0Directory)
            {
                var xmpDirectory = new XmpReader().Extract(reader.GetNullTerminatedBytes(tagOffset, byteCount));
                xmpDirectory.Parent = CurrentDirectory;
                Directories.Add(xmpDirectory);
                return true;
            }

            return false;
        }

        public override bool TryCustomProcessFormat(int tagId, TiffDataFormatCode formatCode, uint componentCount, out long byteCount)
        {
            if ((ushort)formatCode == 13u)
            {
                byteCount = 4 * componentCount;
                return true;
            }

            byteCount = default(int);
            return false;
        }

        public override void Completed(IndexedReader reader, int tiffHeaderOffset)
        {
            if (_storeThumbnailBytes)
            {
                // after the extraction process, if we have the correct tags, we may be able to store thumbnail information
                var thumbnailDirectory = Directories.OfType<ExifThumbnailDirectory>().FirstOrDefault();
                if (thumbnailDirectory != null && thumbnailDirectory.ContainsTag(ExifDirectoryBase.TagCompression))
                {
                    int offset;
                    int length;
                    if (thumbnailDirectory.TryGetInt32(ExifThumbnailDirectory.TagThumbnailOffset, out offset) &&
                        thumbnailDirectory.TryGetInt32(ExifThumbnailDirectory.TagThumbnailLength, out length))
                    {
                        try
                        {
                            var thumbnailData = reader.GetBytes(tiffHeaderOffset + offset, length);
                            thumbnailDirectory.ThumbnailData = thumbnailData;
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
        private bool ProcessMakernote(int makernoteOffset, [NotNull] ICollection<int> processedIfdOffsets, int tiffHeaderOffset, [NotNull] IndexedReader reader)
        {
            var cameraMake = Directories.OfType<ExifIfd0Directory>().FirstOrDefault()?.GetString(ExifDirectoryBase.TagMake);

            var firstTwoChars = reader.GetString(makernoteOffset, 2, Encoding.UTF8);
            var firstThreeChars = reader.GetString(makernoteOffset, 3, Encoding.UTF8);
            var firstFourChars = reader.GetString(makernoteOffset, 4, Encoding.UTF8);
            var firstFiveChars = reader.GetString(makernoteOffset, 5, Encoding.UTF8);
            var firstSixChars = reader.GetString(makernoteOffset, 6, Encoding.UTF8);
            var firstSevenChars = reader.GetString(makernoteOffset, 7, Encoding.UTF8);
            var firstEightChars = reader.GetString(makernoteOffset, 8, Encoding.UTF8);
            var firstTenChars = reader.GetString(makernoteOffset, 10, Encoding.UTF8);
            var firstTwelveChars = reader.GetString(makernoteOffset, 12, Encoding.UTF8);

            var byteOrderBefore = reader.IsMotorolaByteOrder;

            if ("OLYMP\0" == firstSixChars || "EPSON" == firstFiveChars || "AGFA" == firstFourChars)
            {
                // Olympus Makernote
                // Epson and Agfa use Olympus makernote standard: http://www.ozhiker.com/electronics/pjmt/jpeg_info/
                PushDirectory(typeof(OlympusMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
            }
            else if ("OLYMPUS\0II" == firstTenChars)
            {
                // Olympus Makernote (alternate)
                // Note that data is relative to the beginning of the makernote
                // http://exiv2.org/makernote.html
                PushDirectory(typeof(OlympusMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 12, makernoteOffset);
            }
            else if (cameraMake != null && cameraMake.ToUpper().StartsWith("MINOLTA"))
            {
                // Cases seen with the model starting with MINOLTA in capitals seem to have a valid Olympus makernote
                // area that commences immediately.
                PushDirectory(typeof(OlympusMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
            }
            else if (cameraMake != null && cameraMake.Trim().ToUpper().StartsWith("NIKON"))
            {
                if ("Nikon" == firstFiveChars)
                {
                    switch (reader.GetByte(makernoteOffset + 6))
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
                            PushDirectory(typeof(NikonType1MakernoteDirectory));
                            TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
                            break;
                        }

                        case 2:
                        {
                            PushDirectory(typeof(NikonType2MakernoteDirectory));
                            TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 18, makernoteOffset + 10);
                            break;
                        }

                        default:
                        {
                            Error("Unsupported Nikon makernote data ignored.");
                            break;
                        }
                    }
                }
                else
                {
                    // The IFD begins with the first Makernote byte (no ASCII name).  This occurs with CoolPix 775, E990 and D1 models.
                    PushDirectory(typeof(NikonType2MakernoteDirectory));
                    TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
                }
            }
            else if ("SONY CAM" == firstEightChars || "SONY DSC" == firstEightChars)
            {
                PushDirectory(typeof(SonyType1MakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 12, tiffHeaderOffset);
            }
            else if ("SEMC MS\u0000\u0000\u0000\u0000\u0000" == firstTwelveChars)
            {
                // force MM for this directory
                reader.IsMotorolaByteOrder = true;
                // skip 12 byte header + 2 for "MM" + 6
                PushDirectory(typeof(SonyType6MakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 20, tiffHeaderOffset);
            }
            else if ("SIGMA\u0000\u0000\u0000" == firstEightChars || "FOVEON\u0000\u0000" == firstEightChars)
            {
                PushDirectory(typeof(SigmaMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 10, tiffHeaderOffset);
            }
            else if ("KDK" == firstThreeChars)
            {
                reader.IsMotorolaByteOrder = firstSevenChars == "KDK INFO";
                var directory = new KodakMakernoteDirectory();
                Directories.Add(directory);
                ProcessKodakMakernote(directory, makernoteOffset, reader);
            }
            else if ("Canon".Equals(cameraMake, StringComparison.OrdinalIgnoreCase))
            {
                PushDirectory(typeof(CanonMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
            }
            else if (cameraMake != null && cameraMake.ToUpper().StartsWith("CASIO"))
            {
                if ("QVC\u0000\u0000\u0000" == firstSixChars)
                {
                    PushDirectory(typeof(CasioType2MakernoteDirectory));
                    TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 6, tiffHeaderOffset);
                }
                else
                {
                    PushDirectory(typeof(CasioType1MakernoteDirectory));
                    TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, tiffHeaderOffset);
                }
            }
            else if ("FUJIFILM" == firstEightChars || "Fujifilm".Equals(cameraMake, StringComparison.OrdinalIgnoreCase))
            {
                // Note that this also applies to certain Leica cameras, such as the Digilux-4.3
                reader.IsMotorolaByteOrder = false;
                // the 4 bytes after "FUJIFILM" in the makernote point to the start of the makernote
                // IFD, though the offset is relative to the start of the makernote, not the TIFF
                // header (like everywhere else)
                var ifdStart = makernoteOffset + reader.GetInt32(makernoteOffset + 8);
                PushDirectory(typeof(FujifilmMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, ifdStart, makernoteOffset);
            }
            else if ("KYOCERA" == firstSevenChars)
            {
                // http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
                PushDirectory(typeof(KyoceraMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 22, tiffHeaderOffset);
            }
            else if ("LEICA" == firstFiveChars)
            {
                reader.IsMotorolaByteOrder = false;
                if ("Leica Camera AG" == cameraMake)
                {
                    PushDirectory(typeof(LeicaMakernoteDirectory));
                    TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
                }
                else if ("LEICA" == cameraMake)
                {
                    // Some Leica cameras use Panasonic makernote tags
                    PushDirectory(typeof(PanasonicMakernoteDirectory));
                    TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, tiffHeaderOffset);
                }
                else
                {
                    return false;
                }
            }
            else if ("Panasonic\u0000\u0000\u0000" == reader.GetString(makernoteOffset, 12, Encoding.UTF8))
            {
                // NON-Standard TIFF IFD Data using Panasonic Tags. There is no Next-IFD pointer after the IFD
                // Offsets are relative to the start of the TIFF header at the beginning of the EXIF segment
                // more information here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html
                PushDirectory(typeof(PanasonicMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 12, tiffHeaderOffset);
            }
            else if ("AOC\u0000" == firstFourChars)
            {
                // NON-Standard TIFF IFD Data using Casio Type 2 Tags
                // IFD has no Next-IFD pointer at end of IFD, and
                // Offsets are relative to the start of the current IFD tag, not the TIFF header
                // Observed for:
                // - Pentax ist D
                PushDirectory(typeof(CasioType2MakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 6, makernoteOffset);
            }
            else if (cameraMake != null && (cameraMake.ToUpper().StartsWith("PENTAX") || cameraMake.ToUpper().StartsWith("ASAHI")))
            {
                // NON-Standard TIFF IFD Data using Pentax Tags
                // IFD has no Next-IFD pointer at end of IFD, and
                // Offsets are relative to the start of the current IFD tag, not the TIFF header
                // Observed for:
                // - PENTAX Optio 330
                // - PENTAX Optio 430
                PushDirectory(typeof(PentaxMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset, makernoteOffset);
            }
//          else if ("KC" == firstTwoChars || "MINOL" == firstFiveChars || "MLY" == firstThreeChars || "+M+M+M+M" == firstEightChars)
//          {
//              // This Konica data is not understood.  Header identified in accordance with information at this site:
//              // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html
//              // TODO add support for minolta/konica cameras
//              exifDirectory.addError("Unsupported Konica/Minolta data ignored.");
//          }
            else if ("SANYO\x0\x1\x0" == firstEightChars)
            {
                PushDirectory(typeof(SanyoMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, makernoteOffset);
            }
            else if (cameraMake != null && cameraMake.ToLower().StartsWith("ricoh"))
            {
                if (firstTwoChars == "Rv" || firstThreeChars == "Rev")
                {
                    // This is a textual format, where the makernote bytes look like:
                    //   Rv0103;Rg1C;Bg18;Ll0;Ld0;Aj0000;Bn0473800;Fp2E00:������������������������������
                    //   Rv0103;Rg1C;Bg18;Ll0;Ld0;Aj0000;Bn0473800;Fp2D05:������������������������������
                    //   Rv0207;Sf6C84;Rg76;Bg60;Gg42;Ll0;Ld0;Aj0004;Bn0B02900;Fp10B8;Md6700;Ln116900086D27;Sv263:0000000000000000000000��
                    // This format is currently unsupported
                    return false;
                }
                if (firstFiveChars.Equals("Ricoh", StringComparison.OrdinalIgnoreCase))
                {
                    // Always in Motorola byte order
                    reader.IsMotorolaByteOrder = true;
                    PushDirectory(typeof(RicohMakernoteDirectory));
                    TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 8, makernoteOffset);
                }
            }
            else if (firstTenChars == "Apple iOS\0")
            {
                // Always in Motorola byte order
                var orderBefore = reader.IsMotorolaByteOrder;
                reader.IsMotorolaByteOrder = true;
                PushDirectory(typeof(AppleMakernoteDirectory));
                TiffReader.ProcessIfd(this, reader, processedIfdOffsets, makernoteOffset + 14, makernoteOffset);
                reader.IsMotorolaByteOrder = orderBefore;
            }
            else if ("Reconyx".Equals(cameraMake, StringComparison.OrdinalIgnoreCase) || reader.GetUInt16(makernoteOffset) == ReconyxMakernoteDirectory.HyperFireMakernoteVersion)
            {
                var directory = new ReconyxMakernoteDirectory();
                Directories.Add(directory);
                ProcessReconyxMakernote(directory, makernoteOffset, reader);
            }
            else
            {
                // The makernote is not comprehended by this library.
                // If you are reading this and believe a particular camera's image should be processed, get in touch.
                return false;
            }
            reader.IsMotorolaByteOrder = byteOrderBefore;
            return true;
        }

        private static void ProcessKodakMakernote([NotNull] KodakMakernoteDirectory directory, int tagValueOffset, [NotNull] IndexedReader reader)
        {
            // Kodak's makernote is not in IFD format. It has values at fixed offsets.
            var dataOffset = tagValueOffset + 8;
            try
            {
                directory.Set(KodakMakernoteDirectory.TagKodakModel, reader.GetString(dataOffset, 8, Encoding.UTF8));
                directory.Set(KodakMakernoteDirectory.TagQuality, reader.GetByte(dataOffset + 9));
                directory.Set(KodakMakernoteDirectory.TagBurstMode, reader.GetByte(dataOffset + 10));
                directory.Set(KodakMakernoteDirectory.TagImageWidth, reader.GetUInt16(dataOffset + 12));
                directory.Set(KodakMakernoteDirectory.TagImageHeight, reader.GetUInt16(dataOffset + 14));
                directory.Set(KodakMakernoteDirectory.TagYearCreated, reader.GetUInt16(dataOffset + 16));
                directory.Set(KodakMakernoteDirectory.TagMonthDayCreated, reader.GetBytes(dataOffset + 18, 2));
                directory.Set(KodakMakernoteDirectory.TagTimeCreated, reader.GetBytes(dataOffset + 20, 4));
                directory.Set(KodakMakernoteDirectory.TagBurstMode2, reader.GetUInt16(dataOffset + 24));
                directory.Set(KodakMakernoteDirectory.TagShutterMode, reader.GetByte(dataOffset + 27));
                directory.Set(KodakMakernoteDirectory.TagMeteringMode, reader.GetByte(dataOffset + 28));
                directory.Set(KodakMakernoteDirectory.TagSequenceNumber, reader.GetByte(dataOffset + 29));
                directory.Set(KodakMakernoteDirectory.TagFNumber, reader.GetUInt16(dataOffset + 30));
                directory.Set(KodakMakernoteDirectory.TagExposureTime, reader.GetUInt32(dataOffset + 32));
                directory.Set(KodakMakernoteDirectory.TagExposureCompensation, reader.GetInt16(dataOffset + 36));
                directory.Set(KodakMakernoteDirectory.TagFocusMode, reader.GetByte(dataOffset + 56));
                directory.Set(KodakMakernoteDirectory.TagWhiteBalance, reader.GetByte(dataOffset + 64));
                directory.Set(KodakMakernoteDirectory.TagFlashMode, reader.GetByte(dataOffset + 92));
                directory.Set(KodakMakernoteDirectory.TagFlashFired, reader.GetByte(dataOffset + 93));
                directory.Set(KodakMakernoteDirectory.TagIsoSetting, reader.GetUInt16(dataOffset + 94));
                directory.Set(KodakMakernoteDirectory.TagIso, reader.GetUInt16(dataOffset + 96));
                directory.Set(KodakMakernoteDirectory.TagTotalZoom, reader.GetUInt16(dataOffset + 98));
                directory.Set(KodakMakernoteDirectory.TagDateTimeStamp, reader.GetUInt16(dataOffset + 100));
                directory.Set(KodakMakernoteDirectory.TagColorMode, reader.GetUInt16(dataOffset + 102));
                directory.Set(KodakMakernoteDirectory.TagDigitalZoom, reader.GetUInt16(dataOffset + 104));
                directory.Set(KodakMakernoteDirectory.TagSharpness, reader.GetSByte(dataOffset + 107));
            }
            catch (IOException ex)
            {
                directory.AddError("Error processing Kodak makernote data: " + ex.Message);
            }
        }

        private static void ProcessReconyxMakernote([NotNull] ReconyxMakernoteDirectory directory, int makernoteOffset, [NotNull] IndexedReader reader)
        {
            directory.Set(ReconyxMakernoteDirectory.TagMakernoteVersion, reader.GetUInt16(makernoteOffset));

            // revision and build are reversed from .NET ordering
            ushort major = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagFirmwareVersion);
            ushort minor = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagFirmwareVersion + 2);
            ushort revision = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagFirmwareVersion + 4);
            string buildYear = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagFirmwareVersion + 6).ToString("x4");
            string buildDate = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagFirmwareVersion + 8).ToString("x4");
            int build = int.Parse(buildYear + buildDate);
            directory.Set(ReconyxMakernoteDirectory.TagFirmwareVersion, new Version(major, minor, revision, build));

            directory.Set(ReconyxMakernoteDirectory.TagTriggerMode, new string((char)reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagTriggerMode), 1));
            directory.Set(ReconyxMakernoteDirectory.TagSequence,
                          new ushort[]
                          {
                              reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagSequence),
                              reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagSequence + 2)
                          });

            uint eventNumberHigh = (uint)reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagEventNumber);
            uint eventNumberLow = (uint)reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagEventNumber + 2);
            directory.Set(ReconyxMakernoteDirectory.TagEventNumber, (eventNumberHigh << 16) + eventNumberLow);

            ushort seconds = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagDateTimeOriginal);
            ushort minutes = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagDateTimeOriginal + 2);
            ushort hour = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagDateTimeOriginal + 4);
            ushort month = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagDateTimeOriginal + 6);
            ushort day = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagDateTimeOriginal + 8);
            ushort year = reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagDateTimeOriginal + 10);
            directory.Set(ReconyxMakernoteDirectory.TagDateTimeOriginal, new DateTime(year, month, day, hour, minutes, seconds));

            directory.Set(ReconyxMakernoteDirectory.TagMoonPhase, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagMoonPhase));
            directory.Set(ReconyxMakernoteDirectory.TagAmbientTemperatureFarenheit, reader.GetInt16(makernoteOffset + ReconyxMakernoteDirectory.TagAmbientTemperatureFarenheit));
            directory.Set(ReconyxMakernoteDirectory.TagAmbientTemperature, reader.GetInt16(makernoteOffset + ReconyxMakernoteDirectory.TagAmbientTemperature));
            directory.Set(ReconyxMakernoteDirectory.TagSerialNumber, reader.GetString(makernoteOffset + ReconyxMakernoteDirectory.TagSerialNumber, 28, Encoding.Unicode));
            // two unread bytes: the serial number's terminating null

            directory.Set(ReconyxMakernoteDirectory.TagContrast, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagContrast));
            directory.Set(ReconyxMakernoteDirectory.TagBrightness, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagBrightness));
            directory.Set(ReconyxMakernoteDirectory.TagSharpness, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagSharpness));
            directory.Set(ReconyxMakernoteDirectory.TagSaturation, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagSaturation));
            directory.Set(ReconyxMakernoteDirectory.TagInfraredIlluminator, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagInfraredIlluminator));
            directory.Set(ReconyxMakernoteDirectory.TagMotionSensitivity, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagMotionSensitivity));
            directory.Set(ReconyxMakernoteDirectory.TagBatteryVoltage, reader.GetUInt16(makernoteOffset + ReconyxMakernoteDirectory.TagBatteryVoltage) / 1000.0);
            directory.Set(ReconyxMakernoteDirectory.TagUserLabel, reader.GetNullTerminatedString(makernoteOffset + ReconyxMakernoteDirectory.TagUserLabel, 44));
        }
    }
}