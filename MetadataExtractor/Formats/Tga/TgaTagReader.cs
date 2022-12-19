// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga
{
    internal readonly struct TgaTagInfo
    {
        public short Id { get; }
        public int Offset { get; }
        public int Size { get; }

        public TgaTagInfo(short id, int offset, int size)
        {
            Id = id;
            Offset = offset;
            Size = size;
        }
    }

    internal sealed class TgaTagReader : TgaReader<TgaTagInfo[]>
    {
        protected override TgaTagInfo[] Extract(Stream stream, int _)
        {
            var reader = new SequentialStreamReader(stream, isMotorolaByteOrder: false);
            var count = reader.GetUInt16();
            var tags = new TgaTagInfo[count];
            for (int i = 0; i < count; i++)
                tags[i] = GetTag(reader);
            return tags;

            static TgaTagInfo GetTag(SequentialReader reader)
            {
                return new(reader.GetInt16(), reader.GetInt32(), reader.GetInt32());
            }
        }
    }
}
