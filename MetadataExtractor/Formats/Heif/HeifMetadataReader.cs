using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var itemPropertyBox = _sourceBoxes.Descendant<ItemPropertyBox>();
            var props = itemPropertyBox.Boxes.Descendant<ItemPropertyContainerBox>().Boxes;
            foreach (var pbox in InterestingPropertyBoxes(itemPropertyBox, primaryItem, itemRefs))
            {
                var dir = new HeicImagePropertiesDirectory(pbox.ItemId == primaryItem?"HEIC Primary Item Properties":
                    "HEIC Thumbnal Properties");
                foreach (var prop in pbox.Properties.Select(i=>props.ElementAt(i.Index-1)))
                {
                    switch (prop)
                    {
                        case ImageSpatialExtentsBox ipse:
                            dir.Set(HeicImagePropertiesDirectory.ImageWidth, ipse.X);
                            dir.Set(HeicImagePropertiesDirectory.ImageHeight, ipse.Y);
                            break;
                    }
                }
                _directories.Add(dir);
            }
            /*
            var propGroups = InterestingPropertyBoxes(itemPropertyBox, primaryItem, itemRefs)
                .Select(i=>new { i.ItemId, Props = i.Properties.Select(j=>props.ElementAt(j.Index-1)).ToList()})
                .ToList();
                */

            return _directories;
        }

        private static IEnumerable<ItemPropertyAssociationEntry> InterestingPropertyBoxes(
            ItemPropertyBox itemPropertyBox, uint primaryItem, List<SingleItemTypeReferenceBox> itemRefs)
        {
            return itemPropertyBox.Boxes.Descendant<ItemPropertyAssociationBox>().Entries
                .Where(i => i.ItemId == primaryItem || itemRefs.Any(j => j.FromItemId == i.ItemId));
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
