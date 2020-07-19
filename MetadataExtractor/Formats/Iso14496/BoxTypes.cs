// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496
{
    internal static class BoxTypes
    {
        public const uint MdatTag = 0x6D646174; // mdat
        public const uint MetaTag = 0x6D657461; // meta
        public const uint FTypTag = 0x66747970; // ftyp
        public const uint HdlrTag = 0x68646C72; // hdlr
        public const uint DinfTag = 0x64696E66; // dinf
        public const uint DrefTag = 0x64726566; // dref
        public const uint UrlTag = 0x75726C20; // 'url '
        public const uint UrnTag = 0x75726E20; // 'urn '
        public const uint PitmTag = 0x7069746D; // pitm
        public const uint Iinftag = 0x69696E66; // iinf
        public const uint InfeTag = 0x696E6665; // infe
        public const uint IrefTag = 0x69726566; // iref
        public const uint IprpTag = 0x69707270; // iprp
        public const uint IpcoTag = 0x6970636F; // ipco
        public const uint IspeTag = 0x69737065; // ipse
        public const uint HvcCTag = 0x68766343; // hvcC
        public const uint ColrTag = 0x636F6C72; // colr
        public const uint IrotTag = 0x69726f74; // irot
        public const uint PixiTag = 0x70697869; // pixi
        public const uint IdatTag = 0x69646174; // idat
        public const uint IlocTag = 0x696C6F63; // iloc
        public const uint IpmaTag = 0x69706D61; // ipma
        public const uint ThmbTag = 0x74686D62; // thmb
        public const uint CdscTag = 0x63647363; // cdsc
        public const uint DimgTag = 0x64696D67; // dimg
        public const uint MimeTag = 0x6D696D65; // mime
    }
}
