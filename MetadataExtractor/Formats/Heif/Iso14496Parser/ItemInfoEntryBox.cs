using System;
using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemInfoEntryBox : FullBox
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
        ulong TransferLength { get; }
        public byte GroupIdCount { get; }
        public uint[] GroupIds { get; } = new uint[0];
        public uint ItemType { get; }
        public string ItemTypeString => TypeStringConverter.ToTypeString(ItemType);
        public string ItemUri { get; } = "";

        private const uint FdelTag = 0x6664656C; // fdel
        private const uint MimeTag = 0x6D696D65; // fdel
        private const uint UriTag = 0x75726920; // fdel

        public ItemInfoEntryBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            if (Version <= 1)
            {
                ItemId = sr.GetUInt16();
                ItemProtectionIndex = sr.GetUInt16();
                ItemName = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
            }
            if (Version == 1)
            {
                ExtensionType = sr.GetUInt32();
                if (ExtensionType == FdelTag)
                {
                    Location = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
                    MD5 = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
                    ContentLength = sr.GetUInt64();
                    TransferLength = sr.GetUInt64();
                    GroupIdCount = sr.GetByte();
                    GroupIds = new uint[GroupIdCount];
                    for (int i = 0; i < GroupIdCount; i++)
                    {
                        GroupIds[i] = sr.GetUInt32();
                    }
                }
            }
            if (Version >= 2)
            {
                ItemId = Version == 2 ? sr.GetUInt16() : sr.GetUInt32();
                ItemProtectionIndex = sr.GetUInt16();
                ItemType = sr.GetUInt32();
                ItemName = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);

                if (ItemType == MimeTag)
                {
                    ContentType = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
                    ContentEncoding = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
                }
                if (ItemType == UriTag)
                {
                    ItemUri = sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
                }
            }
        }
    }
}
