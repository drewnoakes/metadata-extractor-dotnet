// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iso14496;
using MetadataExtractor.Formats.Iso14496.Boxes;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Xmp;
#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Heif
{
    public static class HeifMetadataReader
    {
        private const int Hvc1Tag = 0x68766331; // "hvc1"
        private const int ExifTag = 0x45786966; // "Exif"
        private const int MimeTag = 0x6D696D65; // "mime"

        public static DirectoryList ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();

            //
            // Read all boxes from the file
            //

            var reader = new SequentialStreamReader(stream);

            var boxes = BoxReader.ReadBoxes(reader);

            //
            // Map those boxes to directories
            //

            ParseQuickTimeTest();

            uint primaryItem = boxes.Descendant<PrimaryItemBox>()?.PrimaryItem ?? uint.MaxValue;
            var itemRefs = (boxes.Descendant<ItemReferenceBox>()?.Boxes ?? new SingleItemTypeReferenceBox[0])
                .Where(i => i.Type == BoxTypes.ThmbTag || i.Type == BoxTypes.CdscTag || i.Type == BoxTypes.MimeTag).ToList();

            ParseImageProperties();

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
                ParseItemSegments();
            }

            return directories;

            void ParseQuickTimeTest()
            {
                if (boxes.Descendant<FileTypeBox>() is { } ftype)
                {
                    var dir = new QuickTimeFileTypeDirectory();
                    if (ftype.MajorBrand > 0)
                    {
                        dir.Set(QuickTimeFileTypeDirectory.TagMajorBrand, ftype.MajorBrandString);
                    }

                    if (ftype.MinorBrand > 0)
                    {
                        dir.Set(QuickTimeFileTypeDirectory.TagMinorVersion, ftype.MinorBrandString);
                    }

                    if (ftype.CompatibleBrands.Count > 0)
                    {
                        dir.Set(
                            QuickTimeFileTypeDirectory.TagCompatibleBrands,
                            string.Join(", ", ftype.CompatibleBrandStrings.ToArray()));
                    }

                    directories.Add(dir);
                }
            }

            void ParseImageProperties()
            {
                uint[] allPrimaryTiles = (boxes.Descendant<ItemReferenceBox>()?.Boxes ?? new SingleItemTypeReferenceBox[0])
                    .SelectMany(i => i.FromItemId == primaryItem && i.Type == BoxTypes.DimgTag ? i.ToItemIds : [])
                    .ToArray();
                var itemPropertyBox = boxes.Descendant<ItemPropertyBox>();
                if (itemPropertyBox is null)
                    return;
                var props = itemPropertyBox.Boxes.Descendant<ItemPropertyContainerBox>()?.Boxes;
                var associations = itemPropertyBox.Boxes.Descendant<ItemPropertyAssociationBox>();

                if (props is not null && associations is not null)
                {
                    ParsePropertyBoxes(
                        "HEIC Primary Item Properties",
                        ImageProperties(primaryItem, allPrimaryTiles, associations, props));

                    foreach (var itemRef in itemRefs)
                    {
                        ParsePropertyBoxes(
                            "HEIC Thumbnail Properties",
                            ImageProperties(itemRef.FromItemId, [], associations, props));
                    }
                }

                return;

                static IEnumerable<Box> ImageProperties(
                    uint primaryId,
                    uint[] secondary,
                    ItemPropertyAssociationBox associations,
                    IList<Box> props)
                {
                    return DirectProperties(primaryId, associations, props)
                        .Concat(
                            (from associationBox in associations.Entries.Where(i => secondary.Contains(i.ItemId))
                             from propIndex in associationBox.Properties
                             let i = props.ElementAt(propIndex.Index - 1)
                             where i is DecoderConfigurationBox || i is ColorInformationBox
                             select i)
                            .Distinct());

                    static IEnumerable<Box> DirectProperties(uint primaryId, ItemPropertyAssociationBox associations, IList<Box> props)
                    {
                        return from associationBox in associations.Entries.Where(i => i.ItemId == primaryId)
                               from propIndex in associationBox.Properties
                               select props.ElementAt(propIndex.Index - 1);
                    }
                }

                void ParsePropertyBoxes(string propertyBoxTitle, IEnumerable<Box> props)
                {
                    var dir = new HeicImagePropertiesDirectory(propertyBoxTitle);
                    bool hasProp = false;
                    directories.Add(dir); // add now so it will precede the ICC profile directory
                    foreach (var prop in props)
                    {
                        switch (prop)
                        {
                            case ImageSpatialExtentsBox ipse:
                                ParseImageSize(ipse);
                                break;
                            case ImageRotationBox irot:
                                ParseImageRotation(irot);
                                break;
                            case PixelInformationBox pixi:
                                ParsePixelDepth(pixi);
                                break;
                            case DecoderConfigurationBox hvcC:
                                ParseDecoderInformation(hvcC);
                                break;
                            case ColorInformationBox colr:
                                ParseColorBox(colr);
                                break;
                            default:
                                continue;
                        }

                        hasProp = true;
                    }

                    if (!hasProp)
                    {
                        directories.Remove(dir);
                    }

                    return;

                    void ParseImageSize(ImageSpatialExtentsBox ipse)
                    {
                        dir.Set(HeicImagePropertiesDirectory.TagImageWidth, ipse.X);
                        dir.Set(HeicImagePropertiesDirectory.TagImageHeight, ipse.Y);
                    }

                    void ParseImageRotation(ImageRotationBox irot)
                    {
                        dir.Set(HeicImagePropertiesDirectory.TagRotation, irot.Rotation);
                    }

                    void ParsePixelDepth(PixelInformationBox pixi)
                    {
                        dir.Set(HeicImagePropertiesDirectory.TagPixelDepths, pixi.BitsPerChannel);
                    }

                    void ParseDecoderInformation(DecoderConfigurationBox hvcC)
                    {
                        dir.Set(HeicImagePropertiesDirectory.TagConfigurationVersion, hvcC.ConfigurationVersion);
                        dir.Set(HeicImagePropertiesDirectory.TagGeneralProfileSpace, hvcC.GeneralProfileSpace);
                        dir.Set(HeicImagePropertiesDirectory.TagGeneralTierTag, hvcC.GeneralTierTag);
                        dir.Set(HeicImagePropertiesDirectory.TagGeneralProfileIdc, hvcC.GeneralProfileIdc);
                        dir.Set(HeicImagePropertiesDirectory.TagGeneralProfileCompatibilityTag, hvcC.GeneralProfileCompatibilityFlags);
                        dir.Set(HeicImagePropertiesDirectory.TagGeneralLevelIdc, hvcC.GeneralLevelIdc);
                        dir.Set(HeicImagePropertiesDirectory.TagMinSpacialSegmentationIdc, hvcC.MinSpacialSegmentationIdc);
                        dir.Set(HeicImagePropertiesDirectory.TagParallelismType, hvcC.ParallelismType);
                        dir.Set(HeicImagePropertiesDirectory.TagChromaFormat, hvcC.ChromaFormat);
                        dir.Set(HeicImagePropertiesDirectory.TagBitDepthLuma, hvcC.BitDepthLumaMinus8 + 8);
                        dir.Set(HeicImagePropertiesDirectory.TagBitDepthChroma, hvcC.BitDepthChromaMinus8 + 8);
                        dir.Set(HeicImagePropertiesDirectory.TagAverageFrameRate, hvcC.AvgFrameRate);
                        dir.Set(HeicImagePropertiesDirectory.TagConstantFrameRate, hvcC.ConstantFrameRate);
                        dir.Set(HeicImagePropertiesDirectory.TagNumTemporalLayers, hvcC.NumTemporalLayers);
                        dir.Set(HeicImagePropertiesDirectory.TagLengthSize, hvcC.LengthSizeMinus1 + 1);
                    }

                    void ParseColorBox(ColorInformationBox colr)
                    {
                        dir.Set(HeicImagePropertiesDirectory.TagColorFormat, colr.ColorType);
                        if (colr.ColorType == ColorInformationBox.NclxTag)
                        {
                            dir.Set(HeicImagePropertiesDirectory.TagColorPrimaries, colr.ColorPrimaries);
                            dir.Set(HeicImagePropertiesDirectory.TagColorTransferCharacteristics, colr.TransferCharacteristics);
                            dir.Set(HeicImagePropertiesDirectory.TagColorMatrixCharacteristics, colr.MatrixCharacteristics);
                            dir.Set(HeicImagePropertiesDirectory.TagFullRangeColor, colr.FullRangeFlag);
                        }
                        else
                        {
                            var iccDirectory = new IccReader().Extract(new ByteArrayReader(colr.IccProfile));
                            iccDirectory.Parent = dir;
                            directories.Add(iccDirectory);
                        }
                    }
                }
            }

            void ParseItemSegments()
            {
                var locations = boxes.Descendant<ItemLocationBox>();
                var information = boxes.Descendant<ItemInformationBox>();

                if (locations is null || information is null)
                    return;

                var segments = itemRefs.Select(
                        i =>
                            new
                            {
                                Info = information.Boxes.OfType<ItemInfoEntryBox>().First(j => j.ItemId == i.FromItemId),
                                Location = locations.ItemLocations.First(j => j.ItemId == i.FromItemId)
                            })
                    .OrderBy(i => i.Location.BaseOffset)
                    .ThenBy(i => i.Location.ExtentList.FirstOrDefault()?.ExtentOffset);

                foreach (var segment in segments)
                {
                    // ItemLocationBox is a complex structure that can load data in multiple extents in
                    // different locations in different files. It appears the HEICs made by Apple only use
                    // the simplest of references. Thus we check that the simplifying assumption is true
                    // and then proceed with the simple case.
                    if (segment.Location.ConstructionMethod == ConstructionMethod.FileOffset && segment.Location.ExtentList.Length == 1)
                    {
                        ParseSingleSegment(
                            segment.Info.ItemType,
                            segment.Location.BaseOffset + segment.Location.ExtentList[0].ExtentOffset,
                            segment.Location.ExtentList[0].ExtentLength);
                    }
                }

                return;

                void ParseSingleSegment(uint itemType, ulong extentOffset, ulong extentLength)
                {
                    switch (itemType)
                    {
                        case Hvc1Tag:
                            ParseThumbnail();
                            break;
                        case ExifTag:
                            ParseExif();
                            break;
                        case MimeTag:
                            ParseXmp();
                            break;
                    }

                    return;

                    void ParseThumbnail()
                    {
                        var dir = new HeicThumbnailDirectory();
                        dir.Set(HeicThumbnailDirectory.TagFileOffset, extentOffset);
                        dir.Set(HeicThumbnailDirectory.TagLength, extentLength);
                        directories.Add(dir);
                    }

                    void ParseExif()
                    {
                        if ((long)extentLength + (long)extentOffset - reader.Position > reader.Available())
                            return;

                        reader.Skip((long)extentOffset - reader.Position);
                        var exifBytes = GetExifBytes();

                        var parser = new ExifReader();
                        var dirs = parser.Extract(new ByteArrayReader(exifBytes), exifStartOffset: 0);
                        directories.AddRange(dirs);

                        byte[] GetExifBytes()
                        {
                            var headerLength = reader.GetUInt32();
                            if (headerLength >> 16 == 0x4d4d && extentLength >= 4) // "MM"
                            {
                                var ret = new byte[extentLength];
                                ret[0] = (byte)(headerLength >> 24 & 0xFF);
                                ret[1] = (byte)(headerLength >> 16 & 0xFF);
                                ret[2] = (byte)(headerLength >> 8 & 0xFF);
                                ret[3] = (byte)(headerLength & 0xFF);
                                reader.GetBytes(ret, 4, (int)extentLength - 4);
                                return ret;
                            }

                            reader.Skip((int)headerLength);
                            return reader.GetBytes((int)extentLength - 4 - (int)headerLength);
                        }
                    }

                    void ParseXmp()
                    {
                        if ((long)extentLength + (long)extentOffset - reader.Position > reader.Available())
                            return;

                        reader.Skip((long)extentOffset - reader.Position);
                        var bytes = reader.GetBytes((int)extentLength);
                        var xmpDir = new XmpReader().Extract(bytes);
                        directories.Add(xmpDir);
                    }
                }
            }
        }
    }
}
