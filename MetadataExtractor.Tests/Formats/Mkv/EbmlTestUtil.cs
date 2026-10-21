// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

// All documents in this suite are generated here; no media fixtures or network access are needed.
internal static class EbmlTestUtil
{
    internal static class Id
    {
        public const int Header = 0x1A45DFA3, Version = 0x4286, ReadVersion = 0x42F7,
            MaxIdLength = 0x42F2, MaxSizeLength = 0x42F3, DocType = 0x4282,
            DocTypeVersion = 0x4287, DocTypeReadVersion = 0x4285;
        public const int Segment = 0x18538067, Info = 0x1549A966, MuxingApp = 0x4D80,
            TimestampScale = 0x2AD7B1, Duration = 0x4489, SegmentUuid = 0x73A4,
            Cluster = 0x1F43B675, Void = 0xEC;
        public const int Tracks = 0x1654AE6B, TrackEntry = 0xAE, TrackNumber = 0xD7,
            TrackUid = 0x73C5, TrackType = 0x83, Lacing = 0x9C, CodecId = 0x86,
            Language = 0x22B59C, LanguageBcp47 = 0x22B59D, DefaultDuration = 0x23E383,
            CodecDelay = 0x56AA, SeekPreRoll = 0x56BB;
        public const int Video = 0xE0, PixelWidth = 0xB0, PixelHeight = 0xBA,
            FlagInterlaced = 0x9A, Colour = 0x55B0, MatrixCoefficients = 0x55B1,
            TransferCharacteristics = 0x55BA, Primaries = 0x55BB, Range = 0x55B9,
            ChromaSitingHorz = 0x55B7, ChromaSitingVert = 0x55B8;
        public const int Audio = 0xE1, Channels = 0x9F, SamplingFrequency = 0xB5;
        public const int Tags = 0x1254C367, Tag = 0x7373, Targets = 0x63C0,
            SimpleTag = 0x67C8, TagName = 0x45A3, TagLanguage = 0x447A,
            TagString = 0x4487, TagLanguageBcp47 = 0x447B, TagDefault = 0x4484,
            TagBinary = 0x4485, TargetType = 0x63CA, TargetTypeValue = 0x68CA,
            TagTrackUid = 0x63C5, TagEditionUid = 0x63C9, TagChapterUid = 0x63C4,
            TagAttachmentUid = 0x63C6;
    }

    public static byte[] Concat(params byte[][] values) => values.SelectMany(value => value).ToArray();

    public static byte[] Bytes(ulong value)
    {
        int length = 1;
        for (ulong remaining = value; (remaining >>= 8) != 0;)
            length++;
        var bytes = new byte[length];
        for (int i = length - 1; i >= 0; i--, value >>= 8)
            bytes[i] = (byte)value;
        return bytes;
    }

    public static byte[] Size(ulong value, int width = 0)
    {
        if (width == 0)
        {
            width = 1;
            while (value >= (1UL << (7 * width)) - 1)
                width++;
        }
        var bytes = new byte[width];
        for (int i = width - 1; i >= 0; i--, value >>= 8)
            bytes[i] = (byte)value;
        bytes[0] |= (byte)(1 << (8 - width));
        return bytes;
    }

    public static byte[] Element(int id, params byte[][] children)
    {
        var payload = Concat(children);
        return Concat(Bytes((uint)id), Size((ulong)payload.Length), payload);
    }

    public static byte[] Unknown(int id, int width, params byte[][] children)
    {
        var size = Enumerable.Repeat((byte)0xFF, width).ToArray();
        size[0] = (byte)((1 << (9 - width)) - 1);
        return Concat(Bytes((uint)id), size, Concat(children));
    }

    public static byte[] Unsigned(int id, ulong value) => Element(id, Bytes(value));
    public static byte[] Text(int id, string value) => Element(id, Encoding.UTF8.GetBytes(value));

    public static byte[] Floating(int id, double value, bool singlePrecision = false)
    {
        var bytes = singlePrecision ? BitConverter.GetBytes((float)value) : BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return Element(id, bytes);
    }

    public static byte[] Header(string docType = "matroska") => Element(Id.Header,
        Unsigned(Id.Version, 1), Unsigned(Id.ReadVersion, 1),
        Unsigned(Id.MaxIdLength, 4), Unsigned(Id.MaxSizeLength, 8),
        Text(Id.DocType, docType), Unsigned(Id.DocTypeVersion, 4), Unsigned(Id.DocTypeReadVersion, 2));

    public static byte[] Document(params byte[][] children) => Concat(Header(), Element(Id.Segment, children));

    public static byte[] Video(ulong number) => Element(Id.TrackEntry,
        Unsigned(Id.TrackNumber, number), Unsigned(Id.TrackUid, number), Unsigned(Id.TrackType, 1),
        Text(Id.CodecId, "V_VP8"),
        Element(Id.Video, Unsigned(Id.PixelWidth, 32), Unsigned(Id.PixelHeight, 24)));

    public static byte[] Audio(ulong number) => Element(Id.TrackEntry,
        Unsigned(Id.TrackNumber, number), Unsigned(Id.TrackUid, number), Unsigned(Id.TrackType, 2),
        Text(Id.CodecId, "A_PCM/INT/LIT"),
        Element(Id.Audio, Floating(Id.SamplingFrequency, 8000), Unsigned(Id.Channels, 1)));

    public static byte[] Simple(string name, string value, params byte[][] children) =>
        Element(Id.SimpleTag, Text(Id.TagName, name), Text(Id.TagString, value), Concat(children));

    public static IReadOnlyList<Directory> Extract(byte[] bytes) =>
        new MkvReader().Extract(new SequentialByteArrayReader(bytes));

    public static void NoErrors(IEnumerable<Directory> directories) =>
        Assert.Empty(directories.SelectMany(directory => directory.Errors));

    public static ulong UInt(Directory directory, int tag) => Assert.IsType<ulong>(directory.GetObject(tag));

    internal sealed class NonSeekableStream(byte[] bytes, int chunkSize = 2) : Stream
    {
        private int _position;
        public int BytesRead => _position;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int length = Math.Min(Math.Min(count, chunkSize), bytes.Length - _position);
            Array.Copy(bytes, _position, buffer, offset, length);
            _position += length;
            return length;
        }
        public override int ReadByte() => _position == bytes.Length ? -1 : bytes[_position++];
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
