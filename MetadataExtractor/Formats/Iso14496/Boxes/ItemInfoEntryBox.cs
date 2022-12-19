// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal sealed class ItemInfoEntryBox : FullBox
    {
        public uint ItemId { get; }
        public ushort ItemProtectionIndex { get; }
        public string ItemName { get; } = "";
        public string ContentType { get; } = "";

        public string ContentEncoding { get; } = "";

        // version 1
        public uint ExtensionType { get; }
        public string Location { get; } = "";
        public string MD5 { get; } = "";
        public ulong ContentLength { get; }
        public ulong TransferLength { get; }
        public byte GroupIdCount { get; }
        public uint[] GroupIds { get; } = new uint[0];
        public uint ItemType { get; }
        public string ItemUri { get; } = "";

        private const uint FdelTag = 0x6664656C; // fdel
        private const uint MimeTag = 0x6D696D65; // fdel
        private const uint UriTag = 0x75726920; // fdel

        public ItemInfoEntryBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            if (Version <= 1)
            {
                ItemId = reader.GetUInt16();
                ItemProtectionIndex = reader.GetUInt16();
                ItemName = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
            }

            if (Version == 1)
            {
                ExtensionType = reader.GetUInt32();
                if (ExtensionType == FdelTag)
                {
                    Location = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
                    MD5 = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
                    ContentLength = reader.GetUInt64();
                    TransferLength = reader.GetUInt64();
                    GroupIdCount = reader.GetByte();
                    GroupIds = new uint[GroupIdCount];
                    for (int i = 0; i < GroupIdCount; i++)
                    {
                        GroupIds[i] = reader.GetUInt32();
                    }
                }
            }

            if (Version >= 2)
            {
                ItemId = Version == 2 ? reader.GetUInt16() : reader.GetUInt32();
                ItemProtectionIndex = reader.GetUInt16();
                ItemType = reader.GetUInt32();
                ItemName = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);

                if (ItemType == MimeTag)
                {
                    ContentType = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
                    ContentEncoding = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
                }

                if (ItemType == UriTag)
                {
                    ItemUri = reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
                }
            }
        }
    }
}
