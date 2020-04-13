using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Heif.Iso14496Parser;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.IO;
#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Heif
{
    public class HeifMetadataReader
    {
        private const int Hvc1Tag = 0x68766331; // hvc1
        private const int ExifTag = 0x45786966; // Exif

        public static DirectoryList ReadMetadata(Stream stream) => new HeifMetadataReader(stream).Process();

        private List<Box> _sourceBoxes = new List<Box>();
        private readonly Stream _stream;
        List<Directory> _directories = new List<Directory>();

        private HeifMetadataReader(Stream stream)
        {
            _stream = stream;
        }

        private DirectoryList Process()
        {
            var reader = new SequentialStreamReader(_stream);
            ReadBoxes(reader);
            ParseQuickTimeTest();
            uint primaryItem = _sourceBoxes.Descendant<PrimaryItemBox>()?.PrimaryItem ?? uint.MaxValue;
            var itemRefs = (_sourceBoxes.Descendant<ItemReferenceBox>()?.Boxes ?? new SingleItemTypeReferenceBox[0])
                .Where(i => i.Type == BoxTypes.ThmbTag || i.Type == BoxTypes.CdscTag).ToList();
            ParseImageProperties(primaryItem, itemRefs);

            ParseItemSegments(itemRefs, reader);
            return _directories;
        }

        private void ParseItemSegments(List<SingleItemTypeReferenceBox> itemRefs, SequentialStreamReader reader)
        {
            var locations = _sourceBoxes.Descendant<ItemLocationBox>();
            var information = _sourceBoxes.Descendant<ItemInformationBox>();

            var segments = itemRefs.Select(
                    i =>
                        new
                        {
                            Info = information.Boxes.OfType<ItemInfoEntryBox>().First(j => j.ItemId == i.FromItemId),
                            Location = locations.ItemLocations.First(j => j.ItemId == i.FromItemId)
                        })
                .OrderBy(i => i.Location.BaseOffset);
            foreach (var segment in segments)
            {
                // Itemlocation is a complex structure that can load data in multiple extents in
                // different locations in different files.  It appears the Heics made by apple only use
                // the simpleist of references.  Thus we check that the simplifying assumption is true
                // and then proceed with the simple case.
                Debug.Assert(segment.Location.ConstructionMethod == ConstructionMethod.FileOffset);
                Debug.Assert(segment.Location.ExtentList.Length == 1);
                ParseSingleSegment(
                    segment.Info.ItemType,
                    segment.Location.BaseOffset + segment.Location.ExtentList[0].ExtentOffset,
                    segment.Location.ExtentList[0].ExtentLength, reader);
            }
        }

        private void ParseSingleSegment(
            uint itemType,
            ulong extentOffset,
            ulong extentLength,
            SequentialStreamReader reader)
        {
            switch (itemType)
            {
                case Hvc1Tag:
                    ParseThumbnail(extentOffset, extentLength);
                    break;
                case ExifTag:
                    ParseExif(extentOffset, extentLength, reader);
                    break;
            }
        }

        private void ParseExif(ulong extentOffset, ulong extentLength, SequentialStreamReader reader)
        {
            if ((long)extentLength + (long)extentOffset - reader.Position > reader.Available()) return;
            reader.Skip((long)extentOffset - reader.Position);
            var exifBytes = GetExifBytes(extentLength, reader);

            var parser = new ExifReader();
            var dirs = parser.Extract(new ByteArrayReader(exifBytes));
            _directories.AddRange(dirs);
        }

        private static byte[] GetExifBytes(ulong extentLength, SequentialStreamReader reader)
        {
            var headerLength = reader.GetUInt32();
            if (headerLength >> 16 == 0x4d4d)
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
            var exifBytes = reader.GetBytes((int)extentLength);
            return exifBytes;
        }

        private void ParseThumbnail(ulong extentOffset, ulong extentLength)
        {
            var dir = new HeicThumbnailDirectory();
            dir.Set(HeicThumbnailDirectory.FileOffset, extentOffset);
            dir.Set(HeicThumbnailDirectory.Length, extentLength);
            _directories.Add(dir);
        }

        private void ParseImageProperties(uint primaryItem, List<SingleItemTypeReferenceBox> itemRefs)
        {
            uint[] allPrimaryTiles = (_sourceBoxes.Descendant<ItemReferenceBox>()?.Boxes ??
                                      new SingleItemTypeReferenceBox[0])
                .SelectMany(i => i.FromItemId == primaryItem && i.Type == BoxTypes.DimgTag ? i.ToItemIds : new uint[0])
                .ToArray();
            var itemPropertyBox = _sourceBoxes.Descendant<ItemPropertyBox>();
            if (itemPropertyBox == null) return;
            var props = itemPropertyBox.Boxes.Descendant<ItemPropertyContainerBox>().Boxes;
            var associations = itemPropertyBox.Boxes.Descendant<ItemPropertyAssociationBox>();

            ParsePropertyBoxes(
                "HEIC Primary Item Properties", ImageProperties(
                    primaryItem, allPrimaryTiles,
                    associations, props));
            foreach (var itemRef in itemRefs)
            {
                ParsePropertyBoxes(
                    "HEIC Thumbnail Properties", ImageProperties(
                        itemRef.FromItemId, new uint[0],
                        associations, props));
            }
        }

        private IEnumerable<Box> ImageProperties(
            uint primaryId,
            uint[] secondary,
            ItemPropertyAssociationBox associations,
            IList<Box> props)
        {
            return DirectProperties(primaryId, associations, props).Concat(
                (from associationBox in associations.Entries.Where(i => secondary.Contains(i.ItemId))
                    from propIndex in associationBox.Properties
                    select props.ElementAt(propIndex.Index - 1))
                .Where(i => i is DecoderConfigurationBox || i is ColorInformationBox)
                .Distinct()
            );
        }

        private static IEnumerable<Box> DirectProperties(uint primaryId, ItemPropertyAssociationBox associations, IList<Box> props)
        {
            return from associationBox in associations.Entries.Where(i => i.ItemId == primaryId)
                from propIndex in associationBox.Properties
                select props.ElementAt(propIndex.Index - 1);
        }

        private void ParsePropertyBoxes(string propertyBoxTitle, IEnumerable<Box> props)
        {
            var dir = new HeicImagePropertiesDirectory(propertyBoxTitle);
            bool hasProp = false;
            _directories.Add(dir); // add now so it will preceed the ICC profile directory
            foreach (var prop in props)
            {
                switch (prop)
                {
                    case ImageSpatialExtentsBox ipse:
                        ParseImageSize(dir, ipse);
                        break;
                    case ImageRotationBox irot:
                        ParseImageRotation(dir, irot);
                        break;
                    case PixelInformationBox pixi:
                        ParsePixelDepth(dir, pixi);
                        break;
                    case DecoderConfigurationBox hvcC:
                        ParseDecoderInformation(dir, hvcC);
                        break;
                    case ColorInformationBox colr:
                        ParseColorBox(dir, colr);
                        break;
                    default: continue;
                }

                hasProp = true;
            }

            if (!hasProp)
            {
                _directories.Remove(dir);
            }
        }

        private void ParseColorBox(HeicImagePropertiesDirectory dir, ColorInformationBox colr)
        {
            dir.Set(HeicImagePropertiesDirectory.ColorFormat, colr.ColorType);
            if (colr.ColorType == ColorInformationBox.NclxTag)
            {
                dir.Set(HeicImagePropertiesDirectory.ColorPrimaries, colr.ColorPrimaries);
                dir.Set(HeicImagePropertiesDirectory.ColorTransferCharacteristics, colr.TransferCharacteristics);
                dir.Set(HeicImagePropertiesDirectory.ColorMatrixCharacteristics, colr.MatrixCharacteristics);
                dir.Set(HeicImagePropertiesDirectory.FullRangeColor, colr.FullRangeFlag);
            }
            else
            {
                var iccDirectory = new IccReader().Extract(new ByteArrayReader(colr.IccProfile));
                iccDirectory.Parent = dir;
                _directories.Add(iccDirectory);
            }
        }

        private void ParseDecoderInformation(HeicImagePropertiesDirectory dir, DecoderConfigurationBox hvcC)
        {
            dir.Set(HeicImagePropertiesDirectory.ConfigurationVersion, hvcC.ConfigurationVersion);
            dir.Set(HeicImagePropertiesDirectory.GeneralProfileSpace, hvcC.GeneralProfileSpace);
            dir.Set(HeicImagePropertiesDirectory.GeneralTierTag, hvcC.GeneralTierTag);
            dir.Set(HeicImagePropertiesDirectory.GeneralProfileIdc, hvcC.GeneralProfileIdc);
            dir.Set(HeicImagePropertiesDirectory.GeneralProfileCompatibilityTag, hvcC.GeneralProfileCompatibilityFlags);
            dir.Set(HeicImagePropertiesDirectory.GeneralLevelIdc, hvcC.GeneralLevelIdc);
            dir.Set(HeicImagePropertiesDirectory.MinSpacialSegmentationIdc, hvcC.MinSpacialSegmentationIdc);
            dir.Set(HeicImagePropertiesDirectory.ParallelismType, hvcC.ParallelismType);
            dir.Set(HeicImagePropertiesDirectory.ChromaFormat, hvcC.ChromaFormat);
            dir.Set(HeicImagePropertiesDirectory.BitDepthLuma, hvcC.BitDepthLumaMinus8 + 8);
            dir.Set(HeicImagePropertiesDirectory.BitDepthChroma, hvcC.BitDepthChromaMinus8 + 8);
            dir.Set(HeicImagePropertiesDirectory.AverageFrameRate, hvcC.AvgFrameRate);
            dir.Set(HeicImagePropertiesDirectory.ConstantFrameRate, hvcC.ConstantFrameRate);
            dir.Set(HeicImagePropertiesDirectory.NumTemporalLayers, hvcC.NumTemporalLayers);
            dir.Set(HeicImagePropertiesDirectory.LengthSize, hvcC.LengthSizeMinus1 + 1);
        }

        private void ParsePixelDepth(HeicImagePropertiesDirectory dir, PixelInformationBox pixi)
        {
            dir.Set(HeicImagePropertiesDirectory.PixelDepths, pixi.BitsPerChannel);
        }

        private void ParseImageRotation(HeicImagePropertiesDirectory dir, ImageRotationBox irot)
        {
            dir.Set(HeicImagePropertiesDirectory.Rotation, irot.Rotation);
        }

        private static void ParseImageSize(HeicImagePropertiesDirectory dir, ImageSpatialExtentsBox ipse)
        {
            dir.Set(HeicImagePropertiesDirectory.ImageWidth, ipse.X);
            dir.Set(HeicImagePropertiesDirectory.ImageHeight, ipse.Y);
        }

        private void ParseQuickTimeTest()
        {
            if (_sourceBoxes.Descendant<FileTypeBox>() is {} ftype)
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

                _directories.Add(dir);
            }
        }

        private void ReadBoxes(SequentialStreamReader reader)
        {
            Box? item = null;
            while ((item = BoxReader.ReadBox(reader)) != null)
            {
                _sourceBoxes.Add(item);
            }
        }
    }
}
