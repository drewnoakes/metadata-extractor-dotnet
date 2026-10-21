// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Mkv;

namespace MetadataExtractor.Formats.Ebml;

using FileType = Util.FileType;

/// <summary>Probes at most 64 KiB of an EBML header for its DocType, leaving unresolved schemas as generic EBML.</summary>
internal sealed class EbmlTypeChecker : IStreamTypeChecker
{
    private const int HeaderProbeLimit = 65536;

    public int ByteCount => 4;

    public FileType CheckType(byte[] bytes)
        => bytes.Length >= 4 && bytes[0] == 0x1A && bytes[1] == 0x45 && bytes[2] == 0xDF && bytes[3] == 0xA3
            ? FileType.Ebml : FileType.Unknown;

    public FileType CheckType(byte[] bytes, Stream stream)
    {
        if (CheckType(bytes) == FileType.Unknown)
            return FileType.Unknown;

        long position = stream.Position;
        try
        {
            var directories = new MkvReader().ExtractHeader(new ProbeReader(stream));
            return directories.OfType<EbmlDirectory>().FirstOrDefault()?.GetString(EbmlDirectory.TagDocType) switch
            {
                "webm" => FileType.WebM,
                "matroska" => FileType.Mkv,
                _ => FileType.Ebml
            };
        }
        finally
        {
            stream.Position = position;
        }
    }

    private sealed class ProbeReader(Stream stream) : SequentialStreamReader(stream)
    {
        private readonly long _start = stream.Position;

        public override long Position => base.Position - _start;

        private void CheckCount(long count)
        {
            if (count > HeaderProbeLimit - Position)
                throw new EndOfStreamException("EBML header exceeds the file type probe limit.");
        }

        public override byte GetByte()
        {
            CheckCount(1);
            return base.GetByte();
        }

        public override byte[] GetBytes(int count)
        {
            CheckCount(count);
            return base.GetBytes(count);
        }

        public override void GetBytes(byte[] buffer, int offset, int count)
        {
            CheckCount(count);
            base.GetBytes(buffer, offset, count);
        }

        public override void GetBytes(Span<byte> bytes)
        {
            CheckCount(bytes.Length);
            base.GetBytes(bytes);
        }

        public override bool TrySkip(long n)
        {
            long allowed = Math.Min(n, HeaderProbeLimit - Position);
            bool skipped = base.TrySkip(allowed);
            return allowed == n && skipped;
        }
    }
}
