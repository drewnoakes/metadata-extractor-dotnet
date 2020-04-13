// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public static class BoxReader
    {
        public static Box? ReadBox(SequentialReader sr) => ReadBox(sr, LoadFinalType);

        public static Box? ReadBox(SequentialReader sr, Func<BoxLocation, SequentialReader, Box> reader)
        {
            if (sr.Available() < 8) return null;
            var location = new BoxLocation(sr);
            if ((long)location.NextPosition - sr.Position >= sr.Available()) return null; // skip the last box of the file.
            // if (location.Type == BoxTypes.MdatTag) return null;
            var ret = reader(location, sr);
            ret.SkipRemainingData(sr);
            return ret;
        }

        private static Box LoadFinalType(BoxLocation location, SequentialReader sr)
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

        public static IList<Box> BoxList(BoxLocation loc, SequentialReader sr)
        {
            var ret = new List<Box>();
            while (!loc.DoneReading(sr))
            {
                ret.Add(ReadBox(sr));
            }

            return ret;
        }
    }
}
