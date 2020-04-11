using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Heif.Iso14496Parser;
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
        public static DirectoryList ReadMetadata(Stream stream) => new HeifMetadataReader(stream).Process();
        private List<Box> _sourceBoxes = new List<Box>();

        private readonly Stream _stream;
        private HeifMetadataReader(Stream stream)
        {
            _stream = stream;
        }

        List<Directory> _directories = new List<Directory>();

        private DirectoryList Process()
        {
            var reader = new SequentialStreamReader(_stream);
            ReadBoxes(reader);
            ParseQuickTimeTest();
            uint primaryItem = _sourceBoxes.Descendant<PrimaryItemBox>()?.PrimaryItem ?? uint.MaxValue;
            var itemRefs = _sourceBoxes.Descendant<ItemReferenceBox>().Boxes
                .Where(i=>i.Type == BoxTypes.ThmbTag || i.Type == BoxTypes.CdscTag).ToList();
            uint[] allPrimaryTiles = _sourceBoxes.Descendant<ItemReferenceBox>().Boxes
                .SelectMany(i=>i.FromItemId == primaryItem && i.Type == BoxTypes.DimgTag?
                  i.ToItemIds:new uint[0]).ToArray();

            var itemPropertyBox = _sourceBoxes.Descendant<ItemPropertyBox>();
            var props = itemPropertyBox.Boxes.Descendant<ItemPropertyContainerBox>().Boxes;
            var associations = itemPropertyBox.Boxes.Descendant<ItemPropertyAssociationBox>();

            ParsePropertyBoxes("HEIC Primary Item Properties", ImageProperties(primaryItem, allPrimaryTiles,
                associations, props));
            foreach (var itemRef in itemRefs)
            {
                ParsePropertyBoxes("HEIC Thumbnail Properties", ImageProperties(itemRef.FromItemId, new uint[0],
                    associations, props));
            }
            return _directories;
        }

        private IEnumerable<Box> ImageProperties(uint primaryId, uint[] secondary,
            ItemPropertyAssociationBox associations, IList<Box> props)
        {
            return DirectProperties(primaryId, associations, props). Concat(
                (from associationBox in associations.Entries.Where(i=>secondary.Contains(i.ItemId))
                from propIndex in associationBox.Properties
                select props.ElementAt(propIndex.Index - 1))
                .Where(i=>i is DecoderConfigurationBox || i is ColorInformationBox)
                .Distinct()
                );
        }

        private static IEnumerable<Box> DirectProperties(uint primaryId, ItemPropertyAssociationBox associations, IList<Box> props)
        {
            return from associationBox in associations.Entries.Where(i=>i.ItemId == primaryId)
                from propIndex in associationBox.Properties
                select props.ElementAt(propIndex.Index - 1);
        }

        private void ParsePropertyBoxes(string propertyBoxTitle, IEnumerable<Box> props)
        {
            var dir = new HeicImagePropertiesDirectory(propertyBoxTitle);
            bool hasProp = false;
            foreach (var prop in props)
            {
                switch (prop)
                {
                    case ImageSpatialExtentsBox ipse: ParseImageSize(dir, ipse); break;
                    case ImageRotationBox irot: ParseImageRotation(dir, irot); break;
                    case PixelInformationBox pixi : ParsePixelDepth(dir, pixi); break;
                    case DecoderConfigurationBox hvcC : ParseDecoderInformation(dir, hvcC); break;
                    case ColorInformationBox colr: ParseColorBox(dir, colr); break;
                    default: continue;
                }

                hasProp = true;
            }

            if (hasProp)
            {
                _directories.Add(dir);
            }
        }

        private void ParseColorBox(HeicImagePropertiesDirectory dir, ColorInformationBox colr)
        {
            dir.Set(HeicImagePropertiesDirectory.ColorFormat, colr.ColorType);
            if (colr.ColorType == ColorInformationBox.NclxTag)
            {
                dir.Set(HeicImagePropertiesDirectory.ColorPrimaries, colr.ColorPrimaries);
                dir.Set(HeicImagePropertiesDirectory.ColorTransferCharacteristics, colr.TransferCharacteristics);
                dir.Set(HeicImagePropertiesDirectory.ColorMatrixCharacteristicis, colr.MatrixCharacteristics);
                dir.Set(HeicImagePropertiesDirectory.FullRangeColor, colr.FullRangeFlag);
            }
            else
            {
                // parse ICC
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
            dir.Set(HeicImagePropertiesDirectory.BitDepthLuma, hvcC.BitDepthLumaMinus8+8);
            dir.Set(HeicImagePropertiesDirectory.BitDepthChroma, hvcC.BitDepthChromaMinus8+8);
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
                    dir.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands,
                        String.Join(", ", ftype.CompatibleBrandStrings.ToArray()));
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
