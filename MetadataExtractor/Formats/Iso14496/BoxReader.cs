// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Iso14496.Boxes;

namespace MetadataExtractor.Formats.Iso14496
{
    internal static class BoxReader
    {
        public static List<Box> ReadBoxes(SequentialReader reader)
        {
            var boxes = new List<Box>();

            Box? item;
            while ((item = ReadBox(reader)) != null)
            {
                boxes.Add(item);
            }

            return boxes;
        }

        public static List<Box> ReadBoxes(SequentialReader sr, BoxLocation loc)
        {
            var ret = new List<Box>();
            while (sr.IsWithinBox(loc))
            {
                var box = ReadBox(sr);
                if (box != null)
                    ret.Add(box);
            }

            return ret;
        }

        private static Box? ReadBox(SequentialReader sr)
        {
            return ReadBox(sr, ParseBox);

            static Box ParseBox(BoxLocation location, SequentialReader sr)
            {
                return location.Type switch
                {
                    BoxTypes.FTypTag => new FileTypeBox(location, sr),
                    BoxTypes.MetaTag => new MetaBox(location, sr),
                    BoxTypes.HdlrTag => new HandlerBox(location, sr),
                    BoxTypes.DinfTag => new DataInformationBox(location, sr),
                    BoxTypes.DrefTag => new DataReferenceBox(location, sr),
                    BoxTypes.UrlTag => new DataEntryLocationBox(location, sr, hasName: false),
                    BoxTypes.UrnTag => new DataEntryLocationBox(location, sr, hasName: true),
                    BoxTypes.PitmTag => new PrimaryItemBox(location, sr),
                    BoxTypes.Iinftag => new ItemInformationBox(location, sr),
                    BoxTypes.InfeTag => new ItemInfoEntryBox(location, sr),
                    BoxTypes.IrefTag => new ItemReferenceBox(location, sr),
                    BoxTypes.IprpTag => new ItemPropertyBox(location, sr),
                    BoxTypes.IpcoTag => new ItemPropertyContainerBox(location, sr),
                    BoxTypes.IspeTag => new ImageSpatialExtentsBox(location, sr),
                    BoxTypes.HvcCTag => new DecoderConfigurationBox(location, sr),
                    BoxTypes.ColrTag => new ColorInformationBox(location, sr),
                    BoxTypes.IrotTag => new ImageRotationBox(location, sr),
                    BoxTypes.PixiTag => new PixelInformationBox(location, sr),
                    BoxTypes.IdatTag => new ItemDataBox(location, sr),
                    BoxTypes.IlocTag => new ItemLocationBox(location, sr),
                    BoxTypes.IpmaTag => new ItemPropertyAssociationBox(location, sr),
                    _ => new Box(location)
                };
            }
        }

        internal static Box? ReadBox(SequentialReader sr, Func<BoxLocation, SequentialReader, Box> parseBox)
        {
            if (sr.Available() < 8)
                return null;

            var location = new BoxLocation(sr);

            if ((long)location.NextPosition - sr.Position >= sr.Available())
            {
                // skip the last box of the file.
                return null;
            }

            var box = parseBox(location, sr);

            var unreadBytes = (long)box.NextPosition - sr.Position;

            if (unreadBytes < 0)
            {
                throw new ImageProcessingException($"Reader for box with type {location.TypeString} read beyond end of allocated space.");
            }

            if (unreadBytes > 0)
            {
                sr.Skip(unreadBytes);
            }

            return box;
        }
    }
}
