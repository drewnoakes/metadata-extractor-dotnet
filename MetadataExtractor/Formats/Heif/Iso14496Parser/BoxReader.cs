using System;
using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public static class BoxReader
    {
        public static Box ReadBox(SequentialReader sr) => ReadBox(sr, LoadFinalType);

        public static Box ReadBox(SequentialReader sr, Func<BoxLocation, SequentialReader, Box> reader)
        {
            var location = new BoxLocation(sr);
            var ret = reader(location, sr);
            ret.SkipRemainingData(sr);
            return ret;

        }

        private const uint MetaTag = 0x6D657461; // meta
        private const uint FTypTag = 0x66747970; // ftyp
        private const uint HdlrTag = 0x68646C72; // hdlr
        private const uint DinfTag = 0x64696E66; // dinf
        private const uint DrefTag = 0x64726566; // dref
        private const uint UrlTag  = 0x75726C20; // 'url '
        private const uint UrnTag  = 0x75726E20; // 'urn '
        private const uint PitmTag = 0x7069746D; // pitm
        private const uint Iinftag = 0x69696E66; // iinf
        private const uint InfeTag = 0x696E6665; // infe
        private const uint IrefTag = 0x69726566; // iref
        private const uint IprpTag = 0x69707270; // iprp
        private const uint IpcoTag = 0x6970636F; // ipco
        private const uint IspeTag = 0x69737065; // ipse
        private const uint HvcCTag = 0x68766343; // hvcC
        private const uint ColrTag = 0x636F6C72; // colr
        private const uint IrotTag = 0x69726f74; // irot
        private const uint PixiTag = 0x70697869; // pixi
        private const uint IdatTag = 0x69646174; // idat
        private const uint IlocTag = 0x696C6F63; // iloc
        private static Box LoadFinalType(BoxLocation location, SequentialReader sr)
        {
            return location.Type switch
            {
                FTypTag => new FileTypeBox(location, sr),
                MetaTag => new MetaBox(location, sr),
                HdlrTag => new HandlerBox(location, sr),
                DinfTag => new DataInformationBox(location, sr),
                DrefTag => new DataReferenceBox(location, sr),
                UrlTag  => new DataEntryLocationBox(location, sr, false),
                UrnTag  => new DataEntryLocationBox(location, sr, true),
                PitmTag => new PrimaryItemBox(location, sr),
                Iinftag => new ItemInformationBox(location, sr),
                InfeTag => new ItemInfoEntryBox(location, sr),
                IrefTag => new ItemReferenceBox(location, sr),
                IprpTag => new ItemPropertyBox(location, sr),
                IpcoTag => new ItemPropertyContainerBox(location, sr),
                IspeTag => new ImageSpatialExtentsBox(location, sr),
                HvcCTag => new DecoderConfigurationBox(location, sr),
                ColrTag => new ColorInformationBox(location, sr),
                IrotTag => new ImageRotationBox(location, sr),
                PixiTag => new PixelInformationBox(location, sr),
                IdatTag => new ItemDataBox(location, sr),
                IlocTag => new ItemLocationBox(location, sr),
                _=> new Box(location)
            };
        }

        public static IList<Box> BoxList(BoxLocation loc, SequentialReader sr) {
            var ret = new List<Box>();
            while (!loc.DoneReading(sr)){
                ret.Add(ReadBox(sr));
            }
            return ret;
        }
    }
}
